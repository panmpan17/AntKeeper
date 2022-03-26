using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace MapGenerate
{
    public class MapGeneratorManager : MonoBehaviour
    {
        [SerializeField]
        private bool randomSeed = true;
        [SerializeField]
        private int seed;

        [SerializeField]
        private int width;
        [SerializeField]
        private int height;

        [SerializeField]
        private GenerateMapProcess generateMapProcess;

        [Header("Place map setting")]
        [SerializeField]
        private Tilemap baseMap;
        [SerializeField]
        private Tile[] baseMapTileSet;

        [SerializeField]
        private Tilemap wallMap;
        [SerializeField]
        private Tile wallTile;

        void Start()
        {
            ClearMap();
            Generate();
        }

        public void ClearMap()
        {
            baseMap.ClearAllTiles();
            wallMap.ClearAllTiles();
        }

        public void Generate()
        {
            int[,] map = RunThroughGenerateProcess();

            int xOffset = width / 2;
            int yOffset = height / 2;

            List<Vector3Int> edgeTiles = new List<Vector3Int>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int cell = map[x, y];
                    int neighborCount = SmoothProcessor.CountFourNeighbor(map, x, y);
                    if (cell == 0 && neighborCount > 0 && neighborCount < 4)
                    {
                        edgeTiles.Add(new Vector3Int(x - xOffset, y - yOffset, 0));
                    }

                    baseMap.SetTile(new Vector3Int(x - xOffset, y - yOffset, 0), baseMapTileSet[cell]);
                }
            }

            for (int i = 0; i < edgeTiles.Count; i++)
            {
                wallMap.SetTile(edgeTiles[i], wallTile);
            }
        }

        int[,] RunThroughGenerateProcess()
        {
            int[,] map = new int[width, height];

            if (randomSeed)
                seed = Random.Range(0, 1000000);
            Random.InitState(seed);

            for (int i = 0; i < generateMapProcess.processorList.Length; i++)
            {
                switch (generateMapProcess.processorList[i].processorType)
                {
                    case ProcessorType.RandomPlace:
                        generateMapProcess.processorList[i].randomPlaceProcessor.Process(ref map);
                        break;
                    case ProcessorType.Smooth:
                        generateMapProcess.processorList[i].smoothProcessor.Process(ref map);
                        break;
                    case ProcessorType.PlaceCircle:
                        generateMapProcess.processorList[i].placeCircleProcessor.Process(ref map);
                        break;
                }
            }

            return map;
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
                manager.ClearMap();
                manager.Generate();
            }
        }
    }
    #endif
}