using System.Collections.Generic;
using UnityEngine;
using CheesyFX;
using MacGruber;

namespace PPirate.VoxReactor
{
    internal class PannelHandJobSettings : UIPannel
    {

        private readonly UITabBar tabBar;
        private readonly List<UIPannel> subPannels;

        private readonly ConfigHandJob hjConfig;

        public PannelHandJobSettings(UIPannel parentPannel, ConfigHandJob hjConfig) : base("HandJobSettings", parentPannel)
        {
            this.hjConfig = hjConfig;

            tabBar = new UITabBar("<PannelHandJobSettings/TabBar>", mvrScript, leftUIElements, rightUIElements, null, false);
            subPannels = new List<UIPannel>()
            {
               new PannelDirtyTalkGlobal(this, hjConfig.dirtyTalkConfig)
            };
        }
    


        public override void DrawPannelUI()
        {
            ClearPannelUI();
            MakeBackButton();

            var enabledToggle = (UIDynamicToggle)hjConfig.handJobEnabled.CreateUI(uiElements, false);
            enabledToggle.label = "Hand Job Enabled";

            tabBar.CreateTabs(subPannels);




        }



    }
}
