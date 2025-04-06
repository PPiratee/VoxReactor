using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using LeapInternal;

namespace PPirate.VoxReactor
{
    internal class ObserverRegistry
    {
        
        private Logger logger = new Logger("ObserverRegistry");

        public ObserverRegistry()
        { 

        }

        Dictionary<string, List<Action>> observerMap = new Dictionary<string, List<Action>>();
        public void InvokeObservers(String keyName)
        {
            logger.StartMethod("InvokeObservers(): " + keyName);

            try
            { 
                List<Action> observers = observerMap[keyName];

                logger.DEBUG("actions found for key " + keyName);
                foreach (Action action in observers)
                {
                    action();
                }
            
            } catch { 
                logger.DEBUG("no actions found for key " + keyName);

            }
        }

        public void RegisterObserver(String keyName, Action callback)
        {
            logger.DEBUG("registering key " + keyName);

            if (!observerMap.ContainsKey(keyName))
                observerMap[keyName] = new List<Action>();
            
            observerMap[keyName].Add(callback);

        }

    }
}
