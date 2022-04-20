using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using UnityEditor.Tilemaps;

public class PolygonColliderTilemapStyleEditor : EditorWindow
{
    public static PolygonColliderTilemapStyleEditor ins;

    private PolygonCollider2D _editingPolygon;

    private TileBase _fillerTile;

    private Grid _grid;
    private Tilemap _tilemap;
    public Tilemap EditingTilemap => _tilemap;

    private GUIContent FillerTileUIText = new GUIContent("Filler Tile");
    private GUIContent PolygonUIText = new GUIContent("Polygon Collider 2D");

    private CompositeCollider2D _compositeCollider;

    [MenuItem("MPack/Polygon Collider Editor")]
    public static void OpenWindow()
    {
        var window = GetWindow<PolygonColliderTilemapStyleEditor>("Polygon Collider Editor");
    }

    void OnEnable()
    {
        ins = this;
    }

    void OnDisable()
    {
        if (_tilemap != null)
        {
            DestroyImmediate(_tilemap.gameObject);
            _tilemap = null;
            _compositeCollider = null;
        }
        ins = null;
    }

    void OnGUI()
    {
        _fillerTile = (TileBase)EditorGUILayout.ObjectField(FillerTileUIText, _fillerTile, typeof(TileBase), allowSceneObjects: true);

        GUILayout.Label("P");

        EditorGUI.BeginChangeCheck();
        PolygonCollider2D newPolygon = (PolygonCollider2D)EditorGUILayout.ObjectField(PolygonUIText, _editingPolygon, typeof(PolygonCollider2D), allowSceneObjects: true);
        if (EditorGUI.EndChangeCheck())
        {
            if (_editingPolygon != null)
            {
                if (EditorUtility.DisplayDialog(
                    "Change Polygon Collider",
                    "Are you sure you want to switch polygon collider? Progress won't be saved",
                    "Don't Save, Continue", "Cancel"))
                {
                    DestroyImmediate(_tilemap.gameObject);
                    _tilemap = null;
                    _compositeCollider = null;
                }
                else
                {
                    return;
                }
            }

            _editingPolygon = newPolygon;
            return;
        }

        if (_editingPolygon == null || _fillerTile == null)
        {
            EditorGUILayout.HelpBox("Need to assign Fillter Tile and Polygon Collider 2D", MessageType.Warning);
            return;
        }

        GUILayout.Space(5);
        TilemapEditGUI();
    }

    void TilemapEditGUI()
    {
        if (_tilemap == null)
        {
            if (GUILayout.Button("Edit Polygon using Tilemap"))
            {
                CreateEditingTilemap();
                ApplyColliderToTilemap();
            }
        }
        else
        {
            if (GUILayout.Button("Apply"))
            {
                ApplyTilemapToCollider();
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Cancel"))
            {
                if (EditorUtility.DisplayDialog(
                    "Cancel Editing",
                    "Are you sure you want to cancel editing polygon collider? Progress won't be saved",
                    "Don't Save, Continue", "Go Back"))
                {
                    DestroyImmediate(_tilemap.gameObject);
                    _tilemap = null;
                }
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Save and Close Tilemap"))
            {
                ApplyTilemapToCollider();
                DestroyImmediate(_tilemap.gameObject);
                _tilemap = null;
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    void CreateEditingTilemap()
    {
        _grid = FindObjectOfType<Grid>();

        var newTilemapObj = new GameObject("_EditingPolygonCollider (Don't Touch)");
        newTilemapObj.transform.SetParent(_grid.transform);

        _tilemap = newTilemapObj.AddComponent<Tilemap>();
        _tilemap.color = new Color(0.1f, 1f, 0.1f, 0.5f);

        var tilemapRenderer = newTilemapObj.AddComponent<TilemapRenderer>();
        tilemapRenderer.sortingOrder = 100;

        var rigidbody = newTilemapObj.AddComponent<Rigidbody2D>();
        rigidbody.bodyType = RigidbodyType2D.Static;

        _compositeCollider = newTilemapObj.AddComponent<CompositeCollider2D>();
        _compositeCollider.geometryType = CompositeCollider2D.GeometryType.Polygons;

        var tilemapCollider = newTilemapObj.AddComponent<TilemapCollider2D>();
        tilemapCollider.usedByComposite = true;
    }

    void ApplyColliderToTilemap()
    {
        Bounds polygonBound = _editingPolygon.bounds;

        for (float x = polygonBound.min.x; x <= polygonBound.max.x; x += _tilemap.cellSize.x)
        {
            x = Mathf.Round(x);
            for (float y = polygonBound.min.y; y <= polygonBound.max.y; y += _tilemap.cellSize.y)
            {
                y = Mathf.Round(y);
                Vector3Int cellPosition = _grid.WorldToCell(new Vector3(x, y, 0));
                if (_editingPolygon.OverlapPoint(_grid.GetCellCenterWorld(cellPosition)))
                {
                    _tilemap.SetTile(cellPosition, _fillerTile);
                }
            }
        }
    }

    void ApplyTilemapToCollider()
    {
        if (_editingPolygon)
        {
            _editingPolygon.pathCount = _compositeCollider.pathCount;

            for (int pathIndex = 0; pathIndex < _compositeCollider.pathCount; pathIndex++)
            {
                var points = new Vector2[_compositeCollider.GetPathPointCount(pathIndex)];
                _compositeCollider.GetPath(pathIndex, points);
                _editingPolygon.SetPath(pathIndex, points);
            }
        }
    }
}
