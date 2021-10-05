using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New HealingItem", menuName = "ScriptableObjects/HealingItem", order = 4)]
public class HealingItem : Item
{
    [Header("Prototype")]
    public HealingItem Prototype;

    [Header("Healing Stats (Overrides if Prototype is defined)")]
    [SerializeField]
    private int baseHealingAmount;

    public int BaseHealingAmount
    {
        get
        {
            return Prototype && baseHealingAmount == 0 ? Prototype.BaseHealingAmount : baseHealingAmount;
        }
        set => baseHealingAmount = value;
    }

    [SerializeField] // TODO: Duplication with DamageItem?? Hrmmmrmmmm.....
    private int range;

    public int Range
    {
        get { return (Prototype && range == 0) ? Prototype.Range : range; }
        set => range = value;
    }
}