using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MacGruber;
using System.Linq;
using CheesyFX;
using LeapInternal;


namespace PPirate.VoxReactor
{

    public class Main : MVRScript
    {
        private readonly SafeMvr safeMvr = new SafeMvr();
        private readonly Logger logger = new Logger("Main", 0);

        public static Main singleton;

        private VoxtaService voxtaService;
        private readonly List<Action<float>> fixedUpdateTimeConsumers = new List<Action<float>>();//todo maybe consumers should just be MVR scripts
        private readonly List<Action<float>> consumersToRemove = new List<Action<float>>();

        private PannelMain mainPannel;

        private ConfigVoxReactor config;
        public ConfigVoxReactor Config => config;




        // private JSONStorable voxtaStorable = null;
        private VoxtaPlugin voxtaPlugin = null;
        public override void Init()
        {

            logger.StartMethod("Init()");


            singleton = this;

            config = new ConfigVoxReactor(this);
            RegisterAction(new JSONStorableAction("OnReload", LoadCharacters));

            LoadUI();
            if (voxtaPlugin == null)
            {
                GetMainVoxtaAtom();
            }

            LoadVoxtaService();
            DrawUI();
        }
        #region temp
        public void LoadCharacters()
        {
            logger.StartMethod("LoadCharacters()");
            // LoadVoxtaService(atomWithVoxtaPlugin);
            if (voxtaPlugin == null)
            {
                GetMainVoxtaAtom();
            }
            LoadVoxtaService();

        }



        public void RunCoroutine(IEnumerator callback)
        { //todo this is stupid. use main.StartCoroutine
            StartCoroutine(callback);
        }
        public void PushFixedDeltaTimeConsumer(Action<float> callback)
        {
            if (fixedUpdateTimeConsumers.Contains(callback))
            {
                return;
            }
            fixedUpdateTimeConsumers.Add(callback);
        }
        public void RemoveFixedDeltaTimeConsumer(Action<float> callback)
        {
            consumersToRemove.Add(callback);
        }

        void FixedUpdate()
        {
            try
            {
                foreach (var action in consumersToRemove)
                {
                    fixedUpdateTimeConsumers.Remove(action); //why not remove when RemoveFixedDeltaTimeConsumer() is called? I dont think I did this for no reason.
                }
                consumersToRemove.Clear();
                foreach (var action in fixedUpdateTimeConsumers)
                {
                    action.Invoke(Time.deltaTime);
                }
            }
            catch (Exception e)
            {
                logger.ERR("Exception caught: " + e);

            }
        }
        #endregion
        #region UI
        private void LoadUI()
        {
            UIManager.SetScript(this, CreateUIElement, leftUIElements, rightUIElements);
            Utils.OnInitUI(CreateUIElement);
            mainPannel = new PannelMain(this, leftUIElements, rightUIElements);

        }
        public void DrawUI()
        {
            mainPannel.DrawPannelUI();
        }
        private void GetMainVoxtaAtom()
        {// this is retarded
            logger.StartMethod("GetMainVoxtaAtom()");

            List<string> atomIds = SuperController.singleton.GetAtomUIDs();
            foreach (string atomId in atomIds)
            {
                var atom = AtomUtils.getAtomByUid(atomId);
                try
                {
                    var voxtaStorable = AtomUtils.GetReciever(atom, VoxtaPlugin.voxtaPluginStorableID);
                    safeMvr.RemoveChild(this.voxtaPlugin);
                    this.voxtaPlugin = new VoxtaPlugin(this, voxtaStorable);
                    safeMvr.AddChild(this.voxtaPlugin);
                    logger.DEBUG("Found atom with voxta plugin.");
                    voxtaPlugin.setUpCallbacks();


                }
                catch (Exception e)
                {
                    continue;
                }
                break;
            }
        }
        void OnDestroy()
        {
            logger.StartMethod("OnDestroy()");

            // voxtaService.CleanUp();
            safeMvr.CleanUpCallbacks();
            Utils.OnDestroyUI();
        }
        private void LoadVoxtaService()
        {
            logger.StartMethod("LoadVoxtaService()");

            safeMvr.RemoveChild(voxtaService);
            voxtaService = new VoxtaService(this, voxtaPlugin);
            safeMvr.AddChild(voxtaService);

            mainPannel.characterPannel.OnVoxCharactersLoaded();
            DrawUI();
            NotifyClothingItems();

        }
        public static void RunAfterDelay(float delay, Action actionToCall)
        {
            Main.singleton.RunCoroutine(RunAfterDelayEnumerator(delay, actionToCall));
        }
        private static IEnumerator RunAfterDelayEnumerator(float delay, Action actionToCall)
        {
            yield return new WaitForSeconds(delay);
            actionToCall();
        }
        private void NotifyClothingItems()
        {
            VoxtaService.singleton.characters.ForEach(character =>
            {
              //  AtomUtils.GetRecievers(character.atom, )
            });
        }
    }

    #endregion


}