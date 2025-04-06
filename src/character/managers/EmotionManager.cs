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


        private readonly int incrementHorny = 10;
        private readonly int incrementHappy = 10;
        private readonly int incrementSadness = 10;
        private readonly int incrementAnger = 10;
        private readonly int incremenEmbarrassment = 10;





        List<Emotion> mainEmotions = new List<Emotion>(); // only context on the strongest will be provided
        List<Emotion> concurrentEmotions = new List<Emotion>(); // does not need to be the strongest
        public List<Emotion> allEmotions = new List<Emotion>(); // does not need to be the strongest

        public readonly Hornieness hornieness;
        public readonly Happyness happyness;
        public readonly Sadness sadness;
        public readonly Anger anger;
        public readonly Embarrassment embarrasement;




        private JSONStorableAction OnHornyIncrease;
        private JSONStorableAction OnHornyDecrease;
        private readonly String VOX_ACTION_AROUSAL_INCREASE = "arousal_increase_char";
        private readonly String VOX_ACTION_AROUSAL_DECREASE = "arousal_decrease_char";
        private readonly String VOX_ACTION_HAPPY_INCREASE = "happy_increase_char";
        private readonly String VOX_ACTION_SADNESS_INCREASE = "sad_increase_char";
        private readonly String VOX_ACTION_ANGER_INCREASE = "anger_increase_char";
        private readonly String VOX_ACTION_EMBARRASSMENT_INCREASE = "embarrassment_increase_char";



       // private readonly String VOX_ACTION_EMOTE_HAPPY = "emote_happy";
       // private readonly String VOX_ACTION_EMOTE_BLUSH = "emote_embarrased";
       // private readonly String VOX_ACTION_EMOTE_HORNY = "emote_horny";

        public readonly ObserverRegistry emotionObserverRegistry;

        public static String REGISTRY_ON_HORNY_CHANGED = "hornyChanged";





        public VoxtaCharacter character;
      


        //Saves\ExpressionRouter\Moods\

        public readonly BlushManager blushManager;
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


    
            blushManager = new BlushManager(character);
           

          //  character.actionObserverRegistry.RegisterObserver(VOX_ACTION_EMOTE_HAPPY, EmoteHappy);
           // character.actionObserverRegistry.RegisterObserver(VOX_ACTION_EMOTE_BLUSH, EmoteBlush);
          //  character.actionObserverRegistry.RegisterObserver(VOX_ACTION_EMOTE_HORNY, EmoteHorny);

            UpdateContext();
        }
    
        private void OnHornyIncreaseCallback() {
            logger2.StartMethod("OnHornyIncreaseCallback()");
            hornieness.Increase(incrementHorny);

           // this.levelHorny = BindPercent(levelHorny, incrementHorny);
            UpdateContext();
            emotionObserverRegistry.InvokeObservers(REGISTRY_ON_HORNY_CHANGED);

        }
        private void OnHornyDecreaseCallback()
        {
            logger2.StartMethod("OnHornyDecreaseCallback()");
            hornieness.Decrease(-incrementHorny);


            //this.levelHorny = BindPercent(levelHorny, -1 * incrementHorny);
            UpdateContext();

            //character.logger.SPECIAL("HORNY Down");
            emotionObserverRegistry.InvokeObservers(REGISTRY_ON_HORNY_CHANGED);
        }
        private void OnHappyIncreaseCallback()
        {
            logger2.StartMethod("OnHappyIncreaseCallback()");
            happyness.Increase(incrementHappy);
            UpdateContext();
        }
        private void OnSadnessIncreaseCallback()
        {
            logger2.StartMethod("OnSadnessIncreaseCallback()");
            sadness.Increase(incrementSadness);
            UpdateContext();
           }
        private void OnAngerIncreaseCallback()
        {
            logger2.StartMethod("OnAngerIncreaseCallback()");
            anger.Increase(incrementAnger);
            UpdateContext();
        }
        private void OnEmbarrassmentIncreaseCallback()
        {
            logger2.StartMethod("OnEmbarrassmentIncreaseCallback()");
            embarrasement.Increase(incremenEmbarrassment);
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
      

            Emotion strongest = GetStrongestEmotion(mainEmotions);
            context += character.name + "'s current mood:";
            strongest.ApplyExpression();
            if (strongest.ShouldShowInContext())
            {
                 context += strongest.GetInfo();//(This is to inform how {1} {0} will be in conversation).
                 context += ", ";
            }
      

            for (int i = 0; i < concurrentEmotions.Count; i++) {
                if (i != 0)
                {
                    context += ", ";
                }
                context += concurrentEmotions[i].GetInfo();

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
                logMsg += " " + e.GetInfo();
            
            }
            logger2.DEBUG(logMsg);
        }

    }
    internal abstract class Emotion {
        protected EmotionManager emotionManager;
        public readonly string name;
        public float value;
        public bool isNegativeEmotion = false;
        

        public Emotion(EmotionManager emotionManager, string name, float startingValue) {
            this.emotionManager = emotionManager;
            this.name = name;
            this.value = startingValue;
        }


        public void Increase(float increment) {
            value = BindPercent(value, increment);
            IncreaseOverride(increment);
            EffectOtherEmotions(increment);

        }
        protected abstract void IncreaseOverride(float increment);

        public void Decrease(float decrement)
        {
            value = BindPercent(value, decrement);
            DecreaseOverride(decrement);
          
        }
        protected abstract void DecreaseOverride(float decrement);
        public virtual void ApplyExpression() {
            emotionManager.character.expressionManager.LoadExpression(name);
        }


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
        public string GetInfo() { 
            return string.Format("{0} ({1}%)", name, value);

        }

        public virtual bool ShouldShowInContext() { 
            return value != 0;
        }

        public void EffectOtherEmotions(float increment) {
            var emotionsToDecrease = emotionManager.allEmotions.Where(e => e.isNegativeEmotion == !isNegativeEmotion && e != this);

            foreach (var item in emotionsToDecrease)
            {
                item.Decrease(-increment);
            }
        }


    }
    internal class Happyness : Emotion {
        
        public Happyness(EmotionManager emotionManager) : base(emotionManager, "happy", 30f) {
            
        }
        protected override void IncreaseOverride(float increment) { 
            
        }
        protected override void DecreaseOverride(float decrement)
        {

        }
        public override void ApplyExpression()
        {
            emotionManager.character.expressionManager.LoadExpression("happy2");
        }

    }
    internal class Sadness : Emotion
    {

        public Sadness(EmotionManager emotionManager) : base(emotionManager, "sad", 0f)
        {
            this.isNegativeEmotion = true;
        }
        protected override void IncreaseOverride(float increment)
        {

        }
        protected override void DecreaseOverride(float decrement)
        {

        }
   
    }
    internal class Anger : Emotion
    {

        public Anger(EmotionManager emotionManager) : base(emotionManager, "angry", 0f)
        {
            this.isNegativeEmotion = true;
        }
        protected override void IncreaseOverride(float increment)
        {

        }
        protected override void DecreaseOverride(float decrement)
        {

        }
    }
    internal class Embarrassment : Emotion
    {

        public Embarrassment(EmotionManager emotionManager) : base(emotionManager, "embarrassed", 0f)
        {
            this.isNegativeEmotion = true;
        }
        protected override void IncreaseOverride(float increment)
        {
            //emotionManager.blushManager.SetMinBlush(value);
            emotionManager.blushManager.OnBlush();

        }
        protected override void DecreaseOverride(float decrement)
        {
            //emotionManager.blushManager.SetMinBlush(value);
           // emotionManager.blushManager.OnBlush();

        }
    }
    internal class Hornieness : Emotion
    {

        public Hornieness(EmotionManager emotionManager) : base(emotionManager, "horny", 0f)
        {

        }
        protected override void IncreaseOverride(float increment)
        {

        }
        protected override void DecreaseOverride(float decrement)
        {

        }
        public override bool ShouldShowInContext() {
            return true;
        }
        public override void ApplyExpression()
        {
           // emotionManager.character.expressionManager.LoadExpression("embarrassed"); todo
        }
    }

}
