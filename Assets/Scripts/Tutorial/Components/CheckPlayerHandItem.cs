using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerHandItem : MonoBehaviour
{
    [SerializeField]
    private AbstractTutorialStep step;
    [SerializeField]
    private bool checkHandIsEmpty;
    [SerializeField]
    private string requireHoldItemTypeName;

    private PlayerBehaviour _playerBehaviour;

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
        if (checkHandIsEmpty)
        {
            if (!_playerBehaviour.IsHolding)
                step.Skip();
            return;
        }

        if (!_playerBehaviour.IsHolding)
            return;
        if (_playerBehaviour.HoldItem.GetType().Name == requireHoldItemTypeName)
            step.Skip();
    }
}
