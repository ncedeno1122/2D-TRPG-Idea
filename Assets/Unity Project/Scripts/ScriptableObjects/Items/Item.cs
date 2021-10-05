using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObjects/Items/Item", order = 1)]
public class Item : ScriptableObject
{
    public string Name;
    public int Price;
    public int UsesTotal, UsesLeft;

    // TODO: Add icon Sprite(s)
}