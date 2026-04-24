using System.Collections.Generic;
using UnityEngine;

public class NavGridElementsLayer
{
    int _layer;
    List<NavGridElement> _elementsInCellLayer = new List<NavGridElement>();
    bool _obstructed = false;

    public int Layer => _layer;
    public bool Obstructed => _obstructed;

    public NavGridElementsLayer(int layer)
    {
        _layer = layer;
        _elementsInCellLayer = new List<NavGridElement>();
        _obstructed = false;
    }

    public NavGridElementsLayer(int layer, List<NavGridElement> elementsInCellLayer, bool obstructed)
    {
        _layer = layer;
        _elementsInCellLayer = elementsInCellLayer;
        _obstructed = obstructed;
    }

    public List<NavGridElement> GetElementsInCellLayer()
    {
        return _elementsInCellLayer;
    }

    public List<T> GetElementsInCellLayerOfType<T>() where T : NavGridElement
    {
        List<T> elementsOfType = new List<T>();
        foreach (NavGridElement element in _elementsInCellLayer)
        {
            if (element is T typedElement)
            {
                elementsOfType.Add(typedElement);
            }
        }
        return elementsOfType;
    }

    void UpdateObstructed()
    {
        List<NavGridObstacle> remainingObstacles = GetElementsInCellLayerOfType<NavGridObstacle>();
        foreach (NavGridObstacle remainingObstacle in remainingObstacles)
        {
            if (remainingObstacle.isCarving)
            {
                _obstructed = true; // Still obstructed by another carving obstacle
                return;
            }
        }
        _obstructed = false; // No more carving obstacles, cell is no longer obstructed
    }

    public void AddElement(NavGridElement element)
    {
        _elementsInCellLayer.Add(element);
        if (element is NavGridObstacle obstacle && obstacle.isCarving)
        {
            _obstructed = true;
        }
    }

    public void RemoveElement(NavGridElement element)
    {
        _elementsInCellLayer.Remove(element);
        if (element is NavGridObstacle obstacle && obstacle.isCarving)
        {
            List<NavGridObstacle> remainingObstacles = GetElementsInCellLayerOfType<NavGridObstacle>();
            foreach (NavGridObstacle remainingObstacle in remainingObstacles)
            {
                if (remainingObstacle.isCarving)
                {
                    return; // Still obstructed by another carving obstacle
                }
            }
            _obstructed = false; // No more carving obstacles, cell is no longer obstructed
        }
    }
}
