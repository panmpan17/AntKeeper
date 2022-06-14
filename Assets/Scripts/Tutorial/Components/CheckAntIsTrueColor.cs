using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CheckAntIsTrueColor : MonoBehaviour
{
    [SerializeField]
    private AbstractTutorialStep step;

    [SerializeField]
    [Multiline]
    private string format;
    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private AntNestHub[] hubs;
    private int _lastCount = -1;


    void OnEnable()
    {
        Check();
    }

    void Check()
    {
        int count = 0;
        for (int i = 0; i < hubs.Length; i++)
        {
            if (hubs[i].IsShowTrueColor)
                count++;
        }

        if (_lastCount != count)
        {
            _lastCount = count;
            text.text = string.Format(format, count, hubs.Length);
            if (count == hubs.Length)
            {
                step.Skip();
            }
        }
    }

    void Update()
    {
        Check();
    }
}
