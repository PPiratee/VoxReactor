using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using LeapInternal;

namespace PPirate.VoxReactor
{
    internal class BlowJobManager
    {
        private Logger logger;

        private VoxtaCharacter character;
        private SilverBlowJobPlugin bjPlugin;

        
        private readonly float minBlowJBreakDelay = 6.0f;
        private readonly float maxBlowJBreakDelay = 15.0f;
        private readonly float blowJBreakChance = 100f;//60.0f;// /100

        private readonly float minBlowJResumeDelay = 0.9f;
        private readonly float maxBlowJResumeDelay = 1.1f;

        public bool isGivingBj = false;
        public bool isPausingBj = false;

        public BlowJobManager(VoxtaCharacter character) {
            logger = new Logger("BlowJobManager:Char#" + character.characterNumber);

            this.character = character;

            bjPlugin = character.plugins.bjPlugin;

            
            character.stateManager.observerRegistry.RegisterObserver(StateManager.REGISTRY_SPEAKING_START, OnCharacterStartTalking);
            character.stateManager.observerRegistry.RegisterObserver(StateManager.REGISTRY_SPEAKING_STOP, OnsStopTalking);
            VoxtaService.singleton.globalObserverRegistry.RegisterObserver(VoxtaService.REGISTRY_USER_SPEAKING, OnUserSpeaking);
           
        }

        String bjContextItem = null;
        bool startingSecretSent;
        public void ActionBlowJobStart() {//todo
            isPausingBj = true; // this action is called before a state change, so the bj will start then pause for the reply to the blowjob accept
            startingSecretSent = false;

            resumeEnumerator = AtomUtils.RunAfterDelay(UnityEngine.Random.Range(2, 2), () => {
                if(!character.stateManager.StateIsSpeaking())
                    DoBj();
            });
        }

        public void ActionBlowJobStop()
        {
            StopResumeCoroutine();
            StopBreakCoroutine();
            logger.DEBUG("Stopping BJ");
            isGivingBj = false;
            isPausingBj = false;
            //character.voiceManager.SetCharacterCanSpeak(true);
            bjPlugin.SetIsActive(false);
            character.gazeManager.SetEnabled(true);
            character.voxtaService.voxtaContextService.RemoveContextItem(bjContextItem);
        }




        IEnumerator breakEnumerator;
        private void StopBreakCoroutine() {
            if(breakEnumerator != null)
                Main.singleton.StopCoroutine(breakEnumerator);
        }
        private void DoBj() {
            isPausingBj = false;
            isGivingBj = true;
            if(bjContextItem != null)
                character.voxtaService.voxtaContextService.RemoveContextItem(bjContextItem);
            bjContextItem = $"{character.name} is giving {character.voxtaService.userName} a blowjob.";
            character.voxtaService.voxtaContextService.AddContextItem(bjContextItem);
            if (!startingSecretSent)
            {
                character.voxtaService.SendSecret($"{character.name} started giving {character.voxtaService.userName} a blowjob.");
                startingSecretSent = true;
            }
            else {
                character.voxtaService.SendSecret($"{character.name} resumed giving {character.voxtaService.userName} a blowjob.");
            }
                bjPlugin.SetIsActive(true);
            character.gazeManager.SetEnabled(false);

            breakEnumerator = AtomUtils.RunAfterDelay(UnityEngine.Random.Range(minBlowJBreakDelay, maxBlowJBreakDelay), () => {
                if (!(UnityEngine.Random.Range(0, 100) <= blowJBreakChance))
                {
                    return;
                }
                PauseBlowJob();
                character.voxtaService.SendEventNow(" ");
            });
        }


        private void PauseBlowJob() {
            if (!isGivingBj || isPausingBj) {
                return;
            }
            
            isPausingBj = true;
            isGivingBj = false;
            bjPlugin.SetIsActive(false);
            character.gazeManager.SetEnabled(true);

            character.voxtaService.voxtaContextService.RemoveContextItem(bjContextItem);
            bjContextItem = $"{character.name} is pausing giving {character.voxtaService.userName} a blowjob to talk dirty.";
            character.voxtaService.voxtaContextService.AddContextItem(bjContextItem);
           
            character.voxtaService.SendSecret($"{character.name} pauses giving {character.voxtaService.userName} a blowjob and looks up at him. ");
        }
        private void OnCharacterStartTalking()
        {
            StopResumeCoroutine();
            StopBreakCoroutine();
            if (isGivingBj && !isPausingBj)
            {
                logger.DEBUG("OnStartTalking, while giving bj, pause bj until char stops speaking ");

               PauseBlowJob();   
            }
        }
        public void OnUserSpeaking() {
            StopBreakCoroutine();
            StopResumeCoroutine();
            if (isGivingBj && !isPausingBj)
            {
                logger.DEBUG("OnUserStartTalking, while giving bj, pause bj until char stops speaking ");

                PauseBlowJob();
            }
        }

        IEnumerator resumeEnumerator;
        private void StopResumeCoroutine()
        {
            if (resumeEnumerator != null)
                Main.singleton.StopCoroutine(resumeEnumerator);
        }
        private void OnsStopTalking()
        {
            if (isPausingBj)
            {
                logger.DEBUG("Done speaking, doing BJ now ");

                resumeEnumerator = AtomUtils.RunAfterDelay(UnityEngine.Random.Range(minBlowJResumeDelay, maxBlowJResumeDelay), () => {
                    DoBj();
                });
            }
        }
    }
}
