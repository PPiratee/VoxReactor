using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using MacGruber;
using System.Linq;
using CheesyFX;
using MeshVR;
using MVR.FileManagementSecure;
using UnityEngine.Events;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using Object = UnityEngine.Object;
using Battlehub.RTCommon;


namespace PPirate.VoxReactor
{

    public class UITabBar
    {
        public UITabBar parentTabBar;
        private List<UIPannel> pannels;
        public List<object> UIElements = new List<object>();
        public List<Transform> leftUIElements;
        public List<Transform> rightUIElements;
        public UIDynamicTabBar tabbar;
        public int currentTab;

        public MVRScript mvrScript;
        public bool selectFirst;
        public bool isSubTabBar;
        string tabBarName;

        public UITabBar(string tabBarName, MVRScript mvrScript, List<Transform> leftUIElements, List<Transform> rightUIElements, UITabBar parentTabBar, bool selectFirst = false, bool isSubTabBar = false)
        {
            this.tabBarName = tabBarName ;
            this.mvrScript = mvrScript;
            this.parentTabBar = parentTabBar;
            this.leftUIElements = leftUIElements;
            this.rightUIElements = rightUIElements;
            this.selectFirst = selectFirst;
            this.isSubTabBar = isSubTabBar;
           // SuperController.LogError("(UITabBar)" + tabBarName + " - Instanciating");

        }
        private void MakeBackButton()
        {
            //SuperController.LogError("(UITabBar)" + tabBarName + " - MakeBackButton()");

            var button = this.mvrScript.CreateButton("Back");

            Utils.SetupSpacer(this.mvrScript, 51, true);
            button.button.onClick.AddListener(() => {
                //SuperController.LogError("(UITabBar) - BackButtonCallback -> " + parentTabBar.tabBarName);

                if (selectFirst) { 
                }
                this.ClearUI();
                this.parentTabBar.CreateTabs();
            });
        }
        /*
        !> (UITabBar) - BackButtonCallback -> <Main/MainTabBar>
!> (UITabBar)<PannelCharacterSelect/Characters> - Clearing UI 
!> (UITabBar)<Main/MainTabBar> - Selecting pannel Main Settings*/
        public void CreateTabs(List<UIPannel> pannels)
        {
            // SuperController.LogError("(UITabBar)" + tabBarName + " - Create Tabs");
            if (pannels.Count == 0) {
                return;
            }
            this.pannels = pannels;
            CreateTabs();
        }
  
        public void CreateTabs()
        {
            if (isSubTabBar)
            {
                return;
            }
            tabbar = UIManager.CreateTabBar(pannels, SelectTab, 5);
            if (selectFirst) { 
                tabbar.SelectTab(currentTab); //crash
            }
            
        }
        public void CreateSubTabs()
        {
           // SuperController.LogError("(UITabBar)" + tabBarName + " - Create Sub tabs");
            if (!isSubTabBar)
            {
                return;
            }
            if (parentTabBar != null && isSubTabBar)
            {
                MakeBackButton();
            }

            tabbar = UIManager.CreateTabBar(pannels, SelectTab, 5);
            if (selectFirst)
            {
                tabbar.SelectTab(currentTab); //crash
            }

        }

        private void SelectTab(UIPannel pannel)
        {
            //SuperController.LogError("(UITabBar)" + tabBarName + " - Selecting pannel: " + pannel.tabLabel);

            RemoveUIElements(UIElements);
            UIElements.Clear();
            /*
            if (selectFirst == false && isSubTabBar) {
                pannel.containerTabBar.ClearUI();
                pannel.containerTabBar?.parentTabBar.ClearUI();
                 ((UITabPannelWithSubMenu)pannel).OnSubPannelSelected();
            }*/ //todo
            //pannel.CreatePannelUI();
            //annel.containerTabBar.CreateTabs2();// crash
            ClearUI();
            pannel.DrawPannelUI();

        }

        public void ClearUI()
        {
           // SuperController.LogError("(UITabBar)" + tabBarName + " - Clearing UI ");

            RemoveUIElements(leftUIElements.Select(x => (object)x.GetComponent<UIDynamic>()).ToList());
            RemoveUIElements(rightUIElements.Select(x => (object)x.GetComponent<UIDynamic>()).ToList());
            UIElements.Clear();
        }

        public void RemoveUIElements(List<object> UIElements)
        {
            for (int i = 0; i < UIElements.Count; ++i)
            {
                RemoveUIElement(UIElements[i]);
            }
            UIElements.Clear();
        }
        public void RemoveUIElement(object element)
        {
            if (element == null) return;/*
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
            */
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
