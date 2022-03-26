// #define HAS_2D_TILEMAP_EXTRA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;


#if HAS_2D_TILEMAP_EXTRA
public class TileSetMaker : MonoBehaviour
{
    private static Vector3Int TOPLEFT = new Vector3Int(-1, 1, 0);
    private static Vector3Int TOP = new Vector3Int(0, 1, 0);
    private static Vector3Int TOPRIGHT = new Vector3Int(1, 1, 0);

    private static Vector3Int LEFT = new Vector3Int(-1, 0, 0);
    private static Vector3Int CENTER = new Vector3Int(0, 0, 0);
    private static Vector3Int RIGHT = new Vector3Int(1, 0, 0);

    private static Vector3Int DOWNLEFT = new Vector3Int(-1, -1, 0);
    private static Vector3Int DOWN = new Vector3Int(0, -1, 0);
    private static Vector3Int DOWNRIGHT = new Vector3Int(1, -1, 0);

    [MenuItem("MPack/Import Blob Tile", true)]
    static private bool ValidateAnalyzeSprites()
    {
        return Selection.objects.Length == 1;
    }

    [MenuItem("MPack/Import Blob Tile")]
    static private void AnalyzeSprites() {
        if (Selection.objects.Length != 1)
            return;

        Object file = Selection.objects[0];
        // Texture2D texture = (Texture2D) file;
        string path = AssetDatabase.GetAssetPath(file);
        Object[] spriteObjs = AssetDatabase.LoadAllAssetsAtPath(path);
        List<Sprite> sprites = new List<Sprite>();

        for (int i = 0; i < spriteObjs.Length; i++) {
            try {
                Sprite sprite = (Sprite) spriteObjs[i];
                sprites.Add(sprite);
            }
            catch (System.Exception) {}
        }

        sprites.Sort((sprite1, sprite2) => {
            int dashIndex1 = sprite1.name.LastIndexOf("_");
            int dashIndex2 = sprite2.name.LastIndexOf("_");
            int index1 = int.Parse(sprite1.name.Substring(dashIndex1 + 1));
            int index2 = int.Parse(sprite2.name.Substring(dashIndex2 + 1));

            // // return 0;
            return index1.CompareTo(index2);
        });

        
        RuleTile ruleTile = ScriptableObject.CreateInstance<RuleTile>();
        ruleTile.m_DefaultSprite = sprites[11];

        for (int i = 0; i < sprites.Count; i++) {
            RuleTile.TilingRule rule = FindBlobTileRule(i);
            if (rule != null) {
                rule.m_Sprites = new Sprite[] { sprites[i] };
                ruleTile.m_TilingRules.Add(rule);
            }
        }

        AssetDatabase.CreateAsset(ruleTile, "Assets/NewBlobTile.asset");
        AssetDatabase.SaveAssets();
    }

    static private RuleTile.TilingRule FindBlobTileRule(int index) {
        RuleTile.TilingRule rule = new RuleTile.TilingRule();
        Dictionary<Vector3Int, int> tileRule = new Dictionary<Vector3Int, int>();

        switch (index) {
            case 0:
                tileRule.Add(TOP, 2);
                tileRule.Add(LEFT, 2);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 1);
                tileRule.Add(DOWNRIGHT, 1);
                break;
            case 1:
                tileRule.Add(TOP, 2);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWNLEFT, 1);
                tileRule.Add(DOWN, 1);
                tileRule.Add(DOWNRIGHT, 1);
                break;
            case 2:
                tileRule.Add(TOP, 2);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 2);
                tileRule.Add(DOWNLEFT, 1);
                tileRule.Add(DOWN, 1);
                break;

            case 10:
                tileRule.Add(TOP, 1);
                tileRule.Add(TOPRIGHT, 1);
                tileRule.Add(LEFT, 2);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 1);
                tileRule.Add(DOWNRIGHT, 1);
                break;
            case 11:
                tileRule.Add(TOPLEFT, 1);
                tileRule.Add(TOP, 1);
                tileRule.Add(TOPRIGHT, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWNLEFT, 1);
                tileRule.Add(DOWN, 1);
                tileRule.Add(DOWNRIGHT, 1);
                break;
            case 12:
                tileRule.Add(TOPLEFT, 1);
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 2);
                tileRule.Add(DOWNLEFT, 1);
                tileRule.Add(DOWN, 1);
                break;

            case 20:
                tileRule.Add(TOP, 1);
                tileRule.Add(TOPRIGHT, 1);
                tileRule.Add(LEFT, 2);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 2);
                break;
            case 21:
                tileRule.Add(TOPLEFT, 1);
                tileRule.Add(TOP, 1);
                tileRule.Add(TOPRIGHT, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 2);
                break;
            case 22:
                tileRule.Add(TOPLEFT, 1);
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 2);
                tileRule.Add(DOWN, 2);
                break;

            case 3:
                tileRule.Add(TOP, 2);
                tileRule.Add(LEFT, 2);
                tileRule.Add(RIGHT, 2);
                tileRule.Add(DOWN, 1);
                break;
            case 13:
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 2);
                tileRule.Add(RIGHT, 2);
                tileRule.Add(DOWN, 1);
                break;
            case 23:
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 2);
                tileRule.Add(RIGHT, 2);
                tileRule.Add(DOWN, 2);
                break;

            case 31:
                tileRule.Add(TOP, 2);
                tileRule.Add(LEFT, 2);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 2);
                break;
            case 32:
                tileRule.Add(TOP, 2);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 2);
                break;
            case 33:
                tileRule.Add(TOP, 2);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 2);
                tileRule.Add(DOWN, 2);
                break;

            case 34:
                tileRule.Add(TOP, 2);
                tileRule.Add(LEFT, 2);
                tileRule.Add(RIGHT, 2);
                tileRule.Add(DOWN, 2);
                break;

            case 4:
                // 1: 
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 1);
                // 2: 
                tileRule.Add(TOP, 2);
                tileRule.Add(LEFT, 2);
                tileRule.Add(DOWNRIGHT,2);
                break;
            case 5:
                // 1:
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWNLEFT, 1);
                tileRule.Add(DOWN, 1);
                // 2:
                tileRule.Add(TOP, 2);
                tileRule.Add(DOWNRIGHT, 2);
                break;
            case 6:
                // 1:
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 1);
                tileRule.Add(DOWNRIGHT, 1);
                // 2:
                tileRule.Add(TOP, 2);
                tileRule.Add(DOWNLEFT, 2);
                break;
            case 7:
                // 1:
                tileRule.Add(LEFT, 1);
                tileRule.Add(DOWN, 1);
                // 2:
                tileRule.Add(TOP, 2);
                tileRule.Add(RIGHT, 2);
                tileRule.Add(DOWNLEFT, 2);
                break;
            case 8:
                // 1:
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 1);
                // 2:
                tileRule.Add(TOP, 2);
                tileRule.Add(DOWNLEFT, 2);
                tileRule.Add(DOWNRIGHT, 2);
                break;
            
            case 9:
                // 1:
                tileRule.Add(TOPLEFT, 1);
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 1);
                tileRule.Add(DOWNRIGHT, 1);
                // 2:
                tileRule.Add(TOPRIGHT, 2);
                tileRule.Add(DOWNLEFT, 2);
                break;
            case 14:
                // 1:
                tileRule.Add(TOP, 1);
                tileRule.Add(TOPRIGHT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 1);
                // 2:
                tileRule.Add(LEFT, 2);
                tileRule.Add(DOWNRIGHT, 2);
                break;
            case 15:
                // 1:
                tileRule.Add(TOPLEFT, 1);
                tileRule.Add(TOP, 1);
                tileRule.Add(TOPRIGHT, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWNLEFT, 1);
                tileRule.Add(DOWN, 1);
                // 2:
                tileRule.Add(DOWNRIGHT, 2);
                break;
            case 16:
                // 1:
                tileRule.Add(TOPLEFT, 1);
                tileRule.Add(TOP, 1);
                tileRule.Add(TOPRIGHT, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 1);
                tileRule.Add(DOWNRIGHT, 1);
                // 2:
                tileRule.Add(DOWNLEFT, 2);
                break;
            case 17:
                // 1:
                tileRule.Add(TOPLEFT, 1);
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(DOWN, 1);
                // 2:
                tileRule.Add(RIGHT, 2);
                tileRule.Add(DOWNLEFT, 2);
                break;
            case 18:
                // 1:
                tileRule.Add(TOPLEFT, 1);
                tileRule.Add(TOP, 1);
                tileRule.Add(TOPRIGHT, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 1);
                // 2:
                tileRule.Add(DOWNLEFT, 2);
                tileRule.Add(DOWNRIGHT, 2);
                break;
            case 19:
                // 1:
                tileRule.Add(TOP, 1);
                tileRule.Add(TOPRIGHT, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWNLEFT, 1);
                tileRule.Add(DOWN, 1);
                // 2:
                tileRule.Add(TOPLEFT, 2);
                tileRule.Add(DOWNRIGHT, 2);
                break;

            case 24:
                // 1:
                tileRule.Add(TOP, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 1);
                tileRule.Add(DOWNRIGHT, 1);
                // 2:
                tileRule.Add(TOPRIGHT, 2);
                tileRule.Add(LEFT, 2);
                break;
            case 25:
                // 1:
                tileRule.Add(TOPLEFT, 1);
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWNLEFT, 1);
                tileRule.Add(DOWN, 1);
                tileRule.Add(DOWNRIGHT, 1);
                // 2:
                tileRule.Add(TOPRIGHT, 2);
                break;
            case 26:
                // 1:
                tileRule.Add(TOP, 1);
                tileRule.Add(TOPRIGHT, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWNLEFT, 1);
                tileRule.Add(DOWN, 1);
                tileRule.Add(DOWNRIGHT, 1);
                // 2:
                tileRule.Add(TOPLEFT, 2);
                break;
            case 27:
                // 1:
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(DOWNLEFT, 1);
                tileRule.Add(DOWN, 1);
                // 2:
                tileRule.Add(TOPLEFT, 2);
                tileRule.Add(RIGHT, 2);
                break;
            case 28:
                // 1:
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWNLEFT, 1);
                tileRule.Add(DOWN, 1);
                tileRule.Add(DOWNRIGHT, 1);
                // 2:
                tileRule.Add(TOPLEFT, 2);
                tileRule.Add(TOPRIGHT, 2);
                break;
            case 29:
                // 1:
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 1);
                tileRule.Add(DOWNRIGHT, 1);
                // 2:
                tileRule.Add(TOPLEFT, 2);
                tileRule.Add(TOPRIGHT, 2);
                tileRule.Add(DOWNLEFT, 2);
                break;
            case 30:
                // 1:
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWNLEFT, 1);
                tileRule.Add(DOWN, 1);
                // 2:
                tileRule.Add(TOPLEFT, 2);
                tileRule.Add(TOPRIGHT, 2);
                tileRule.Add(DOWNRIGHT,  2);
                break;

            case 35:
                // 1:
                tileRule.Add(TOP, 1);
                tileRule.Add(RIGHT, 1);
                // 2:
                tileRule.Add(TOPRIGHT, 2);
                tileRule.Add(LEFT, 2);
                tileRule.Add(DOWN, 2);
                break;
            case 36:
                // 1:
                tileRule.Add(TOPLEFT, 1);
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT,  1);
                // 2:
                tileRule.Add(TOPRIGHT, 2);
                tileRule.Add(DOWN, 2);
                break;
            case 37:
                // 1:
                tileRule.Add(TOP, 1);
                tileRule.Add(TOPRIGHT, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                // 2:
                tileRule.Add(TOPLEFT, 2);
                tileRule.Add(DOWN, 2);
                break;
            case 38:
                // 1:
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 1);
                // 2:
                tileRule.Add(TOPLEFT, 2);
                tileRule.Add(RIGHT, 2);
                tileRule.Add(DOWN, 2);
                break;
            case 39:
                // 1:
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                // 2:
                tileRule.Add(TOPLEFT, 2);
                tileRule.Add(TOPRIGHT, 2);
                tileRule.Add(DOWN, 2);
                break;
            case 40:
                // 1:
                tileRule.Add(TOP, 1);
                tileRule.Add(TOPRIGHT, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 1);
                // 2:
                tileRule.Add(TOPLEFT, 2);
                tileRule.Add(DOWNLEFT, 2);
                tileRule.Add(DOWNRIGHT, 2);
                break;
            case 41:
                // 1:
                tileRule.Add(TOPLEFT, 1);
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 1);
                // 2:
                tileRule.Add(TOPRIGHT, 2);
                tileRule.Add(DOWNLEFT, 2);
                tileRule.Add(DOWNRIGHT, 2);
                break;

            case 42:
                // 1:
                tileRule.Add(TOP, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 1);
                // 2:
                tileRule.Add(TOPRIGHT, 2);
                tileRule.Add(LEFT, 2);
                tileRule.Add(DOWNRIGHT, 2);
                break;
            case 43:
                // 1:
                tileRule.Add(TOPLEFT, 1);
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWNLEFT, 1);
                tileRule.Add(DOWN, 1);
                // 2:
                tileRule.Add(TOPRIGHT, 2);
                tileRule.Add(DOWNRIGHT, 2);
                break;
            case 44:
                // 1:
                tileRule.Add(TOP, 1);
                tileRule.Add(TOPRIGHT, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 1);
                tileRule.Add(DOWNRIGHT, 1);
                // 2:
                tileRule.Add(TOPLEFT, 2);
                tileRule.Add(DOWNLEFT, 2);
                break;
            case 45:
                // 1:
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(DOWN, 1);
                // 2:
                tileRule.Add(TOPLEFT, 2);
                tileRule.Add(RIGHT, 2);
                tileRule.Add(DOWNLEFT, 2);
                break;
            case 46:
                // 1:
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 1);
                // 2:
                tileRule.Add(TOPLEFT, 2);
                tileRule.Add(TOPRIGHT, 2);
                tileRule.Add(DOWNLEFT, 2);
                tileRule.Add(DOWNRIGHT, 2);
                break;

            default:
                return null;
        }

        rule.ApplyNeighbors(tileRule);
        return rule;
    }

    [MenuItem("MPack/Import Wang Tile")]
    static private void ImportWangTile()
    {
        if (Selection.objects.Length != 1)
            return;

        Object file = Selection.objects[0];
        string path = AssetDatabase.GetAssetPath(file);
        Object[] spriteObjs = AssetDatabase.LoadAllAssetsAtPath(path);
        List<Sprite> sprites = new List<Sprite>();

        for (int i = 0; i < spriteObjs.Length; i++)
        {
            try
            {
                Sprite sprite = (Sprite)spriteObjs[i];
                sprites.Add(sprite);
            }
            catch (System.Exception) { }
        }

        sprites.Sort((sprite1, sprite2) =>
        {
            int dashIndex1 = sprite1.name.LastIndexOf("_");
            int dashIndex2 = sprite2.name.LastIndexOf("_");
            int index1 = int.Parse(sprite1.name.Substring(dashIndex1 + 1));
            int index2 = int.Parse(sprite2.name.Substring(dashIndex2 + 1));

            // // return 0;
            return index1.CompareTo(index2);
        });


        RuleTile ruleTile = ScriptableObject.CreateInstance<RuleTile>();
        ruleTile.m_DefaultSprite = sprites[0];

        for (int i = 0; i < sprites.Count; i++)
        {
            RuleTile.TilingRule rule = FindWangTileRule(i);
            if (rule != null)
            {
                rule.m_Sprites = new Sprite[] { sprites[i] };
                ruleTile.m_TilingRules.Add(rule);
            }
        }

        RuleTile.TilingRule first = ruleTile.m_TilingRules[0];
        ruleTile.m_TilingRules.RemoveAt(0);
        ruleTile.m_TilingRules.Add(first);

        AssetDatabase.CreateAsset(ruleTile, "Assets/NewWangTile.asset");
        AssetDatabase.SaveAssets();
    }

    static private RuleTile.TilingRule FindWangTileRule(int index) {
        RuleTile.TilingRule rule = new RuleTile.TilingRule();
        Dictionary<Vector3Int, int> tileRule = new Dictionary<Vector3Int, int>();

        switch (index) {
            case 0:
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 1);
                break;

            case 1:
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 1);
                tileRule.Add(DOWNRIGHT, 2);
                break;
            case 3:
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 1);
                tileRule.Add(DOWNLEFT, 2);
                break;
            case 11:
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 1);
                tileRule.Add(TOPRIGHT, 2);
                break;
            case 13:
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 1);
                tileRule.Add(TOPLEFT, 2);
                break;

            case 2:
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 2);
                break;
            case 6:
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 2);
                tileRule.Add(DOWN, 1);
                break;
            case 8:
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 2);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 1);
                break;
            case 12:
                tileRule.Add(TOP, 2);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 1);
                break;

            case 4:
                tileRule.Add(TOP, 2);
                tileRule.Add(LEFT, 2);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 1);
                tileRule.Add(TOPLEFT, 2);
                break;
            case 5:
                tileRule.Add(TOP, 2);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 2);
                tileRule.Add(DOWN, 1);
                tileRule.Add(TOPRIGHT, 2);
                break;
            case 9:
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 2);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 2);
                tileRule.Add(DOWNLEFT, 2);
                break;
            case 10:
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 2);
                tileRule.Add(DOWN, 2);
                tileRule.Add(DOWNRIGHT, 2);
                break;

            case 14:
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 1);
                tileRule.Add(TOPLEFT, 2);
                tileRule.Add(DOWNRIGHT, 2);
                break;
            case 15:
                tileRule.Add(TOP, 1);
                tileRule.Add(LEFT, 1);
                tileRule.Add(RIGHT, 1);
                tileRule.Add(DOWN, 1);
                tileRule.Add(TOPRIGHT, 2);
                tileRule.Add(DOWNLEFT, 2);
                break;
            
            case 7:
                return null;
        }

        rule.ApplyNeighbors(tileRule);
        return rule;
    }

    [MenuItem("MPack/Change Collider To Grid", true)]
    static private bool ValideChangeRuletileCollider()
    {
        if (Selection.objects.Length != 1)
            return false;

        Object file = Selection.objects[0];
        // Texture2D texture = (Texture2D) file;
        string path = AssetDatabase.GetAssetPath(file);
        RuleTile ruleTile = AssetDatabase.LoadAssetAtPath<RuleTile>(path);
        return ruleTile != null;
    }

    [MenuItem("MPack/Change Collider To Grid")]
    static private void ChangeRuletileCollider()
    {
        if (Selection.objects.Length != 1)
            return;

        Object file = Selection.objects[0];
        // Texture2D texture = (Texture2D) file;
        string path = AssetDatabase.GetAssetPath(file);
        RuleTile ruleTile = AssetDatabase.LoadAssetAtPath<RuleTile>(path);

        for (int i = 0; i < ruleTile.m_TilingRules.Count; i++)
        {
            ruleTile.m_TilingRules[i].m_ColliderType = Tile.ColliderType.Grid;
        }

        // AssetDatabase.dir
        AssetDatabase.SaveAssets();
    }
}
#endif