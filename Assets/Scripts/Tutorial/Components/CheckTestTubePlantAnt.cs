using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTestTubePlantAnt : MonoBehaviour
{
    [SerializeField]
    private AbstractTutorialStep step;

    private PlayerBehaviour _playerBehaviour;
    private QueenAntJar _testTube;

    void OnEnable()
    {
        _playerBehaviour = GameObject.FindWithTag(PlayerBehaviour.Tag).GetComponent<PlayerBehaviour>();
        _playerBehaviour.OnHoldItemChanged += CheckHandItem;
        CheckHandItem();
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
                _testTube.OnSuccessPlantAnt -= OnSuccessPlantAnt;
            _testTube = null;
            return;
        }

        try
        {
            _testTube = (QueenAntJar)_playerBehaviour.HoldItem;
            _testTube.OnSuccessPlantAnt += OnSuccessPlantAnt;
        }
        catch (System.InvalidCastException)
        {
            if (_testTube)
                _testTube.OnSuccessPlantAnt -= OnSuccessPlantAnt;
            _testTube = null;
        }
    }

    void OnSuccessPlantAnt()
    {
        step.Skip();
    }
}