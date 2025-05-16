using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine.UI;
using LeapInternal;
using System.Linq;

namespace PPirate.VoxReactor
{
    internal class DirtyTalkManager : SafeMvr
    {
        private Logger logger;

        VoxtaCharacter character;

        private readonly float minDirtyTalkDelay = 16.0f;
        private readonly float maxDirtyTalkDelay = 25.0f;
        private readonly float dirtyTalkChance = 80f;

        //private readonly float minDirtyTalkDelay = 5.0f;
        //private readonly float maxDirtyTalkDelay = 5.0f;
        //private readonly float dirtyTalkChance = 100f;

        public JSONStorableAction OnDirtyTalk;
        JSONStorableString dirtyTalkConfig;
        public DirtyTalkManager(VoxtaCharacter character) {
            logger = new Logger("DirtyTalkManager:Char#" + character.characterNumber);

            this.character = character;

            

            JSONStorableString globalLines = ConfigVoxReactor.singeton.globalDirtyTalkConfig.dirtyTalkLines;

            AddCallback(globalLines, (string lines) =>
            {
                OnGlobalDirtyTalkLinesChange(lines);
            });
            OnGlobalDirtyTalkLinesChange(globalLines.val);




            OnDirtyTalk = new JSONStorableAction("OnDirtyTalk", DirtyTalkHelper);
            character.main.RegisterAction(OnDirtyTalk);
            character.stateManager.observerRegistry.RegisterObserver(StateManager.REGISTRY_SPEAKING_START, PauseDirtyTalk);
            character.stateManager.observerRegistry.RegisterObserver(StateManager.REGISTRY_SPEAKING_STOP, ResumeDirtyTalk);
            VoxtaService.singleton.globalObserverRegistry.RegisterObserver(VoxtaService.REGISTRY_USER_SPEAKING, PauseDirtyTalk);
            VoxtaService.singleton.globalObserverRegistry.RegisterObserver(VoxtaService.REGISTRY_USER_SPEAKING_STOP, ResumeDirtyTalk);

        }
        public void OnGlobalDirtyTalkLinesChange(string lines) {

            DirtyTalkManager.globalDirtyTalkLines = ParseDirtyTalkLines(lines);
        }
        public static List<string> ParseDirtyTalkLines(string lines)
        {

            return lines
                .Split(',')
                .Select(s => lineBase + s.Trim())
                .ToList();
        }
        public static List<String> globalDirtyTalkLines;
        private static string lineBase = "{{ char }}'s next reply SHALL be ";


        private List<String> currentDirtyTalkLInes = new List<string>();
        private bool shouldDirtyTalk = false;

        IEnumerator currentDirtyTalkEnumerator;
        public void StartDirtyTalk(List<String> lines) {
            shouldDirtyTalk = true;
            currentDirtyTalkLInes = lines;
            logger.DEBUG("Starting Dirty talk");

            StartEnumerator();
        }
        public void StartEnumerator() {
            StopEnumerator();
            currentDirtyTalkEnumerator = DirtyTalkEnumerator();
            character.main.RunCoroutine(currentDirtyTalkEnumerator);
        }
        public void StopDirtyTalk() {
            shouldDirtyTalk = false;
            currentDirtyTalkLInes.Clear();
            StopEnumerator();
        }
        public void StopEnumerator() {
            if (currentDirtyTalkEnumerator != null)
            {
                character.main.StopCoroutine(currentDirtyTalkEnumerator);
                currentDirtyTalkEnumerator = null;
            }
        }
        bool isPausing = false;
        public void PauseDirtyTalk()
        {
            isPausing = true;
            StopEnumerator();

        }
        public void ResumeDirtyTalk() {
            isPausing = false;
            if (currentDirtyTalkEnumerator == null)
                StartEnumerator();
        }

        IEnumerator DirtyTalkEnumerator()
        {
            logger.DEBUG("Waiting!!!");
            yield return new WaitForSeconds(UnityEngine.Random.Range(minDirtyTalkDelay, maxDirtyTalkDelay));
            logger.DEBUG("Done waiting !!!");

            if (shouldDirtyTalk)
                DirtyTalk();
        }
        System.Random random = new System.Random();
        private void DirtyTalk() {
            var randomNumber = UnityEngine.Random.Range(0, 100);
            logger.StartMethod("DirtyTalk Random: " + randomNumber);
            logger.StartMethod("dirtyTalkChance: " + dirtyTalkChance);
            logger.StartMethod("compare: " + (randomNumber <= dirtyTalkChance));
            if (randomNumber > dirtyTalkChance)
            {
                logger.StartMethod("chance failed");
                return;
            }
            DirtyTalkHelper();
            StartEnumerator();
        }
        private void DirtyTalkHelper() {
            logger.StartMethod("DirtyTalkHelper()");

            try
            {
                if (!DirtyTalkEnabled())
                {
                    logger.DEBUG("dirty talk disabled");
                    return;
                }
                bool shouldAddGlobal = currentDirtyTalkLInes.Count == 0;
                if (shouldAddGlobal)
                {
                    logger.DEBUG("no current lines, adding global");
                    currentDirtyTalkLInes = AtomUtils.ConcatList(new List<string>(), globalDirtyTalkLines); //todo
                }
                logger.DEBUG("Count is +"+ currentDirtyTalkLInes.Count);
                int randomIndex = random.Next(currentDirtyTalkLInes.Count);
                logger.DEBUG("Random is is +" + randomIndex);

                string randomItem = currentDirtyTalkLInes[randomIndex];
                //character.voxtaService.SendEventNow(randomItem);
                character.voxtaService.SendSecret(randomItem);
                character.voxtaService.SendEventNow(" ");

                if (shouldAddGlobal)
                {
                    currentDirtyTalkLInes.Clear();
                }
            }
            catch (Exception e) {
                logger.ERR(e.Message);
            }
            
            
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

        private bool DirtyTalkEnabled() {
            return ConfigVoxReactor.singeton.globalDirtyTalkConfig.dirtyTalkEnabled.val;
        }
    }
}
