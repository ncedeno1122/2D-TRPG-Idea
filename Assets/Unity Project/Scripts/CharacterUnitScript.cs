using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUnitScript : MonoBehaviour
{
    public Vector3Int TilePosition;
    private bool IsMoving;

    public CharacterUnit UnitData;
    public Grid Grid;

    private void Start()
    {
        gameObject.name = UnitData.Name;

        // TODO: Should be optional somehow.
        TilePosition = Vector3Int.FloorToInt(transform.position);
        transform.position = Grid.GetCellCenterWorld(TilePosition);

        for (int i = 0; i < UnitData.Inventory.Length - 1; i++)
        {
            var item = UnitData.Inventory[i];

            if (item)
            {
                if (item is Weapon)
                {
                    var weapon = item as Weapon;
                    Debug.Log($"{weapon.Name} is a { weapon.WeaponElement } { weapon.DamageType } weapon with { weapon.BaseDamageAmount } base damage, {weapon.WeaponAccuracy } accuracy, and an attack range of { weapon.AttackRange }.");
                }
                else
                {
                    Debug.Log($"{item.Name} has a price of {item.Price}.");
                }
            }
        }
    }

    // + + + + | Functions | + + + +

    public void MoveTo(Vector3Int newPosition)
    {
        if (!IsMoving)
        {
            var moveToCRT = MoveToCRT(transform.position, Grid.GetCellCenterWorld(newPosition));
            StartCoroutine(moveToCRT);
        }
    }

    private IEnumerator MoveToCRT(Vector3 oldPosition, Vector3 newPosition)
    {
        int numSteps = 100;
        float moveSpeed = 0.0125f;

        IsMoving = true;

        for (int i = 0; i <= numSteps; i++)
        {
            transform.position = Vector3.Lerp(oldPosition, newPosition, (float)i / numSteps);
            yield return new WaitForSecondsRealtime(moveSpeed);
        }

        // When done,
        IsMoving = false;
        TilePosition = Vector3Int.FloorToInt(newPosition);
    }
}