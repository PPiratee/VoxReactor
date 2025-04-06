using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using PPirate.VoxReactor;

namespace PPirate.VoxReactor
{
    internal class BodyTimeline
    {
        private readonly string timelineStorableID = "5_VamTimeline.AtomPlugin";
        private readonly JSONStorable timelinePlugin;

        public BodyTimeline(Atom atom) { 
            timelinePlugin = AtomUtils.GetReciever(atom, timelineStorableID);
        }
        public void PlayShakeTits() {
            timelinePlugin.CallAction("Play shake");
        }
        public void PlayShakeIdle()
        {
            timelinePlugin.CallAction("Play idle");
        }
        public void PlayShowTitsDown()
        {
            timelinePlugin.CallAction("Play showTitsDown");
        }
        public void PlayTakeOffDress()
        {
            timelinePlugin.CallAction("Play armsTakeoffDress");
        }
        public void PlayArmsDownHj()
        {
            timelinePlugin.CallAction("Play armsDownHj");
        }
    }
}
