using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon : IBattleItem
{
    public DamageType DamageType { get; set; }

    public WeaponElement WeaponElement { get; set; }

    public int BaseDamageAmount { get; set; }

    public float Accuracy { get; set; }
}