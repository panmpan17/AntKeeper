using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;
using System.Text;

public class PlayerAnimation : MonoBehaviour
{
    public const string RightPrefix = "player_right_";
    public const string LeftPrefix = "player_left_";
    public const string UpPrefix = "player_up_";
    public const string DownPrefix = "player_down_";

    public const float UpRotation = 0;
    public const float RightRotation = (90f / 180f) * Mathf.PI;
    public const float DownRotation = Mathf.PI;
    public const float LeftRotation = (270f / 180f) * Mathf.PI;

    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private ParticleSystem footStepParticle;
    private ParticleSystem.MainModule footStepParticleMain;

    [Header("Dash")]
    [SerializeField]
    private FloatLerpTimer spritePositionTimer;
    [SerializeField]
    private AnimationCurveVariable positionCurve;
    [SerializeField]
    private TrailRenderer dashTrail;

    [Header("Sprites")]
    [SerializeField]
    [HideInInspector]
    private Sprite faceUpSprite;
    [SerializeField]
    [HideInInspector]
    private Sprite faceDownSprite;
    [SerializeField]
    [HideInInspector]
    private Sprite faceRightSprite;
    [SerializeField]
    [HideInInspector]
    private Sprite faceLeftSprite;

    private PlayerBehaviour _behaviour;
    private PlayerMovement _movement;

    private Animator _animator;

    private string _animationKey;

    void Awake()
    {
        _behaviour = GetComponent<PlayerBehaviour>();
        _behaviour.OnHoldItemChanged += OnHoldItemChange;

        _movement = GetComponent<PlayerMovement>();
        _movement.OnFacingChange += ChangeFacingAnimation;
        _movement.OnWalkStarted += OnWalkStarted;
        _movement.OnWalkEnded += OnWalkEnded;
        _movement.OnDashPerformed += OnDashStarted;
        _movement.OnDashEnded += OnDashEnded;

        _animator = GetComponentInChildren<Animator>();

        footStepParticleMain = footStepParticle.main;
    }

    void Start()
    {
        SwitchToAnimation("idle");
    }

    void Update()
    {
        if (spritePositionTimer.Timer.Running)
        {
            if (spritePositionTimer.Timer.UpdateEnd)
            {
                spritePositionTimer.Timer.Running = false;
            }
            spriteRenderer.transform.localPosition = new Vector3(0, spritePositionTimer.CurvedValue(positionCurve.Value), 0);
        }
    }

    public void ChangeFacingAnimation()
    {
        ChangeFacingAnimation(_movement.Facing);
    }
    public void ChangeFacingAnimation(Facing newFacing)
    {
        switch (newFacing)
        {
            case Facing.Up:
                spriteRenderer.sprite = faceUpSprite;
                footStepParticleMain.startRotation = UpRotation;
                SwitchToAnimationWithFacingPrefix(UpPrefix);
                break;
            case Facing.Down:
                spriteRenderer.sprite = faceDownSprite;
                footStepParticleMain.startRotation = DownRotation;
                SwitchToAnimationWithFacingPrefix(DownPrefix);
                break;
            case Facing.Right:
                spriteRenderer.sprite = faceRightSprite;
                footStepParticleMain.startRotation = RightRotation;
                SwitchToAnimationWithFacingPrefix(RightPrefix);
                break;
            case Facing.Left:
                spriteRenderer.sprite = faceLeftSprite;
                footStepParticleMain.startRotation = LeftRotation;
                SwitchToAnimationWithFacingPrefix(LeftPrefix);
                break;
        }

        // SwitchToAnimation();
    }

    void OnWalkStarted()
    {
        footStepParticle.Play();
        SwitchToAnimation("walk");
    }

    void OnWalkEnded()
    {
        footStepParticle.Stop();
        SwitchToAnimation("idle");
    }

    void OnDashStarted()
    {
        dashTrail.emitting = true;
        spritePositionTimer.Timer.Reset();
    }

    void OnDashEnded()
    {
        dashTrail.emitting = false;
    }

    void OnHoldItemChange()
    {
        SwitchToAnimation();
    }

    void SwitchToAnimation(string animationKey)
    {
        _animationKey = animationKey;
        SwitchToAnimation();
    }

    void SwitchToAnimation()
    {
        switch (_movement.Facing)
        {
            case Facing.Up:
                SwitchToAnimationWithFacingPrefix(UpPrefix);
                break;
            case Facing.Down:
                SwitchToAnimationWithFacingPrefix(DownPrefix);
                break;
            case Facing.Right:
                SwitchToAnimationWithFacingPrefix(RightPrefix);
                break;
            case Facing.Left:
                SwitchToAnimationWithFacingPrefix(LeftPrefix);
                break;
        }
    }

    void SwitchToAnimationWithFacingPrefix(string prefix)
    {
        var builder = new StringBuilder(prefix);

        if (_behaviour.IsHolding)
            builder.Append("h_");

        builder.Append(_animationKey);

        _animator.Play(builder.ToString());
    }
}
