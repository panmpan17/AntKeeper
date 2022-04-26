using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using MPack;

[RequireComponent(typeof(AntNestHub))]
public class AntRouteGrowControl : MonoBehaviour
{
    static Vector3Int[] FourDirections = new Vector3Int[] {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(0, 1, 0),
    };

    private AntNestHub _hub;

    [SerializeField]
    private TilemapReference tilemapReference;
    private Tilemap routeMap => tilemapReference.Tilemap;
    [SerializeField]
    private LineRenderer routeLine;

    [SerializeField]
    private bool initialSizeOnStart;
    [SerializeField]
    private IntRangeReference initialSize;

    [Header("Grow")]
    [SerializeField]
    private RangeReference growInterval;
    private Timer _growIntervalTimer;
    [SerializeField]
    private IntRangeReference maxSize;
    private int _maxSize;
    [SerializeField]
    private float branchOffChance = 0.05f;

    [SerializeField]
    private int showTrueColorAfterSize;

    [Header("Die")]
    [SerializeField]
    private RangeReference disconnectDieTime;
    [SerializeField]
    private Timer unableToGrowDieTimer;

    private int _size;
    public int Size => _size;

    public event System.Action OnSizeIncrease;

    #if UNITY_EDITOR
    [Header("Editor Only")]
    [SerializeField]
    private bool initialSizeStepByStepDebug;
    #endif


    #region Start
    void Awake()
    {
        _hub = GetComponent<AntNestHub>();
        _hub.OnStart += OnHubStarted;

        _maxSize = maxSize.PickRandomNumber();
        _growIntervalTimer = new Timer(growInterval.PickRandomNumber());
    }

    void OnHubStarted()
    {
        for (int i = 0; i < FourDirections.Length; i++)
        {
            _hub.routeBranches.Add(new AntRouteBranch(
                _hub.RootGridPosition,
                routeMap.GetCellCenterWorld(_hub.RootGridPosition),
                FourDirections[i],
                Instantiate(routeLine, transform),
                disconnectDieTime
                ));
        }

        if (initialSizeOnStart)
        {
#if UNITY_EDITOR
            if (initialSizeStepByStepDebug)
                StartCoroutine(C_GrowSizeByCount(initialSize.PickRandomNumber()));
            else
#endif
                GrowSizeByCount(initialSize.PickRandomNumber());
        }
    }

    IEnumerator C_GrowSizeByCount(int time, int maxLoop = 1000)
    {
        int count = 0;

        while (count < time && count < maxLoop)
        {
            if (TryExpandBranch())
            {
                count++;
                yield return null;
            }
        }
    }

    void GrowSizeByCount(int time, int maxLoop=1000)
    {
        int count = 0;

        while (count < time && count < maxLoop)
        {
            if (TryExpandBranch())
            {
                count++;
            }
        }
    }
    #endregion


    void Update()
    {
        if (_growIntervalTimer.UpdateEnd)
        {
            if (TryExpandBranch())
            {
                unableToGrowDieTimer.Reset();
                _growIntervalTimer.Reset();
                _growIntervalTimer.TargetTime = growInterval.PickRandomNumber();
            }
            // else if ()
        }

        for (int i = 0; i < _hub.routeBranches.Count; i++)
        {
            AntRouteBranch branch = _hub.routeBranches[i];
            if (!branch.IsConnectedToNest && branch.UpdateNotConnectedDieTimer())
            {
                _hub.RemoveBranch(i);

                Vector3Int[] gridPosition = branch.AllGridPosition();
                _hub.RemoveGridCollider(gridPosition);
                branch.OnDestroy();
            }
        }
    }


    #region Expand
    bool TryExpandBranch()
    {
        AntRouteBranch branch = _hub.routeBranches[Random.Range(0, _hub.routeBranches.Count)];
        if (!branch.IsConnectedToNest)
            return false;

        if (Random.value > branchOffChance)
        {
            return TryGrowBranch(branch);
        }
        else
        {
            return TryBranchOffFromBranch(branch);
        }
    }

    bool TryGrowBranch(AntRouteBranch branch)
    {
        if (branch.Size >= _maxSize)
            return false;

        Vector3Int position = branch.FindNextGrowPosition();

        if (position == _hub.RootGridPosition)
        {
            branch.AddGrowPosition(position, routeMap.GetCellCenterWorld(position));
            Vector3Int nextPosition = branch.FindNextGrowPosition();
            return GrowBranchInGridPosition(branch, nextPosition);
        }

        return GrowBranchInGridPosition(branch, position);
    }

    bool GrowBranchInGridPosition(AntRouteBranch branch, Vector3Int position)
    {
        if (!GridManager.ins.CheckGroundAvalibleForAnt(position))
            return false;

        if (GridManager.ins.TryFindAntNestBranch(position, out AntNestHub overlapNest, out AntRouteBranch overlapBranch))
        {
            if (overlapNest == _hub)
            {
                if (overlapBranch.IsConnectedToNest)
                    return true;

                overlapBranch.IsConnectedToNest = true;
                RouteSizeIncrease();
                branch.AddGrowPosition(position, routeMap.GetCellCenterWorld(position));
                branch.AddBranchOff(overlapBranch);
                return true;
            }
            else
            {
                if (!CompeteOtherNest(position, overlapNest, overlapBranch))
                    return false;
            }
        }

        RouteSizeIncrease();
        branch.AddGrowPosition(position, routeMap.GetCellCenterWorld(position));
        routeMap.SetTile(position, tilemapReference.ColliderTile);
        return true;
    }

    bool TryBranchOffFromBranch(AntRouteBranch branch)
    {
        if (!branch.FindPotentialBranchOff(out BranchData newBranchData))
            return false;
        if (newBranchData.ExccedPositionCount >= _maxSize)
            return false;
        if (!GridManager.ins.CheckGroundAvalibleForAnt(newBranchData.NextPosition))
            return false;
        

        for (int i = 0; i < _hub.routeBranches.Count; i++)
        {
            AntRouteBranch _branch = _hub.routeBranches[i];
            if (_branch.RootGridPosition == newBranchData.Root && _branch.Direction == newBranchData.Direction)
                return false;
            if (_branch.RootGridPosition == newBranchData.NextPosition)
                return false;
        }

        if (GridManager.ins.TryFindAntNestBranch(newBranchData.NextPosition, out AntNestHub overlapNest, out AntRouteBranch overlapBranch))
        {
            if (overlapNest == _hub)
                return false;
            if (!CompeteOtherNest(newBranchData.NextPosition, overlapNest, overlapBranch))
                return false;
        }

        AntRouteBranch newBranch = new AntRouteBranch(
            newBranchData.Root,
            routeMap.GetCellCenterWorld(newBranchData.Root),
            newBranchData.Direction,
            Instantiate(routeLine, transform),
            disconnectDieTime,
            length: newBranchData.ExccedPositionCount
            );
        newBranch.AddGrowPosition(newBranchData.NextPosition, routeMap.GetCellCenterWorld(newBranchData.NextPosition));

        branch.AddBranchOff(newBranch);
        _hub.routeBranches.Add(newBranch);
        routeMap.SetTile(newBranchData.NextPosition, tilemapReference.ColliderTile);

        RouteSizeIncrease();
        return true;
    }


    void RouteSizeIncrease()
    {
        _size++;

        if (!_hub.IsShowTrueColor)
        {
            if (_size >= showTrueColorAfterSize)
            {
                _hub.ShowTrueColor();
            }
        }

        OnSizeIncrease?.Invoke();
    }
    #endregion


    #region Compete with others
    bool CompeteOtherNest(Vector3Int position, AntNestHub overlapNest, AntRouteBranch overlapBranch)
    {
        if (!CanCompeteWithOtherAntNest(overlapNest, overlapBranch))
            return false;

        if (position == overlapNest.RootGridPosition)
        {
            if (!CanKillOtherAntNest(overlapNest))
                return false;
            overlapNest.MainNestHubDestroy();
        }
        else
        {
            overlapNest.TryKillSpot(overlapBranch, position, BranchSpot.MaxHealth);
            overlapNest.TakeDamageFromOtherNest(1 / overlapNest.RouteSize);
        }
        return true;
    }

    bool CanCompeteWithOtherAntNest(AntNestHub overlapNest, AntRouteBranch overlapBranch)
    {
        if (_hub.IsBiggerThan(overlapNest))
            return true;
        return overlapBranch.IsConnectedToNest;
    }

    bool CanKillOtherAntNest(AntNestHub overlapNest)
    {
        return _hub.IsBiggerThan(overlapNest);
    }
    #endregion
}
