using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleItem : IItem
{
    public BattleItemType BattleItemType { get; set; }

    public int Range { get; set; }
}