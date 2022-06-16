#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;


namespace MPack {
    public class ColorTween : MonoBehaviour
    {
        [SerializeField]
        private TweenType type;

        [SerializeField]
        private bool sameInterval = true;
        [SerializeField]
        private float interval = 0.2f;

        [SerializeField]
        private KeyPoint[] keyPoints;
        private int nextKeyPointIndex = 1;
        private bool indexForward = true;

        [SerializeField]
        private SpriteRenderer[] spriteRenderers;
        [SerializeField]
        private Graphic[] graphics;

        private ColorLerpTimer timer;

        private Color color {
            set {
                for (int i = 0; i < spriteRenderers.Length; i++)
                    spriteRenderers[i].color = value;
                for (int i = 0; i < graphics.Length; i++)
                    graphics[i].color = value;
            }
        }

        private void Awake()
        {
            if (sameInterval) timer = new ColorLerpTimer(interval);
            else timer = new ColorLerpTimer(keyPoints[0].Interval);

            int currentKeyPointIndex = 0;
            if (type == TweenType.BackwardLoop)
            {
                currentKeyPointIndex = keyPoints.Length - 2;
                nextKeyPointIndex = currentKeyPointIndex - 1;
                indexForward = false;
            }

            timer.From = keyPoints[currentKeyPointIndex].Color;
            timer.To = keyPoints[nextKeyPointIndex].Color;
        }

        private void CalculateNextIndex() {
            if (indexForward)
            {
                nextKeyPointIndex++;
                if (nextKeyPointIndex >= keyPoints.Length)
                {
                    if (type == TweenType.Loop) nextKeyPointIndex = 0;
                    else if (type == TweenType.ForwardOnce) enabled = false;
                    else
                    {
                        indexForward = false;
                        nextKeyPointIndex = keyPoints.Length - 2;
                    }
                }
            }
            else
            {
                nextKeyPointIndex--;
                if (nextKeyPointIndex < 0)
                {
                    if (type == TweenType.BackwardLoop) nextKeyPointIndex = keyPoints.Length - 1;
                    else if (type == TweenType.BackwardOnce) enabled = false;
                    else
                    {
                        indexForward = true;
                        nextKeyPointIndex = 1;
                    }
                }
            }
        }

        private void Update()
        {
            if (timer.Timer.UpdateEnd)
            {
                color = timer.Value;

                timer.Timer.Reset();
                if (!sameInterval) timer.Timer.TargetTime = keyPoints[nextKeyPointIndex].Interval;

                int currentKeyPointIndex = nextKeyPointIndex;
                CalculateNextIndex();

                timer.From = keyPoints[currentKeyPointIndex].Color;
                timer.To = keyPoints[nextKeyPointIndex].Color;
            }
            else color = timer.Value;
        }

        [System.Serializable]
        public struct KeyPoint {
            public Color Color;
            public float Interval;
        }

    #if UNITY_EDITOR
        private void Reset() {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            graphics = GetComponentsInChildren<Graphic>();
        }
    #endif
    }
}