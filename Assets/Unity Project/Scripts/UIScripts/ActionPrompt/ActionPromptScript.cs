using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Unity_Project.Scripts.UIScripts.ActionPrompt
{
    public class ActionPromptScript : MonoBehaviour
    {
        public Button TalkButton;
        public Button AttackButton;
        public Button CombatArtsButton;
        public Button ChestButton;
        public Button ItemsButton;
        public Button TradeButton;
        public Button ConvoyButton;
        public Button WaitButton;

        public List<Button> ActionButtons;

        public CharacterUnitScript TestUnitData;

        public CanvasGroup CanvasGroup;

        private void Awake()
        {
            ActionButtons = new List<Button>()
            {
                TalkButton, AttackButton, CombatArtsButton, ChestButton,
                ItemsButton, TradeButton, ConvoyButton, WaitButton
            };
        }

        private void Start()
        {
            OpenActionPrompt(TestUnitData);
        }

        // + + + + | Functions | + + + +

        private void Hide() => gameObject.SetActive(false);
        private void Show() => gameObject.SetActive(true);

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
        
        private void ShowAllowedButtons(CharacterUnitScript unit)
        {
            foreach (var b in ActionButtons)
            {
                HideButton(b);
            }
            
            // Wait
            ShowButton(WaitButton);
            ShowButton(ConvoyButton);
        }
        
        public void OpenActionPrompt(CharacterUnitScript unit)
        {
            Debug.Log($"Opening ActionPromptScript for { unit.UnitData.Name }!");
            ShowAllowedButtons(unit);
            Show();

        }
    }
}
