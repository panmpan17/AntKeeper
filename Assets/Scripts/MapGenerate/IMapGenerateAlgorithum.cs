using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMapGenerateAlgorithum
{
    int[,] GenerateMap(int width, int height);
}
