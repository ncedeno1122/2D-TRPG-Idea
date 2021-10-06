using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Tilemaps;

public enum GridHelperState
{
    NO_TILE_SELECTED = 0,
    FIRST_TILE_SELECTED = 1,
    SECOND_TILE_SELECTED = 2,
}

public class GridHelperScript : MonoBehaviour
{
    [Header("Tilemaps and Entities")]
    public Tilemap BattleTilemap;

    public Tilemap ActionTilemap;
    public Transform TileEntities; // Helps me keep track of ALL TileEntities

    [Header("Tile Types")]
    public Tile WalkableTile;

    public Tile AttackableTile;
    public Tile OriginTile;
    public Tile HealableTile;

    [Header("Other Information")]
    private List<Vector3Int> ValidMoveTiles = new List<Vector3Int>();

    private List<Vector3Int> ValidAttackTiles = new List<Vector3Int>();

    [SerializeField]
    private List<CharacterUnitScript> CharacterUnits;

    public GridHelperState CurrentState = GridHelperState.NO_TILE_SELECTED;

    public Vector3Int SelectedOriginTile, SelectedTargetTile;
    public Queue<Vector3Int> SelectedTilePath;

    private void OnValidate()
    {
        CharacterUnits.Clear();
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
    /// Handles Selection logic per Tile
    /// </summary>
    /// <param name="tilePosition"></param>
    public void SelectTile(Vector3Int tilePosition) // TODO: Deselect Tile if already Selected.
    {
        HandleSelectTile(tilePosition);
    }

    public void HandleSelectTile(Vector3Int tilePosition)
    {
        // Tile info
        var currTileData = BattleTilemap.GetTile(tilePosition) as TerrainScriptableTile;

        // CharacterUnit info
        var characterOnTile = GetCharacterOnTile(tilePosition);

        Debug.Log($"Current Selection State is {CurrentState}!");
        switch (CurrentState)
        {
            // If we haven't selected any Tiles yet,
            case GridHelperState.NO_TILE_SELECTED:
                if (characterOnTile)
                {
                    var charData = characterOnTile.UnitData;
                    var equippedBattleItem = characterOnTile.EquippedBattleItem;
                    int equippedBattleItemRange = equippedBattleItem ? equippedBattleItem.Range : 0;

                    // Mark tile as selected
                    CurrentState = GridHelperState.FIRST_TILE_SELECTED;
                    SelectedOriginTile = tilePosition;

                    // Paint Interaction Range, which defines legal SelectedTargetTile candidates!
                    Debug.Log($"Found character {charData.Name} on SelectedOriginTile {SelectedOriginTile}! Painting Interaction Range.");

                    PaintInteractionRange(charData.Prototype.MoveRange, equippedBattleItemRange, characterOnTile.TilePosition);
                }
                break;

            case GridHelperState.FIRST_TILE_SELECTED:
                if (!ValidMoveTiles.Contains(tilePosition) && !ValidAttackTiles.Contains(tilePosition))
                {
                    Debug.Log($"Second selection {tilePosition} is not in the interaction range. Exiting Selection mode.");
                    // If we click outside the range, clear it all out
                    CurrentState = GridHelperState.NO_TILE_SELECTED;
                    ClearSelectedTiles();
                    ClearInteractionRange();
                    return;
                }
                CurrentState = GridHelperState.SECOND_TILE_SELECTED; // Redundant if unreachable?
                SelectedTargetTile = tilePosition;

                // TODO: Find Path to Target
                SelectedTilePath = FindWalkablePathToTarget(SelectedOriginTile, SelectedTargetTile);

                foreach (Vector3Int position in SelectedTilePath)
                {
                    ActionTilemap.SetTile(position, OriginTile);
                }

                // TODO: Pull up context UI and all that.
                break;

            case GridHelperState.SECOND_TILE_SELECTED: // Unreachable?
                break;
        }
    }

    private void ClearSelectedTiles()
    {
        // Clear selected Tiles
        SelectedOriginTile = Vector3Int.zero;
        SelectedTargetTile = Vector3Int.zero;
        SelectedTilePath.Clear();
    }

    private void ClearInteractionRange()
    {
        ActionTilemap.ClearAllTiles();
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

    private int GetManhattanDistance(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    // + + + + | A* Implementation | + + +

    private class GridNode
    {
        public GridNode Parent;
        public Vector3Int Position;
        public int f, g, h;

        public GridNode(Vector3Int position)
        {
            Position = position;
        }

        public override bool Equals(System.Object o)
        {
            if (o == null) return false;
            GridNode other = o as GridNode;
            if (other == null) return false;
            else return Equals(other);
        }

        public bool Equals(GridNode other)
        {
            if (other == null) return false;
            return (this.Position.Equals(other.Position));
        }
    }

    /// <summary>
    /// Uses the A* Algorithm to find a Path within the ValidMoveTiles List
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    private Queue<Vector3Int> FindWalkablePathToTarget(Vector3Int origin, Vector3Int target)
    {
        GridNode start = new GridNode(origin);
        GridNode end = new GridNode(target);
        List<GridNode> openList = new List<GridNode>();
        List<GridNode> closedList = new List<GridNode>();
        Queue<Vector3Int> path = new Queue<Vector3Int>();
        List<GridNode> adjacents;
        GridNode current = start;

        openList.Add(start);

        while (openList.Count != 0 && !closedList.Exists(x => x.Position == end.Position))
        {
            current = openList[0];
            openList.Remove(current);
            closedList.Add(current);
            adjacents = GetInteractableAdjacents(current);
            ActionTilemap.SetTile(current.Position, HealableTile);

            foreach (GridNode node in adjacents)
            {
                if (!closedList.Contains(node))
                {
                    if (!openList.Contains(node))
                    {
                        node.Parent = current;
                        node.g = GetManhattanDistance(origin, node.Position);
                        node.h = GetManhattanDistance(node.Position, target);
                        node.f = node.g + node.h;
                        openList.Add(node);
                        // Re-sort the openList!
                        openList = openList.OrderBy(listNode => listNode.f).ToList<GridNode>();
                    }
                }
            }
        }

        // If there's no path, return null...
        if (!closedList.Exists(x => x.Position == end.Position))
        {
            return null;
        }

        // Otherwise, assemble and return the path!
        GridNode temp = closedList[closedList.IndexOf(current)];
        if (temp == null) return null;
        do
        {
            path.Enqueue(temp.Position);
            ActionTilemap.SetTile(temp.Position, OriginTile);
            temp = temp.Parent;
        } while (temp != start && temp != null);

        Debug.Log($"Size of closedList: {closedList.Count}. Size of openList: {openList.Count}. Length of path: {path.Count}");
        return path;
    }

    private List<GridNode> GetInteractableAdjacents(GridNode node)
    {
        List<Vector3Int> validTiles = new List<Vector3Int>(); // TODO: Consider making this its own data OR referencing one of the lists and adding the other
        validTiles.AddRange(ValidMoveTiles);
        validTiles.AddRange(ValidAttackTiles);
        List<GridNode> adjacents = new List<GridNode>();

        //
        Vector3Int upNeighbor = node.Position + Vector3Int.up;
        if (validTiles.Contains(upNeighbor))
        {
            adjacents.Add(new GridNode(upNeighbor));
        }

        Vector3Int rightNeighbor = node.Position + Vector3Int.right;
        if (validTiles.Contains(rightNeighbor))
        {
            adjacents.Add(new GridNode(rightNeighbor));
        }

        Vector3Int leftNeighbor = node.Position + Vector3Int.left;
        if (validTiles.Contains(leftNeighbor))
        {
            adjacents.Add(new GridNode(leftNeighbor));
        }

        Vector3Int downNeighbor = node.Position + Vector3Int.down;
        if (validTiles.Contains(downNeighbor))
        {
            adjacents.Add(new GridNode(downNeighbor));
        }

        //

        return adjacents;
    }
}