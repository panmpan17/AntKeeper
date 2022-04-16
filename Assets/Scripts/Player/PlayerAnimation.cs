using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public const float UpRotation = 0;
    public const float RightRotation = (90f / 180f) * Mathf.PI;
    public const float DownRotation = Mathf.PI;
    public const float LeftRotation = (270f / 180f) * Mathf.PI;

    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private ParticleSystem footStepParticle;
    private ParticleSystem.MainModule footStepParticleMain;
    [SerializeField]
    private TrailRenderer dashTrail;

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
        _movement.OnWalkStarted += OnWalkStarted;
        _movement.OnWalkEnded += OnWalkEnded;
        _movement.OnDashPerformed += OnDashStarted;
        _movement.OnDashEnded += OnDashEnded;

        footStepParticleMain = footStepParticle.main;
    }


    public void ChangeFacingAnimation()
    {
        ChangeFacingAnimation(_movement.Facing);
    }
    public void ChangeFacingAnimation(Facing newFacing)
    {
        switch (newFacing)
        {
            case Facing.Right:
                spriteRenderer.sprite = faceRightSprite;
                footStepParticleMain.startRotation = RightRotation;
                break;
            case Facing.Left:
                spriteRenderer.sprite = faceLeftSprite;
                footStepParticleMain.startRotation = LeftRotation;
                break;
            case Facing.Up:
                spriteRenderer.sprite = faceUpSprite;
                footStepParticleMain.startRotation = UpRotation;
                break;
            case Facing.Down:
                spriteRenderer.sprite = faceDownSprite;
                footStepParticleMain.startRotation = DownRotation;
                break;
        }
    }

    void OnWalkStarted()
    {
        footStepParticle.Play();
    }

    void OnWalkEnded()
    {
        footStepParticle.Stop();
    }

    void OnDashStarted()
    {
        dashTrail.emitting = true;
    }

    void OnDashEnded()
    {
        dashTrail.emitting = false;
    }
}
