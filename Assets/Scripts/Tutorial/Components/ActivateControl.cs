using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateControl : MonoBehaviour
{
    [SerializeField]
    private GameObject[] activates;
    [SerializeField]
    private GameObject[] deactivates;
    [SerializeField]
    private MonoBehaviour[] enables;
    [SerializeField]
    private MonoBehaviour[] disables;

    void OnEnable()
    {
        for (int i = 0; i < activates.Length; i++)
        {
            if (activates[i] != null)
                activates[i].SetActive(true);
        }
        for (int i = 0; i < deactivates.Length; i++)
        {
            if (deactivates[i] != null)
                deactivates[i].SetActive(false);
        }

        for (int i = 0; i < enables.Length; i++)
        {
            if (enables[i])
                enables[i].enabled = true;
        }
        for (int i = 0; i < disables.Length; i++)
        {
            if (disables[i])
                disables[i].enabled = false;
        }
    }
}
