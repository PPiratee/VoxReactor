using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine.UI;
using LeapInternal;

namespace PPirate.VoxReactor
{
    internal class DirtyTalkManager
    {
        private Logger logger;

        VoxtaCharacter character;
        /*
        private readonly float minDirtyTalkDelay = 5f;
        private readonly float maxDirtyTalkDelay = 5f;
        private readonly float dirtyTalkChance = 100f;
         */
         private readonly float minDirtyTalkDelay = 16.0f;
        private readonly float maxDirtyTalkDelay = 25.0f;
        private readonly float dirtyTalkChance = 70f;

        public JSONStorableAction OnDirtyTalk;
        JSONStorableString dirtyTalkConfig;
        public DirtyTalkManager(VoxtaCharacter character) {
            logger = new Logger("DirtyTalkManager:Char#" + character.characterNumber);

            this.character = character;

            OnDirtyTalk = new JSONStorableAction("OnDirtyTalk", DirtyTalkHelper);
            character.main.RegisterAction(OnDirtyTalk);
           // var labelField = character.main.CreateTextField(new JSONStorableString("DirtyTalkConfigLabel", "Dirty talk config. Seperate concepts to talk dirty about with commas."), true);

           // dirtyTalkConfig = new JSONStorableString("DirtyTalkConfig", "");
            //CreateTextInput(dirtyTalkConfig, true);
        }

        private static List<String> globalDirtyTalkLines = new List<String> 
        {
            
            "{{ char }}'s next reply SHALL be talking dirty about the situation.",
            "{{ char }}'s next reply SHALL be asking if he likes her sexually servicing him.",
            "{{ char }}'s next reply SHALL be dirty talk about how she is his slut.",
            "{{ char }}'s next reply SHALL pervy dirty talk about wanting to be used.",
            "{{ char }}'s next reply SHALL be dirty talk about how wet she is.",
            
           
        };
        private List<String> currentDirtyTalkLInes = globalDirtyTalkLines;
        private bool shouldDirtyTalk = false;
        public void StartDirtyTalk(List<String> lines) {
            shouldDirtyTalk = true;
            currentDirtyTalkLInes = new List<string>();
            foreach (String line in globalDirtyTalkLines)
            {
               // currentDirtyTalkLInes.Add(line);
            }
            foreach (String line in lines)
            {
                currentDirtyTalkLInes.Add(line);
            }
            logger.DEBUG("Starting Dirty talk");
            character.main.RunCoroutine(DirtyTalkEnumerator());
        }
        public void StopDirtyTalk() {
            shouldDirtyTalk = false;
        }
        IEnumerator DirtyTalkEnumerator()
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(minDirtyTalkDelay, maxDirtyTalkDelay));
            if(shouldDirtyTalk)
                DirtyTalk();
        }
        System.Random random = new System.Random();
        private void DirtyTalk() {
            if (UnityEngine.Random.Range(0, 100) <= dirtyTalkChance)
            {
                return;
            }
            DirtyTalkHelper();
            character.main.RunCoroutine(DirtyTalkEnumerator());
        }
        private void DirtyTalkHelper() {
            int randomIndex = random.Next(currentDirtyTalkLInes.Count);
            string randomItem = currentDirtyTalkLInes[randomIndex];
            //character.voxtaService.SendEventNow(randomItem);
            character.voxtaService.SendSecret(randomItem);
            character.voxtaService.SendEventNow(" ");
            
        }
        
        public UIDynamicTextField CreateTextInput(JSONStorableString jss, bool rightSide = false)
        {
            var textfield = character.main.CreateTextField(jss, rightSide);
            textfield.height = 20f;
            textfield.backgroundColor = Color.white;
            var input = textfield.gameObject.AddComponent<InputField>();
            var rect = input.GetComponent<RectTransform>().sizeDelta = new Vector2(1f, 0.4f);
            input.textComponent = textfield.UItext;
            jss.inputField = input;
            return textfield;
        }

    }
}
