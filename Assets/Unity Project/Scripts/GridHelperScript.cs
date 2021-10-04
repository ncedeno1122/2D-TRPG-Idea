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

    public Tile WalkableTile, AttackableTile, OriginTile;

    public List<Vector3Int> ValidMoveTiles = new List<Vector3Int>();
    public List<Vector3Int> ValidAttackTiles = new List<Vector3Int>();
    
    
    // + + + + | Functions | + + + +

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
                    ValidMoveTiles.Add(positionToCheck);

                    // Recurse!
                    RecursivelyGetMoveRange(positionToCheck + Vector3Int.up, moveRange - 1 + currTile.MovementCost);
                    RecursivelyGetMoveRange(positionToCheck + Vector3Int.right, moveRange - 1 + currTile.MovementCost);
                    RecursivelyGetMoveRange(positionToCheck + Vector3Int.left, moveRange - 1 + currTile.MovementCost);
                    RecursivelyGetMoveRange(positionToCheck + Vector3Int.down, moveRange - 1 + currTile.MovementCost);
                }
            }
        }
    }

    private void GetActionRange(Vector3Int positionToCheck, int actionRange)
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
        GetActionRange(position, actionRange);
        
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
