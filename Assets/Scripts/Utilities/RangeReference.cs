using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class RangeReference : ScriptableObject
{
    public float Min;
    public float Max;

    public float PickRandomNumber()
    {
        return Random.Range(Min, Max);
    }

    public float Clamp(float number)
    {
        return Mathf.Clamp(number, Min, Max);
    }
}
