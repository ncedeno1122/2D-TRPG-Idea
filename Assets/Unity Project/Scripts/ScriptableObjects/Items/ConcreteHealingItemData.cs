using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ConcreteHealingItemData", menuName = "ScriptableObjects/Items/ConcreteHealingItemData", order = 2)]
public class ConcreteHealingItemData : ItemData, IConcreteHealing
{
    [SerializeField]
    public int m_HealingAmount;

    public int HealingAmount { get => m_HealingAmount; set => m_HealingAmount = value; }
}