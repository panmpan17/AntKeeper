using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntColorReveal : MonoBehaviour
{
    [SerializeField]
    private AntNestHub[] antNestHubs;
    [SerializeField]
    private float stepInterval;

    void Start()
    {
        Time.timeScale = 0;
        for (int i = 0; i < antNestHubs.Length; i++)
        {
            antNestHubs[i].StartRevealColor(stepInterval);
        }
    }
}
