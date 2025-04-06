using CheesyFX;
using MacGruber;
using System.Collections.Generic;

namespace PPirate.VoxReactor
{
    internal class PannelCharacter : UIPannel
    {
        
        public JSONStorableBool testBool;
        private readonly VoxtaCharacter character;

        private readonly UITabBar characterSettingsTabBar;
        private readonly List<UIPannel> subPannels;
        private readonly ConfigCharacterBase characterConfig;

        public PannelCharacter(UIPannel parentPannel, VoxtaCharacter character, ConfigCharacterBase characterConfig) : base("Char#" + character.characterNumber + " " + character.name, parentPannel)
        {
            this.character = character;
            this.characterConfig = characterConfig;
            testBool = new JSONStorableBool("Char#" + character.characterNumber, true);
            mvrScript.RegisterBool(testBool);

            characterSettingsTabBar = new UITabBar("<PannelCharacter/CharacterSettingsTabBar>", mvrScript, leftUIElements, rightUIElements, null, false);
            subPannels = new List<UIPannel>()
            {
                new PannelCharacterClothingSettings(this, character, characterConfig.clothingConfig)
            };
        }
        public  void SetContainerTabBar(UITabBar containerTabBar)
        {
            /*
            base.SetContainerTabBar(containerTabBar);
            testBool2 = new JSONStorableBool(tabLabel, true);
            containerTabBar.mvrScript.RegisterBool(testBool2);

            createSubMenuBar( "<PannelCharacter/SubSub>" + characterName,
                new List<UIPannel>
                {
                    new SubSubMenu("subsub1"),
                    new SubSubMenu("subsub12")
                });
            */
           
        }
        public override void DrawPannelUI()
        {
            ClearPannelUI();
            MakeBackButton();
            testBool.CreateUI(uiElements, false);

            // testBool2.CreateUI(containerTabBar.UIElements, true);
            characterSettingsTabBar.CreateTabs(subPannels);

        }
    }
}
