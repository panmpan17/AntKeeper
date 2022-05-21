using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

public class ScreenShotAimControl : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera followPlayerVCamera;

    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private GameObject shutter;
    [SerializeField]
    private float shutterTime = 0.2f;

    [SerializeField]
    private float minMoveSpeed;
    [SerializeField]
    private float maxMoveSpeed;
    private float moveSpeed {
        get {
            return Mathf.Lerp(minMoveSpeed, maxMoveSpeed, Mathf.InverseLerp(minZoom, maxZoom, _virtualCamera.m_Lens.OrthographicSize));
        }
    }

    [SerializeField]
    private float minZoom;
    [SerializeField]
    private float maxZoom;


    private CinemachineVirtualCamera _virtualCamera;
    private InputScheme _inputScheme;

    private Vector2 _movmentAxis;
    private float _zoomAxis;
    private bool _active;
    private bool _takingShot;

    void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _inputScheme = new InputScheme();

        _inputScheme.ScreenShot.Activate.performed += HandleActivePerformed;
        _inputScheme.ScreenShot.Cancel.performed += HandleCancelPerformed;

        _inputScheme.ScreenShot.Movement.performed += HandleMovementChanged;
        _inputScheme.ScreenShot.Movement.canceled += HandleMovementChanged;

        _inputScheme.ScreenShot.Zoom.performed += HandleZoomChanged;
        _inputScheme.ScreenShot.Zoom.canceled += HandleZoomChanged;

        _inputScheme.ScreenShot.Take.performed += HandleTakePerformed;

        _inputScheme.ScreenShot.Proceed.performed += HandleProceedPerformed;

        canvas.enabled = false;
    }

    void OnEnable()
    {
        _inputScheme.Enable();
    }

    void OnDisable()
    {
        _inputScheme.Disable();
    }

    void Update()
    {
        if (_active)
        {
            transform.position += (Vector3)_movmentAxis * moveSpeed  * Time.fixedDeltaTime;

            if (_zoomAxis != 0)
            {
                float orthographicSize = _virtualCamera.m_Lens.OrthographicSize;
                orthographicSize = Mathf.Clamp(orthographicSize + _zoomAxis * Time.unscaledDeltaTime, minZoom, maxZoom);
                _virtualCamera.m_Lens.OrthographicSize = orthographicSize;
            }
        }
    }

    
    #region Input
    void HandleActivePerformed(CallbackContext callbackContext)
    {
        if (_active)
            return;
        _active = true;
        _movmentAxis = Vector2.zero;
        _zoomAxis = 0;

        Time.timeScale = 0;

        GameManager.ins.Player.Input.LockForScreenShot();
        canvas.enabled = true;
        if (HUDManager.ins != null) HUDManager.ins.enabled = false;

        transform.position = followPlayerVCamera.transform.position;
        _virtualCamera.m_Lens.OrthographicSize = followPlayerVCamera.m_Lens.OrthographicSize;
        CameraManager.ins.SwitchCamera(CameraManager.CameraState.ScreenShot);
    }

    void HandleCancelPerformed(CallbackContext callbackContext)
    {
        if (!_active)
            return;
        _active = false;
        CameraManager.ins.SwitchCamera(CameraManager.CameraState.FollowPlayer);
        Time.timeScale = 1;

        GameManager.ins.Player.Input.UnlockForScreenShot();
        canvas.enabled = false;
        if (HUDManager.ins != null) HUDManager.ins.enabled = true;
    }


    void HandleMovementChanged(CallbackContext callbackContext)
    {
        _movmentAxis = callbackContext.ReadValue<Vector2>();
    }

    void HandleZoomChanged(CallbackContext callbackContext)
    {
        _zoomAxis = callbackContext.ReadValue<float>();
    }

    void HandleTakePerformed(CallbackContext callbackContext)
    {
        if (!_active)
            return;
        if (_takingShot)
            return;

        StartCoroutine(C_TakeShot());
    }

    void HandleProceedPerformed(CallbackContext callbackContext)
    {
        if (!_active)
            return;

        StartCoroutine(ProceedOneFrame());
    }
    #endregion

    IEnumerator C_TakeShot()
    {
        _takingShot = true;

        yield return null;
        shutter.SetActive(true);
        yield return new WaitForSecondsRealtime(shutterTime);
        canvas.enabled = false;
        shutter.SetActive(false);
        yield return null;
        ScreenShotTaker.TakeScreenShot();
        yield return null;
        canvas.enabled = true;

        _takingShot = false;
    }

    IEnumerator ProceedOneFrame()
    {
        Time.timeScale = 1;
        yield return null;
        yield return null;
        Time.timeScale = 0;
    }
}
