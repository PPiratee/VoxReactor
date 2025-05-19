using System.Collections.Generic;
using CheesyFX;

namespace PPirate.VoxReactor
{
    internal class PannelCharacterSpecificEmotionSettings : UIPannel
    {
        private readonly VoxtaCharacter character;
        
        private readonly ConfigCharacterSpecificEmotion emotionConfig;

       
        public PannelCharacterSpecificEmotionSettings(UIPannel parentPannel, VoxtaCharacter character, ConfigCharacterSpecificEmotion emotionConfig) : base(emotionConfig.emotionName, parentPannel)
        { 
            this.character = character;
            this.emotionConfig = emotionConfig;

          
        }
        public override void DrawPannelUI()
        {
            ClearPannelUI();
            MakeBackButton();

            string info = "DecayRate: how quickly this emotion will decrease in percent/minute";
            info.CreateStaticInfo(120, uiElements, false);

            info = "DecayInterval: how many seconds pass in between applying the decay";
            info.CreateStaticInfo(120, uiElements, false);

            info = "Multipliers: If this emotion is increased by X, then emotion B's value is increased by X * BMultiplier";
            info.CreateStaticInfo(120, uiElements, false);

            var contextEnabledToggle = (UIDynamicToggle)emotionConfig.emotionEnabled.CreateUI(uiElements, false);
            contextEnabledToggle.label = $"{emotionConfig.emotionName} enabled";

            var slider = (UIDynamicSlider)emotionConfig.decayRate.CreateUI(uiElements, false);
            slider.label = $"DecayRate";

            slider = (UIDynamicSlider)emotionConfig.decayInterval.CreateUI(uiElements, false);
            slider.label = $"DecayInterval";

            if (emotionConfig.emotionName != Hornieness.hornieNessName) {
                slider = (UIDynamicSlider)emotionConfig.hornynessMultiplier.CreateUI(uiElements, false);
                slider.label = $"HornynessMultiplier";
            }
            if (emotionConfig.emotionName != Happyness.happynessName)
            {
                slider = (UIDynamicSlider)emotionConfig.happynessMultiplier.CreateUI(uiElements, false);
                slider.label = $"HappynessMultiplier";
            }
            if (emotionConfig.emotionName != Sadness.sadnessName)
            {
                slider = (UIDynamicSlider)emotionConfig.sadnessMultiplier.CreateUI(uiElements, false);
                slider.label = $"SadnessMultiplier";
            }
            if (emotionConfig.emotionName != Anger.angerName)
            {
                slider = (UIDynamicSlider)emotionConfig.angerMultiplier.CreateUI(uiElements, false);
                slider.label = $"AngerMultiplier";
            }
            if (emotionConfig.emotionName != Embarrassment.embarrassmentName)
            {
                slider = (UIDynamicSlider)emotionConfig.embarrassmentMultiplier.CreateUI(uiElements, false);
                slider.label = $"EmbarrasementMultiplier";
            }


        }
    }
}
