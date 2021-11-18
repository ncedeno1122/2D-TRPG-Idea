using UnityEngine;
using Unity_Project.Scripts.BattleDataScripts;

namespace Unity_Project.Scripts.TileSelectionLogic
{
    public abstract class TileSelectionState
    {
        protected readonly TileSelectionManager m_TileSelectionManager;
        protected readonly GridHelperScript m_GridHelper;
        protected readonly GridCursorController m_GridCursor;
        
        protected TileSelectionState(TileSelectionManager tsm)
        {
            m_TileSelectionManager = tsm;
            m_GridHelper = m_TileSelectionManager.GridHelper;
            m_GridCursor = m_TileSelectionManager.GridCursor;

        }
        
        public abstract void Enter();
        public abstract void Exit();
        public abstract void HandleInput(Vector3Int tilePosition);
        public abstract void HandleInput(TurnAction action);
        public abstract void HandleInput(KeyCode kc);
        public abstract void HandleRevertState();
        
        protected void PlayConfirmSound()
        {
            AudioManager.Instance.PlaySFXByName("UIMenu_Accept");
        }

        protected void PlayDeclineSound()
        {
            AudioManager.Instance.PlaySFXByName("UIMenu_Decline");
        }

        protected void PlaySelectSound()
        {
            AudioManager.Instance.PlaySFXByName("UIMenu_Select1");
        }
    }
}
