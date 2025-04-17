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
           // SuperController.LogError("Stopall");
            plugin.CallAction("StopAll");
        }
        public void StartAll()
        {
           // SuperController.LogError("StartAll");

            plugin.GetBoolJSONParam("[Brows] Active").val = true;
            plugin.GetBoolJSONParam("[Eyes] Active").val = true;
            plugin.GetBoolJSONParam("[Nose] Active").val = true;
            plugin.GetBoolJSONParam("[Mouth] Active").val = true;
            plugin.GetBoolJSONParam("[Lips] Active").val = true;
            plugin.GetBoolJSONParam("[T10] Active").val = true;
          

            SuperController.LogError("Done");

        }

        public void LoadMood(string mood) {
           // SuperController.LogError("LoadMood");

            plugin.CallPresetFileAction("LoadMoodFromPath", "Saves/ExpressionRouter/PPirate/" + mood +".json");
        }

        

    }
}
