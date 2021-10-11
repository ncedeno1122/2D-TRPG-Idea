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

public enum TileAction
{
    NONE = 0,
    ATTACK = 1,
    HEAL = 2,
    INTERACT = 3,
    TALK = 4,
    CHEST = 5,
    // TODO: Specify more actions to be done on Tiles
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
    /// Handles Selection logic per Tile
    /// </summary>
    /// <param name="tilePosition"></param>
    public void SelectTile(Vector3Int tilePosition) // TODO: Deselect Tile if already Selected.
    {
        HandleSelectTile(tilePosition);
    }

    /// <summary>
    /// Uses GridHelperScript's current state to perform selection-related actions.
    /// </summary>
    /// <param name="tilePosition"></param>
    public void HandleSelectTile(Vector3Int tilePosition)
    {
        // Tile info
        var currTileData = BattleTilemap.GetTile(tilePosition) as TerrainScriptableTile;

        // CharacterUnit info
        var characterOnTile = GetCharacterOnTile(tilePosition);

        //Debug.Log($"Current Selection State is {CurrentState}!");
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
                    SelectedCharacterUnit = characterOnTile;

                    // Paint Interaction Range, which defines legal SelectedTargetTile candidates!
                    //Debug.Log($"Found character {charData.Name} on SelectedOriginTile {SelectedOriginTile}! Painting Interaction Range.");

                    TileAction desiredAction = TileAction.NONE;
                    if (equippedBattleItem)
                    {
                        desiredAction = equippedBattleItem is IWeapon ? TileAction.ATTACK : TileAction.HEAL;
                    }

                    PaintInteractionRange(charData.Prototype.MoveRange, equippedBattleItemRange, characterOnTile.TilePosition, desiredAction);
                }
                break;

            case GridHelperState.FIRST_TILE_SELECTED:
                if (!ValidMoveTiles.Contains(tilePosition) && !ValidActionableTiles.Contains(tilePosition))
                {
                    //Debug.Log($"Second selection {tilePosition} is not in the interaction range. Exiting Selection mode.");
                    // If we click outside the range, clear it all out
                    CurrentState = GridHelperState.NO_TILE_SELECTED;
                    ClearFoundTilePath();
                    ClearInteractionRange();
                    return;
                }
                CurrentState = GridHelperState.SECOND_TILE_SELECTED; // Redundant if unreachable?
                SelectedTargetTile = tilePosition;

                // Find Path to Target
                if (FindPathToTarget(SelectedOriginTile, SelectedTargetTile, ref SelectedTilePath))
                {
                    EnsureWalkablePath(ref SelectedTilePath);
                    if (SelectedTilePath.Count > 0)
                    {
                        SelectedCharacterUnit.FollowPath(SelectedTilePath);
                    }
                }

                // Debug: FUN
                CurrentState = GridHelperState.NO_TILE_SELECTED;
                ClearFoundTilePath();
                ClearInteractionRange();

                // TODO: Pull up context UI and all that.
                break;

            case GridHelperState.SECOND_TILE_SELECTED: // Unreachable?
                break;
        }
    }

    /// <summary>
    /// Clears the A*-found path of Tiles to a particular point.
    /// </summary>
    private void ClearFoundTilePath()
    {
        // Clear selected Tiles
        SelectedOriginTile = Vector3Int.zero;
        SelectedTargetTile = Vector3Int.zero;
        SelectedTilePath.Clear();
    }

    /// <summary>
    /// Clears the ActionTilemap, wiping the entire interaction range.
    /// </summary>
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
    /// <param name="equippedItem"></param>
    private void PaintInteractionRange(int range, int actionRange, Vector3Int position, TileAction desiredAction)
    {
        // TODO: Refactor this... it feels inefficient. KEEP primitives for ease of testing!

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
    private Tile GetActionableTileForTileAction(TileAction ta)
    {

        switch (ta)
        {
            case TileAction.NONE:
                //return NoneableTile;
            case TileAction.ATTACK:
                return AttackableTile;
            case TileAction.HEAL:
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
    /// Tries to find if a CharacterUnitScript is on a give tilePosition
    /// </summary>
    /// <param name="tilePosition"></param>
    /// <returns></returns>
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
        public bool IsPassable;

        public GridNode(Vector3Int position, bool isPassable)
        {
            Position = position;
            IsPassable = isPassable;
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
    private bool FindPathToTarget(Vector3Int origin, Vector3Int target, ref List<Vector3Int> path)
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
    private void EnsureWalkablePath(ref List<Vector3Int> path)
    {
        path.RemoveAll(x => !ValidMoveTiles.Contains(x) && ValidActionableTiles.Contains(x)); // ONLY remove the path indices that are solely attackable.
    }
}