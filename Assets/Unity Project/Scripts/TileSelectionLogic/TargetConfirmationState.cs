using Unity_Project.Scripts.BattleDataScripts;
using UnityEngine;

namespace Unity_Project.Scripts.TileSelectionLogic
{
    public class TargetConfirmationState : TileSelectionState
    {
        private TileEntity m_SelectedEntity;
        public TargetConfirmationState(TileSelectionManager tsm, TileEntity selectedEntity) : base(tsm)
        {
            m_SelectedEntity = selectedEntity;
        }

        public override void Enter()
        {
            Debug.Log("Entering TargetConfirmationState!");
        }

        public override void Exit()
        {
            Debug.Log("Exiting TargetConfirmationState!");
        }

        public override void HandleInput(Vector3Int tilePosition)
        {
            throw new System.NotImplementedException();
        }

        public override void HandleInput(TurnAction action)
        {
            throw new System.NotImplementedException();
        }

        public override void HandleInput(KeyCode kc)
        {
            throw new System.NotImplementedException();
        }

        public override void HandleRevertState()
        {
            m_TileSelectionManager.ChangeState(new TargetSelectionState(m_TileSelectionManager));
        }
    }
}
