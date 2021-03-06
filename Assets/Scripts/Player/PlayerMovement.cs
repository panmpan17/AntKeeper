using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum Facing { Left, Right, Up, Down }

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private FloatReference stuckWalkSpeedMultiplier;

    [Header("Dash")]
    [SerializeField]
    private bool dashUseIgnoreLayer;
    [SerializeField]
    [Layer]
    private int dashIgnoreLayer;
    [SerializeField]
    private DashInfo dashInfo;
    [SerializeField]
    [ShortTimer]
    private Timer dashColddownTimer;
    private DashInfo _runningDash;

    // [Header("Sound")]
    // [SerializeField]
    // private AudioPreset footstepPreset;
    // [SerializeField]
    // private Timer footstepSoundTimer;
    // [SerializeField]
    // private AudioPreset dashSound;

    private Facing _facing = Facing.Right;
    public Facing Facing => _facing;

    public event System.Action OnFacingChange;
    public event System.Action OnPositionChange;

    public event System.Action OnDashStarted;
    public event System.Action OnDashPerformed;
    public event System.Action OnDashEnded;

    public event System.Action OnWalkStarted;
    public event System.Action OnWalkEnded;

    public event System.Action OnStuckAntRoute;
    public event System.Action OnExitAntRoute;

    private PlayerInput _playerInput;
    private Rigidbody2D _rigidbody;

    private bool _stuckInAntRoute;

    private enum MovementState
    {
        Idle,
        Walk,
        Dash,
    }
    private MovementState _movementState;

    public bool IsDashing {
        get => _movementState == MovementState.Dash;
        set {
            if (value)
                _movementState = MovementState.Dash;
            else
            {
                if (_playerInput.MovementAxis.sqrMagnitude > 0.01f)
                    _movementState = MovementState.Walk;
                else
                {
                    _movementState = MovementState.Idle;
                    OnWalkEnded?.Invoke();
                }
            }
        }
    }

    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.OnDashPerformedEvent += Dash;

        _rigidbody = GetComponent<Rigidbody2D>();
    }
    
    void Start()
    {
        ApplyFacing(_facing);
        OnPositionChange?.Invoke();
    }

    void FixedUpdate()
    {
        if (HandleDashUpdate()) return;

        Vector2 axis = _playerInput.MovementAxis;

        if (axis.sqrMagnitude > 0.01f)
        {
            Vector2 velocity = axis * walkSpeed;
            if (_stuckInAntRoute)
                velocity *= stuckWalkSpeedMultiplier.Value;

            _rigidbody.velocity = velocity;


            Facing newFacing;
            if (Mathf.Abs(axis.x) > Mathf.Abs(axis.y))
                newFacing = axis.x > 0 ? Facing.Right : Facing.Left;
            else
                newFacing = axis.y > 0 ? Facing.Up : Facing.Down;
            
            if (newFacing != _facing)
                ApplyFacing(newFacing);

            OnPositionChange?.Invoke();

            if (_movementState == MovementState.Idle)
            {
                _movementState = MovementState.Walk;
                OnWalkStarted?.Invoke();
            }
        }
        else
        {
            _rigidbody.velocity = Vector3.zero;

            if (_movementState == MovementState.Walk)
            {
                _movementState = MovementState.Idle;
                OnWalkEnded?.Invoke();
            }
        }
    }


    bool HandleDashUpdate()
    {
        if (IsDashing)
        {
            if (_runningDash.Delay.Running)
            {
                if (!_runningDash.Delay.FixedUpdateEnd)
                    return true;

                _runningDash.Delay.Running = false;
                OnDashPerformed?.Invoke();
            }

            if (_runningDash.Timer.FixedUpdateEnd)
            {
                IsDashing = false;
                _runningDash.InvokeEndEvent();

                if (dashUseIgnoreLayer)
                    Physics2D.IgnoreLayerCollision(gameObject.layer, dashIgnoreLayer, false);

                OnDashEnded?.Invoke();

                if (_movementState == MovementState.Idle)
                    OnWalkEnded?.Invoke();
                else
                    OnWalkStarted?.Invoke();
            }
            else
            {
                _rigidbody.velocity = _runningDash.DashForceWithDirection * _runningDash.Curve.Value.Evaluate(_runningDash.Timer.Progress);
            }

            OnPositionChange?.Invoke();
            return true;
        }

        if (dashColddownTimer.Running)
        {
            if (dashColddownTimer.FixedUpdateEnd)
            {
                dashColddownTimer.Running = false;
            }
        }
        return false;
    }

    void ApplyFacing(Facing newFacing)
    {
        _facing = newFacing;
        OnFacingChange?.Invoke();
    }

    void Dash()
    {
        if (IsDashing) return;
        if (dashColddownTimer.Running) return;

        dashColddownTimer.Reset();

        IsDashing = true;
        _runningDash = dashInfo;
        _runningDash.Delay.Running = _runningDash.Delay.TargetTime > 0;


        // Set dash direction
        if (_playerInput.MovementAxis.sqrMagnitude > 0.01f)
        {
            _runningDash.DashForceWithDirection = dashInfo.Force * _playerInput.MovementAxis.normalized;
        }
        else
        {
            switch (_facing)
            {
                case Facing.Up:
                    _runningDash.DashForceWithDirection = new Vector2(0, dashInfo.Force);
                    break;
                case Facing.Down:
                    _runningDash.DashForceWithDirection = new Vector2(0, -dashInfo.Force);
                    break;
                case Facing.Right:
                    _runningDash.DashForceWithDirection = new Vector2(dashInfo.Force, 0);
                    break;
                case Facing.Left:
                    _runningDash.DashForceWithDirection = new Vector2(-dashInfo.Force, 0);
                    break;
            }
        }

        if (dashUseIgnoreLayer)
            Physics2D.IgnoreLayerCollision(gameObject.layer, dashIgnoreLayer, true);

        OnDashStarted?.Invoke();
    }

    public void Freeze()
    {
        throw new System.NotImplementedException();
    }
    public void Unfreeze()
    {
        throw new System.NotImplementedException();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("AntRoute"))
        {
            _rigidbody.velocity = Vector3.zero;
            _stuckInAntRoute = true;
            OnStuckAntRoute?.Invoke();
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("AntRoute"))
        {
            _stuckInAntRoute = false;
            OnExitAntRoute?.Invoke();
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerMovement))]
public class PlayerMovementEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        if (GUILayout.Button("Face Up")) TestFacing(Facing.Up);
        EditorGUILayout.Space();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Face Left"))  TestFacing(Facing.Left);
        EditorGUILayout.Space();
        if (GUILayout.Button("Face Right"))  TestFacing(Facing.Right);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        if (GUILayout.Button("Face Down")) TestFacing(Facing.Down);
        EditorGUILayout.Space();
        GUILayout.EndHorizontal();
    }

    void TestFacing(Facing facing)
    {
        var playerMovement = (PlayerMovement)target;
        playerMovement.GetComponent<PlayerAnimation>().ChangeFacingAnimation(facing);
        playerMovement.GetComponent<PlayerBehaviour>().ChangeHoldItemPosition(facing);
    }
}
#endif
