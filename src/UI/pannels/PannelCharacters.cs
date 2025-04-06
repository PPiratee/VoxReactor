using CheesyFX;
using System.Collections.Generic;
using System;
namespace PPirate.VoxReactor
{
    internal class PannelCharacters : UIPannel
    {
        
        public JSONStorableBool testBool2;

        private readonly UITabBar charactersTabBar;
        private List<UIPannel> subPannels = new List<UIPannel>();
        private readonly ConfigVoxReactor configVoxReactor;
        public PannelCharacters(UIPannel parentPannel) : base("Characters", parentPannel)
        {
            charactersTabBar = new UITabBar("<PannelCharacters/CharactersTabBar>", mvrScript, leftUIElements, rightUIElements, null, false);
            
            testBool2 = new JSONStorableBool("PannelCharacters", true);
            mvrScript.RegisterBool(testBool2);
            this.configVoxReactor = Main.singleton.Config;
        }
        public void OnVoxCharactersLoaded() {
            subPannels.Clear();
            VoxtaService.singleton.characters.ForEach(character =>
            {
                ConfigCharacterBase configCharacter = GetCharacterConfig(character);
                subPannels.Add(new PannelCharacter(this, character, configCharacter));
            });
        }

        public override void DrawPannelUI()
        {
            //SuperController.LogError("(PannelCharacters) - CreatePannelUI()");
            ClearPannelUI();
            MakeBackButton();
            testBool2.CreateUI(uiElements, false);
            // OnSubPannelSelected();
            //base.CreatePannelUI();

            if(subPannels.Count > 0)
            {
                charactersTabBar.CreateTabs(subPannels);
            }
        }
        private ConfigCharacterBase GetCharacterConfig(VoxtaCharacter voxChar) {
            if (voxChar.characterNumber == 1) {
                return configVoxReactor.char1Config;
            }
            throw new Exception("PannelCharacters - Unknown character number");
        }

    }
}
