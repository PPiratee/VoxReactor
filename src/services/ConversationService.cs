using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace PPirate.VoxReactor
{
    internal class ConversationService
    {
        private readonly VoxtaService voxtaService;
        public ConversationService(VoxtaService voxtaService) {
            this.voxtaService = voxtaService;
            voxtaService.globalObserverRegistry.RegisterObserver(VoxtaService.REGISTRY_USER_SPEAKING, OnUserSpeaking);
        }

        public void OnCharacterSpeaking(Atom speakingAtom) {
            foreach (VoxtaCharacter character in voxtaService.characters) {
                if (character.atom == speakingAtom) {
                    character.gazeManager.LookAtPlayer();
                    continue;
            //todo handle atom that is speaking
                }

                character.gazeManager.LookAtAtom(speakingAtom.idText.text);
            }
        }
        private void OnUserSpeaking() {
            foreach (VoxtaCharacter character in voxtaService.characters)
            {
                character.gazeManager.LookAtPlayer();
            }
        }
    }
}
