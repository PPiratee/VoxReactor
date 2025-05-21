using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace PPirate.VoxReactor
{
    internal class NavigationManager : SafeMvr
    {

        private readonly VoxtaCharacter character;
        private readonly LocomotionManager locomotionManager;


        Atom playerHipAtom;
        public NavigationManager(VoxtaCharacter character)
        {
            this.character = character;
            playerHipAtom = Main.singleton.GetAtomById("playerHipPosition");
            locomotionManager = new LocomotionManager(character);
            AddChild(locomotionManager);

            ToggleFollowPlayer(true);
        }
        public bool isFollowing = false;
        public void FollowAtom(Atom atomToFollow) {
            locomotionManager.SetRotationTarget(atomToFollow);
            locomotionManager.SetTranslationTarget(atomToFollow);
            locomotionManager.ToggleFollowRotation(true);
            locomotionManager.ToggleFollowTranslation(true);
        }
        public void StopFollowAtom() {
            locomotionManager.ToggleFollowRotation(false);
            locomotionManager.ToggleFollowTranslation(false);
        }
        public void ToggleFollowPlayer(bool val)
        {
            //playerHipPosition
            if (val)
            {
                FollowAtom(playerHipAtom);

            }
            else { 
                StopFollowAtom(); 
            }

        }
    }
}
