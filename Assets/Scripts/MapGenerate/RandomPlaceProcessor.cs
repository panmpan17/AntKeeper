using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGenerate
{
    [System.Serializable]
    public class RandomPlaceProcessor : IMapProcessor
    {
        [SerializeField]
        [Range(0, 1)]
        public float coverage;

        public void Process(ref int[,] map)
        {
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
}