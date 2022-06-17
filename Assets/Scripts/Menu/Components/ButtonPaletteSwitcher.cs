using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif


[ExecuteInEditMode]
public class ButtonPaletteSwitcher : MonoBehaviour
{
    public ButtonPaletteReference paletteReference;

    void Awake()
    {
        ApplyColor();
    }

    #if UNITY_EDITOR
    void Update()
    {
        ApplyColor();
    }
    #endif

    void ApplyColor()
    {
        if (!paletteReference)
            return;

        var button = GetComponent<Button>();
        if (!button)
            return;

        button.transition = paletteReference.Transition;
        button.colors = paletteReference.Colors;
        button.spriteState = paletteReference.SpriteState;
        button.animationTriggers = paletteReference.AnimationTriggers;
    }
}
