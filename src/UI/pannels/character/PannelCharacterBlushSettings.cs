using System.Collections.Generic;
using CheesyFX;

namespace PPirate.VoxReactor
{
    internal class PannelCharacterBlushSettings : UIPannel
    {
        private readonly VoxtaCharacter character;
        
        private readonly ConfigCharacterBlushSettings blushConfig;

 
        public PannelCharacterBlushSettings(UIPannel parentPannel, VoxtaCharacter character, ConfigCharacterBlushSettings blushConfig) : base("Blush", parentPannel)
        {
            this.character = character;
            this.blushConfig = blushConfig;
            
        }
        
        public override void DrawPannelUI()
        {
            ClearPannelUI();
            MakeBackButton();

            var contextEnabledToggle = (UIDynamicToggle)blushConfig.blushEnabled.CreateUI(uiElements, false);
            contextEnabledToggle.label = "Enable Blushing";



            var slider = (UIDynamicSlider)blushConfig.blushDurationMin.CreateUI(uiElements, false);
            slider.label = $"BlushDurationMin (seconds)";

            slider = (UIDynamicSlider)blushConfig.blushDurationMax.CreateUI(uiElements, false);
            slider.label = $"BlushDurationMax (seconds)";

        }
    }
}
