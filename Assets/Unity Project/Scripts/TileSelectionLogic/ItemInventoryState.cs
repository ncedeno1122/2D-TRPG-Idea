using Unity_Project.Scripts.BattleDataScripts;
using UnityEngine;

namespace Unity_Project.Scripts.TileSelectionLogic
{
    public class ItemInventoryState : TileSelectionState
    {
        private CharacterUnitScript m_InventoryCharacter;
        public ItemInventoryState(TileSelectionManager tsm) : base(tsm)
        {
            m_InventoryCharacter = m_TileSelectionManager.CurrentMoveInProgress.User;
        }

        public override void Enter()
        {
            m_TileSelectionManager.InventoryPanel.OpenInventoryPanelFor(m_InventoryCharacter);
            m_TileSelectionManager.InventoryPanel.Show();
        }

        public override void Exit()
        {
            throw new System.NotImplementedException();
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
            m_TileSelectionManager.ChangeState(new ActionPromptState(m_TileSelectionManager));
        }
    }
}
