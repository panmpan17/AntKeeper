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

        public void Process(ref int[,] map)
        {
            _width = map.GetLength(0);
            _height = map.GetLength(1);

            for (int i = 0; i < smoothCount; i++)
                Smooth(ref map);
        }

        void Smooth(ref int[,] map)
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    int neighborCount = CountEightNeighbor(map, x, y);

                    if (map[x, y] > 0)
                    {
                        // Is alive, check is there enough neighbor to keep alive
                        if (neighborCount < keepAliveNeighborNeedCount)
                            map[x, y] = 0;
                    }
                    else
                    {
                        // Is dead, check is there enough neighbor to become alive
                        if (neighborCount >= becomeAliveNeighborNeedCount)
                            map[x, y] = 1;
                    }
                }
            }
        }

        public static int CountEightNeighbor(int[,] map, int x, int y)
        {
            int count = 0;

            int width = map.GetLength(0);
            int height = map.GetLength(1);

            for (int i = 0; i < EightDirections.Length; i++)
            {
                Vector2Int position = EightDirections[i];
                position.x += x;
                position.y += y;

                if (position.x >= 0 && position.x < width && position.y >= 0 && position.y < height)
                {
                    if (map[position.x, position.y] == 1)
                        count++;
                }
            }

            return count;
        }

        public static int CountFourNeighbor(int[,] map, int x, int y)
        {
            int count = 0;

            int width = map.GetLength(0);
            int height = map.GetLength(1);

            for (int i = 0; i < FourDirections.Length; i++)
            {
                Vector2Int position = FourDirections[i];
                position.x += x;
                position.y += y;

                if (position.x >= 0 && position.x < width && position.y >= 0 && position.y < height)
                {
                    if (map[position.x, position.y] == 1)
                        count++;
                }
            }

            return count;
        }
    }
}