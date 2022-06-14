using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualAnimalSpot : MonoBehaviour
{
    [SerializeField]
    protected Sprite emptySprite;
    [SerializeField]
    protected float health;

    public bool IsAlive { get; protected set; }

    protected SpriteRenderer _spriteRenderer;
    public Vector3Int GridPosition { get; protected set; }

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        IsAlive = true;
    }

    void Start()
    {
        GridManager.ins.ReigsterAnimal(this, out Vector3Int griPosition);
        GridPosition = griPosition;
    }

    public void Kill()
    {
        _spriteRenderer.sprite = emptySprite;
        IsAlive = false;

        HUDManager.ins?.UpdateAnimalCount();
    }

    public void ChangePosition(Vector3 position, Vector3Int gridPosition)
    {
        transform.position = position;
        GridPosition = gridPosition;
    }
}
