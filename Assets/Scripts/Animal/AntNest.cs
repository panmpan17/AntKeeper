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

    [Header("Reference")]
    [SerializeField]
    private TilemapReference routeMapReference;
    private Tilemap routeMap => routeMapReference.Tilemap;

    [SerializeField]
    private LineRenderer lineRenderer;
    [SerializeField]
    private RuleTile routeTile;

    [Header("Grow route settings")]
    [SerializeField]
    private RangeReference growRouteInterval;
    private Timer _growRouteTimer;
    [SerializeField]
    private IntRangeReference maxRouteSizeReference;
    private int _maxRouteSize;
    [SerializeField]
    private float branchOffChance = 0.05f;
    [SerializeField]
    private RangeReference routeDisconnectDieTimeReference;

    [Header("Spread settings")]
    [SerializeField]
    private RangeReference spreadRangeReference;
    [SerializeField]
    private RangeReference spreadIntervalReference;
    private Timer _spreadIntervalTimer;

    [Header("Kill Animal settings")]
    [SerializeField]
    private RangeReference killAnimalInterval;
    private Timer _killAnimalTimer;

    [Header("Colors")]
    [SerializeField]
    private ColorReference hiddenColorReference;
    [SerializeField]
    private ColorReference trueColorReference;
    [SerializeField]
    private int showTrueColorAfterSize;

    [SerializeField]
    private IntRangeReference startedSize;

    private int size {
        get => _size;
        set {
            _size = value;

            if (lineRenderer.startColor == hiddenColorReference.Value)
            {
                if (_size >= showTrueColorAfterSize)
                {
                    ChangeRouteLineRendererColor(trueColorReference.Value);
                }
            }
        }
    }
    private int _size;

    private List<AntRouteBranch> _routeBranches;

    public bool IsShowTrueColor => lineRenderer.startColor == trueColorReference.Value;


    void Awake()
    {
        _routeBranches = new List<AntRouteBranch>();

        lineRenderer.startColor = hiddenColorReference.Value;
        lineRenderer.endColor = hiddenColorReference.Value;

        _maxRouteSize = maxRouteSizeReference.PickRandomNumber();
        _growRouteTimer = new Timer(growRouteInterval.PickRandomNumber());
        _killAnimalTimer = new Timer(killAnimalInterval.PickRandomNumber());
    }

    void Start()
    {
        Vector3Int rootPosition = routeMap.WorldToCell(transform.position);
        routeMap.SetTile(rootPosition, routeTile);

        for (int i = 0; i < FourDirections.Length; i++)
        {
            _routeBranches.Add(new AntRouteBranch(
                rootPosition,
                routeMap.GetCellCenterWorld(rootPosition),
                FourDirections[i],
                Instantiate(lineRenderer, transform),
                routeDisconnectDieTimeReference
                ));
        }

        GridManager.ins.RegisterAntNest(this);

        if (startedSize != null)
            WarmUp(startedSize.PickRandomNumber());
    }

    void Update()
    {
        if (_growRouteTimer.UpdateEnd)
        {
            if (TryGrowBranch())
            {
                _growRouteTimer.Reset();
                _growRouteTimer.TargetTime = growRouteInterval.PickRandomNumber();
            }
        }

        if (_killAnimalTimer.UpdateEnd)
        {
            if (TryKillAnimal())
            {
                _killAnimalTimer.Reset();
                _killAnimalTimer.TargetTime = killAnimalInterval.PickRandomNumber();
            }
        }

        // Update branches not connected die timer
        for (int i = 0; i < _routeBranches.Count; i++)
        {
            AntRouteBranch branch = _routeBranches[i];
            if (!branch.IsConnectedToNest && branch.UpdateNotConnectedDieTimer())
            {
                _routeBranches.RemoveAt(i);

                Vector3Int[] gridPosition = branch.AllGridPosition();
                RemoveGridCollider(gridPosition);
                branch.OnDestroy();

            }
        }
    }

    bool TryGrowBranch()
    {
        AntRouteBranch branch = _routeBranches[Random.Range(0, _routeBranches.Count)];
        if (!branch.IsConnectedToNest)
            return false;

        float randomValue = Random.value;

        if (randomValue > branchOffChance)
        {
            if (branch.Size < _maxRouteSize)
            {
                Vector3Int position = branch.FindNextGrowPosition();

                if (IsGridPositionOverlapBranch(position, out AntRouteBranch overlapBranch))
                {
                    if (!overlapBranch.IsConnectedToNest)
                    {
                        size++;
                        overlapBranch.IsConnectedToNest = true;
                        branch.AddGrowPosition(position, routeMap.GetCellCenterWorld(position));
                        branch.AddBranchOff(overlapBranch);
                        return true;
                    }
                }
                else
                {
                    size++;
                    branch.AddGrowPosition(position, routeMap.GetCellCenterWorld(position));
                    routeMap.SetTile(position, routeTile);
                    return true;
                }
            }
        }
        else if (branch.FindPotentialBranchOff(out BranchData newBranchData))
        {
            if (newBranchData.ExccedPositionCount < _maxRouteSize && !IsGridPositionOverlapBranch(newBranchData.NextPosition))
            {
                AntRouteBranch newBranch = new AntRouteBranch(
                    newBranchData.Root,
                    routeMap.GetCellCenterWorld(newBranchData.Root),
                    newBranchData.Direction,
                    Instantiate(lineRenderer, transform),
                    routeDisconnectDieTimeReference,
                    length: newBranchData.ExccedPositionCount
                    );
                newBranch.AddGrowPosition(newBranchData.NextPosition, routeMap.GetCellCenterWorld(newBranchData.NextPosition));

                size++;
                branch.AddBranchOff(newBranch);
                _routeBranches.Add(newBranch);

                routeMap.SetTile(newBranchData.NextPosition, routeTile);
                return true;
            }
        }

        return false;
    }

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

        if (newBranch != null)
        {
            newBranch.RecalculateLineRenderer(routeMap);
            _routeBranches.Add(newBranch);
        }

        if (branch.IsEmpty)
        {
            int index = _routeBranches.IndexOf(branch);
            if (index > 3)
            {
                _routeBranches.RemoveAt(index);
            }
        }
    }


    #region Utilties
    void RemoveGridCollider(Vector3Int[] gridPositions)
    {
        for (int i = 0; i < gridPositions.Length; i++)
        {
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

    public bool IsGridPositionOverlapBranch(Vector3Int position, out AntRouteBranch branch)
    {
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
            if (TryGrowBranch())
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
        ChangeRouteLineRendererColor(trueColorReference.Value);
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

        if (spreadRangeReference != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, spreadRangeReference.Min);
            Gizmos.DrawWireSphere(transform.position, spreadRangeReference.Max);
        }

        if (maxRouteSizeReference != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, maxRouteSizeReference.Min);
            Gizmos.DrawWireSphere(transform.position, maxRouteSizeReference.Max);
        }
    }
    #endregion
}
