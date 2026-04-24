using Unity.Collections;
using UnityEngine;

public class NavGrid_Test : MonoBehaviour
{
    [SerializeField] NavGridSurface _navGridSurface;
    [SerializeField] bool _inCell = false;
    [SerializeField] Vector3Int _localCoordinates;
    [SerializeField] Vector3Int _gridCoordinates;

    private void OnDrawGizmos()
    {
        if (_navGridSurface == null)
            return;

        NavGridCell cell = _navGridSurface.WorldToNavGridCell(transform.position);
        if (cell == null)
        {
            _inCell = false;
            return;
        }

        _inCell = true;
        _localCoordinates = cell.LocalCoordinates;
        _gridCoordinates = cell.GridCoordinates;
    }
}
