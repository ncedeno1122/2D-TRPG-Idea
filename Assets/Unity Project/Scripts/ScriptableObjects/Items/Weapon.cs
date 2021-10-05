using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    PHYSICAL = 1,
    MAGICAL = 2,
    TRUE = 3
}

public enum WeaponElement
{
    NONE = 1,
    FIRE = 2,
    LIGHTNING = 3,
    DARK = 4,
    LIGHT = 5
}

[CreateAssetMenu(fileName = "New Weapon", menuName = "ScriptableObjects/Weapon", order = 3)]
public class Weapon : Item
{
    [Header("Prototype")]
    public Weapon Prototype;

    [Header("Weapon Stats (Overriden if Prototype is defined)")]
    [SerializeField]
    private DamageType damageType;

    public DamageType DamageType
    {
        get { return (Prototype && damageType == 0) ? Prototype.DamageType : damageType; }
        set => damageType = value;
    }

    [SerializeField]
    private WeaponElement weaponElement;

    public WeaponElement WeaponElement
    {
        get { return (Prototype && weaponElement == 0) ? Prototype.WeaponElement : weaponElement; }
        set => weaponElement = value;
    }

    [SerializeField]
    private int baseDamageAmount;

    public int BaseDamageAmount
    {
        get { return (Prototype && baseDamageAmount == 0) ? Prototype.BaseDamageAmount : baseDamageAmount; }
        set => baseDamageAmount = value;
    }

    [SerializeField]
    private float weaponAccuracy;

    public float WeaponAccuracy
    {
        get { return (Prototype && weaponAccuracy == 0f) ? Prototype.WeaponAccuracy : weaponAccuracy; }
        set => weaponAccuracy = value;
    }

    [SerializeField]
    private int attackRange;

    public int AttackRange
    {
        get { return (Prototype && attackRange == 0) ? Prototype.AttackRange : attackRange; }
        set => attackRange = value;
    }
}