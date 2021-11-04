using Unity_Project.Scripts.BattleDataScripts;
using UnityEngine;

namespace Unity_Project.Scripts.TileSelectionLogic
{
    public class TargetSelectionState : TileSelectionState
    {
        public TargetSelectionState(TileSelectionManager tsm) : base(tsm)
        {
        }

        public override void Enter()
        {
            Debug.Log("Entered TargetSelectionState!");
            m_TileSelectionManager.CurrentMoveInProgress.Target = m_TileSelectionManager.CurrentMoveInProgress.Target;
            m_TileSelectionManager.CurrentMoveInProgress.TargetPosition = m_TileSelectionManager.CurrentMoveInProgress.TargetPosition;
        }

        public override void Exit()
        {
            Debug.Log("Exiting TargetSelectionState!");
        }
        
        public override void HandleInput(Vector3Int tilePosition)
        {
            var characterOnTile = m_TileSelectionManager.GridHelper.GetCharacterOnTile(tilePosition);
            
            m_TileSelectionManager.CurrentMoveInProgress.Target = characterOnTile;
            m_TileSelectionManager.CurrentMoveInProgress.TargetPosition = tilePosition; // TODO: Make function in GHS to use DFS to get nearest Walkable tile to tilePosition!
            
            m_TileSelectionManager.AdvanceState();
        }

        public override void HandleInput(TurnAction action)
        {
            //
        }
    }
}
