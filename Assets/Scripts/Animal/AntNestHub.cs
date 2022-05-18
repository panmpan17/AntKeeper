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
    [SerializeField]
    private LineRenderer routeLine;
    [SerializeField]
    private EffectReference revealColorEffect;

    [Header("Colors")]
    [SerializeField]
    private ColorReference hiddenColor;
    [SerializeField]
    private ColorReference trueColor;

    [System.NonSerialized]
    public List<AntRouteBranch> routeBranches = new List<AntRouteBranch>();

    [System.NonSerialized]
    public Vector3Int RootGridPosition;
    private AntRouteGrowControl antRouteGrowControl;
    private AntNestSizeControl antNestSizeControl;

    public event System.Action OnStart;
    public event System.Action OnNestDestroy;
    public event System.Action OnAntRouteBranchEmpty;
    public event System.Action<float> OnOtherNestAttack;
    public event System.Action<float> OnRootGridTakeDamage;

    public float Size => antNestSizeControl.Size;
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
        revealColorEffect.AddWaitingList(new EffectReference.EffectQueue
        {
            Parent = null,
            Position = transform.position,
            UseScaleTime = true,
        });

        ChangeRouteLineRendererColor(trueColor.Value);
    }


    public void DamageBranchAtPosition(AntRouteBranch targetBranch, Vector3Int position, float damageAmount)
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
                RemoveBranch(index);
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
        if (position == RootGridPosition && enabled)
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


    #region Utilities
    public void Freeze()
    {
        if (GetComponent<AntRouteGrowControl>() is var growControl && growControl != null)
        {
            growControl.enabled = false;
        }
        if (GetComponent<AntKillAnimalControl>() is var killControl && killControl != null)
        {
            killControl.enabled = false;
        }
        if (GetComponent<AntRouteSpreadControl>() is var spreadControl && spreadControl != null)
        {
            spreadControl.enabled = false;
        }
    }

    public void Unfreeze()
    {
        if (GetComponent<AntRouteGrowControl>() is var growControl && growControl != null)
        {
            growControl.enabled = true;
        }
        if (GetComponent<AntKillAnimalControl>() is var killControl && killControl != null)
        {
            killControl.enabled = true;
        }
        if (GetComponent<AntRouteSpreadControl>() is var spreadControl && spreadControl != null)
        {
            spreadControl.enabled = true;
        }
    }

    public bool IsBiggerThan(AntNestHub otherHub)
    {
        return antNestSizeControl.Size > otherHub.antNestSizeControl.Size;
    }

    bool IsRootBranchAlive()
    {
        if (routeBranches.Count < 4)
            return false;

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

    public int CountAreaSize()
    {
        var list = new List<Vector3Int>();
        for (int i = 0; i < routeBranches.Count; i++)
        {
            Vector3Int[] positions = routeBranches[i].AllGridPosition();
            for (int e = 0; e < positions.Length; e++)
            {
                if (list.Contains(positions[e]))
                    continue;
                list.Add(positions[e]);
            }
        }
        return list.Count;
    }
    #endregion


    #region Take Damage
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

    public void TakeDamageFromOtherNest(float damageAmount)
    {
        OnOtherNestAttack?.Invoke(damageAmount);
    }

    public void RemoveBranch(int index)
    {
        routeBranches.RemoveAt(index);

        if (routeBranches.Count == 0)
            NestCompletelyDestroy();
    }

    public void MainNestHubDestroy()
    {
        if (!enabled) return;
        enabled = false;

        tilemapReference.Tilemap.SetTile(RootGridPosition, null);

        for (int i = routeBranches.Count - 1; i >= 0; i--)
        {
            if (routeBranches[i].IsEmpty)
            {
                routeBranches[i].OnDestroy();
                RemoveBranch(i);
                i++;
            }
            else
            {
                routeBranches[i].IsConnectedToNest = false;
            }
        }

        OnNestDestroy?.Invoke();
    }

    void NestCompletelyDestroy()
    {
        GridManager.ins.UnregisterAntNest(this);
        OnAntRouteBranchEmpty?.Invoke();
        Destroy(gameObject);
    }
    #endregion


    #region Color
    public void StartRevealColor(float stepInterval)
    {
        revealColorEffect.AddWaitingList(new EffectReference.EffectQueue {
            Parent = null,
            Position = transform.position,
            UseScaleTime = false,
        });

        StartCoroutine(C_RevealColor(stepInterval));
    }

    IEnumerator C_RevealColor(float stepInterval)
    {
        List<RouteColorIncrement> routeColorIncrements = new List<RouteColorIncrement>();

        for (int i = 0; i < routeBranches.Count; i++)
        {
            if (routeBranches[i].RootGridPosition == RootGridPosition)
            {
                routeColorIncrements.Add(new RouteColorIncrement(routeBranches[i], trueColor.Value));
            }
        }

        while (true)
        {
            bool allFinished = true;
            for (int i = 0; i < routeColorIncrements.Count; i++)
            {
                if (routeColorIncrements[i].Finished)
                    continue;

                if (!routeColorIncrements[i].Increment())
                    allFinished = false;
                
                T(routeColorIncrements, routeColorIncrements[i].CurrentPosition);
            }

            if (allFinished)
            {
                break;
            }
            
            yield return new WaitForSecondsRealtime(stepInterval);
        }
    }

    void T(List<RouteColorIncrement> routeColorIncrements, Vector3Int position)
    {
        for (int i = 0; i < routeBranches.Count; i++)
        {
            if (routeBranches[i].RootGridPosition == position)
            {
                bool overlap = false;
                for (int e = 0; e < routeColorIncrements.Count; e++)
                {
                    if (routeColorIncrements[e].line == routeBranches[i].line)
                    {
                        overlap = true;
                        break;
                    }
                }

                if (!overlap)
                    routeColorIncrements.Add(new RouteColorIncrement(routeBranches[i], trueColor.Value));
            }
        }
    }

    private class RouteColorIncrement
    {
        private Vector3Int[] _gridPositions;
        private int _gridPositionIndex;
        public Vector3Int CurrentPosition => _gridPositions[_gridPositionIndex];

        public bool Finished => _gridPositionIndex >= (line.positionCount - 1);

        public LineRenderer line;

        private Gradient _gradient;
        private GradientColorKey[] _colorKeys;
        private GradientAlphaKey[] _alphaKeys;

        private Color _oldColor;
        private Color _newColor;

        public RouteColorIncrement(AntRouteBranch branch, Color newColor)
        {
            _gridPositions = branch.AllGridPosition();
            _gridPositionIndex = 0;
            line = branch.line;

            _gradient = line.colorGradient;
            _gradient.mode = GradientMode.Fixed;
            GradientColorKey[] oldColorKeys = _gradient.colorKeys;
            _alphaKeys = _gradient.alphaKeys;

            GradientColorKey firstKey = oldColorKeys[0];
            firstKey.color = newColor;
            GradientColorKey lastKey = oldColorKeys[1];

            _colorKeys = new GradientColorKey[3];
            _colorKeys[0] = firstKey;
            _colorKeys[1] = new GradientColorKey(newColor, 0f);
            _colorKeys[2] = lastKey;

            _oldColor = line.startColor;
            _newColor = newColor;
        }

        public bool Increment()
        {
            if (Finished) return true;

            _gridPositionIndex += 1;

            float progress = (float)_gridPositionIndex / (float)(line.positionCount - 1);
            _colorKeys[1].time = progress;
            _gradient.SetKeys(_colorKeys, _alphaKeys);
            line.colorGradient = _gradient;

            return Finished;
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
                Gizmos.DrawSphere(tilemapReference.Tilemap.GetCellCenterWorld(routeBranches[i].RootGridPosition), 0.1f);
            }
        }
    }
    #endregion
}
