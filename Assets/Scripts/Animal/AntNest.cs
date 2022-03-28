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
    private SpreadType spreadType;
    [SerializeField]
    private Timer spreadSpeed;
    [SerializeField]
    private int maxSpreadSize;

    private List<Vector3Int> routePositions;

    private List<Branch> routeBranches;

    void Awake()
    {
        Vector3Int rootPosition = routeMap.WorldToCell(transform.position);
        routeMap.SetTile(rootPosition, routeTile);

        switch (spreadType)
        {
            case SpreadType.RandomSpreadFromExistRoute:
                routePositions = new List<Vector3Int>();
                routePositions.Add(rootPosition);
                break;
            case SpreadType.BranchSpread:
                routeBranches = new List<Branch>();

                for (int i = 0; i < FourDirections.Length; i++)
                {
                    routeBranches.Add(new Branch(
                        rootPosition,
                        routeMap.GetCellCenterWorld(rootPosition),
                        FourDirections[i],
                        Instantiate(lineRenderer, transform)
                        ));
                }
                break;
        }
    }

    void Update()
    {
        if (spreadSpeed.UpdateEnd)
        {
            switch(spreadType)
            {
                case SpreadType.RandomSpreadFromExistRoute:
                    RandomSpreadFromExistRoute();
                    spreadSpeed.Reset();
                    break;
                case SpreadType.BranchSpread:
                    if (BranchSpread())
                        spreadSpeed.Reset();
                    break;
            }
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

    public bool IsOverlapBranch(Vector3Int position)
    {
        for (int i = 0; i < routeBranches.Count; i++)
        {
            if (routeBranches[i].IsOverlap(position))
                return true;
        }
        return false;
    }
    public bool IsOverlapBranch(Vector3Int position, out Branch branch)
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

    bool BranchSpread()
    {
        Branch branch = routeBranches[Random.Range(0, routeBranches.Count)];

        float randomValue = Random.value;

        if (randomValue > 0.05f)
        {
            if (branch.Size < maxSpreadSize)
            {
                Vector3Int position = branch.FindNextSpreadPosition();

                if (!IsOverlapBranch(position))
                {
                    branch.AddPosition(position, routeMap.GetCellCenterWorld(position));
                    routeMap.SetTile(position, routeTile);
                    return true;
                }
            }
        }
        else
        {
            BranchData newBranchData = branch.BranchOff();

            if (newBranchData.ExccedPositionCount < maxSpreadSize && !IsOverlapBranch(newBranchData.NextPosition))
            {
                Branch newBranch = new Branch(
                    newBranchData.Root,
                    routeMap.GetCellCenterWorld(newBranchData.Root),
                    newBranchData.Direction,
                    Instantiate(lineRenderer, transform),
                    length: newBranchData.ExccedPositionCount);

                newBranch.AddPosition(newBranchData.NextPosition, routeMap.GetCellCenterWorld(newBranchData.NextPosition));
                routeMap.SetTile(newBranchData.NextPosition, routeTile);

                routeBranches.Add(newBranch);
                return true;
            }
        }

        return false;
    }

    void OnDrawGizmosSelected()
    {
        if (routeBranches != null)
        {
            for (int i = 0; i < routeBranches.Count; i++)
            {
                Gizmos.DrawSphere(routeMap.GetCellCenterWorld(routeBranches[i].Root), 0.1f);
            }
        }
    }

    public class Branch
    {
        static Vector3Int SideWayDirection(Vector3Int direction)
        {
            int multiplier = Random.value > 0.5f ? 1 : -1;
            return new Vector3Int(direction.y * multiplier, direction.x * multiplier, 0);
        }

        private List<Vector3Int> _positions;
        private Vector3Int _direction;
        private int originalLength;

        private LineRenderer _branchLine;

        public int Size => _positions.Count + originalLength;
        public Vector3Int Root => _positions[0];

        public Branch(Vector3Int root, Vector3 rootPosition, Vector3Int direciton, LineRenderer branchLine, int length=0)
        {
            _positions = new List<Vector3Int>();
            _positions.Add(root);

            _branchLine = branchLine;

            _direction = direciton;
            originalLength = length;

            _branchLine.positionCount = 1;
            _branchLine.SetPositions(new Vector3[] { rootPosition });
        }

        public bool IsOverlap(Vector3Int position) => _positions.Contains(position);

        public Vector3Int FindNextSpreadPosition()
        {
            return _positions[_positions.Count - 1] + (Random.value > 0.8f ? SideWayDirection(_direction) : _direction);
        }

        public BranchData BranchOff()
        {
            int index = Random.Range(0, _positions.Count);
            Vector3Int newDirection = SideWayDirection(_direction);
            // Vector3Int newPosition = _positions[index] + newDirection;
            // newBranch = new Branch(newPosition, newDirection, index + 1);
            return new BranchData {
                Root = _positions[index],
                Direction = newDirection,
                ExccedPositionCount = originalLength + index + 1,
            };
        }

        public void AddPosition(Vector3Int position, Vector3 worldPosition)
        {
            _positions.Add(position);

            Vector3[] newLinePoints = new Vector3[_branchLine.positionCount + 1];
            _branchLine.GetPositions(newLinePoints);
            _branchLine.positionCount = _branchLine.positionCount + 1;
            newLinePoints[newLinePoints.Length - 1] = worldPosition;
            _branchLine.SetPositions(newLinePoints);
        }
    }

    public struct BranchData
    {
        public Vector3Int Root;
        public Vector3Int Direction;
        public int ExccedPositionCount;

        public Vector3Int NextPosition => Root + Direction;
    }

    public enum SpreadType
    {
        RandomSpreadFromExistRoute,
        BranchSpread,
    }
}
