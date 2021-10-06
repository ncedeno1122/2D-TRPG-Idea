using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUnitScript : MonoBehaviour
{
    public int CurrentHP;
    public Vector3Int TilePosition;
    public CharacterUnit UnitData;

    [SerializeField] // TODO: It would be really nice to expose this somehow. [SerializeReference] won't work...
    public IItem[] Inventory = new IItem[INVENTORY_SIZE]; // It's said that an abstract base class would work here...

    public BattleItemData EquippedBattleItem;

    private const int INVENTORY_SIZE = 5;
    private bool IsMoving;
    private bool IsDead;
    private Grid m_Grid; // TODO: Do we even need a reference to the Grid?

    private void Start()
    {
        m_Grid = transform.parent.parent.GetComponent<Grid>();

        gameObject.name = UnitData.Name;

        // TODO: Should be optional somehow.
        TilePosition = Vector3Int.FloorToInt(transform.position);
        transform.position = m_Grid.GetCellCenterWorld(TilePosition);
    }

    private void OnValidate()
    {
        // Helps prevent resizing of the StoredInventory array in the editor!
        if (Inventory.Length != INVENTORY_SIZE)
        {
            Debug.LogWarning("Don't change the StoredInventory field's array size!");
            System.Array.Resize(ref Inventory, INVENTORY_SIZE);
        }

        // Equip first weapon in inventory if undefined
        if (!EquippedBattleItem)
        {
            for (int i = 0; i < Inventory.Length - 1; i++)
            {
                var item = Inventory[i];
                if (item is IBattleItem)
                {
                    EquippedBattleItem = item as BattleItemData;
                }
            }
        }
    }

    // + + + + | Functions | + + + +

    public void MoveTo(Vector3Int newPosition)
    {
        if (!IsMoving)
        {
            var moveToCRT = MoveToCRT(transform.position, m_Grid.GetCellCenterWorld(newPosition));
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