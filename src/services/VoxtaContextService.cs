using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using CheesyFX;
using static JSONStorableBool;

namespace PPirate.VoxReactor
{
    internal class VoxtaContextService : SafeMvr
    {
        public static VoxtaContextService singleton;
        private readonly VoxtaPlugin voxtaPlugin;


        private readonly Logger logger2 = new Logger("VoxtaContextService");
        public JSONStorableAction OnRefreshContext;


        public VoxtaContextService(VoxtaPlugin voxtaPlugin) {
            this.voxtaPlugin = voxtaPlugin;
            AddCallback(voxtaPlugin.GetIsActiveStorable(), val => OnChatActiveChange(val));

            OnRefreshContext = new JSONStorableAction("OnRefreshContext", () => {
                RefreshContext();
            });
            Main.singleton.RegisterAction(OnRefreshContext);

            VoxtaContextService.singleton = this;
        }
        private List<string> contextItems = new List<string>();
        private List<string> queuedItems = new List<string>();
        public void AddContextItem(String newItem) {
            logger2.DEBUG("attempt AddContextItem " + newItem);
            if (contextItems.Contains(newItem) || newItem == "" )
            {
                return;
            }
            if (!voxtaPlugin.GetIsActiveStorable().val) {
                logger2.DEBUG("Voxta chat not active, queuing item ");

                queuedItems.Add(newItem);
                return;
            }
            logger2.DEBUG("adding context item now ");

            contextItems.Add(newItem);

            RefreshContext();
        }
        public void RemoveContextItem(String item)
        {
            logger2.DEBUG("RemoveContextItem " + item);
            if (queuedItems.Contains(item)) { 
                queuedItems.Remove(item);
            }
            if (!contextItems.Contains(item))
            {
                return;
            }
            contextItems.Remove(item);

            RefreshContext();
        }

        private void OnChatActiveChange(bool isActive)
        {
            

            logger2.DEBUG("OnChatActiveChange" + isActive);//todo remove this
            

            

            if (isActive) {
                
                Main.RunAfterDelay(2f, () => {
                    foreach (var item in queuedItems)
                    {
                        AddContextItem(item);
                    }
                    queuedItems.Clear();
                    RefreshContext();
                });    
            }
        }
        private void RefreshContext() {
            logger2.StartMethod("RefreshContext()");
            string contextSlot1Value = string.Empty;
            foreach (String item in contextItems)
            {
                contextSlot1Value += " " + item;
            }

            voxtaPlugin.SetContextSlot1(contextSlot1Value);
            logger2.DEBUG("set slot to " + contextSlot1Value);

        }

    }
    public interface IHasContext {
         string GetContext();
    }
    public class ContextItem
    {
        
    }
}
