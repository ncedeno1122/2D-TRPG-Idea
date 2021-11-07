using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity_Project.Scripts.BattleDataScripts;
using UnityEngine;
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
    [SerializeField]
    private List<Vector3Int> ValidMoveTiles = new List<Vector3Int>();
    [SerializeField]
    private List<Vector3Int> ValidActionableTiles = new List<Vector3Int>();

    [SerializeField]
    private List<CharacterUnitScript> CharacterUnits;

    public GridHelperState CurrentState = GridHelperState.NO_TILE_SELECTED;

    public Vector3Int SelectedOriginTile, SelectedTargetTile;
    public CharacterUnitScript SelectedCharacterUnit;
    public List<Vector3Int> SelectedTilePath = new List<Vector3Int>();

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
    /// Clears the A*-found path of Tiles to a particular point.
    /// </summary>
    public void ClearFoundTilePath()
    {
        // Clear selected Tiles
        SelectedOriginTile = Vector3Int.zero;
        SelectedTargetTile = Vector3Int.zero;
        SelectedTilePath.Clear();
    }

    /// <summary>
    /// Clears the ActionTilemap, wiping the entire interaction range.
    /// </summary>
    public void ClearInteractionRange()
    {
        ActionTilemap.ClearAllTiles();
    }

    /// <summary>
    /// Returns a list of all tile positions within a given range.
    /// </summary>
    /// <param name="tilePosition"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public List<Vector3Int> GetTilesInRange(Vector3Int tilePosition, int range)
    {
        List<Vector3Int> tileList = new List<Vector3Int>();
        
        RecursivelyGetTilesInRange(ref tileList, tilePosition, range);

        return tileList;
    }

    /// <summary>
    /// Helper function to get all surrounding tiles within a given range, updates a given list.
    /// </summary>
    /// <param name="tileList"></param>
    /// <param name="positionToCheck"></param>
    /// <param name="moveRange"></param>
    private void RecursivelyGetTilesInRange(ref List<Vector3Int> tileList, Vector3Int positionToCheck, int moveRange)
    {
        if (moveRange < 0) return;
        if (tileList.Contains(positionToCheck)) return;
        RecursivelyGetTilesInRange(ref tileList, positionToCheck + Vector3Int.up, moveRange - 1);
        RecursivelyGetTilesInRange(ref tileList, positionToCheck + Vector3Int.right, moveRange - 1);
        RecursivelyGetTilesInRange(ref tileList, positionToCheck + Vector3Int.left, moveRange - 1);
        RecursivelyGetTilesInRange(ref tileList, positionToCheck + Vector3Int.down, moveRange - 1);
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

            if (!currTile) return;
            // Is the tile passable?
            if (currTile is { } && !currTile.IsPassable) return;
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

    /// <summary>
    /// Finds the Action range for a defined list of ValidMoveTiles
    /// </summary>
    /// <param name="actionRange"></param>
    private void GetActionableRange(int actionRange)
    {
        foreach (Vector3Int position in ValidMoveTiles)
        {
            for (int x = position.x - actionRange; x <= position.x + actionRange; x++)
            {
                for (int y = position.y - actionRange; y <= position.y + actionRange; y++)
                {
                    var currTilePosition = new Vector3Int(x, y, 0);
                    if (GetManhattanDistance(position, currTilePosition) > actionRange) continue;

                    if (!ValidActionableTiles.Contains(currTilePosition) &&
                            !ValidMoveTiles.Contains(currTilePosition))
                        {
                            ValidActionableTiles.Add(currTilePosition);
                        }
                }
            }
        }
    }

    /// <summary>
    /// Clears the current InteractionRange and paints a new one based on CharacterUnit parameters.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="actionRange"></param>
    /// <param name="position"></param>
    /// <param name="desiredAction"></param>
    public void PaintInteractionRange(int range, int actionRange, Vector3Int position, TurnAction desiredAction)
    {
        // TODO: Refactor this... it feels inefficient. KEEP primitive arguments for ease of testing!
        //Debug.Log($"$ Painting range: {range}, actionRange: {actionRange}, position: {position}, and action {desiredAction}.");
        // Clear all Lists of previously painted ranges
        ValidMoveTiles.Clear();
        ValidActionableTiles.Clear();
        ActionTilemap.ClearAllTiles();

        // Get each range
        RecursivelyGetMoveRange(position, range);
        GetActionableRange(actionRange);

        // Get type of tile to use for the Equipped Item
        Tile tileToUse = GetActionableTileForTileAction(desiredAction);

        // Paint the ranges
        foreach (Vector3Int tilePosition in ValidMoveTiles)
        {
            ActionTilemap.SetTile(tilePosition, WalkableTile);
        }

        foreach (Vector3Int tilePosition in ValidActionableTiles)
        {
            ActionTilemap.SetTile(tilePosition, tileToUse);
        }

        ActionTilemap.SetTile(position, OriginTile);
    }

    /// <summary>
    /// Returns a Tile for a specific TileAction 'ta'
    /// </summary>
    /// <param name="ta"></param>
    /// <returns></returns>
    private Tile GetActionableTileForTileAction(TurnAction ta)
    {

        switch (ta)
        {
            case TurnAction.WAIT:
                //return NoneableTile;
            case TurnAction.ATTACK:
                return AttackableTile;
            case TurnAction.HEAL:
                return HealableTile;
            //case TileAction.INTERACT:
            //    return InteractableTile;
            //    break;
            //case TileAction.TALK:
            //    return TalkableTile;
            //    break;
            //case TileAction.CHEST:
            //    return ChestableTile; LOL Chest-able
            //    break;
            default:
                return OriginTile;
        }
    }

    /// <summary>
    /// Returns whether the given tile position is within the ValidMoveTiles list.
    /// </summary>
    /// <param name="tilePosition"></param>
    /// <returns></returns>
    public bool IsTileMovable(Vector3Int tilePosition)
    {
        return ValidMoveTiles.Contains(tilePosition);
    }

    /// <summary>
    /// Returns whether the given tile position is within the ValidActionableTiles list.
    /// </summary>
    /// <param name="tilePosition"></param>
    /// <returns></returns>
    public bool IsTileActionable(Vector3Int tilePosition)
    {
        return ValidActionableTiles.Contains(tilePosition);
    }
    
    public CharacterUnitScript GetCharacterOnTile(Vector3Int tilePosition)
    {
        var boxPosition = new Vector2(tilePosition.x, tilePosition.y);
        var hitCollider = Physics2D.OverlapBox(boxPosition, Vector2.one, 0f);

        if (hitCollider)
        {
            Debug.Log($"Successfully hit {hitCollider.gameObject.name}!");
            
            var cus = hitCollider.gameObject.GetComponent<CharacterUnitScript>();
            return cus;
        }

        return null;
    }

    public List<CharacterUnitScript> GetCharactersInRange(Vector3Int tilePosition, int range)
    {
        List<CharacterUnitScript> targetsInRange = new List<CharacterUnitScript>();
        List<Vector3Int> vec3Range = new List<Vector3Int>();
        
        
        
        
        // Finally, return.
        return targetsInRange;
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
        public bool IsPassable;

        public GridNode(Vector3Int position, bool isPassable)
        {
            Position = position;
            IsPassable = isPassable;
        }

        public override bool Equals(object o)
        {
            if (o == null) return false;
            return o is GridNode other && Equals(other);
        }

        public bool Equals(GridNode other)
        {
            return other != null && this.Position.Equals(other.Position);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// Uses the A* Algorithm to find a Path within the ValidMoveTiles List
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool FindPathToTarget(Vector3Int origin, Vector3Int target, ref List<Vector3Int> path)
    {
        GridNode start = new GridNode(origin, true);
        GridNode end = new GridNode(target, true);
        List<GridNode> openList = new List<GridNode>();
        List<GridNode> closedList = new List<GridNode>();
        List<GridNode> adjacents;
        GridNode current = start;

        openList.Add(start);

        while (openList.Count != 0 && !closedList.Exists(x => x.Position == end.Position))
        {
            current = openList[0];
            openList.Remove(current);
            closedList.Add(current);
            adjacents = GetInteractableAdjacents(current);

            foreach (GridNode node in adjacents)
            {
                if (!closedList.Contains(node) && (node.IsPassable || node.Position == end.Position)) // Allows selected Tile to be reached if Attackable
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
            //Debug.Log("No path found!");
            return false;
        }

        // Otherwise, assemble and return the path!
        GridNode temp = closedList[closedList.IndexOf(current)];
        if (temp == null) return false;
        do
        {
            path.Add(temp.Position);

            temp = temp.Parent;
        } while (temp != start && temp != null);

        //Debug.Log($"Size of closedList: {closedList.Count}. Size of openList: {openList.Count}. Length of path: {path.Count}");
        return true;
    }

    /// <summary>
    /// Helper function for FindWalkablePathToTarget, finds adjacent, Walkable GridNodes.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private List<GridNode> GetInteractableAdjacents(GridNode node)
    {
        List<Vector3Int> validTiles = new List<Vector3Int>(); // TODO: Consider making this its own data OR referencing one of the lists and adding the other
        validTiles.AddRange(ValidMoveTiles);
        validTiles.AddRange(ValidActionableTiles);
        List<GridNode> adjacents = new List<GridNode>();

        //
        Vector3Int upNeighbor = node.Position + Vector3Int.up;
        if (validTiles.Contains(upNeighbor))
        {
            adjacents.Add(new GridNode(upNeighbor, GetIsPassable(upNeighbor)));
        }

        Vector3Int rightNeighbor = node.Position + Vector3Int.right;
        if (validTiles.Contains(rightNeighbor))
        {
            adjacents.Add(new GridNode(rightNeighbor, GetIsPassable(rightNeighbor)));
        }

        Vector3Int leftNeighbor = node.Position + Vector3Int.left;
        if (validTiles.Contains(leftNeighbor))
        {
            adjacents.Add(new GridNode(leftNeighbor, GetIsPassable(leftNeighbor)));
        }

        Vector3Int downNeighbor = node.Position + Vector3Int.down;
        if (validTiles.Contains(downNeighbor))
        {
            adjacents.Add(new GridNode(downNeighbor, GetIsPassable(downNeighbor)));
        }

        //

        return adjacents;
    }

    private bool GetIsPassable(Vector3Int tilePosition)
    {
        var tileData = BattleTilemap.GetTile(tilePosition) as TerrainScriptableTile;
        if (tileData)
        {
            return tileData.IsPassable;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Validates and ensures that the given path is Walkable and doesn't contain any Attackable tiles.
    /// </summary>
    /// <param name="path"></param>
    public void EnsureWalkablePath(ref List<Vector3Int> path)
    {
        path.RemoveAll(x => !ValidMoveTiles.Contains(x) && ValidActionableTiles.Contains(x)); // ONLY remove the path indices that are solely attackable.
    }
}