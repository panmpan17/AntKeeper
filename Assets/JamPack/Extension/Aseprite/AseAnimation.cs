using UnityEngine;

namespace MPack.Aseprite {
    public class AseAnimation : ScriptableObject
    {
        public string Name;

        public bool Loop;
        public LoopAnimation LoopType;

        public KeyPoint[] Points;

        public enum LoopAnimation : byte
        {
            Forward = 0,
            Reverse = 1,
            PingPong = 2,
        }

        [System.Serializable]
        public struct KeyPoint {
            public Sprite Sprite;
            public float Time;

            public KeyPoint (Sprite sprite, float time) {
                Sprite = sprite;
                Time = time;
            }
        }
    }
}