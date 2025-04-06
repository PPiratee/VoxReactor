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
            faceTimelinePlugin = new FaceTimelinePlugin(character);
            headTimeLine = new HeadTimeLine(character);
            readMyLipsPlugin = new ReadMyLipsPlugin(character);
            hjPlugin = new SilverHandJobPlugin(character.atom);
            bodyTimeline = new BodyTimeline(character.atom);
            bjPlugin = new SilverBlowJobPlugin(character);
            poseMePlugin = new PoseMePlugin(character);
        }
        public AcidGlancePlugin glancePlugin;
        public FaceTimelinePlugin faceTimelinePlugin;
        public ReadMyLipsPlugin readMyLipsPlugin;
        public SilverHandJobPlugin hjPlugin;
        public BodyTimeline bodyTimeline;
        public SilverBlowJobPlugin bjPlugin;
        public PoseMePlugin poseMePlugin;
        public HeadTimeLine headTimeLine;

    }
}
