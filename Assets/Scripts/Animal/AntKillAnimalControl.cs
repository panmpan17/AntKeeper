using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


[RequireComponent(typeof(AntNestHub))]
public class AntKillAnimalControl : MonoBehaviour
{
    [SerializeField]
    private RangeReference killAnimalInterval;
    private Timer _killAnimalTimer;

    private AntNestHub _hub;

    void Awake()
    {
        _killAnimalTimer = new Timer(killAnimalInterval.PickRandomNumber());
        _hub = GetComponent<AntNestHub>();
    }

    void Update()
    {
        if (_killAnimalTimer.UpdateEnd)
        {
            if (TryKillAnimal())
            {
                _killAnimalTimer.Reset();
                _killAnimalTimer.TargetTime = killAnimalInterval.PickRandomNumber();
            }
        }
    }

    bool TryKillAnimal()
    {
        Vector3Int position = _hub.routeBranches[Random.Range(0, _hub.routeBranches.Count)].PickRandomPosition();

        if (GridManager.ins.TryFindAnimal(position, out VirtualAnimalSpot animalSpot))
        {
            animalSpot.Kill();
            return true;
        }

        return false;
    }
}
