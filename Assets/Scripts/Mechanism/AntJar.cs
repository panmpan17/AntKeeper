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

    [SerializeField]
    private PromptItem alreadyExamine;

    [Header("Collect")]
    [SerializeField]
    private ParticleSystem particle;
    [SerializeField]
    [ShortTimer]
    private Timer collectTimer;

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
        // collectProgressBar.gameObject.SetActive(false);
    }

    void Update()
    {
        if (collectTimer.Running)
        {
            if (collectTimer.UpdateEnd)
            {
                PlayerBehaviour.Input.enabled = true;

                collectTimer.Running = false;
                PlayerBehaviour.ProgressBar.gameObject.SetActive(false);
                particle.Stop();
                _spriteRenderer.sprite = jarHasAntSprite;
                return;
            }

            PlayerBehaviour.ProgressBar.SetFillAmount(collectTimer.Progress);
        }
    }

    public override void OnDash()
    {
        // throw new System.NotImplementedException();
    }

    public override void OnInteractStart()
    {
        if (GridManager.ins.TryFindAntNestBranch(PlayerBehaviour.SelectedGridPosition, out AntNestHub targetNest, out AntRouteBranch targetBranch))
        {
            if (HasAnt)
                return;

            Debug.Log(preventRepeat);
            Debug.Log(targetNest.IsShowTrueColor);
            if (preventRepeat && targetNest.IsShowTrueColor)
            {
                alreadyExamine.Show();
                return;
            }

            _targetAntNest = targetNest;
            PlayerBehaviour.Input.enabled = false;

            collectTimer.Reset();
            PlayerBehaviour.ProgressBar.gameObject.SetActive(true);
            particle.Play();
            particle.transform.position = PlayerBehaviour.SelectedGridCenterPosition;
        }
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
