using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


public class QueenAntJar : AbstractHoldItem
{
    [SerializeField]
    private ParticleSystem particle;
    [SerializeField]
    private Timer collectTimer;
    [SerializeField]
    private Timer plantTimer;
    [SerializeField]
    private FillBarControl progressBar;

    [SerializeField]
    private Sprite emptySprite;
    [SerializeField]
    private Sprite oneAntSprite;
    [SerializeField]
    private Sprite twoAntSprite;
    // [SerializeField]
    // private Color emptyJarColor;
    // [SerializeField]
    // private Color oneAntJarColor;
    // [SerializeField]
    // private Color twoAntJarColor;

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
        _spriteRenderer.sprite = emptySprite;

        // _spriteRenderer.sprite = emptyJarSprite;

        collectTimer.Running = false;
        plantTimer.Running = false;
        progressBar.gameObject.SetActive(false);
    }

    void Update()
    {
        if (collectTimer.Running)
        {
            if (_targetAntNest == null || !_targetAntNest.enabled)
            {
                collectTimer.Running = false;
                PlayerBehaviour.Input.enabled = true;
                progressBar.gameObject.SetActive(false);
                particle.Stop();
                return;
            }

            if (collectTimer.UpdateEnd)
            {
                collectTimer.Running = false;
                PlayerBehaviour.Input.enabled = true;

                progressBar.gameObject.SetActive(false);
                particle.Stop();

                if (_state == State.Empty)
                {
                    _state = State.OneAnt;
                    _spriteRenderer.sprite = oneAntSprite;
                    _firstAntNest = _targetAntNest;
                    _firstIsFireAnt = _firstAntNest.IsFireAnt;
                }
                else
                {
                    _state = State.TwoAnt;
                    _spriteRenderer.sprite = twoAntSprite;
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
                particle.Stop();

                PlantAnt();

                _state = State.Empty;
                _firstAntNest = null;
                _targetAntNest = null;
                _spriteRenderer.sprite = emptySprite;
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
        if (GridManager.ins.TryFindAntNest(PlayerBehaviour.SelectedGridPosition, out AntNestHub overlapNest))
        {
            if (!overlapNest.enabled)
                return;

            _targetAntNest = overlapNest;
            PlayerBehaviour.Input.enabled = false;

            collectTimer.Reset();
            progressBar.gameObject.SetActive(true);
            particle.transform.position = PlayerBehaviour.SelectedGridCenterPosition;
            particle.Play();
        }
    }

    void OneAntJarCheck()
    {
        if (GridManager.ins.TryFindAntNest(PlayerBehaviour.SelectedGridPosition, out AntNestHub overlapNest))
        {
            if (overlapNest == _firstAntNest)
                return;
            if (!overlapNest.enabled)
                return;

            _targetAntNest = overlapNest;
            PlayerBehaviour.Input.enabled = false;

            collectTimer.Reset();
            progressBar.gameObject.SetActive(true);
            particle.transform.position = PlayerBehaviour.SelectedGridCenterPosition;
            particle.Play();
        }
    }

    void TwoAntJarCheck()
    {
        if (GridManager.ins.TryFindAntNestBranch(PlayerBehaviour.SelectedGridPosition))
            return;

        PlayerBehaviour.Input.enabled = false;

        plantTimer.Reset();
        progressBar.gameObject.SetActive(true);
        particle.transform.position = PlayerBehaviour.SelectedGridCenterPosition;
        particle.Play();
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
