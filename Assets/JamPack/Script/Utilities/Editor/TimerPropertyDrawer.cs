using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MPack {
    [CustomPropertyDrawer(typeof(Timer))]
    public class TimerPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect labelRect = position;
            labelRect.width *= 0.4f;

            EditorGUI.BeginProperty(labelRect, label, property);
            EditorGUI.LabelField(labelRect, label);

            Rect valueRect = position;
            valueRect.width = position.width * 0.6f;
            valueRect.x += labelRect.width;
            EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("TargetTime"), GUIContent.none);
            EditorGUI.EndProperty();
        }
    }
}