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

    [SerializeField]
    private Timer killAnimalTimer;

    [SerializeField]
    private Color routeColor;

    private List<AntRouteBranch> routeBranches;

    void Awake()
    {
        Vector3Int rootPosition = routeMap.WorldToCell(transform.position);
        routeMap.SetTile(rootPosition, routeTile);

        routeBranches = new List<AntRouteBranch>();
        
        lineRenderer.startColor = routeColor;
        lineRenderer.endColor = routeColor;

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

        if (killAnimalTimer.UpdateEnd)
        {
            if (TryFindAnimalToKill())
                killAnimalTimer.Reset();
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
            if (newBranchData.ExccedPositionCount < maxSpreadSize && !IsGridPositionOverlapBranch(newBranchData.NextPosition))
            {
                AntRouteBranch newBranch = new AntRouteBranch(
                    newBranchData.Root,
                    routeMap.GetCellCenterWorld(newBranchData.Root),
                    newBranchData.Direction,
                    Instantiate(lineRenderer, transform),
                    length: newBranchData.ExccedPositionCount);
                newBranch.AddSpreadPosition(newBranchData.NextPosition, routeMap.GetCellCenterWorld(newBranchData.NextPosition));

                branch.AddBranchOff(newBranch);
                routeBranches.Add(newBranch);

                routeMap.SetTile(newBranchData.NextPosition, routeTile);
                return true;
            }
        }

        return false;
    }

    bool TryFindAnimalToKill()
    {
        Vector3Int position = routeBranches[Random.Range(0, routeBranches.Count)].RandomPosition;

        if (GridManager.ins.TryFindAnimal(position, out VirtualAnimalSpot animalSpot))
        {
            animalSpot.Kill();
            return true;
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


    public void TryKillSpot(Vector3Int position, float killAmount, AntRouteBranch branch)
    {
        Vector3Int[] removedPositions = branch.KillSpot(position, killAmount, out AntRouteBranch newBranch);
        if (removedPositions != null)
        {
            for (int i = 0; i < removedPositions.Length; i++)
            {
                routeMap.SetTile(removedPositions[i], null);

                // for (int e = 0; e < routeBranches.Count; e++)
                // {
                //     if (removedPositions[i] == routeBranches[e].RootGridPosition)
                //     {
                //         routeBranches[e].IsConnectedToNest = false;
                //     }
                // }
            }
        }

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
                var branch = routeBranches[i];
                Gizmos.color = branch.IsConnectedToNest ? Color.green : Color.red;
                Gizmos.DrawSphere(routeMap.GetCellCenterWorld(routeBranches[i].RootGridPosition), 0.1f);
            }
        }
    }
}
