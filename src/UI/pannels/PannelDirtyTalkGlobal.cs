using System.Collections.Generic;
using UnityEngine;
using CheesyFX;
using MacGruber;

namespace PPirate.VoxReactor
{
    internal class PannelDirtyTalkGlobal : PannelDirtyTalk
    {


        public PannelDirtyTalkGlobal(UIPannel parentPannel, ConfigDirtyTalk config) : base(parentPannel, config)
        {
        }

        public override void DrawPannelUI()
        {
            base.DrawPannelUI();

        }
        public override void SetUpEnabledInfo() {
            string info = "Enabled Toggle: global configuration which determines if any character may talk dirty during the sexy sex";
            info.CreateStaticInfo(120, uiElements, false);
        }

        public override void SetUpLinesInfo()
        {
            string info = PannelDirtyTalk.linesLabelDefault;
            info.CreateStaticInfo(120, uiElements, false);
        }
    }
}
