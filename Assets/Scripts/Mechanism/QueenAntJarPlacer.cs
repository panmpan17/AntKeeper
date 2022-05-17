using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


public class QueenAntJarPlacer : AbstractGroundInteractive
{
    [SerializeField]
    [SortingLayer]
    private int sortingLayerID;
    [SerializeField]
    private int sortingOrder;

    [Header("Reference")]
    [SerializeField]
    private Transform offsetPoint;
    [SerializeField]
    private QueenAntJar antJar;

    public override bool OnEmptyHandInteract(PlayerBehaviour playerBehaviour)
    {
        if (antJar != null)
        {
            playerBehaviour.SetHandItem(antJar);
            antJar = null;
            return true;
        }

        return false;
    }

    public override bool CanItemPlaceDown(AbstractHoldItem item)
    {
        if (item.GetType() == typeof(QueenAntJar))
        {
            return true;
        }

        return false;
    }


    public override void PlaceDownItem(AbstractHoldItem item)
    {
        antJar = (QueenAntJar)item;
        PlaceItem(item);
    }

    void PlaceItem(AbstractHoldItem item)
    {
        item.ChangeRendererSorting(sortingLayerID, sortingOrder);
        item.transform.SetParent(offsetPoint, true);
        item.transform.localPosition = Vector3.zero;
        item.transform.localScale = Vector3.one;
    }
}
