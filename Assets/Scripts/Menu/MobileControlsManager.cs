using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MobileControlsManager : MonoBehaviour
{
    // [SerializeField]
    // private PlayerInput playerInput;

    [SerializeField]
    private VirtualStick virtualStick;
    [SerializeField]
    private MoreEventButton dashButton;
    [SerializeField]
    private MoreEventButton interactButton;

    [SerializeField]
    private PlayerInputEvent inputEvent;

    void Awake()
    {
        virtualStick.OnMovementAxisChange += OnMobileMovementPerformed;
        dashButton.onPointerDown.AddListener(OnDashClicked);
        interactButton.onPointerDown.AddListener(OnInteractStarted);
        interactButton.onPointerUp.AddListener(OnInteractEnded);
    }

    void OnMobileMovementPerformed(Vector2 axis)
    {
        inputEvent.OnMovementAxisChange.Invoke(axis);
    }

    void OnDashClicked(PointerEventData eventData)
    {
        inputEvent.OnDash.Invoke();
    }

    void OnInteractStarted(PointerEventData eventData)
    {
        inputEvent.OnInteractPerformed.Invoke();
    }

    void OnInteractEnded(PointerEventData eventData)
    {
        inputEvent.OnInteractCanceled.Invoke();
    }
}
