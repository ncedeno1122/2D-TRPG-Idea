using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "ScriptableObjects/BattleItems/BattleWeapon", order = 1)]
public class BattleWeapon : ScriptableObject
{
    public Item ItemData;
}