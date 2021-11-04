using Unity_Project.Scripts.BattleDataScripts;
using UnityEngine;

namespace Unity_Project.Scripts.TileSelectionLogic
{
    public class CharacterSelectionState : TileSelectionState
    {
        public CharacterSelectionState(TileSelectionManager tsm) : base(tsm)
        {
        }
    
        public override void Enter()
        {
            Debug.Log("Entered CharacterSelectionState!");
            m_TileSelectionManager.CurrentMoveInProgress.User = m_TileSelectionManager.CurrentMoveInProgress.User; // Set it to what it was, or null if undefined yet.
            m_TileSelectionManager.CurrentMoveInProgress.OriginPosition = m_TileSelectionManager.CurrentMoveInProgress.OriginPosition;
        }

        public override void Exit()
        {
            Debug.Log("Exiting CharacterSelectionState!");
        }

        public override void HandleInput(Vector3Int tilePosition)
        {
            var characterOnTile = m_TileSelectionManager.GridHelper.GetCharacterOnTile(tilePosition);
            
            m_TileSelectionManager.CurrentMoveInProgress.User = characterOnTile;
            m_TileSelectionManager.CurrentMoveInProgress.OriginPosition = tilePosition;
            
            m_TileSelectionManager.AdvanceState();
        }

        public override void HandleInput(TurnAction action)
        {
            //
        }
    }
}
