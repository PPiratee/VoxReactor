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
        

            var contextEnabledToggle = (UIDynamicToggle)emotionConfig.emotionEnabled.CreateUI(uiElements, false);
            contextEnabledToggle.label = "Emotion Enabled";


        }
    }
}
