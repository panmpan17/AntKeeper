using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


public class AntJar : AbstractHoldItem
{
    [SerializeField]
    private bool preventRepeat = true;
    [SerializeField]
    [ShortTimer]
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
                PlayerBehaviour.Input.enabled = true;

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

    public override bool OnInteractStart()
    {
        if (GridManager.ins.TryFindGroundInteractive(PlayerBehaviour.SelectedGridPosition, out AbstractGroundInteractive groundInteractive))
        {
            return groundInteractive.OnHoldItemInteract(this);
        }

        if (GridManager.ins.TryFindAntNestBranch(PlayerBehaviour.SelectedGridPosition, out AntNest targetNest, out AntRouteBranch targetBranch))
        {
            if (HasAnt)
                return false;

            if (preventRepeat && targetNest.IsShowTrueColor)
                return false;

            _targetAntNest = targetNest;
            PlayerBehaviour.Input.enabled = false;

            collectTimer.Reset();
            collectProgressBar.gameObject.SetActive(true);
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
            _targetAntNest.ShowTrueColor();

        _targetAntNest = null;
        _spriteRenderer.sprite = emptyJarSprite;
    }
}
