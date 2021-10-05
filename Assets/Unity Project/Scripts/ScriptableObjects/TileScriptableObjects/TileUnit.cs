using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
    WARRIOR = 0,
    ARCHER = 1,
    MAGE = 2
}

[CreateAssetMenu(fileName = "New TileUnit", menuName = "ScriptableObjects/TileUnits/TileUnit", order = 1)]
public class TileUnit : ScriptableObject
{
    public UnitType UnitType;

    [Header("Stats")]
    public int Level = 1;

    public int MaxHP;

    public int PhysicalAttack, MagicalAttack;
    public int PhysicalDefense, MagicalDefense;

    [Header("Tile-Related Info")]
    public int MoveRange;
}