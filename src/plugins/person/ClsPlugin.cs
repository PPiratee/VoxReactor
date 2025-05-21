using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace PPirate.VoxReactor
{
    internal class ClsPlugin
    {
        VoxtaCharacter character;

        private readonly String storableID = "JaxPlugins.CLS";
        private readonly JSONStorable plugin;

        public ClsPlugin(VoxtaCharacter character) { 
            this.character = character;

            plugin = AtomUtils.GetReciever(character.atom, storableID);
        }

        public void ToggleEnabled(bool val) {
          //  plugin.SetBoolParamValue("enabled", val);
        }

        public void ToggleCanMove(bool val)
        {
            plugin.SetBoolParamValue("Can Move", val);
        }




    }
}
