using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;
using DigitalRuby.Tween;


public class PromptDisplay : MonoBehaviour
{
    public static PromptDisplay ins;

    [SerializeField]
    private Transform parent;
    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private bool updatePosition;

    [SerializeField]
    private SpriteRenderer iconRenderer;
    [SerializeField]
    private LanguageText descriptionText;
    private SpriteRenderer _background;

    [SerializeField]
    private float fadeDuration;
    
    private PromptItem _currentPrompt;
    private Coroutine _waitFadeOut;
    private FloatTween _fadeTween;

    void Awake()
    {
        ins = this;

        _background = GetComponent<SpriteRenderer>();
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (updatePosition)
        {
            transform.position = parent.position + offset;
        }
    }

    public void Display(PromptItem item)
    {
        if (_waitFadeOut != null)
        {
            StopCoroutine(_waitFadeOut);
            _waitFadeOut = null;
        }
        if (_fadeTween != null)
        {
            TweenFactory.RemoveTween(_fadeTween, TweenStopBehavior.DoNotModify);
        }

        gameObject.SetActive(true);

        _currentPrompt = item;
        iconRenderer.sprite = item.Icon;
        descriptionText.ChangeId(item.DescriptionLanguageID);

        _fadeTween = gameObject.Tween("FadeIn", 0, 1, fadeDuration, TweenScaleFunctions.CubicEaseInOut, UpdateTween, FadeInFinished);
    }

    void UpdateTween(ITween<float> tweenData)
    {
        Color backgroundColor = _background.color;
        Color iconColor = iconRenderer.color;
        Color textColor = descriptionText.TextMeshPro.color;

        backgroundColor.a = tweenData.CurrentValue;
        iconColor.a = tweenData.CurrentValue;
        textColor.a = tweenData.CurrentValue;

        _background.color = backgroundColor;
        iconRenderer.color = iconColor;
        descriptionText.TextMeshPro.color = textColor;
    }

    void FadeInFinished(ITween<float> tweenData)
    {
        _fadeTween = null;
        _waitFadeOut = StartCoroutine(WaitForFadeOut());
    }

    IEnumerator WaitForFadeOut()
    {
        yield return new WaitForSeconds(_currentPrompt.DisplayDuration);
        _waitFadeOut = null;
        _fadeTween = gameObject.Tween("FadeOut", 1, 0, fadeDuration, TweenScaleFunctions.CubicEaseInOut, UpdateTween, FadeOutFinished);
    }

    void FadeOutFinished(ITween<float> tweenData)
    {
        gameObject.SetActive(false);
        _currentPrompt = null;
        _fadeTween = null;
    }
}
