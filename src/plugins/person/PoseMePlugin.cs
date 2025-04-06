using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace PPirate.VoxReactor
{
    internal class PoseMePlugin
    {
        VoxtaCharacter character;

        private readonly String poseMePluginStorableID = "CheesyFX.PoseMe";
        private readonly JSONStorable poseMePlugin;

        public PoseMePlugin(VoxtaCharacter character) { 
            this.character = character;

            poseMePlugin = AtomUtils.GetReciever(character.atom, poseMePluginStorableID);
        }

        public void LookAtPlayer() {
            poseMePlugin.CallAction("Focus Now");
        }

 


    }
}
