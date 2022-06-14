using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckAntJarHasAnt : MonoBehaviour
{
    [SerializeField]
    private AbstractTutorialStep step;

    private PlayerBehaviour _playerBehaviour;
    private AntJar _antJar;

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
            _antJar = null;
            return;
        }

        try
        {
            _antJar = (AntJar)_playerBehaviour.HoldItem;
        }
        catch (System.InvalidCastException)
        {
            _antJar = null;
        }
    }

    void Update()
    {
        if (_antJar)
        {
            if (_antJar.HasAnt)
                step.Skip();
        }
    }
}
