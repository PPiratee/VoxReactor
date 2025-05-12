using System;
using UnityEngine;
using System.Collections;
using LeapInternal;
using static MeshVR.PresetManager;

namespace PPirate.VoxReactor
{
    internal class BlushManager : SafeMvr
    {
        VoxtaCharacter character;
        //todo may glance shy and look away on blush and embarrased

        public float currentSpeed;
        public float blushTarget = 0f; //in percent out of 100



        private readonly BlushClothingConfig clothingItem1;
        private readonly BlushClothingConfig clothingItem2;


        readonly AcidGlancePlugin glancePlugin;

        private readonly Logger logger;
        private readonly JSONStorableAction OnBlush;

        private readonly ConfigCharacterBlushSettings blushConfig;

        public BlushManager(VoxtaCharacter character) {
            logger = new Logger("BlushManager:Char#" + character.characterNumber);
            logger.Constructor();

            this.character = character;
            glancePlugin = character.plugins.glancePlugin;


            clothingItem1 = new BlushClothingConfig("VRDollz:Makeup Blush2 VRDMaterialFace", this, -1.0f, -0.75f);
            clothingItem2 = new BlushClothingConfig("crimeless:face_blushMaterialCombined", this, -0.61f, -0.365f);
            OnBlush = new JSONStorableAction($"OnBlush_{character.role}", Blush);
            Main.singleton.RegisterAction(OnBlush);

            blushConfig = ConfigVoxReactor.singeton.GetCharacterConfig(character).blushConfig;

            AddCallback(blushConfig.blushEnabled, BlushEnabledToggle);

            AddCallback(character.plugins.readMyLipsPlugin.GetStimulationStorable(), OnStimulationChange);

            character.main.PushFixedDeltaTimeConsumer(BlushUpdate); //if blushing enable
            glancePlugin.LoadPresetDefault();

        }
        private void BlushEnabledToggle(bool enabled) {
            logger.LOG("blush enabled: " + enabled);
            if (!enabled) {
                isBLushing = false;
                pendingDeBlush = false;
                fixedUpdateEnabled = false;
                //character.main.RemoveFixedDeltaTimeConsumer(BlushUpdate);//might cause problem
            }
        }

        private bool pendingDeBlush = false;
        private bool isBLushing = false; // as in blush event, going to the bax blush,
        bool fixedUpdateEnabled = false;
        public void Blush() {
            logger.StartMethod("Blush()");
            logger.DEBUG("isBLushing: " + isBLushing);
            if (isBLushing || !blushConfig.blushEnabled.val)
            {
                return;
            }
            glancePlugin.LoadPresetShy();

            isBLushing = true;
            isDeblushing = false;
            currentSpeed = blushConfig.blushSpeed.val /10f;
            blushTarget = 100f;


            StartBlushInterpolating();

        }



        public void LerpToMinBLush()
        {
            logger.StartMethod("LerpToMinBLush()");
            logger.DEBUG("isBLushing: " + isBLushing);
            if (isBLushing || isDeblushing || !blushConfig.blushEnabled.val)
            {
                return;
            }
            isBLushing = true;
            isDeblushing = false;
            currentSpeed = blushConfig.deBlushSpeed.val / 10f;

            blushTarget = GetMinBlush();
            StartBlushInterpolating();
        }

        public void CancelPendingDeblush()
        {
            pendingDeBlush = false;
        }

        private bool isDeblushing = false;
        public void BlushUpdate(float deltaTime) {
            if (!fixedUpdateEnabled) {
                return;
            }
            bool isDone = clothingItem1.BlushUpdate(deltaTime);
            bool isDone2 = clothingItem2.BlushUpdate(deltaTime);

            if (isDone && isDone2) {

                if (isBLushing)
                {
                    logger.LOG("DONE BLUSHING");
                    isBLushing = false;

                    float deblushDelay = UnityEngine.Random.Range(blushConfig.blushDurationMin.val, blushConfig.blushDurationMax.val);
                    AtomUtils.RunAfterDelay(deblushDelay, () => {
                        logger.StartMethod("RunAfterDelay deblushing");

                        pendingDeBlush = false;
                        isDeblushing = true;
                        currentSpeed = blushConfig.deBlushSpeed.val / 10;
                        blushTarget = GetMinBlush();
                        StartBlushInterpolating();
                    });
                }

                if (isDeblushing)
                {
                    logger.LOG("DONE De-Blushing");
                    fixedUpdateEnabled = false;
                    isDeblushing = false;
                    glancePlugin.LoadPresetDefault();
                    return;
                }
            }
        }


        private void StartBlushInterpolating() {
            logger.StartMethod("StartBlushInterpolating()");
            clothingItem1.SetTimeInterpolating(0.0f);
            clothingItem2.SetTimeInterpolating(0.0f);


            clothingItem1.UpdateInterpolationValue();
            clothingItem2.UpdateInterpolationValue();

            fixedUpdateEnabled = true;
            // character.main.PushFixedDeltaTimeConsumer(BlushUpdate);
        }

        private void OnStimulationChange(float stimulation) {
            if (!blushConfig.bodyLanguageStimulationSetsMinBlush.val) {
                return;
            }

            LerpToMinBLush();
        }
       
        private float GetMinBlush()
        {
            float embarass = blushConfig.emotionEmbarrasedSetsMinBlush.val ? character.emotionManager.embarrasement.value : 0;
            float horny = blushConfig.emotionHornySetsMinBlush.val ? character.emotionManager.hornieness.value : 0;
            float stimulation = blushConfig.bodyLanguageStimulationSetsMinBlush.val ? character.plugins.readMyLipsPlugin.GetStimulationValue() * 100 : 0;

            return Mathf.Max(stimulation, Mathf.Max(embarass, horny));
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
                clothing.SetFloatParamValue("Alpha Adjust", alphaNoBlush);
                interpolationStartingValue = alphaNoBlush;
            }

            public void UpdateInterpolationValue() {
                interpolationStartingValue = clothing.GetFloatParamValue("Alpha Adjust");
            }

            public bool BlushUpdate(float deltaTime)
            {

                float targetAlpha = SimpleLerp(alphaNoBlush, alphaFullBlush, blushManager.blushTarget);
                float blush1NewAlpha = LerpWithSpeed(interpolationStartingValue, targetAlpha, blushManager.currentSpeed, deltaTime);

                clothing.SetFloatParamValue("Alpha Adjust", blush1NewAlpha);

                bool isDone = IsDone(blush1NewAlpha);
                if (!isDone) {

                    blushManager.logger.DEBUG("current alpha: "+ clothing.GetFloatParamValue("Alpha Adjust"));
                    blushManager.logger.DEBUG("target: " + targetAlpha);
                }

                return isDone;
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
