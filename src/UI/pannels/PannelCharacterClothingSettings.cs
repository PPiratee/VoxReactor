using CheesyFX;

namespace PPirate.VoxReactor
{
    internal class PannelCharacterClothingSettings : UIPannel
    {
        private readonly VoxtaCharacter character;
        

        private readonly ConfigCharacterClothing clothingConfig;
        public PannelCharacterClothingSettings(UIPannel parentPannel, VoxtaCharacter character, ConfigCharacterClothing clothingConfig) : base("Clothing", parentPannel)
        { 
            this.character = character;
            this.clothingConfig = clothingConfig;
        }
        public override void DrawPannelUI()
        {
            ClearPannelUI();
            MakeBackButton();
            var contextEnabledToggle = (UIDynamicToggle)clothingConfig.clothingContextEnabled.CreateUI(uiElements, false);
            contextEnabledToggle.label = "Enable Clothing Context";

            //clothingConfig.clothingContextEnabled.AddCallback(isEnabled => to safmvvr
           // {
           //     character.clothingManager.ContextEnabled = isEnabled;
          //  }, false);
        }
    }
}
