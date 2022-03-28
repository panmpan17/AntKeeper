using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct BranchData
{
    public Vector3Int Root;
    public Vector3Int Direction;
    public int ExccedPositionCount;

    public Vector3Int NextPosition => Root + Direction;
}

// public struct BranchSpot
// {
//     public Vector3 WorldPosition;
//     public Vector3Int GridPosition;
//     public float Health;
// }

public class AntRouteBranch
{
    static Vector3Int SideWayDirection(Vector3Int direction)
    {
        int multiplier = Random.value > 0.5f ? 1 : -1;
        return new Vector3Int(direction.y * multiplier, direction.x * multiplier, 0);
    }

    private Vector3Int _root;
    private List<Vector3Int> _positions;
    private Vector3Int _direction;
    private int originalLength;

    private LineRenderer _lineRenderer;

    public int Size => _positions.Count + originalLength;
    public Vector3Int RootGridPosition => _root;
    public bool IsEmpty => _positions.Count == 0;

    public AntRouteBranch(Vector3Int rootGridPosition, Vector3 rootWorldPosition, Vector3Int direciton, LineRenderer lineRenderer, int length = 0)
    {
        _root = rootGridPosition;
        _positions = new List<Vector3Int>();
        _positions.Add(rootGridPosition);

        _lineRenderer = lineRenderer;

        _direction = direciton;
        originalLength = length;

        _lineRenderer.positionCount = 1;
        _lineRenderer.SetPositions(new Vector3[] { rootWorldPosition });
    }

    public bool IsOverlap(Vector3Int position) => _positions.Contains(position);

    public Vector3Int FindNextSpreadPosition()
    {
        if (_positions.Count == 0)
            return _root;
        return _positions[_positions.Count - 1] + (Random.value > 0.8f ? SideWayDirection(_direction) : _direction);
    }

    public bool BranchOff(out BranchData branchData)
    {
        if (IsEmpty)
        {
            branchData = new BranchData();
            return false;
        }
        int index = Random.Range(0, _positions.Count);
        Vector3Int newDirection = SideWayDirection(_direction);

        branchData = new BranchData
        {
            Root = _positions[index],
            Direction = newDirection,
            ExccedPositionCount = originalLength + index + 1,
        };
        return true;
    }

    public Vector3Int[] KillSpot(Vector3Int position)
    {
        for (int i = 0; i < _positions.Count; i++)
        {
            if (_positions[i] == position)
            {
                Vector3Int[] removedPositions = RemovePositionFrom(i);

                // Vector3[] newLinePoints = new Vector3[i];
                // _lineRenderer.GetPositions(newLinePoints);
                _lineRenderer.positionCount = i;
                // _lineRenderer.SetPositions(newLinePoints);
                return removedPositions;
            }
        }

        return null;
    }

    public Vector3Int[] RemovePositionFrom(int index)
    {
        Vector3Int[] removedPositions = new Vector3Int[_positions.Count - index];

        for (int i = 0; i < removedPositions.Length; i++)
        {
            removedPositions[i] = _positions[index];
            _positions.RemoveAt(index);
        }

        return removedPositions;
    }

    public void AddPosition(Vector3Int position, Vector3 worldPosition)
    {
        _positions.Add(position);

        // Resize line rendere position size
        Vector3[] newLinePoints = new Vector3[_lineRenderer.positionCount + 1];
        _lineRenderer.GetPositions(newLinePoints);
        newLinePoints[newLinePoints.Length - 1] = worldPosition;

        _lineRenderer.positionCount = _lineRenderer.positionCount + 1;
        _lineRenderer.SetPositions(newLinePoints);
    }
}