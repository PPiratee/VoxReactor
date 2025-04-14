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

        public HandJobManager handJobManager;
        public ReadMyLipsPlugin readMyLipsPlugin;
        public StimulationManager stimulationManager;
        public SexManager(VoxtaCharacter character) {
            this.character = character;
            handJobManager = new HandJobManager(character);
            readMyLipsPlugin = character.plugins.readMyLipsPlugin;
            stimulationManager = new StimulationManager(character);
            AddChild(stimulationManager);

            OnUserCum = new JSONStorableAction("OnUserCum", OnUserCumCallback);//todo should be service
            character.main.RegisterAction(OnUserCum);

            character.actionObserverRegistry.RegisterObserver("user_cums", OnUserCumCallback);
            character.actionObserverRegistry.RegisterObserver("user_boner", OnUserBonerCallback);

            //bonerRelay = AtomUtils.GetReciever(character.main.GetAtomById("UIButtonHard"), "EnabledRelay");
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
