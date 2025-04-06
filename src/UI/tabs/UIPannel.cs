using MacGruber;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CheesyFX;

namespace PPirate.VoxReactor
{
    public abstract class UIPannel
    {
        protected readonly MVRScript mvrScript;
        public readonly string tabLabel;
        protected readonly UIPannel parentPannel;
        protected readonly List<object> uiElements;
        protected List<Transform> leftUIElements;
        protected List<Transform> rightUIElements;

        public UIPannel(string tabLabel, UIPannel parentPannel)
        {
            this.tabLabel = tabLabel;
            this.parentPannel = parentPannel;
            this.mvrScript = parentPannel.mvrScript;
            this.uiElements = parentPannel.uiElements;
            this.leftUIElements = parentPannel.leftUIElements;
            this.rightUIElements = parentPannel.rightUIElements;
        }
        //Use this constructor in your root UIPannel
        public UIPannel(string tabLabel, MVRScript mvrScript, List<Transform> leftUIElements, List<Transform> rightUIElements)
        {
            this.tabLabel = tabLabel;
            this.mvrScript = mvrScript;
            this.leftUIElements = leftUIElements;
            this.rightUIElements = rightUIElements;
            this.uiElements = new List<object>();
        }

        public abstract void DrawPannelUI();

        protected void MakeBackButton()
        {
            var button = this.mvrScript.CreateButton("Back to " + parentPannel.tabLabel);

            Utils.SetupSpacer(this.mvrScript, 51, true);
            button.button.onClick.AddListener(() => {
                this.parentPannel.DrawPannelUI();
            });
        }
        public void ClearPannelUI()
        {
            //SuperController.LogError("(UIPannel)" + tabLabel + " - ClearPannelUI() ");

            RemoveUIElements(leftUIElements.Select(x => (object)x.GetComponent<UIDynamic>()).ToList());
            RemoveUIElements(rightUIElements.Select(x => (object)x.GetComponent<UIDynamic>()).ToList());
            uiElements.Clear();
        }
        private void RemoveUIElements(List<object> UIElements)
        {
            for (int i = 0; i < UIElements.Count; ++i)
            {
                RemoveUIElement(UIElements[i]);
            }
            UIElements.Clear();
        }
        private void RemoveUIElement(object element)
        {
            if (element == null) return;
            if (element is JSONStorableParam) 
            {
                JSONStorableParam jsp = element as JSONStorableParam;
                if (jsp is JSONStorableFloat)
                    mvrScript.RemoveSlider(jsp as JSONStorableFloat);
                else if (jsp is JSONStorableBool)
                    mvrScript.RemoveToggle(jsp as JSONStorableBool);
                else if (jsp is JSONStorableColor)
                    mvrScript.RemoveColorPicker(jsp as JSONStorableColor);
                else if (jsp is JSONStorableString)
                    mvrScript.RemoveTextField(jsp as JSONStorableString);
                else if (jsp is JSONStorableStringChooser)
                {
                    // Workaround for VaM not cleaning its panels properly.
                    JSONStorableStringChooser jssc = jsp as JSONStorableStringChooser;
                    RectTransform popupPanel = jssc.popup?.popupPanel;
                    mvrScript.RemovePopup(jssc);
                    if (popupPanel != null)
                        UnityEngine.Object.Destroy(popupPanel.gameObject);
                }
            }
            
            else if (element is UIDynamic)
            {
                UIDynamic uid = element as UIDynamic;
                if (uid is UIDynamicButton)
                    mvrScript.RemoveButton(uid as UIDynamicButton);
                else if (uid is MyUIDynamicSlider)
                {
                    var uidSlider = uid as MyUIDynamicSlider;
                    leftUIElements.Remove(uidSlider.transform);
                    rightUIElements.Remove(uidSlider.transform);
                    MVRScript.Destroy(uidSlider.gameObject);
                }
                else if (uid is UIDynamicSlider)
                    mvrScript.RemoveSlider(uid as UIDynamicSlider);
                else if (uid is UIDynamicColorPicker)
                    mvrScript.RemoveColorPicker(uid as UIDynamicColorPicker);
                else if (uid is UIDynamicTextField)
                    mvrScript.RemoveTextField(uid as UIDynamicTextField);
                else if (uid is UIDynamicPopup)
                {
                    // Workaround for VaM not cleaning its panels properly.
                    UIDynamicPopup uidp = uid as UIDynamicPopup;
                    RectTransform popupPanel = uidp.popup?.popupPanel;
                    mvrScript.RemovePopup(uidp);
                    if (popupPanel != null)
                        UnityEngine.Object.Destroy(popupPanel.gameObject);
                }
                else if (uid is MyUIDynamicToggle)
                {
                    var uidToggle = uid as MyUIDynamicToggle;
                    leftUIElements.Remove(uidToggle.transform);
                    rightUIElements.Remove(uidToggle.transform);
                    MVRScript.Destroy(uidToggle.gameObject);
                }
                else if (uid is UIDynamicToggle)
                    mvrScript.RemoveToggle(uid as UIDynamicToggle);
                else if (uid is UIDynamicV3Slider)
                {
                    var v3Slider = uid as UIDynamicV3Slider;
                    leftUIElements.Remove(v3Slider.transform);
                    rightUIElements.Remove(v3Slider.spacer.transform);
                    MVRScript.Destroy(v3Slider.spacer.gameObject);
                    MVRScript.Destroy(v3Slider.gameObject);
                }
                else if (uid is UIDynamicTabBar)
                {
                    var tabbar = uid as UIDynamicTabBar;
                    leftUIElements.Remove(tabbar.transform);
                    rightUIElements.Remove(tabbar.spacer.transform);
                    MVRScript.Destroy(tabbar.spacer.gameObject);
                    MVRScript.Destroy(tabbar.gameObject);
                }
                else if (uid is UIDynamicToggleArray)
                {
                    if (uid == null) return;
                    var toggleArray = uid as UIDynamicToggleArray;
                    leftUIElements.Remove(toggleArray.transform);
                    rightUIElements.Remove(toggleArray.spacer.transform);
                    MVRScript.DestroyImmediate(toggleArray.spacer.gameObject);
                    MVRScript.DestroyImmediate(uid.gameObject);
                }
                else
                {
                    mvrScript.RemoveSpacer(uid);
                }
            }
        }

    }


}
