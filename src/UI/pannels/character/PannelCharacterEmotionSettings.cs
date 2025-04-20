using System.Collections.Generic;
using CheesyFX;

namespace PPirate.VoxReactor
{
    internal class PannelCharacterEmotionSettings : UIPannel
    {
        private readonly VoxtaCharacter character;
        
        private readonly ConfigCharacterEmotions emotionConfig;

        private readonly UITabBar emotionsTabBar;
        private readonly List<UIPannel> subPannels;
        public PannelCharacterEmotionSettings(UIPannel parentPannel, VoxtaCharacter character, ConfigCharacterEmotions emotionsConfig) : base("Emotions", parentPannel)
        { 
            this.character = character;
            this.emotionConfig = emotionsConfig;

            emotionsTabBar = new UITabBar($"<char#{character.characterNumber}EmotionsTabBar>", mvrScript, leftUIElements, rightUIElements, null, false);
            subPannels = new List<UIPannel>()
            {
                new PannelCharacterSpecificEmotionSettings(this, character, emotionsConfig.hornienessConfig),
                new PannelCharacterSpecificEmotionSettings(this, character, emotionsConfig.happynessConfig),
                new PannelCharacterSpecificEmotionSettings(this, character, emotionsConfig.sadnessConfig),
                new PannelCharacterSpecificEmotionSettings(this, character, emotionsConfig.angerConfig),
                new PannelCharacterSpecificEmotionSettings(this, character, emotionsConfig.embarrassmentConfig),
            };
        }
        
        public override void DrawPannelUI()
        {
            ClearPannelUI();
            MakeBackButton();

            var contextEnabledToggle = (UIDynamicToggle)emotionConfig.emotionsEnabled.CreateUI(uiElements, false);
            contextEnabledToggle.label = "Emotions Enabled";
            
          
           emotionsTabBar.CreateTabs(subPannels);
        }
    }
}
