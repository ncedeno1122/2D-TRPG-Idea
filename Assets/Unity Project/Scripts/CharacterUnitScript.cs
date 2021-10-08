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
    private const float MOVEMENT_SPEED = 25f;
    private bool IsMoving;
    private bool IsDead;
    private Grid m_Grid; // TODO: Do we even need a reference to the Grid?
    private Animator m_Animator;

    private void Start()
    {
        m_Grid = transform.parent.parent.GetComponent<Grid>();
        m_Animator = GetComponent<Animator>();

        gameObject.name = UnitData.Name;

        // Should be some way to do this in editor...
        transform.position = m_Grid.GetCellCenterWorld(TilePosition);
        TilePosition = Vector3Int.FloorToInt(transform.position);
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

    /// <summary>
    /// Handles the FollowPathCRT, only allowing movement if not currently moving.
    /// </summary>
    /// <param name="path"></param>
    public void FollowPath(List<Vector3Int> path)
    {
        Vector3Int[] pathArr = path.ToArray();

        if (!IsMoving)
        {
            var followPathCRT = FollowPathCRT(pathArr);
            StartCoroutine(followPathCRT);
        }
    }

    /// <summary>
    /// An IEnumerator that makes the character follow an array of path positions, following a path!
    /// </summary>
    /// <param name="pathArr"></param>
    /// <returns></returns>
    private IEnumerator FollowPathCRT(Vector3Int[] pathArr)
    {
        float moveSpeed = 0.0175f; // Oroginal value: 0.0125f
        Vector3 gridCenterPosition;

        IsMoving = true;
        m_Animator.SetBool("IsMoving", true);

        // For loops make me kinda scared in this instance...
        for (int i = pathArr.Length - 1; i >= 0; i--)
        {
            var path = pathArr[i];
            gridCenterPosition = m_Grid.GetCellCenterWorld(path);

            // Animator: Run in the direction the next cell is
            var difference = Vector3Int.CeilToInt(gridCenterPosition - transform.position);
            m_Animator.SetInteger("HorizontalMoveDirection", difference.x);
            m_Animator.SetInteger("VerticalMoveDirection", difference.y);

            while (transform.position != gridCenterPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, m_Grid.GetCellCenterWorld(path), MOVEMENT_SPEED * Time.deltaTime);
                yield return new WaitForSeconds(moveSpeed);
            }
        }

        // When finished,
        IsMoving = false;
        m_Animator.SetInteger("HorizontalMoveDirection", 0);
        m_Animator.SetInteger("VerticalMoveDirection", 0);
        m_Animator.SetBool("IsMoving", false);
        TilePosition = Vector3Int.FloorToInt(pathArr[0]);
    }
}