using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace PPirate.VoxReactor
{
    internal class SilverHandJobPlugin
    {
        private readonly string storableID = "ClockwiseSilver.HJ";
        private readonly JSONStorable silverHjPlugin;

        public SilverHandJobPlugin(Atom characterAtom) {

            silverHjPlugin = AtomUtils.GetReciever(characterAtom, storableID);
        }

        public void SetSpeedMin(float min) {
            silverHjPlugin.SetFloatParamValue("Speed Min", min);
        }

        public void SetSpeedMax(float max)
        {
            silverHjPlugin.SetFloatParamValue("Speed Max", max);
        }

        public void SetOverallSpeed(float speed)
        {
            silverHjPlugin.SetFloatParamValue("Overall Speed", speed);
        }
        public void SetIsActive(bool active) { 
            silverHjPlugin.SetBoolParamValue("isActive", active);
        }
        public void SetTopOnlyChance(float chance) {
            silverHjPlugin.SetFloatParamValue("Top Only Chance", chance);
        }
    }
}
