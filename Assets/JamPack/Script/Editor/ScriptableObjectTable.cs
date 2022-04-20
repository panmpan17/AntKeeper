using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace MPack
{
    public class ScriptableObjectTable : EditorWindow
    {
        private const float FieldPadding = 2;

        #region Static
        static private Color ObjectNameBackgroundColor = EditorUtilities.From256Color(75, 75, 75);
        static private Color LineColor = EditorUtilities.From256Color(65, 65, 65);

        [MenuItem("Window/Scriptable Object Table")]
        [MenuItem("MPack/Scriptable Object Table")]
        static public void OpenTableWindow()
        {
            GetWindow<ScriptableObjectTable>("Scriptable Object Table");
        }
        #endregion

        private GUIStyle m_scrollBarStyle;
        private GUIStyle m_headerStyle;

        private Type[] m_classTypes;
        private string[] m_classNames;
        private int m_classIndex = 0;
        private ClassField[] m_classFields;
        private ScriptableObject[] m_objects;
        private Vector2 m_scrollViewPos;
        private float m_objectNameWidth;

        #region Setup
        private void OnEnable() {
            SetUpStyle();
            FindAllScriptableObject();
            // m_classTypes = new Type[] {
            //     typeof(SelectableStyle),
            // };
            // m_classNames = new string[] {
            //     "SelectableStyle",
            // };
            FindAllFieldUnderScriptableClass();
            FindAllScriptablesUnderType();
        }

        private void SetUpStyle()
        {
            m_scrollBarStyle = new GUIStyle();
            m_scrollBarStyle.normal.textColor = Color.white;

            m_headerStyle = new GUIStyle();
            m_headerStyle.fontSize = 14;
            m_headerStyle.normal.textColor = Color.white;
        }
        #endregion

        #region Reflection
        private void FindAllScriptableObject()
        {
            string projectPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/"));
            List<Type> classTypeList = new List<Type>();
            List<string> classNameList = new List<string>();

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (a.CodeBase.IndexOf(projectPath) == -1 || a.CodeBase.IndexOf("Unity.") >= 0 || a.CodeBase.IndexOf("com.unity.") >= 0)
                {
                    continue;
                }

                foreach (Type t in a.GetTypes())
                {
                    if (t.Name.IndexOf("Editor") != -1)
                        continue;
                    if (!t.IsSubclassOf(typeof(ScriptableObject)))
                        continue;
                    if (t.IsSubclassOf(typeof(Editor)) || t.IsSubclassOf(typeof(EditorWindow)))
                        continue;

                    classTypeList.Add(t);
                    classNameList.Add(t.Name);
                }
            }

            m_classTypes = classTypeList.ToArray();
            m_classNames = classNameList.ToArray();
        }

        private void FindAllFieldUnderScriptableClass()
        {
            FieldInfo[] infos = m_classTypes[m_classIndex].GetFields(
                BindingFlags.Instance | BindingFlags.Public);
            // BindingFlags.NonPublic |

            List<ClassField> fields = new List<ClassField>();
            foreach (FieldInfo info in infos)
            {
                fields.Add(new ClassField(info.Name));
            }

            m_classFields = fields.ToArray();
        }

        private void FindAllScriptablesUnderType()
        {
            string[] files = AssetDatabase.FindAssets("t:" + m_classNames[m_classIndex]);
            m_objects = new ScriptableObject[files.Length];
            m_objectNameWidth = 150;

            for (int i = 0; i < files.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(files[i]);
                ScriptableObject data = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                m_objects[i] = data;
            }
        }
        #endregion

        #region Draw Editor
        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            m_classIndex = EditorGUILayout.Popup(m_classIndex, m_classNames);
            if (EditorGUI.EndChangeCheck())
            {
                FindAllFieldUnderScriptableClass();
                FindAllScriptablesUnderType();
            }

            GUILayout.Space(10);

            float width = CalculateTableWidth();
            DrawFieldNameHeader(width);

            m_scrollViewPos = EditorGUILayout.BeginScrollView(m_scrollViewPos, false, false);
            for (int i = 0; i < m_objects.Length; i++)
            {
                DrawObjectRow(m_objects[i], width);
            }
            EditorGUILayout.EndScrollView();
        }

        private float CalculateTableWidth()
        {
            float width = m_objectNameWidth + FieldPadding;
            for (int i = 0 ; i < m_classFields.Length; i++)
            {
                width += m_classFields[i].width + FieldPadding;
            }
            return width;
        }

        private void DrawFieldNameHeader(float width)
        {
            EditorGUILayout.BeginScrollView(
                new Vector2(m_scrollViewPos.x, 0),
                true,
                true,
                m_scrollBarStyle,
                m_scrollBarStyle,
                m_scrollBarStyle,
                GUILayout.Height(30));

            Rect rowRect = EditorGUILayout.GetControlRect(false, 30, GUILayout.Width(width));

            Rect labelRect = rowRect;
            labelRect.x += m_objectNameWidth + FieldPadding;
            labelRect.y += 2;
            labelRect.height -= 4;

            for (int i = 0; i < m_classFields.Length; i++)
            {
                EditorGUI.LabelField(labelRect, m_classFields[i].name, m_headerStyle);
                labelRect.x += m_classFields[i].width + FieldPadding;
            }

            Rect hrRect = rowRect;
            hrRect.y += rowRect.height - 1;
            hrRect.height = 1;

            EditorGUI.DrawRect(hrRect, LineColor);

            EditorGUILayout.EndScrollView();
        }

        private void DrawObjectRow(ScriptableObject scriptableObject, float width)
        {
            Rect rowRect = EditorGUILayout.GetControlRect(false, 30, GUILayout.Width(width));

            Rect rect = rowRect;
            rect.width = m_objectNameWidth;
            rect.height -= 3;
            rect.y++;

            if (GUI.Button(rect, scriptableObject.name))
            {
                // AssetDatabase.OpenAsset(scriptableObject);
                Selection.activeObject = scriptableObject;
            }

            rect.x += m_objectNameWidth + FieldPadding;

            SerializedObject serializedObject = new SerializedObject(scriptableObject);

            for (int i = 0; i < m_classFields.Length; i++)
            {
                rect.width = m_classFields[i].width;
                EditorGUI.PropertyField(rect, serializedObject.FindProperty(m_classFields[i].name), GUIContent.none);
                rect.x += m_classFields[i].width + FieldPadding;
            }

            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region Structures
        public struct ClassField
        {
            public string name;
            public float width;

            public ClassField(string name, float width=150)
            {
                this.name = name;
                this.width = width;
            }
        }
        #endregion
    }
}