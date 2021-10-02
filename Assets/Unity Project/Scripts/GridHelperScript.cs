using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridHelperScript : MonoBehaviour
{
    public Tilemap BattleTilemap;
    public Tilemap ActionTilemap;

    public Tile WalkableTile, AttackableTile;
    
    
    // Start is called before the first frame update
    void Start()
    {
        PaintInteractionRange(3, 2, Vector3Int.zero);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // + + + + | Functions | + + + + 

    private void PaintInteractionRange(int range, int actionRange, Vector3Int position)
    {
        for (int x = position.x - (range + actionRange); x < position.x + (range + actionRange); x++)
        {
            for (int y = position.y - (range + actionRange); y < position.y + (range + actionRange); y++)
            {
                int manhattanDist = Mathf.Abs(x + position.x) + Mathf.Abs(y + position.y);
                if (manhattanDist < (range + actionRange))
                {
                    var tileToUse = (manhattanDist > range - 1) ? AttackableTile : WalkableTile;
                    ActionTilemap.SetTile(new Vector3Int(x, y, 0), tileToUse);
                }
            }
        }
    }
}
