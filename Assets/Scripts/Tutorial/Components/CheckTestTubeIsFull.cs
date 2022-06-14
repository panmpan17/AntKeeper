using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MPack;

public class CheckTestTubeIsFull : MonoBehaviour
{
    [SerializeField]
    private AbstractTutorialStep step;
    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    [LauguageID]
    private int formatLanguageID;
    [SerializeField]
    private QueenAntJar.State requireState;

    private PlayerBehaviour _playerBehaviour;
    private QueenAntJar _testTube;

    void OnEnable()
    {
        _playerBehaviour = GameObject.FindWithTag(PlayerBehaviour.Tag).GetComponent<PlayerBehaviour>();
        _playerBehaviour.OnHoldItemChanged += CheckHandItem;
        CheckHandItem();
        OnTestTubeStateChange(QueenAntJar.State.Empty);
    }

    void OnDisable()
    {
        _playerBehaviour.OnHoldItemChanged -= CheckHandItem;
        _playerBehaviour = null;
    }

    void CheckHandItem()
    {
        if (!_playerBehaviour.IsHolding)
        {
            if (_testTube)
                _testTube.OnStateChange -= OnTestTubeStateChange;
            _testTube = null;
            return;
        }

        try
        {
            _testTube = (QueenAntJar)_playerBehaviour.HoldItem;
            _testTube.OnStateChange += OnTestTubeStateChange;
        }
        catch (System.InvalidCastException)
        {
            if (_testTube)
                _testTube.OnStateChange -= OnTestTubeStateChange;
            _testTube = null;
        }
    }

    void OnTestTubeStateChange(QueenAntJar.State state)
    {
        string format = LanguageMgr.GetTextById(formatLanguageID);
        switch(state)
        {
            case QueenAntJar.State.Empty:
                text.text = string.Format(format, 0, 2);
                break;
            case QueenAntJar.State.OneAnt:
                text.text = string.Format(format, 1, 2);
                break;
            case QueenAntJar.State.TwoAnt:
                text.text = string.Format(format, 2, 2);
                break;
        }

        if (state == requireState)
            step.Skip();
    }
}
