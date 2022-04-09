using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using MPack;


public struct BranchData
{
    public Vector3Int Root;
    public Vector3Int Direction;
    public int ExccedPositionCount;

    public Vector3Int NextPosition => Root + Direction;
}

public struct BranchSpot
{
    public const float MaxHealth = 3;

    public Vector3 WorldPosition;
    public Vector3Int GridPosition;
    public float Health;
}

public class AntRouteBranch
{
    static Vector3Int SideWayDirection(Vector3Int direction)
    {
        int multiplier = Random.value > 0.5f ? 1 : -1;
        return new Vector3Int(direction.y * multiplier, direction.x * multiplier, 0);
    }

    private Vector3Int _root;
    private List<BranchSpot> _spots;

    private Vector3Int _direction;
    private int _parentBranchSize;

    private LineRenderer _lineRenderer;

    public Vector3Int RandomPosition => _spots.Count == 0 ? _root : _spots[Random.Range(0, _spots.Count)].GridPosition;

    public int Size => _spots.Count + _parentBranchSize;
    public Vector3Int RootGridPosition => _root;
    public bool IsEmpty => _spots.Count == 0;

    public bool IsConnectedToNest {
        get {
            return !_notConnectedDieTimer.Running;
        }
        set {
            if (value)
            {
                _notConnectedDieTimer.Running = false;
            }
            else if (!_notConnectedDieTimer.Running)
            {
                _notConnectedDieTimer.Reset();
            }
        }
    }
    private Timer _notConnectedDieTimer;


    public AntRouteBranch(Vector3Int rootGridPosition, Vector3 rootWorldPosition, Vector3Int direciton, LineRenderer lineRenderer, int length = 0)
    {
        _root = rootGridPosition;
        _spots = new List<BranchSpot>();
        _spots.Add(new BranchSpot()
        {
            GridPosition = rootGridPosition,
            WorldPosition = rootWorldPosition,
            Health = BranchSpot.MaxHealth,
        });

        _lineRenderer = lineRenderer;

        _direction = direciton;
        _parentBranchSize = length;

        _lineRenderer.positionCount = 1;
        _lineRenderer.SetPositions(new Vector3[] { rootWorldPosition });

        IsConnectedToNest = true;
    }
    public AntRouteBranch(Vector3Int[] gridPositions, Vector3[] worldPositions, Vector3Int direction, LineRenderer lineRenderer, int length)
    {
        _root = gridPositions[0];
        _spots = new List<BranchSpot>();
        for (int i = 0; i < gridPositions.Length; i++)
        {
            _spots.Add(new BranchSpot() {
                GridPosition=gridPositions[i],
                WorldPosition=worldPositions[i],
                Health=BranchSpot.MaxHealth,
            });
        }

        _lineRenderer = lineRenderer;

        _direction = direction;
        _parentBranchSize = length;

        IsConnectedToNest = false;
    }

    public bool IsOverlap(Vector3Int position)
    {
        for (int i = 0; i < _spots.Count; i++)
            if (_spots[i].GridPosition == position)
                return true;
        return false;
    }

    public Vector3Int FindNextSpreadPosition()
    {
        if (_spots.Count == 0)
            return _root;
        return _spots[_spots.Count - 1].GridPosition + (Random.value > 0.8f ? SideWayDirection(_direction) : _direction);
    }

    public bool BranchOff(out BranchData branchData)
    {
        if (IsEmpty)
        {
            branchData = new BranchData();
            return false;
        }
        int index = Random.Range(0, _spots.Count);
        Vector3Int newDirection = SideWayDirection(_direction);

        branchData = new BranchData
        {
            Root = _spots[index].GridPosition,
            Direction = newDirection,
            ExccedPositionCount = _parentBranchSize + index + 1,
        };
        return true;
    }

    public Vector3Int[] KillSpot(Vector3Int position, float killAmount, out AntRouteBranch newBranch)
    {
        for (int i = 0; i < _spots.Count; i++)
        {
            if (_spots[i].GridPosition != position)
                continue;

            BranchSpot spot = _spots[i];
            spot.Health -= killAmount;

            if (spot.Health <= 0)
                return HandleBranchSpotDead(i, out newBranch);
            
            _spots[i] = spot;
            break;
        }

        newBranch = null;
        return null;
    }

    Vector3Int[] HandleBranchSpotDead(int spotIndex, out AntRouteBranch newBranch)
    {
        int originalSize = Size;

        Vector3Int targetGridPosition = _spots[spotIndex].GridPosition;

        Vector3Int[] removedGridPositions = RemoveGridPositionFrom(spotIndex + 1);
        _spots.RemoveAt(spotIndex);

        if (removedGridPositions.Length >= 2)
        {
            Vector3[] removedWorldPositions = new Vector3[removedGridPositions.Length];
            for (int e = 0; e < removedWorldPositions.Length; e++)
                removedWorldPositions[e] = _lineRenderer.GetPosition(e + spotIndex + 1);

            newBranch = new AntRouteBranch(
                removedGridPositions,
                removedWorldPositions,
                _direction,
                GameObject.Instantiate(_lineRenderer, _lineRenderer.transform.parent),
                originalSize);

            _lineRenderer.positionCount = spotIndex;
            return new Vector3Int[] { targetGridPosition };
        }
        else
        {
            newBranch = null;
            _lineRenderer.positionCount = spotIndex;

            if (removedGridPositions.Length >= 1)
                return new Vector3Int[] { targetGridPosition, removedGridPositions[0] };
            else
                return new Vector3Int[] { targetGridPosition };
        }
    }

    public Vector3Int[] RemoveGridPositionFrom(int index)
    {
        Vector3Int[] removedPositions = new Vector3Int[ _spots.Count - index];

        for (int i = 0; i < removedPositions.Length; i++)
        {
            removedPositions[i] = _spots[index].GridPosition;
            _spots.RemoveAt(index);
        }

        return removedPositions;
    }

    public void AddPosition(Vector3Int position, Vector3 worldPosition)
    {
        _spots.Add(new BranchSpot() {
            GridPosition=position,
            WorldPosition=worldPosition,
            Health=BranchSpot.MaxHealth,
        });

        // Resize line rendere position size
        Vector3[] newLinePoints = new Vector3[_lineRenderer.positionCount + 1];
        _lineRenderer.GetPositions(newLinePoints);
        newLinePoints[newLinePoints.Length - 1] = worldPosition;

        _lineRenderer.positionCount = _lineRenderer.positionCount + 1;
        _lineRenderer.SetPositions(newLinePoints);
    }

    public void RecalculateLineRenderer(Tilemap map)
    {
        Vector3[] linePositions = new Vector3[_spots.Count];
        for (int i = 0; i < _spots.Count; i++)
        {
            linePositions[i] = map.GetCellCenterWorld(_spots[i].GridPosition);
        }
        _lineRenderer.positionCount = _spots.Count;
        _lineRenderer.SetPositions(linePositions);
    }
}