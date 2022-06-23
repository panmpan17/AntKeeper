using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class AntRouteSpreadControl : MonoBehaviour
{
    [SerializeField]
    private float canSpreadSize;
    [SerializeField]
    private RangeReference radiusRange;
    [SerializeField]
    private RangeReference spreadInterval;
    private Timer _spreadIntervalTimer;

    public bool CanSpreadNewNest => _sizeControl.Size >= canSpreadSize;

    private AntNestHub _hub;
    private AntNestSizeControl _sizeControl;

    void Awake()
    {
        _hub = GetComponent<AntNestHub>();
        _sizeControl = GetComponent<AntNestSizeControl>();

        _spreadIntervalTimer = new Timer(spreadInterval.PickRandomNumber());
    }

    void Update()
    {
        if (CanSpreadNewNest && _spreadIntervalTimer.UpdateEnd)
        {
            if (TrySpreadNewNest())
            {
                _spreadIntervalTimer.Reset();
                _spreadIntervalTimer.TargetTime = spreadInterval.PickRandomNumber();
            }
        }
    }

    bool TrySpreadNewNest()
    {
        Vector2 relativeVector = Random.insideUnitCircle;
        relativeVector.Normalize();
        relativeVector *= radiusRange.PickRandomNumber();

        Vector3Int newNestGridPosition = GridManager.ins.WorldToCell(transform.position + (Vector3)relativeVector);

        if (GridManager.ins.CheckGroundAvalibleForNewAntAndCap(newNestGridPosition))
        {
            GridManager.ins.InstantiateAntNest(newNestGridPosition, _hub.IsFireAnt);
            return true;
        }

        return false;
    }
}
