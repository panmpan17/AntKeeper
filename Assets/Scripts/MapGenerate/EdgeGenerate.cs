using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MapGenerate
{
    [System.Serializable]
    public class EdgeGenerate
    {
        [Header("Tile")]
        [SerializeField]
        private TileBase tallEdge;
        [SerializeField]
        private TileBase tallEdgeRight;
        [SerializeField]
        private TileBase smallEdge;
        [SerializeField]
        private TileBase edgeWaveTile;


        private GridManager.GridLayer[] _layers;
        private Tilemap _edgeWaveTilemap;

        public void Process(GridManager.GridLayer[] layers, Tilemap edgeWaveTilemap)
        {
            _layers = layers;
            _edgeWaveTilemap = edgeWaveTilemap;

            for (int i = 0; i < _layers.Length; i++)
            {
                ScanLayer(i);
            }
        }

        void ScanLayer(int index)
        {
            Tilemap map = _layers[index].BaseMap;
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
                        if (_layers[index - 1].BaseMap.HasTile(gridPosition))
                        {
                            _layers[index].EdgeMap.SetTile(gridPosition, smallEdge);
                            continue;
                        }

                        _layers[index - 1].EdgeMap.SetTile(gridPosition, tallEdge);
                        _layers[index - 1].EdgeMap.SetTile(gridPosition + Vector3Int.down, tallEdge);
                        _edgeWaveTilemap.SetTile(gridPosition + Vector3Int.down, edgeWaveTile);
                        continue;
                    }

                    _layers[index].EdgeMap.SetTile(gridPosition, tallEdge);
                    _layers[index].EdgeMap.SetTile(gridPosition + Vector3Int.down, tallEdge);
                    _edgeWaveTilemap.SetTile(gridPosition + Vector3Int.down, edgeWaveTile);
                }
            }
        }
    }
}