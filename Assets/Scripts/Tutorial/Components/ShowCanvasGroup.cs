using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;
using DigitalRuby.Tween;

public class ShowCanvasGroup : AbstractTutorialStep
{
    [SerializeField]
    private FloatReference fadeInTime;
    [SerializeField]
    private FloatReference fadeOutTime;
    [SerializeField]
    private ValueWithEnable<float> autoSkipSeconds;

    private CanvasGroup _canvasGroup;
    private Coroutine _waitAutomaticSkip;
    private bool _isSkipping;

    private FloatTween _fadeTween;
    
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
    }

    public override void StartStep()
    {
        _fadeTween = gameObject.Tween(gameObject, 0, 1, fadeInTime.Value, TweenScaleFunctions.CubicEaseInOut, ChangeCanvasGroupAlpha, FadeInFinished);
    }

    void ChangeCanvasGroupAlpha(ITween<float> tweenData)
    {
        _canvasGroup ??= GetComponent<CanvasGroup>();
        _canvasGroup.alpha = tweenData.CurrentValue;
    }

    void FadeInFinished(ITween<float> tweenData)
    {
        if (autoSkipSeconds.Enable)
        {
            _waitAutomaticSkip = StartCoroutine(C_WaitSecond(autoSkipSeconds.Value, Skip));
        }
        _fadeTween = null;
    }

    void FadeOutFinished(ITween<float> tweenData)
    {
        TutorialManager.ins.NextStep();
    }

    public override void Skip()
    {
        if (_isSkipping)
            return;
        _isSkipping = true;

        if (_fadeTween != null)
            TweenFactory.RemoveTween(_fadeTween, TweenStopBehavior.DoNotModify);

        if (_waitAutomaticSkip != null)
            StopCoroutine(_waitAutomaticSkip);
        gameObject.Tween(gameObject, 1, 0, fadeOutTime.Value, TweenScaleFunctions.CubicEaseInOut, ChangeCanvasGroupAlpha, FadeOutFinished);
    }


    public override void SkipWithoutCallNext()
    {
        if (_isSkipping)
            return;
        _isSkipping = true;

        if (_fadeTween != null)
            TweenFactory.RemoveTween(_fadeTween, TweenStopBehavior.DoNotModify);

        if (_waitAutomaticSkip != null)
            StopCoroutine(_waitAutomaticSkip);
        gameObject.Tween(gameObject, 1, 0, fadeOutTime.Value, TweenScaleFunctions.CubicEaseInOut, ChangeCanvasGroupAlpha);
    }
}
