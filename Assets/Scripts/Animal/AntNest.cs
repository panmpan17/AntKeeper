using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using MPack;


public class AntNest : MonoBehaviour
{
    static Vector3Int[] FourDirections = new Vector3Int[] {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(0, 1, 0),
    };


    [SerializeField]
    private bool isFireAnt;
    public bool IsFireAnt => isFireAnt;

    [Header("Reference")]
    [SerializeField]
    private TilemapReference tilemapReference;
    private Tilemap routeMap => tilemapReference.Tilemap;

    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private LineRenderer lineRenderer;

    [Header("Grow route settings")]
    [SerializeField]
    private RangeReference growRouteInterval;
    private Timer _growRouteTimer;
    [SerializeField]
    private IntRangeReference maxRouteSize;
    private int _maxRouteSize;
    [SerializeField]
    private float branchOffChance = 0.05f;
    [SerializeField]
    private RangeReference routeDisconnectDieTime;
    [SerializeField]
    private Timer unableToGrowDieTimer;

    [Header("Grow sprite size")]
    [SerializeField]
    private RangeReference spriteSizeRange;
    [SerializeField]
    private float spriteGrowStep;
    [SerializeField]
    private IntRangeReference spriteGrowByRouteRange;
    [SerializeField]
    [Range(0, 200f)]
    private float spriteKillResistent;
    private int _spriteGrowByRouteCount;
    private float _spriteSize;

    [Header("Spread settings")]
    [SerializeField]
    private RangeReference spreadRange;
    [SerializeField]
    private RangeReference spreadInterval;
    private Timer _spreadIntervalTimer;

    public bool CanSpreadNewNest => _spriteSize >= spriteSizeRange.Max * 0.8f;

    [Header("Kill Animal settings")]
    [SerializeField]
    private RangeVariable killAnimalInterval;
    private Timer _killAnimalTimer;

    [Header("Colors")]
    [SerializeField]
    private ColorReference hiddenColor;
    [SerializeField]
    private ColorReference trueColor;
    [SerializeField]
    private int showTrueColorAfterSize;

    [SerializeField]
    private IntRangeVariable startedSize;
    private int _routeSize;
    private Vector3Int rootPosition;
    public Vector3Int RootPosition => rootPosition;

    private List<AntRouteBranch> _routeBranches;

    public bool IsShowTrueColor { get; protected set; }


    void Awake()
    {
        _routeBranches = new List<AntRouteBranch>();

        ChangeRouteLineRendererColor(hiddenColor.Value);

        _maxRouteSize = maxRouteSize.PickRandomNumber();
        _growRouteTimer = new Timer(growRouteInterval.PickRandomNumber());
        _spreadIntervalTimer = new Timer(spreadInterval.PickRandomNumber());

        if (killAnimalInterval != null)
            _killAnimalTimer = new Timer(killAnimalInterval.PickRandomNumber());

        _spriteSize = spriteSizeRange.Min;
        spriteRenderer.transform.localScale = new Vector3(_spriteSize, _spriteSize, _spriteSize);
        _spriteGrowByRouteCount = spriteGrowByRouteRange.PickRandomNumber();
    }

    void Start()
    {
        rootPosition = routeMap.WorldToCell(transform.position);
        routeMap.SetTile(rootPosition, tilemapReference.ColliderTile);

        for (int i = 0; i < FourDirections.Length; i++)
        {
            _routeBranches.Add(new AntRouteBranch(
                rootPosition,
                routeMap.GetCellCenterWorld(rootPosition),
                FourDirections[i],
                Instantiate(lineRenderer, transform),
                routeDisconnectDieTime
                ));
        }

        // GridManager.ins.RegisterAntNest(this);

        if (startedSize != null)
            WarmUp(startedSize.PickRandomNumber());
    }

    void Update()
    {
        if (_growRouteTimer.UpdateEnd)
        {
            if (TryExpandBranch())
            {
                unableToGrowDieTimer.Reset();
                _growRouteTimer.Reset();
                _growRouteTimer.TargetTime = growRouteInterval.PickRandomNumber();
            }
            else if (_spriteSize < spriteSizeRange.Max && unableToGrowDieTimer.UpdateEnd)
            {
                SpriteSizeTakeDamage(0.2f);

                // DestroyNest();
                return;
            }
        }

        if (killAnimalInterval != null && _killAnimalTimer.UpdateEnd)
        {
            if (TryKillAnimal())
            {
                _killAnimalTimer.Reset();
                _killAnimalTimer.TargetTime = killAnimalInterval.PickRandomNumber();
            }
        }

        if (CanSpreadNewNest && _spreadIntervalTimer.UpdateEnd)
        {
            if (TrySpreadNewNest())
            {
                _spreadIntervalTimer.Reset();
                _spreadIntervalTimer.TargetTime = spreadInterval.PickRandomNumber();
            }
        }

        // Update branches not connected die timer
        for (int i = 0; i < _routeBranches.Count; i++)
        {
            AntRouteBranch branch = _routeBranches[i];
            if (!branch.IsConnectedToNest && branch.UpdateNotConnectedDieTimer())
            {
                // _hub.RemoveBranch(i);

                Vector3Int[] gridPosition = branch.AllGridPosition();
                RemoveGridCollider(gridPosition);
                branch.OnDestroy();
            }
        }
    }


    #region Expand
    bool TryExpandBranch()
    {
        AntRouteBranch branch = _routeBranches[Random.Range(0, _routeBranches.Count)];
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
        if (branch.Size >= _maxRouteSize)
            return false;

        Vector3Int position = branch.FindNextGrowPosition();

        if (position == rootPosition)
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

        // if (GridManager.ins.TryFindAntNestBranch(position, out AntNest againstNest, out AntRouteBranch overlapBranch))
        // {
        //     if (againstNest == this)
        //     {
        //         if (overlapBranch.IsConnectedToNest)
        //             return true;

        //         overlapBranch.IsConnectedToNest = true;
        //         RouteSizeIncrease();
        //         branch.AddGrowPosition(position, routeMap.GetCellCenterWorld(position));
        //         branch.AddBranchOff(overlapBranch);
        //         return true;
        //     }
        //     else
        //     {
        //         if (!CompeteOtherNest(position, againstNest, overlapBranch))
        //             return false;
        //     }
        // }

        RouteSizeIncrease();
        branch.AddGrowPosition(position, routeMap.GetCellCenterWorld(position));
        routeMap.SetTile(position, tilemapReference.ColliderTile);
        return true;
    }

    bool TryBranchOffFromBranch(AntRouteBranch branch)
    {
        if (!branch.FindPotentialBranchOff(out BranchData newBranchData))
            return false;
        if (newBranchData.ExccedPositionCount >= _maxRouteSize)
            return false;
        if (!GridManager.ins.CheckGroundAvalibleForAnt(newBranchData.NextPosition))
            return false;

        // if (GridManager.ins.TryFindAntNestBranch(newBranchData.NextPosition, out AntNest againstNest, out AntRouteBranch overlapBranch))
        // {
        //     if (againstNest == this)
        //         return false;
        //     if (!CompeteOtherNest(newBranchData.NextPosition, againstNest, overlapBranch))
        //         return false;
        // }

        AntRouteBranch newBranch = new AntRouteBranch(
            newBranchData.Root,
            routeMap.GetCellCenterWorld(newBranchData.Root),
            newBranchData.Direction,
            Instantiate(lineRenderer, transform),
            routeDisconnectDieTime,
            length: newBranchData.ExccedPositionCount
            );
        newBranch.AddGrowPosition(newBranchData.NextPosition, routeMap.GetCellCenterWorld(newBranchData.NextPosition));

        branch.AddBranchOff(newBranch);
        _routeBranches.Add(newBranch);
        routeMap.SetTile(newBranchData.NextPosition, tilemapReference.ColliderTile);

        RouteSizeIncrease();
        return true;
    }

    void RouteSizeIncrease()
    {
        _routeSize++;

        if (!IsShowTrueColor)
        {
            if (_routeSize >= showTrueColorAfterSize)
            {
                ShowTrueColor();
            }
        }

        if (--_spriteGrowByRouteCount <= 0)
        {
            _spriteGrowByRouteCount = spriteGrowByRouteRange.PickRandomNumber();
            _spriteSize += spriteGrowStep;
            if (_spriteSize > spriteSizeRange.Max)
                _spriteSize = spriteSizeRange.Max;

            spriteRenderer.transform.localScale = new Vector3(_spriteSize, _spriteSize, _spriteSize);
        }
    }


    bool TrySpreadNewNest()
    {
        Vector2 relativeVector = Random.insideUnitCircle;
        relativeVector.Normalize();
        relativeVector *= spreadRange.PickRandomNumber();

        Vector3Int gridPosition = routeMap.WorldToCell(transform.position + (Vector3)relativeVector);
        return GridManager.ins.InstantiateAntNestOnGrid(gridPosition, IsFireAnt);
    }
    #endregion


    #region Kill and destroy
    bool TryKillAnimal()
    {
        Vector3Int position = _routeBranches[Random.Range(0, _routeBranches.Count)].PickRandomPosition();

        if (GridManager.ins.TryFindAnimal(position, out VirtualAnimalSpot animalSpot))
        {
            animalSpot.Kill();
            return true;
        }

        return false;
    }

    public void TryKillSpot(Vector3Int position, float killAmount, AntRouteBranch branch)
    {
        Vector3Int[] removedPositions = branch.KillSpot(position, killAmount, out AntRouteBranch newBranch);
        if (removedPositions != null)
            RemoveGridCollider(removedPositions);

        // If there's new branch spereate from original, add it to branch list
        if (newBranch != null)
        {
            newBranch.RecalculateLineRenderer(routeMap);
            _routeBranches.Add(newBranch);
        }

        // If there's nothing left in branch try to remove it
        if (branch.IsEmpty)
        {
            // Prevent root branch
            int index = _routeBranches.IndexOf(branch);
            if (index > 3)
            {
                // _hub.RemoveBranch(index);
            }
        }

        if (position == rootPosition)
        {
            if (!FindRootAliveBranch())
            {
                SpriteSizeTakeDamage(killAmount / spriteKillResistent);
            }
        }
    }

    public void SpriteSizeTakeDamage(float damageAmount)
    {
        _spriteSize -= damageAmount;
        spriteRenderer.transform.localScale = new Vector3(_spriteSize, _spriteSize, _spriteSize);

        if (_spriteSize < spriteSizeRange.Min)
        {
            DestroyNest();
        }
    }

    void DestroyNest()
    {
        // GridManager.ins.UnregisterAntNest(this);

        for (int i = 0; i < _routeBranches.Count; i++)
        {
            Vector3Int[] gridPositions = _routeBranches[i].AllGridPosition();
            for (int e = 0; e < gridPositions.Length; e++)
                routeMap.SetTile(gridPositions[e], null);
            _routeBranches[i].OnDestroy();
        }

        routeMap.SetTile(rootPosition, null);
        Destroy(gameObject);
    }



    bool CompeteOtherNest(Vector3Int position, AntNest againstNest, AntRouteBranch overlapBranch)
    {
        if (!CanCompeteWithOtherAntNest(againstNest, overlapBranch))
            return false;

        if (position == againstNest.rootPosition)
        {
            if (!CanKillOtherAntNest(againstNest))
                return false;
            againstNest.DestroyNest();
        }
        else
        {
            againstNest.TryKillSpot(position, BranchSpot.MaxHealth, overlapBranch);
            againstNest.SpriteSizeTakeDamage(1 / againstNest._routeSize);
        }
        return true;
    }

    bool CanCompeteWithOtherAntNest(AntNest againstNest, AntRouteBranch overlapBranch)
    {
        if (againstNest._spriteSize > _spriteSize)
        {
            if (overlapBranch.IsConnectedToNest)
                return false;
        }
        return true;
    }

    bool CanKillOtherAntNest(AntNest againstNest)
    {
        return againstNest._spriteSize > _spriteSize;
    }
    #endregion


    #region Utilties
    void RemoveGridCollider(Vector3Int[] gridPositions)
    {
        for (int i = 0; i < gridPositions.Length; i++)
        {
            if (gridPositions[i] == rootPosition)
                continue;
            if (!IsGridPositionOverlapBranch(gridPositions[i]))
                routeMap.SetTile(gridPositions[i], null);
        }
    }

    public bool IsGridPositionOverlapBranch(Vector3Int position)
    {
        for (int i = 0; i < _routeBranches.Count; i++)
        {
            if (_routeBranches[i].IsOverlap(position))
                return true;
        }
        return false;
    }

    bool FindRootAliveBranch()
    {
        for (int i = 0; i < 4; i++)
        {
            if (!_routeBranches[i].IsEmpty)
                return true;
        }
        return false;
    }
    bool FindRootAliveBranch(out AntRouteBranch branch)
    {
        for (int i = 0; i < 4; i++)
        {
            if (!_routeBranches[i].IsEmpty)
            {
                branch = _routeBranches[i];
                return true;
            }
        }
        branch = null;
        return false;
    }

    public bool IsGridPositionOverlapBranch(Vector3Int position, out AntRouteBranch branch)
    {
        if (position == rootPosition)
        {
            if (FindRootAliveBranch(out branch))
                return true;
            branch = _routeBranches[0];
            return true;
        }

        for (int i = 0; i < _routeBranches.Count; i++)
        {
            if (_routeBranches[i].IsOverlap(position))
            {
                branch = _routeBranches[i];
                return true;
            }
        }
        branch = null;
        return false;
    }

    public void WarmUp(int warmUpTime, int maxLoop=1000)
    {
        int count = 0;

        while (count < warmUpTime && count < maxLoop)
        {
            if (TryExpandBranch())
            {
                count++;
            }
        }
    }

    void ChangeRouteLineRendererColor(Color newColor)
    {
        lineRenderer.startColor = newColor;
        lineRenderer.endColor = newColor;

        for (int i = 0; i < _routeBranches.Count; i++)
        {
            _routeBranches[i].ChangeRouteLineRendererColor(newColor);
        }
    }

    public void ShowTrueColor()
    {
        IsShowTrueColor = true;
        ChangeRouteLineRendererColor(trueColor.Value);
    }
    #endregion


    #region Editor
    void OnDrawGizmosSelected()
    {
        if (_routeBranches != null)
        {
            for (int i = 0; i < _routeBranches.Count; i++)
            {
                var branch = _routeBranches[i];
                Gizmos.color = branch.IsConnectedToNest ? Color.green : Color.red;
                Gizmos.DrawSphere(routeMap.GetCellCenterWorld(_routeBranches[i].RootGridPosition), 0.1f);
            }
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, spreadRange.Min);
        Gizmos.DrawWireSphere(transform.position, spreadRange.Max);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxRouteSize.Min);
        Gizmos.DrawWireSphere(transform.position, maxRouteSize.Max);
    }
    #endregion
}
