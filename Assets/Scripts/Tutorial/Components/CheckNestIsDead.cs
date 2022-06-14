using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckNestIsDead : MonoBehaviour
{
    [SerializeField]
    private AbstractTutorialStep step;
    [SerializeField]
    private AntNestHub hub;


    void OnEnable()
    {
        hub.OnNestDestroy += OnNestDestroy;
    }

    void OnDisable()
    {
        if (hub)
            hub.OnNestDestroy -= OnNestDestroy;
    }

    void OnNestDestroy()
    {
        step.Skip();
    }
}
