using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


[RequireComponent(typeof(AntNestHub), typeof(AntRouteGrowControl))]
public class AntNestSizeControl : MonoBehaviour
{
    [SerializeField]
    private Transform targetTransform;
    [SerializeField]
    private RangeReference sizeRange;
    [SerializeField]
    private float spriteGrowStep;
    [SerializeField]
    private IntRangeReference spriteGrowByRouteRange;
    [SerializeField]
    [Range(0, 200f)]
    private float rootResistent;
    private int _routeCountDown;

    private float _size;
    public float Size => _size;
    public float MaxSize => sizeRange.Max;

    private AntNestHub _hub;
    private AntRouteGrowControl _growControl;

    void Awake()
    {
        _hub = GetComponent<AntNestHub>();
        _hub.OnRootGridTakeDamage += RootPositionTakeDamage;
        _hub.OnOtherNestAttack += TakeDamageFromOtherNest;
        _hub.OnNestDestroy += OnNestDestroy;

        _growControl = GetComponent<AntRouteGrowControl>();
        _growControl.OnSizeIncrease += OnRouteSizeIncrease;

        _size = sizeRange.Min;
        targetTransform.localScale = new Vector3(_size, _size, _size);

        _routeCountDown = spriteGrowByRouteRange.PickRandomNumber();
    }

    void OnRouteSizeIncrease()
    {
        if (--_routeCountDown <= 0)
        {
            _routeCountDown = spriteGrowByRouteRange.PickRandomNumber();
            _size += spriteGrowStep;
            if (_size > sizeRange.Max)
                _size = sizeRange.Max;

            targetTransform.localScale = new Vector3(_size, _size, _size);
        }
    }

    void RootPositionTakeDamage(float damageAmount)
    {
        if (!_hub.enabled)
            return;

        _size -= damageAmount / rootResistent;
        targetTransform.localScale = new Vector3(_size, _size, _size);

        if (_size < sizeRange.Min)
        {
            StatisticTracker.ins.AddPlayerDestroyHubRecord(_hub.IsFireAnt);
            _hub.MainNestHubDestroy();
        }
    }

    void TakeDamageFromOtherNest(float damageAmount)
    {
        _size -= damageAmount;
        targetTransform.localScale = new Vector3(_size, _size, _size);

        if (_size < sizeRange.Min)
        {
            _hub.MainNestHubDestroy();
        }
    }

    void OnNestDestroy()
    {
        targetTransform.gameObject.SetActive(false);
        enabled = false;
    }
}
