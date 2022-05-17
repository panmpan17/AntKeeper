using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class Microscope : AbstractGroundInteractive
{
    [SerializeField]
    [ShortTimer]
    private Timer examineTimer;
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
    private AntJar antJar;
    [SerializeField]
    private FillBarControl progressBar;

    void Awake()
    {
        examineTimer.Running = false;
        progressBar.gameObject.SetActive(false);
    }

    protected override void Start()
    {
        base.Start();

        if (antJar != null)
            PlaceItem(antJar);
    }

    void Update()
    {
        if (examineTimer.Running)
        {
            if (examineTimer.UpdateEnd)
            {
                ExamineEnd();
                return;
            }

            progressBar.SetFillAmount(examineTimer.Progress);
        }
    }

    public override bool OnEmptyHandInteract(PlayerBehaviour playerBehaviour)
    {
        if (examineTimer.Running)
            return false;

        playerBehaviour.SetHandItem(antJar);
        antJar = null;

        return true;
    }

    public override bool CanItemPlaceDown(AbstractHoldItem item)
    {
        if (item.GetType() == typeof(AntJar))
        {
            return true;
        }

        return false;
    }

    public override void PlaceDownItem(AbstractHoldItem item)
    {
        antJar = (AntJar)item;
        PlaceItem(item);

        if (antJar.HasAnt)
            ExamineStart();
    }

    void PlaceItem(AbstractHoldItem item)
    {
        item.ChangeRendererSorting(sortingLayerID, sortingOrder);
        item.transform.SetParent(offsetPoint, true);
        item.transform.localPosition = Vector3.zero;
        item.transform.localScale = Vector3.one;
    }

    void ExamineStart()
    {
        particleSystem.Play();
        examineTimer.Reset();
        progressBar.gameObject.SetActive(true);
    }

    void ExamineEnd()
    {
        antJar.ShowAntNestTrueColor();

        particleSystem.Stop();
        examineTimer.Running = false;
        progressBar.gameObject.SetActive(false);
    }
}
