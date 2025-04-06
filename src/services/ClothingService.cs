using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using PPirate.VoxReactor;
using static Leap.Unity.MultiTypedList;

namespace PPirate.VoxReactor
{
    internal class ClothingService
    {
        public static ClothingService singleton;

        private readonly VoxtaService voxtaService;
        private readonly VoxtaContextService contextService;

        private readonly JSONStorableString ClothDescription;
        private readonly JSONStorableString ClothAtomName;

        public ClothingService(Main main, VoxtaService voxtaService, VoxtaContextService contextService)
        {
            this.voxtaService = voxtaService;
            this.contextService = contextService;

            main.RegisterAction(new JSONStorableAction("OnAddCloth", OnAddClothCallback));
            main.RegisterAction(new JSONStorableAction("OnRemoveCloth", OnRemoveClothCallback));
            ClothDescription = new JSONStorableString("ClothDescription", "");
            main.RegisterString(ClothDescription);
            ClothAtomName = new JSONStorableString("ClothAtomName", "");
            main.RegisterString(ClothAtomName);

            foreach (VoxtaCharacter character in voxtaService.characters) {
                map.Add(character.name, new List<string>());
            }

            UpdateClothingContext();
            ClothingService.singleton = this;
        }
     

        Dictionary<string, List<string>> map = new Dictionary<string, List<string>>();
        public void OnAddClothCallback()
        {
            if (ShouldSkip()) {
                return;
            }
            voxtaService.getCharacterByAtomName(ClothAtomName.val)
                .clothingManager.OnAdd(ClothDescription.val);
        }
        public void OnRemoveClothCallback() {
            if (ShouldSkip())
            {
                return;
            }
            voxtaService.getCharacterByAtomName(ClothAtomName.val)
                .clothingManager.OnRemove(ClothDescription.val);

        }
        String oldContextItem = "";

        public void UpdateClothingContext() {
            String contextItem = "";
            VoxtaService.singleton.characters.ForEach(character =>
            {
                contextItem += character.clothingManager.GetContext();
            });

            if (oldContextItem != "") { 
                contextService.RemoveContextItem(oldContextItem);
                contextService.AddContextItem(contextItem);
                oldContextItem = contextItem;
            }
        }
        private bool ShouldSkip() {
            return ClothDescription.val == "" || ClothAtomName.val == "";
        }
    }
}
