using CheesyFX;

namespace PPirate.VoxReactor
{
    internal class PannelCharacterEmotionSettings : UIPannel
    {
        private readonly VoxtaCharacter character;
        

        private readonly ConfigCharacterEmotions emotionConfig;
        public PannelCharacterEmotionSettings(UIPannel parentPannel, VoxtaCharacter character, ConfigCharacterEmotions emotionConfig) : base("Emotion", parentPannel)
        { 
            this.character = character;
            this.emotionConfig = emotionConfig;
        }
        public override void DrawPannelUI()
        {
            ClearPannelUI();
            MakeBackButton();
            var contextEnabledToggle = (UIDynamicToggle)emotionConfig.emotionsEnabled.CreateUI(uiElements, false);
            contextEnabledToggle.label = "Emotions Enabled";
            
            //clothingConfig.clothingContextEnabled.AddCallback(isEnabled => to safmvvr
            // {
            //     character.clothingManager.ContextEnabled = isEnabled;
            //  }, false);
        }
    }
}
