using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class RandomPlaceProcessor : IMapProcessor
{
    [SerializeField]
    private bool randomSeed = true;
    [SerializeField]
    private int seed;
    [SerializeField]
    [Range(0, 1)]
    public float coverage;

    public void Process(ref int[,] map)
    {
        if (randomSeed)
            seed = Random.Range(0, 1000000);
        Random.InitState(seed);

        int width = map.GetLength(0);
        int height = map.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (Random.value <= coverage)
                    map[x, y] = 1;
                else
                    map[x, y] = 0;
            }
        }
    }
}
