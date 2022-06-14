using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealColorTrigger : MonoBehaviour
{
    [SerializeField]
    private AntNestHub hub;
    [SerializeField]
    private bool triggerOnStart;


    void Start()
    {
        if (triggerOnStart)
            Trigger();
    }


    public void Trigger()
    {
        hub.ShowTrueColor();
    }
}
