using System.Collections;
using System.Collections.Generic;
using MPack;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AntNestHub : MonoBehaviour
{
    [SerializeField]
    private bool isFireAnt;
    public bool IsFireAnt => isFireAnt;

    [Header("Reference")]
    [SerializeField]
    private TilemapReference tilemapReference;
    // private Tilemap routeMap => tilemapReference.Tilemap;

    // [SerializeField]
    // private SpriteRenderer spriteRenderer;
    [SerializeField]
    private LineRenderer routeLine;

    [Header("Colors")]
    [SerializeField]
    private ColorReference hiddenColor;
    [SerializeField]
    private ColorReference trueColor;

    [System.NonSerialized]
    public List<AntRouteBranch> routeBranches = new List<AntRouteBranch>();

    public Vector3Int RootGridPosition;
    private AntRouteGrowControl antRouteGrowControl;
    private AntNestSizeControl antNestSizeControl;

    public event System.Action OnStart;
    public event System.Action<float> OnOtherNestAttack;
    public event System.Action<float> OnRootGridTakeDamage;

    public int RouteSize => antRouteGrowControl.Size;
    public bool IsShowTrueColor { get; protected set; }

    void Awake()
    {
        ChangeRouteLineRendererColor(hiddenColor.Value);
        antRouteGrowControl = GetComponent<AntRouteGrowControl>();
        antNestSizeControl = GetComponent<AntNestSizeControl>();
    }

    void Start()
    {
        RootGridPosition = tilemapReference.Tilemap.WorldToCell(transform.position);
        tilemapReference.Tilemap.SetTile(RootGridPosition, tilemapReference.ColliderTile);

        OnStart?.Invoke();

        GridManager.ins.RegisterAntNest(this);
    }


    void ChangeRouteLineRendererColor(Color newColor)
    {
        routeLine.startColor = newColor;
        routeLine.endColor = newColor;

        for (int i = 0; i < routeBranches.Count; i++)
        {
            routeBranches[i].ChangeRouteLineRendererColor(newColor);
        }
    }
    public void ShowTrueColor()
    {
        ChangeRouteLineRendererColor(trueColor.Value);
    }


    public void TryKillSpot(AntRouteBranch targetBranch, Vector3Int position, float damageAmount)
    {
        Vector3Int[] removedPositions = targetBranch.KillSpot(position, damageAmount, out AntRouteBranch newBranch);
        if (removedPositions != null)
            RemoveGridCollider(removedPositions);

        // If there's new branch spereate from original, add it to branch list
        if (newBranch != null)
        {
            newBranch.RecalculateLineRenderer(tilemapReference.Tilemap);
            routeBranches.Add(newBranch);
        }

        // If there's nothing left in branch try to remove it
        if (targetBranch.IsEmpty)
        {
            // Prevent root branch
            int index = routeBranches.IndexOf(targetBranch);
            if (index > 3)
            {
                routeBranches.RemoveAt(index);
            }
        }

        if (position == RootGridPosition)
        {
            if (!IsRootBranchAlive())
            {
                OnRootGridTakeDamage?.Invoke(damageAmount);
            }
        }
    }


    #region Check overlapping
    public bool IsGridPositionOverlapBranch(Vector3Int position)
    {
        if (position == RootGridPosition)
            return true;
        for (int i = 0; i < routeBranches.Count; i++)
        {
            if (routeBranches[i].IsOverlap(position))
            {
                return true;
            }
        }
        return false;
    }
    public bool IsGridPositionOverlapBranch(Vector3Int position, out AntRouteBranch branch)
    {
        if (position == RootGridPosition)
        {
            if (IsRootBranchAlive(out branch))
                return true;
            branch = routeBranches[0];
            return true;
        }
            
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
    #endregion



    public bool IsBiggerThan(AntNestHub otherHub)
    {
        return true;
    }

    public void DestroyNest()
    {
        throw new System.NotImplementedException();
    }

    public void RemoveGridCollider(Vector3Int[] gridPositions)
    {
        for (int i = 0; i < gridPositions.Length; i++)
        {
            if (gridPositions[i] == RootGridPosition)
                continue;
            if (!IsGridPositionOverlapBranch(gridPositions[i]))
                tilemapReference.Tilemap.SetTile(gridPositions[i], null);
        }
    }

    bool IsRootBranchAlive()
    {
        for (int i = 0; i < 4; i++)
        {
            if (!routeBranches[i].IsEmpty)
                return true;
        }
        return false;
    }
    bool IsRootBranchAlive(out AntRouteBranch branch)
    {
        for (int i = 0; i < 4; i++)
        {
            if (!routeBranches[i].IsEmpty)
            {
                branch = routeBranches[i];
                return true;
            }
        }
        branch = null;
        return false;
    }

    public void TakeDamageFromOtherNest(float damageAmount)
    {
        OnOtherNestAttack?.Invoke(damageAmount);
    }

    #region Editor
    void OnDrawGizmosSelected()
    {
        if (routeBranches != null)
        {
            for (int i = 0; i < routeBranches.Count; i++)
            {
                var branch = routeBranches[i];
                Gizmos.color = branch.IsConnectedToNest ? Color.green : Color.red;
                Gizmos.DrawSphere(tilemapReference.Tilemap.GetCellCenterWorld(routeBranches[i].RootGridPosition), 0.1f);
            }
        }
    }
    #endregion
}
