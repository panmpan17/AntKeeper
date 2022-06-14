using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveToAntRoute : MonoBehaviour
{
    [SerializeField]
    private Grid grid;
    [SerializeField]
    private AntNestHub hub;
    [SerializeField]
    private VirtualAnimalSpot animalSpot;

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        Vector3Int gridPosition = hub.PickRandomSpotOnRoute(true);
        animalSpot.ChangePosition(grid.GetCellCenterWorld(gridPosition), gridPosition);
    }
}
