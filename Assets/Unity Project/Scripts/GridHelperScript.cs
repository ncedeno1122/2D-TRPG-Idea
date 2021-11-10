using System;
using System.Collections.Generic;
using System.Linq;
using Unity_Project.Scripts.BattleDataScripts;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridHelperScript : MonoBehaviour
{
    public Grid Grid;
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
        
        // Get Grid
        Grid = GetComponent<Grid>();
    }

    // + + + + | Functions | + + + +

    /// <summary>
    /// Clears the A*-found path of Tiles to a particular point.
    /// </summary>
    public void ClearFoundTilePath()
    {
        SelectedTilePath.Clear();
    }

    /// <summary>
    /// Clears the ActionTilemap.
    /// </summary>
    public void ClearActionTilemap()
    {
        ActionTilemap.ClearAllTiles();
    }

    /// <summary>
    /// Returns a list of all tile positions within a given range.
    /// </summary>
    /// <param name="tilePosition"></param>
    /// <param name="range"></param>
    /// <param name="includeOrigin"></param>
    /// <returns></returns>
    public List<Vector3Int> GetTilesInRange(Vector3Int tilePosition, int range, bool includeOrigin)
    {
        var tileList = new List<Vector3Int>();
        
        RecursivelyGetTilesInRangeByAdjacent(ref tileList, tilePosition, range);
        //Debug.Log($"Done GettingTilesInRange; count {tileList.Count}!");

        if (!includeOrigin)
        {
            tileList.Remove(tilePosition);
        }

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
        // TODO: THIS FUNCTION IS BROKEN FOR SOME REASON I CAN'T FIGURE OUT
        /*
         * So the thing is, when we have a range that should look like this,
         *             [ ] 
         *         [ ] [ ] [ ]
         *     [ ] [ ] [ ] [ ] [ ]
         * [ ] [ ] [ ] [O] [ ] [ ] [ ] having a range of 3 around the Origin Tile,
         *     [ ] [ ] [ ] [ ] [ ]
         *         [ ] [ ] [ ]
         *             [ ]
         *
         * I get this instead:
         *
         *             [ ]
         *         [ ] [ ] [ ]
         *     [ ] [ ] [ ] [ ] [ ]
         *         [ ] [O] [ ]         <- This weird happenstance occurs
         *     [ ] [ ] [ ] [ ] [ ]
         *         [ ] [ ] [ ]
         *             [ ]
         *
         * This pattern occurs when I start with the + up or + down recursive calls, and
         * I get it flipped by 90 degrees when I start with the + right or + left recursive calls.
         * I don't get why this isn't working, but I've made a function that does this with a for
         * loop and it works fine (while admittedly less efficient iI think)
         */
        
        if (moveRange < 0) return;
        if (tileList.Contains(positionToCheck)) return;
        
        // Add Tiles to list
        tileList.Add(positionToCheck);

        RecursivelyGetTilesInRange(ref tileList, positionToCheck + Vector3Int.up, moveRange - 1);
        RecursivelyGetTilesInRange(ref tileList, positionToCheck + Vector3Int.right, moveRange - 1);
        RecursivelyGetTilesInRange(ref tileList, positionToCheck + Vector3Int.down, moveRange - 1);
        RecursivelyGetTilesInRange(ref tileList, positionToCheck + Vector3Int.left, moveRange - 1);
    }

    private void RecursivelyGetTilesInRangeByAdjacent(ref List<Vector3Int> tileList, Vector3Int positionToCheck, int moveRange)
    {
        for (int x = positionToCheck.x - moveRange; x <= positionToCheck.x + moveRange; x++)
        {
            for (int y = positionToCheck.y - moveRange; y <= positionToCheck.y + moveRange; y++)
            {
                var currTilePos = new Vector3Int(x, y, 0);

                if (GetManhattanDistance(currTilePos, positionToCheck) > moveRange) continue;
                tileList.Add(currTilePos);
            }
        }
    }

    /// <summary>
    /// Returns a list of all movable tile positions within a given range.
    /// </summary>
    /// <param name="tilePosition"></param>
    /// <param name="moveRange"></param>
    /// <returns></returns>
    public List<Vector3Int> GetMovableRange(Vector3Int tilePosition, int moveRange)
    {
        //Debug.Log($"Getting Movable Range {moveRange} tiles around {tilePosition}");
        var tileList = new List<Vector3Int>();
        
        RecursivelyGetMovableTiles(ref tileList, tilePosition, moveRange);
        //Debug.Log($"Done RecursivelyGettingMovableTiles, got {tileList.Count}");

        return tileList;
    }

    /// <summary>
    /// Helper function for GetMovableRange, edits a predefined list with all valid movable tiles within a given range.
    /// </summary>
    /// <param name="tileList"></param>
    /// <param name="positionToCheck"></param>
    /// <param name="moveRange"></param>
    private void RecursivelyGetMovableTiles(ref List<Vector3Int> tileList, Vector3Int positionToCheck, int moveRange)
    {
        if (moveRange < 0) return;
        if (tileList.Contains(positionToCheck)) return;
        
        // Get tile info
        var currTile = BattleTilemap.GetTile(positionToCheck) as TerrainScriptableTile;

        if (!currTile) return;
        if (!currTile.IsPassable) return;
        //Debug.Log($"Valid movable tile at { positionToCheck }! Range is { moveRange } at present!");
        tileList.Add(positionToCheck);
        
        // Finally, recurse!
        RecursivelyGetMovableTiles(ref tileList, positionToCheck + Vector3Int.up, (moveRange - 1) + currTile.MovementCost);
        RecursivelyGetMovableTiles(ref tileList, positionToCheck + Vector3Int.right, (moveRange - 1) + currTile.MovementCost);
        RecursivelyGetMovableTiles(ref tileList, positionToCheck + Vector3Int.down, (moveRange - 1) + currTile.MovementCost);
        RecursivelyGetMovableTiles(ref tileList, positionToCheck + Vector3Int.left, (moveRange - 1) + currTile.MovementCost);
    }

    /// <summary>
    /// Finds the ActionableRange around a predefined movableList.
    /// </summary>
    /// <param name="movableList"></param>
    /// <param name="actionRange"></param>
    public List<Vector3Int> GetActionableRange(ref List<Vector3Int> movableList, int actionRange)
    {
        var actionableList = new List<Vector3Int>();
        
        foreach (var position in movableList)
        {
            for (int x = position.x - actionRange; x <= position.x + actionRange; x++)
            {
                for (int y = position.y - actionRange; y <= position.y + actionRange; y++)
                {
                    var currTilePosition = new Vector3Int(x, y, 0);
                    if (GetManhattanDistance(position, currTilePosition) > actionRange) continue;

                    if (!actionableList.Contains(currTilePosition) &&
                        !movableList.Contains(currTilePosition))
                    {
                        actionableList.Add(currTilePosition);
                    }
                }
            }
        }

        return actionableList;
    }
    
    public void PaintRange(Vector3Int position, ref List<Vector3Int> rangeList, Tile tileToUse)
    {
        foreach (var tilePosition in rangeList)
        {
            ActionTilemap.SetTile(tilePosition, tileToUse);
        }
    }

    /// <summary>
    /// Clears the current InteractionRange and paints a new one based on CharacterUnit parameters.
    /// </summary>
    public void PaintInteractionRange(Vector3Int position, ref List<Vector3Int> movableRange, ref List<Vector3Int> actionableRange, IBattleItem battleItem)
    {
        ActionTilemap.ClearAllTiles();

        var desiredAction = battleItem is IWeapon ? TurnAction.ATTACK : TurnAction.HEAL;  
        var tileToUse = GetActionableTileForTileAction(desiredAction);

        // Paint the ranges
        PaintRange(position, ref movableRange, WalkableTile);
        PaintRange(position, ref actionableRange, tileToUse);

        //ActionTilemap.SetTile(position, OriginTile);
    }

    public void PaintEntityTiles(ref List<TileEntity> entityList)
    {
        foreach (var entity in entityList)
        {
            ActionTilemap.SetTile(entity.TilePosition, OriginTile);
        }
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
                //return None-ableTile;
            case TurnAction.ATTACK:
                return AttackableTile;
            case TurnAction.HEAL:
                return HealableTile;
            //case TileAction.INTERACT:
            //    return InteractableTile;
            //    break;
            //case TileAction.TALK:
            //    return Talk-ableTile;
            //    break;
            //case TileAction.CHEST:
            //    return Chest-ableTile; LOL Chest-able
            //    break;
            default:
                return OriginTile;
        }
    }
    
    public TileEntity GetTileEntityOnTile(Vector3Int tilePosition)
    {
        var alignedTilePosition = Grid.GetCellCenterWorld(tilePosition);
        var boxPosition = new Vector2(alignedTilePosition.x, alignedTilePosition.y);
        var hitCollider = Physics2D.OverlapBox(boxPosition, Grid.cellSize, 0f);

        if (!hitCollider) return null;
        //Debug.Log($"Successfully hit {hitCollider.gameObject.name}!");
            
        var tileEntity = hitCollider.gameObject.GetComponent<TileEntity>();
        return tileEntity;

    }
    
    public CharacterUnitScript GetCharacterOnTile(Vector3Int tilePosition)
    {
        var alignedTilePosition = Grid.GetCellCenterWorld(tilePosition);
        var boxPosition = new Vector2(alignedTilePosition.x, alignedTilePosition.y);
        var hitCollider = Physics2D.OverlapBox(boxPosition, Grid.cellSize, 0f);

        if (!hitCollider) return null;
        //Debug.Log($"Successfully hit {hitCollider.gameObject.name}!");
            
        var cus = hitCollider.gameObject.GetComponent<CharacterUnitScript>();
        return cus;

    }

    public List<TileEntity> GetTileEntitiesInRange(Vector3Int tilePosition, int range)
    {
        List<TileEntity> entitiesInRange = new List<TileEntity>();
        List<Vector3Int> vec3Range = new List<Vector3Int>();
        
        RecursivelyGetTilesInRange(ref vec3Range, tilePosition, range);

        foreach (var position in vec3Range)
        {
            var tileEntity = GetTileEntityOnTile(position);
            if (tileEntity)
            {
                entitiesInRange.Add(tileEntity);
            }
        }

        return entitiesInRange;
    }
    
    public List<CharacterUnitScript> GetCharactersInRange(Vector3Int tilePosition, int range)
    {
        List<CharacterUnitScript> targetsInRange = new List<CharacterUnitScript>();
        List<Vector3Int> vec3Range = new List<Vector3Int>();
        
        // Get vec3Range
        RecursivelyGetTilesInRange(ref vec3Range, tilePosition, range);

        foreach (var position in vec3Range)
        {
            var cus = GetCharacterOnTile(position);
            if (cus)
            {
                targetsInRange.Add(cus);
            }
        }

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

        public override string ToString()
        {
            return Position.ToString();
        }
    }

    /// <summary>
    /// Uses the A* Algorithm to find a Path within the ValidMoveTiles List
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="target"></param>
    /// <param name="path"></param>
    /// <param name="validTiles"></param>
    /// <returns></returns>
    public bool FindPathToTarget(Vector3Int origin, Vector3Int target, ref List<Vector3Int> path, ref List<Vector3Int> validTiles)
    {
        //Debug.Log($"Finding path from {origin} to {target}!");
        GridNode startNode = new GridNode(origin, true);
        GridNode endNode = new GridNode(target, true);
        List<GridNode> openList = new List<GridNode>();
        List<GridNode> closedList = new List<GridNode>();
        GridNode current = startNode;

        openList.Add(startNode);

        while (openList.Count != 0 && !closedList.Exists(x => x.Position == endNode.Position))
        {
            current = openList[0];
            openList.Remove(current);
            closedList.Add(current);
            var adjacentNodes = GetInteractableAdjacentNodes(current, ref validTiles);

            //Debug.Log($"Considering Node {current} with {adjacentNodes.Count} adjacent nodes...");
            
            foreach (var node in adjacentNodes)
            {
                if (closedList.Contains(node) || (!node.IsPassable && node.Position != endNode.Position)) continue;
                if (openList.Contains(node)) continue;
                node.Parent = current;
                node.g = GetManhattanDistance(origin, node.Position);
                node.h = GetManhattanDistance(node.Position, target);
                node.f = node.g + node.h;
                openList.Add(node);
                //Debug.Log($"Node {node} is a valid adjacent and is added to the openList!");
                // Re-sort the openList!
                openList = openList.OrderBy(listNode => listNode.f).ToList<GridNode>();
            }
        }

        // If there's no path, return null...
        if (!closedList.Exists(x => x.Position == endNode.Position))
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
        } while (!temp.Equals(startNode) && !temp.Equals(null));
        
        //Debug.Log($"Size of closedList: {closedList.Count}. Size of openList: {openList.Count}. Length of path: {path.Count}");
        return true;
    }

    /// <summary>
    /// Helper function for FindWalkablePathToTarget, finds adjacent, Walkable GridNodes.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="validTiles"></param>
    /// <returns></returns>
    private List<GridNode> GetInteractableAdjacentNodes(GridNode node, ref List<Vector3Int> validTiles)
    {
        List<GridNode> adjacentNodes = new List<GridNode>();

        //
        Vector3Int upNeighbor = node.Position + Vector3Int.up;
        if (validTiles.Contains(upNeighbor))
        {
            adjacentNodes.Add(new GridNode(upNeighbor, GetIsPassable(upNeighbor)));
        }

        Vector3Int rightNeighbor = node.Position + Vector3Int.right;
        if (validTiles.Contains(rightNeighbor))
        {
            adjacentNodes.Add(new GridNode(rightNeighbor, GetIsPassable(rightNeighbor)));
        }

        Vector3Int leftNeighbor = node.Position + Vector3Int.left;
        if (validTiles.Contains(leftNeighbor))
        {
            adjacentNodes.Add(new GridNode(leftNeighbor, GetIsPassable(leftNeighbor)));
        }

        Vector3Int downNeighbor = node.Position + Vector3Int.down;
        if (validTiles.Contains(downNeighbor))
        {
            adjacentNodes.Add(new GridNode(downNeighbor, GetIsPassable(downNeighbor)));
        }

        //

        return adjacentNodes;
    }

    private bool GetIsPassable(Vector3Int tilePosition)
    {
        var tileData = BattleTilemap.GetTile(tilePosition) as TerrainScriptableTile;
        return tileData && tileData.IsPassable;
    }

    /// <summary>
    /// Validates and ensures that the given path is Walkable and doesn't contain any Attack-able tiles.
    /// </summary>
    /// <param name="path"></param>
    public void EnsureWalkablePath(ref List<Vector3Int> path)
    {
        path.RemoveAll(x => !ValidMoveTiles.Contains(x) && ValidActionableTiles.Contains(x)); // ONLY remove the path indices that are solely attack-able.
    }
}