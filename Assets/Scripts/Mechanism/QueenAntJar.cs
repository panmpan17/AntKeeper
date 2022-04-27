using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


public class QueenAntJar : AbstractHoldItem
{
    [SerializeField]
    private Timer collectTimer;
    [SerializeField]
    private Timer plantTimer;
    [SerializeField]
    private FillBarControl progressBar;

    // [SerializeField]
    // private Sprite emptyJarSprite;
    // [SerializeField]
    // private Sprite oneAntJarSprite;
    // [SerializeField]
    // private Sprite twoAntJarSprite;
    [SerializeField]
    private Color emptyJarColor;
    [SerializeField]
    private Color oneAntJarColor;
    [SerializeField]
    private Color twoAntJarColor;

    private enum State {
        Empty,
        OneAnt,
        TwoAnt,
    }
    private State _state;

    private SpriteRenderer _spriteRenderer;
    private bool _firstIsFireAnt;
    private bool _secondIsFireAnt;
    private AntNestHub _firstAntNest;
    private AntNestHub _targetAntNest;

    public bool HasAnt => _targetAntNest != null;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.color = emptyJarColor;

        // _spriteRenderer.sprite = emptyJarSprite;

        collectTimer.Running = false;
        plantTimer.Running = false;
        progressBar.gameObject.SetActive(false);
    }

    void Update()
    {
        if (collectTimer.Running)
        {
            if (_targetAntNest == null)
            {
                collectTimer.Running = false;
                PlayerBehaviour.Input.enabled = true;
                progressBar.gameObject.SetActive(false);
                return;
            }

            if (collectTimer.UpdateEnd)
            {
                collectTimer.Running = false;
                PlayerBehaviour.Input.enabled = true;

                progressBar.gameObject.SetActive(false);

                if (_state == State.Empty)
                {
                    _state = State.OneAnt;
                    _spriteRenderer.color = oneAntJarColor;
                    _firstAntNest = _targetAntNest;
                    _firstIsFireAnt = _firstAntNest.IsFireAnt;
                }
                else
                {
                    _state = State.TwoAnt;
                    _spriteRenderer.color = twoAntJarColor;
                    _secondIsFireAnt = _targetAntNest.IsFireAnt;
                }

                return;
            }

            progressBar.SetFillAmount(collectTimer.Progress);
        }

        if (plantTimer.Running)
        {
            if (plantTimer.UpdateEnd)
            {
                plantTimer.Running = false;

                PlayerBehaviour.Input.enabled = true;

                progressBar.gameObject.SetActive(false);

                PlantAnt();

                _state = State.Empty;
                _firstAntNest = null;
                _targetAntNest = null;
                _spriteRenderer.color = emptyJarColor;
                return;
            }

            progressBar.SetFillAmount(plantTimer.Progress);
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

        switch (_state)
        {
            case State.Empty:
                EmptyJarCheck();
                break;

            case State.OneAnt:
                OneAntJarCheck();
                break;

            case State.TwoAnt:
                TwoAntJarCheck();
                break;
        }

        return false;
    }

    void EmptyJarCheck()
    {
        if (GridManager.ins.TryFindAntNest(PlayerBehaviour.SelectedGridPosition, out AntNestHub targetNest))
        {
            _targetAntNest = targetNest;
            PlayerBehaviour.Input.enabled = false;

            collectTimer.Reset();
            progressBar.gameObject.SetActive(true);
        }
    }

    void OneAntJarCheck()
    {
        if (GridManager.ins.TryFindAntNest(PlayerBehaviour.SelectedGridPosition, out AntNestHub targetNest))
        {
            if (targetNest == _firstAntNest)
                return;

            _targetAntNest = targetNest;
            PlayerBehaviour.Input.enabled = false;

            collectTimer.Reset();
            progressBar.gameObject.SetActive(true);
        }
    }

    void TwoAntJarCheck()
    {
        if (GridManager.ins.TryFindAntNestBranch(PlayerBehaviour.SelectedGridPosition))
            return;

        PlayerBehaviour.Input.enabled = false;

        plantTimer.Reset();
        progressBar.gameObject.SetActive(true);
    }

    public override void OnInteractEnd() { }

    public override void OnSelectedGridChanged() { }
    public override void OnFacingChanged() { }

    public override void ChangeRendererSorting(int layerID, int order)
    {
        _spriteRenderer.sortingLayerID = layerID;
        _spriteRenderer.sortingOrder = order;
    }

    public void PlantAnt()
    {
        // Debug.LogFormat("plant ant {0} at {1}", _firstAntNest.IsFireAnt, PlayerBehaviour.SelectedGridPosition);
        if (_firstIsFireAnt != _secondIsFireAnt)
            return;
        
        GridManager.ins.InstantiateAntNestOnGridWithoutChecking(PlayerBehaviour.SelectedGridPosition, _firstAntNest.IsFireAnt);
    }

    // public void ShowAntNestTrueColor()
    // {
    //     _targetAntNest.ShowTrueColor();

    //     _targetAntNest = null;
    //     _spriteRenderer.sprite = emptyJarSprite;
    // }
}
