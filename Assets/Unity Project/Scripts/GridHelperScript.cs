using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Tilemaps;

public class GridHelperScript : MonoBehaviour
{
    public Tilemap BattleTilemap;
    public Tilemap ActionTilemap;
    public Transform TileEntities; // Helps me keep track of ALL TileEntities

    public Tile WalkableTile, AttackableTile, OriginTile;

    public List<Vector3Int> ValidMoveTiles = new List<Vector3Int>();
    public List<Vector3Int> ValidAttackTiles = new List<Vector3Int>();

    public List<CharacterUnitScript> CharacterUnits;

    private void Start()
    {
        foreach (Transform child in TileEntities)
        {
            var cus = child.GetComponent<CharacterUnitScript>();
            if (cus)
            {
                //Debug.Log($"Added {child.name} to CharacterUnits list!");
                CharacterUnits.Add(cus);
            }
        }
    }

    // + + + + | Functions | + + + +

    /// <summary>
    /// "Selects" the Tile under the cursor, helping UI find what it needs to.
    /// </summary>
    /// <param name="tilePosition"></param>
    public void SelectTile(Vector3Int tilePosition)
    {
        // Tile info
        var currTileData = BattleTilemap.GetTile(tilePosition) as TerrainScriptableTile;

        // CharacterUnit info
        var characterOnTile = GetCharacterOnTile(tilePosition);

        //

        if (currTileData)
        {
            Debug.Log($"Tile is a {currTileData.name}, movement cost of {currTileData.MovementCost}, and its passability is {currTileData.IsPassable}.");
        }
        if (characterOnTile)
        {
            var charData = characterOnTile.UnitData;
            Debug.Log($"Character on Tile is {charData.Name}, a {charData.Allegiance} {charData.Prototype.name}.");
        }
    }

    private CharacterUnitScript GetCharacterOnTile(Vector3Int tilePosition)
    {
        foreach (CharacterUnitScript cus in CharacterUnits)
        {
            if (cus.TilePosition == tilePosition)
            {
                return cus;
            }
        }

        return null;
    }

    /// <summary>
    /// Recursively finds all valid movement tiles within the moveRange
    /// </summary>
    /// <param name="positionToCheck"></param>
    /// <param name="moveRange"></param>
    private void RecursivelyGetMoveRange(Vector3Int positionToCheck, int moveRange)
    {
        // If the Tile is unknown and within our WalkableRange
        if (moveRange >= 0)
        {
            var currTile = BattleTilemap.GetTile(positionToCheck) as TerrainScriptableTile;

            if (currTile)
            {
                // Is the tile passable?
                if (currTile.IsPassable)
                {
                    if (!ValidMoveTiles.Contains(positionToCheck))
                    {
                        ValidMoveTiles.Add(positionToCheck);
                    }

                    // Recurse!
                    RecursivelyGetMoveRange(positionToCheck + Vector3Int.up, moveRange - 1 + currTile.MovementCost);
                    RecursivelyGetMoveRange(positionToCheck + Vector3Int.right, moveRange - 1 + currTile.MovementCost);
                    RecursivelyGetMoveRange(positionToCheck + Vector3Int.left, moveRange - 1 + currTile.MovementCost);
                    RecursivelyGetMoveRange(positionToCheck + Vector3Int.down, moveRange - 1 + currTile.MovementCost);
                }
            }
        }
    }

    /// <summary>
    /// Finds the Action range for a defined list of ValidMoveTiles
    /// </summary>
    /// <param name="actionRange"></param>
    private void GetActionRange(int actionRange)
    {
        foreach (Vector3Int position in ValidMoveTiles)
        {
            for (int x = position.x - actionRange; x <= position.x + actionRange; x++)
            {
                for (int y = position.y - actionRange; y <= position.y + actionRange; y++)
                {
                    var currTilePosition = new Vector3Int(x, y, 0);
                    if (GetManhattanDistance(position, currTilePosition) > actionRange) continue;
                    if (!ValidAttackTiles.Contains(currTilePosition) &&
                        !ValidMoveTiles.Contains(currTilePosition))
                    {
                        ValidAttackTiles.Add(currTilePosition);
                    }
                }
            }
        }
    }

    public void PaintInteractionRange(int range, int actionRange, Vector3Int position)
    {
        ValidMoveTiles.Clear();
        ValidAttackTiles.Clear();
        ActionTilemap.ClearAllTiles();

        RecursivelyGetMoveRange(position, range);
        GetActionRange(actionRange);

        foreach (Vector3Int tilePosition in ValidMoveTiles)
        {
            ActionTilemap.SetTile(tilePosition, WalkableTile);
        }

        foreach (Vector3Int tilePosition in ValidAttackTiles)
        {
            ActionTilemap.SetTile(tilePosition, AttackableTile);
        }

        ActionTilemap.SetTile(position, OriginTile);
    }

    private int GetManhattanDistance(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}