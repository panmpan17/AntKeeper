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
    private RandomGeenrateAlgorithum algorithum;

    void Start()
    {
        Generate();
    }

    public void Generate()
    {
        int[,] maps = algorithum.GenerateMap(width, height);

        int xOffset = width / 2;
        int yOffset = height / 2;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                baseMap.SetTile(new Vector3Int(x - xOffset, y - yOffset, 0), baseMapTileSet[maps[x, y]]);
            }
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

        if (GUILayout.Button("Generate"))
        {
            var manager = (MapGeneratorManager)target;
            manager.Generate();
        }
    }
}
#endif
