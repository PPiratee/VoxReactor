using System.Collections.Generic;
using UnityEngine;
using CheesyFX;
using MacGruber;

namespace PPirate.VoxReactor
{
    internal abstract class PannelDirtyTalk : UIPannel
    {
        private readonly ConfigDirtyTalk config;
        //private readonly string enabledLabel;

        public PannelDirtyTalk(UIPannel parentPannel, ConfigDirtyTalk config) : base("DirtyTalk", parentPannel)
        {
            this.config = config;
        }
        public abstract void SetUpEnabledInfo();
        public abstract void SetUpLinesInfo();


        public static string linesLabelDefault = "Dirty Talk lines. This is a comma seperated list of randomly selected topics to talk dirty about.";
        public override void DrawPannelUI()
        {
            ClearPannelUI();
            MakeBackButton();

            var enabledToggle = (UIDynamicToggle)config.dirtyTalkEnabled.CreateUI(uiElements, false);
            enabledToggle.label = "Enable Dirty Talk";
            SetUpEnabledInfo();

            var lines = (UIDynamicLabelInput)config.dirtyTalkLines.CreateUI(uiElements, false);
            lines.label.text = "Dirty Talk Lines";
            lines.height = 500;
            SetUpLinesInfo();



        }
    }
}
