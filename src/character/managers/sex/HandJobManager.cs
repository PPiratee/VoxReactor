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

        public bool isGivingHj = false;
        public HandJobManager(VoxtaCharacter character) {
            logger = new Logger("HandJobManager:Char#" + character.characterNumber);
            logger.Constructor();
            this.character = character;
            hjPlugin = character.plugins.hjPlugin;

            
            this.globalHandjobConfig = ConfigVoxReactor.singeton.globalHandJobConfig;
        }

        string contextItem = null;
        bool tipFocus = false;
        public void ActionHandjob(bool shouldDirtyTalk)
        {
            logger.StartMethod("ActionHandjob()");
            currentSpeed = SPEED_MEDIUM;
            tipFocus = false;

            SetContextItem();

            UpdateSpeed();
            hjPlugin.SetIsActive(true);
            isGivingHj = true;
            //play hj anim
            //character.plugins.bodyTimeline.PlayArmsDownHj();

            if (shouldDirtyTalk)
                character.dirtyTalkManager.StartDirtyTalk(GetDirtyTalkLines());
        }

        public void ActionBlowjobStart() {
            if(isGivingHj)
                character.dirtyTalkManager.StopDirtyTalk();
        }

        public void ActionBlowjobStop()
        {
            if (isGivingHj)
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
        public void ActionHandjobStop()
        {
            isGivingHj = false;
            hjPlugin.SetIsActive(false);
            character.voxtaService.voxtaContextService.RemoveContextItem(contextItem);
            character.dirtyTalkManager.StopDirtyTalk();
        }

        public void ActionTip() {
            TipAdjust(1f);
            tipFocus = true;
            SetContextItem();
        }

        public void ActionShaft()
        {
            TipAdjust(0f);
            tipFocus = false;
            SetContextItem();
        }

        private void TipAdjust(float chance) { 
            hjPlugin.SetTopOnlyChance(chance);
        }

        public void ActionFaster() {
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

        public void ActionSlower() {
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
        private void SetContextItem() {
            string newContextItem = "";
            if (currentSpeed == SPEED_SLOW)
            {
                if (tipFocus)
                {
                    newContextItem = $"{character.name} is slowly stroking the tip of {character.voxtaService.userName}'s cock.";
                }
                else { 
                    newContextItem = $"{character.name} is slowly stroking {character.voxtaService.userName}'s cock.";
                }
            }
            else if (currentSpeed == SPEED_MEDIUM)
            {
                if (tipFocus)
                {
                    newContextItem = $"{character.name} is stroking the tip of {character.voxtaService.userName}'s cock at a medium pace.";
                }
                else
                {
                    newContextItem = $"{character.name} is stroking {character.voxtaService.userName}'s cock at a medium pace.";
                }
                
            }
            else 
            {
                if (tipFocus)
                {
                    newContextItem = $"{character.name} is vigorusly stroking the tip of {character.voxtaService.userName}'s cock.";
                }
                else
                {
                    newContextItem = $"{character.name} is vigorusly stroking {character.voxtaService.userName}'s cock.";
                }
                
            }


            if (contextItem != null)
                character.voxtaService.voxtaContextService.RemoveContextItem(contextItem);
            character.voxtaService.voxtaContextService.AddContextItem(newContextItem);
            contextItem = newContextItem;
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
            SetContextItem();
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
