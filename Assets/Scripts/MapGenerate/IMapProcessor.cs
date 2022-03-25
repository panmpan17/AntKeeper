using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMapProcessor
{
    void Process(ref int[,] map);
}
