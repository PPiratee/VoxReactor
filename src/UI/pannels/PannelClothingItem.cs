
using CheesyFX;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using MacGruber;

namespace PPirate.VoxReactor.Clothing
{
    internal class PannelClothingItem : UIPannel
    {
        private readonly ClothingItem clothingItem;
        public PannelClothingItem(ClothingItem clothingItem, List<Transform> leftUIElements, List<Transform> rightUIElements) : base("Main Menu", clothingItem, leftUIElements, rightUIElements)
        {
            this.clothingItem = clothingItem;
        }

        public override void DrawPannelUI()
        {
            
            ClearPannelUI();
            clothingItem.description.CreateUI(uiElements);
            mvrScript.CreateButton("Put on test").button.onClick.AddListener(clothingItem.TryPutOn);
            mvrScript.CreateButton("Take off test").button.onClick.AddListener(clothingItem.TryTakeOff);

            DrawMaterialChooserUI();

            if (clothingItem.isMaterialHidden != null)
            {
                clothingItem.isOffWhenHidden.CreateUI(uiElements, true);
                clothingItem.isOffWhenHidden.toggle.onValueChanged.AddListener(clothingItem.ShouldTakeOffOnHiddenChanged);
            }
            else {
                clothingItem.isOffWhenHidden.val = false;
            }
        }

        private void DrawMaterialChooserUI() {
            var recievers = mvrScript.containingAtom.GetStorableIDs();
            recievers = recievers.Where(word => word.Contains("MaterialCombined")).ToList();//todo remove this
            JSONStorableStringChooser materialChooser2 = new JSONStorableStringChooser("material2", recievers, clothingItem.materialRecieverName ?? "", "Material Storable", MaterialChooserChosen);
            mvrScript.RegisterStringChooser(materialChooser2);
            materialChooser2.CreateUI(uiElements, true, 2);
        }

        private void MaterialChooserChosen(string recieverName)
        {
            SuperController.LogError("CHOSEN");
            if (recieverName == null || recieverName == "")
            {
                return;
            }
            clothingItem.materialRecieverName = recieverName;
            clothingItem.GetMaterialHiddenTarget();
            DrawPannelUI();
        }
    }
}

