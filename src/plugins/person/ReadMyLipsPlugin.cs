using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace PPirate.VoxReactor
{
    internal class ReadMyLipsPlugin
    {
        VoxtaCharacter character;

        private readonly String readMyLipsPluginStorableID = "CheesyFX.ReadMyLips";
        private readonly JSONStorable plugin;

        public ReadMyLipsPlugin(VoxtaCharacter character) { 
            this.character = character;

            plugin = AtomUtils.GetReciever(character.atom, readMyLipsPluginStorableID);
        }

        public void SetMoansEnabled(bool val) {
            plugin.SetBoolParamValue("Voice Enabled", val);
        }

        public bool GetMoansEnabled()
        {
            return plugin.GetBoolParamValue("Voice Enabled");
        }

        public void OrgasmDildosNow() {
            plugin.CallAction("Orgasm Dildos Now");
        }
        public void CharacterOrgasmNow() {
            plugin.CallAction("Orgasm Now");
        }
        public JSONStorableAction GetCharacterOrgasmNowAction()
        {
            return plugin.GetAction("Orgasm Now");
        }
        public JSONStorableFloat GetStimulationStorable() {
            return plugin.GetFloatJSONParam("Stimulation");
        }
        public float GetStimulationValue()
        {
            return plugin.GetFloatJSONParam("Stimulation").val;
        }
      


    }
}
