using System;
using System.Collections.Generic;

using static JSONStorableBool;
using static JSONStorableString;
using static JSONStorableStringChooser;


namespace PPirate.VoxReactor
{
    internal class SafeMvr 
    {

        private readonly List<IClearable> callBacks = new List<IClearable>();
        private readonly List<SafeMvr> children = new List<SafeMvr>();

        public void CleanUpCallbacks()
        {
            foreach (SafeMvr child in children) { 
                child.CleanUpCallbacks(); 
            }


            foreach (var item in callBacks)
            {
                item.Clear();
            }
        }
        public void AddChild(SafeMvr child) {
            if (child == this) {
                return;
            }
            if (!children.Contains(child)) { 
                children.Add(child);
            }
        }
        public void RemoveChild(SafeMvr child) {
            if (children.Contains(child)) { 
                children.Remove(child);
                child.CleanUpCallbacks();
            }
        }

        public void RemoveCallback(IClearable callback)
        {
            callback.Clear();
            callBacks.Remove(callback);
        }

        public BoolCallback AddCallback(JSONStorableBool jsonParam, Action<bool> callbackDelegate)
        {
            var newCallback = new BoolCallback(jsonParam, new SetBoolCallback(callbackDelegate));
            callBacks.Add(newCallback);
            return newCallback;
        }

        public StringCallback AddCallback(JSONStorableString jsonParam, Action<string> callbackDelegate)
        {
            var newCallback = new StringCallback(jsonParam, new JSONStorableString.SetStringCallback(callbackDelegate));
            callBacks.Add(newCallback);
            return newCallback;
        }

        public StringChooserCallback AddCallback(JSONStorableStringChooser jsonParam, Action<string> callbackDelegate)
        {
            var newCallback = new StringChooserCallback(jsonParam, new JSONStorableStringChooser.SetStringCallback(callbackDelegate));
            callBacks.Add(newCallback);
            return newCallback;
        }
        

        public class BoolCallback : IClearable
        {
            private readonly JSONStorableBool jsonParam;
            private readonly SetBoolCallback setBoolCallback;
            public BoolCallback(JSONStorableBool jsonParam, SetBoolCallback setBoolCallback) { 
                this.jsonParam = jsonParam;
                this.setBoolCallback = setBoolCallback;
                jsonParam.setCallbackFunction += setBoolCallback;
            }

            public void Clear() {
                jsonParam.setCallbackFunction -= setBoolCallback;

            }
        }
        public class StringCallback : IClearable
        {
            private readonly JSONStorableString jsonParam;
            private readonly JSONStorableString.SetStringCallback setStringCallback;
            public StringCallback(JSONStorableString jsonParam, JSONStorableString.SetStringCallback setStringCallback)
            {
                this.jsonParam = jsonParam;
                this.setStringCallback = setStringCallback;
                jsonParam.setCallbackFunction += setStringCallback;
            }

            public void Clear()
            {
                jsonParam.setCallbackFunction -= setStringCallback;

            }
        }
        public class StringChooserCallback : IClearable
        {
            private readonly JSONStorableStringChooser jsonParam;
            private readonly JSONStorableStringChooser.SetStringCallback setStringCallback;
            public StringChooserCallback(JSONStorableStringChooser jsonParam, JSONStorableStringChooser.SetStringCallback setStringCallback)
            {
                this.jsonParam = jsonParam;
                this.setStringCallback = setStringCallback;
                jsonParam.setCallbackFunction += setStringCallback;
            }

            public void Clear()
            {
                jsonParam.setCallbackFunction -= setStringCallback;

            }
        }
        public interface IClearable
        {
            void Clear();
        }

    }
    
}
