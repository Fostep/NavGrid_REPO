using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NavGridCell
{
    Vector3Int _localCoordinates;
    Vector3Int _gridCoordinates;

    Vector3 _worldPosition;
    Vector3 _worldCenterPosition;

    List<NavGridCell> _neighbors = new List<NavGridCell>();
    List<NavGridElementsLayer> _elementsPerLayerInCell = new List<NavGridElementsLayer>();

    public Vector3Int LocalCoordinates => _localCoordinates;
    public Vector3Int GridCoordinates => _gridCoordinates;

    public Vector3 WorldPosition => _worldPosition;
    public Vector3 WorldCenterPosition => _worldCenterPosition;

    public List<NavGridCell> Neighbors => _neighbors;

    public event Action<NavGridCell, NavGridElement, int> OnElementAdded;
    public event Action<NavGridCell, NavGridElement, int> OnElementRemoved;


    public NavGridCell(Vector3Int localCoordinates, Vector3Int gridCoordinates, Vector3 worldPosition, Vector3 worldCenterPosition)
    {
        _localCoordinates = localCoordinates;
        _gridCoordinates = gridCoordinates;
        _worldPosition = worldPosition;
        _worldCenterPosition = worldCenterPosition;

        _neighbors = new List<NavGridCell>();
        _elementsPerLayerInCell = new List<NavGridElementsLayer>();
    }

    public void AddNeighbor(NavGridCell neighbor)
    {
        _neighbors.Add(neighbor);
    }

    #region Layers

    bool ContainsLayer(int layer)
    {
        foreach (NavGridElementsLayer elementsInLayer in _elementsPerLayerInCell)
        {
            if (elementsInLayer.Layer == layer)
                return true;
        }
        return false;
    }

    NavGridElementsLayer GetElementsLayer(int layer)
    {
        foreach (NavGridElementsLayer elementsInLayer in _elementsPerLayerInCell)
        {
            if (elementsInLayer.Layer == layer)
                return elementsInLayer;
        }
        return null;
    }

    public bool IsLayerObstructed(int layer)
    {
        NavGridElementsLayer elementsLayer = GetElementsLayer(layer);
        if (elementsLayer == null)
            return false;

        return elementsLayer.Obstructed;
    }

    public bool IsObstructed()
    {
        foreach (NavGridElementsLayer elementsLayer in _elementsPerLayerInCell)
        {
            if (elementsLayer.Obstructed)
                return true;
        }
        return false;
    }

    #endregion Layers

    #region Elements

    public void AddElement(NavGridElement element, int layer)
    {
        // Doesn't have the layer yet, create it
        if (!ContainsLayer(layer))
        {
            // Add new layer to the cell
            _elementsPerLayerInCell.Add(new NavGridElementsLayer(layer));
        }

        // Get the layer and add the element to it
        NavGridElementsLayer elementsInLayer = GetElementsLayer(layer);
        elementsInLayer.AddElement(element);

        OnElementAdded?.Invoke(this, element, layer);
    }

    public void RemoveElement(NavGridElement element, int layer)
    {
        // Doesn't have the layer, nothing to remove
        if (!ContainsLayer(layer))
            return;

        NavGridElementsLayer elementsInLayer = GetElementsLayer(layer);
        List<NavGridElement> elementsInLayerList = elementsInLayer.GetElementsInCellLayer();
        
        // Layer doesn't contain the element, nothing to remove
        if(!elementsInLayerList.Contains(element))
            return;

        // Remove the element from the layer
        elementsInLayer.RemoveElement(element);

        // No more element in the layer, remove the layer from the cell
        if(elementsInLayerList.Count == 0)
            _elementsPerLayerInCell.Remove(elementsInLayer);
        
        OnElementRemoved?.Invoke(this, element, layer);
    }

    #endregion Elements
}
