using System.Collections;
using System.Collections.Generic;
using MPack;
using UnityEngine;
using UnityEngine.UI;


public class TutorialAnimation : MonoBehaviour
{
    [SerializeField]
    private Image image;
    [SerializeField]
    private Timer intervalTimer;
    [SerializeField]
    private Sprite[] sprites;
    [SerializeField]
    private ValueWithEnable<float> loopDelay;

    private int _currentAnimationSpriteIndex;
    private Timer _delayLoopTimer;

    void Awake()
    {
        intervalTimer.Running = true;
    }

    void Start()
    {
        image.sprite = sprites[0];
    }

    void Update()
    {
        if (intervalTimer.Running && intervalTimer.UpdateEnd)
        {
            intervalTimer.Reset();

            if (++_currentAnimationSpriteIndex >= sprites.Length)
            {
                intervalTimer.Running = false;

                if (loopDelay.Enable)
                {
                    _delayLoopTimer = new Timer(loopDelay.Value);
                }
                return;
            }

            image.sprite = sprites[_currentAnimationSpriteIndex];
        }

        if (_delayLoopTimer.Running && _delayLoopTimer.UpdateEnd)
        {
            _delayLoopTimer.Running = false;
            intervalTimer.Reset();
            _currentAnimationSpriteIndex = 0;
            image.sprite = sprites[_currentAnimationSpriteIndex];
        }
    }
}
