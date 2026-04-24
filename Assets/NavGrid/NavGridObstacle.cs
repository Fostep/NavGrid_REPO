using UnityEngine;

public class NavGridObstacle : NavGridElement
{
    bool _isCarving = false;

    public bool isCarving
    {
        get => _isCarving;
        set
        {
            if (_isCarving != value)
            {
                _isCarving = value;
                HandleCarving();
            }
        }
    }

    void HandleCarving()
    {
        // Notify occupied cells that the obstacle is carving on their layers
        foreach (NavGridCell cell in m_occupiedCells)
        {
            for (int i = 0; i < m_navLayers.Length; i++)
            {
                int layer = m_navLayers[i];
                if (_isCarving)
                {
                }
                else
                {
                }
            }
        }
    }
}
