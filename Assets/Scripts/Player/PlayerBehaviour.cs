using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


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
    private FillBarControl progressBar;
    public FillBarControl ProgressBar => progressBar;
    [SerializeField]
    private AbstractHoldItem holdItem;
    public AbstractHoldItem HoldItem => holdItem;

    [SerializeField]
    private Transform holdItemParent;
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

    public bool IsHolding => holdItem != null;

    public event System.Action OnHoldItemChanged;

    void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _input.OnInteractPerformedEvent += OnIneractPerformed;
        _input.OnInteractCanceledEvent += OnIneractCanceled;

        _movement = GetComponent<PlayerMovement>();
        _movement.OnFacingChange += ChangeHoldItemPosition;
        _movement.OnDashStarted += HandleDashPerformed;

        selectedGridIndicator.SetParent(null);

        progressBar.gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        ChangeSelectedGrid();

        if (IsHolding)
            ChangeHoldItemPosition(_movement.Facing);
    }


    #region Player Input Event
    void OnIneractPerformed()
    {
        if (holdItem != null)
        {
            if (holdItem.CanPlaceDownToGrounInteractive(out AbstractGroundInteractive groundInteractive))
            {
                var _holdItem = holdItem;
                holdItem = null;
                _holdItem.OnPlaceToGround(groundInteractive);
                OnHoldItemChanged?.Invoke();
                return;
            }

            holdItem.OnInteractStart();
        }
        else
        {
            if (GridManager.ins.TryFindGroundInteractive(SelectedGridPosition, out AbstractGroundInteractive groundInteractive))
            {
                if (groundInteractive.OnEmptyHandInteract(this))
                {
                    OnHoldItemChanged?.Invoke();
                }
                return;
            }
        }
    }

    void OnIneractCanceled()
    {
        holdItem?.OnInteractEnd();
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

        holdItem?.OnFacingChanged();
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
        holdItem?.OnDash();
    }
    #endregion

    void RaycasyCell(Vector3 position)
    {
        if (GridManager.ins.RaycastCell(position, out Vector3Int cellPosition, out Vector3 centerPosition))
        {
            if (cellPosition != SelectedGridPosition)
            {
                SelectedGridPosition = cellPosition;
                SelectedGridCenterPosition = centerPosition;
                selectedGridIndicator.position = centerPosition;

                selectedGridIndicator.gameObject.SetActive(true);

                holdItem?.OnSelectedGridChanged();
            }
        }
        else
        {
            selectedGridIndicator.gameObject.SetActive(false);
        }
    }

    public void SetHandItem(AbstractHoldItem item)
    {
        if (holdItem != item)
        {
            holdItem = item;
            holdItem.transform.SetParent(holdItemParent, false);
            holdItem.OnPickUpByHand(this);
            holdItem.ChangeRendererSorting(sortingLayerID, sortingOrder);
        }

        ChangeHoldItemPosition(_movement.Facing);
    }


    [System.Serializable]
    public struct FacingSetting
    {
        public Vector3 HoldItemPosition;
        public Transform SelectGridPoint;
    }
}
