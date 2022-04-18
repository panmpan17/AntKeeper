#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MPack.Aseprite {
    public class AseAnimator : MonoBehaviour
    {
        [SerializeField]
        private AseAnimation[] animations;
        private int animI = -1, animKeyI;
        private float timer;
        private bool stop;

        private SpriteRenderer spriteRenderer;
        private Image image;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            Play(0);
        }

        private void Update()
        {
            if (stop)
                return;

            timer += Time.deltaTime;
            if (timer > animations[animI].Points[animKeyI].Time) {
                timer = 0;
                animKeyI++;

                if (animKeyI >= animations[animI].Points.Length) {
                    if (animations[animI].Loop)
                        animKeyI = 0;
                    else {
                        stop = true;
                        return;
                    }
                }

                spriteRenderer.sprite = animations[animI].Points[animKeyI].Sprite;
            }
        }

        public void Play(int index) {
            if (animI == index)
                return;

            animI = index;
            animKeyI = 0;

            stop = false;
            spriteRenderer.sprite = animations[animI].Points[animKeyI].Sprite;
        }

        public void PlayAnimation(string name)
        {
            int index = 0;
            bool found = false;

            for (; index < animations.Length; index++)
            {
                if (animations[index].Name == name)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
            #if UNITY_EDITOR
                Debug.LogWarningFormat("{0} animation not found", name);
            #endif
                return;
            }

            if (animI == index)
                return;

            animI = index;
            animKeyI = 0;

            stop = false;
            spriteRenderer.sprite = animations[animI].Points[animKeyI].Sprite;
        }
    }
}