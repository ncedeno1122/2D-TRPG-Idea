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
            // TODO: Show attack range and all that.
        }

        public override void Exit()
        {
            Debug.Log("Exiting TargetSelectionState!");
            m_TileSelectionManager.GridHelper.ClearInteractionRange();
        }
        
        public override void HandleInput(Vector3Int tilePosition)
        {
            var characterOnTile = m_TileSelectionManager.GridHelper.GetCharacterOnTile(tilePosition);

            m_TileSelectionManager.AdvanceState();
        }

        public override void HandleInput(TurnAction action)
        {
            //
        }
    }
}
