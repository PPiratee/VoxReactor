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

            toggle = (UIDynamicToggle)blushConfig.emotionEmbarrasedSetsMinBlush.CreateUI(uiElements, false);
            toggle.label = "Embarrassment sets minimum";

            toggle = (UIDynamicToggle)blushConfig.emotionHornySetsMinBlush.CreateUI(uiElements, false);
            toggle.label = "Hornyness sets minimum";

            toggle = (UIDynamicToggle)blushConfig.bodyLanguageStimulationSetsMinBlush.CreateUI(uiElements, false);
            toggle.label = "Stimulation sets minimum";

            

            var slider = (UIDynamicSlider)blushConfig.blushDurationMin.CreateUI(uiElements, false);
            slider.label = $"BlushDurationMin (seconds)";

            slider = (UIDynamicSlider)blushConfig.blushDurationMax.CreateUI(uiElements, false);
            slider.label = $"BlushDurationMax (seconds)";

            slider = (UIDynamicSlider)blushConfig.blushSpeed.CreateUI(uiElements, false);
            slider.label = $"Blush speed";

            slider = (UIDynamicSlider)blushConfig.deBlushSpeed.CreateUI(uiElements, false);
            slider.label = $"De-Blush speed";

            string info = @"
<X> sets minimum: If the the value of X is 100% then the character will be fully blushed; if 0% there is no blush.
This is line three. the stimulation one refers to BodyLanguage stimulation

BlushDuration: When a blush event occurs, it will blush the character to maximum. Blush duration sets the time to remain at that maximum before de-blushing.

Blush speed: The rate at which bulshing will occur to reach the maximum blushing level (I have no clue what unit it is in lol)

De-Blush speed: The rate at which de-bulshing will occur to reach the minimum blushing level
";



            info.CreateStaticInfo(1200, uiElements, false);
        }
    }
}
