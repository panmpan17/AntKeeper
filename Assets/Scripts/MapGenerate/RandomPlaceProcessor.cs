using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

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
        [SerializeField]
        private ValueWithEnable<int> specificLayer;

        private int _width;
        private int _height;

        public void Process(ref int[,,] map)
        {
            _width = map.GetLength(1);
            _height = map.GetLength(2);

            if (specificLayer.Enable)
            {
                PlaceTile(ref map, specificLayer.Value);
                return;
            }

            int layerCount = map.GetLength(0);

            for (int layerIndex = 0; layerIndex < layerCount; layerIndex ++)
            {
                PlaceTile(ref map, layerIndex);
            }
        }

        void PlaceTile(ref int[,,] map, int layerIndex)
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    if (Random.value <= coverage)
                    {
                        map[layerIndex, x, y] = (int)((PlaceItem)map[layerIndex, x, y] | placeItem);
                    }
                }
            }
        }
    }
}