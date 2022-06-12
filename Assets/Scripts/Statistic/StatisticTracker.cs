using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StatisticTracker : MonoBehaviour
{
    public static StatisticTracker ins;

    [SerializeField]
    private StatisticProvider statisticProvider;

    private GameStatic _statistic;
    private int _bucketDestroyFireAntCount = 0;
    private int _bucketDestroyNativeAntCount = 0;
    private int _breedFireAntCount = 0;
    private int _breedNativeAntCount = 0;

    void Awake()
    {
        ins = this;

        statisticProvider.Get = CollectGameStatic;
        _statistic = ScriptableObject.CreateInstance<GameStatic>();
    }

    public GameStatic CollectGameStatic()
    {
        CollectAntNestHubData();
        CollectBaseMapTiles();
        _statistic.OriginalAnimalCount = GridManager.ins.OriginAnimalCount;
        _statistic.ResultAnimalCount = GridManager.ins.CountAliveAnimal();
        _statistic.BucketDestroyFireAntCount = _bucketDestroyFireAntCount;
        _statistic.BucketDestroyNativeAntCount = _bucketDestroyNativeAntCount;
        _statistic.BreedFireAntCount = _breedFireAntCount;
        _statistic.BreedNativeAntCount = _breedNativeAntCount;

        return _statistic;
    }

    public void CollectAntNestHubData()
    {
        List<GameStatic.AntNestInfo> fireAnts = new List<GameStatic.AntNestInfo>();
        List<GameStatic.AntNestInfo> nativeAnts = new List<GameStatic.AntNestInfo>();

        List<AntNestHub> hubs = GridManager.ins.AntNestHubs;

        for (int i = 0; i < hubs.Count; i++)
        {
            if (hubs[i].IsFireAnt)
            {
                fireAnts.Add(new GameStatic.AntNestInfo
                {
                    NestSize = hubs[i].Size,
                    AreaSize = hubs[i].CountAreaSize(),
                    StillAlive = hubs[i].enabled,
                    Revealed = hubs[i].IsShowTrueColor
                });
            }
            else
            {
                nativeAnts.Add(new GameStatic.AntNestInfo
                {
                    NestSize = hubs[i].Size,
                    AreaSize = hubs[i].CountAreaSize(),
                    StillAlive = hubs[i].enabled,
                    Revealed = hubs[i].IsShowTrueColor
                });
            }
        }

        _statistic.FireAnts = fireAnts.ToArray();
        _statistic.NativeAnts = nativeAnts.ToArray();
    }

    public void CollectBaseMapTiles()
    {
        Tilemap baseMap = GridManager.ins.BaseMap;

        int totalTileCount = 0;
        for (int x = baseMap.cellBounds.xMin; x < baseMap.cellBounds.xMax; x++)
        {
            for (int y = baseMap.cellBounds.yMin; y < baseMap.cellBounds.yMax; y++)
            {
                if (baseMap.HasTile(new Vector3Int(x, y, 0)))
                    totalTileCount += 1;
            }
        }
        _statistic.TotalMapArea = totalTileCount;
    }

    public void AddPlayerDestroyHubRecord(bool isFireAnt)
    {
        if (isFireAnt)
            _bucketDestroyFireAntCount++;
        else
            _bucketDestroyNativeAntCount++;
    }

    public void AddBreedAntRecord(bool isFireAnt)
    {
        if (isFireAnt)
            _breedFireAntCount++;
        else
            _breedNativeAntCount++;
    }
}
