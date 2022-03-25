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

    private Bucket _filledBucket;

    void Update()
    {
        if (_filledBucket != null && !_filledBucket.IsFull)
        {
            _filledBucket.FillAmount += fillSpeed * Time.deltaTime;

            if (_filledBucket.IsFull)
            {
                particleSystem.Stop();
            }
        }
    }

    public override bool OnEmptyHandInteract(PlayerBehaviour playerBehaviour)
    {
        playerBehaviour.SetHandItem(_filledBucket);
        _filledBucket = null;
        particleSystem.Stop();

        return true;
    }

    public override bool OnHoldItemInteract(AbstractHoldItem item)
    {
        if (item.GetType() == typeof(Bucket))
        {
            _filledBucket = (Bucket)item;

            _filledBucket.PlayerBahviour.ClearHandItem();

            _filledBucket.transform.SetParent(offsetPoint, true);
            _filledBucket.transform.localPosition = Vector3.zero;
            _filledBucket.transform.localScale = Vector3.one;

            if (!_filledBucket.IsFull)
                particleSystem.Play();
        }

        return true;
    }
}
