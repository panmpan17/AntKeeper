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
        private GenerateMapProcess[] generateMapProcess;

        [Header("Tilemap")]
        [SerializeField]
        private Tilemap baseMap;
        [SerializeField]
        private Tilemap wallMap;
        [SerializeField]
        private Tilemap grassMap;
        [SerializeField]
        private Transform animalCollection;

        [Header("Place Item")]
        [SerializeField]
        private Tile groundTile;
        [SerializeField]
        private Tile wallTile;
        [SerializeField]
        private TileBase grassTite;
        [SerializeField]
        private GameObject animalPrefab;

        void Start()
        {
            ClearMap();
            Generate();
        }

        public void ClearMap()
        {
            baseMap.ClearAllTiles();
            wallMap.ClearAllTiles();

            while (animalCollection.childCount > 0)
            {
#if UNITY_EDITOR
                if (!EditorApplication.isPlaying)
                    DestroyImmediate(animalCollection.GetChild(0).gameObject);
                else
                    Destroy(animalCollection.GetChild(0).gameObject);
#else
                    Destroy(animalCollection.GetChild(0).gameObject);
#endif
            }
        }

        public void Generate()
        {
            int[,] map = RunThroughGenerateProcess();

            int xOffset = width / 2;
            int yOffset = height / 2;

            // List<Vector3Int> edgeTiles = new List<Vector3Int>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3Int gridPosition = new Vector3Int(x - xOffset, y - yOffset, 0);

                    int cell = map[x, y];
                    PlaceTile(gridPosition, map[x, y]);
                }
            }

            // for (int i = 0; i < edgeTiles.Count; i++)
            // {
            //     wallMap.SetTile(edgeTiles[i], wallTile);
            // }
        }

        void PlaceTile(Vector3Int position, int cell)
        {
            var item = (PlaceItem) cell;

            var hasGround = (item & PlaceItem.Ground) == PlaceItem.Ground;
            var hasAnimal = (item & PlaceItem.Animal) == PlaceItem.Animal;
            var hasGrass = (item & PlaceItem.Grass) == PlaceItem.Grass;

            if (hasGround)
            {
                baseMap.SetTile(position, groundTile);

                if (hasAnimal)
                {
                    GameObject newAnimal;
#if UNITY_EDITOR
                    if (!EditorApplication.isPlaying)
                    {
                        newAnimal = (GameObject)PrefabUtility.InstantiatePrefab(animalPrefab, animalCollection);
                    }
                    else
                        newAnimal = Instantiate(animalPrefab, animalCollection);
#else
                    newAnimal = Instantiate(animalPrefab, animalCollection);
#endif

                    newAnimal.transform.position = baseMap.GetCellCenterWorld(position);
                }

                grassMap.SetTile(position, hasGrass ? grassTite : null);
            }
            else
            {
                wallMap.SetTile(position, wallTile);
                grassMap.SetTile(position, null);
            }
        }

        int[,] RunThroughGenerateProcess()
        {
            int[,] map = new int[width, height];

            if (randomSeed)
                seed = Random.Range(0, 1000000);
            Random.InitState(seed);

            for (int i = 0; i < generateMapProcess.Length; i++)
            {
                for (int e = 0; e < generateMapProcess[i].processorList.Length; e++)
                {
                    GenerateMapProcess.ProcessorRule process = generateMapProcess[i].processorList[e];
                    switch (process.processorType)
                    {
                        case ProcessorType.RandomPlace:
                            process.randomPlaceProcessor.Process(ref map);
                            break;
                        case ProcessorType.Smooth:
                            process.smoothProcessor.Process(ref map);
                            break;
                        case ProcessorType.PlaceCircle:
                            process.placeCircleProcessor.Process(ref map);
                            break;
                        case ProcessorType.PerlinNoise:
                            process.perlinNoiseProccessor.Process(ref map);
                            break;
                    }
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