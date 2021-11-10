using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity_Project.Scripts.BattleDataScripts
{
    public abstract class TurnActionCommand
    {
        public CharacterUnitScript User { get; protected set; }
        public CharacterUnitScript Target { get; protected set; }
        public TurnAction Action { get; protected set; }
        public Vector3Int OriginPosition { get; protected set; }
        public Vector3Int? TargetPosition { get; protected set; }

        protected TurnActionCommand(CharacterUnitScript user, CharacterUnitScript target, Vector3Int? targetPosition)
        {
            User = user;
            Target = target;
            OriginPosition = user.TilePosition;
            TargetPosition = targetPosition;
        }

        // + + + + | Functions | + + + + 

        public abstract bool IsActionValid();

        public abstract void Execute();
        public abstract void Undo();

        public bool Equals(TurnActionCommand other)
        {
            return (User == other.User &&
                    Target == other.Target &&
                    Action == other.Action &&
                    OriginPosition == other.OriginPosition &&
                    TargetPosition == other.TargetPosition);
        }
    }
}