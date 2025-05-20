

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using static System.Collections.Specialized.BitVector32;
using LeapInternal;
using Leap.Unity.Query;
using System.Linq;
using System.CodeDom;
using Leap;
namespace PPirate.VoxReactor
{
    internal class LocomotionManager : SafeMvr
    {
        
        private readonly VoxtaCharacter character;
        private readonly ClsPlugin clsPlugin;

        private readonly FreeControllerV3 hipController;

        
        //TRANSLATION

        private readonly Atom translationControl;
        private Atom transationTarget;
        private Atom translationTestTarg;//TEST
        private bool isFollowTargetTranslation = true;

        IntervalCoroutine translateCheck;
        private float followTranslationDistance = 1f;

        //ROTATION
        private readonly Atom rotationControl;
        private Atom rotationTarget;
        private Atom rotationTestTarg;//TEST

        private bool isFollowTargetRotation = true;

        IntervalCoroutine rotateCheck;
        private float followRotateDeadZone = 30f;



        private float followInterval = 0.5f; //seconds


        public LocomotionManager(VoxtaCharacter character) { 
            this.character = character;
            translationControl = Main.singleton.GetAtomById(string.Format("char{0}_Locomotion_Translation", character.characterNumber));
            rotationControl = Main.singleton.GetAtomById(string.Format("char{0}_Locomotion_Rotation", character.characterNumber));
            clsPlugin = character.plugins.clsPlugin;
            var personControllers = character.atom.freeControllers;
            hipController = Array.Find(personControllers, c => c.name == "hipControl");



            rotationTestTarg = Main.singleton.GetAtomById("test_targ_r");//todo this is temporary 
            rotationTarget = rotationTestTarg;//temp
            //SuperController.LogError("loc" + rotationTarget.mainController.control.position);

            translationTestTarg = Main.singleton.GetAtomById("test_targ_t");//todo temp
            transationTarget = translationTestTarg;//temp


            rotateCheck = new IntervalCoroutine(followInterval, CheckShouldRotateToTarget);
            rotateCheck.Run();//temp

            translateCheck = new IntervalCoroutine(followInterval, CheckShouldFollow);
            translateCheck.Run();//temp

        }

        public void ToggleFollowTranslation(bool val) {
            //todo check for target

            if (val && !isFollowTargetRotation)
            {
                rotateCheck.Run();
            }
            else if(!val && isFollowTargetRotation)
            {
                rotateCheck.Stop();
            }
        }

        public void ToggleFollowRotation(bool val)
        {
            //todo check for target

            if (val && !isFollowTargetTranslation)
            {
                translateCheck.Run();
            }
            else if (!val && isFollowTargetTranslation)
            {
                translateCheck.Stop();
            }
        }
        public void CheckShouldRotateToTarget() {
            //todo check for target
            //SuperController.LogError("CheckShouldRotateToTarget");

            Vector3 targetPosition = rotationTarget.mainController.control.position;

            Vector3 hipPosition = hipController.control.position;
            Vector3 hipForward = hipController.control.forward; 

            // Direction from hips to target
            Vector3 hipsToTarget = targetPosition - hipPosition;

            // Project onto XZ plane (plane normal to Vector3.up) and convert to Vector2
            Vector3 hipsToTargetProjected = Vector3.ProjectOnPlane(hipsToTarget, Vector3.up);
            Vector2 hipsToTarget2 = new Vector2(hipsToTargetProjected.x, hipsToTargetProjected.z);

            // Project onto XZ plane and convert to Vector2
            Vector3 hipForwardProjected = Vector3.ProjectOnPlane(hipForward, Vector3.up);
            Vector2 hipForward2 = new Vector2(hipForwardProjected.x, hipForwardProjected.z);


            float delta = Vector2.SignedAngle(hipForward2.normalized, hipsToTarget2.normalized);
            if (Mathf.Abs(delta) >= followRotateDeadZone)
            {
                UpdateRotationTargetPostion();
            }
        }
        public void CheckShouldFollow() {
            Vector3 targetPosition = transationTarget.mainController.control.position;
            Vector3 hipPosition = hipController.control.position;
            Vector3 hipsToTarget = targetPosition - hipPosition;
            float distance = (Vector3.ProjectOnPlane(hipsToTarget, Vector3.up).magnitude);
            SuperController.LogError("DistanceIs" + distance);
            if (distance >= followTranslationDistance) {
                UpdateTranslationTargetPostion();
            }
        }
        private void UpdateRotationTargetPostion() {
            try
            {

                rotationControl.mainController.SetPositionNoForce(rotationTarget.mainController.control.position);

            }
            catch (Exception e) {
                SuperController.LogError(e.Message);
            }
        }
        private void UpdateTranslationTargetPostion()
        {
            try
            {
                SuperController.LogError("Translating");

                translationControl.mainController.SetPositionNoForce(transationTarget.mainController.control.position);

            }
            catch (Exception e)
            {
                SuperController.LogError(e.Message);
            }
        }

        private class IntervalCoroutine
        {
            float interval;
            Action callback;
            IEnumerator enumerator;
            public IntervalCoroutine(float interval, Action callback)
            {
                this.callback = callback;
                this.interval = interval;
            }
            public void Run()
            {
                enumerator = GetEnumerator();
                Main.singleton.RunCoroutine(enumerator);
            }
            public void Stop()
            {
                Main.singleton.StopCoroutine(enumerator);
                enumerator = null;
            }
            IEnumerator GetEnumerator()
            {
                yield return new WaitForSeconds(interval);
                callback?.Invoke();
                Run();
            }
        }


    }
}
