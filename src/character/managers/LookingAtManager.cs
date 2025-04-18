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

        private readonly String VOX_ACTION_BATT_EYES = "batt_eyes_char";
        private readonly String VOX_ACTION_WINK = "wink_char";

        private readonly Logger logger;

        public LookingAtManager(VoxtaCharacter character) {
            logger = new Logger("VoxtaCharacter:Char#" + character.characterNumber);
            logger.Constructor();
            this.character = character;
            OnLookAtTits = new JSONStorableAction("Char#"+ character.characterNumber+"OnLookAtTits", OnLookAtTitsCallback);
            character.main.RegisterAction(OnLookAtTits);

            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_BATT_EYES + character.characterNumber, OnBattEyes);
            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_WINK + character.characterNumber, Wink);
            character.stateManager.observerRegistry.RegisterObserver(StateManager.REGISTRY_STOP_SPEAKING, OnSpeakingStop);

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
        int batMinBlink = 3;
        int batMaxBlink = 5;
        int timesToBlink;
        int timesBlinked = 0;

        float timeBetweenBatts = 0.26f;

        public void OnBattEyes() {
            SuperController.LogError("BATTS EYES");
            character.plugins.glancePlugin.SetBlinkEnabled(false);
            timesToBlink = UnityEngine.Random.Range(batMinBlink, batMaxBlink + 1);
            timesBlinked = 0;
            for (int i = 0; i < timesToBlink; i++)
            {
                Main.RunAfterDelay(i * timeBetweenBatts, Blink);
            }


        }
        private void Blink() {
            timesBlinked++;
            SuperController.LogError("timesToBlink" + timesToBlink);


            SuperController.LogError("TimesBlinked" + timesBlinked);

            character.plugins.glancePlugin.BlinkNow();
            if (timesBlinked == timesToBlink) { 
                character.plugins.glancePlugin.SetBlinkEnabled(true);

            }
        }
        private bool pendingWink = false;
        private void Wink()
        {
            logger.StartMethod("Wink()");
            pendingWink = true;
        }

        private void OnSpeakingStop()
        {
            logger.StartMethod("OnSpeakingStop()");
            
            if (pendingWink)
            {
                pendingWink = false;
                character.plugins.glancePlugin.SetBlinkEnabled(false);
                character.plugins.headTimeLine.PlayWink();
                Main.RunAfterDelay(1f, () => {
                    character.plugins.glancePlugin.SetBlinkEnabled(true);
                });
            }
        }
    }
}
