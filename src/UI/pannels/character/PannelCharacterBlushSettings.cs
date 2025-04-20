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

            var toggle = (UIDynamicToggle)blushConfig.blushEnabled.CreateUI(uiElements, false);
            toggle.label = "Enable Blushing";

            toggle = (UIDynamicToggle)blushConfig.emotionEmbarrasedSetsMinimumBLush.CreateUI(uiElements, false);
            toggle.label = "Embarrassment sets minimum";

            toggle = (UIDynamicToggle)blushConfig.emotionHornySetsMinimumBlush.CreateUI(uiElements, false);
            toggle.label = "Hornyness sets minimum";

            var slider = (UIDynamicSlider)blushConfig.blushDurationMin.CreateUI(uiElements, false);
            slider.label = $"BlushDurationMin (seconds)";

            slider = (UIDynamicSlider)blushConfig.blushDurationMax.CreateUI(uiElements, false);
            slider.label = $"BlushDurationMax (seconds)";

            string info = @"BlushDuration: When a blush event occurs, it will blush the character to maximum. Blush duration sets the time to remain at that maximum before de-blushing.

<X> sets minimum: If the the value of X is 100% then the character will be fully blushed; if 0% there is no blush.
This is line three.";

            info.CreateStaticInfo(600, uiElements, false);
        }
    }
}
