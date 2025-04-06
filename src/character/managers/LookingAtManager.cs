using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using LeapInternal;

namespace PPirate.VoxReactor
{
    internal class LookingAtManager
    {
        private readonly VoxtaCharacter character;
        private bool canLookAtTits = true;
        public JSONStorableAction OnLookAtTits;
        private float lookAtTitsCooldown = 40;
        public LookingAtManager(VoxtaCharacter character) { 
            this.character = character;
            OnLookAtTits = new JSONStorableAction("OnLookAtTits", OnLookAtTitsCallback);
            character.main.RegisterAction(OnLookAtTits);
        }
        public void OnLookAtTitsCallback()
        {
            if (!canLookAtTits)
                return;
            canLookAtTits = false;

            renableLookAtTits = RenableLookAtTitsEnumerator();
            character.main.RunCoroutine(renableLookAtTits);

            character.voxtaService.SendSecret(character.name + " caught {{user}} looking at her tits.");
            character.voxtaService.SendEventNow(" ");

        }

        private IEnumerator renableLookAtTits;
        IEnumerator RenableLookAtTitsEnumerator()
        {
            yield return new WaitForSeconds(lookAtTitsCooldown);
            canLookAtTits = true;
        }
    }
}
