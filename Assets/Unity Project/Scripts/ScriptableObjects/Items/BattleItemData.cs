using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleItemData : ScriptableObject, IBattleItem
{
    [Header("BattleItem Data")]
    [SerializeField]
    protected BattleItemType m_BattleItemType;

    public BattleItemType BattleItemType { get => m_BattleItemType; set => m_BattleItemType = value; }

    [SerializeField]
    protected int m_Range;

    public int Range { get => m_Range; set => m_Range = value; }

    // From IItem

    [SerializeField]
    protected string m_ItemName;

    public string ItemName { get => m_ItemName; set => m_ItemName = value; }

    [SerializeField]
    protected int m_Price;

    public int Price { get => m_Price; set => m_Price = value; }

    [SerializeField]
    protected int m_UsesTotal;

    public int UsesTotal { get => m_UsesTotal; set => m_UsesTotal = value; }

    [SerializeField]
    protected int m_UsesLeft;

    public int UsesLeft { get => m_UsesLeft; set => m_UsesLeft = value; }

    [SerializeField]
    protected int m_ID;

    public int ID { get => m_ID; set => m_ID = value; }

    [SerializeField]
    protected Sprite m_Icon;

    public Sprite Icon { get => m_Icon; set => m_Icon = value; }

    //
}