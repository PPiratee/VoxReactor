using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;


namespace PPirate.VoxReactor
{
    internal class HeadTimeLine
    {

        private readonly String timelineStorableID = "10_VamTimeline.AtomPlugin";
        private readonly JSONStorable timelinePlugin;

        public HeadTimeLine(VoxtaCharacter character)
        {

            timelinePlugin = AtomUtils.GetReciever(character.atom, timelineStorableID);

        }
        public void Talk()
        {
            timelinePlugin.CallAction("Play talking/*");
        }
        public void StopTalk()
        {
            timelinePlugin.CallAction("Play idle");
        }


    }
}
