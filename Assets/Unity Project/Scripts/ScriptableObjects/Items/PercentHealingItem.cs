using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PercentHealingItem", menuName = "ScriptableObjects/PercentHealingItem", order = 5)]
public class PercentHealingItem : Item
{
    [Header("Prototype")]
    public PercentHealingItem Prototype;

    [Header("Healing Stats (Overrides if Prototype is defined)")]
    [SerializeField, Range(0.01f, 1f)]
    private float percentHealingAmount;

    public float PercentHealingAmount
    {
        get
        {
            return Prototype && percentHealingAmount == 0f ? Prototype.PercentHealingAmount : percentHealingAmount;
        }

        set => percentHealingAmount = value;
    }

    [SerializeField] // TODO: Duplication with DamageItem?? Hrmmmrmmmm.....
    private int range;

    public int Range
    {
        get { return (Prototype && range == 0) ? Prototype.Range : range; }
        set => range = value;
    }
}