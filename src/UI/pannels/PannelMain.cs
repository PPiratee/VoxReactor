using CheesyFX;
using Leap.Unity;
using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace PPirate.VoxReactor
{
    internal class PannelMain : UIPannel
    {
        Main main;
        private readonly UITabBar mainTabBar;
        private readonly List<UIPannel> subPannels;
        public readonly PannelCharacters characterPannel;

        public PannelMain(Main main, List<Transform> leftUIElements, List<Transform> rightUIElements) : base("Main Menu", main, leftUIElements, rightUIElements)
        {
            this.main = main;
            mainTabBar = new UITabBar("<Main/MainTabBar>", mvrScript, leftUIElements, rightUIElements, null, false);
            this.characterPannel = new PannelCharacters(this);
            subPannels = new List<UIPannel>
            {
                new PannelGlobalSettings(this),
                this.characterPannel,
               // new PannelDirtyTalkGlobal(this, ConfigVoxReactor.singeton.globalDirtyTalkConfig)
            };
        }
        public override void DrawPannelUI() {
            ClearPannelUI();

            mainTabBar.CreateTabs(subPannels);
        }
    }
}
