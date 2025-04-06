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

        private readonly String VOX_ACTION_BLOWJOB = "blowjob";
        private readonly String VOX_ACTION_BLOWJOB_STOP = "blowjob_stop";

        private readonly float minBlowJBreakDelay = 6.0f;
        private readonly float maxBlowJBreakDelay = 15.0f;
        private readonly float blowJBreakChance = 100f;//60.0f;// /100

        private readonly float minBlowJResumeDelay = 5.0f;
        private readonly float maxBlowJResumeDelay = 5.0f;

        private bool isGivingBj = false;
        public BlowJobManager(VoxtaCharacter character) {
            logger = new Logger("BlowJobManager:Char#" + character.characterNumber);

            this.character = character;

            bjPlugin = character.plugins.bjPlugin;

            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_BLOWJOB, BlowJobStart);
            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_BLOWJOB_STOP, BlowJobStop);
        }

        String bjContextItem = null;
        private void BlowJobStart() {
            logger.StartMethod("BlowJobStart()");
            isGivingBj = true;
            StateManager stateManager = character.stateManager;
            if (stateManager.StateIsSpeaking())
            {
                logger.DEBUG("Char is speaking, waiting for stop speaking to start bj ");
                stateManager.observerRegistry.RegisterObserver(StateManager.REGISTRY_STOP_SPEAKING, BlowJobStartHelper);

            }
            else {
                DoBj();
            }
            

        }
        private void BlowJobStartHelper() { 
            if (isGivingBj) {
                logger.DEBUG("Done speaking, doing BJ now ");
                DoBj();
            }
        }
        private void DoBj() {
            character.main.RunCoroutine(BlowjobBreakEnumerator());
            character.voiceManager.SetCharacterCanSpeak(false);
            bjContextItem = $"{character.name} is giving {character.voxtaService.userName} a blowjob";
            character.voxtaService.voxtaContextService.AddContextItem(bjContextItem);
            character.voxtaService.SendSecret($"{character.name} started giving {character.voxtaService.userName} a blowjob");
            bjPlugin.SetIsActive(true);
            character.gazeManager.SetEnabled(false);

        }
        private void BlowJobStop() {
            logger.DEBUG("Stopping BJ");
            isGivingBj = false;
            character.voiceManager.SetCharacterCanSpeak(true);
            bjPlugin.SetIsActive(false);
            character.gazeManager.SetEnabled(true);
            character.voxtaService.voxtaContextService.RemoveContextItem(bjContextItem);

        }

        IEnumerator BlowjobBreakEnumerator()
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(minBlowJBreakDelay, maxBlowJBreakDelay));
            BlowjobBreak();

        }

        private void BlowjobBreak() {
            if (!isGivingBj) {
                return;
            }
            if (!(UnityEngine.Random.Range(0, 100) <= blowJBreakChance))
            {
                return;
            }
            character.voiceManager.SetCharacterCanSpeak(true);
            bjPlugin.SetIsActive(false);
            character.gazeManager.SetEnabled(true);
            character.voxtaService.voxtaContextService.RemoveContextItem(bjContextItem);
            bjContextItem = $"{character.name} is taking a break from giving {character.voxtaService.userName} a blowjob.";
            character.voxtaService.voxtaContextService.AddContextItem(bjContextItem);
           
            character.voxtaService.SendSecret($"{character.name} pauses giving {character.voxtaService.userName} a blowjob and looks up at him. ");
            character.voxtaService.SendEventNow(" ");
            character.main.RunCoroutine(BlowJobResumeEnumerator());

        }

        IEnumerator BlowJobResumeEnumerator()
        {
            yield return new WaitForSeconds(1);
            if (isGivingBj)
            {
                if (character.stateManager.StateIsIdle())
                {
                    BlowJobResume();
                }
                else { 
                    character.main.RunCoroutine(BlowJobResumeEnumerator());
                }
               
            }
        }
        private void BlowJobResume() {
            isGivingBj = true;

            character.main.RunCoroutine(BlowjobBreakEnumerator());
            character.voiceManager.SetCharacterCanSpeak(false);
            bjPlugin.SetIsActive(true);
            character.gazeManager.SetEnabled(false);
            character.voxtaService.SendSecret($"{character.name} resumed giving {character.voxtaService.userName} a blowjob");
            character.voxtaService.voxtaContextService.RemoveContextItem(bjContextItem);
            bjContextItem = $"{character.name} is giving {character.voxtaService.userName} a blowjob";
            character.voxtaService.voxtaContextService.AddContextItem(bjContextItem);
        }
    }
}
