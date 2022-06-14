using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


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
    [SerializeField]
    private FillBarControl barControl;

    protected override void Start()
    {
        base.Start();

        if (filledBucket != null)
        {
            barControl.gameObject.SetActive(!filledBucket.IsFull);
            PlaceItem(filledBucket);
        }
        else
            barControl.gameObject.SetActive(false);
    }

    void Update()
    {
        if (filledBucket != null && !filledBucket.IsFull)
        {
            filledBucket.FillAmount += fillSpeed * Time.deltaTime;
            barControl.SetFillAmount(filledBucket.FillAmountProgress);

            if (filledBucket.IsFull)
            {
                particleSystem.Stop();
                barControl.gameObject.SetActive(false);
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
            barControl.gameObject.SetActive(false);

            return true;
        }

        return false;
    }

    public override bool CanItemPlaceDown(AbstractHoldItem item)
    {
        if (item.GetType() == typeof(Bucket))
        {
            return true;
        }

        return false;
    }

    public override void PlaceDownItem(AbstractHoldItem item)
    {
        var newBucket = (Bucket)item;

        if (filledBucket != null)
        {
            item.PlayerBehaviour.SetHandItem(filledBucket);
            particleSystem.Stop();
        }

        filledBucket = newBucket;
        PlaceItem(item);

        if (!filledBucket.IsFull)
        {
            barControl.gameObject.SetActive(true);
            particleSystem.Play();
        }
        else
        {
            barControl.gameObject.SetActive(false);
        }
    }

    void PlaceItem(AbstractHoldItem item)
    {
        item.ChangeRendererSorting(sortingLayerID, sortingOrder);
        item.transform.SetParent(offsetPoint, true);
        item.transform.localPosition = Vector3.zero;
        item.transform.localScale = Vector3.one;
    }
}
