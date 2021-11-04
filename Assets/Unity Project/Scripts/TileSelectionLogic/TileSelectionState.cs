using UnityEngine;
using Unity_Project.Scripts.BattleDataScripts;

namespace Unity_Project.Scripts.TileSelectionLogic
{
    public abstract class TileSelectionState
    {
        protected TileSelectionManager m_TileSelectionManager;
        
        protected TileSelectionState(TileSelectionManager tsm)
        {
            m_TileSelectionManager = tsm;
        }
        
        public abstract void Enter();
        public abstract void Exit();
        public abstract void HandleInput(Vector3Int tilePosition);
        public abstract void HandleInput(TurnAction action);

        public void HandleRevertState()
        {
            m_TileSelectionManager.RevertState();
        }
    }
}
