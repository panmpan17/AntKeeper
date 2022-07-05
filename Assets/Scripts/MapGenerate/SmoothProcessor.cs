using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MapGenerate
{
    [System.Serializable]
    public class SmoothProcessor
    {
        static Vector2Int[] EightDirections = new Vector2Int[] {
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1),
            new Vector2Int(0, -1),
            new Vector2Int(0, 1),
            new Vector2Int(1, -1),
            new Vector2Int(1, 0),
            new Vector2Int(1, 1),
        };

        static Vector2Int[] FourDirections = new Vector2Int[] {
            new Vector2Int(-1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
        };

        [SerializeField]
        [Range(1, 10)]
        private int smoothCount;
        [SerializeField]
        [Range(1, 8)]
        private int keepAliveNeighborNeedCount;
        [SerializeField]
        [Range(1, 8)]
        private int becomeAliveNeighborNeedCount;

        int _width;
        int _height;

        public void Process(ref int[,,] map)
        {
            int layerCount = map.GetLength(0);
            _width = map.GetLength(1);
            _height = map.GetLength(2);

            for (int i = 0; i < layerCount; i++)
                for (int e = 0; e < smoothCount; e++)
                    Smooth(ref map, i);
        }

        void Smooth(ref int[,,] map, int layerIndex)
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    int neighborCount = CountEightNeighbor(map, layerIndex, x, y);

                    if (map[layerIndex, x, y] > 0)
                    {
                        // Is alive, check is there enough neighbor to keep alive
                        if (neighborCount < keepAliveNeighborNeedCount)
                            map[layerIndex, x, y] = 0;
                    }
                    else
                    {
                        // Is dead, check is there enough neighbor to become alive
                        if (neighborCount >= becomeAliveNeighborNeedCount)
                            map[layerIndex, x, y] = 1;
                    }
                }
            }
        }

        public int CountEightNeighbor(int[,,] map, int layerIndex, int x, int y)
        {
            int count = 0;

            for (int i = 0; i < EightDirections.Length; i++)
            {
                Vector2Int position = EightDirections[i];
                position.x += x;
                position.y += y;

                if (position.x >= 0 && position.x < _width && position.y >= 0 && position.y < _height)
                {
                    if (map[layerIndex, position.x, position.y] != 0)
                        count++;
                }
            }

            return count;
        }

        public int CountFourNeighbor(int[,,] map, int layerIndex, int x, int y)
        {
            int count = 0;

            for (int i = 0; i < FourDirections.Length; i++)
            {
                Vector2Int position = FourDirections[i];
                position.x += x;
                position.y += y;

                if (position.x >= 0 && position.x < _width && position.y >= 0 && position.y < _height)
                {
                    if (map[layerIndex, position.x, position.y] != 0)
                        count++;
                }
            }

            return count;
        }
    }
}