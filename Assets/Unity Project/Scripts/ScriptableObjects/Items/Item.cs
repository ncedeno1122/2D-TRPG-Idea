using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObjects/Item", order = 3)]
public class Item : ScriptableObject
{
    public string Name;
    public int Price;
}