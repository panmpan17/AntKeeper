using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class MapGeneratorManager : MonoBehaviour
{
    [SerializeField]
    private Tilemap baseMap;
    [SerializeField]
    private Tile[] baseMapTileSet;

    [SerializeField]
    private int width;
    [SerializeField]
    private int height;

    [SerializeField]
    private ProcessorRule[] processorList;

    void Start()
    {
        Generate();
    }

    public void Generate()
    {
        int[,] map = new int[width, height];

        for (int i = 0; i < processorList.Length; i++)
        {
            switch (processorList[i].processorType)
            {
                case ProcessorType.RandomPlace:
                    processorList[i].randomPlaceProcessor.Process(ref map);
                    break;
                case ProcessorType.Smooth:
                    processorList[i].smoothProcessor.Process(ref map);
                    break;
            }
        }

        int xOffset = width / 2;
        int yOffset = height / 2;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                baseMap.SetTile(new Vector3Int(x - xOffset, y - yOffset, 0), baseMapTileSet[map[x, y]]);
            }
        }
    }

    [System.Serializable]
    public struct ProcessorRule
    {
        public ProcessorType processorType;

        public RandomPlaceProcessor randomPlaceProcessor;
        public SmoothProcessor smoothProcessor;
    }

    public enum ProcessorType
    {
        RandomPlace,
        Smooth,
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MapGeneratorManager))]
public class MapGeneratorManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate"))
        {
            var manager = (MapGeneratorManager)target;
            manager.Generate();
        }
    }
}
#endif
