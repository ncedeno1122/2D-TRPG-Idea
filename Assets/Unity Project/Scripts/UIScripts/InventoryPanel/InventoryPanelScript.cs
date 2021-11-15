using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity_Project.Scripts.UIScripts.InventoryPanel
{
    public class InventoryPanelScript : MonoBehaviour
    {
        public CanvasGroup CanvasGroup;
        public ItemPanelScript EquippedItemPanel;
        public ItemPanelScript[] ItemPanels;

        private void Start()
        {
            Hide();
        }
        
        // + + + + | Functions | + + + + 

        public void Hide()
        {
            CanvasGroup.alpha = 0f;
        }

        public void Show()
        {
            CanvasGroup.alpha = 1f;
        }

        public void OpenInventoryPanelFor(CharacterUnitScript unit)
        {
            var equippedWeapon = unit.EquippedBattleItem;
            var inventory = unit.Inventory;

            Debug.Log($"local inventory: {inventory.Length} | unit inventory {unit.Inventory.Length} | panels array {ItemPanels.Length}");
            
            EquippedItemPanel.WriteItemData(equippedWeapon);
            
            for (int i = 0; i < inventory.Length - 1; i++)
            {
                ItemPanels[i].WriteItemData(inventory[i]);
            }
        }

        private void WriteItemInfoTo(ItemPanelScript itemPanel)
        {
            //itemPanel.WriteItemData();
        }
        
    }
}
