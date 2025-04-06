using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace PPirate.VoxReactor
{
    internal class FaceTimelinePlugin
    {


        private readonly String timelineStorableID = "0_VamTimeline.AtomPlugin";
        private readonly JSONStorable timelinePlugin;

        public FaceTimelinePlugin(VoxtaCharacter character)
        {
           
            timelinePlugin = AtomUtils.GetReciever(character.atom, timelineStorableID);

        }
        public void PlaySmile()
        {
            timelinePlugin.CallAction("Play Smile/*");
        }

        public void PlayStartHorny()
        {
            timelinePlugin.CallAction("Play start-Flirty");
        }
        public void PlayNervous()
        {
            timelinePlugin.CallAction("Play Face Nervous");
        }
    }
}
