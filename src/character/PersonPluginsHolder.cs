using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;


namespace PPirate.VoxReactor
{
    internal class PersonPluginsHolder
    {
        public PersonPluginsHolder(VoxtaCharacter character)
        {
            glancePlugin = new AcidGlancePlugin(character.atom);
            headTimeLine = new HeadTimeLine(character);
            readMyLipsPlugin = new ReadMyLipsPlugin(character);
            hjPlugin = new SilverHandJobPlugin(character.atom);
            bodyTimeline = new BodyTimeline(character.atom);
            bjPlugin = new SilverBlowJobPlugin(character);
            poseMePlugin = new PoseMePlugin(character);
            expressionRouterPlugin = new ExpressionRouterPlugin(character);
            clsPlugin = new ClsPlugin(character);
        }
        public AcidGlancePlugin glancePlugin;
        public ReadMyLipsPlugin readMyLipsPlugin;
        public SilverHandJobPlugin hjPlugin;
        public BodyTimeline bodyTimeline;
        public SilverBlowJobPlugin bjPlugin;
        public PoseMePlugin poseMePlugin;
        public HeadTimeLine headTimeLine;
        public ExpressionRouterPlugin expressionRouterPlugin;
        public ClsPlugin clsPlugin;

    }
}
