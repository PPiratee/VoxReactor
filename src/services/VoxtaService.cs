using System;
using System.Collections.Generic;

namespace PPirate.VoxReactor
{
    internal class VoxtaService : SafeMvr
    {
        public static VoxtaService singleton;
        private readonly Main main;

        public readonly VoxtaPlugin voxtaPlugin;
   

        public List<VoxtaCharacter> characters = new List<VoxtaCharacter>();
        public readonly VoxtaCharacter character1;
        public readonly VoxtaCharacter character2;
        public readonly VoxtaCharacter character3;

        public readonly VoxtaContextService voxtaContextService;
        public readonly ConversationService conversationService;
        public readonly ClothingService clothingService;

        public readonly ObserverRegistry globalObserverRegistry;
        public static string REGISTRY_USER_SPEAKING = "userSpeaking";

        public string userName;


        private readonly Logger logger = new Logger("VoxtaService");
        public VoxtaService(Main main, VoxtaPlugin voxtaPlugin)
        {
            try {
                logger.StartMethod("Constructor");

                VoxtaService.singleton = this;

                this.main = main;
                this.voxtaPlugin = voxtaPlugin;


                this.userName = voxtaPlugin.GetUserName();//todo may want to wait util chat is active.
      
                voxtaPlugin.setActionConsumer(VoxtaActionConsumer);
                voxtaPlugin.setStateConsumer(VoxtaStateConsumer);

                voxtaContextService = new VoxtaContextService(voxtaPlugin);
                AddChild(voxtaContextService);
                globalObserverRegistry = new ObserverRegistry();
                conversationService = new ConversationService(this);
                

                SetUpCharacters();


                clothingService = new ClothingService(main, this, voxtaContextService);// must be after SetUpCharacters()
            } catch(Exception e) {
                logger.ERR("FAILURE during VoxtaService Constructor: " + e.Message);
            }
            
        }

        private void SetUpCharacters() {
            logger.StartMethod("SetUpCharacters()");
            string char1Name = voxtaPlugin.GetCharacterName(1);
          
            if (char1Name != null)
            {
              

                characters.Add(new VoxtaCharacter(1, char1Name, voxtaPlugin.GetChar1Atom(), main, this));//todo get atom from voxta plugin
                
            }
            /*
            string char2Name = voxtaPlugin.GetCharacter2Name();
            if (char2Name != null && main.GetAtomById("Person#2") != null)
            {
                characters.Add(new VoxtaCharacter(char2Name, main.GetAtomById("Person#2"), main, this, logger));
            }
            */
        }
        private VoxtaCharacter GetCharacterByName(string name) {
            logger.StartMethod("GetCharacterByName("+ name +")");

            foreach (VoxtaCharacter character in characters)
            {
                if (character.name == name) { 
                    return character;
                }
            }
            logger.ERR("Character " + name + " not found");
            throw new Exception("Character " + name + " not found");
        }
        private void VoxtaActionConsumer(String action)
        {
            logger.LOG("VoxtaService got action: " + action);
            // GetCharacterByName(voxtaPlugin.GetEnumFlagValue("currentName"))
            //.actionObserverRegistry.InvokeObservers(action);
            characters[0].actionObserverRegistry.InvokeObservers(action);
        }
        private void VoxtaStateConsumer(String state)
        {
            logger.DEBUG("Recieved state: " + state);

            if (state == VoxtaPlugin.STATE_LISTENING) { 
                globalObserverRegistry.InvokeObservers(REGISTRY_USER_SPEAKING);
            }
            // GetCharacterByName(voxtaPlugin.GetEnumFlagValue("currentName"))
             //.stateManager.UpdateState(state);
            characters[0].stateManager.UpdateState(state);

        }
        public void RequestCharacterSpeech(string speech) {
            
            voxtaPlugin.RequestCharacterSpeech(speech);
        }

        public void SendEventNow(string eventMsg) {
            voxtaPlugin.SendEvent(eventMsg);
        }
        public void SendSecret(string secret) {
            voxtaPlugin.SendSecret(secret);
        }
        public void SetCharacterCanSpeak(bool value)
        {
            voxtaPlugin.SetCharacterCanSpeak(value);
        }
        public void TriggerCommand(string arg) { 
            voxtaPlugin.TriggerCommand(arg);
        }
        public VoxtaCharacter getCharacterByAtomName(String atomName) {
            logger.DEBUG("getCharacterByAtomName(): " + atomName);

            VoxtaCharacter rv = null;
            foreach (VoxtaCharacter character in characters) {
                if (character.atom.name == atomName) {
                    rv =  character;
                    break;
                }
            }
            if(rv == null)
            {
                logger.ERR("Unable to find Voxta character with name " + atomName);
            }

            return rv;
        }
   
    }
}
