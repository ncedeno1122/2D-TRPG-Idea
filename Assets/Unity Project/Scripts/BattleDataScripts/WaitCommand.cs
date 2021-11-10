using UnityEngine;

namespace Unity_Project.Scripts.BattleDataScripts
{
    public class WaitCommand : TurnActionCommand
    {
        public WaitCommand(CharacterUnitScript user, Vector3Int? targetPosition) : base(user, null, targetPosition)
        {
            Action = TurnAction.WAIT;
        }
    
        public override bool IsActionValid()
        {
            return true; // Waiting should always be valid :]
        }

        public override void Execute()
        {
            if (!IsActionValid()) return;
            if (TargetPosition == OriginPosition) return;
            if (TargetPosition != null) User.TilePosition = (Vector3Int)TargetPosition;
        }

        public override void Undo()
        {
            if (!IsActionValid()) return;
            if (TargetPosition != OriginPosition)
            {
                User.TilePosition = OriginPosition;
            }
        }
    }
}