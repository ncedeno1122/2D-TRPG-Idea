using System.Collections.Generic;
using Unity_Project.Scripts.BattleDataScripts;
using UnityEngine;

namespace Unity_Project.Scripts.TileSelectionLogic
{
    public class SelectTargetFromRangeState : TileSelectionState
    {
        private List<CharacterUnitScript> m_TargetsInRange; // TODO: Refactor to be a parent of CharacterUnitScript or interface member. We want chests, doors, interactables, etc.
        
        public SelectTargetFromRangeState(TileSelectionManager tsm) : base(tsm)
        {
        }

        public override void Enter()
        {
            // Define list of m_TargetsInRange
        }

        public override void Exit()
        {
            //
        }

        public override void HandleInput(Vector3Int tilePosition)
        {
            //
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
                    // Navigate
                    break;
                case KeyCode.RightArrow:
                    // Navigate
                    break;
                case KeyCode.Return:
                    // Go to confirm the target
                    break;
            }
        }

        public override void HandleRevertState()
        {
            m_TileSelectionManager.ChangeState(new TargetSelectionState(m_TileSelectionManager));
        }
    }
}
