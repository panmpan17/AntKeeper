using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("Face settings")]
    [SerializeField]
    private FacingSetting faceUpSetting;
    [SerializeField]
    private FacingSetting faceDownSetting;
    [SerializeField]
    private FacingSetting faceRightSetting;
    [SerializeField]
    private FacingSetting faceLeftSetting;

    [Header("Other")]
    [SerializeField]
    private AbstractHoldItem holdItem;
    [SerializeField]
    private Transform selectedGridIndicator;
    public Vector3Int SelectedGridPosition { get; private set; }
    public Vector3 SelectedGridCenterPosition { get; private set; }

    private PlayerMovement _movement;
    private PlayerInput _input;

    void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _movement.OnFacingChange += ChangeHoldItemPosition;
        _movement.OnPositionChange += ChangeSelectedGrid;

        _input = GetComponent<PlayerInput>();
        _input.OnInteractPerformedEvent += OnIneractPerformed;
        _input.OnInteractCanceledEvent += OnIneractCanceled;

        holdItem.Setup(this);
    }

    void OnIneractPerformed()
    {
        if (holdItem != null)
        {
            holdItem.OnInteractStart();
        }
        else
        {
            if (GridManager.ins.TryFindGroundInteractive(SelectedGridPosition, out AbstractGroundInteractive groundInteractive))
            {
                groundInteractive.OnEmptyHandInteract(this);
                return;
            }
        }
    }

    void OnIneractCanceled()
    {
        if (holdItem != null)
        {
            holdItem.OnInteractEnd();
        }
    }

    public void ChangeHoldItemPosition(Facing newFacing)
    {
        switch (newFacing)
        {
            case Facing.Up:
                if (holdItem != null)
                    holdItem.transform.localPosition = faceUpSetting.HoldItemPosition;
                break;
            case Facing.Down:
                if (holdItem != null)
                    holdItem.transform.localPosition = faceDownSetting.HoldItemPosition;
                break;
            case Facing.Right:
                if (holdItem != null)
                    holdItem.transform.localPosition = faceRightSetting.HoldItemPosition;
                break;
            case Facing.Left:
                if (holdItem != null)
                    holdItem.transform.localPosition = faceLeftSetting.HoldItemPosition;
                break;
        }
    }

    void ChangeSelectedGrid()
    {
        switch (_movement.Facing)
        {
            case Facing.Up:
                RaycasyCell(faceUpSetting.SelectGridPoint.position);
                break;
            case Facing.Down:
                RaycasyCell(faceDownSetting.SelectGridPoint.position);
                break;
            case Facing.Right:
                RaycasyCell(faceRightSetting.SelectGridPoint.position);
                break;
            case Facing.Left:
                RaycasyCell(faceLeftSetting.SelectGridPoint.position);
                break;
        }
    }

    void RaycasyCell(Vector3 position)
    {
        Grid grid = GridManager.ins.Grid;
        Vector3Int cellPos = grid.WorldToCell(position);
        // if (tilemap.HasTile(cellPos))
        // {

        SelectedGridPosition = cellPos;
        SelectedGridCenterPosition = grid.GetCellCenterWorld(cellPos);
        selectedGridIndicator.transform.position = SelectedGridCenterPosition;
        // selectedGridIndicator.gameObject.SetActive(true);
        // }
        // else
        // {
        //     gridIndicate.gameObject.SetActive(false);
        // }
    }

    public void SetHandItem(AbstractHoldItem item)
    {
        holdItem = item;
        holdItem.transform.SetParent(transform, false);
        ChangeHoldItemPosition(_movement.Facing);
    }

    public void ClearHandItem()
    {
        holdItem = null;
    }


    [System.Serializable]
    public struct FacingSetting
    {
        public Vector3 HoldItemPosition;
        public Transform SelectGridPoint;
    }
}
