using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor.Tilemaps;
using UnityEditor;
using System.Linq;

[CustomGridBrush(true, false, true, "Default Brush (PolygonColliderEditor)")]
public class LineBrush : GridBrush
{
}
[CustomEditor(typeof(LineBrush))]
public class LineBrushEditor : GridBrushEditor
{
    public override GameObject[] validTargets {
        get {
            if (PolygonColliderTilemapStyleEditor.ins != null && PolygonColliderTilemapStyleEditor.ins.EditingTilemap != null)
                return new GameObject[] { PolygonColliderTilemapStyleEditor.ins.EditingTilemap.gameObject };
            else
            {
                var list = from map in GameObject.FindObjectsOfType<Tilemap>() select map.gameObject;
                return list.ToArray();
            }
        }
    }
}