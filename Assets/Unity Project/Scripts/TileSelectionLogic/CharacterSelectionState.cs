using Unity_Project.Scripts.BattleDataScripts;
using UnityEditor;
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
            m_GridHelper.ClearInteractionRange();
            Debug.Log("Entered CharacterSelectionState!");
        }

        public override void Exit()
        {
            Debug.Log("Exiting CharacterSelectionState!");
        }

        public override void HandleInput(Vector3Int tilePosition)
        {
            // Initial CharacterUnit Selection
            var characterOnTile = m_GridHelper.GetCharacterOnTile(tilePosition);

            if (characterOnTile) // TODO: Verify that Character is of the current phase's allegiance
            {
                // Get Character Data on Tile
                var charData = characterOnTile.UnitData;
                var equippedBattleItem = characterOnTile.EquippedBattleItem;
                var equippedBattleItemRange = equippedBattleItem ? equippedBattleItem.Range : 0;

                // Store MoveInProgress data
                m_TileSelectionManager.CurrentMoveInProgress.User = characterOnTile;
                m_TileSelectionManager.CurrentMoveInProgress.OriginPosition = tilePosition;
                
                // Advance state only if we have a valid character on the tile.
                m_TileSelectionManager.ChangeState(new TargetSelectionState(m_TileSelectionManager));
            }
            else
            {
                // Clear MoveInProgress data
                m_TileSelectionManager.CurrentMoveInProgress.User = null;
                m_TileSelectionManager.CurrentMoveInProgress.OriginPosition = Vector3Int.zero; // TODO: How to make Vector3Int null / some blank value??
            }
        }

        public override void HandleInput(TurnAction action)
        {
            //
        }

        public override void HandleInput(KeyCode kc)
        {
            // TODO: Move GridCursor or something for arrow keys
            switch (kc)
            {
                case KeyCode.UpArrow:
                    m_GridCursor.Translate(Vector2Int.up);
                    break;
                case KeyCode.DownArrow:
                    m_GridCursor.Translate(Vector2Int.down);
                    break;
                case KeyCode.RightArrow:
                    m_GridCursor.Translate(Vector2Int.right);
                    break;
                case KeyCode.LeftArrow:
                    m_GridCursor.Translate(Vector2Int.left);
                    break;
                case KeyCode.Return:
                    // Confirm and advance state
                    HandleInput(Vector3Int.FloorToInt(m_GridCursor.transform.position));
                    break;
            }
        }

        public override void HandleRevertState()
        {
            // Can't revert from this state, baby! >:]
        }
    }
}
