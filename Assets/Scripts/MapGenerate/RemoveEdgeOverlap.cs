using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace MapGenerate
{
    [System.Serializable]
    public class RemoveEdgeOverlap
    {
        public void Process(MapGeneratorManager manager)
        {
            for (int i = 0; i < manager.Layers.Length; i++)
            {
                LoopThroughTileMap(manager.Layers[i].EdgeMap, (position, hasTile) => {
                    if (hasTile)
                    {
                        manager.GrassMap.SetTile(position, null);
                        manager.RemoveAnimal(position);
                    }
                });
            }
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