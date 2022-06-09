using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MPack;
using TMPro;


public class StartMenu : AbstractMenu
{
    [SerializeField]
    private EventReference startCountDownEvent;
    [SerializeField]
    private EventReference openTutorialEvent;

#if UNITY_EDITOR
    [Header("Editor Only")]
    [SerializeField]
    private bool skipMenu;
#endif

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

#if UNITY_EDITOR
        if (skipMenu)
        {
            _canvasGroup.interactable = false;
            _canvasGroup.alpha = 0;

            EventSystem.current.SetSelectedGameObject(null);
            MenuManager.ins.InstantClose(startCountDownEvent.Invoke);
        }
#endif
    }

    public void StartButtonPressed()
    {
        EventSystem.current.SetSelectedGameObject(null);
        _canvasGroup.interactable = false;
        MenuManager.ins.CloseMenu(startCountDownEvent.Invoke);
    }

    public void OpenTutorial()
    {
        _lastButton = EventSystem.current.currentSelectedGameObject;
        openTutorialEvent.Invoke();
    }

    public void ExitButtonPreseed()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    public void OnPopupClose()
    {
        EventSystem.current.SetSelectedGameObject(_lastButton);
    }
}
