using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu]
public class RangeReference : ScriptableObject
{
    public float Min;
    public float Max;

    public float PickRandomNumber()
    {
        return Random.Range(Min, Max);
    }

    public float Clamp(float number)
    {
        return Mathf.Clamp(number, Min, Max);
    }
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(RangeReference))]
public class RangeReferenceDrawer : PropertyDrawer
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

            position.width = originWidth * 0.3f - 5;
            SerializedProperty min = serializedObject.FindProperty("Min");
            min.floatValue = EditorGUI.FloatField(position, min.floatValue);

            position.x += position.width + 10;
            SerializedProperty max = serializedObject.FindProperty("Max");
            max.floatValue = EditorGUI.FloatField(position, max.floatValue);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif