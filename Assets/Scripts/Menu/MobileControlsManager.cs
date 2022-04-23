using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    void OnDashClicked()
    {
        playerInput.OnMobileDashButtonPerformed();
    }

    void OnInteractStarted()
    {
        playerInput.OnMobileInteractPerforemed();
    }

    void OnInteractEnded()
    {
        playerInput.OnMobileInteractCanceled();
    }
}
