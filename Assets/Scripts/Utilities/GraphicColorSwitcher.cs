using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


[ExecuteInEditMode]
public class GraphicColorSwitcher : MonoBehaviour
{
    public Graphic[] targetGraphics;
    public ColorReference colorReference;

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
        if (!colorReference)
            return;

        for (int i = 0; i < targetGraphics.Length; i++)
        {
            // Graphic graphic = targetGraphics[i];
            // Debug.Log(graphic);

            // try
            // {
            //     var text = ((TextMeshProUGUI)graphic);
            //     text.faceColor = colorReference.Value;
            // }
            // catch (System.InvalidCastException)
            // {
                targetGraphics[i].color = colorReference.Value;
            // }
        }
    }
}
