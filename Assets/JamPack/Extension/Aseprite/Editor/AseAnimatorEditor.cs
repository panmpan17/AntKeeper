using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MPack.Aseprite {
    [CustomEditor(typeof(AseAnimator))]
    public class AseAnimatorEditor : Editor
    {
        private AseAnimator animator;
        private SerializedProperty animations;

        private void OnEnable() {
            animator = (AseAnimator) target;
            animations = serializedObject.FindProperty("animations");
        }

        public override void OnInspectorGUI() {
            base.DrawDefaultInspector();

            if (EditorApplication.isPlaying) {
                for (int i = 0; i < animations.arraySize; i++) {
                    AseAnimation anim = animations.GetArrayElementAtIndex(i).objectReferenceValue as System.Object as AseAnimation;
                    if (GUILayout.Button(string.Format("Play '{0}' Animation", anim.Name))) {
                        animator.Play(i);
                    }
                }
            }
        }
    }
}