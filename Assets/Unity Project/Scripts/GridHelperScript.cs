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

    public Tile WalkableTile, AttackableTile;

    public List<Vector3Int> ValidMoveTiles = new List<Vector3Int>();
    public List<Vector3Int> ValidAttackTiles = new List<Vector3Int>();
    
    
    // + + + + | Functions | + + + +

    /// <summary>
    /// Recursively finds all walkable and attackable Tiles around an origin point.
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="positionToCheck"></param>
    /// <param name="range"></param>
    /// <param name="actionRange"></param>
    private void RecursivelyGetInteractionRange(Vector3Int origin, Vector3Int positionToCheck, int range,
        int actionRange)
    {
        // Do we know this Tile already?
        if (ValidMoveTiles.Contains(positionToCheck) || ValidAttackTiles.Contains(positionToCheck))
        {
            return;
        }
        // If the Tile is unknown,
        else
        {
            var currTile = BattleTilemap.GetTile(positionToCheck) as TerrainScriptableTile;
            
            if (currTile)
            {
                int manhattanFromOrigin = GetManhattanDistance(origin, positionToCheck);
                
                // Is the Tile passable?
                if (currTile.IsPassable)
                {
                    // Then, is the Tile walkable?
                    if (manhattanFromOrigin + currTile.MovementCost <= range)
                    {
                        ValidMoveTiles.Add(positionToCheck);

                        // Recurse!
                        RecursivelyGetInteractionRange(origin, positionToCheck + Vector3Int.up, range, actionRange);
                        RecursivelyGetInteractionRange(origin, positionToCheck + Vector3Int.left, range, actionRange);
                        RecursivelyGetInteractionRange(origin, positionToCheck + Vector3Int.right, range, actionRange);
                        RecursivelyGetInteractionRange(origin, positionToCheck + Vector3Int.down, range, actionRange);
                    }
                    // If not, is it Attackable?
                    else
                    {
                        if (manhattanFromOrigin + currTile.MovementCost <= range + actionRange)
                        {
                            ValidAttackTiles.Add(positionToCheck);
                        }
                    }
                }

                // If not, is it at least Attackable?
                else
                {
                    if (manhattanFromOrigin + currTile.MovementCost <= range + actionRange)
                    {
                        ValidAttackTiles.Add(positionToCheck);
                    }
                }
            }
        }
    }

    public void PaintInteractionRange(int range, int actionRange, Vector3Int position)
    {
        ActionTilemap.ClearAllTiles();

        RecursivelyGetInteractionRange(position, position, range, actionRange);
        
        foreach (Vector3Int tilePosition in ValidMoveTiles)
        {
            ActionTilemap.SetTile(tilePosition, WalkableTile);
        }
        
        foreach (Vector3Int tilePosition in ValidAttackTiles)
        {
            ActionTilemap.SetTile(tilePosition, AttackableTile);
        }
        
        ValidMoveTiles.Clear();
        ValidAttackTiles.Clear();
    }

    private int GetManhattanDistance(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}
