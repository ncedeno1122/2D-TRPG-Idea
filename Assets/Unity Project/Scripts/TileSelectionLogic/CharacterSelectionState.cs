using Unity_Project.Scripts.BattleDataScripts;
using UnityEngine;

namespace Unity_Project.Scripts.TileSelectionLogic
{
    public class CharacterSelectionState : TileSelectionState
    {
        public CharacterSelectionState(TileSelectionManager tsm) : base(tsm)
        {
        }
    
        public override void Enter()
        {
            Debug.Log("Entered CharacterSelectionState!");
        }

        public override void Exit()
        {
            Debug.Log("Exiting CharacterSelectionState!");
        }

        public override void HandleInput(Vector3Int tilePosition)
        {
            // Initial CharacterUnit Selection
            var characterOnTile = m_TileSelectionManager.GridHelper.GetCharacterOnTile(tilePosition);

            if (characterOnTile)
            {
                // Get Character Data on Tile
                var charData = characterOnTile.UnitData;
                var equippedBattleItem = characterOnTile.EquippedBattleItem;
                var equippedBattleItemRange = equippedBattleItem ? equippedBattleItem.Range : 0;

                TurnAction desiredAction = TurnAction.WAIT;
                if (equippedBattleItem)
                {
                    desiredAction = equippedBattleItem is IWeapon ? TurnAction.ATTACK : TurnAction.HEAL;
                }
                
                // PaintInteractionRange
                m_TileSelectionManager.GridHelper.PaintInteractionRange(charData.Prototype.MoveRange, equippedBattleItemRange, characterOnTile.TilePosition, desiredAction);
            }
            
            m_TileSelectionManager.AdvanceState();
        }

        public override void HandleInput(TurnAction action)
        {
            //
        }
    }
}
