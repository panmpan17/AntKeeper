using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MapGenerate
{
    public enum ProcessorType
    {
        RandomPlace,
        Smooth,
        PlaceCircle,
    }

    [CreateAssetMenu]
    public class GenerateMapProcess : ScriptableObject
    {
        public ProcessorRule[] processorList;

        [System.Serializable]
        public struct ProcessorRule
        {
            public ProcessorType processorType;

            public RandomPlaceProcessor randomPlaceProcessor;
            public SmoothProcessor smoothProcessor;
            public PlaceCircleProcessor placeCircleProcessor;
        }
    }
}