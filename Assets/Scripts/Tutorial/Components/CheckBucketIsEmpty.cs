using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckBucketIsEmpty : MonoBehaviour
{
    [SerializeField]
    private AbstractTutorialStep step;
    [SerializeField]
    private UnityEvent callback;

    private PlayerBehaviour _playerBehaviour;
    private Bucket _bucket;

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
            _bucket = null;
            return;
        }

        try
        {
            _bucket = (Bucket)_playerBehaviour.HoldItem;
        }
        catch (System.InvalidCastException)
        {
            _bucket = null;
        }
    }

    void Update()
    {
        if (_bucket)
        {
            if (_bucket.IsEmpty)
            {
                step?.Skip();
                callback.Invoke();
            }
        }
    }
}
