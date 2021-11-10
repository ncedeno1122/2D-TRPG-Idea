using System;
using System.Collections.Generic;
using System.Linq;
using Unity_Project.Scripts.BattleDataScripts;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Unity_Project.Scripts.TileSelectionLogic
{
    public class ActionPromptState : TileSelectionState
    {
        private List<Vector3Int> m_AdjacentTiles; // Adjacent Tiles
        private List<Vector3Int> m_ActionableTilesInRange; // Actionable Range from TargetPosition
        private List<TileEntity> m_TileEntitiesInRange;
        private readonly List<Button> m_ActionButtons;
        private Dictionary<TileEntity, string> m_TileEntityActions;
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

            if (m_TileSelectionManager.CurrentMoveInProgress.TargetPosition != null)
            {
                var targetPosition = (Vector3Int) m_TileSelectionManager.CurrentMoveInProgress.TargetPosition;
                //var userMoveRange = m_TileSelectionManager.CurrentMoveInProgress.User.UnitData.Prototype.MoveRange;
                var battleItem = m_TileSelectionManager.CurrentMoveInProgress.User.EquippedBattleItem;
                var battleItemRange = battleItem ? battleItem.Range : 0;
            
                // Define TilesInRange, determine list of Targets
                m_ActionableTilesInRange = m_GridHelper.GetTilesInRange(targetPosition, battleItemRange, true);
                //Debug.Log($"Got m_ActionableTilesInRange, count of {m_ActionableTilesInRange.Count}!");

                // Get Adjacent Tiles
                m_AdjacentTiles = m_GridHelper.GetTilesInRange(targetPosition, 1, false);
                //Debug.Log($"Got m_AdjacentTiles, count of {m_AdjacentTiles.Count}!");
            
                // Get Characters within ActionableRange
                m_TileEntitiesInRange = m_GridHelper.GetTileEntitiesInRange(targetPosition, battleItemRange);
                
                // Define Dictionary of all TileEntities with valid TurnActions
                m_TileEntityActions = new Dictionary<TileEntity, string>();
                PopulateTileEntityActions(ref m_TileEntityActions, ref m_TileEntitiesInRange);

                // Get Actions to show for ActionPrompt
                LoadProperActionButtons();

                // Paint the new Interaction Range
                m_GridHelper.PaintInteractionRange(targetPosition, ref m_AdjacentTiles, ref m_ActionableTilesInRange, battleItem);
            }

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
            
        }

        public override void HandleInput(TurnAction action)
        {
            var validTileEntitiesForAction = GetTileEntitiesForAction(action, ref m_TileEntityActions);
            
            // TODO: Send this to SelectFromRangeState with this list if not empty
            m_TileSelectionManager.ChangeState(new SelectTargetFromRangeState(m_TileSelectionManager, validTileEntitiesForAction));
        }
        
        public override void HandleInput(KeyCode kc)
        {
            // Select different buttons based on arrow keys and confirm with enter or something
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
        
        // + + + + | Functions | + + + + 

        private void LoadProperActionButtons()
        {
            m_TileSelectionManager.ActionPrompt.LoadValidButtons(m_TileSelectionManager.CurrentMoveInProgress.User, m_TileEntityActions.Values.ToList());
        }

        /// <summary>
        /// Returns a binary string representing all valid User-to-Target interactions. 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private string GetValidActionsString(CharacterUnitScript user, TileEntity target)
        {
            var finalStr = string.Empty;
            var isTileAdjacent = m_AdjacentTiles.Contains(target.TilePosition);
            finalStr += Convert.ToInt32(user.CanTalkWith(target) && isTileAdjacent);
            finalStr += Convert.ToInt32(user.CanInteractWith(target) && isTileAdjacent);
            finalStr += Convert.ToInt32(user.CanAttack(target));
            finalStr += Convert.ToInt32(user.CanHeal(target));
            finalStr += Convert.ToInt32(user.CanUnlockChest(target) && isTileAdjacent);
            finalStr += Convert.ToInt32(user.CanTrade(target) && isTileAdjacent);
            return finalStr;
        }

        private void PopulateTileEntityActions(ref Dictionary<TileEntity, string> dictionary, ref List<TileEntity> entitiesWithinRange)
        {
            foreach (var entity in entitiesWithinRange)
            {
                // Don't include the user
                if (entity == m_TileSelectionManager.CurrentMoveInProgress.User) continue;
                dictionary.Add(entity, GetValidActionsString(m_TileSelectionManager.CurrentMoveInProgress.User, entity));
            }
            
            // Debug - Print the Dictionary after its definition
            // Debug.Log("Printing Dictionary in PopulateTileEntityActions!");
            // foreach (var kvp in dictionary)
            // {
            //     Debug.Log($"Key: {kvp.Key} | {kvp.Value}");
            // }
        }

        private bool DoesBinStringAllowAction(string binString, TurnAction action)
        {
            int? searchIndex = null;
            switch (action)
            {
                case TurnAction.TALK:
                    searchIndex = 0;
                    break;
                case TurnAction.INTERACT:
                    searchIndex = 1;
                    break;
                case TurnAction.ATTACK:
                    searchIndex = 2;
                    break;
                case TurnAction.HEAL:
                    searchIndex = 3;
                    break;
                case TurnAction.CHEST:
                    searchIndex = 4;
                    break;
                case TurnAction.TRADE:
                    searchIndex = 5;
                    break;
            }

            if (searchIndex != null)
            {
                return binString[(int) searchIndex] == '1';
            }
            return false;
        }

        private List<TileEntity> GetTileEntitiesForAction(TurnAction action, ref Dictionary<TileEntity, string> dictionary)
        {
            var validEntitiesForActions = new List<TileEntity>();
            
            foreach (var entity in dictionary.Keys)
            {
                if (!dictionary.TryGetValue(entity, out var entityBinString)) continue;
                if (DoesBinStringAllowAction(entityBinString, action))
                {
                    validEntitiesForActions.Add(entity);
                }
            }
            
            // Debug - Print the list
            // Debug.Log($"Printing list for {action}-able TileEntities in the dictionary!");
            // foreach (var entity in validEntitiesForActions)
            // {
            //     if (dictionary.TryGetValue(entity, out var binString))
            //     {
            //         Debug.Log($"{entity}, {binString}");
            //     }
            // }
            
            return validEntitiesForActions;
        }
    }
}
