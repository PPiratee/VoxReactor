using System;
using System.Collections.Generic;

namespace PPirate.VoxReactor
{
    internal class ClothingManager
    {
        private Logger logger;


        VoxtaCharacter character;
        private bool contextEnabled = true;
        public bool ContextEnabled
        {
            get { return contextEnabled; }
            set { contextEnabled = value;
                ClothingService.singleton.UpdateClothingContext();
            }
        }
        List<string> clothingDescriptions = new List<string>();

        String isWearing = " is wearing: ";
        string context = "";

        public ClothingManager(VoxtaCharacter character)
        {
            logger = new Logger("ClothingManager:Char#" + character.characterNumber, 0);

            this.character = character;
            
        }
        public void OnAdd(string clothDescription)
        {
            logger.DEBUG("OnAdd: " + clothDescription);
            if (clothDescription != "" &&  clothingDescriptions.Contains(clothDescription))
            {
                logger.DEBUG("Already put clothing item on");
                return;
            }
            clothingDescriptions.Add(clothDescription);
            UpdateContext();
            ClothingService.singleton.UpdateClothingContext();
        }
        public void OnRemove(string clothDescription) {
            if (!clothingDescriptions.Contains(clothDescription))
            {
                return;
            }
            clothingDescriptions.Remove(clothDescription);
            UpdateContext();
            ClothingService.singleton.UpdateClothingContext();
        }

        public string GetContext() {
            if (contextEnabled == false) {
                return "";
            }
            return context;
        }
        public void UpdateContext()
        {
            string newContext ="";
            if (clothingDescriptions.Count == 0)
            {
                newContext += character.name + " is nude.";
            }
            else
            {
                newContext += getStartOfContext();
                for (int i = 0; i < clothingDescriptions.Count -1; i++)
                {
                    newContext += clothingDescriptions[i] + ", ";
                }
                newContext += clothingDescriptions[clothingDescriptions.Count - 1] + ".";
            }

            context = newContext;
            logger.DEBUG("New context is:" + newContext);
        }
        private string getStartOfContext() {
            if (clothingDescriptions.Count > 1) {
                return character.name + isWearing;
            }

            string rv = character.name + " is wearing only a ";
            if ("aeiouAEIOU".IndexOf(clothingDescriptions[0][0]) >= 0)
            {
                rv = character.name + " is wearing only an ";  // Append 'an' instead of 'a' for vowels
            }
            return rv;
        }
      
    }
}
