using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


[System.Serializable]
public struct DashInfo
{
    public float Force;
    [ShortTimer]
    public Timer Timer;
    public AnimationCurveVariable Curve;

    public event System.Action EndEvent;

    [System.NonSerialized]
    public Vector2 DashForceWithDirection;

    public void InvokeEndEvent()
    {
        EndEvent?.Invoke();
    }
}