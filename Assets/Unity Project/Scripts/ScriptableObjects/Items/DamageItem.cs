using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New DamageItem", menuName = "ScriptableObjects/Items/DamageItem", order = 2)]
public class DamageItem : Item
{
    [Header("Prototype")]
    public DamageItem Prototype;

    [Header("Damage Stats (Overrides if Prototype is defined)")]
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
    private int range;

    public int Range
    {
        get { return (Prototype && range == 0) ? Prototype.Range : range; }
        set => range = value;
    }
}