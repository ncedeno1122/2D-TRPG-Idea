using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ItemData", menuName = "ScriptableObjects/Items/Standard Item", order = 1)]
public class ItemData : ScriptableObject, IItem
{
    [Header("Item Data")]
    [SerializeField]
    private string m_ItemName;

    public string ItemName
    {
        get => m_ItemName;
        set => m_ItemName = value;
    }

    [SerializeField]
    private int m_ID;

    public int ID
    {
        get => m_ID;
        set => m_ID = value;
    }

    [SerializeField]
    private int m_Price;

    public int Price
    {
        get => m_Price;
        set => m_Price = value;
    }

    [SerializeField]
    private int m_UsesTotal;

    public int UsesTotal
    {
        get => m_UsesTotal;
        set => m_UsesTotal = value;
    }

    [SerializeField]
    private int m_UsesLeft;

    public int UsesLeft
    {
        get => m_UsesLeft;
        set => m_UsesLeft = value;
    }

    [SerializeField]
    private Sprite m_Icon;

    public Sprite Icon
    {
        get => m_Icon;
        set => m_Icon = value;
    }
}