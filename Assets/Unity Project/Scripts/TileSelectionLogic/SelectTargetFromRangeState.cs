using System;
using System.Collections.Generic;
using System.Linq;
using Unity_Project.Scripts.BattleDataScripts;
using UnityEngine;

namespace Unity_Project.Scripts.TileSelectionLogic
{
    public class SelectTargetFromRangeState : TileSelectionState
    {
        private List<TileEntity> m_TargetsInRange;
        private int m_SelectedTargetIndex = -1;
        
        public SelectTargetFromRangeState(TileSelectionManager tsm, List<TileEntity> entitiesInRange) : base(tsm)
        {
            m_TargetsInRange = entitiesInRange;
        }

        public override void Enter()
        {
            //Debug.Log($"Entering SelectTargetFromRangeState with a list of {m_TargetsInRange.Count}!");
            
            // Mark Tiles
            m_GridHelper.PaintEntityTiles(ref m_TargetsInRange);
            
            // Set GridCursor to the first target in range
            m_GridCursor.transform.position = m_GridHelper.Grid.GetCellCenterWorld(m_TargetsInRange[0].TilePosition);
        }

        public override void Exit()
        {
            Debug.Log("Exiting SelectTargetFromRangeState!");
            m_GridHelper.ClearActionTilemap();
        }

        public override void HandleInput(Vector3Int tilePosition)
        {
            // See if the clicked tilePosition is in the list
            if (IsTargetOnPositionInRange(tilePosition))
            {
                // Confirm Target by setting target data
                var target = m_GridHelper.GetTileEntityOnTile(tilePosition);
                m_TileSelectionManager.CurrentMoveInProgress.Target = target;
                
                m_TileSelectionManager.ChangeState(new TargetConfirmationState(m_TileSelectionManager, target));
            }
            else
            {
                HandleRevertState();
            }
        }

        public override void HandleInput(TurnAction action)
        {
            //
        }
        
        public override void HandleInput(KeyCode kc)
        {
            // TODO: Cycle through list of targets and all that
            switch (kc)
            {
                case KeyCode.LeftArrow:
                    m_SelectedTargetIndex--;
                    break;
                case KeyCode.RightArrow:
                    m_SelectedTargetIndex++;
                    break;
                case KeyCode.UpArrow:
                    m_SelectedTargetIndex--;
                    break;
                case KeyCode.DownArrow:
                    m_SelectedTargetIndex++;
                    break;
                case KeyCode.Return:
                    HandleInput(m_TargetsInRange[m_SelectedTargetIndex].TilePosition);
                    break;
            }
            
            // Clamp and select value
            m_SelectedTargetIndex = Mathf.Clamp(m_SelectedTargetIndex, 0, m_TargetsInRange.Count - 1);
            m_GridCursor.transform.position = m_GridHelper.Grid.GetCellCenterWorld(m_TargetsInRange[m_SelectedTargetIndex].TilePosition);
            //Debug.Log($"Selected {m_TargetsInRange[m_SelectedTargetIndex].name} in a list of {m_TargetsInRange.Count} entities.");
        }

        public override void HandleRevertState()
        {
            m_TileSelectionManager.ChangeState(new ActionPromptState(m_TileSelectionManager));
        }
        
        // + + + + | Functions | + + + +

        /// <summary>
        /// Returns if a target has a given position in m_TargetsInRange.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool IsTargetOnPositionInRange(Vector3Int position)
        {
            foreach (var entity in m_TargetsInRange)
            {
                if (entity.TilePosition == position) return true;
            }
            return false;
        }
    }
}
