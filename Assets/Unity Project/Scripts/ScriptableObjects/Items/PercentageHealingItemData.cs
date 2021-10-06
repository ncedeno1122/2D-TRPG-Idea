using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ConcreteHealingItemData", menuName = "ScriptableObjects/Items/PercentageHealingItemData", order = 3)]
public class PercentageHealingItemData : ItemData, IPercentageHealing
{
    [SerializeField, Range(0f, 1f)]
    private float m_PercentageHealing;

    public float PercentageHealing { get => m_PercentageHealing; set => m_PercentageHealing = value; }
}