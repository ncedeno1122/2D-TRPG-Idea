using System.Collections.Generic;
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

        private void HideAllButtons()
        {
            foreach (var btn in ActionButtons)
            {
                HideButton(btn);
            }
        }

        private void ShowButton(Button b)
        {
            b.interactable = true;
            b.gameObject.SetActive(true);
        }

        public void HoverButton(Button b)
        {
            //b.image.color = Color.yellow;
            b.OnPointerEnter(null);
        }

        public void HoverExitButton(Button b)
        {
            //b.image.color = Color.white;
            b.OnPointerExit(null);
        }

        public void HoverExitAllButtons()
        {
            foreach (var b in ActionButtons)
            {
                HoverExitButton(b);
            }
        }

        public void LoadValidButtons(CharacterUnitScript user, List<string> binStringList)
        {
            HideAllButtons();

            foreach (var binString in binStringList)
            {
                // Talk
                if (binString[0] == '1')
                {
                    ShowButton(TalkButton);
                }
                // Interact
                if (binString[1] == '1')
                {
                    ShowButton(InteractButton);
                }
                // Attack
                if (binString[2] == '1')
                {
                    ShowButton(AttackButton);
                }
                // Heal
                if (binString[3] == '1')
                {
                    ShowButton(HealButton);
                }
                // Chest
                if (binString[4] == '1')
                {
                    ShowButton(ChestButton);
                }
                // Trade
                if (binString[5] == '1')
                {
                    ShowButton(TradeButton);
                }
            }
            
            // Items
            if (user.CanUseItems())
            {
                ShowButton(ItemsButton);
            }
            // Convoy
            if (user.CanUseConvoy())
            {
                ShowButton(ConvoyButton);
            }

            // Wait
            ShowButton(WaitButton);
            
            // TODO: Show Items and Convoy buttons based on user (new arg?)
            // Cancel
            ShowButton(CancelButton);
        }
    }
}
