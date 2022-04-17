using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public const string Tag = "Player";

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

    [SerializeField]
    [SortingLayer]
    private int sortingLayerID;
    [SerializeField]
    private int sortingOrder;

    private PlayerInput _input;
    private PlayerMovement _movement;

    public PlayerInput Input => _input;
    public PlayerMovement Movement => _movement;

    void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _input.OnInteractPerformedEvent += OnIneractPerformed;
        _input.OnInteractCanceledEvent += OnIneractCanceled;

        _movement = GetComponent<PlayerMovement>();
        _movement.OnFacingChange += ChangeHoldItemPosition;
        _movement.OnDashPerformed += HandleDashPerformed;

        if (holdItem != null)
            holdItem.Setup(this);
    }

    void FixedUpdate()
    {
        ChangeSelectedGrid();
    }


    #region Player Input Event
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
    #endregion


    #region Player Movement Event
    public void ChangeHoldItemPosition()
    {
        ChangeHoldItemPosition(_movement.Facing);
    }
    public void ChangeHoldItemPosition(Facing newFacing)
    {
        switch (newFacing)
        {
            case Facing.Up:
                if (holdItem)
                    holdItem.transform.localPosition = faceUpSetting.HoldItemPosition;
                break;
            case Facing.Down:
                if (holdItem)
                    holdItem.transform.localPosition = faceDownSetting.HoldItemPosition;
                break;
            case Facing.Right:
                if (holdItem)
                    holdItem.transform.localPosition = faceRightSetting.HoldItemPosition;
                break;
            case Facing.Left:
                if (holdItem)
                    holdItem.transform.localPosition = faceLeftSetting.HoldItemPosition;
                break;
        }

        if (holdItem)
            holdItem.OnFacingChanged();
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

    void HandleDashPerformed()
    {
        if (holdItem != null)
        {
            holdItem.OnDash();
        }
    }
    #endregion

    void RaycasyCell(Vector3 position)
    {
        if (GridManager.ins.RaycastCell(position, out Vector3Int cellPosition, out Vector3 centerPosition))
        {
            SelectedGridPosition = cellPosition;
            SelectedGridCenterPosition = centerPosition;
            selectedGridIndicator.transform.position = centerPosition;

            selectedGridIndicator.gameObject.SetActive(true);
        }
        else
        {
            selectedGridIndicator.gameObject.SetActive(false);
        }
    }

    public void SetHandItem(AbstractHoldItem item)
    {
        item.ChangeRendererSorting(sortingLayerID, sortingOrder);

        holdItem = item;
        holdItem.transform.SetParent(transform, false);
        holdItem.Setup(this);
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
