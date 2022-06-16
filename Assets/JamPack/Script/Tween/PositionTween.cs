#pragma warning disable 0649

using UnityEngine;


namespace MPack {
    public enum TweenType { Loop, PingPong, BackwardLoop, ForwardOnce, BackwardOnce }

    public class PositionTween : MonoBehaviour
    {
        [SerializeField]
        private TweenType type;

        [SerializeField]
        private bool uselocalPosition = false;

        [SerializeField]
        private bool sameInterval = true;
        [SerializeField]
        private float interval = 0.2f;

        [SerializeField]
        private KeyPoint[] keyPoints;
        private int nextKeyPointIndex = 1;
        private bool indexForward = true;

        private Vector3LerpTimer timer;

        private void Awake()
        {
            if (sameInterval) timer = new Vector3LerpTimer(interval);
            else timer = new Vector3LerpTimer(keyPoints[0].Interval);

            int currentKeyPointIndex = 0;
            if  (type == TweenType.BackwardLoop) {
                currentKeyPointIndex = keyPoints.Length - 2;
                nextKeyPointIndex = currentKeyPointIndex - 1;
                indexForward = false;
            }

            timer.From = keyPoints[currentKeyPointIndex].Position;
            timer.To = keyPoints[nextKeyPointIndex].Position;

            if (uselocalPosition) transform.localPosition = timer.From;
            else transform.position = timer.From;
        }

        private void CalculateNextIndex()
        {
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
                if (uselocalPosition) transform.localPosition = timer.Value;
                else transform.position = timer.Value;

                timer.Timer.Reset();
                if (!sameInterval) timer.Timer.TargetTime = keyPoints[nextKeyPointIndex].Interval;

                timer.From = keyPoints[nextKeyPointIndex].Position;
                CalculateNextIndex();
                timer.To = keyPoints[nextKeyPointIndex].Position;
            }
            else {
                if (uselocalPosition) transform.localPosition = timer.Value;
                else transform.position = timer.Value;
            }
        }

        [System.Serializable]
        private struct KeyPoint
        {
            public Vector3 Position;
            public float Interval;
        }
    
    #if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            Vector3 delta = Vector3.zero;
            if (uselocalPosition && transform.parent != null) delta = transform.parent.position;

            for (int i = 0; i < keyPoints.Length - 1; i++) {
                Gizmos.DrawSphere(keyPoints[i].Position + delta, 0.2f);
                Gizmos.DrawLine(keyPoints[i].Position + delta, keyPoints[i + 1].Position + delta);
            }
            Gizmos.DrawSphere(keyPoints[keyPoints.Length - 1].Position + delta, 0.2f);
        }
    #endif
    }
}