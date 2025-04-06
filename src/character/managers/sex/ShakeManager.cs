using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;


namespace PPirate.VoxReactor
{
    internal class ShakeManager
    {
        private Logger logger;

        VoxtaCharacter character;

        private readonly String VOX_ACTION_SHAKE_TITS = "shake_tits";
        private readonly BodyTimeline timeline;
        public ShakeManager(VoxtaCharacter character) {
            this.character = character;
            timeline = character.plugins.bodyTimeline;
            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_SHAKE_TITS, ShakeTits);
            OnShakeIncriment = new JSONStorableAction("OnShakeIncriment", ShakeIncrement);
            character.main.RegisterAction(OnShakeIncriment);

        }
        private int shakeTitsNumMin = 2;
        private int shakeTitsNumMax = 5;
        private int shakeNumber = -1;

        public JSONStorableAction OnShakeIncriment;
        private void ShakeTits() {
          //  character.logger.LOG("ShakeManager - ShakeTits");
            shakeNumber = -1;
            timeline.PlayShakeTits();
            shakeLimit = UnityEngine.Random.Range(shakeTitsNumMin, shakeTitsNumMax);
        }
        private int shakeLimit;
        private void ShakeIncrement() {
          //  character.logger.LOG("ShakeManager - ShakeIncrement");
            shakeNumber++;
            if(shakeNumber >= shakeLimit)
            {
                timeline.PlayShakeIdle();
            }
        }

    }
}
