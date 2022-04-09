using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualAnimalSpot : MonoBehaviour
{
    [SerializeField]
    protected Sprite emptySprite;
    [SerializeField]
    protected float health;

    protected SpriteRenderer _spriteRenderer;
    public Vector3Int GridPosition { get; protected set; }

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        GridManager.ins.ReigsterAnimal(this, out Vector3Int griPosition);
        GridPosition = griPosition;
    }

    public void Kill()
    {
        _spriteRenderer.sprite = emptySprite;
    }
}
