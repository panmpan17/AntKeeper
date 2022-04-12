using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


public class AntJar : AbstractHoldItem
{
    [SerializeField]
    private Timer collectTimer;
    [SerializeField]
    private FillBarControl collectProgressBar;

    [SerializeField]
    private Sprite jarHasAntSprite;
    [SerializeField]
    private Sprite emptyJarSprite;

    private SpriteRenderer _spriteRenderer;
    private AntNest _targetAntNest;

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
                collectTimer.Running = false;
                collectProgressBar.gameObject.SetActive(false);
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

    public override void OnInteract() => throw new System.NotImplementedException();

    public override void OnInteractStart()
    {
        if (GridManager.ins.TryFindGroundInteractive(PlayerBehaviour.SelectedGridPosition, out AbstractGroundInteractive groundInteractive))
        {
            groundInteractive.OnHoldItemInteract(this);
            return;
        }

        if (GridManager.ins.TryFindAntNestBranch(PlayerBehaviour.SelectedGridPosition, out AntNest targetNest, out AntRouteBranch targetBranch))
        {
            _targetAntNest = targetNest;
            collectTimer.Reset();
            collectProgressBar.gameObject.SetActive(true);
            // PlayerBehaviour.
        }
    }

    public override void OnInteractEnd()
    {
    }

    public void ShowAntNestTrueColor()
    {
        _targetAntNest.ShowTrueColor();

        _targetAntNest = null;
        _spriteRenderer.sprite = emptyJarSprite;
    }
}
