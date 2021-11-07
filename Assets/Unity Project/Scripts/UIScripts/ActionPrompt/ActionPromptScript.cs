using System;
using System.Collections.Generic;
using Unity_Project.Scripts.BattleDataScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Unity_Project.Scripts.UIScripts.ActionPrompt
{
    public class ActionPromptScript : MonoBehaviour
    {
        public Button TalkButton;
        public Button InteractButton;
        public Button AttackButton;
        public Button HealButton;
        public Button CombatArtsButton;
        public Button ChestButton;
        public Button ItemsButton;
        public Button TradeButton;
        public Button ConvoyButton;
        public Button WaitButton;
        public Button CancelButton;

        public List<Button> ActionButtons;
        public CanvasGroup CanvasGroup;

        private void OnValidate()
        {
            foreach (Transform child in transform)
            {
                var btn = child.GetComponent<Button>();
                if (child.name.Contains("Talk")) TalkButton = btn;
                else if (child.name.Contains("Interact")) InteractButton = btn;
                else if (child.name.Contains("Attack")) AttackButton = btn;
                else if (child.name.Contains("Heal")) HealButton = btn;
                else if (child.name.Contains("CombatArts")) CombatArtsButton = btn;
                else if (child.name.Contains("Chest")) ChestButton = btn;
                else if (child.name.Contains("Items")) ItemsButton = btn;
                else if (child.name.Contains("Trade")) TradeButton = btn;
                else if (child.name.Contains("Convoy")) ConvoyButton = btn;
                else if (child.name.Contains("Wait")) WaitButton= btn;
                else if (child.name.Contains("Cancel")) CancelButton = btn;
                
                //
                if (!ActionButtons.Contains(btn))
                {
                    ActionButtons.Add(btn);
                }
            }
        }

        private void Start()
        {
            Hide();
        }

        // + + + + | Functions | + + + +

        public void Hide()
        {
            CanvasGroup.alpha = 0f;
            
            foreach (var btn in ActionButtons)
            {
                btn.interactable = false;
            }
        }
        public void Show()
        {
            CanvasGroup.alpha = 1f;
            
            foreach (var btn in ActionButtons)
            {
                if (btn.isActiveAndEnabled)
                {
                    btn.interactable = true;
                }
            }
        }

        private void HideButton(Button b)
        {
            b.interactable = false;
            b.gameObject.SetActive(false);
        }

        private void ShowButton(Button b)
        {
            b.interactable = true;
            b.gameObject.SetActive(true);
        }

        public void HoverButton(Button b)
        {
            b.image.color = Color.yellow;
        }

        public void HoverExitButton(Button b)
        {
            b.image.color = Color.white;
        }

        public void HoverExitAllButtons()
        {
            foreach (var b in ActionButtons)
            {
                HoverExitButton(b);
            }
        }
        
        private void ShowAllowedButtons(CharacterUnitScript unit)
        {
            foreach (var b in ActionButtons)
            {
                HideButton(b);
            }
            
            // Wait
            ShowButton(WaitButton);
            ShowButton(CancelButton);
        }
        
        public void OpenActionPrompt(CharacterUnitScript unit)
        {
            Debug.Log($"Opening ActionPromptScript for { unit.UnitData.Name }!");
            ShowAllowedButtons(unit);
            Show();

        }
    }
}
