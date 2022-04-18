using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterHeater : AbstractGroundInteractive
{
    [SerializeField]
    private float fillSpeed;
    [SerializeField]
    [SortingLayer]
    private int sortingLayerID;
    [SerializeField]
    private int sortingOrder;

    [Header("Reference")]
    [SerializeField]
    private Transform offsetPoint;
    [SerializeField]
    private new ParticleSystem particleSystem;
    [SerializeField]
    private Bucket filledBucket;

    protected override void Start()
    {
        base.Start();

        if (filledBucket != null)
            PlaceItem(filledBucket);
    }

    void Update()
    {
        if (filledBucket != null && !filledBucket.IsFull)
        {
            filledBucket.FillAmount += fillSpeed * Time.deltaTime;

            if (filledBucket.IsFull)
            {
                particleSystem.Stop();
            }
        }
    }

    public override bool OnEmptyHandInteract(PlayerBehaviour playerBehaviour)
    {
        if (filledBucket != null)
        {
            playerBehaviour.SetHandItem(filledBucket);
            filledBucket = null;
            particleSystem.Stop();

            return true;
        }

        return false;
    }

    public override bool OnHoldItemInteract(AbstractHoldItem item)
    {
        if (item.GetType() == typeof(Bucket))
        {
            var newBucket = (Bucket)item;

            if (filledBucket != null)
            {
                item.PlayerBehaviour.SetHandItem(filledBucket);
                particleSystem.Stop();
            }
            else
            {
                newBucket.PlayerBehaviour.ClearHandItem();
            }

            filledBucket = newBucket;
            PlaceItem(item);

            if (!filledBucket.IsFull)
                particleSystem.Play();

            return true;
        }

        return false;
    }

    void PlaceItem(AbstractHoldItem item)
    {
        item.ChangeRendererSorting(sortingLayerID, sortingOrder);
        item.transform.SetParent(offsetPoint, true);
        item.transform.localPosition = Vector3.zero;
        item.transform.localScale = Vector3.one;
    }
}
