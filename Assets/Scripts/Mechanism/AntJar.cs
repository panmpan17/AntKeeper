using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


public class AntJar : AbstractHoldItem
{
    [SerializeField]
    private bool preventRepeat = true;
    [SerializeField]
    private Sprite jarHasAntSprite;
    [SerializeField]
    private Sprite emptyJarSprite;

    [Header("Collect")]
    [SerializeField]
    private ParticleSystem particle;
    [SerializeField]
    [ShortTimer]
    private Timer collectTimer;
    [SerializeField]
    private FillBarControl collectProgressBar;

    [Header("Reveal")]
    [SerializeField]
    private ParticleSystem revealTrailParticle;
    [SerializeField]
    private Timer revealTimer;
    [SerializeField]
    private AnimationCurve trailCurve;

    private SpriteRenderer _spriteRenderer;
    private AntNestHub _targetAntNest;

    public bool HasAnt => _targetAntNest != null;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _spriteRenderer.sprite = emptyJarSprite;

        collectTimer.Running = false;
        collectProgressBar.gameObject.SetActive(false);
    }

    void Update()
    {
        if (collectTimer.Running)
        {
            if (collectTimer.UpdateEnd)
            {
                PlayerBehaviour.Input.enabled = true;

                collectTimer.Running = false;
                collectProgressBar.gameObject.SetActive(false);
                particle.Stop();
                _spriteRenderer.sprite = jarHasAntSprite;
                return;
            }

            collectProgressBar.SetFillAmount(collectTimer.Progress);
        }
    }

    public override void OnDash()
    {
        // throw new System.NotImplementedException();
    }

    public override bool OnInteractStart()
    {
        if (GridManager.ins.TryFindGroundInteractive(PlayerBehaviour.SelectedGridPosition, out AbstractGroundInteractive groundInteractive))
        {
            return groundInteractive.OnHoldItemInteract(this);
        }

        if (GridManager.ins.TryFindAntNestBranch(PlayerBehaviour.SelectedGridPosition, out AntNestHub targetNest, out AntRouteBranch targetBranch))
        {
            if (HasAnt)
                return false;

            if (preventRepeat && targetNest.IsShowTrueColor)
                return false;

            _targetAntNest = targetNest;
            PlayerBehaviour.Input.enabled = false;

            collectTimer.Reset();
            collectProgressBar.gameObject.SetActive(true);
            particle.Play();
            particle.transform.position = PlayerBehaviour.SelectedGridCenterPosition;
        }

        return false;
    }

    public override void OnInteractEnd() {}

    public override void OnSelectedGridChanged() {}
    public override void OnFacingChanged() {}

    public override void ChangeRendererSorting(int layerID, int order)
    {
        _spriteRenderer.sortingLayerID = layerID;
        _spriteRenderer.sortingOrder = order;
    }

    public void ShowAntNestTrueColor()
    {
        if (_targetAntNest != null)
        {
            StartCoroutine(FlyTo(_targetAntNest));
        }

        _targetAntNest = null;
        _spriteRenderer.sprite = emptyJarSprite;
    }

    IEnumerator FlyTo(AntNestHub hub)
    {
        Vector3 startPosition = transform.position;
        revealTrailParticle.transform.position = startPosition;
        revealTrailParticle.Play();
        revealTimer.Reset();

        while (!revealTimer.UpdateEnd)
        {
            revealTrailParticle.transform.position = Vector3.Lerp(startPosition, hub.transform.position, trailCurve.Evaluate(revealTimer.Progress));
            yield return null;
        }
        revealTrailParticle.Stop();
        hub.ShowTrueColor();
    }
}
