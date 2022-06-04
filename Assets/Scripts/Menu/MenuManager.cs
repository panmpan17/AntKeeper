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

    void FadeIn()
    {}

    public void CloseMenu(System.Action finishEvent)
    {
        gameObject.Tween("FadeOut", 1, 0, fadeDuration.Value, TweenScaleFunctions.QuadraticEaseInOut, ChangeGradientAlpha, delegate {
            _canvas.enabled = false;
            finishEvent?.Invoke();
        });
    }

    void ChangeGradientAlpha(ITween<float> tweenData)
    {
        graidentCanvasGroup.alpha = tweenData.CurrentValue;
    }

#region  Menu switching
    public void OpenMenu(string name)
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
