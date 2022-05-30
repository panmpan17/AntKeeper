using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DigitalRuby.Tween;

public class ChoiceSwitch : Selectable
{
    [SerializeField]
    private Image left;
    private Vector2 leftAnchorPosition;
    [SerializeField]
    private Image right;
    private Vector2 rightAnchorPosition;
    [SerializeField]
    private Vector2 anchorOffset;
    [SerializeField]
    private float tweenTime;

    [SerializeField]
    private UnityEvent leftEvent;
    [SerializeField]
    private UnityEvent rightEvent;

    protected override void Start()
    {
        base.Start();

        Color color = left.color;
        color.a = 0;
        left.color = color;

        color = right.color;
        color.a = 0;
        right.color = color;

        leftAnchorPosition = left.rectTransform.anchoredPosition;
        rightAnchorPosition = right.rectTransform.anchoredPosition;
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        FloatTween tween = gameObject.Tween("OnSelect", 0, 1, tweenTime, TweenScaleFunctions.CubicEaseIn, ChangeArrowAlpha);
        tween.TimeFunc = TweenFactory.TimeFuncUnscaledDeltaTimeFunc;
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        FloatTween tween = gameObject.Tween("OnDeselect", 1, 0, tweenTime, TweenScaleFunctions.CubicEaseIn, ChangeArrowAlpha);
        tween.TimeFunc = TweenFactory.TimeFuncUnscaledDeltaTimeFunc;
    }

    public override void OnMove(AxisEventData eventData)
    {
        FloatTween tween;
        switch (eventData.moveDir)
        {
            case MoveDirection.Left:
                tween = gameObject.Tween("Left", 0, 1, tweenTime, TweenScaleFunctions.QuarticEaseOut, ChangeLeftArrowPosition, ResetLeftArrowPosition);
                tween.TimeFunc = TweenFactory.TimeFuncUnscaledDeltaTimeFunc;
                leftEvent.Invoke();
                break;

            case MoveDirection.Right:
                tween = gameObject.Tween("Right", 0, 1, tweenTime, TweenScaleFunctions.QuarticEaseOut, ChangeRightArrowPosition, ResetRightArrowPosition);
                tween.TimeFunc = TweenFactory.TimeFuncUnscaledDeltaTimeFunc;
                rightEvent.Invoke();
                break;

            case MoveDirection.Up:
            case MoveDirection.Down:
                base.OnMove(eventData);
                break;
        }
    }

    void ChangeArrowAlpha(ITween<float> eventData)
    {
        Color color = left.color;
        color.a = eventData.CurrentValue;
        left.color = color;

        color = right.color;
        color.a = eventData.CurrentValue;
        right.color = color;
    }

    void ChangeLeftArrowPosition(ITween<float> eventData)
    {
        left.rectTransform.anchoredPosition = Vector2.Lerp(leftAnchorPosition, leftAnchorPosition - anchorOffset, eventData.CurrentValue);
    }

    void ResetLeftArrowPosition(ITween<float> eventData)
    {
        left.rectTransform.anchoredPosition = leftAnchorPosition;
    }

    void ChangeRightArrowPosition(ITween<float> eventData)
    {
        right.rectTransform.anchoredPosition = Vector2.Lerp(rightAnchorPosition, rightAnchorPosition + anchorOffset, eventData.CurrentValue);
    }

    void ResetRightArrowPosition(ITween<float> eventData)
    {
        right.rectTransform.anchoredPosition = rightAnchorPosition;
    }
}
