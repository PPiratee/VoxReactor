using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace PPirate.VoxReactor
{

    internal class VoiceManager
    {
        private Logger logger;


        private readonly VoxtaCharacter character;
        private readonly GazeManager gazeManager;

        private readonly ReadMyLipsPlugin readMyLipsPlugin;
        private readonly HeadTimeLine headTimeLine;

        public JSONStorableAction OnNarrateThoughts;


        public VoiceManager(VoxtaCharacter character) {
            logger = new Logger("VoiceManager:Char#" + character.characterNumber);

            this.character = character;
            readMyLipsPlugin = character.plugins.readMyLipsPlugin;
            headTimeLine = character.plugins.headTimeLine;
            gazeManager = character.gazeManager;

            character.stateManager.observerRegistry.RegisterObserver(StateManager.REGISTRY_START_SPEAKING, OnSpeaking);
            character.stateManager.observerRegistry.RegisterObserver(StateManager.REGISTRY_STOP_SPEAKING, OnSpeakingStop);

            OnNarrateThoughts = new JSONStorableAction("OnNarrateThoughts", OnNarrateThoughtsCallback);
            character.main.RegisterAction(OnNarrateThoughts);
            //character.stateObserverRegistry.RegisterObserver("speaking");
        }
        private bool wasMoaning = false;
        private bool enableMoansFlag = false;
        private void OnSpeaking() {
            logger.DEBUG("VoiceManager - OnSpeaking");


            if (isNarratingThoughts) { return; }
            wasMoaning = readMyLipsPlugin.GetMoansEnabled();
            readMyLipsPlugin.SetMoansEnabled(false);
            //todo who are you speaking to?
            
            gazeManager.LookAtPlayer();
            float talkingHeadBobDelay = 1f;
            AtomUtils.RunAfterDelay(talkingHeadBobDelay,() => { 
                headTimeLine.Talk();
                
            });

        }
        
        private void OnSpeakingStop()
        {
            logger.DEBUG("VoiceManager - OnSpeakingStop" + wasMoaning);
            bool condition = wasMoaning || enableMoansFlag;


            if (wasMoaning || enableMoansFlag) {
                readMyLipsPlugin.SetMoansEnabled(true);
                logger.DEBUG("VoiceManager - enabling moans ");
                enableMoansFlag = false;

            }
            StopSpeakingHelper();
            isNarratingThoughts = false;
        }
        public void SetCharacterCanSpeak(bool canSpeak) {
            character.voxtaService.SetCharacterCanSpeak(canSpeak);
            if (!canSpeak) {
                StopSpeakingHelper();
            }
        }

        private void StopSpeakingHelper()
        {
            headTimeLine.StopTalk();
        }
        private bool isNarratingThoughts = false;
        private void OnNarrateThoughtsCallback() { //todo work with multiple characters
            isNarratingThoughts = true;
            character.voxtaService.TriggerCommand("thoughts");
        }
    }

}
