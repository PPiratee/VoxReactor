using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using LeapInternal;
using System.Linq;

namespace PPirate.VoxReactor
{
    internal class HandJobManager
    {
        private Logger logger;

        VoxtaCharacter character;
        SilverHandJobPlugin hjPlugin;
        ConfigHandJob globalHandjobConfig;


        private readonly String VOX_ACTION_HANDJOB = "handjob";
        private readonly String VOX_ACTION_HANDJOB_STOP = "handjob_stop";
        private readonly String VOX_ACTION_FASTER = "faster";
        private readonly String VOX_ACTION_SLOWER = "slower";
        private readonly String VOX_ACTION_TIP = "tip";
        private readonly String VOX_ACTION_SHAFT = "shaft";


        public HandJobManager(VoxtaCharacter character) {
            logger = new Logger("HandJobManager:Char#" + character.characterNumber);
            logger.Constructor();
            this.character = character;
            hjPlugin = character.plugins.hjPlugin;

            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_HANDJOB, ActionHandjob);
            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_HANDJOB_STOP, ActionHandjobStop);
            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_FASTER, Faster);
            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_SLOWER, Slower);
            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_TIP, Tip);
            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_SHAFT, Shaft);

            this.globalHandjobConfig = ConfigVoxReactor.singeton.globalHandJobConfig;
        }

        string contextItem = null;
        private void ActionHandjob()
        {
            logger.StartMethod("ActionHandjob()");
            string newContextItem = $"{character.name} is giving {character.voxtaService.userName} a hand job.";
            contextItem = newContextItem;
            character.voxtaService.voxtaContextService.AddContextItem(newContextItem);

            currentSpeed = SPEED_MEDIUM;
            UpdateSpeed();
            hjPlugin.SetIsActive(true);
            //play hj anim
            //character.plugins.bodyTimeline.PlayArmsDownHj();
            character.dirtyTalkManager.StartDirtyTalk(GetDirtyTalkLines());
        }
        public List<string> GetDirtyTalkLines() {
            //todo overriding/extending logic n shit
            //currently just extending

            var rv = AtomUtils.ConcatList( new List<string>(),
                    DirtyTalkManager.ParseDirtyTalkLines(globalHandjobConfig.dirtyTalkConfig.dirtyTalkLines.val)
              );
            rv = AtomUtils.ConcatList(rv, DirtyTalkManager.globalDirtyTalkLines);



            return rv;
        }
        private void ActionHandjobStop()
        {
            hjPlugin.SetIsActive(false);
            character.voxtaService.voxtaContextService.RemoveContextItem(contextItem);
            character.dirtyTalkManager.StopDirtyTalk();

        }
        private void Tip() {
            TipAdjust(1f);
        }
        private void Shaft()
        {
            TipAdjust(0f);

        }
        private void TipAdjust(float chance) { 
            hjPlugin.SetTopOnlyChance(chance);
        }
        private void Faster() {
            if (currentSpeed == SPEED_FAST) {
                return;
            } else if (currentSpeed == SPEED_SLOW) {
                currentSpeed = SPEED_MEDIUM;
            }
            else if (currentSpeed == SPEED_MEDIUM){
                currentSpeed = SPEED_FAST;
            }
            UpdateSpeed();
        }
        private void Slower() {
            if (currentSpeed == SPEED_SLOW)
            {
                return;
            }
            else if (currentSpeed == SPEED_MEDIUM)
            {
                currentSpeed = SPEED_SLOW;
            }
            else if (currentSpeed == SPEED_FAST)
            {
                currentSpeed = SPEED_MEDIUM;
            }
            UpdateSpeed();
        }
        private string currentSpeed = "medium";
        private readonly string SPEED_SLOW = "slow";
        private readonly string SPEED_MEDIUM = "medium";
        private readonly string SPEED_FAST = "fast";

        private readonly HjSpeed HJ_SPEEDS_SLOW = new HjSpeed(0.36f, 3.0f, 0.21f);
        private readonly HjSpeed HJ_SPEEDS_MEDIUM = new HjSpeed(1.05f, 4.8f, 0.88f);
        private readonly HjSpeed HJ_SPEEDS_FAST = new HjSpeed(1.7f, 6.3f, 1.50f);



        private void UpdateSpeed() {
            HjSpeed curretSpeedData = HJ_SPEEDS_MEDIUM;
            if (currentSpeed == SPEED_SLOW) {
                curretSpeedData =  HJ_SPEEDS_SLOW;
            } else if (currentSpeed == SPEED_FAST) {
                curretSpeedData = HJ_SPEEDS_FAST;

            }
            hjPlugin.SetSpeedMin(curretSpeedData.min);
            hjPlugin.SetSpeedMax(curretSpeedData.max);
            hjPlugin.SetOverallSpeed(curretSpeedData.overall);
        }
    }
    internal class HjSpeed {
        public readonly float min;
        public readonly float max;
        public readonly float overall;
        public HjSpeed(float min, float max, float overall) { 
            this.min = min;
            this.max = max;
            this.overall = overall;
        }
    }
}
