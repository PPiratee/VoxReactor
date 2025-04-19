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
        public PannelCharacterEmotionSettings(UIPannel parentPannel, VoxtaCharacter character, ConfigCharacterEmotions emotionConfig) : base("Emotion", parentPannel)
        { 
            this.character = character;
            this.emotionConfig = emotionConfig;

            emotionsTabBar = new UITabBar($"<char#{character.characterNumber}EmotionsTabBar>", mvrScript, leftUIElements, rightUIElements, null, false);
            subPannels = new List<UIPannel>()
            {
                //new PannelCharacterClothingSettings(this, character, characterConfig.clothingConfig),
               // new PannelCharacterEmotionSettings(this, character, characterConfig.emotionConfig)
            };
        }
        public override void DrawPannelUI()
        {
            ClearPannelUI();
            MakeBackButton();
            emotionsTabBar.CreateTabs(subPannels);

            var contextEnabledToggle = (UIDynamicToggle)emotionConfig.emotionsEnabled.CreateUI(uiElements, false);
            contextEnabledToggle.label = "Emotions Enabled";

            //clothingConfig.clothingContextEnabled.AddCallback(isEnabled => to safmvvr
            // {
            //     character.clothingManager.ContextEnabled = isEnabled;
            //  }, false);
        }
    }
}
