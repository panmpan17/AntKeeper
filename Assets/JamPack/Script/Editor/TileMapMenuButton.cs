using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;


static public class TileMapMenuButton {
    private static Dictionary<Vector3Int, TileBase> copyboardTiles = null;

    static private bool CheckForSelectedTilemap(out Tilemap tilemap)
    {
        GameObject selectedGameObj = Selection.activeGameObject;
        if (selectedGameObj != null)
        {
            tilemap = selectedGameObj.GetComponent<Tilemap>();
            return tilemap != null;
        }
        tilemap = null;
        return false;
    }
    static private bool CheckForSelectedTilemap()
    {
        GameObject selectedGameObj = Selection.activeGameObject;
        if (selectedGameObj != null)
            return selectedGameObj.GetComponent<Tilemap>() != null;
        return false;
    }

    [MenuItem("MPack/Tilemap/Copy All Tiles", true)]
    static public bool ValidateCopyAllTiles()
    {
        return CheckForSelectedTilemap();
    }

    [MenuItem("MPack/Tilemap/Copy All Tiles")]
    static public void CopyAllTiles()
    {
        copyboardTiles = new Dictionary<Vector3Int, TileBase>();

        Tilemap tilemap;
        if (CheckForSelectedTilemap(out tilemap))
        {
            BoundsInt bound = tilemap.cellBounds;
            for (int x = bound.min.x; x <= bound.max.x; x++)
            {
                for (int y = bound.min.y; y <= bound.max.y; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    if (tilemap.HasTile(pos))
                    {
                        copyboardTiles.Add(pos, tilemap.GetTile(pos));
                    }
                }
            }
        }
    }

    [MenuItem("MPack/Tilemap/Paste All Tiles", true)]
    static public bool ValidatePasteAllTiles()
    {
        if (copyboardTiles == null)
            return false;

        return CheckForSelectedTilemap();
    }

    [MenuItem("MPack/Tilemap/Paste All Tiles")]
    static public void PasteAllTiles()
    {
        Tilemap tilemap;
        if (CheckForSelectedTilemap(out tilemap))
        {
            Undo.RecordObject(tilemap, "Before paste the tiles to the tilemap");

            foreach (KeyValuePair<Vector3Int, TileBase> pair in copyboardTiles)
            {
                tilemap.SetTile(pair.Key, pair.Value);
            }
        }
    }

    [MenuItem("MPack/Tilemap/Clear All Tiles", true)]
    static public bool ValidateClearAllTiles()
    {
        if (CheckForSelectedTilemap())
            return true;
        return false;
    }

    [MenuItem("MPack/Tilemap/Clear All Tiles")]
    static public void ClearAllTiles()
    {
        Tilemap tilemap;
        if (CheckForSelectedTilemap(out tilemap))
        {
            bool confirm = EditorUtility.DisplayDialog("Clear Tilemap", "Are you sure you want to clear all the tiles in the tilemap?", "Yes", "No");
            if (confirm)
            {
                Undo.RecordObject(tilemap, "Before clear all the tiles");
                tilemap.ClearAllTiles();
            }
        }
    }
}