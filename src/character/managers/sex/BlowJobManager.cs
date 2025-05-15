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

        private readonly float minBlowJResumeDelay = 1.0f;
        private readonly float maxBlowJResumeDelay = 5.0f;

        public bool isGivingBj = false;
        public bool isPausingBj = false;

        public BlowJobManager(VoxtaCharacter character) {
            logger = new Logger("BlowJobManager:Char#" + character.characterNumber);

            this.character = character;

            bjPlugin = character.plugins.bjPlugin;

            
            character.stateManager.observerRegistry.RegisterObserver(StateManager.REGISTRY_START_SPEAKING, OnStartTalking);
            character.stateManager.observerRegistry.RegisterObserver(StateManager.REGISTRY_STOP_SPEAKING, OnsStopTalking);
        }

        String bjContextItem = null;
        public void ActionBlowJobStart() {
            logger.StartMethod("BlowJobStart()");
            isGivingBj = true;
            StateManager stateManager = character.stateManager;
            if (stateManager.StateIsSpeaking())
            {
                isPausingBj = true;
                logger.DEBUG("Char is speaking, waiting for stop speaking to start bj ");
            }
            else {
                DoBj();
            }
        }

    

     

        private void OnsStopTalking() { 
            if (isPausingBj) {
                logger.DEBUG("Done speaking, doing BJ now ");
                DoBj();
            }
        }

        private void OnStartTalking() {
            if (isGivingBj)
            {
                isGivingBj = false;
                isPausingBj = true;
                logger.DEBUG("OnStartTalking, while giving bj, pause bj until char stops speaking ");
                bjPlugin.SetIsActive(false);
                character.gazeManager.SetEnabled(true);
                character.voxtaService.voxtaContextService.RemoveContextItem(bjContextItem);
                bjContextItem = $"{character.name} is taking a break from giving {character.voxtaService.userName} a blowjob.";
                character.voxtaService.voxtaContextService.AddContextItem(bjContextItem);
            }
        }

        private void DoBj() {
            AtomUtils.RunAfterDelay(UnityEngine.Random.Range(minBlowJBreakDelay, maxBlowJBreakDelay), () => {
                BlowjobBreak();
            });
            isPausingBj = false;
            isGivingBj = true;
            if(bjContextItem != null)
                character.voxtaService.voxtaContextService.RemoveContextItem(bjContextItem);
            bjContextItem = $"{character.name} is giving {character.voxtaService.userName} a blowjob";
            character.voxtaService.voxtaContextService.AddContextItem(bjContextItem);
            character.voxtaService.SendSecret($"{character.name} started giving {character.voxtaService.userName} a blowjob");
            bjPlugin.SetIsActive(true);
            character.gazeManager.SetEnabled(false);
        }

        public void ActionBlowJobStop() {
            logger.DEBUG("Stopping BJ");
            isGivingBj = false;
            isPausingBj = false;
            //character.voiceManager.SetCharacterCanSpeak(true);
            bjPlugin.SetIsActive(false);
            character.gazeManager.SetEnabled(true);
            character.voxtaService.voxtaContextService.RemoveContextItem(bjContextItem);

        }

        private void BlowjobBreak() {
            if (!isGivingBj || isPausingBj) {
                return;
            }
            if (!(UnityEngine.Random.Range(0, 100) <= blowJBreakChance))
            {
                return;
            }
            //character.voiceManager.SetCharacterCanSpeak(true);
            isPausingBj = true;
            isGivingBj = false;
            bjPlugin.SetIsActive(false);
            character.gazeManager.SetEnabled(true);
            character.voxtaService.voxtaContextService.RemoveContextItem(bjContextItem);
            bjContextItem = $"{character.name} is taking a break from giving {character.voxtaService.userName} a blowjob.";
            character.voxtaService.voxtaContextService.AddContextItem(bjContextItem);
           
            character.voxtaService.SendSecret($"{character.name} pauses giving {character.voxtaService.userName} a blowjob and looks up at him. ");
            character.voxtaService.SendEventNow(" ");

            AtomUtils.RunAfterDelay(1, () => {
                if (isGivingBj)
                {
                    if (character.stateManager.StateIsIdle())
                    {
                        BlowJobResume();
                    }
                    else
                    {
                        isPausingBj = true;
                    }

                }
            });

        }


        private void BlowJobResume() {
            isGivingBj = true;
            isPausingBj = false;

            bjPlugin.SetIsActive(true);
            character.gazeManager.SetEnabled(false);
            character.voxtaService.SendSecret($"{character.name} resumed giving {character.voxtaService.userName} a blowjob");
            character.voxtaService.voxtaContextService.RemoveContextItem(bjContextItem);
            bjContextItem = $"{character.name} is giving {character.voxtaService.userName} a blowjob";
            character.voxtaService.voxtaContextService.AddContextItem(bjContextItem);
        }
    }
}
