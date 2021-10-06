using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class TerrainScriptableTile : Tile
{
    //public Sprite m_Preview;
    public int MovementCost;

    public bool IsPassable;

    // + + + + | Functions | + + + +

    public override void RefreshTile(Vector3Int location, ITilemap tilemap)
    {
        for (int yd = -1; yd <= 1; yd++)
        {
            for (int xd = -1; xd <= 1; xd++)
            {
                Vector3Int position = new Vector3Int(location.x + xd, location.y + yd, location.z);
                if (HasTerrainScriptableTile(tilemap, position))
                {
                    tilemap.RefreshTile(position);
                }
            }
        }
    }

    public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
    {
        //tileData.sprite = m_Preview;
        tileData.sprite = sprite;
        tileData.color = Color.white;
        var m = tileData.transform;
        //m.SetTRS(Vector3.zero, GetRotation((byte) mask), Vector3.one);
        tileData.transform = m;
        tileData.flags = TileFlags.LockTransform;
        tileData.colliderType = ColliderType.None;
    }

    private bool HasTerrainScriptableTile(ITilemap tilemap, Vector3Int position)
    {
        return tilemap.GetTile(position) == this;
    }

#if UNITY_EDITOR

    [MenuItem("Assets/Create/TerrainScriptableTile")]
    public static void CreateTerrainScriptableTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save TerrainScriptableTile", "New TerrainScriptableTile", "Asset", "Save TerrainScriptableTile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TerrainScriptableTile>(), path);
    }

#endif
}