using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class Microscope : AbstractGroundInteractive
{
    [SerializeField]
    private Transform offsetPoint;
    [SerializeField]
    private AntJar antJar;

    [SerializeField]
    private Timer examineTimer;
    [SerializeField]
    private FillBarControl progressBar;

    void Awake()
    {
        examineTimer.Running = false;
        progressBar.gameObject.SetActive(false);
    }

    void Update()
    {
        if (examineTimer.Running)
        {
            if (examineTimer.UpdateEnd)
            {
                // Show the color of the ant
                antJar.ShowAntNestTrueColor();

                examineTimer.Running = false;
                progressBar.gameObject.SetActive(false);
                return;
            }

            progressBar.SetFillAmount(examineTimer.Progress);
        }
    }

    public override bool OnEmptyHandInteract(PlayerBehaviour playerBehaviour)
    {
        if (examineTimer.Running)
            return true;

        playerBehaviour.SetHandItem(antJar);
        antJar = null;

        return true;
    }

    public override bool OnHoldItemInteract(AbstractHoldItem item)
    {
        if (item.GetType() == typeof(AntJar))
        {
            antJar = (AntJar)item;

            antJar.PlayerBehaviour.ClearHandItem();

            antJar.transform.SetParent(offsetPoint, true);
            antJar.transform.localPosition = Vector3.zero;
            antJar.transform.localScale = Vector3.one;

            if (antJar.HasAnt)
            {
                examineTimer.Reset();
                progressBar.gameObject.SetActive(true);
            }
        }

        return true;
    }
}
