using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;


namespace PPirate.VoxReactor
{
    internal class EventShowTits
    {
        private readonly VoxtaCharacter character;

        private readonly String VOX_ACTION_SHOW_TITS = "show_tits";
        private readonly BodyTimeline timeline;

        private readonly String VOX_ACTION_DROP_DRESS = "drop_dress";
        public EventShowTits(VoxtaCharacter character) { 
            this.character = character;
            timeline = character.plugins.bodyTimeline;
            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_SHOW_TITS, ShowTits);
            character.actionObserverRegistry.RegisterObserver(VOX_ACTION_DROP_DRESS, DropDress);

            character.emotionManager.emotionObserverRegistry.RegisterObserver(EmotionManager.REGISTRY_ON_HORNY_CHANGED, SetCanShowTits);
        }

        public void SetCanShowTits() {
            if (character.emotionManager.GetHornyLevel() >= 50)
            {
                character.voxtaService.voxtaPlugin.SetFlag("canShowTits");
            }
            else {
                character.voxtaService.voxtaPlugin.SetFlag("!canShowTits");
            }
        }

        public void ShowTits()
        {
           // character.logger.LOG("EventShowTits - Showing tits");
            timeline.PlayShowTitsDown();
            //character.voxtaService.voxtaContextService.AddContextItem($"{character.name}'s top is pulled up, her tits are out.");
        }

        public void DropDress()
        {
           // character.logger.LOG("EventShowTits - DropDress");
            timeline.PlayTakeOffDress();
            //character.voxtaService.voxtaContextService.AddContextItem($"{character.name}'s top is pulled up, her tits are out.");
        }
    }
}
