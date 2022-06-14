using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckPlayerHandItem : MonoBehaviour
{
    [SerializeField]
    private AbstractTutorialStep step;
    [SerializeField]
    private bool checkHandIsEmpty;
    [SerializeField]
    private string requireHoldItemTypeName;

    [SerializeField]
    private UnityEvent callback;

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
            {
                step?.Skip();
                callback.Invoke();
            }
            return;
        }

        if (!_playerBehaviour.IsHolding)
            return;
        if (_playerBehaviour.HoldItem.GetType().Name != requireHoldItemTypeName)
            return;

        step?.Skip();
        callback.Invoke();
    }
}
