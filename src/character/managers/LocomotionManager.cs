

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
        Atom translationControl;
        
        VoxtaCharacter character;
        ClsPlugin clsPlugin;

        FreeControllerV3 hipController;

        Atom testTarg;

        Atom rotationControl;
        Atom rotationTarget;

        private bool shouldRotateToTarget = true;
        private float rotateDeadZone = 30f;
        private float rotateCheckInterval = 0.5f; //seconds

        IntervalCoroutine rotateCheck;

        public LocomotionManager(VoxtaCharacter character) { 
            this.character = character;
            translationControl = Main.singleton.GetAtomById(string.Format("char{0}_Locomotion_Translation", character.characterNumber));
            rotationControl = Main.singleton.GetAtomById(string.Format("char{0}_Locomotion_Rotation", character.characterNumber));
            clsPlugin = character.plugins.clsPlugin;
            var personControllers = character.atom.freeControllers;
            hipController = Array.Find(personControllers, c => c.name == "hipControl");



            testTarg = Main.singleton.GetAtomById("test_targ_r");//todo this is temporary
            rotationTarget = testTarg;
            SuperController.LogError("loc" + rotationTarget.mainController.control.position);
 


            rotateCheck = new IntervalCoroutine(rotateCheckInterval, CheckShouldRotateToTarget);
            rotateCheck.Run();


        }

        public void ToggleRotateToTarget(bool val) {
            //todo check for target

            if (val && !shouldRotateToTarget)
            {
                rotateCheck.Run();
            }
            else if(!val && shouldRotateToTarget)
            {
                rotateCheck.Stop();
            }
                

        }
        public void CheckShouldRotateToTarget() {
            //todo check for target
            //SuperController.LogError("CheckShouldRotateToTarget");

            Vector3 targetPosition = rotationTarget.mainController.control.position; // TODO: get actual value

            Vector3 hipPosition = hipController.control.position;
            Vector3 hipForward = hipController.control.forward; // TODO: get actual value

            // Direction from hips to target
            Vector3 hipsToTarget = targetPosition - hipPosition;

            // Project onto XZ plane (plane normal to Vector3.up) and convert to Vector2
            Vector3 hipsToTargetProjected = Vector3.ProjectOnPlane(hipsToTarget, Vector3.up);
            Vector2 hipsToTarget2 = new Vector2(hipsToTargetProjected.x, hipsToTargetProjected.z);

            // Project onto XZ plane and convert to Vector2
            Vector3 hipForwardProjected = Vector3.ProjectOnPlane(hipForward, Vector3.up);
            Vector2 hipForward2 = new Vector2(hipForwardProjected.x, hipForwardProjected.z);

            // Calculate signed angle between the two vectors in degrees
            float delta = Vector2.SignedAngle(hipForward2.normalized, hipsToTarget2.normalized);

            //// Use absolute value if you only care about magnitude
            if (Mathf.Abs(delta) >= rotateDeadZone)
            {
                UpdateRotationTargetPostion();
            }
            else
            {
                //disable rotaiton
                // nah do nothing
            }



        }
        private void UpdateRotationTargetPostion() {
            try
            {
                //SuperController.LogError((rotationControl.mainController == null).ToString());
                rotationControl.mainController.SetPositionNoForce(rotationTarget.mainController.control.position);

            }
            catch (Exception e) {
                //SuperController.LogError(e.Message);
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
