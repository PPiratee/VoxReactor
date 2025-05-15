using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace PPirate.VoxReactor
{
    internal class SexManager : SafeMvr
    {
        VoxtaCharacter character;
        

        //todo make service!! not on character
        public JSONStorableAction OnUserCum;

        public ReadMyLipsPlugin readMyLipsPlugin;
        public StimulationManager stimulationManager;

        //HAND JOB
        public HandJobManager handJobManager;
        private readonly String VOX_ACTION_HANDJOB = "handjob";
        private readonly String VOX_ACTION_HANDJOB_STOP = "handjob_stop";
        private readonly String VOX_ACTION_HANDJOB_ACCEPT = "handjob_accept";

        private readonly String VOX_ACTION_FASTER = "faster";
        private readonly String VOX_ACTION_SLOWER = "slower";
        private readonly String VOX_ACTION_TIP = "tip";
        private readonly String VOX_ACTION_SHAFT = "shaft";

        //BLOW JOB
        public readonly BlowJobManager blowjobManager;
        private readonly String VOX_ACTION_BLOWJOB = "blowjob";
        private readonly String VOX_ACTION_BLOWJOB_ACCEPT = "blowjob_accept";
        private readonly String VOX_ACTION_BLOWJOB_STOP = "blowjob_stop";

        public SexManager(VoxtaCharacter character) {
            this.character = character;
            handJobManager = new HandJobManager(character);
            blowjobManager = new BlowJobManager(character);
            readMyLipsPlugin = character.plugins.readMyLipsPlugin;
            stimulationManager = new StimulationManager(character);
            AddChild(stimulationManager);

            OnUserCum = new JSONStorableAction("OnUserCum", OnUserCumCallback);//todo should be service
            character.main.RegisterAction(OnUserCum);

            character.actionObserverRegistry.RegisterObserver("user_cums", OnUserCumCallback);
            character.actionObserverRegistry.RegisterObserver("user_boner", OnUserBonerCallback);

            //bonerRelay = AtomUtils.GetReciever(character.main.GetAtomById("UIButtonHard"), "EnabledRelay");

            //HJ callbacks
            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_HANDJOB, OnHandJob);
            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_HANDJOB_ACCEPT, OnHandJob);
            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_HANDJOB_STOP, handJobManager.ActionHandjobStop);

            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_FASTER, handJobManager.ActionFaster);
            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_SLOWER, handJobManager.ActionSlower);
            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_TIP, handJobManager.ActionTip);
            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_SHAFT, handJobManager.ActionShaft);

            //BJ callbacks
            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_BLOWJOB, OnBlowJob);
            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_BLOWJOB_ACCEPT, OnBlowJob);
            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_BLOWJOB_STOP, OnBlowJobStop);
        }

        private void OnHandJob() {
            if (blowjobManager.isGivingBj || blowjobManager.isPausingBj)
            {
                handJobManager.ActionHandjob(false);
            }
            else {
                handJobManager.ActionHandjob(true);
            }
        }

        private void OnBlowJob()
        {
            blowjobManager.ActionBlowJobStart();
            handJobManager.ActionBlowjobStart();
        }

        private void OnBlowJobStop()
        {
            blowjobManager.ActionBlowJobStop();
            handJobManager.ActionBlowjobStop();
        }


        private void OnUserCumCallback() {
            //user_cums
            readMyLipsPlugin.OrgasmDildosNow();
            //todo if giving blowjob, or inserted in oriface,
            //otherwise
            character.voxtaService.SendEventNow("{{user}} just ejaculated on her chest and face.");
        }
        JSONStorable bonerRelay;
        private void OnUserBonerCallback()
        {
            //user_cums
            //readMyLipsPlugin.OrgasmDildosNow();
            //todo if giving blowjob, or inserted in oriface,
            //otherwise
            ;
            //bonerRelay.CallAction("Trigger");
            character.voxtaService.SendEventNow("{{user}} just got a boner.");
        }


    }
}
