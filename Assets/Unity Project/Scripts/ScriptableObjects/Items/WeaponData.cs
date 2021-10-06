using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WeaponData", menuName = "ScriptableObjects/BattleItems/WeaponData", order = 1)]
public class WeaponData : BattleItemData, IWeapon
{
    [Header("Weapon Data")]
    [SerializeField]
    private DamageType m_DamageType;

    public DamageType DamageType { get => m_DamageType; set => m_DamageType = value; }

    [SerializeField]
    private WeaponElement m_WeaponElement;

    public WeaponElement WeaponElement { get => m_WeaponElement; set => m_WeaponElement = value; }

    [SerializeField]
    private int m_BaseDamageAmount;

    public int BaseDamageAmount { get => m_BaseDamageAmount; set => m_BaseDamageAmount = value; }

    [SerializeField, Range(0f, 1f)]
    private float m_Accuracy;

    public float Accuracy { get => m_Accuracy; set => m_Accuracy = value; }
}