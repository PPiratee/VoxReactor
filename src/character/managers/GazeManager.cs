using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace PPirate.VoxReactor
{
    internal class GazeManager
    {

        private readonly VoxtaCharacter character;
        private readonly PoseMePlugin poseMePlugin;

        public GazeManager(VoxtaCharacter character)
        {
            this.character = character;

            poseMePlugin = character.plugins.poseMePlugin;
        }
        public void Defaults() {
           
            //gazer.SetMaxAngleVertical(70f);
            //gazer.SetLookAtPositionOffset(0f);
            //gazer.SetEyeAngleVerticalOffset(-4f);
        }
        public void GazeDown()
        {
            //gazer.SetMaxAngleVertical(70f);
            //gazer.SetLookAtPositionOffset(-0.9f);
            //gazer.SetEyeAngleVerticalOffset(-12f);
        }
        public void LookAtAtom(string atomName) { 
            //gazer.SetLookAtAtom(atomName);
        }
        public void LookAtPlayer() {
            poseMePlugin.LookAtPlayer();
        }
        public void SetEnabled(bool val) {
            //gazer.SetEnabled(val);
        }
    }
}
