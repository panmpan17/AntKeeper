using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu]
public class IntRangeReference : ScriptableObject
{
    public int Min;
    public int Max;

    public int PickRandomNumber(bool maxInclusive=false)
    {
        return maxInclusive ? Random.Range(Min, Max + 1) : Random.Range(Min, Max);
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(IntRangeReference))]
public class IntRangeReferenceDrawer : PropertyDrawer
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
            min.intValue = EditorGUI.IntField(position, min.intValue);

            position.x += position.width + 10;
            SerializedProperty max = serializedObject.FindProperty("Max");
            max.intValue = EditorGUI.IntField(position, max.intValue);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif