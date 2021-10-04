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
    
    // Start is called before the first frame update
    void Start()
    {
        PaintInteractionRange(3, 1, Vector3Int.zero);

        var tile = BattleTilemap.GetTile(new Vector3Int(1, 3, 0)) as TerrainScriptableTile;
        if (tile)
        {
            //Debug.Log($"The name of tile (1, 3) is {tile.name}");
            //Debug.Log($"The movement cost of tile (1, 3) is {tile.MovementCost}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // + + + + | Functions | + + + + 
    private void PaintInteractionRange(int range, int actionRange, Vector3Int position)
    {
        for (int x = position.x - (range + actionRange); x <= position.x + (range + actionRange); x++)
        {
            for (int y = position.y - (range + actionRange); y <= position.y + (range + actionRange); y++)
            {
                // Get Current Position as tile
                var currTilePosition = new Vector3Int(x, y, 0);
                var currTile = BattleTilemap.GetTile(currTilePosition) as TerrainScriptableTile;
                var manhattanDist = GetManhattanDistance(position, currTilePosition);
                var finalDist = manhattanDist - currTile.MovementCost; // Negative move costs INCREASE the final distance
                // Check if Tile is valid
                if (finalDist <= range + actionRange)
                {
                    //Debug.Log($"Range was {range}; Manhattan distance from {position} to {currTilePosition} is {manhattanDist}, - {currTile.MovementCost} is {finalDist}!");
                    if (GetManhattanDistance(position, currTilePosition) > range ||  !currTile.IsPassable)
                    {
                        ValidAttackTiles.Add(currTilePosition);
                    }
                    else
                    {
                        ValidMoveTiles.Add(currTilePosition);
                    }
                }
            }
        }

        foreach (Vector3Int tilePosition in ValidMoveTiles)
        {
            ActionTilemap.SetTile(tilePosition, WalkableTile);
        }
        
        foreach (Vector3Int tilePosition in ValidAttackTiles)
        {
            ActionTilemap.SetTile(tilePosition, AttackableTile);
        }
    }

    private int GetManhattanDistance(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}
