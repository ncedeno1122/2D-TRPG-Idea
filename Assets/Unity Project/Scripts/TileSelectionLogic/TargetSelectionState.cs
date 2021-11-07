using Unity_Project.Scripts.BattleDataScripts;
using UnityEngine;

namespace Unity_Project.Scripts.TileSelectionLogic
{
    public class TargetSelectionState : TileSelectionState
    {
        private CharacterUnitScript m_SelectedUnit;
        private CharacterUnit m_UnitData;
        private IBattleItem m_EquippedItem;
        private int m_EquippedItemRange;
        private TurnAction m_DesiredAction;
        
        public TargetSelectionState(TileSelectionManager tsm) : base(tsm)
        {
            m_SelectedUnit = tsm.CurrentMoveInProgress.User;
            m_UnitData = m_SelectedUnit.UnitData;
            m_EquippedItem = m_SelectedUnit.EquippedBattleItem;
            m_EquippedItemRange = m_EquippedItem?.Range ?? 0; // This syntax shakes me to my core, LOL...
            
            m_DesiredAction = TurnAction.WAIT;
            if (m_EquippedItem != null)
            {
                m_DesiredAction = m_EquippedItem is IWeapon ? TurnAction.ATTACK : TurnAction.HEAL;
            }

        }

        public override void Enter()
        {
            Debug.Log("Entered TargetSelectionState!");
            
            // If the TileManager's SelectedTilePath is set, we must have reverted. Set the Unit's position back to the Origin.
            if (m_TileSelectionManager.SelectedTilePath.Count > 0)
            {
                m_TileSelectionManager.CurrentMoveInProgress.User.SnapToPosition(m_TileSelectionManager.CurrentMoveInProgress.OriginPosition);
                m_TileSelectionManager.SelectedTilePath.Clear();
            }
            
            // Regardless, show the interaction range again
            
            // PaintInteractionRange
            m_GridHelper.PaintInteractionRange(m_TileSelectionManager.CurrentMoveInProgress.User.UnitData.Prototype.MoveRange, m_EquippedItemRange, m_SelectedUnit.TilePosition, m_DesiredAction);
        }

        public override void Exit()
        {
            Debug.Log("Exiting TargetSelectionState!");
            m_TileSelectionManager.GridHelper.ClearInteractionRange();
        }
        
        public override void HandleInput(Vector3Int tilePosition)
        {
            // If the tilePosition is valid (walkable or actionable)
            if (m_GridHelper.IsTileMovable(tilePosition) || m_GridHelper.IsTileActionable(tilePosition))
            {
                // Set MoveInProgress' data
                m_TileSelectionManager.CurrentMoveInProgress.TargetPosition = tilePosition;
                m_TileSelectionManager.CurrentMoveInProgress.Target = m_GridHelper.GetCharacterOnTile(tilePosition);
                
                // Animatedly move the CharacterUnitScript 
                // TODO: There's got to be a more organized way...
                if (m_GridHelper.FindPathToTarget(m_TileSelectionManager.CurrentMoveInProgress.OriginPosition, tilePosition, ref m_TileSelectionManager.SelectedTilePath))
                {
                    m_GridHelper.EnsureWalkablePath(ref m_TileSelectionManager.SelectedTilePath);
                    if (m_TileSelectionManager.SelectedTilePath.Count > 0);
                    {
                        m_TileSelectionManager.CurrentMoveInProgress.User.FollowPath(m_TileSelectionManager.SelectedTilePath);
                    }
                }
                
                // Finally, advance state to ActionPrompt
                m_TileSelectionManager.ChangeState(new ActionPromptState(m_TileSelectionManager));
            }
            else
            {
                // If we've clicked on a Tile that's invalid (usually outside the Interaction range), 
                m_GridHelper.ClearFoundTilePath();
                m_GridHelper.ClearInteractionRange();
                
                // Also, revert to the last state.
                HandleRevertState();
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
                    HandleInput(Vector3Int.FloorToInt(m_GridCursor.transform.position));
                    break;
            }
        }

        public override void HandleRevertState()
        {
            m_TileSelectionManager.ChangeState(new CharacterSelectionState(m_TileSelectionManager));
        }
    }
}
