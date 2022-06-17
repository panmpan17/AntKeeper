using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu]
public class ButtonPaletteReference : ScriptableObject
{
    public Selectable.Transition Transition;

    public ColorBlock Colors;
    public SpriteState SpriteState;
    public AnimationTriggers AnimationTriggers;
}


#if UNITY_EDITOR
[CustomEditor(typeof(ButtonPaletteReference))]
public class ButtonPaletteReferenceEditor : Editor
{
    SerializedProperty transitionProperty;
    SerializedProperty colorsProperty, spriteStateProperty, animationTriggersProperty;

    void OnEnable()
    {
        transitionProperty = serializedObject.FindProperty("Transition");
        colorsProperty = serializedObject.FindProperty("Colors");
        spriteStateProperty = serializedObject.FindProperty("SpriteState");
        animationTriggersProperty = serializedObject.FindProperty("AnimationTriggers");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(transitionProperty);
        GUILayout.Space(10);

        var transition = (Selectable.Transition) transitionProperty.enumValueIndex;

        switch (transition)
        {
            case Selectable.Transition.ColorTint:
                EditorGUILayout.PropertyField(colorsProperty);
                break;

            case Selectable.Transition.SpriteSwap:
                EditorGUILayout.PropertyField(spriteStateProperty);
                break;

            case Selectable.Transition.Animation:
                EditorGUILayout.PropertyField(animationTriggersProperty);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif