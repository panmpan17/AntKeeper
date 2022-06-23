using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EdgeGenerate : MonoBehaviour
{
    [SerializeField]
    private GridManager.GridLayer[] layers;
    [SerializeField]
    private Tilemap edgeWaveTilemap;

    [Header("Tile")]
    [SerializeField]
    private TileBase tallEdge;
    [SerializeField]
    private TileBase tallEdgeRight;
    [SerializeField]
    private TileBase smallEdge;
    [SerializeField]
    private TileBase edgeWaveTile;

    void Start()
    {
        for (int i = 0; i < layers.Length; i++)
            ScanLayer(i);
    }

    void ScanLayer(int index)
    {
        Tilemap map = layers[index].BaseMap;
        for (int x = map.cellBounds.xMin; x <= map.cellBounds.xMax; x++)
        {
            bool lastHasTile = false;

            for (int y = map.cellBounds.yMax; y >= map.cellBounds.yMin - 1; y--)
            {
                Vector3Int gridPosition = new Vector3Int(x, y, 0);

                if (map.HasTile(gridPosition))
                {
                    lastHasTile = true;
                    continue;
                }

                if (!lastHasTile)
                    continue;
                lastHasTile = false;

                if (index > 0)
                {
                    if (layers[index - 1].BaseMap.HasTile(gridPosition))
                    {
                        layers[index].EdgeMap.SetTile(gridPosition, smallEdge);
                        continue;
                    }

                    layers[index - 1].EdgeMap.SetTile(gridPosition, tallEdge);
                    layers[index - 1].EdgeMap.SetTile(gridPosition + Vector3Int.down, tallEdge);
                    edgeWaveTilemap.SetTile(gridPosition + Vector3Int.down, edgeWaveTile);
                    continue;
                }

                layers[index].EdgeMap.SetTile(gridPosition, tallEdge);
                layers[index].EdgeMap.SetTile(gridPosition + Vector3Int.down, tallEdge);
                edgeWaveTilemap.SetTile(gridPosition + Vector3Int.down, edgeWaveTile);
            }
        }
    }
}
