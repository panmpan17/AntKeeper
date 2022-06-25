using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace MapGenerate
{
    [System.Serializable]
    public class WallGenerate
    {
        [SerializeField]
        private TileBase wallTile;

        private GridManager.GridLayer[] _layers;

        public void Process(GridManager.GridLayer[] layers)
        {
            _layers = layers;

            for (int i = 0; i < layers.Length; i++)
                GenerateWall(i);

            _layers = null;
        }

        void GenerateWall(int index)
        {
            Tilemap wallMap = _layers[index].WallMap;

            LoopThroughTileMap(_layers[index].BaseMap, (gridPosition, hasTile) => {
                if (!hasTile)
                    wallMap.SetTile(gridPosition, wallTile);
            });

            for (int i = index + 1; i < _layers.Length; i++)
            {
                LoopThroughTileMap(_layers[i].EdgeMap, (gridPosition, hasTile) =>
                {
                    if (hasTile)
                        wallMap.SetTile(gridPosition, wallTile);
                });

                LoopThroughTileMap(_layers[i].BaseMap, (gridPosition, hasTile) =>
                {
                    if (hasTile)
                        wallMap.SetTile(gridPosition, wallTile);
                });
            }

            wallMap.GetComponent<CompositeCollider2D>().GenerateGeometry();
        }

        void LoopThroughTileMap(Tilemap map, System.Action<Vector3Int, bool> callback)
        {
            for (int x = map.cellBounds.xMin - 1; x <= map.cellBounds.xMax; x++)
            {
                for (int y = map.cellBounds.yMin - 1; y <= map.cellBounds.yMax; y++)
                {
                    Vector3Int gridPosition = new Vector3Int(x, y, 0);
                    callback.Invoke(gridPosition, map.HasTile(gridPosition));
                }
            }
        }
    }
}