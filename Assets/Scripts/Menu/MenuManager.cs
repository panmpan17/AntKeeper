using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;
using MPack;

public class MenuManager : MonoBehaviour
{
    public static MenuManager ins;


    [SerializeField]
    private FloatReference fadeDuration;

    [SerializeField]
    private AbstractMenu[] menus;
    [SerializeField]
    private bool openFirstOnStart;

    private Stack<AbstractMenu> openedMenus = new Stack<AbstractMenu>(5);

    [SerializeField]
    private CanvasGroup graidentCanvasGroup;
    private Canvas _canvas;

    [Header("Audio")]
    [SerializeField]
    private AudioClip buttonOnSelectSound;
    [SerializeField]
    private AudioClip buttonOnSubmitSound;
    [SerializeField]
    private AudioClip switchSound;
    [SerializeField]
    private AudioSource audioSource;

    void Awake()
    {
        ins = this;

        _canvas = GetComponent<Canvas>();

        if (openFirstOnStart)
        {
            openedMenus.Push(menus[0]);
            menus[0].InstantOpen();

            for (int i = 1; i < menus.Length; i++)
                menus[i].InstantClose();
        }
        else
        {
            _canvas.enabled = false;
            graidentCanvasGroup.alpha = 0;
            for (int i = 0; i < menus.Length; i++)
                menus[i].InstantClose();
        }
    }

    void ChangeGradientAlpha(ITween<float> tweenData)
    {
        graidentCanvasGroup.alpha = tweenData.CurrentValue;
    }


#region Menu open and close
    public void OpenMenu(AbstractMenu menu)
    {
        _canvas.enabled = true;
        openedMenus.Push(menu);
        menu.FadeInFromLeft();
        FloatTween tween = gameObject.Tween("FadeIn", 0, 1, fadeDuration.Value, TweenScaleFunctions.QuadraticEaseInOut, ChangeGradientAlpha);
        tween.TimeFunc = TweenFactory.TimeFuncUnscaledDeltaTimeFunc;
    }
    public void OpenMenu(string menuName)
    {
        AbstractMenu menu = null;
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].name == menuName)
            {
                menu = menus[i];
                break;
            }
        }

        if (menu == null)
        {
            return;
        }

        _canvas.enabled = true;
        openedMenus.Push(menu);
        menu.FadeInFromLeft();
        FloatTween tween = gameObject.Tween("FadeIn", 0, 1, fadeDuration.Value, TweenScaleFunctions.QuadraticEaseInOut, ChangeGradientAlpha);
        tween.TimeFunc = TweenFactory.TimeFuncUnscaledDeltaTimeFunc;
    }


    public void CloseMenu(System.Action finishEvent)
    {
        openedMenus.Peek().FadeOutToLeft();
        openedMenus.Clear();
        gameObject.Tween("FadeOut", 1, 0, fadeDuration.Value, TweenScaleFunctions.QuadraticEaseInOut, ChangeGradientAlpha, delegate {
            _canvas.enabled = false;
            finishEvent?.Invoke();
        });
    }

#if UNITY_EDITOR
    public void InstantClose(System.Action finishEvent)
    {
        openedMenus.Peek().InstantClose();
        openedMenus.Clear();
        _canvas.enabled = false;
        finishEvent?.Invoke();
    }
#endif
#endregion


#region  Menu switching
    public void SwitchMenu(string name)
    {
        PlayButtonOnSubmitSound();
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].name == name)
            {
                openedMenus.Peek().FadeOutToLeft();

                openedMenus.Push(menus[i]);
                menus[i].FadeInFromRight();
                break;
            }
        }
    }

    public void BackToLastMenu()
    {
        PlayButtonOnSubmitSound();
        openedMenus.Pop().FadeOutToRight();

        openedMenus.Peek().FadeInFromLeft();
    }
#endregion


    public void PlayButtonOnSelectSound()
    {
        audioSource.PlayOneShot(buttonOnSelectSound);
    }

    public void PlayButtonOnSubmitSound()
    {
        audioSource.PlayOneShot(buttonOnSubmitSound);
    }

    public void PlaySwitchSound()
    {
        audioSource.PlayOneShot(switchSound);
    }
}
