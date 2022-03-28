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
    private Tilemap routeMap;
    [SerializeField]
    private LineRenderer lineRenderer;
    [SerializeField]
    private RuleTile routeTile;

    [SerializeField]
    private Timer spreadSpeed;
    [SerializeField]
    private int maxSpreadSize;

    private List<Vector3Int> routePositions;

    private List<AntRouteBranch> routeBranches;

    void Awake()
    {
        Vector3Int rootPosition = routeMap.WorldToCell(transform.position);
        routeMap.SetTile(rootPosition, routeTile);

        routeBranches = new List<AntRouteBranch>();

        for (int i = 0; i < FourDirections.Length; i++)
        {
            routeBranches.Add(new AntRouteBranch(
                rootPosition,
                routeMap.GetCellCenterWorld(rootPosition),
                FourDirections[i],
                Instantiate(lineRenderer, transform)
                ));
        }
    }

    void Start()
    {
        GridManager.ins.RegisterAntNest(this);
    }

    void Update()
    {
        if (spreadSpeed.UpdateEnd)
        {
            if (BranchSpread())
                spreadSpeed.Reset();
        }
    }

    void RandomSpreadFromExistRoute()
    {
        Vector3Int position = routePositions[Random.Range(0, routePositions.Count)];
        position += FourDirections[Random.Range(0, FourDirections.Length)];

        if (!routePositions.Contains(position))
        {
            routeMap.SetTile(position, routeTile);
            routePositions.Add(position);
        }
    }

    bool BranchSpread()
    {
        AntRouteBranch branch = routeBranches[Random.Range(0, routeBranches.Count)];
        if (!branch.IsConnectedToNest)
            return false;

        float randomValue = Random.value;

        if (randomValue > 0.05f)
        {
            if (branch.Size < maxSpreadSize)
            {
                Vector3Int position = branch.FindNextSpreadPosition();

                if (IsGridPositionOverlapBranch(position, out AntRouteBranch overlapBranch))
                {
                    if (!overlapBranch.IsConnectedToNest)
                    {
                        overlapBranch.IsConnectedToNest = true;
                        branch.AddPosition(position, routeMap.GetCellCenterWorld(position));
                        return true;
                    }
                }
                else
                {
                    branch.AddPosition(position, routeMap.GetCellCenterWorld(position));
                    routeMap.SetTile(position, routeTile);
                    return true;
                }
            }
        }
        else if (branch.BranchOff(out BranchData newBranchData))
        {
            if (newBranchData.ExccedPositionCount < maxSpreadSize && !IsGridPositionOverlapBranch(newBranchData.NextPosition))
            {
                AntRouteBranch newBranch = new AntRouteBranch(
                    newBranchData.Root,
                    routeMap.GetCellCenterWorld(newBranchData.Root),
                    newBranchData.Direction,
                    Instantiate(lineRenderer, transform),
                    length: newBranchData.ExccedPositionCount);
                newBranch.AddPosition(newBranchData.NextPosition, routeMap.GetCellCenterWorld(newBranchData.NextPosition));
                routeBranches.Add(newBranch);

                routeMap.SetTile(newBranchData.NextPosition, routeTile);
                return true;
            }
        }

        return false;
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


    public void TryKillSpot(Vector3Int position, AntRouteBranch branch)
    {
        Vector3Int[] removedPositions = branch.KillSpot(position, out AntRouteBranch newBranch);
        routeMap.SetTile(position, null);

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

    void OnDrawGizmosSelected()
    {
        if (routeBranches != null)
        {
            for (int i = 0; i < routeBranches.Count; i++)
            {
                Gizmos.DrawSphere(routeMap.GetCellCenterWorld(routeBranches[i].RootGridPosition), 0.1f);
            }
        }
    }
}
