using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace PPirate.VoxReactor
{
    internal class AcidGlancePlugin
    {

        private readonly string glancePluginStorableID = "Glance";
        private readonly JSONStorable glancePlugin;

        public AcidGlancePlugin(Atom atom)
        {
            glancePlugin = AtomUtils.GetReciever(atom, glancePluginStorableID);

        }
        public static readonly String GLANCE_PRESET_SHY = "Shy";
        public static readonly String GLANCE_PRESET_DEFAULT = "Defaults";
        public static readonly String GLANCE_PRESET_HORNY = "Horny";


        private string currentPreset = GLANCE_PRESET_DEFAULT;
        public string CurrentPreset
        {
            get { return currentPreset; }
        }

        public void LoadPresetShy() {
            currentPreset = GLANCE_PRESET_SHY;
            glancePlugin.SetStringChooserParamValue("Presets", GLANCE_PRESET_SHY);
        }
        public void LoadPresetDefault()
        {
            currentPreset = GLANCE_PRESET_DEFAULT;
            glancePlugin.SetStringChooserParamValue("Presets", GLANCE_PRESET_DEFAULT);

        }
    }
}
