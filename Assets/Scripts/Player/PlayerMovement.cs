using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum Facing { Left, Right, Up, Down }

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float walkSpeed;

    private Facing _facing = Facing.Right;
    public Facing Facing => _facing;

    public event System.Action<Facing> OnFacingChange;
    public event System.Action OnPositionChange;

    private PlayerInput _playerInput;
    private Rigidbody2D _rigidbody;


    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    
    void Start()
    {
        ApplyFacing(_facing);
        OnPositionChange?.Invoke();
    }

    void FixedUpdate()
    {
        Vector2 axis = _playerInput.MovementAxis;

        if (axis.sqrMagnitude > 0.01f)
        {
            _rigidbody.velocity = axis * walkSpeed;


            Facing newFacing;
            if (Mathf.Abs(axis.x) > Mathf.Abs(axis.y))
                newFacing = axis.x > 0 ? Facing.Right : Facing.Left;
            else
                newFacing = axis.y > 0 ? Facing.Up : Facing.Down;
            
            if (newFacing != _facing)
                ApplyFacing(newFacing);

            OnPositionChange?.Invoke();
        }
        else
            _rigidbody.velocity = Vector3.zero;
    }

    void ApplyFacing(Facing newFacing)
    {
        OnFacingChange?.Invoke(newFacing);
        _facing = newFacing;
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
