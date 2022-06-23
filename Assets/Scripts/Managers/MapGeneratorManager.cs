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
        private int layerCount;
        // [SerializeField]
        private GridManager.GridLayer[] layers;

        [SerializeField]
        private GenerateMapProcess[] generateMapProcess;

        [Header("Tilemap")]
        [SerializeField]
        private Tilemap grassMap;

        [SerializeField]
        private GameObject baseMapPrefab;
        [SerializeField]
        private GameObject edgeMapPrefab;
        [SerializeField]
        private GameObject wallMapPrefab;

        [Header("Place Item")]
        [SerializeField]
        private TileBase groundTile;
        [SerializeField]
        private TileBase wallTile;
        [SerializeField]
        private TileBase grassTite;
        [SerializeField]
        private Transform animalCollection;
        [SerializeField]
        private GameObject animalPrefab;


        public void ClearMap()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.name.StartsWith("layer-"))
                {
#if UNITY_EDITOR
                    if (!EditorApplication.isPlaying)
                        DestroyImmediate(child.gameObject);
                    else
                        Destroy(child.gameObject);
#else
                    Destroy(child.gameObject);
#endif
                    i--;
                }
            }

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
            InstantiateTilemapLayers();

            int[,,] map = RunThroughGenerateProcess();


            int xOffset = width / 2;
            int yOffset = height / 2;

            for (int layerIndex = 0; layerIndex < layerCount; layerIndex++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        Vector3Int gridPosition = new Vector3Int(x - xOffset, y - yOffset, 0);

                        int cell = map[layerIndex, x, y];
                        PlaceTile(layerIndex, gridPosition, cell);
                    }
                }
            }
        }

        void InstantiateTilemapLayers()
        {
            layers = new GridManager.GridLayer[layerCount];
            for (int i = 0; i < layerCount; i++)
            {
                var newLayerObject = new GameObject("layer-" + (i + 1));
                newLayerObject.transform.SetParent(transform);
                newLayerObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

                layers[i] = new GridManager.GridLayer
                {
                    BaseMap = Instantiate(baseMapPrefab, newLayerObject.transform).GetComponent<Tilemap>(),
                    EdgeMap = Instantiate(edgeMapPrefab, newLayerObject.transform).GetComponent<Tilemap>(),
                    WallMap = Instantiate(wallMapPrefab, newLayerObject.transform).GetComponent<Tilemap>(),
                };
            }
        }


        int[,,] RunThroughGenerateProcess()
        {
            int[,,] map = new int[layerCount, width, height];

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
                        // case ProcessorType.Smooth:
                        //     process.smoothProcessor.Process(ref map);
                        //     break;
                        // case ProcessorType.PlaceCircle:
                        //     process.placeCircleProcessor.Process(ref map);
                        //     break;
                        case ProcessorType.PerlinNoise:
                            process.perlinNoiseProccessor.Process(ref map);
                            break;
                    }
                }
            }

            return map;
        }

        void PlaceTile(int layerIndex, Vector3Int position, int cell)
        {
            var item = (PlaceItem) cell;

            var hasGround = (item & PlaceItem.Ground) == PlaceItem.Ground;
            var hasAnimal = (item & PlaceItem.Animal) == PlaceItem.Animal;
            var hasGrass = (item & PlaceItem.Grass) == PlaceItem.Grass;

            if (hasGround)
            {
                layers[layerIndex].BaseMap.SetTile(position, groundTile);

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

                    newAnimal.transform.position = layers[layerIndex].BaseMap.GetCellCenterWorld(position);
                }

                grassMap.SetTile(position, hasGrass ? grassTite : null);
            }
            else
            {
                grassMap.SetTile(position, null);
            }
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(MapGeneratorManager))]
    public class MapGeneratorManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear"))
            {
                var manager = (MapGeneratorManager)target;
                manager.ClearMap();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Generate"))
            {
                var manager = (MapGeneratorManager)target;
                manager.ClearMap();
                manager.Generate();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
    #endif
}