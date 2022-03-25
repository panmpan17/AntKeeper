using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager ins;

    [SerializeField]
    private Grid grid;
    public Grid Grid => grid;

    private List<AbstractGroundInteractive> _groundInteractives;

    void Awake()
    {
        ins = this;
        _groundInteractives = new List<AbstractGroundInteractive>();
    }

    public void RegisterGroundInteractive(AbstractGroundInteractive groundInteractive, out Vector3Int gridPosition)
    {
        _groundInteractives.Add(groundInteractive);
        gridPosition = grid.WorldToCell(groundInteractive.transform.position);
    }

    public bool TryFindGroundInteractive(Vector3Int gridPosition, out AbstractGroundInteractive groundInteractve)
    {
        for (int i = 0; i < _groundInteractives.Count; i++)
        {
            if (gridPosition == _groundInteractives[i].GridPosition)
            {
                groundInteractve = _groundInteractives[i];
                return true;
            }
        }

        groundInteractve = null;
        return false;
    }
}
