using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using static System.Collections.Specialized.BitVector32;
using LeapInternal;
using Leap.Unity.Query;
using System.Linq;

namespace PPirate.VoxReactor
{
    internal class EmotionManager
    {
        private Logger logger2;


        private int levelHorny = 0;
        private readonly int incrementHorny = 10;
        private readonly int incrementHappy = 10;
        private readonly int incrementSadness = 10;
        private readonly int incrementAnger = 10;
        private readonly int incremenEmbarrassment = 10;





        List<Emotion> mainEmotions = new List<Emotion>(); // only context on the strongest will be provided
        List<Emotion> concurrentEmotions = new List<Emotion>(); // does not need to be the strongest
        public List<Emotion> allEmotions = new List<Emotion>(); // does not need to be the strongest

        Hornieness hornieness;
        Happyness happyness;
        Sadness sadness;
        Anger anger;
        Embarrassment embarrasement;




        private JSONStorableAction OnHornyIncrease;
        private JSONStorableAction OnHornyDecrease;
        private readonly String VOX_ACTION_AROUSAL_INCREASE = "arousal_increase_char";
        private readonly String VOX_ACTION_AROUSAL_DECREASE = "arousal_decrease_char";
        private readonly String VOX_ACTION_HAPPY_INCREASE = "happy_increase_char";
        private readonly String VOX_ACTION_SADNESS_INCREASE = "sad_increase_char";
        private readonly String VOX_ACTION_ANGER_INCREASE = "anger_increase_char";
        private readonly String VOX_ACTION_EMBARRASSMENT_INCREASE = "embarrassment_increase_char";



        private readonly String VOX_ACTION_EMOTE_HAPPY = "emote_happy";
        private readonly String VOX_ACTION_EMOTE_BLUSH = "emote_embarrased";
        private readonly String VOX_ACTION_EMOTE_HORNY = "emote_horny";

        public readonly ObserverRegistry emotionObserverRegistry;

        public static String REGISTRY_ON_HORNY_CHANGED = "hornyChanged";
        public static String REGISTRY_ON_HAPPY_CHANGED = "happyChanged";





        public VoxtaCharacter character;
        FaceTimelinePlugin faceTimeline;


        private readonly BlushManager blushManager;
        public EmotionManager(VoxtaCharacter character) {
            logger2 = new Logger("VoxtaCharacter:Char#" + character.characterNumber, 0);
            logger2.Constructor();
            this.character = character;

            hornieness = new Hornieness(this);
            concurrentEmotions.Add(hornieness);

            happyness = new Happyness(this);
            mainEmotions.Add(happyness);

            sadness = new Sadness(this);
            mainEmotions.Add(sadness);

            anger  = new Anger(this);
            mainEmotions.Add(anger);

            embarrasement = new Embarrassment(this);
            mainEmotions.Add(embarrasement);

            allEmotions.AddRange(mainEmotions);
            allEmotions.AddRange(concurrentEmotions);






            this.emotionObserverRegistry = new ObserverRegistry();

            ///horny

            OnHornyIncrease = new JSONStorableAction("OnHornyIncrease", OnHornyIncreaseCallback);//todo these vam actions should be regiserted on a service and demuxed to here // do I even need this?>
            Main.singleton.RegisterAction(OnHornyIncrease);
            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_AROUSAL_INCREASE + character.characterNumber, OnHornyIncreaseCallback);
      


            OnHornyDecrease = new JSONStorableAction("OnHornyDecrease", OnHornyDecreaseCallback);
            Main.singleton.RegisterAction(OnHornyDecrease);
            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_AROUSAL_DECREASE + character.characterNumber, OnHornyDecreaseCallback);

            //happy 
            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_HAPPY_INCREASE + character.characterNumber, OnHappyIncreaseCallback);
            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_SADNESS_INCREASE + character.characterNumber, OnSadnessIncreaseCallback);
            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_ANGER_INCREASE + character.characterNumber, OnAngerIncreaseCallback);
            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_EMBARRASSMENT_INCREASE + character.characterNumber, OnEmbarrassmentIncreaseCallback);



            // character.actionObserverRegistry.RegisterObserver(VOX_ACTION_HAPPY_DECREASE + character.characterNumber, OnHappyDecreaseCallback);




            this.faceTimeline = character.plugins.faceTimelinePlugin;
    
            blushManager = new BlushManager(character);
           

            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_EMOTE_HAPPY, EmoteHappy);
            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_EMOTE_BLUSH, EmoteBlush);
            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_EMOTE_HORNY, EmoteHorny);

            UpdateContext();
        }
        
        public int GetHornyLevel() {
            return levelHorny;
        }
        private void OnHornyIncreaseCallback() {
            logger2.StartMethod("OnHornyIncreaseCallback()");
            hornieness.increase(incrementHorny);

           // this.levelHorny = BindPercent(levelHorny, incrementHorny);
            UpdateContext();
            emotionObserverRegistry.InvokeObservers(REGISTRY_ON_HORNY_CHANGED);

        }
        private void OnHornyDecreaseCallback()
        {
            logger2.StartMethod("OnHornyDecreaseCallback()");
            hornieness.decrease(-incrementHorny);


            //this.levelHorny = BindPercent(levelHorny, -1 * incrementHorny);
            UpdateContext();

            //character.logger.SPECIAL("HORNY Down");
            emotionObserverRegistry.InvokeObservers(REGISTRY_ON_HORNY_CHANGED);
        }
        private void OnHappyIncreaseCallback()
        {
            logger2.StartMethod("OnHappyIncreaseCallback()");
            happyness.increase(incrementHappy);
            UpdateContext();
        }
        private void OnSadnessIncreaseCallback()
        {
            logger2.StartMethod("OnSadnessIncreaseCallback()");
            sadness.increase(incrementSadness);
            UpdateContext();
           }
        private void OnAngerIncreaseCallback()
        {
            logger2.StartMethod("OnAngerIncreaseCallback()");
            anger.increase(incrementAnger);
            UpdateContext();
        }
        private void OnEmbarrassmentIncreaseCallback()
        {
            logger2.StartMethod("OnEmbarrassmentIncreaseCallback()");
            embarrasement.increase(incremenEmbarrassment);
            UpdateContext();
        }
        private Emotion GetStrongestEmotion(List<Emotion> emotions) {
            Emotion strongest = null;
            foreach (Emotion e in emotions) {
                if (strongest == null) { 
                    strongest = e;
                    continue; 
                }
                if (e.value > strongest.value) {
                    strongest = e;
                }
            }
            return strongest;
        }
        
        string lastContextItem;
        private void UpdateContext() {
            logger2.DEBUG("EmotionManager UpdateContext()");
            
            if (lastContextItem != null) { 
                character.voxtaService.voxtaContextService.RemoveContextItem(lastContextItem);
            }
            string context = String.Empty;
            if (this.levelHorny != 0) {
              //  context += string.Format(templateHorny, character.name, levelHorny);//todo multiple levels logic here
            }

            Emotion strongest = GetStrongestEmotion(mainEmotions);
            context += character.name + "'s current mood:";
            if (strongest.ShouldShowInContext())
            {
                 context += strongest.getInfo();//(This is to inform how {1} {0} will be in conversation).
                 context += ", ";
            }
      

            for (int i = 0; i < concurrentEmotions.Count; i++) {
                if (i != 0)
                {
                    context += ", ";
                }
                context += concurrentEmotions[i].getInfo();

                if (i == concurrentEmotions.Count - 1)
                {
                    context += ".";
                }
            }

            character.voxtaService.voxtaContextService.AddContextItem(context);
            lastContextItem = context;
            DebugLogEmotions();
        }
        private void DebugLogEmotions() {
            string logMsg = String.Empty;
            foreach (Emotion e in allEmotions) {
                logMsg += " " + e.getInfo();
            
            }
            logger2.DEBUG(logMsg);
        }

        private String lastEmote = null;
        private void EmoteHappy() {
            logger2.DEBUG("VOX_ACTION_EMOTE_HAPPY");
            faceTimeline.PlaySmile();
        }

        private void EmoteBlush()
        {
            logger2.DEBUG("VOX_ACTION_EMOTE_BLUSH");           
            blushManager.OnBlush();
        }

        private void EmoteHorny()
        {
            logger2.DEBUG("VOX_ACTION_EMOTE_HORNY");            
            faceTimeline.PlayStartHorny();
        }
    }
    internal abstract class Emotion {
        EmotionManager emotionManager;
        public readonly string name;
        public float value;
        public bool isNegativeEmotion = false;
        

        public Emotion(EmotionManager emotionManager, string name, float startingValue) {
            this.emotionManager = emotionManager;
            this.name = name;
            this.value = startingValue;
        }


        public void increase(float increment) {
            value = BindPercent(value, increment);
            increaseOverride(increment);
            EffectOtherEmotions(increment);

        }
        protected abstract void increaseOverride(float increment);

        public void decrease(float decrement)
        {
            value = BindPercent(value, decrement);
            decreaseOverride(decrement);
          
        }
        protected abstract void decreaseOverride(float decrement);

        private float BindPercent(float currentLevel, float increment)
        {
            float newVal = currentLevel + increment;
            if (newVal < 0)
            {
                newVal = 0;
            }
            else if (newVal > 100)
            {
                newVal = 100;
            }

            return newVal;
        }
        public string getInfo() { 
            return string.Format("{0} ({1}%)", name, value);

        }

        public virtual bool ShouldShowInContext() { 
            return value != 0;
        }

        public void EffectOtherEmotions(float increment) {
            var emotionsToDecrease = emotionManager.allEmotions.Where(e => e.isNegativeEmotion == !isNegativeEmotion && e != this);

            foreach (var item in emotionsToDecrease)
            {
                item.decrease(-increment);
            }
        }


    }
    internal class Happyness : Emotion {
        
        public Happyness(EmotionManager emotionManager) : base(emotionManager, "happy", 30f) {
            
        }
        protected override void increaseOverride(float increment) { 
            
        }
        protected override void decreaseOverride(float decrement)
        {

        }
    }
    internal class Sadness : Emotion
    {

        public Sadness(EmotionManager emotionManager) : base(emotionManager, "sad", 0f)
        {
            this.isNegativeEmotion = true;
        }
        protected override void increaseOverride(float increment)
        {

        }
        protected override void decreaseOverride(float decrement)
        {

        }
    }
    internal class Anger : Emotion
    {

        public Anger(EmotionManager emotionManager) : base(emotionManager, "angry", 0f)
        {
            this.isNegativeEmotion = true;
        }
        protected override void increaseOverride(float increment)
        {

        }
        protected override void decreaseOverride(float decrement)
        {

        }
    }
    internal class Embarrassment : Emotion
    {

        public Embarrassment(EmotionManager emotionManager) : base(emotionManager, "embarrassed", 0f)
        {
            this.isNegativeEmotion = true;
        }
        protected override void increaseOverride(float increment)
        {

        }
        protected override void decreaseOverride(float decrement)
        {

        }
    }
    internal class Hornieness : Emotion
    {

        public Hornieness(EmotionManager emotionManager) : base(emotionManager, "horny", 0f)
        {

        }
        protected override void increaseOverride(float increment)
        {

        }
        protected override void decreaseOverride(float decrement)
        {

        }
        public override bool ShouldShowInContext() {
            return true;
        }
    }

}
