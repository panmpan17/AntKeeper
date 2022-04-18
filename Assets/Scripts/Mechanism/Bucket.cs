using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class Bucket : AbstractHoldItem
{
    [SerializeField]
    private Damage damage;

    [Header("Fill and pour setting")]
    [SerializeField]
    private float maxFillAmount;
    private float _currentFillAmount;
    public float FillAmount {
        get => _currentFillAmount;
        set {
            value = Mathf.Clamp(value, 0, maxFillAmount);
            fillBar.SetFillAmount(value / maxFillAmount);

            bool wasUseFullBucket = UseFullBucketSprite;
            _currentFillAmount = value;

            if (wasUseFullBucket != UseFullBucketSprite)
            {
                ChangeBucketFullSprite();
            }
        }
    }
    public bool IsFull => _currentFillAmount >= maxFillAmount;
    public bool UseFullBucketSprite => _currentFillAmount > maxFillAmount * 0.9f;

    [SerializeField]
    private float pourSpeed;
    [SerializeField]
    private FillBarControl fillBar;
    private Vector3 _fillBarLocalPosition;
    private Vector3 _fillBarLocalScale;
    [SerializeField]
    private float killSpeed;
    private bool _pouring;

    [Header("Pour animation")]
    [SerializeField]
    private PourAnimation faceUpPour;
    [SerializeField]
    private PourAnimation faceDownPour;
    [SerializeField]
    private PourAnimation faceRightPour;
    [SerializeField]
    private PourAnimation faceLeftPour;


    [Header("Reference")]
    [SerializeField]
    private Sprite emptyBucket;
    [SerializeField]
    private Sprite fullBucket;
    [SerializeField]
    private ParticleSystem pourEffect;

    private SpriteRenderer _spriteRenderer;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _fillBarLocalPosition = fillBar.transform.localPosition;
        _fillBarLocalScale = fillBar.transform.localScale;
    }

    void Start()
    {
        FillAmount = maxFillAmount;
    }

    void Update()
    {
        if (_pouring)
        {
            FillAmount -= pourSpeed * Time.deltaTime;
            if (FillAmount == 0)
                PourEnd();

            if (GridManager.ins.TryFindAntNestBranch(PlayerBehaviour.SelectedGridPosition, out AntNest antNest, out AntRouteBranch branch))
            {
                antNest.TryKillSpot(PlayerBehaviour.SelectedGridPosition, killSpeed * Time.deltaTime, branch);
            }
        }
    }

    public override bool OnInteractStart()
    {
        if (GridManager.ins.TryFindGroundInteractive(PlayerBehaviour.SelectedGridPosition, out AbstractGroundInteractive groundInteractive))
        {
            return groundInteractive.OnHoldItemInteract(this);
        }

        if (FillAmount > 0)
            PourStart();

        return false;
    }

    public override void OnInteractEnd()
    {
        if (_pouring)
            PourEnd();
    }


    #region Player movement event
    public override void OnSelectedGridChanged()
    {
        pourEffect.transform.position = PlayerBehaviour.SelectedGridCenterPosition;
    }

    public override void OnFacingChanged()
    {
        if (_pouring)
        {
            ApplyPourAnimation();
        }
    }
    public override void OnDash()
    {
        if (_pouring)
            PourEnd();
    }
    #endregion


    public override void ChangeRendererSorting(int layerID, int order)
    {
        _spriteRenderer.sortingLayerID = layerID;
        _spriteRenderer.sortingOrder = order;
    }

    void PourStart()
    {
        _pouring = true;

        ApplyPourAnimation();
        pourEffect.transform.position = PlayerBehaviour.SelectedGridCenterPosition;
        pourEffect.Play();
    }

    void PourEnd()
    {
        _pouring = false;
        transform.rotation = Quaternion.identity;
        pourEffect.Stop();
        ChangeBucketFullSprite();


        fillBar.transform.localPosition = _fillBarLocalPosition;
        fillBar.transform.localScale = _fillBarLocalScale;
    }


    #region Animation sprite
    void ApplyPourAnimation()
    {
        switch (PlayerBehaviour.Movement.Facing)
        {
            case Facing.Up:
                ApplyPourAnimation(faceUpPour);
                break;
            case Facing.Down:
                ApplyPourAnimation(faceDownPour);
                break;
            case Facing.Right:
                ApplyPourAnimation(faceRightPour);
                break;
            case Facing.Left:
                ApplyPourAnimation(faceLeftPour);
                break;
        }
    }

    void ApplyPourAnimation(PourAnimation pourAnimation)
    {
        switch (pourAnimation.Type)
        {
            case PourAnimation.TransitionType.Rotation:
                transform.rotation = Quaternion.Euler(0, 0, pourAnimation.Rotation);
                _spriteRenderer.sprite = UseFullBucketSprite ? fullBucket : emptyBucket;
                break;
            case PourAnimation.TransitionType.Sprite:
                transform.rotation = Quaternion.identity;
                _spriteRenderer.sprite = UseFullBucketSprite ? pourAnimation.FullSprite : pourAnimation.EmptySprite;
                break;
        }

        if (pourAnimation.ChangeFillBarTransform)
        {
            fillBar.transform.localPosition = pourAnimation.LocalPosition;
            fillBar.transform.localScale = pourAnimation.LocalScale;
        }
        else
        {
            fillBar.transform.localPosition = _fillBarLocalPosition;
            fillBar.transform.localScale = _fillBarLocalScale;
        }
    }

    void ChangeBucketFullSprite()
    {
        if (UseFullBucketSprite)
        {
            if (_pouring)
            {
                if (CheckSpriteOvrewrite(out PourAnimation pourAnimation))
                {
                    _spriteRenderer.sprite = pourAnimation.FullSprite;
                    return;
                }
            }
            _spriteRenderer.sprite = fullBucket;
        }
        else
        {
            if (_pouring)
            {
                if (CheckSpriteOvrewrite(out PourAnimation pourAnimation))
                {
                    _spriteRenderer.sprite = pourAnimation.EmptySprite;
                    return;
                }
            }
            _spriteRenderer.sprite = emptyBucket;
        }
    }

    bool CheckSpriteOvrewrite(out PourAnimation pourAnimation)
    {
        switch (PlayerBehaviour.Movement.Facing)
        {
            case Facing.Up:
                if (faceUpPour.Type == PourAnimation.TransitionType.Sprite)
                {
                    pourAnimation = faceUpPour;
                    return true;
                }
                break;
            case Facing.Down:
                if (faceDownPour.Type == PourAnimation.TransitionType.Sprite)
                {
                    pourAnimation = faceDownPour;
                    return true;
                }
                break;
            case Facing.Right:
                if (faceRightPour.Type == PourAnimation.TransitionType.Sprite)
                {
                    pourAnimation = faceRightPour;
                    return true;
                }
                break;
            case Facing.Left:
                if (faceLeftPour.Type == PourAnimation.TransitionType.Sprite)
                {
                    pourAnimation = faceLeftPour;
                    return true;
                }
                break;
        }

        pourAnimation = new PourAnimation();
        return false;
    }

    [System.Serializable]
    public struct PourAnimation
    {
        public TransitionType Type;
        public float Rotation;
        public Sprite FullSprite;
        public Sprite EmptySprite;

        public bool ChangeFillBarTransform;
        public Vector3 LocalPosition;
        public Vector3 LocalScale;

        public enum TransitionType { Rotation, Sprite }
    }
    #endregion
}

[System.Serializable]
public struct Damage
{
    public float Amount;
    public Timer InteractTimer;
}