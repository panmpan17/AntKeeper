using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [Header("Sprites")]
    [SerializeField]
    private Sprite faceUpSprite;
    [SerializeField]
    private Sprite faceDownSprite;
    [SerializeField]
    private Sprite faceRightSprite;
    [SerializeField]
    private Sprite faceLeftSprite;

    private PlayerMovement _movement;

    void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _movement.OnFacingChange += ChangeFacingAnimation;
    }

    public void ChangeFacingAnimation(Facing newFacing)
    {
        switch (newFacing)
        {
            case Facing.Right:
                spriteRenderer.sprite = faceRightSprite;
                break;
            case Facing.Left:
                spriteRenderer.sprite = faceLeftSprite;
                break;
            case Facing.Up:
                spriteRenderer.sprite = faceUpSprite;
                break;
            case Facing.Down:
                spriteRenderer.sprite = faceDownSprite;
                break;
        }
    }
}
