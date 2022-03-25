using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RandomGeenrateAlgorithum : IMapGenerateAlgorithum
{
    static Vector2Int[] AllDirections = new Vector2Int[] {
        new Vector2Int(-1, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1, 1),
        new Vector2Int(0, -1),
        new Vector2Int(0, 1),
        new Vector2Int(1, -1),
        new Vector2Int(1, 0),
        new Vector2Int(1, 1),
    };

    [SerializeField]
    private bool randomSeed = true;
    [SerializeField]
    private int seed;
    [SerializeField]
    [Range(0, 1)]
    public float coverage;

    [SerializeField]
    [Range(1, 10)]
    private int smoothCount;
    [SerializeField]
    [Range(1, 8)]
    private int keepAliveNeighborNeedCount;
    [SerializeField]
    [Range(1, 8)]
    private int becomeAliveNeighborNeedCount;

    private int _width;
    private int _height;
    private int[,] _maps;

    public int[,] GenerateMap(int width, int height)
    {
        _width = width;
        _height = height;
        _maps = new int[width, height];

        if (randomSeed)
            seed = Random.Range(0, 1000000);

        Random.InitState(seed);
        FillMapWithRandomChance();

        for (int i = 0; i < smoothCount; i++)
            SmoothMap();

        return _maps;
    }

    void FillMapWithRandomChance()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (Random.value <= coverage)
                    _maps[x, y] = 1;
                else
                    _maps[x, y] = 0;
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                int neighborCount = CountNeighborAlive(x, y);

                if (_maps[x, y] > 0)
                {
                    // Is alive, check is there enough neighbor to keep alive
                    if (neighborCount < keepAliveNeighborNeedCount)
                        _maps[x, y] = 0;
                }
                else
                {
                    // Is dead, check is there enough neighbor to become alive
                    if (neighborCount >= becomeAliveNeighborNeedCount)
                        _maps[x, y] = 1;
                }
            }
        }
    }

    int CountNeighborAlive(int x, int y)
    {
        int count = 0;

        for (int i = 0; i < AllDirections.Length; i++)
        {
            Vector2Int position = AllDirections[i];
            position.x += x;
            position.y += y;

            if (position.x >= 0 && position.x < _width && position.y >= 0 && position.y < _height)
            {
                if (_maps[position.x, position.y] == 1)
                    count++;
            }
        }

        return count;
    }
}
