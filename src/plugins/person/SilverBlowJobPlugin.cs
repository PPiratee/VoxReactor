using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace PPirate.VoxReactor
{
    internal class SilverBlowJobPlugin
    {
        VoxtaCharacter character;
        private readonly String silverBjPluginStorableID = "ClockwiseSilver.BJ";
        private readonly JSONStorable silverBjPlugin;
        public SilverBlowJobPlugin(VoxtaCharacter character) {
            this.character = character;

            silverBjPlugin = AtomUtils.GetReciever(character.atom, silverBjPluginStorableID);
        }

        public void SetIsActive(bool active)
        {
            silverBjPlugin.SetBoolParamValue("isActive", active);
        }
    }
}
