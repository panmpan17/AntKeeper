using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace MPack
{
    public class MultipleObjectPositioningWindow : EditorWindow
    {
        [MenuItem("MPack/Open Multiple Object Positining")]
        public static void OpenWindow()
        {
            MultipleObjectPositioningWindow window = GetWindow<MultipleObjectPositioningWindow>("Multiple Objects Positioning");
            window.RescanSelectedGameObjects();
        }

        private List<GameObject> positioningObjects;
        private Dictionary<GameObject, Vector3> originPositions;

        private Vector3 m_startposition;
        private Vector3 m_delta;

        private ReorderableList objectList;

        private bool showSameDisplacement;
        private bool showPreciseAxis;

        /// <summary>
        /// Initialize the varible of the class
        /// </summary>
        private void Initialize()
        {
            positioningObjects = new List<GameObject>();
            originPositions = new Dictionary<GameObject, Vector3>();

            objectList = new ReorderableList(positioningObjects, typeof(GameObject), true,  false, true, true);

            RescanSelectedGameObjects();
        }

        /// <summary>
        /// Called when window is drawing
        /// </summary>
        private void OnGUI() {
            if (positioningObjects == null) return;

            Space();

            showSameDisplacement = EditorGUILayout.Foldout(showSameDisplacement, "Same Displacement");
            if (showSameDisplacement)
            {
                EditorGUI.indentLevel++;
                SameDisplacement();
                EditorGUI.indentLevel--;
            }

            showPreciseAxis = EditorGUILayout.Foldout(showPreciseAxis, "Precise Axis");
            if (showPreciseAxis)
            {
                EditorGUI.indentLevel++;
                PreciseAxis();
                EditorGUI.indentLevel--;
            }
            
            ObjectList();
        }

        private void SameDisplacement()
        {
            EditorGUI.BeginChangeCheck();
            m_startposition = EditorGUILayout.Vector3Field("First GameObject Position", m_startposition);
            m_delta = EditorGUILayout.Vector3Field("Every GameObject Delta", m_delta);
            if (EditorGUI.EndChangeCheck())
            {
                RearrangeObjectsByDisplacement();
                return;
            }
        }

        private void PreciseAxis()
        {
            EditorGUI.BeginChangeCheck();
            Vector3 centerPosition = GetCenterPosition();
            Vector3 newCenterPosition = EditorGUILayout.Vector3Field("Center Position", centerPosition);
            if (EditorGUI.EndChangeCheck())
            {
                RearrangeObjectsByDelta(newCenterPosition - centerPosition);
                return;
            }

            EditorGUI.BeginChangeCheck();
            Vector3 firstObjectPosition = positioningObjects.Count > 0? positioningObjects[0].transform.position: Vector3.zero;
            Vector3 newFirstObjectPosition = EditorGUILayout.Vector3Field("First Position", firstObjectPosition);
            if (EditorGUI.EndChangeCheck())
            {
                RearrangeObjectsByDelta(newFirstObjectPosition - firstObjectPosition);
                return;
            }
        }

        private void ObjectList()
        {
            GUILayout.Space(8);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Objects List");
            if (GUILayout.Button("Reload"))
            {
                RevertPosition();
                RescanSelectedGameObjects();
                return;
            }
            EditorGUILayout.EndHorizontal();

            Space();
            objectList.DoLayoutList();


            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Revert"))
            {
                RevertPosition();
                return;
            }
            if (GUILayout.Button("Apply"))
            {
                ClearVarible();
                return;
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Called when window appear or code been recomplie
        /// </summary>
        private void OnEnable() {
            Initialize();
            RescanSelectedGameObjects();
        }

        /// <summary>
        /// Called when window been clean up
        /// </summary>
        private void OnDisable() {
            RevertPosition();
            // positioningObjects = null;
        }

        private void Space()
        {
            EditorGUILayout.Space(8);
        }

        private Vector3 GetCenterPosition()
        {
            Vector3 position = Vector3.zero;
            for (int i = 0; i < positioningObjects.Count; i++)
            {
                position.x += positioningObjects[i].transform.position.x / positioningObjects.Count;
                position.y += positioningObjects[i].transform.position.y / positioningObjects.Count;
                position.z += positioningObjects[i].transform.position.z / positioningObjects.Count;
            }
            return position;
        }

        /// <summary>
        /// Rearrange the objects according to the startPosition and delta
        /// </summary>
        private void RearrangeObjectsByDisplacement()
        {
            // EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            if (positioningObjects.Count > 0 && positioningObjects[0] != null)
            {
                Undo.RecordObject(positioningObjects[0].transform, "");
                positioningObjects[0].transform.position = m_startposition;
            }

            for (int i = 1; i < positioningObjects.Count; i++)
            {
                if (positioningObjects[i] != null)
                {
                    Undo.RecordObject(positioningObjects[i].transform, "");
                    positioningObjects[i].transform.position = m_startposition + (m_delta * i);
                }
            }
        }

        private void RearrangeObjectsByDelta(Vector3 delta)
        {
            for (int i = 0; i < positioningObjects.Count; i++)
            {
                Undo.RecordObject(positioningObjects[i].transform, "");
                if (positioningObjects[i] != null) positioningObjects[i].transform.position += delta;
            }
        }

        /// <summary>
        /// Revert all the object back to origin position
        /// </summary>
        private void RevertPosition()
        {
            for (int i = 0; i < positioningObjects.Count; i++)
            {
                if (positioningObjects[i] != null) positioningObjects[i].transform.position = originPositions[positioningObjects[i]];
            }

            ClearVarible();
        }

        /// <summary>
        /// Clear the varible values that been stored
        /// </summary>
        private void ClearVarible()
        {
            positioningObjects.Clear();
            originPositions.Clear();
            m_startposition = m_delta = Vector3.zero;
        }

        /// <summary>
        /// Rescan all the selection gameobject into list
        /// </summary>
        public void RescanSelectedGameObjects()
        {
            positioningObjects.Clear();
            originPositions.Clear();

            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                if (!PrefabUtility.IsPartOfPrefabAsset(Selection.gameObjects[i]))
                {
                    positioningObjects.Add(Selection.gameObjects[i]);
                    originPositions.Add(Selection.gameObjects[i], Selection.gameObjects[i].transform.position);
                }
            }

            if (positioningObjects.Count == 0)
            {
                m_startposition = m_delta = Vector3.zero;
                return;
            }

            positioningObjects.Sort(delegate (GameObject obj1, GameObject obj2) {
                int index1 = obj1.transform.GetSiblingIndex();
                int index2 = obj2.transform.GetSiblingIndex();

                if (index1 > index2) return 1;
                if (index1 < index2) return -1;

                return 0;
            });

            m_startposition = positioningObjects[0].transform.position;

            m_delta = positioningObjects.Count > 1? positioningObjects[1].transform.position - positioningObjects[0].transform.position: Vector3.zero;
            RearrangeObjectsByDisplacement();
        }
    }
}