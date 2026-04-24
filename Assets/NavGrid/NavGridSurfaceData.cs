using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NavGridSurfaceData", menuName = "Scriptable Objects/NavGridSurfaceData")]
public class NavGridSurfaceData : ScriptableObject
{
    Vector2Int _gridSize;
    NavGridCell[] _cells;

    public void StoreSurfaceDatas(Vector2Int gridSize, NavGridCell[,] cells)
    {
        _gridSize = gridSize;
        _cells = new NavGridCell[gridSize.x * gridSize.y];

        for(int x = 0; x < gridSize.x; x++)
        {
            for(int y = 0; y < gridSize.y; y++)
            {
                _cells[x * gridSize.y + y] = cells[x, y];
            }
        }
    }
}
