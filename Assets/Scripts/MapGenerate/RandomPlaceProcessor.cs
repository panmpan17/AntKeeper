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

        public void Process(ref int[,] map)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (Random.value <= coverage)
                    {
                        map[x, y] = (int) ((PlaceItem)map[x, y] | placeItem);
                    }
                }
            }
        }
    }
}