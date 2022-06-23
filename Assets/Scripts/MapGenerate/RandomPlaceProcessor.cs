using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGenerate
{
    [System.Flags]
    public enum PlaceItem
    {
        None = 0,
        Ground = 1,
        Animal = 2,
        Grass = 4,
    }

    [System.Serializable]
    public class RandomPlaceProcessor : IMapProcessor
    {
        [SerializeField]
        private PlaceItem placeItem;
        [SerializeField]
        [Range(0, 1)]
        private float coverage;

        public void Process(ref int[,,] map)
        {
            int layerCount = map.GetLength(0);
            int width = map.GetLength(1);
            int height = map.GetLength(2);

            for (int layerIndex = 0; layerIndex < layerCount; layerIndex ++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (Random.value <= coverage)
                        {
                            map[layerIndex, x, y] = (int) ((PlaceItem)map[layerIndex, x, y] | placeItem);
                        }
                    }
                }
            }
        }
    }
}