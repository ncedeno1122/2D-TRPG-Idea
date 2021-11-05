using Unity_Project.Scripts.BattleDataScripts;
using UnityEngine;

namespace Unity_Project.Scripts.TileSelectionLogic
{
    public class ActionPromptState : TileSelectionState
    {
        public ActionPromptState(TileSelectionManager tsm) : base(tsm)
        {
        }

        public override void Enter()
        {
            Debug.Log("Entered ActionPromptState!");
            m_TileSelectionManager.ActionPrompt.Show();
        }

        // Commits the current move after the desired action and information is entered!
        public override void Exit()
        {
            Debug.Log("Exiting ActionPromptState!");
            m_TileSelectionManager.ActionPrompt.Hide();
            
            m_TileSelectionManager.CommitMoveInProgress();
        }
        
        public override void HandleInput(Vector3Int tilePosition)
        {
            //
        }

        public override void HandleInput(TurnAction action)
        {
            m_TileSelectionManager.AdvanceState();
        }
    }
}
