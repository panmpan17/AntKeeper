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
    private List<AntRouteBranch> _spreadBranch;

    private Vector3Int _direction;
    private int _parentBranchSize;

    private LineRenderer _lineRenderer;

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

                for (int i = 0; i < _spreadBranch.Count; i++)
                    _spreadBranch[i].IsConnectedToNest = true;
            }
            else if (!_notConnectedDieTimer.Running)
            {
                _notConnectedDieTimer.Reset();
                _notConnectedDieTimer.TargetTime = _routeDisconnectDieTimeReference.PickRandomNumber();

                for (int i = 0; i < _spreadBranch.Count; i++)
                    _spreadBranch[i].IsConnectedToNest = false;
            }
        }
    }
    private Timer _notConnectedDieTimer;

    private RangeReference _routeDisconnectDieTimeReference;

    #region Constructor
    public AntRouteBranch(Vector3Int rootGridPosition, Vector3 rootWorldPosition, Vector3Int direciton,
                          LineRenderer lineRenderer, RangeReference routeDisconnectDieTimeReference, int length=0)
    {
        _root = rootGridPosition;
        _spots = new List<BranchSpot>();
        _spreadBranch = new List<AntRouteBranch>();

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

        _routeDisconnectDieTimeReference = routeDisconnectDieTimeReference;
        // _notConnectedDieTimer = new Timer(DieAfterDisconnectedFromNest, running: false);
    }

    public AntRouteBranch(Vector3Int[] gridPositions, Vector3[] worldPositions, Vector3Int direction, List<AntRouteBranch> antRouteBranches,
                          LineRenderer lineRenderer, RangeReference routeDisconnectDieTimeReference, int length)
    {
        _root = gridPositions[0];
        _spots = new List<BranchSpot>();
        _spreadBranch = antRouteBranches;

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

        _routeDisconnectDieTimeReference = routeDisconnectDieTimeReference;
        // _notConnectedDieTimer = new Timer(DieAfterDisconnectedFromNest, running: false);
    }
    #endregion


    #region Ant route spreading
    public Vector3Int FindNextGrowPosition()
    {
        if (_spots.Count == 0)
            return _root;
        return _spots[_spots.Count - 1].GridPosition + (Random.value > 0.8f ? SideWayDirection(_direction) : _direction);
    }

    public void AddGrowPosition(Vector3Int position, Vector3 worldPosition)
    {
        _spots.Add(new BranchSpot()
        {
            GridPosition = position,
            WorldPosition = worldPosition,
            Health = BranchSpot.MaxHealth,
        });

        // Resize line rendere position size
        Vector3[] newLinePoints = new Vector3[_lineRenderer.positionCount + 1];
        _lineRenderer.GetPositions(newLinePoints);
        newLinePoints[newLinePoints.Length - 1] = worldPosition;

        _lineRenderer.positionCount = _lineRenderer.positionCount + 1;
        _lineRenderer.SetPositions(newLinePoints);
    }

    public bool FindPotentialBranchOff(out BranchData branchData)
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

    public void AddBranchOff(AntRouteBranch branch)
    {
        _spreadBranch.Add(branch);
    }
    #endregion


    #region Kill Route
    public Vector3Int[] KillSpot(Vector3Int position, float killAmount, out AntRouteBranch newBranch)
    {
        for (int i = 0; i < _spots.Count; i++)
        {
            if (_spots[i].GridPosition != position)
                continue;

            BranchSpot spot = _spots[i];
            spot.Health -= killAmount;

            if (spot.Health <= 0)
                return HandleBranchSpotDead(i, position, out newBranch);
            
            _spots[i] = spot;
            break;
        }

        newBranch = null;
        return null;
    }

    Vector3Int[] HandleBranchSpotDead(int spotIndex, Vector3Int targetGridPosition, out AntRouteBranch newBranch)
    {
        int originalSize = Size;

        SpereateBranch(spotIndex + 1, out Vector3Int[] speratedGridPositions, out List<AntRouteBranch> connectedBranches);
        _spots.RemoveAt(spotIndex);

        // Only generate new branch if new branch length is at least 2
        if (speratedGridPositions.Length >= 2)
        {
            Vector3[] removedWorldPositions = new Vector3[speratedGridPositions.Length];
            for (int e = 0; e < removedWorldPositions.Length; e++)
                removedWorldPositions[e] = _lineRenderer.GetPosition(e + spotIndex + 1);

            newBranch = new AntRouteBranch(
                speratedGridPositions,
                removedWorldPositions,
                _direction,
                connectedBranches,
                GameObject.Instantiate(_lineRenderer, _lineRenderer.transform.parent),
                _routeDisconnectDieTimeReference,
                length: originalSize);
            newBranch.IsConnectedToNest = false;

            _lineRenderer.positionCount = spotIndex;
            return new Vector3Int[] { targetGridPosition };
        }
        else
        {
            newBranch = null;
            _lineRenderer.positionCount = spotIndex;

            // Set connected Brnach to not connect to ant nest
            for (int i = 0; i < connectedBranches.Count; i++)
            {
                connectedBranches[i].IsConnectedToNest = false;
            }

            if (speratedGridPositions.Length >= 1)
                return new Vector3Int[] { targetGridPosition, speratedGridPositions[0] };
            else
                return new Vector3Int[] { targetGridPosition };
        }
    }

    void SpereateBranch(int index, out Vector3Int[] gridPositions, out List<AntRouteBranch> connectedBranches)
    {
        gridPositions = new Vector3Int[ _spots.Count - index];
        connectedBranches = new List<AntRouteBranch>();

        for (int i = 0; i < gridPositions.Length; i++)
        {
            gridPositions[i] = _spots[index].GridPosition;
            _spots.RemoveAt(index);

            for (int e = 0; e < _spreadBranch.Count; e++)
            {
                if (_spreadBranch[e].RootGridPosition == gridPositions[i])
                {
                    connectedBranches.Add(_spreadBranch[e]);
                    _spreadBranch.RemoveAt(e);
                    e--;
                }
            }
        }
    }

    public bool UpdateNotConnectedDieTimer()
    {
        return _notConnectedDieTimer.UpdateEnd;
    }
    #endregion


    #region Utilities
    public bool IsOverlap(Vector3Int position)
    {
        for (int i = 0; i < _spots.Count; i++)
            if (_spots[i].GridPosition == position)
                return true;
        return false;
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

    public Vector3Int PickRandomPosition() => _spots.Count == 0 ? _root : _spots[Random.Range(0, _spots.Count)].GridPosition;

    public Vector3Int[] AllGridPosition()
    {
        Vector3Int[] allGridPosition = new Vector3Int[_spots.Count];
        for (int i = 0; i < _spots.Count; i++)
        {
            allGridPosition[i] = _spots[i].GridPosition;
        }
        return allGridPosition;
    }

    public void ChangeRouteLineRendererColor(Color newColor)
    {
        _lineRenderer.startColor = newColor;
        _lineRenderer.endColor = newColor;
    }

    public void OnDestroy()
    {
        GameObject.Destroy(_lineRenderer);
    }
    #endregion
}