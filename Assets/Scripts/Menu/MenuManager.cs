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

    private Stack<AbstractMenu> openedMenus = new Stack<AbstractMenu>(5);

    [SerializeField]
    private CanvasGroup graidentCanvasGroup;
    private Canvas _canvas;

    void Awake()
    {
        ins = this;

        _canvas = GetComponent<Canvas>();

        openedMenus.Push(menus[0]);
        menus[0].InstantOpen();

        for (int i = 1; i < menus.Length; i++)
            menus[i].InstantClose();
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
        FloatTween tween = gameObject.Tween("FadeOut", 0, 1, fadeDuration.Value, TweenScaleFunctions.QuadraticEaseInOut, ChangeGradientAlpha);
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
        openedMenus.Pop().FadeOutToRight();

        openedMenus.Peek().FadeInFromLeft();
    }
#endregion
}
