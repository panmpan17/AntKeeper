using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MPack;


[ExecuteInEditMode]
public class GraphicColorSwitcher : MonoBehaviour
{
    [SerializeField]
    private Graphic[] targetGraphics;
    [SerializeField]
    private ColorReference colorVariable;
    public ColorReference Color {
        get => colorVariable;
        set {
            colorVariable = value;
            ApplyColor();
        }
    }

    void Awake()
    {
        ApplyColor();
    }

#if UNITY_EDITOR
    void Start()
    {
        if (UnityEditor.EditorApplication.isPlaying)
            enabled = false;
    }

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
        for (int i = 0; i < targetGraphics.Length; i++)
        {
            targetGraphics[i].color = colorVariable.Value;
        }
    }
}
