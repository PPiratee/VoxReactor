using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using LeapInternal;
using System.Reflection.Emit;
using System.Linq;
using Leap.Unity;

namespace PPirate.VoxReactor
{
    internal class TouchManager
    {

        private Logger logger;


        private readonly VoxtaCharacter character;

        private readonly List<BodyPart> touchables = new List<BodyPart>();
        private readonly List<BodyPart> slapables = new List<BodyPart>();
        private readonly BodyPart partFace = new BodyPart("Face", "face");
        private readonly BodyPart partBreastL = new BodyPart("BreastL", "left breast");
        private readonly BodyPart partBreastR = new BodyPart("BreastR", "right breast");
        private readonly BodyPart partGluteL = new BodyPart("GluteL", "left ass cheek");
        private readonly BodyPart partGluteR = new BodyPart("GluteR", "right ass cheek");



        private List<string> currentlyTouching = new List<string>();

        private bool touchReactionsEnabled = true;//todo
        private float touchReactionsCooldown = 5f;
        private float touchReactionsPartCooldown = 60f;

        private bool slapEnabled = true;
        private float slapCoolDown;

        private Dictionary<string, float> partToTimeReactedMap = new Dictionary<string, float>();

        System.Random random = new System.Random();
        List<string> voiceLinesTouchPos = new List<string> {
            "mm...", "...mm", "ooh...", "hmm", "mmMm", "oo?", "ooh", "UH!"
        };
        List<string> voiceLinesSlap = new List<string> {
            "ouch", "awh!", "ooh!", "ahh!!", "ahhh!", 
        };
        public string GetRandomItem(List<string> list) {
            int randomIndex = random.Next(list.Count);
            string randomItem = list[randomIndex];
            return randomItem;
        }

        public TouchManager(VoxtaCharacter character)
        {
            logger = new Logger("TouchManager:Char#" + character.characterNumber);
            logger.Constructor();
            this.character = character;


            touchables.Add(partBreastL, partBreastR, partGluteL, partGluteR);
            slapables.Add(partBreastL, partBreastR, partGluteL, partGluteR);
            slapables.Add(partFace);


            touchables.ForEach(bodyPart => {
                character.main.RegisterAction(new JSONStorableAction($"OnTouchingStartChar{character.characterNumber}{bodyPart.part}", () => TouchingStartedCallback(bodyPart)));
            });
            touchables.ForEach(bodyPart => {
                character.main.RegisterAction(new JSONStorableAction($"OnTouchingStopChar{character.characterNumber}{bodyPart.part}", () => TouchingStoppedCallback(bodyPart)));
            });
            slapables.ForEach(bodyPart => {
                character.main.RegisterAction(new JSONStorableAction($"OnSlapChar{character.characterNumber}{bodyPart.part}", () => SlapCallback(bodyPart)));
            });

            SetUpUI();
        }
        public void TouchingStartedCallback(BodyPart bodyPart) {

            string part = bodyPart.readablePart;
            if (!touchReactionsEnabled)
            {
                return;
            }
            touchReactionsEnabled = false;
            character.main.RunCoroutine(TouchReactionsEnumerator());

            logger.DEBUG("TouchManager - started touching: " + part);
            if (currentlyTouching.Contains(part))
            {
                logger.DEBUG("TouchManager - already touching: " + part);
                return;
            }

            currentlyTouching.Add(part);
           // character.voxtaService.RequestCharacterSpeech(GetRandomItem(voiceLinesTouchPos));todo make optional

            if (!TouchReactionEvent(part))
                character.voxtaService.SendSecret($"{character.voxtaService.userName} started touching {character.name}'s: " + part);



            UpdateVoxtaContext();
            logger.DEBUG("TouchManager - Done");
        }
        public void TouchingStoppedCallback(BodyPart bodyPart)
        {
            string part = bodyPart.readablePart;

            logger.DEBUG("TouchManager - stopped touching: " + part);
            if (currentlyTouching.Contains(part))
            {
                currentlyTouching.Remove(part);
                UpdateVoxtaContext();
                character.voxtaService.SendSecret($"{character.voxtaService.userName} stopped touching {character.name}'s: " + part);
            }
        }
        public void SlapCallback(BodyPart bodyPart)
        {
            if (!slapEnabled)
                return;
            slapEnabled = false;

            character.main.RunCoroutine(EnableSlapCoroutineEnumerator());
            character.voxtaService.RequestCharacterSpeech(GetRandomItem(voiceLinesSlap));

            string part = bodyPart.readablePart;
            string eventToSend = $"{character.voxtaService.userName} just slapped {character.name}'s {part}";
            character.voxtaService.SendEventNow(eventToSend);
        }

    
        private bool TouchReactionEvent(string part) {
            bool needsReaction = NeedsReaction(part);
            if (!needsReaction) { 
                logger.DEBUG("TouchManager - Does not need reaction event");

                return false;
            }

            character.voxtaService.SendEventNow($"{character.voxtaService.userName} started touching {character.name}'s: " + part);
            return true;
        }
        private bool NeedsReaction(string part) {
            if (!isTouchReactionsEnabled.val) {
                return false;
            }
            bool timeExceeded = true;
            string otherPart = GetOther(part);
            if (partToTimeReactedMap.ContainsKey(part))
            {
                timeExceeded &= TimeExceeded(part);
            }
            if (partToTimeReactedMap.ContainsKey(otherPart))
            {
                timeExceeded &= TimeExceeded(otherPart);
            }

            partToTimeReactedMap[part] = Time.time;
            partToTimeReactedMap[otherPart] = Time.time;

            return timeExceeded;
        }

        IEnumerator EnableSlapCoroutineEnumerator()
        {
            yield return new WaitForSeconds(0.5f);
            slapEnabled = true;

        }
        IEnumerator DelayEnumerator(Action action)
        {
            yield return new WaitForSeconds(0.3f);
            action.Invoke();

        }

        private bool TimeExceeded(string part) {
            return Time.time - partToTimeReactedMap[part] > touchReactionsPartCooldown;
        }

        IEnumerator TouchReactionsEnumerator()
        {
            yield return new WaitForSeconds(touchReactionsCooldown);
            touchReactionsEnabled = true;

        }
        
        
        
        string lastContextItem = null;
        public void UpdateVoxtaContext() {
            logger.DEBUG("TouchManager - UpdateVoxtaContext");
            if (lastContextItem != null)
            {
                character.voxtaService.voxtaContextService.RemoveContextItem(lastContextItem);
            }
            if (currentlyTouching.Count == 0) { 
                lastContextItem = null;
                return;
            }
            string newContextItem = $"{character.voxtaService.userName} is currently touching {character.name}'s ";
            foreach(string part in EnrichCurrentlyTouching())
            {
                newContextItem += part + ", ";
            }
            character.voxtaService.voxtaContextService.AddContextItem(newContextItem);
            lastContextItem = newContextItem;
            logger.DEBUG("TouchManager - UpdateVoxtaContext DONE");
        }
        public List<string> EnrichCurrentlyTouching() { 
            List<string> enrichment = new List<string>();
            List<string> dontAdd = new List<string>();

            foreach (string part in currentlyTouching)
            {
                string reciporcalPart = GetOther(part);
                if (currentlyTouching.Contains(reciporcalPart)) {
                    enrichment.Add(GetBoth(part));
                    dontAdd.Add(reciporcalPart);
                    dontAdd.Add(part);
                    break;

                }
                enrichment.Add(part);
            }
            List<string> enrichment2 = new List<string>();

            foreach (string part in enrichment)
            {
                if (!dontAdd.Contains(part)) { 
                    enrichment2.Add(part);
                }
            }


            return enrichment2;
        }
        public string GetOther(string part) {
            string[] words = part.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string other = words[1];
            if (words.Contains("left"))
            {
                return "right " + other;
            } else if (words.Contains("right"))
            {
                return "left " + other;
            }

            return null;
        }
        public string GetBoth(string part)
        {
            string[] words = part.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string other = words[1];
            return "both " + other + "s";
        }

        internal class BodyPart { 
            public readonly string part;
            public readonly string readablePart;
            public BodyPart(string part, string readablePart)
            {
                this.part = part;
                this.readablePart = readablePart;
            }

        }
        JSONStorableBool isTouchReactionsEnabled;
        JSONStorableBool isSlapReactionsEnabled;

        private void SetUpUI() {
            isTouchReactionsEnabled = new JSONStorableBool("Touch reactions", true);
            //character.main.CreateToggle(isTouchReactionsEnabled, true);
            isSlapReactionsEnabled = new JSONStorableBool("Slap reactions", true);
           // character.main.CreateToggle(isSlapReactionsEnabled, true);
        }
    }
   
}
