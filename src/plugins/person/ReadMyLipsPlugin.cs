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
        private readonly JSONStorable readMyLipsPlugin;

        public ReadMyLipsPlugin(VoxtaCharacter character) { 
            this.character = character;

            readMyLipsPlugin = AtomUtils.GetReciever(character.atom, readMyLipsPluginStorableID);
        }

        public void SetMoansEnabled(bool val) {
            readMyLipsPlugin.SetBoolParamValue("Voice Enabled", val);
        }

        public bool GetMoansEnabled()
        {
            return readMyLipsPlugin.GetBoolParamValue("Voice Enabled");
        }

        public void OrgasmDildosNow() {
            readMyLipsPlugin.CallAction("Orgasm Dildos Now");
        }


    }
}
