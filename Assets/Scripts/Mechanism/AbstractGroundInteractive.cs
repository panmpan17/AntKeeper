using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractGroundInteractive : MonoBehaviour
{
    public Vector3Int GridPosition { get; private set; }

    protected virtual void Start()
    {
        GridManager.ins.RegisterGroundInteractive(this, out Vector3Int gridPosition);
        GridPosition = gridPosition;
    }

    public abstract bool OnEmptyHandInteract(PlayerBehaviour playerBehaviour);
    // public abstract bool OnHoldItemInteract(AbstractHoldItem item);
    public abstract bool CanItemPlaceDown(AbstractHoldItem item);
    public abstract void PlaceDownItem(AbstractHoldItem item);

    #if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Grid grid = FindObjectOfType<Grid>();
        transform.position = grid.GetCellCenterWorld(grid.WorldToCell(transform.position));
    }
    #endif
}
