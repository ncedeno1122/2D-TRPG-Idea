using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Unity_Project.Scripts.UIScripts.InventoryPanel
{
    public class ItemPanelScript : MonoBehaviour
    {
        public Image IconImage;
        public Text NameText;
        public Text UsesLeftText;

        // + + + + | Functions | + + + + 

        public void WriteItemData(IItem item)
        {
            if (item != null)
            {
                IconImage.sprite = item.Icon;
                NameText.text = item.ItemName;
                UsesLeftText.text = $"{item.UsesLeft} / {item.UsesTotal}";
            }
            else
            {
                IconImage.sprite = null;
                NameText.text = string.Empty;
                UsesLeftText.text = string.Empty;
            }
            
        }
    }
}
