using System;
using System.Collections.Generic;
using Unity_Project.Scripts.BattleDataScripts;
using Unity_Project.Scripts.UIScripts.ActionPrompt;
using Unity_Project.Scripts.UIScripts.InventoryPanel;
using UnityEngine;
using UnityEngine.UI;

namespace Unity_Project.Scripts.TileSelectionLogic
{
    // Holds all of the pertinent data for a TurnActionCommand as it's being defined
    public struct MoveInProgress
    {
        public CharacterUnitScript User;
        public TileEntity Target;
        public TurnAction Action;
        public Vector3Int OriginPosition;
        public Vector3Int? TargetPosition;
    }
    
    public class TileSelectionManager : MonoBehaviour // Functions as Context for State, uses Observer to message states
    {
        private TileSelectionState m_CurrentState;
        public MoveInProgress CurrentMoveInProgress;
        [SerializeField]
        public List<Vector3Int> SelectedTilePath = new List<Vector3Int>();

        public BattleDataManager BattleDataManager;
        public GridHelperScript GridHelper;
        public GridCursorController GridCursor;

        public ActionPromptScript ActionPrompt;
        public InventoryPanelScript InventoryPanel;

        private void Start()
        {
            // Initialize CurrentState
            m_CurrentState = new CharacterSelectionState(this);
            m_CurrentState.Enter();
        }

        // + + + + | Functions | + + + + 

        public void HandleInput(Vector3Int tilePosition)
        {
            m_CurrentState.HandleInput(tilePosition);
        }

        public void HandleTurnAction(int turnActionInt)
        {
            TurnAction action = (TurnAction) turnActionInt;
            m_CurrentState.HandleInput(action);
        }

        public void HandleKeyCodeInput(KeyCode kc)
        {
            m_CurrentState.HandleInput(kc);
        }

        public void HandleRevertState()
        {
            m_CurrentState.HandleRevertState();
        }

        public void ChangeState(TileSelectionState newState)
        {
            m_CurrentState.Exit();

            m_CurrentState = newState;

            m_CurrentState.Enter();
        }

        private bool IsMoveInProgressValid()
        {
            if (!CurrentMoveInProgress.User || !CurrentMoveInProgress.Target) return false;
            switch (CurrentMoveInProgress.Action)
            {
                case TurnAction.WAIT:
                    return true;
                case TurnAction.HEAL:
                    return CurrentMoveInProgress.Target &&
                           CurrentMoveInProgress.TargetPosition != CurrentMoveInProgress.OriginPosition;
                case TurnAction.ATTACK:
                    return CurrentMoveInProgress.Target &&
                           CurrentMoveInProgress.TargetPosition != CurrentMoveInProgress.OriginPosition;
                case TurnAction.TALK:
                    // Get Talk-able Neighbor?
                    return true; // TODO: 
                default:
                    return true;
            }
        }

        private TurnActionCommand GetTurnActionCommandFor(MoveInProgress mip)
        {
            switch (mip.Action)
            {
               case TurnAction.WAIT:
                   return new WaitCommand(mip.User, mip.TargetPosition);
               // TODO: Add more cases for other TACs as I make them
            }
            
            // Unaccounted case
            //Debug.Log("Could not get fitting TurnActionCommand for move...");
            return null;
        }
        
        public void CommitMoveInProgress()
        {
            var turnAction = GetTurnActionCommandFor(CurrentMoveInProgress);
            BattleDataManager.AddTurnActionCommand(turnAction);
        }
    }
}
