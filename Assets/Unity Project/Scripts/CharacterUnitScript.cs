using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUnitScript : TileEntity
{
    public int CurrentHP;
    public CharacterUnit UnitData;

    // TODO: It would be really nice to expose this somehow. [SerializeReference] won't work...
    public ItemData[] Inventory = new ItemData[INVENTORY_SIZE]; // It's said that an abstract base class would work here...

    public BattleItemData EquippedBattleItem;

    private const int INVENTORY_SIZE = 5;
    private const float MOVEMENT_SPEED = 30f;
    private bool m_isMoving;
    private bool IsDead;
    
    private IEnumerator m_FollowCRT;
    
    private Animator m_Animator;
    
    private void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    private void OnValidate()
    {
        // Set Name according to data
        if (UnitData)
        {
            gameObject.name = UnitData.Name;
        }
        
        // Align to Grid on validation
        m_Grid = transform.parent.parent.GetComponent<Grid>();
        TilePosition = Vector3Int.FloorToInt(transform.position); //
        transform.position = m_Grid.GetCellCenterWorld(TilePosition);

        // Helps prevent resizing of the StoredInventory array in the editor!
        if (Inventory.Length != INVENTORY_SIZE)
        {
            Debug.LogWarning("Don't change the StoredInventory field's array size!");
            Array.Resize(ref Inventory, INVENTORY_SIZE);
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
    /// Moves a CharacterUnitScript to a given tilePosition without animation or anything fancy.
    /// </summary>
    /// <param name="tilePosition"></param>
    public void SnapToPosition(Vector3Int tilePosition)
    {
        StopFollowPathCRT();
        transform.position = m_Grid.GetCellCenterWorld(tilePosition);
        TilePosition = tilePosition;
    }
    
    /// <summary>
    /// Handles the FollowPathCRT, only allowing movement if not currently moving.
    /// </summary>
    /// <param name="path"></param>
    public void FollowPath(List<Vector3Int> path)
    {
        Vector3Int[] pathArr = path.ToArray();

        if (!m_isMoving)
        {
            m_FollowCRT = FollowPathCRT(pathArr);
            StartCoroutine(m_FollowCRT);
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

        m_isMoving = true;
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
        m_isMoving = false;
        m_Animator.SetInteger("HorizontalMoveDirection", 0);
        m_Animator.SetInteger("VerticalMoveDirection", 0);
        m_Animator.SetBool("IsMoving", false);
        //TilePosition = Vector3Int.FloorToInt(pathArr[0]); Position is committed when the TileActionCommand is confirmed.
    }

    private void StopFollowPathCRT()
    {
        StopCoroutine(m_FollowCRT);
        m_isMoving = false;
        m_Animator.SetInteger("HorizontalMoveDirection", 0);
        m_Animator.SetInteger("VerticalMoveDirection", 0);
        m_Animator.SetBool("IsMoving", false);
    }

    private bool HasWeaponItem()
    {
        for (int i = 0; i < Inventory.Length - 1; i++)
        {
            if (Inventory[i] is WeaponData)
            {
                return true;
            }
        }
        
        if (EquippedBattleItem)
        {
            return EquippedBattleItem is WeaponData;
        }
        
        return false;
    }

    private bool HasBattleHealingItem()
    {
        for (int i = 0; i < Inventory.Length - 1; i++)
        {
            if (Inventory[i] is ConcreteBattleHealingData || Inventory[i] is PercentageBattleHealingData)
            {
                return true;
            }
        }

        if (EquippedBattleItem)
        {
            return EquippedBattleItem is ConcreteBattleHealingData || EquippedBattleItem is PercentageBattleHealingData;
        }
        
        return false;
    }
    
    // TurnAction functions
    // TODO: Implement CharacterUnitScript abstraction 'TileEntity' and other children that satisfy these function conditions.
    public bool CanTalkWith(TileEntity other) => false;
    
    public bool CanInteractWith(TileEntity other) => false;

    public bool CanAttack(TileEntity other)
    {
        if (!HasWeaponItem()) return false;
        if (!(other is CharacterUnitScript otherCus))
            return false; // TODO: For now. Perhaps an IAttackable or ITargetable interface could be useful for things like breakable walls.
        switch (UnitData.Allegiance)
        {
            case Allegiance.PLAYER:
                return otherCus.UnitData.Allegiance == Allegiance.ENEMY;
            case Allegiance.ALLY:
                return otherCus.UnitData.Allegiance == Allegiance.ENEMY;
            case Allegiance.ENEMY:
                return otherCus.UnitData.Allegiance == Allegiance.PLAYER || otherCus.UnitData.Allegiance == Allegiance.ALLY;
        }

        return false; // TODO: For now. Perhaps an IAttackable or ITargetable interface could be useful for things like breakable walls.
    }

    public bool CanHeal(TileEntity other)
    {
        if (!HasBattleHealingItem()) return false;
        if (!(other is CharacterUnitScript otherCus))
            return false; // TODO: For now. Perhaps an IAttackable or ITargetable interface could be useful for things like breakable walls.
        switch (UnitData.Allegiance)
        {
            case Allegiance.PLAYER:
                return otherCus.UnitData.Allegiance == Allegiance.PLAYER || otherCus.UnitData.Allegiance == Allegiance.ALLY;
            case Allegiance.ALLY:
                return otherCus.UnitData.Allegiance == Allegiance.PLAYER || otherCus.UnitData.Allegiance == Allegiance.ALLY;
            case Allegiance.ENEMY:
                return otherCus.UnitData.Allegiance == Allegiance.ENEMY;
        }

        return false;
    }

    public bool CanUnlockChest(TileEntity other) => false; // TODO: Check inventory for key OR if theif

    public bool CanUseItems()
    {
        for (int i = 0; i < Inventory.Length - 1; i++)
        {
            if (Inventory[i] != null) return true;
        }
        
        // Finally, do we have an equipped item?
        return EquippedBattleItem;
    }

    public bool CanTrade(TileEntity other)
    {
        if (!(other is CharacterUnitScript otherCus)) return false;
        if (UnitData.Allegiance == otherCus.UnitData.Allegiance && UnitData.Allegiance != Allegiance.ALLY)
        {
            // Can only trade if either Unit doesn't have an empty Inventory
            return CanUseItems() || otherCus.CanUseItems(); // TODO: Really checking if inventory is empty. Helper function suits this better.
        }
        
        return false;
    }

    public bool CanUseConvoy() => false; // TODO: Must be certain classes
}