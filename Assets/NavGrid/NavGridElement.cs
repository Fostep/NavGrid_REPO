using System.Collections.Generic;
using UnityEngine;

public abstract class NavGridElement : MonoBehaviour
{
    protected int[] m_navLayers;
    protected List<NavGridCell> m_occupiedCells;

    #region Nav Grid Cells Occupation
    void HandleVacateNavGridCell(NavGridCell cell)
    {
        for (int i = 0; i < m_navLayers.Length; i++)
        {
            cell.RemoveElement(this, m_navLayers[i]);
        }
    }

    public void VacateNavGridCells()
    {
        // Check all occupied cells and remove this element from them
        foreach (NavGridCell cell in m_occupiedCells)
        {
            // Remove this element from the cell for each layer it occupies
            HandleVacateNavGridCell(cell);
        }

        // Clear the occupied cells reference
        m_occupiedCells = null;
    }

    void HandleOccupyNavGridCell(NavGridCell cell)
    {
        for (int i = 0; i < m_navLayers.Length; i++)
        {
            cell.AddElement(this, m_navLayers[i]);
        }
    }

    public void OccupyNavGridCells(List<NavGridCell> cells)
    {
        // First, vacate any currently occupied cells
        VacateNavGridCells();

        // Store new cells
        m_occupiedCells = cells;

        // Add this element to the new cells for each layer it occupies
        foreach (NavGridCell cell in m_occupiedCells)
        {
            HandleOccupyNavGridCell(cell);
        }
    }

    public void OccupyNavGridCell(NavGridCell cell)
    {
        VacateNavGridCells();
        m_occupiedCells = new List<NavGridCell> { cell };
        HandleOccupyNavGridCell(cell);
    }

    #endregion Nav Grid Cells Occupation
}
