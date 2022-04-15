using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu]
public class TilemapReference : ScriptableObject
{
    public Tilemap Tilemap;
    public RuleTile ColliderTile;
}
