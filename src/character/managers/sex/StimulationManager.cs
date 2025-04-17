

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
        private float orgasmCuspThreshold = 0.82f;
        private string contextItem = null;

        private readonly ReadMyLipsPlugin readMyLipsPlugin;
        public StimulationManager(VoxtaCharacter character) { 
            this.character = character;

            logger = new Logger("OrgasmManager:Char#" + character.characterNumber);
            try
            {
                logger.StartMethod("Constructor");

                this.readMyLipsPlugin = character.plugins.readMyLipsPlugin;


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
            character.voxtaService.SendEventNow(character.name + " starts to have an orgasm. She will say as much:");

            
        }

        private void OnOrgasmStop()
        {
            logger.StartMethod("OnOrgasmStop()");

            character.voxtaService.SendEventNow(character.name + " just had an orgasm. " + character.name + " will tell {{user}} about it");
        }
  
        private void OnStimulationChange(float stimulation) { 
            this.stimulation = stimulation;
            ClampStimulation();
            bool sendOrgasmEvent = OrgasmCheck();
            bool sendOrgasmCuspEvent = false;
            if (!sendOrgasmEvent) { 
                sendOrgasmCuspEvent = OrgasmCuspCheck();
            }

            UpdateContext();

            if (sendOrgasmEvent) {
                OnOrgasmStart();
            }
            else if (sendOrgasmCuspEvent) {
                VoxtaPlugin.singleton.SendEvent(character.name + " is about to have an orgasm. She will say as much:");
            }
        }
        private bool OrgasmCheck()
        {
            if (!isHavingOrgasm && stimulation >= readMyLipsPlugin.GetOrgasmThreshold().val)
            { //now on cusp of orgasm
                isHavingOrgasm = true;
                return true;
            }
          
            return false;
        }
        private bool OrgasmCuspCheck() {
            if(!isHavingOrgasm && !isOnCuspOfOrgasm && stimulation >= orgasmCuspThreshold) { //now on cusp of orgasm
                isOnCuspOfOrgasm = true;
                return true;
            }
            if (stimulation < orgasmCuspThreshold) {
                if (isOnCuspOfOrgasm) {
                    isOnCuspOfOrgasm = false;
                }
                isHavingOrgasm = false;
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
                newContextItem += "(Currently having an orgasm)";
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
