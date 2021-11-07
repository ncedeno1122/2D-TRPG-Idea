using System.Collections.Generic;
using Unity_Project.Scripts.BattleDataScripts;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Unity_Project.Scripts.TileSelectionLogic
{
    public class ActionPromptState : TileSelectionState
    {
        private List<Vector3Int> m_TilesInRange;

        private List<Button> m_ActionButtons;
        private Button m_SelectedButton;
        private int m_SelectedButtonIndex;
        
        public ActionPromptState(TileSelectionManager tsm) : base(tsm)
        {
            // Get list of ActionButtons from the ActionPrompt
            m_ActionButtons = tsm.ActionPrompt.ActionButtons;
        }

        public override void Enter()
        {
            Debug.Log("Entered ActionPromptState!");
            
            // Define TilesInRange, determine list of Targets
            m_TilesInRange = m_GridHelper.GetTilesInRange(m_TileSelectionManager.CurrentMoveInProgress.OriginPosition,
                m_TileSelectionManager.CurrentMoveInProgress.User.UnitData.Prototype.MoveRange);

            // This will use helper functions and stuff to determine the ActionPrompt
            m_TileSelectionManager.ActionPrompt.HoverExitAllButtons();
            m_TileSelectionManager.ActionPrompt.Show();
        }

        // Commits the current move after the desired action and information is entered!
        public override void Exit()
        {
            Debug.Log("Exiting ActionPromptState!");
            m_TileSelectionManager.ActionPrompt.HoverExitAllButtons();
            m_TileSelectionManager.ActionPrompt.Hide();
        }
        
        public override void HandleInput(Vector3Int tilePosition)
        {
            //
        }

        public override void HandleInput(TurnAction action)
        {
            // TODO: Switch 'action' and get a roll on baby!
            switch (action)
            {
                //
            }
        }
        
        public override void HandleInput(KeyCode kc)
        {
            // TODO: Select different buttons based on arrow keys and confirm with enter or something
            if (!m_SelectedButton)
            {
                m_SelectedButton = m_ActionButtons[0];
                m_SelectedButtonIndex = -1;
            }
            
            switch (kc)
            {
                case KeyCode.UpArrow:
                    m_SelectedButtonIndex--;
                    break;
                case KeyCode.RightArrow:
                    m_SelectedButtonIndex++;
                    break;
                case KeyCode.DownArrow:
                    m_SelectedButtonIndex++;
                    break;
                case KeyCode.LeftArrow:
                    m_SelectedButtonIndex--;
                    break;
                case KeyCode.Return:
                    Debug.Log($"Selected Button '{ m_SelectedButton.name }' and firing onClick!");
                    m_SelectedButton.onClick.Invoke();
                    break;
            }
            
            // Change the last button's selection color
            m_TileSelectionManager.ActionPrompt.HoverExitButton(m_SelectedButton);
            
            // Select the proper button and change its selection color
            m_SelectedButtonIndex = Mathf.Clamp(m_SelectedButtonIndex, 0, m_ActionButtons.Count - 1);
            m_SelectedButton = m_ActionButtons[m_SelectedButtonIndex];
            m_TileSelectionManager.ActionPrompt.HoverButton(m_SelectedButton);
        }

        public override void HandleRevertState()
        {
            m_TileSelectionManager.ChangeState(new TargetSelectionState(m_TileSelectionManager));
        }
    }
}
