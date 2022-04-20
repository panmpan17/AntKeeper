using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MPack;


[ExecuteInEditMode]
public class GraphicColorSwitcher : MonoBehaviour
{
    public Graphic[] targetGraphics;
    public ColorVariable colorVariable;

    void Awake()
    {
        ApplyColor();
    }

#if UNITY_EDITOR
    void Reset()
    {
        targetGraphics = GetComponentsInChildren<Graphic>();
    }

    void Update()
    {
        ApplyColor();
    }
#endif

    void ApplyColor()
    {
        if (colorVariable == null)
            return;

        for (int i = 0; i < targetGraphics.Length; i++)
        {
            targetGraphics[i].color = colorVariable.Value;
        }
    }
}
