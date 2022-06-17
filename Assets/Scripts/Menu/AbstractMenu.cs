using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DigitalRuby.Tween;
using MPack;


public class AbstractMenu : MonoBehaviour
{
    [SerializeField]
    private FloatReference fadeDuration;

    [SerializeField]
    protected GameObject firstSelected;

    [SerializeField]
    private FloatReference fadeDeltaX;
    private Vector2 fadeLeftAnchoredPosition
    {
        get
        {
            Vector2 position = _originAnchorPosition;
            position.x -= fadeDeltaX.Value;
            return position;
        }
    }
    private Vector2 fadeRightAnchoredPosition
    {
        get
        {
            Vector2 position = _originAnchorPosition;
            position.x += fadeDeltaX.Value;
            return position;
        }
    }

    protected CanvasGroup _canvasGroup;
    protected GameObject _lastButton;
    protected RectTransform _rectTransform;

    private Vector2 _originAnchorPosition;

    protected virtual void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
        _originAnchorPosition = _rectTransform.anchoredPosition;
    }

    public void InstantClose()
    {
        gameObject.SetActive(false);
    }

    public void FadeOutToLeft()
    {
        _lastButton = EventSystem.current.currentSelectedGameObject;
        _canvasGroup.interactable = false;

        FloatTween tween = gameObject.Tween("FadeOutToLeft", 0, 1, fadeDuration.Value, TweenScaleFunctions.CubicEaseInOut,
        (tweenData) => {
            _rectTransform.anchoredPosition = Vector2.Lerp(
                _originAnchorPosition,
                fadeLeftAnchoredPosition,
                tweenData.CurrentValue);
            _canvasGroup.alpha = 1 - tweenData.CurrentValue;
        }, FadeOutFinished);
        tween.TimeFunc = TweenFactory.TimeFuncUnscaledDeltaTimeFunc;
    }

    public void FadeOutToRight()
    {
        _lastButton = null;
        _canvasGroup.interactable = false;

        FloatTween tween = gameObject.Tween("FadeOutToRight", 0, 1, fadeDuration.Value, TweenScaleFunctions.CubicEaseInOut,
        (tweenData) =>
        {
            _rectTransform.anchoredPosition = Vector2.Lerp(
                _originAnchorPosition,
                fadeRightAnchoredPosition,
                tweenData.CurrentValue);
            _canvasGroup.alpha = 1 - tweenData.CurrentValue;
        }, FadeOutFinished);
        tween.TimeFunc = TweenFactory.TimeFuncUnscaledDeltaTimeFunc;
    }

    void FadeOutFinished(ITween<float> tween)
    {
        gameObject.SetActive(false);
    }


    public void InstantOpen()
    {
        gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    public void FadeInFromLeft()
    {
        gameObject.SetActive(true);

        FloatTween tween = gameObject.Tween("FadeInFromLeft", 0, 1, fadeDuration.Value, TweenScaleFunctions.CubicEaseInOut,
        (tweenData) =>
        {
            _rectTransform.anchoredPosition = Vector2.Lerp(
                fadeLeftAnchoredPosition,
                _originAnchorPosition, tweenData.CurrentValue);
            _canvasGroup.alpha = tweenData.CurrentValue;
        }, FadeInFinished);
        tween.TimeFunc = TweenFactory.TimeFuncUnscaledDeltaTimeFunc;
    }

    public void FadeInFromRight()
    {
        gameObject.SetActive(true);

        FloatTween tween = gameObject.Tween("FadeInFromRight", 0, 1, fadeDuration.Value, TweenScaleFunctions.CubicEaseInOut,
        (tweenData) => {
            _rectTransform.anchoredPosition = Vector2.Lerp(
                fadeRightAnchoredPosition,
                _originAnchorPosition, tweenData.CurrentValue);
            _canvasGroup.alpha = tweenData.CurrentValue;
        }, FadeInFinished);
        tween.TimeFunc = TweenFactory.TimeFuncUnscaledDeltaTimeFunc;
    }

    protected virtual void FadeInFinished(ITween<float> tween)
    {
        _canvasGroup.interactable = true;
        EventSystem.current.SetSelectedGameObject(_lastButton != null ? _lastButton : firstSelected);
    }
}
