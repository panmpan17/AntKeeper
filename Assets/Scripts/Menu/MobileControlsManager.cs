using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MobileControlsManager : MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput;

    [SerializeField]
    private VirtualStick virtualStick;
    [SerializeField]
    private MoreEventButton dashButton;
    [SerializeField]
    private MoreEventButton interactButton;

    void Awake()
    {
        virtualStick.OnMovementAxisChange += OnMobileMovementPerformed;
        dashButton.onPointerDown.AddListener(OnDashClicked);
        interactButton.onPointerDown.AddListener(OnInteractStarted);
        interactButton.onPointerUp.AddListener(OnInteractEnded);
    }

    void OnMobileMovementPerformed(Vector2 axis)
    {
        playerInput.OnMobileMovementPerformed(axis);
    }

    void OnDashClicked(PointerEventData eventData)
    {
        playerInput.OnMobileDashButtonPerformed();
    }

    void OnInteractStarted(PointerEventData eventData)
    {
        playerInput.OnMobileInteractPerforemed();
    }

    void OnInteractEnded(PointerEventData eventData)
    {
        playerInput.OnMobileInteractCanceled();
    }
}
