using System.Collections.Generic;
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

        private List<Vector3Int> m_MovableTiles = new List<Vector3Int>();
        private List<Vector3Int> m_ActionableTiles = new List<Vector3Int>();
        
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
            
            // Get Lists
            m_MovableTiles = m_GridHelper.GetMovableRange(m_SelectedUnit.TilePosition, m_SelectedUnit.UnitData.Prototype.MoveRange);
            m_ActionableTiles = m_GridHelper.GetActionableRange(ref m_MovableTiles, m_EquippedItemRange);

            // PaintInteractionRange
            m_GridHelper.PaintInteractionRange(m_TileSelectionManager.CurrentMoveInProgress.OriginPosition, ref m_MovableTiles, ref m_ActionableTiles, m_EquippedItem);
        }

        public override void Exit()
        {
            Debug.Log("Exiting TargetSelectionState!");
            m_TileSelectionManager.GridHelper.ClearActionTilemap();
        }
        
        public override void HandleInput(Vector3Int tilePosition)
        {
            // If the tilePosition is valid (walkable or actionable)
            if (m_MovableTiles.Contains(tilePosition) || m_ActionableTiles.Contains(tilePosition))
            {
                // Set MoveInProgress' data
                m_TileSelectionManager.CurrentMoveInProgress.TargetPosition = tilePosition;
                m_TileSelectionManager.CurrentMoveInProgress.Target = m_GridHelper.GetCharacterOnTile(tilePosition);
                
                // Animatedly move the CharacterUnitScript 
                // TODO: There's got to be a more organized way...
                var origin = m_TileSelectionManager.CurrentMoveInProgress.OriginPosition;
                var tilePath = m_TileSelectionManager.SelectedTilePath;
                var validTiles = new List<Vector3Int>();
                validTiles.AddRange(m_MovableTiles);
                validTiles.AddRange(m_ActionableTiles);
                
                if (m_GridHelper.FindPathToTarget(origin, tilePosition, ref tilePath, ref validTiles))
                {
                    m_GridHelper.EnsureWalkablePath(ref m_TileSelectionManager.SelectedTilePath);
                    if (m_TileSelectionManager.SelectedTilePath.Count > 0);
                    {
                        m_TileSelectionManager.CurrentMoveInProgress.User.FollowPath(m_TileSelectionManager.SelectedTilePath);
                    }
                }
                
                // Finally, advance state
                PlaySelectSound();
                m_TileSelectionManager.ChangeState(DetermineNextState(tilePosition));
            }
            else
            {
                // If we've clicked on a Tile that's invalid (usually outside the Interaction range), 
                m_TileSelectionManager.SelectedTilePath.Clear();
                m_GridHelper.ClearActionTilemap();
                
                // Also, revert to the last state.
                PlayDeclineSound();
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
            m_TileSelectionManager.CurrentMoveInProgress.Target = null;
            m_TileSelectionManager.CurrentMoveInProgress.TargetPosition = null;
            
            m_TileSelectionManager.ChangeState(new CharacterSelectionState(m_TileSelectionManager));
        }
        
        // + + + + | Functions | + + + + 

        private TileSelectionState DetermineNextState(Vector3Int targetTilePosition)
        {
            // First, get some information about the tile
            var isWithinMovableRange = m_MovableTiles.Contains(targetTilePosition);
            var isWithinActionableRange = m_ActionableTiles.Contains(targetTilePosition);
            var targetOnTile = m_GridHelper.GetTileEntityOnTile(targetTilePosition);

            // Are we within range OR clicking our origin tile?
            if (!isWithinActionableRange && !isWithinMovableRange)
            {
                // Essentially, HandleRevertState().
                return new CharacterSelectionState(m_TileSelectionManager);
            }
            else
            {
                // Did we click on a character (that's not the User)?
                if (targetOnTile && targetOnTile != m_TileSelectionManager.CurrentMoveInProgress.User)
                {
                    // First, save the target info
                    m_TileSelectionManager.CurrentMoveInProgress.Target = targetOnTile;
                    m_TileSelectionManager.CurrentMoveInProgress.TargetPosition = targetTilePosition;
                    
                    // TODO: Do we have a valid action with them?
                    
                    //
                    return new TargetConfirmationState(m_TileSelectionManager, targetOnTile);
                }
                else
                {
                    // If there's no target on the tile,
                    
                    // First, save the target info
                    m_TileSelectionManager.CurrentMoveInProgress.Target = null;
                    m_TileSelectionManager.CurrentMoveInProgress.TargetPosition = targetTilePosition;
                    
                    // Then, are we just within Movable Range?
                    if (isWithinMovableRange)
                    {
                        return new ActionPromptState(m_TileSelectionManager);
                    }
                    else
                    {
                        return new CharacterSelectionState(m_TileSelectionManager);
                    }
                }
            }
        }
    }
}
