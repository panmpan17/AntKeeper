using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WallGenerate : MonoBehaviour
{
    [SerializeField]
    private GridManager.GridLayer[] layers;
    [SerializeField]
    private TileBase wallTile;

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < layers.Length; i++)
            GenerateWall(i);
    }

    void GenerateWall(int index)
    {
        Tilemap wallMap = layers[index].WallMap;

        LoopThroughTileMap(layers[index].BaseMap, (gridPosition, hasTile) => {
            if (!hasTile)
                wallMap.SetTile(gridPosition, wallTile);
        });

        for (int i = index + 1; i < layers.Length; i++)
        {
            LoopThroughTileMap(layers[i].EdgeMap, (gridPosition, hasTile) =>
            {
                if (hasTile)
                    wallMap.SetTile(gridPosition, wallTile);
            });

            LoopThroughTileMap(layers[i].BaseMap, (gridPosition, hasTile) =>
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
