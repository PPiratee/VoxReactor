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

        JSONStorableBool toggleFollowPlayer;

        Atom playerHipAtom;
        public NavigationManager(VoxtaCharacter character)
        {
            this.character = character;
            playerHipAtom = Main.singleton.GetAtomById("playerHipPosition");
            locomotionManager = new LocomotionManager(character);
            AddChild(locomotionManager);

            //ToggleFollowPlayer(true);

            toggleFollowPlayer = new JSONStorableBool("ToggleFollowPlayer", false, ToggleFollowPlayer);
            Main.singleton.RegisterBool(toggleFollowPlayer);
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
                locomotionManager.ToggleShouldLookAtTranslationTargetWhenWalking(true);

            }
            else { 
                StopFollowAtom();
                locomotionManager.ToggleShouldLookAtTranslationTargetWhenWalking(false);
            }

        }
    }
}
