using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem
{
    public string ItemName { get; set; }
    public int Price { get; set; }
    public int UsesTotal { get; set; }
    public int UsesLeft { get; set; }
    public int ID { get; set; }
    public Sprite Icon { get; set; }
}