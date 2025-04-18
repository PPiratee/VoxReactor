using System;
using UnityEngine;
using System.Collections;
using LeapInternal;
using static MeshVR.PresetManager;

namespace PPirate.VoxReactor
{
    internal class BlushManager
    {
        VoxtaCharacter character;
        //todo may glance shy and look away on blush and embarrased

        readonly float blushSpeed = 0.12f;
        readonly float deblushSpeed = 0.025f;
        public float currentSpeed;
        public float blushTarget = 0f; //in percent out of 100
        private float minBlush = 0f; //in percent out of 100

        private readonly BlushClothingConfig clothingItem1;
        private readonly BlushClothingConfig clothingItem2;


        readonly float blushDurationMin = 7f;
        readonly float blushDurationMax = 20f;

        readonly AcidGlancePlugin glancePlugin;

        private readonly Logger logger;

        public BlushManager(VoxtaCharacter character) {
            logger = new Logger("BlushManager:Char#" + character.characterNumber);
            logger.Constructor();

            this.character = character;
            glancePlugin = character.plugins.glancePlugin;
  

            clothingItem1 = new BlushClothingConfig("VRDollz:Makeup Blush2 VRDMaterialFace", this, - 1.0f, -0.75f);
            clothingItem2 = new BlushClothingConfig("crimeless:face_blushMaterialCombined", this, -0.61f, - 0.365f);
        }

        private bool pendingDeBlush = false;
        private bool isBLushing = false;
        public void OnBlush() {
           
            logger.StartMethod("OnBlush()");
            if (isBLushing) {
                return;
            }
            glancePlugin.LoadPresetShy();

            isBLushing = true;
            currentSpeed = blushSpeed;
            blushTarget = 100f;


            StartBlushInterpolating();

            float deblushDelay = UnityEngine.Random.Range(blushDurationMin, blushDurationMax);
            pendingDeBlush = true;
            Main.RunAfterDelay(deblushDelay,() => {
                if (!pendingDeBlush) {
                    return;
                }
                currentSpeed = deblushSpeed;
                blushTarget = minBlush;
                StartBlushInterpolating();
            });

        }
        

        public void BlushUpdate(float deltaTime) {
           // SuperController.LogError("interpoalting");
            bool isDone = clothingItem1.BlushUpdate(deltaTime);
            bool isDone2 = clothingItem2.BlushUpdate(deltaTime);

            if (isDone && isDone2) {
                if (!isBLushing) {
                    pendingDeBlush = false;
                    glancePlugin.LoadPresetDefault();
                }
                isBLushing = false;
                character.main.RemoveFixedDeltaTimeConsumer(BlushUpdate);
            }
        }
       
       
        private void StartBlushInterpolating() {
            logger.StartMethod("StartBlushInterpolating()");
            clothingItem1.SetTimeInterpolating(0.0f);
            clothingItem2.SetTimeInterpolating(0.0f);


            clothingItem1.UpdateInterpolationValue();
            clothingItem2.UpdateInterpolationValue();

            character.main.PushFixedDeltaTimeConsumer(BlushUpdate);
        }

        public void SetMinBlush(float minBlush) {
            if (minBlush == blushTarget) { 
                blushTarget = minBlush;
            }
            this.minBlush = minBlush;
        }
        public void LerpToMinBLush()
        {
            if (!isBLushing && !pendingDeBlush) {
                blushTarget = minBlush;
                StartBlushInterpolating();
            }
        }

        public void CancelPendingDeblush() {
            pendingDeBlush = false;
        }
        //todod  glancePlugin.LoadPresetShy();
    
        
        private void GlanceDefaults()
        {
            if (glancePlugin.CurrentPreset == AcidGlancePlugin.GLANCE_PRESET_SHY)
            {
                glancePlugin.LoadPresetDefault();
            }
        }
        internal class BlushClothingConfig {
            private readonly string storableId;
            BlushManager blushManager;
            public readonly float alphaNoBlush;
            public readonly float alphaFullBlush;
            public float interpolationStartingValue;
            private readonly JSONStorable clothing;

            public BlushClothingConfig(string storableId, BlushManager blushManager, float alphaNoBlush, float alphaFullBlush)
            {
                this.storableId = storableId;
                this.blushManager = blushManager;
                this.alphaNoBlush = alphaNoBlush;
                this.alphaFullBlush = alphaFullBlush;

                clothing = blushManager.character.atom.GetStorableByID(storableId);
                if (clothing == null)
                {
                    SuperController.LogError("BlushManager - Unable to find the clothing item: " + storableId);
                }
                interpolationStartingValue = clothing.GetFloatParamValue("Alpha Adjust");
            }

            public void UpdateInterpolationValue() {
                interpolationStartingValue = clothing.GetFloatParamValue("Alpha Adjust");
            }

            public bool BlushUpdate(float deltaTime)
            {
                //SuperController.LogError("current alpha: "+ clothing.GetFloatParamValue("Alpha Adjust"));
                float targetAlpha = SimpleLerp(alphaNoBlush, alphaFullBlush, blushManager.blushTarget);
                float blush1NewAlpha = LerpWithSpeed(interpolationStartingValue, targetAlpha, blushManager.currentSpeed, deltaTime);

                clothing.SetFloatParamValue("Alpha Adjust", blush1NewAlpha);
 
                return IsDone(blush1NewAlpha);
            }
            public bool IsDone(float newAlpha)
            {
                var percentBlushed = Delerp(alphaNoBlush, alphaFullBlush, newAlpha);
                
                return Math.Abs(percentBlushed - blushManager.blushTarget) < 3f;
            }
            float timeInterpolating = 0f;
            public void SetTimeInterpolating(float timeInterpolating) {
                this.timeInterpolating = timeInterpolating;
            }
            //todo adjust speed based on how far there is to go
            private float LerpWithSpeed(float a, float b, float speed, float deltaTime)
            {

                float distance = b - a;
                float duration = distance / speed;
                if (duration < 0)
                {
                    duration = duration * -1.0f;
                }

                if (duration <= 0)
                {
                    return b;
                }
                float progress = timeInterpolating / duration;
                if (progress < 0)
                {
                    progress = 0;
                }
                if (progress > 1)
                {
                    progress = 1;

                }
                timeInterpolating = timeInterpolating + deltaTime;
                return a + (b - a) * progress;
            }

            private float SimpleLerp(float min, float max, float t)
            {
                return min + (max - min) * (t / 100);
            }

            private float Delerp(float min, float max, float value)
            {
                return (value - min) / (max - min) * 100;
            }

        }
    }
}
