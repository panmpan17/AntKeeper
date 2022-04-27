using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class PlayerInputEvent : ScriptableObject
{
    public System.Action<Vector2> OnMovementAxisChange;
    public System.Action OnDash;
    public System.Action OnInteractPerformed;
    public System.Action OnInteractCanceled;
}