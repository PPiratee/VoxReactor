using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace PPirate.VoxReactor
{
    internal class StateManager
    {
        private Logger logger;


        private VoxtaCharacter character;
        private string currentState = VoxtaPlugin.STATE_IDLE;
        public String CurrentState { get; }

        public readonly ObserverRegistry observerRegistry;
        public static string REGISTRY_START_SPEAKING = "startSpeaking";
        public static string REGISTRY_STOP_SPEAKING = "stopSpeaking";


        public StateManager(VoxtaCharacter character) {
            logger = new Logger("StateManager:Char#" + character.characterNumber);

            this.character = character;
            observerRegistry = new ObserverRegistry();
        }

        public void UpdateState(string newState)
        {
            logger.DEBUG("Update state: " + newState);
            // bool isNewState = state == currentState;
            // if (!isNewState) {
            //    return;
            // }

            if (newState == VoxtaPlugin.STATE_SPEAKING)
            {
                observerRegistry.InvokeObservers(REGISTRY_START_SPEAKING);
                character.voxtaService.conversationService
                    .OnCharacterSpeaking(character.atom);
            }
            else if (currentState == VoxtaPlugin.STATE_SPEAKING) {
                observerRegistry.InvokeObservers(REGISTRY_STOP_SPEAKING);
            }
            

            currentState = newState;
        }
        public String GetCurrentState() {
            return currentState;
        }
        public bool StateIsIdle() {
            return currentState == VoxtaPlugin.STATE_IDLE;
        }
        public bool StateIsSpeaking()
        {
            return currentState == VoxtaPlugin.STATE_SPEAKING;
        }
    }


}
