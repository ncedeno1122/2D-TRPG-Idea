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
            m_TileSelectionManager.CurrentMoveInProgress.Action = m_TileSelectionManager.CurrentMoveInProgress.Action;
        }

        // Commits the current move after the desired action and information is entered!
        public override void Exit()
        {
            Debug.Log("Exiting ActionPromptState!");
            m_TileSelectionManager.CommitMoveInProgress();
        }
        
        public override void HandleInput(Vector3Int tilePosition)
        {
            //
        }

        public override void HandleInput(TurnAction action)
        {
            m_TileSelectionManager.CurrentMoveInProgress.Action = action;
            
            m_TileSelectionManager.AdvanceState();
        }
    }
}
