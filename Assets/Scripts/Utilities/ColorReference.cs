using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu]
public class ColorReference : ScriptableObject
{
    public Color Value;
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ColorReference))]
public class ColorReferenceDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 45;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position.y += 1;
        position.height = 18;
        EditorGUI.PropertyField(position, property, label);

        Object reference = property.objectReferenceValue;
        if (reference)
        {
            SerializedObject serializedObject = new SerializedObject(reference);
            serializedObject.Update();

            float originWidth = position.width;
            position.x += originWidth * 0.4f;
            position.y += 20;

            position.width = originWidth * 0.6f;
            SerializedProperty min = serializedObject.FindProperty("Value");
            min.colorValue = EditorGUI.ColorField(position, min.colorValue);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif