using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DigitalRuby.Tween;


public class ButtonTween : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    // [SerializeField]
    [SerializeField]
    private float transitionTime = 0.4f;
    [SerializeField]
    private Vector2 positionOffset;
    [SerializeField]
    private Vector3 scale;
    private RectTransform _rectTransform;

    private Vector2 _startAnchorPosition;
    private Vector3 _originalScale;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _startAnchorPosition = _rectTransform.anchoredPosition;
        _originalScale = transform.localScale;
    }

    public void OnSelect(BaseEventData eventData)
    {
        gameObject.Tween(
            "Move", 0, 1, transitionTime,
            TweenScaleFunctions.QuadraticEaseIn, UpdateAnimation);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        gameObject.Tween(
            "MoveBack", 1, 0, transitionTime,
            TweenScaleFunctions.QuadraticEaseIn, UpdateAnimation);
    }

    void UpdateAnimation(ITween<float> tweenData)
    {
        _rectTransform.anchoredPosition = Vector2.Lerp(_startAnchorPosition, _startAnchorPosition + positionOffset, tweenData.CurrentValue);
        _rectTransform.localScale = Vector3.Lerp(_originalScale, scale, tweenData.CurrentValue);
    }
}
