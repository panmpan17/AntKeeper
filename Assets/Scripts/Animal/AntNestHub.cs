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

    public event System.Action OnStart;

    public bool IsShowTrueColor { get; protected set; }

    void Awake()
    {
        ChangeRouteLineRendererColor(hiddenColor.Value);
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


    public bool TryKillSpot(AntRouteBranch targetBranch, Vector3 position, float damageAmount)
    {
        throw new System.NotImplementedException();
    }


    #region Check overlapping
    public bool IsGridPositionOverlapBranch(Vector3Int position)
    {
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
