using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MapGenerate
{
    [System.Serializable]
    public class PerlinNoiseProccessor : IMapProcessor
    {
        [SerializeField]
        private PlaceItem placeItem;
        [SerializeField]
        [Range(0, 1)]
        private float stepMin;
        [SerializeField]
        [Range(0, 1)]
        private float stepMax;
        [SerializeField]
        private bool randomOffset;
        [SerializeField]
        private Vector2 offset;
        [SerializeField]
        [Range(0, 40)]
        private float scale;

        public void Process(ref int[,,] map)
        {
            if (randomOffset)
                offset = new Vector2(Random.Range(-100, 100f), Random.Range(-100, 100f));

            int layerCount = map.GetLength(0);
            int width = map.GetLength(1);
            int height = map.GetLength(2);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float value = Mathf.PerlinNoise(
                        offset.x + (((float)x) / width * scale),
                        offset.y + (((float)y) / height * scale));
                    
                    if (value < stepMin)
                        continue;
                    if (value > stepMax)
                        value = stepMax;
                    
                    float heightPercentage = (value - stepMin) / (stepMax - stepMin);
                    int toLayer = Mathf.CeilToInt(heightPercentage * layerCount);

                    for (int i = 0; i < toLayer; i++)
                    {
                        map[i, x, y] = (int)((PlaceItem)map[i, x, y] | placeItem);
                    }
                }
            }
        }


#if UNITY_EDITOR
        public void OnValidate()
        {
            if (stepMin > stepMax)
            {
                stepMax = stepMin;
            }
        }
#endif
    }
}