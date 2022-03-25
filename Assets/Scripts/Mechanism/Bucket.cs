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
            _currentFillAmount = value;

            _spriteRenderer.sprite = value > maxFillAmount * 0.9f ? fullBucket : emptyBucket;
        }
    }
    public bool IsFull => _currentFillAmount >= maxFillAmount;

    [SerializeField]
    private float pourSpeed;
    [SerializeField]
    private FillBarControl fillBar;
    private bool _pouring;

    [SerializeField]
    private float pouringRotationg;

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
        FillAmount = maxFillAmount;
    }

    void Update()
    {
        if (_pouring)
        {
            FillAmount -= pourSpeed * Time.deltaTime;
            if (FillAmount == 0)
            {
                _pouring = false;
                pourEffect.Stop();
            }
        }
    }

    public override void OnInteract() => throw new System.NotImplementedException();

    public override void OnInteractStart()
    {
        if (GridManager.ins.TryFindGroundInteractive(PlayerBahviour.SelectedGridPosition, out AbstractGroundInteractive groundInteractive))
        {
            groundInteractive.OnHoldItemInteract(this);
            return;
        }

        if (FillAmount > 0)
        {
            _pouring = true;
            transform.rotation = Quaternion.Euler(0, 0, pouringRotationg);
            pourEffect.transform.position = PlayerBahviour.SelectedGridCenterPosition;
            pourEffect.Play();
        }
    }

    public override void OnInteractEnd()
    {
        _pouring = false;
        transform.rotation = Quaternion.identity;
        pourEffect.Stop();
    }
}

[System.Serializable]
public struct Damage
{
    public float Amount;
    public Timer InteractTimer;
}