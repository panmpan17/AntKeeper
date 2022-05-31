using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class StatisticProvider : ScriptableObject
{
    public System.Func<GameStatic> Get;
}
