using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class GameStatic : ScriptableObject
{
    [Header("Map")]
    public int TotalMapArea;

    [Header("Movement")]
    public float WalkDistance;
    public int JumpCount;
    public float JumpDistance;

    [Header("Bucket")]
    public int PickupBucketCount;
    public float PourWaterAmount;
    public float BucketCauseDamageAmount;

    [Header("Bucket")]
    public int PickupAntJar;
    public int ExamineAntCount;

    [Header("Test Tube")]
    public int PickupTestTube;
    public int CatchAntCount;
    public int PlantNewAntSuccessCount;
    public int PlantNewAntFailCount;

    [Header("Animal")]
    public int OriginalAnimalCount;
    public int ResultAnimalCount;

    [Header("Ants")]
    public AntNestInfo[] NativeAnts;
    public int NativeAntTotalArea
    {
        get
        {
            int count = 0;
            for (int i = 0; i < NativeAnts.Length; i++)
                count += NativeAnts[i].AreaSize;
            return count;
        }
    }
    public float NativeAntAreaPercentage
    {
        get
        {
            return (float) NativeAntTotalArea / (float) TotalMapArea;
        }
    }

    public AntNestInfo[] FireAnts;
    public int FireAntTotalArea
    {
        get
        {
            int count = 0;
            for (int i = 0; i < FireAnts.Length; i++)
                count += FireAnts[i].AreaSize;
            return count;
        }
    }
    public float FireAntAreaPercentage
    {
        get
        {
            return (float) FireAntTotalArea / (float) TotalMapArea;
        }
    }

    public float CalculateScore()
    {
        int nativeAntAreaCount = 0;
        float nativeAntTotalSize = 0;
        for (int i = 0; i < NativeAnts.Length; i++)
        {
            nativeAntAreaCount += NativeAnts[i].AreaSize;

            if (NativeAnts[i].StillAlive)
                nativeAntTotalSize += NativeAnts[i].NestSize;
        }

        int fireAntAreaCount = 0;
        float fireAntTotalSize = 0;
        for (int i = 0; i < FireAnts.Length; i++)
        {
            fireAntAreaCount += FireAnts[i].AreaSize;

            if (FireAnts[i].StillAlive)
                fireAntTotalSize += FireAnts[i].NestSize;
        }

        return (nativeAntTotalSize * 15 + nativeAntAreaCount) - (fireAntTotalSize * 15 + fireAntAreaCount) + ((float)ResultAnimalCount / (float)OriginalAnimalCount * 100);
    }

    [System.Serializable]
    public struct AntNestInfo
    {
        public float NestSize;
        public int AreaSize;
        public bool StillAlive;
    }
}
