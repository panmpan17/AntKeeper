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
    private Sprite emptySprite;
    [SerializeField]
    private Sprite oneAntSprite;
    [SerializeField]
    private Sprite twoAntSprite;

    [SerializeField]
    private PromptItem needTwoNests;
    [SerializeField]
    private PromptItem twoTypesOfAnt;

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
    }

    void Update()
    {
        if (collectTimer.Running)
        {
            if (_targetAntNest == null || !_targetAntNest.enabled)
            {
                collectTimer.Running = false;
                PlayerBehaviour.Input.enabled = true;
                PlayerBehaviour.ProgressBar.gameObject.SetActive(false);
                particle.Stop();
                return;
            }

            if (collectTimer.UpdateEnd)
            {
                collectTimer.Running = false;
                PlayerBehaviour.Input.enabled = true;

                PlayerBehaviour.ProgressBar.gameObject.SetActive(false);
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

            PlayerBehaviour.ProgressBar.SetFillAmount(collectTimer.Progress);
        }

        if (plantTimer.Running)
        {
            if (plantTimer.UpdateEnd)
            {
                plantTimer.Running = false;

                PlayerBehaviour.Input.enabled = true;

                PlayerBehaviour.ProgressBar.gameObject.SetActive(false);
                particle.Stop();

                PlantAnt();

                _state = State.Empty;
                _firstAntNest = null;
                _targetAntNest = null;
                _spriteRenderer.sprite = emptySprite;
                return;
            }

            PlayerBehaviour.ProgressBar.SetFillAmount(plantTimer.Progress);
        }
    }

    public override void OnDash()
    {
        // throw new System.NotImplementedException();
    }

    public override void OnInteractStart()
    {
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
            PlayerBehaviour.ProgressBar.gameObject.SetActive(true);
            particle.transform.position = PlayerBehaviour.SelectedGridCenterPosition;
            particle.Play();
        }
    }

    void OneAntJarCheck()
    {
        if (GridManager.ins.TryFindAntNest(PlayerBehaviour.SelectedGridPosition, out AntNestHub overlapNest))
        {
            if (overlapNest == _firstAntNest)
            {
                needTwoNests.Show();
                return;
            }
            if (!overlapNest.enabled)
                return;

            _targetAntNest = overlapNest;
            PlayerBehaviour.Input.enabled = false;

            collectTimer.Reset();
            PlayerBehaviour.ProgressBar.gameObject.SetActive(true);
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
        PlayerBehaviour.ProgressBar.gameObject.SetActive(true);
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
        if (_firstIsFireAnt != _secondIsFireAnt)
        {
            twoTypesOfAnt.Show();
            return;
        }
        
        StatisticTracker.ins.AddBreedAntRecord(_firstIsFireAnt);
        GridManager.ins.InstantiateAntNestOnGridWithoutChecking(PlayerBehaviour.SelectedGridPosition, _firstAntNest.IsFireAnt);
    }
}
