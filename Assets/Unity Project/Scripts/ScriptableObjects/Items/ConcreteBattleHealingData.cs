using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ConcreteBattleHealingData", menuName = "ScriptableObjects/BattleItems/ConcreteBattleHealingData", order = 2)]
public class ConcreteBattleHealingData : BattleItemData, IConcreteHealing
{
    [SerializeField]
    private int m_HealingAmount;

    public int HealingAmount { get => m_HealingAmount; set => m_HealingAmount = value; }
}