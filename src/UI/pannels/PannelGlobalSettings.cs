using System.Collections.Generic;
using CheesyFX;

namespace PPirate.VoxReactor
{
    internal class PannelGlobalSettings : UIPannel
    {
        private readonly UITabBar mainTabBar;
        private readonly List<UIPannel> subPannels;

        public PannelGlobalSettings(UIPannel parentPannel) : base("Global Settings",  parentPannel) {
            mainTabBar = new UITabBar("<PannelGlobalSettings/TabBar>", mvrScript, leftUIElements, rightUIElements, null, false);
            subPannels = new List<UIPannel>
            {
                new PannelDirtyTalkGlobal(this, ConfigVoxReactor.singeton.globalDirtyTalkConfig),
                new PannelHandJobSettings(this, ConfigVoxReactor.singeton.globalHandJobConfig)
            };
        }
        public override void DrawPannelUI()
        {
            ClearPannelUI();
            MakeBackButton();

            mainTabBar.CreateTabs(subPannels);
        }
    }
}
