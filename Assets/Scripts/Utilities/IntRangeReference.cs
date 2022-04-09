using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class IntRangeReference : ScriptableObject
{
    public int Min;
    public int Max;

    public int PickRandomNumber(bool maxInclusive=false)
    {
        return maxInclusive ? Random.Range(Min, Max + 1) : Random.Range(Min, Max);
    }
}
