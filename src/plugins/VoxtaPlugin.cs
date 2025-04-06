using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using Leap.Unity.Query;
using CheesyFX;

namespace PPirate.VoxReactor
{
    internal class VoxtaPlugin : SafeMvr
    {
        private readonly Main main;
        private readonly Logger logger = new Logger("VoxtaPlugin");

        public static readonly String voxtaPluginStorableID = "3_Voxta";
        private readonly JSONStorable voxtaPlugin;

        public static string STATE_IDLE = "idle";
        public static string STATE_SPEAKING = "speaking";
        public static string STATE_LISTENING = "listening";

        public VoxtaPlugin(Main main, JSONStorable voxtaStorable)
        {

            this.main = main;
           // this.logger = logger;
           // this.actionConsumer = actionConsumer;
           // this.stateConsumer = stateConsumer;
            this.voxtaPlugin = voxtaStorable;
     

            OnVoxtaAction = new JSONStorableAction("OnVoxtaAction", () => {
                OnVoxtaActionCallback(voxtaPlugin.GetStringParamValue("CurrentAction"));
            });
            main.RegisterAction(OnVoxtaAction);

            //OnVoxtaStateChanged = new JSONStorableAction("OnVoxtaStateChanged", () => {
             //   OnVoxtaActionCallback(voxtaPlugin.GetStringParamValue("ST"))
            //});
            //main.RegisterAction(OnVoxtaStateChanged);
           
        }
        public void setUpCallbacks() {
            CleanUpCallbacks();
            AddCallback(voxtaPlugin.GetStringJSONParam("Character Name 1"), (string str) =>
            {
                main.LoadCharacters();
            });

            AddCallback(voxtaPlugin.GetStringChooserJSONParam("AudioAtom"), (string str) =>
            {
                main.LoadCharacters();
            });

            AddCallback(voxtaPlugin.GetStringChooserJSONParam("State"), (string state) =>
            {
                VoxtaStateChangedCallback(state);
            });           
        }

        public JSONStorableAction OnVoxtaAction;
        private Action<string> actionConsumer;
        public void setActionConsumer(Action<string> actionConsumer) {
            this.actionConsumer = actionConsumer;
        }
        public void OnVoxtaActionCallback(string currentAction)
        {
            
            logger.DEBUG("Voxta action: " + currentAction + " recieved");
            actionConsumer?.Invoke(currentAction);
        }


        public JSONStorableAction OnVoxtaStateChanged;
        private Action<string> stateConsumer;

        public void setStateConsumer(Action<string> stateConsumer) { 
            this.stateConsumer = stateConsumer;
        }

        private void VoxtaStateChangedCallback(string currentState)
        {
            
            logger.DEBUG("Voxta state: " + currentState + " recieved");

            stateConsumer?.Invoke(currentState);

        }
        public Atom GetChar1Atom() {
            var atomId = voxtaPlugin.GetStringChooserParamValue("AudioAtom");
            return AtomUtils.getAtomByUid(atomId);
        }

        public string GetCharacterName(int charNumber)
        {
           // logger.ERR("GetCharacterNamevoxtaPlugin:::::::" + voxtaPlugin);

            return voxtaPlugin.GetStringParamValue($"Character Name {charNumber}");
        }
        public string GetCharacterRole(int charNumber)
        {
            return voxtaPlugin.GetStringParamValue($"Character Role {charNumber}");
        }
        public string GetCharacter1Name()
        {
            return voxtaPlugin.GetStringParamValue("Character Name 1");
        }
        public string GetCharacter2Name()
        {
            return voxtaPlugin.GetStringParamValue("Character Name 2");
        }

        public JSONStorableBool GetIsActiveStorable() {
            return voxtaPlugin.GetBoolJSONParam("Active");
        }

        public void SendEvent(string eventMsg) {
            logger.DEBUG("SendEvent: " + eventMsg);
            voxtaPlugin.SetStringParamValue("TriggerMessage", "/event " + eventMsg);
        }
        public void SendSecret(string secretMsg)
        {
            logger.DEBUG("SendSecret: " + secretMsg);
            voxtaPlugin.SetStringParamValue("TriggerMessage", "/secret " + secretMsg);
        }
        public void RequestCharacterSpeech(string speech)
        {
            voxtaPlugin.SetStringParamValue("RequestCharacterSpeech", speech);
        }
        public void TriggerCommand(string arg)
        {
            logger.DEBUG("TriggerCommand: " + arg);
            voxtaPlugin.SetStringParamValue("TriggerMessage", "/trigger " + arg);
        }

        public void SetContextSlot1(string value) {
            logger.StartMethod("SetContextSlot1(): " + value);
            voxtaPlugin.SetStringParamValue("Context (Slot 1)", "");

            voxtaPlugin.SetStringParamValue("Context (Slot 1)", value);
        }
        public void SetContextSlot2(string value)
        {
            voxtaPlugin.SetStringParamValue("Context (Slot 2)", value);
        }
        public void SetFlag(string flag)
        {
            voxtaPlugin.SetStringParamValue("SetFlags", flag);

        }
        public string GetFlags()
        {
            return voxtaPlugin.GetStringParamValue("Flags");
        }
        public string GetEnumFlagValue(string enumBase)
        {
            string allFlags = GetFlags();
            string[] flags = allFlags.Split(',');
            foreach (string flag in flags) { 
                string trimmed = flag.Trim();
                if (trimmed.Contains(enumBase)) { 
                    string value = trimmed.Split('.')[1];
                    return value;
                }
            }

            return null;
        }
        public void SetCharacterCanSpeak(bool value) {
            voxtaPlugin.SetBoolParamValue("CharacterCanSpeak", value);
        }
        public string GetUserName() {
            return voxtaPlugin.GetStringParamValue("User Name");
        }
    }
}
