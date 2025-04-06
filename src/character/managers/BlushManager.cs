using System;
using UnityEngine;
using System.Collections;
using LeapInternal;

namespace PPirate.VoxReactor
{
    internal class BlushManager
    {
        VoxtaCharacter character;


        float blushSpeed = 0.12f;
        float deblushSpeed = 0.025f;

        private readonly String clothing1StorableID = "VRDollz:Makeup Blush2 VRDMaterialFace";
        private readonly JSONStorable clothing1;
        private float clothing1BlushAlphaInterpolationStart = -1.0f; //current val when we start blushing or deblushing //todo init this to current value from constructor //getFloatParam('Alpha Adjust').val;
        private float clothing1AlphaFullBlush = -0.75f;
        private float clothing1AlphaNoBlush = -1.0f; //todo init this to = clothing1BlushAlphaStart
        


        private readonly String clothing2StorableID = "crimeless:face_blushMaterialCombined";
        private readonly JSONStorable clothing2;
        private float clothing2BlushAlphaInterpolationStart = -0.61f; //current val when we start blushing or deblushing //todo init this to current value from constructor //getFloatParam('Alpha Adjust').val;
        private float clothing2AlphaFullBlush = -0.365f;
        private float clothing2AlphaNoBlush = -0.61f; //todo init this to = clothing1BlushAlphaStart
        
        
        private float minBlush = 0f; //in percent /100

        #region temp

        GazeManager gazeManager;
        float gazeAwayDurationMin = 3f;
        float gazeAwayDurationMax = 6f;

        float blushDurationMin = 7f;
        float blushDurationMax = 20f;

        AcidGlancePlugin glancePlugin;
        float glanceAwayDurationMin = 7f;
        float glanceAwayDurationMax = 20f;

        private readonly Logger logger;

        public BlushManager(VoxtaCharacter character) {
            logger = new Logger("BlushManager:Char#" + character.characterNumber);
            logger.Constructor();

            this.character = character;
            glancePlugin = character.plugins.glancePlugin;
            gazeManager = character.gazeManager;

            clothing1 = character.atom.GetStorableByID(clothing1StorableID);
            if (clothing1 == null) { 
                logger.ERR("Unable to find the clothing item: " + clothing1StorableID);
            }
            
            clothing2 = character.atom.GetStorableByID(clothing2StorableID);
            if (clothing1 == null) { 
                logger.ERR("Unable to find the clothing item: " + clothing2StorableID);
            }


            clothing1BlushAlphaInterpolationStart = clothing1.GetFloatParamValue("Alpha Adjust");
            clothing2BlushAlphaInterpolationStart = clothing2.GetFloatParamValue("Alpha Adjust");

        }

        private bool isBlushed = false;

        public void OnBlush() {
            logger.StartMethod("OnBlush()");
            if (isBlushed)
                return;

            deblushCoroutine = DeblushEnumerator();
            character.main.RunCoroutine(deblushCoroutine);

            StartBlushInterpolating(BlushFixedUpdate);

            isBlushed =true;

        }
        private IEnumerator gazeAwayCoroutine;
        IEnumerator GazeAwayEnumerator()
        {
            // suspend execution for 5 seconds
            yield return new WaitForSeconds(UnityEngine.Random.Range(gazeAwayDurationMin, gazeAwayDurationMax));
            GazeAtPlayer();
            

        }
        private void GazeAtPlayer() {
           
            //gazeManager.Defaults();
        }
        private IEnumerator glanceAwayCoroutine;
        IEnumerator GlanceAwayEnumerator()
        {
            // suspend execution for 5 seconds
            yield return new WaitForSeconds(UnityEngine.Random.Range(glanceAwayDurationMin, glanceAwayDurationMax));
            GlanceDefaults();

        }
        private void GlanceDefaults() {
            if (glancePlugin.CurrentPreset == AcidGlancePlugin.GLANCE_PRESET_SHY)
            {
                glancePlugin.LoadPresetDefault();
            }
        }
        private IEnumerator deblushCoroutine;
        IEnumerator DeblushEnumerator()
        {
            // suspend execution for 5 seconds
            isDeblushing = true;
            yield return new WaitForSeconds(UnityEngine.Random.Range(blushDurationMin, blushDurationMax));
            StartBlushInterpolating(DeBlushUpdate);

        }

        #endregion
        float timeInterpolating = 0.0f;

        Action<float> currentInterpolationCallback = null;
        private void StartBlushInterpolating(Action<float> callback) {
            if (callback == currentInterpolationCallback) {
                return;
            }
            character.main.RemoveFixedDeltaTimeConsumer(currentInterpolationCallback);

            timeInterpolating = 0.0f;
            clothing1BlushAlphaInterpolationStart = clothing1.GetFloatParamValue("Alpha Adjust");
            clothing2BlushAlphaInterpolationStart = clothing2.GetFloatParamValue("Alpha Adjust");

            character.main.PushFixedDeltaTimeConsumer(callback);
            currentInterpolationCallback = callback;
        }
        private bool isDeblushing;
        public void DeBlushUpdate(float deltaTime) {
            if (!isDeblushing) {
                return;
            }
            float target = SimpleLerp(clothing1AlphaNoBlush, clothing1AlphaFullBlush, minBlush);
            //SuperController
            float blush1NewAlpha = linearInterpolate(clothing1BlushAlphaInterpolationStart, target, deblushSpeed, deltaTime);

            float target2 = SimpleLerp(clothing2AlphaNoBlush, clothing2AlphaFullBlush, minBlush);
            float blush2NewAlpha = linearInterpolate(clothing2BlushAlphaInterpolationStart, target2, deblushSpeed, deltaTime);

            if (blush1NewAlpha == clothing1AlphaNoBlush && blush2NewAlpha == clothing2AlphaNoBlush)
            {
                character.main.RemoveFixedDeltaTimeConsumer(DeBlushUpdate);
                isDeblushing = false;
                if (!isBlushed)
                    return;
                GlanceDefaults();
                GazeAtPlayer();

                isBlushed = false;
            }
            clothing1.SetFloatParamValue("Alpha Adjust", blush1NewAlpha);
            clothing2.SetFloatParamValue("Alpha Adjust", blush2NewAlpha);
        }
        public void SetMinBlush(float minBlush) { 
            this.minBlush = minBlush;
            //if (minBlush < Delerp(clothing1AlphaNoBlush, clothing1AlphaFullBlush, clothing1.GetFloatParamValue("Alpha Adjust"))) {
            if (isDeblushing)
            {
                character.main.RemoveFixedDeltaTimeConsumer(DeBlushUpdate);
                isDeblushing = false;
            }
            if (isBlushed) {
                character.main.RemoveFixedDeltaTimeConsumer(BlushFixedUpdate);
                isBlushed = false;
            }
           
                
            //}
            
        }
        public void BlushFixedUpdate(float deltaTime)
        {
            float blush1NewAlpha = linearInterpolate(clothing1BlushAlphaInterpolationStart, clothing1AlphaFullBlush, blushSpeed, deltaTime);
            float blush2NewAlpha = linearInterpolate(clothing2BlushAlphaInterpolationStart, clothing2AlphaFullBlush, blushSpeed, deltaTime);

            if (blush1NewAlpha == clothing1AlphaFullBlush && blush2NewAlpha == clothing2AlphaFullBlush)
            {
                character.main.RemoveFixedDeltaTimeConsumer(BlushFixedUpdate);
                glancePlugin.LoadPresetShy();
                //gazeManager.GazeDown();
                gazeAwayCoroutine = GazeAwayEnumerator();
                character.main.RunCoroutine(gazeAwayCoroutine);
                glanceAwayCoroutine = GlanceAwayEnumerator();
                character.main.RunCoroutine(glanceAwayCoroutine);
            }
            clothing1.SetFloatParamValue("Alpha Adjust", blush1NewAlpha);
            clothing2.SetFloatParamValue("Alpha Adjust", blush2NewAlpha);
        }
        private float linearInterpolate(float a, float b, float speed, float deltaTime)
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
        public static float SimpleLerp(float min, float max, float t)
        {
            return min + (max - min) * (t/100);
        }
        public static float Delerp(float min, float max, float value)
        {
            return (value - min) / (max - min) * 100;
        }

       
    }
}
