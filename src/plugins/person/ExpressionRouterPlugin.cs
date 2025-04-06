using System;
using System.Security.Cryptography;
using System.Linq;


namespace PPirate.VoxReactor
{
    internal class ExpressionRouterPlugin
    {
        VoxtaCharacter character;

        private readonly String storableID = "ExpressionRouter";

        private readonly JSONStorable plugin;

        public ExpressionRouterPlugin(VoxtaCharacter character)
        {
            this.character = character;

            plugin = AtomUtils.GetReciever(character.atom, storableID);
        }
        public void StopAll() {
            plugin.CallAction("StopAll");
        }
        public void StartAll()
        {
            plugin.GetBoolJSONParam("[Brows] Active").val = true;
            plugin.GetBoolJSONParam("[Eyes] Active").val = true;
            plugin.GetBoolJSONParam("[Nose] Active").val = true;
            plugin.GetBoolJSONParam("[Mouth] Active").val = true;
            plugin.GetBoolJSONParam("[Lips] Active").val = true;
            plugin.GetBoolJSONParam("[T10] Active").val = true;

        }

        public void LoadMood(string mood) {
            SuperController.LogError("loading mood " + mood);
            //  SuperController.LogError("is plugin null? " + (null == plugin));
            // SuperController.LogError("is it null? " + (null == plugin.GetStringChooserJSONParamChoices("LoadMoodFromPath")));

            // plugin.GetStringChooserActionNames
            /*
            foreach (var item in plugin.GetPresetFilePathActionNames())
            {
                //has LoadMoodFromPath
                
                SuperController.LogError("item: " + item);
            }*/


            // var pathAction = plugin.GetPresetFilePathAction("LoadMoodFromPath");
            plugin.CallPresetFileAction("LoadMoodFromPath", "Saves/ExpressionRouter/PPirate/" + mood +".json");
           // plugin.GetFile
            //plugin.file

            /*
            var options = plugin.GetStringChooserJSONParamChoices("LoadMoodFromPath")
                .Where(path => path.Contains("PPirate"));
            foreach (var option in options) {
                if (option.Contains(mood)) {
                    plugin.SetStringChooserParamValue("LoadMoodFromPath", option);
                    break;
                }
            }*/
        }

    }
}
