using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using static System.Collections.Specialized.BitVector32;
using LeapInternal;
using Leap.Unity.Query;
using System.Linq;
using System.CodeDom;

namespace PPirate.VoxReactor
{
    internal class EmotionManager: SafeMvr
    {
        private Logger logger;

       


        private bool emotionsEnabled = true;


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




        public readonly ObserverRegistry emotionObserverRegistry;

        public static String REGISTRY_ON_HORNY_CHANGED = "hornyChanged";


        public readonly ConfigCharacterEmotions emotionsConfig;


        public VoxtaCharacter character;
      


        //Saves\ExpressionRouter\Moods\

        public readonly BlushManager blushManager;
        public EmotionManager(VoxtaCharacter character) {
            try {
                logger = new Logger("VoxtaCharacter:Char#" + character.characterNumber);
                logger.Constructor();
                this.character = character;

                hornieness = new Hornieness(this);
                concurrentEmotions.Add(hornieness);

                happyness = new Happyness(this);
                mainEmotions.Add(happyness);
               
                sadness = new Sadness(this);
                mainEmotions.Add(sadness);

                anger = new Anger(this);
                mainEmotions.Add(anger);

                embarrasement = new Embarrassment(this);
                concurrentEmotions.Add(embarrasement);

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

                emotionsConfig = ConfigVoxReactor.singeton.GetCharacterConfig(character).emotionConfig;

                AddCallback(emotionsConfig.emotionsEnabled, EmotionsEnabledToggle);
                

                UpdateContext(null);


            } catch (Exception e) {
                SuperController.LogError("EmotionManager constructor failure: " + e.Message);
            }
            
        }
        private void  EmotionsEnabledToggle(bool enableEmotions) {
            if (emotionsEnabled && !enableEmotions) { 
                DisableContext();
            } else if (!emotionsEnabled && enableEmotions)
            {
                EnableContext();
            }
            emotionsEnabled = enableEmotions;
        }
    
        private void OnHornyIncreaseCallback() {
            logger.StartMethod("OnHornyIncreaseCallback()");
            hornieness.Increase(incrementHorny);

           // this.levelHorny = BindPercent(levelHorny, incrementHorny);
            UpdateContext(hornieness);
            emotionObserverRegistry.InvokeObservers(REGISTRY_ON_HORNY_CHANGED);

        }
        private void OnHornyDecreaseCallback()
        {
            logger.StartMethod("OnHornyDecreaseCallback()");
            hornieness.Decrease(-incrementHorny);


            //this.levelHorny = BindPercent(levelHorny, -1 * incrementHorny);
            UpdateContext(hornieness);

            //character.logger.SPECIAL("HORNY Down");
            emotionObserverRegistry.InvokeObservers(REGISTRY_ON_HORNY_CHANGED);
        }
        private void OnHappyIncreaseCallback()
        {
            logger.StartMethod("OnHappyIncreaseCallback()");
            happyness.Increase(incrementHappy);
            UpdateContext(happyness);
        }
        private void OnSadnessIncreaseCallback()
        {
            logger.StartMethod("OnSadnessIncreaseCallback()");
            sadness.Increase(incrementSadness);
            UpdateContext(sadness);
           }
        private void OnAngerIncreaseCallback()
        {
            logger.StartMethod("OnAngerIncreaseCallback()");
            anger.Increase(incrementAnger);
            UpdateContext(anger);
        }
        private void OnEmbarrassmentIncreaseCallback()
        {
            logger.StartMethod("OnEmbarrassmentIncreaseCallback()");
            embarrasement.Increase(incremenEmbarrassment);
            UpdateContext(embarrasement);
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
        public Emotion currentStrongestEmotion;
        private void UpdateContext(Emotion emotionThatChanged) {
            logger.DEBUG("EmotionManager UpdateContext()");
            
            if (lastContextItem != null) { 
                character.voxtaService.voxtaContextService.RemoveContextItem(lastContextItem);
            }
            string context = character.name + "'s current mood:";
         


            Emotion newStrongestEmotion = GetStrongestEmotion(allEmotions);
            if (emotionThatChanged != newStrongestEmotion) {
                emotionThatChanged?.ApplyExpressionReaction();
            }
            if (newStrongestEmotion != currentStrongestEmotion)
            {
                newStrongestEmotion.ApplyExpression();
                currentStrongestEmotion = newStrongestEmotion;
            }
            



            if (currentStrongestEmotion.ShouldShowInContext())
            {
                context += currentStrongestEmotion.GetInfo();
               // context += ", ";
            }



            for (int i = 0; i < allEmotions.Count; i++) {
                var emotion = allEmotions[i];
                if (!emotion.ShouldShowInContext() || emotion == currentStrongestEmotion)
                {
                    continue;
                }

                if (i != 0)
                {
                   // context += ", ";
                }

                context += emotion.GetInfo();

                if (i == allEmotions.Count - 1)
                {
                   // context += ".";
                }
            }

            character.voxtaService.voxtaContextService.AddContextItem(context);
            lastContextItem = context;
            DebugLogEmotions();
        }
        private void EnableContext() {
            character.voxtaService.voxtaContextService.AddContextItem(lastContextItem);
        }
        private void DisableContext() {
            character.voxtaService.voxtaContextService.RemoveContextItem(lastContextItem);
        }
        private void DebugLogEmotions() {
            string logMsg = String.Empty;
            foreach (Emotion e in allEmotions) {
                logMsg += " " + e.GetInfo();
            
            }
            logger.DEBUG(logMsg);
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

        

        public void Increase(float increment, bool effectOtherEmotions = true) {
            value = BindPercent(value, increment);
            IncreaseOverride(increment);
            if(effectOtherEmotions)
                EffectOtherEmotions(increment);

        }
        protected abstract void IncreaseOverride(float increment);

        public void Decrease(float decrement)
        {
            value = BindPercent(value, decrement);
            DecreaseOverride(decrement);
          
        }
        protected abstract void DecreaseOverride(float decrement);
        public void ChangeValueViaMultiplier(float increment)
        {
            if (increment == 0)
            {
                return;
            }
            if (increment > 0)
            {
                Increase(increment, false);
            }
            else
            {
                Decrease(increment);
            }
        }
        public virtual void ApplyExpression() {
            emotionManager.character.expressionManager.LoadExpression(name);
        }

        private static bool isExpressionReactionInProgress = false;
        private static readonly float expressionReactionDuration = 6f;
        public virtual void ApplyExpressionReaction()
        {
            if (isExpressionReactionInProgress) {
                return;
            }
            isExpressionReactionInProgress = true;
            emotionManager.character.expressionManager.LoadExpression(name);
            AtomUtils.RunAfterDelay(expressionReactionDuration, () => {
                isExpressionReactionInProgress = false;
                if (emotionManager.currentStrongestEmotion == this) {
                    return;
                }
                emotionManager.currentStrongestEmotion.ApplyExpression();
            });
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
            return string.Format("({0}: {1}%)", name, value);

        }

        public virtual bool ShouldShowInContext() { 
            return value != 0;
        }

        public void EffectOtherEmotions(float increment) {

            var emotionsToEffect = emotionManager.allEmotions.Where(e => e != this);
            var emotionConfig = emotionManager.emotionsConfig.GetEmotionConfigByName(this.name);
            foreach (var otherEmotion in emotionsToEffect)
            {
                float multiplier = emotionConfig.GetMultiplierForEmotion(otherEmotion.name);
               otherEmotion.ChangeValueViaMultiplier(increment * multiplier);
            }
        }


    }
    internal class Happyness : Emotion {
        public static string happynessName = "happy";

        public Happyness(EmotionManager emotionManager) : base(emotionManager, happynessName, 40f) {
            
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
        public override bool ShouldShowInContext()
        {
            return this == emotionManager.currentStrongestEmotion && value != 0;
        }

    }
    internal class Sadness : Emotion
    {
        public static string sadnessName = "sad";

        public Sadness(EmotionManager emotionManager) : base(emotionManager, sadnessName, 0f)
        {
            this.isNegativeEmotion = true;
        }
        protected override void IncreaseOverride(float increment)
        {

        }
        protected override void DecreaseOverride(float decrement)
        {

        }
        public override bool ShouldShowInContext()
        {
            return this == emotionManager.currentStrongestEmotion && value != 0;
        }

    }
    internal class Anger : Emotion
    {
        public static string angerName = "angry";
        public Anger(EmotionManager emotionManager) : base(emotionManager, angerName, 0f)
        {
            this.isNegativeEmotion = true;
        }
        protected override void IncreaseOverride(float increment)
        {

        }
        protected override void DecreaseOverride(float decrement)
        {

        }
        public override bool ShouldShowInContext()
        {
            return this == emotionManager.currentStrongestEmotion && value != 0;
        }
    }
    internal class Embarrassment : Emotion
    {
        public static string embarrassmentName = "embarrassed";

        public Embarrassment(EmotionManager emotionManager) : base(emotionManager, embarrassmentName, 35f)
        {
            this.isNegativeEmotion = true;
        }
        protected override void IncreaseOverride(float increment)
        {
           // SuperController.LogError("over");
            emotionManager.blushManager.SetMinBlush(value);
            emotionManager.blushManager.CancelPendingDeblush();
            emotionManager.blushManager.OnBlush();

        }
        protected override void DecreaseOverride(float decrement)
        {
            emotionManager.blushManager.SetMinBlush(value);
            emotionManager.blushManager.LerpToMinBLush();
         

        }
        public override bool ShouldShowInContext()
        {
            return value != 0;
        }
    }
    internal class Hornieness : Emotion
    {
        public static string hornieNessName = "horny";


        public Hornieness(EmotionManager emotionManager) : base(emotionManager, hornieNessName, 20f)
        {

        }
        protected override void IncreaseOverride(float increment)
        {

        }
        protected override void DecreaseOverride(float decrement)
        {

        }
        public override void ApplyExpression()
        {
           // emotionManager.character.expressionManager.LoadExpression("embarrassed"); todo
        }
        public override bool ShouldShowInContext()
        {
            return value != 0;
        }
        public override void ApplyExpressionReaction()
        {
            //
        }

    }

}
