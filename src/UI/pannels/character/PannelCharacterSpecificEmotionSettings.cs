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

            string info = "Multipliers: If this emotion is increased by X, then emotion B's value is increased by X * BMultiplier";

         
            info.CreateStaticInfo(120, uiElements, false);

            var contextEnabledToggle = (UIDynamicToggle)emotionConfig.emotionEnabled.CreateUI(uiElements, false);
            contextEnabledToggle.label = $"{emotionConfig.emotionName} enabled";

            if (emotionConfig.emotionName != Hornieness.hornieNessName) {
                var hornySlider = (UIDynamicSlider)emotionConfig.hornynessMultiplier.CreateUI(uiElements, false);
                hornySlider.label = $"HornynessMultiplier";
            }
            if (emotionConfig.emotionName != Happyness.happynessName)
            {
                var hornySlider = (UIDynamicSlider)emotionConfig.happynessMultiplier.CreateUI(uiElements, false);
                hornySlider.label = $"HappynessMultiplier";
            }
            if (emotionConfig.emotionName != Sadness.sadnessName)
            {
                var sadSLider = (UIDynamicSlider)emotionConfig.sadnessMultiplier.CreateUI(uiElements, false);
                sadSLider.label = $"SadnessMultiplier";
            }
            if (emotionConfig.emotionName != Anger.angerName)
            {
                var sadSLider = (UIDynamicSlider)emotionConfig.angerMultiplier.CreateUI(uiElements, false);
                sadSLider.label = $"AngerMultiplier";
            }
            if (emotionConfig.emotionName != Embarrassment.embarrassmentName)
            {
                var sadSLider = (UIDynamicSlider)emotionConfig.embarrassmentMultiplier.CreateUI(uiElements, false);
                sadSLider.label = $"EmbarrasementMultiplier";
            }


        }
    }
}
