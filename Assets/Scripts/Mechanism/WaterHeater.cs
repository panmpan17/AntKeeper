using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterHeater : AbstractGroundInteractive
{
    [SerializeField]
    private float fillSpeed;

    [SerializeField]
    private Transform offsetPoint;
    [SerializeField]
    private new ParticleSystem particleSystem;

    [SerializeField]
    private Bucket filledBucket;

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
        playerBehaviour.SetHandItem(filledBucket);
        filledBucket = null;
        particleSystem.Stop();

        return true;
    }

    public override bool OnHoldItemInteract(AbstractHoldItem item)
    {
        if (item.GetType() == typeof(Bucket))
        {
            filledBucket = (Bucket)item;

            filledBucket.PlayerBehaviour.ClearHandItem();

            filledBucket.transform.SetParent(offsetPoint, true);
            filledBucket.transform.localPosition = Vector3.zero;
            filledBucket.transform.localScale = Vector3.one;

            if (!filledBucket.IsFull)
                particleSystem.Play();
        }

        return true;
    }
}
