using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGenerate
{
    [System.Serializable]
    public class PlaceCircleProcessor : IMapProcessor
    {
        [SerializeField]
        [Range(1, 100)]
        private int circleMinSize;
        [SerializeField]
        [Range(1, 100)]
        private int circleMaxSize;

        public void Process(ref int[,] map)
        {
            int size = Random.Range(circleMinSize, circleMaxSize);
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            Vector2 min = new Vector2(size, size);
            Vector2 max = new Vector2(width - size - 1, height - size - 1);

            Vector2 center = new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2 position = new Vector2(x, y);

                    if ((center - position).sqrMagnitude < size * size)
                        map[x, y] = 1;
                }
            }
        }
    }
}