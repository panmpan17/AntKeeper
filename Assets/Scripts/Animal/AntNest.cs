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

    [Header("Spread settings")]
    [SerializeField]
    private RangeReference spreadRouteInterval;
    private Timer _spreadTimer;
    [SerializeField]
    private IntRangeReference maxSpreadSizeReference;
    private int _maxSpreadSize;
    [SerializeField]
    private float branchOffChance = 0.05f;
    [SerializeField]
    private RangeReference routeDisconnectDieTimeReference;

    [Header("Other settings")]
    [SerializeField]
    private RangeReference killAnimalInterval;
    private Timer _killAnimalTimer;

    [SerializeField]
    private Color routeColor;

    [SerializeField]
    private IntRangeReference startedSize;

    private List<AntRouteBranch> routeBranches;

    void Awake()
    {
        routeBranches = new List<AntRouteBranch>();
        
        lineRenderer.startColor = routeColor;
        lineRenderer.endColor = routeColor;

        _maxSpreadSize = maxSpreadSizeReference.PickRandomNumber();
        _spreadTimer = new Timer(spreadRouteInterval.PickRandomNumber());
        _killAnimalTimer = new Timer(killAnimalInterval.PickRandomNumber());
    }

    void Start()
    {
        Vector3Int rootPosition = routeMap.WorldToCell(transform.position);
        routeMap.SetTile(rootPosition, routeTile);

        for (int i = 0; i < FourDirections.Length; i++)
        {
            routeBranches.Add(new AntRouteBranch(
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
        if (_spreadTimer.UpdateEnd)
        {
            if (TrySpreadBranch())
            {
                _spreadTimer.Reset();
                _spreadTimer.TargetTime = spreadRouteInterval.PickRandomNumber();
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
        for (int i = 0; i < routeBranches.Count; i++)
        {
            if (!routeBranches[i].IsConnectedToNest && routeBranches[i].UpdateNotConnectedDieTimer())
            {
                Vector3Int[] gridPosition = routeBranches[i].AllGridPosition();
                RemoveGridCollider(gridPosition);
                routeBranches[i].OnDestroy();

                routeBranches.RemoveAt(i);
            }
        }
    }

    bool TrySpreadBranch()
    {
        AntRouteBranch branch = routeBranches[Random.Range(0, routeBranches.Count)];
        if (!branch.IsConnectedToNest)
            return false;

        float randomValue = Random.value;

        if (randomValue > branchOffChance)
        {
            if (branch.Size < _maxSpreadSize)
            {
                Vector3Int position = branch.FindNextSpreadPosition();

                if (IsGridPositionOverlapBranch(position, out AntRouteBranch overlapBranch))
                {
                    if (!overlapBranch.IsConnectedToNest)
                    {
                        overlapBranch.IsConnectedToNest = true;
                        branch.AddSpreadPosition(position, routeMap.GetCellCenterWorld(position));
                        branch.AddBranchOff(overlapBranch);
                        return true;
                    }
                }
                else
                {
                    branch.AddSpreadPosition(position, routeMap.GetCellCenterWorld(position));
                    routeMap.SetTile(position, routeTile);
                    return true;
                }
            }
        }
        else if (branch.FindPotentialBranchOff(out BranchData newBranchData))
        {
            if (newBranchData.ExccedPositionCount < _maxSpreadSize && !IsGridPositionOverlapBranch(newBranchData.NextPosition))
            {
                AntRouteBranch newBranch = new AntRouteBranch(
                    newBranchData.Root,
                    routeMap.GetCellCenterWorld(newBranchData.Root),
                    newBranchData.Direction,
                    Instantiate(lineRenderer, transform),
                    routeDisconnectDieTimeReference,
                    length: newBranchData.ExccedPositionCount
                    );
                newBranch.AddSpreadPosition(newBranchData.NextPosition, routeMap.GetCellCenterWorld(newBranchData.NextPosition));

                branch.AddBranchOff(newBranch);
                routeBranches.Add(newBranch);

                routeMap.SetTile(newBranchData.NextPosition, routeTile);
                return true;
            }
        }

        return false;
    }

    bool TryKillAnimal()
    {
        Vector3Int position = routeBranches[Random.Range(0, routeBranches.Count)].PickRandomPosition();

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
            routeBranches.Add(newBranch);
        }

        if (branch.IsEmpty)
        {
            int index = routeBranches.IndexOf(branch);
            if (index > 3)
            {
                routeBranches.RemoveAt(index);
            }
        }
    }


    #region Utilties
    void RemoveGridCollider(Vector3Int[] gridPositions)
    {
        for (int i = 0; i < gridPositions.Length; i++)
        {
            routeMap.SetTile(gridPositions[i], null);
        }
    }

    public bool IsGridPositionOverlapBranch(Vector3Int position)
    {
        for (int i = 0; i < routeBranches.Count; i++)
        {
            if (routeBranches[i].IsOverlap(position))
                return true;
        }
        return false;
    }

    public bool IsGridPositionOverlapBranch(Vector3Int position, out AntRouteBranch branch)
    {
        for (int i = 0; i < routeBranches.Count; i++)
        {
            if (routeBranches[i].IsOverlap(position))
            {
                branch = routeBranches[i];
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
            if (TrySpreadBranch())
            {
                count++;
            }
        }
    }
    #endregion


    #region Editor
    void OnDrawGizmosSelected()
    {
        if (routeBranches != null)
        {
            for (int i = 0; i < routeBranches.Count; i++)
            {
                var branch = routeBranches[i];
                Gizmos.color = branch.IsConnectedToNest ? Color.green : Color.red;
                Gizmos.DrawSphere(routeMap.GetCellCenterWorld(routeBranches[i].RootGridPosition), 0.1f);
            }
        }
    }
    #endregion
}
