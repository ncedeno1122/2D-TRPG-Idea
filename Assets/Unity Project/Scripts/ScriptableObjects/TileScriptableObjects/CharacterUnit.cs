using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An enumeration of whom the Character Unit belongs to.
/// </summary>
public enum Allegiance
{
    PLAYER = 1,
    ENEMY = 2,
    ALLY = 3,
    //NEUTRAL=4
}

[CreateAssetMenu(fileName = "New CharacterUnit", menuName = "ScriptableObjects/TileUnits/CharacterUnit", order = 2)]
public class CharacterUnit : ScriptableObject
{
    [Header("Identification")]
    public string Name;

    public int ID; // TODO: Would be cool to assign a unique ID based on a list of IDs maintained in the Editor somehow.
    public Allegiance Allegiance;

    [Header("Prototype Base Stats")]
    public TileUnit Prototype;

    [Header("Progression")]
    public int XPToNextLevel;

    public int CurrentXP;
}