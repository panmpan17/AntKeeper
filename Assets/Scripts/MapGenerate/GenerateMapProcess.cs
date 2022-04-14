using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif


namespace MapGenerate
{
    public enum ProcessorType
    {
        RandomPlace,
        Smooth,
        PlaceCircle,
        PerlinNoise,
    }

    [CreateAssetMenu]
    public class GenerateMapProcess : ScriptableObject
    {
        public ProcessorRule[] processorList;

        [System.Serializable]
        public struct ProcessorRule
        {
            public ProcessorType processorType;

            public RandomPlaceProcessor randomPlaceProcessor;
            public SmoothProcessor smoothProcessor;
            public PlaceCircleProcessor placeCircleProcessor;
            public PerlinNoiseProccessor perlinNoiseProccessor;
        }
    }

    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(GenerateMapProcess.ProcessorRule))]
    public class GenerateMapProcessPropertyDrawer : PropertyDrawer
    {
        public static SerializedProperty FindProcessTypeSetting(SerializedProperty property)
        {
            var type = (ProcessorType)property.FindPropertyRelative("processorType").enumValueIndex;

            switch (type)
            {
                case ProcessorType.RandomPlace:
                    return property.FindPropertyRelative("randomPlaceProcessor");
                case ProcessorType.Smooth:
                    return property.FindPropertyRelative("smoothProcessor");
                case ProcessorType.PlaceCircle:
                    return property.FindPropertyRelative("placeCircleProcessor");
                case ProcessorType.PerlinNoise:
                    return property.FindPropertyRelative("perlinNoiseProccessor");
            }
            return null;
        }
        public static SerializedProperty FindProcessTypeSetting(SerializedProperty property, int typeEnumValue)
        {
            var type = (ProcessorType) typeEnumValue;

            switch (type)
            {
                case ProcessorType.RandomPlace:
                    return property.FindPropertyRelative("randomPlaceProcessor");
                case ProcessorType.Smooth:
                    return property.FindPropertyRelative("smoothProcessor");
                case ProcessorType.PlaceCircle:
                    return property.FindPropertyRelative("placeCircleProcessor");
                case ProcessorType.PerlinNoise:
                    return property.FindPropertyRelative("perlinNoiseProccessor");
            }
            return null;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty settingProperty = FindProcessTypeSetting(property);
            settingProperty.isExpanded = true;
            return settingProperty == null ? 20 : 20 + EditorGUI.GetPropertyHeight(settingProperty);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float originHeight = position.height;
            position.height = 20;

            SerializedProperty processorTypeProperty = property.FindPropertyRelative("processorType");
            EditorGUI.PropertyField(position, processorTypeProperty);

            position.y += 20;
            position.height = originHeight - 20;

            SerializedProperty settingProperty = FindProcessTypeSetting(property, processorTypeProperty.enumValueIndex);
            if (settingProperty != null)
            {
                settingProperty.isExpanded = true;
                EditorGUI.PropertyField(position, settingProperty, true);
            }
        }
    }
    #endif
}