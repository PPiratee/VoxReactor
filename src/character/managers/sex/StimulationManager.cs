

using System;
using static MeshVR.PresetManager;

namespace PPirate.VoxReactor
{
    internal class StimulationManager: SafeMvr
    {
        private readonly VoxtaCharacter character;
        private readonly Logger logger;
        private float stimulation;

        private bool isHavingOrgasm = false;
        private bool isOnCuspOfOrgasm = false;
        private float orgasmCuspThreshold = 0.9f;
        private string contextItem = null;

        private readonly ReadMyLipsPlugin readMyLipsPlugin;
        public StimulationManager(VoxtaCharacter character) { 
            this.character = character;

            logger = new Logger("OrgasmManager:Char#" + character.characterNumber, 0);
            try
            {
                logger.StartMethod("Constructor");
                character.main.RegisterAction(
                    new JSONStorableAction("Char#" + character.characterNumber + "OnOrgasmStart"
                        , OnOrgasmStart)
                );

                character.main.RegisterAction(
                    new JSONStorableAction("Char#" + character.characterNumber + "OnOrgasmStop"
                        , OnOrgasmStop)
                );

                character.main.RegisterAction(
                    new JSONStorableAction("TESTESTSET"
                        , TEST)
                );
                this.readMyLipsPlugin = character.plugins.readMyLipsPlugin;
                //todo add safe mvr callback 
                // var action = character.plugins.readMyLipsPlugin.GetCharacterOrgasmNowAction();
                AddCallback(readMyLipsPlugin.GetCharacterOrgasmNowAction(),
                    OnOrgasmStart);

                AddCallback(readMyLipsPlugin.GetStimulationStorable(),
                   OnStimulationChange);

                OnStimulationChange(readMyLipsPlugin.GetStimulationValue());
            }
            catch (Exception e)
            {
                logger.ERR("FAILURE during OrgasmManager Constructor: " + e.Message);
            }
        }
        private void OnOrgasmStart()
        {
            logger.ERR("OnOrgasmStart()");
            logger.StartMethod("OnOrgasmStart()");
            character.voxtaService.SendEventNow(character.name + " starts to have an orgasm");

            
        }

        private void OnOrgasmStop()
        {
            logger.StartMethod("OnOrgasmStop()");

            character.voxtaService.SendEventNow(character.name + " just had an orgasm. " + character.name + " will tell {{user}} about it");
        }
        private void TEST()
        {
            logger.StartMethod("TEST()");

           // character.plugins.readMyLipsPlugin.todo();



        }
        private void OnStimulationChange(float stimulation) { 
            this.stimulation = stimulation;
            ClampStimulation();
            bool sendOrgasmCusp = OrgasmCuspCheck();
            UpdateContext();
            if (sendOrgasmCusp) {
                VoxtaPlugin.singleton.SendEvent(character.name + " is about to have an orgasm. She will say as much:");
            }
        }
        private bool OrgasmCuspCheck() {
            if(!isOnCuspOfOrgasm && stimulation >= orgasmCuspThreshold) { //now on cusp of orgasm
                isOnCuspOfOrgasm = true;
                return true;
            }
            if (isOnCuspOfOrgasm && stimulation < orgasmCuspThreshold) // no longer on cusp of orgasm
            {
                isOnCuspOfOrgasm = false;
            }
            return false;
        }
        private void ClampStimulation() {
            var maxStim = character.emotionManager.hornieness.value / 100;
            if (stimulation > maxStim) { 
                readMyLipsPlugin.GetStimulationStorable().val = maxStim;
            }

        }
        private void UpdateContext() {
            int stimPercent = (int)(stimulation * 100);
            if (stimPercent <= 3) {
               
                if (contextItem != null) {
                    VoxtaContextService.singleton.RemoveContextItem(contextItem);
                    contextItem = null;
                }
                
                return;
            }

            string newContextItem = string.Format("{0}'s sexual stimulation level: {1}%", character.name, stimPercent);
            if (isHavingOrgasm) {
                newContextItem += "She is currently having an orgasm.";
            }
            else if (isOnCuspOfOrgasm) {
                newContextItem += "(On cusp of an orgasm)";
            }

            if (contextItem != newContextItem) { 
                VoxtaContextService.singleton.RemoveContextItem(contextItem);
                VoxtaContextService.singleton.AddContextItem(newContextItem);
                contextItem = newContextItem;
            }
        }
    }
}
