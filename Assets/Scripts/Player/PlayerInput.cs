using System;
using System.Collections;
using UnityEngine;
using MPack;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private PlayerInputEvent inputEvent;

    [SerializeField]
    private EventReference openPauseEvent;

    public Vector2 MovementAxis { get; private set; }

    public event Action OnMovementPerformedEvent;
    public event Action OnInteractPerformedEvent;
    public event Action OnInteractCanceledEvent;
    public event Action OnDashPerformedEvent;

    private InputScheme _inputScheme;
    private bool _screenShotLock = false;
    private bool _interactCancelPerformed = false;
    private bool _oneFrameSkip = false;

    void Awake()
    {
        _inputScheme = new InputScheme();

        _inputScheme.Game.Movement.performed += HandleMovementPerformed;
        _inputScheme.Game.Movement.canceled += HandleMovementPerformed;

        _inputScheme.Game.Interact.performed += HandleInteractPerformed;
        _inputScheme.Game.Interact.canceled += HandleInteractCanceled;

        _inputScheme.Game.Dash.performed += HandleDashPerformed;

        _inputScheme.Game.Pause.performed += HandlePausePreformed;

        inputEvent.OnMovementAxisChange += OnMobileMovementPerformed;
        inputEvent.OnDash += OnMobileDashButtonPerformed;
        inputEvent.OnInteractPerformed += OnMobileInteractPerforemed;
        inputEvent.OnInteractCanceled += OnMobileInteractCanceled;
        inputEvent.OnPause += OnPausePrssed;
    }


    #region Input Action handle
    void HandleMovementPerformed(CallbackContext callbackContext)
    {
        MovementAxis = callbackContext.ReadValue<Vector2>();
        OnMovementPerformedEvent?.Invoke();
    }

    void HandleInteractPerformed(CallbackContext callbackContext)
    {
        OnInteractPerformedEvent?.Invoke();
    }

    void HandleInteractCanceled(CallbackContext callbackContext)
    {
        if (_screenShotLock)
            _interactCancelPerformed = true;
        else
            OnInteractCanceledEvent?.Invoke();
    }

    void HandleDashPerformed(CallbackContext callbackContext)
    {
        if (_oneFrameSkip) return;

        if (!_interactCancelPerformed)
            OnDashPerformedEvent?.Invoke();
    }

    void HandlePausePreformed(CallbackContext callbackContext)
    {
        if (!_screenShotLock)
            openPauseEvent.Invoke();
    }
    #endregion


    #region Mobile UI button handle
    public void OnMobileMovementPerformed(Vector2 axis)
    {
        if (!enabled) return;
        MovementAxis = axis;
        OnMovementPerformedEvent?.Invoke();
    }

    public void OnMobileDashButtonPerformed()
    {
        if (!enabled) return;
        OnDashPerformedEvent?.Invoke();
    }

    public void OnMobileInteractPerforemed()
    {
        if (!enabled) return;
        OnInteractPerformedEvent?.Invoke();
    }

    public void OnMobileInteractCanceled()
    {
        if (!enabled) return;
        OnInteractCanceledEvent?.Invoke();
    }

    public void OnPausePrssed()
    {
        if (!enabled) return;
        openPauseEvent.Invoke();
    }
    #endregion

    public void LockForScreenShot() => _screenShotLock = true;
    public void UnlockForScreenShot()
    {
        _screenShotLock = false;
        if (_interactCancelPerformed)
            OnInteractCanceledEvent?.Invoke();

        _interactCancelPerformed = false;
        StartCoroutine(C_OneFrameSkip());
    }

    IEnumerator C_OneFrameSkip()
    {
        _oneFrameSkip = true;
        yield return null;
        _oneFrameSkip = false;
    }

    void OnEnable()
    {
        _inputScheme.Enable();
    }
    void OnDisable()
    {
        _inputScheme.Disable();
    }
}
