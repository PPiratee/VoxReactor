using System;
using System.Collections.Generic;
using System.Linq;
using CheesyFX;
using MacGruber;

namespace PPirate.VoxReactor.Clothing
{
    internal class ClothingItem : MVRScript
    {
        private readonly String voxReactorStorableID = "PPirate.VoxReactor.Main";
        JSONStorable voxReactorPlugin;

        

        



        public JSONStorableString description = new JSONStorableString("description", "");
        public JSONStorableBool isOffWhenHidden = new JSONStorableBool("takeOffOnHidden", false);
        public JSONStorableBool wantsToBeOn = new JSONStorableBool("wantsToBeOn", true);
        public JSONStorableBool isOn = new JSONStorableBool("isOn", true);
        public JSONStorableBool enableLogging = new JSONStorableBool("enableLogging", true);


        public JSONStorableBool isMaterialHidden;
        public string materialRecieverName = "";
        bool startHasRan = false;

        private PannelClothingItem pannelClothingItem;

        private SafeMvr safeMvr;

        public override void Init()
        {
            this.overrideId = $"TEsTSTOREID:{this.storeId}";
            RegisterBool(isOffWhenHidden);
            RegisterBool(wantsToBeOn);

            RegisterBool(isOn);

            RegisterString(description);
            Log("Start Init()");
            LogInfo();





            try
            {
                voxReactorPlugin = AtomUtils.GetReciever(this.GetAtomById("CoreControl"), voxReactorStorableID);
                

            }
            catch
            {
                Log("Unable to find VoxReactor scene plugin.");
            }
            LoadUI();
            DrawUI();
            Log("End Init()");
            LogInfo();
        }
        void Start()
        {
            startHasRan = true;
            if (wantsToBeOn.val)
                TryPutOn();
        }
        public void OnEnable()
        {
            if (!startHasRan)
            {
                return;
            }
            if (wantsToBeOn.val)
                TryPutOn();
        }
        public void OnDisable()
        {
            TakeOff();
        }
        void OnDestroy()
        {
            wantsToBeOn.val = false;
            TakeOff();
            Utils.OnDestroyUI();
            safeMvr.CleanUpCallbacks();
        }
        public void TryPutOn()
        {
            Log("Start TryPutOn()");
            LogInfo();
            wantsToBeOn.val = true;
            PutOn();
            Log("End TryPutOn()");
            LogInfo();

        }
        public void TryTakeOff()
        {
            Log("Start TryTakeOff()");
            LogInfo();
            wantsToBeOn.val = false;
            TakeOff();
            Log("End TryTakeOff()");
            LogInfo();

        }
        private void PutOn()
        {
            Log("Start PutOn()");
            LogInfo();

            if ((isMaterialHidden?.val ?? false) && isOffWhenHidden.val)
            {
                Log("Put on fail");

                return;
            }

            SetClothingServiceArgs();

            voxReactorPlugin?.CallAction("OnAddCloth");
            isOn.val = true;
            Log("End PutOn()");
            LogInfo();
        }
        private void TakeOff()
        {
            Log("Start TakeOff()");
            LogInfo();
            SetClothingServiceArgs();
            voxReactorPlugin?.CallAction("OnRemoveCloth");
            isOn.val = false;
            Log("End TakeOff()");
            LogInfo();
        }


        private void SetClothingServiceArgs()
        {
            //voxReactorPlugin?.SetStringParamValue("ClothAtomName", this.containingAtom.name);//todo use this
            voxReactorPlugin?.SetStringParamValue("ClothAtomName", "Person");//temp
            voxReactorPlugin?.SetStringParamValue("ClothDescription", description.val);
            //Log("SetClothingServiceArgs() ");

        }

        private void Log(string msg)
        {
            if (enableLogging.val)
            {
                SuperController.LogMessage(msg);
            }
        }


        public void GetMaterialHiddenTarget()
        {
            if (materialRecieverName == "")
            {
                return;
            }
            var materialReciever = AtomUtils.GetReciever(containingAtom, materialRecieverName);
            isMaterialHidden = materialReciever.GetBoolJSONParam("hideMaterial");
            //todo use safeMvr
            isMaterialHidden.AddCallback(newIsHidden =>
            {
                Log(" START HIDDENCALLBACK: " + newIsHidden);
                LogInfo();
                if (!newIsHidden && wantsToBeOn.val && !isOn.val)
                {
                    PutOn();
                }
                else if (newIsHidden && isOffWhenHidden.val && isOn.val)
                {
                    TakeOff();
                }
                Log(" END HIDDENCALLBACK");
                LogInfo();

            }, false);


        }
        private void LogInfo()
        {

            Log("wantsToBeOn: " + wantsToBeOn.val);
            Log("isOn: " + isOn.val);
            Log("isMaterialHidden: " + isMaterialHidden?.val ?? "null");
            Log("isOffWhenHidden: " + isOffWhenHidden.val);
            Log("---------------------------");

        }
        public void ShouldTakeOffOnHiddenChanged(bool sholdTakeOffOnHidden)
        {
            Log("Start ShouldTakeOffOnHiddenChanged()");
            LogInfo();
            if (!sholdTakeOffOnHidden)
            {
                if (!isOn.val && wantsToBeOn.val)
                    PutOn();

            }
            else if (sholdTakeOffOnHidden)
            {
                if (isOn.val && (isMaterialHidden?.val ?? false))
                    TakeOff();
                if (!isOn.val && (!isMaterialHidden?.val ?? false))
                    PutOn();
            }
            Log("End ShouldTakeOffOnHiddenChanged()");
            LogInfo();

        }

        /////////////UI
        private void LoadUI()
        {
            UIManager.SetScript(this, CreateUIElement, leftUIElements, rightUIElements);
            Utils.OnInitUI(CreateUIElement);
            pannelClothingItem = new PannelClothingItem(this, leftUIElements, rightUIElements);

        }
        public void DrawUI()
        {
            pannelClothingItem.DrawPannelUI();
        }
        //todod on destory call  Utils.OnDestroyUI();
        public void AddBoolCallback(JSONStorableBool jsonParam, Action<bool> callback, bool useDefault = true)
        {
            jsonParam.setCallbackFunction += val => callback(val);
            if (useDefault) callback(jsonParam.defaultVal);
            else callback(jsonParam.val);
        }

    }
}
