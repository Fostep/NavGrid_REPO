using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[RequireComponent(typeof(Grid))]
public class NavGridSurface : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] Grid _grid;
    [SerializeField] Vector2Int _gridSize;
    [SerializeField] Vector2Int _gridOffset;
    NavGridCell[,] gridCells;
    // NavGridSurfaceData _surfaceData;
    
    #region Surface Creation

    NavGridCell CreateNavGridCell(Vector3Int localCoordinates)
    {
        Vector3Int gridCoordinates = new Vector3Int(localCoordinates.x + _gridOffset.x, localCoordinates.y + _gridOffset.y, 0);
        Vector3 worldPosition = _grid.CellToWorld(gridCoordinates);
        Vector3 worldCenterPosition = _grid.GetCellCenterWorld(gridCoordinates);

        NavGridCell cell = new NavGridCell(localCoordinates, gridCoordinates, worldPosition, worldCenterPosition);
        return cell;
    }

    bool IsValidLocalCoordinates(Vector3Int localCoordinates)
    {
        return localCoordinates.x >= 0 && localCoordinates.x < _gridSize.x &&
               localCoordinates.y >= 0 && localCoordinates.y < _gridSize.y;
    }

    void SetCellNeighborsDuringSurfaceCreation(NavGridCell cell, Vector3Int localCoordinates)
    {
        // Since we are rectangular or isometric, we can just check 4 neighbors
        Vector3Int[] neighborOffsets = new Vector3Int[4]
        {
            new Vector3Int(-1, 0, 0), // Left
            new Vector3Int(-1, -1, 0),  // Bottom Left
            new Vector3Int(0, -1, 0), // Bottom
            new Vector3Int(1, -1, 0)   // Bottom Right
        };

        foreach (Vector3Int offset in neighborOffsets)
        {
            Vector3Int neighborLocalCoordinates = localCoordinates + offset;

            // Neighbor not in grid
            if(!IsValidLocalCoordinates(neighborLocalCoordinates))
                continue;

            NavGridCell neighborCell = gridCells[neighborLocalCoordinates.x, neighborLocalCoordinates.y];

            // Link the cells as neighbors
            cell.AddNeighbor(neighborCell);
            neighborCell.AddNeighbor(cell);
        }
    }

    NavGridCell[,] CreateNavGridCells()
    {
        gridCells = new NavGridCell[_gridSize.x, _gridSize.y];

        for (int y = 0; y < _gridSize.y; y++)
        {
            for (int x = 0; x < _gridSize.x; x++)
            {
                Vector3Int localCoordinates = new Vector3Int(x, y, 0);
                NavGridCell cell = CreateNavGridCell(localCoordinates);
                gridCells[x, y] = cell;
                SetCellNeighborsDuringSurfaceCreation(cell, localCoordinates);
            }
        }

        return gridCells;

    }

    [ContextMenu("Create Surface")]
    void CreateSurface()
    {
        gridCells = CreateNavGridCells();
        // if(_surfaceData == null)
        //     _surfaceData = ScriptableObject.CreateInstance<NavGridSurfaceData>();
    }

    #endregion Surface Creation

    #region World To

    public Vector3Int WorldToGridCoordinates(Vector3 worldPosition)
    {
        Vector3Int gridCoordinates = _grid.WorldToCell(worldPosition);
        return gridCoordinates;
    }

    public NavGridCell WorldToNavGridCell(Vector3 worldPosition)
    {
        Vector3Int gridCoordinates = WorldToGridCoordinates(worldPosition);
        Vector3Int localCoordinates = new Vector3Int(gridCoordinates.x - _gridOffset.x, gridCoordinates.y - _gridOffset.y, 0);

        if(!IsValidLocalCoordinates(localCoordinates))
            return null;

        return gridCells[localCoordinates.x, localCoordinates.y];
    }

    #endregion World To

    #region Debug

    Vector3[] LocalRectangularCorners(Vector2 halfSize)
    {
        Vector3[] corners = new Vector3[4]
        {
            new Vector3(-halfSize.x, -halfSize.y, 0), // Bottom Left
            new Vector3(-halfSize.x, halfSize.y, 0), // Top Left
            new Vector3(halfSize.x, halfSize.y, 0), // Top Right
            new Vector3(halfSize.x, -halfSize.y, 0) // Bottom Right
        };
        return corners;
    }

    Vector3[] LocalIsometricCorners(Vector2 halfSize)
    {
        Vector3[] corners = new Vector3[4]
        {
            new Vector3(0, -halfSize.y, 0), // Bottom
            new Vector3(-halfSize.x, 0, 0), // Left
            new Vector3(0, halfSize.y, 0), // Top
            new Vector3(halfSize.x, 0, 0) // Right
        };
        return corners;
    }

    void OnDrawGizmos()
    {
        if(gridCells == null)
            return;
        
        if(_grid.cellLayout == GridLayout.CellLayout.Hexagon)
            return;


        // Get swizzle axis for drawing the cells
        Vector3 right = Vector3.right;
        Vector3 up = Vector3.up;

        switch(_grid.cellSwizzle)
        {
            case GridLayout.CellSwizzle.XYZ:
                right = Vector3.right;
                up = Vector3.up;
                break;
            case GridLayout.CellSwizzle.XZY:
                right = Vector3.right;
                up = Vector3.forward;
                break;
            case GridLayout.CellSwizzle.YXZ:
                right = Vector3.up;
                up = Vector3.right;
                break;
            case GridLayout.CellSwizzle.YZX:
                right = Vector3.right;
                up = Vector3.forward;
                break;
            case GridLayout.CellSwizzle.ZXY:
                right = Vector3.forward;
                up = Vector3.up;
                break;
            case GridLayout.CellSwizzle.ZYX:
                right = Vector3.forward;
                up = Vector3.up;
                break;
        }
        
        Gizmos.color = Color.green;

        Vector2 halfCellSize = _grid.cellSize / 2f;


        foreach (NavGridCell cell in gridCells)
        {
            List<NavGridCell> neighbors = cell.Neighbors;
            foreach (NavGridCell neighbor in neighbors)
            {
                Vector3[] localCorners = _grid.cellLayout == GridLayout.CellLayout.Rectangle ? LocalRectangularCorners(halfCellSize) : LocalIsometricCorners(halfCellSize);
                Vector3[] worldCorners = new Vector3[localCorners.Length];
                for(int i = 0; i < localCorners.Length; i++)
                {
                    Vector3 localCorner = localCorners[i];
                    Vector3 worldCorner = this.transform.rotation * (localCorner.x * right + localCorner.y * up);
                    worldCorners[i] = worldCorner;
                }

                for(int i = 0; i < worldCorners.Length; i++)
                {
                    Vector3 start = cell.WorldCenterPosition + worldCorners[i];
                    Vector3 end = cell.WorldCenterPosition + worldCorners[(i + 1) % worldCorners.Length];
                    Gizmos.DrawLine(start, end);
                }
            }
        }
    }

    #endregion Debug
}
