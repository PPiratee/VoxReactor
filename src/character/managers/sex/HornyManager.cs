using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using MeshVR;

namespace PPirate.VoxReactor
{
    internal class HornyManager
    {
        VoxtaCharacter character;

        public JSONStorableAction OnOrgasmStart;
        public JSONStorableAction OnOrgasmStop;

        public HornyManager(VoxtaCharacter character) {
            this.character = character;

            OnOrgasmStart = new JSONStorableAction("OnOrgasmStart", OnOrgasmStartCallback);
            character.main.RegisterAction(OnOrgasmStart);

            OnOrgasmStop = new JSONStorableAction("OnOrgasmStop", OnOrgasmStopCallback);
            character.main.RegisterAction(OnOrgasmStop);
        }
        private void OnOrgasmStartCallback() {
            character.voxtaService.SendEventNow(character.name +" starts to have an orgasm");
        }

        private void OnOrgasmStopCallback()
        {
            character.voxtaService.SendEventNow(character.name + " just had an orgasm. " + character.name + " will tell {{user}} about it");
        }


    }
}
