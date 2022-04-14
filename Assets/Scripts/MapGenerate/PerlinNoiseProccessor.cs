using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MapGenerate
{
    [System.Serializable]
    public class PerlinNoiseProccessor : IMapProcessor
    {
        [SerializeField]
        [Range(0, 1)]
        private float step;
        [SerializeField]
        private float x;
        [SerializeField]
        private float y;
        [SerializeField]
        [Range(0, 100)]
        private float scale;

        public void Process(ref int[,] map)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float value = Mathf.PerlinNoise(
                        this.x + (((float)x) / width * scale),
                        this.y + (((float)y) / height * scale));

                    if (value >= step)
                        map[x, y] = 1;
                    else
                        map[x, y] = 0;
                }
            }
        }
    }
}