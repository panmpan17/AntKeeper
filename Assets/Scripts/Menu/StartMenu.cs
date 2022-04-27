#if UNITY_IOS || UNITY_ANDROID || UNITY_WEBGL
#define TURN_ON_MOBILE_CONTROL_AT_START
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MPack;


public class StartMenu : MonoBehaviour
{
    [SerializeField]
    [ShortTimer]
    private Timer fadeTimer;

    [SerializeField]
    private EventSystem eventSystem;
    [SerializeField]
    private GameObject helpButton;
    [SerializeField]
    private GameObject helpMenu;
    [SerializeField]
    private GameObject helpMenuClose;

    [SerializeField]
    private ToggleSwitch mobileControlSwitch;

#if UNITY_EDITOR
    [Header("Editor Only")]
    [SerializeField]
    private bool skipMenu;
    [SerializeField]
    private ValueWithEnable<bool> showVirtualStick;
#endif

    private Canvas _canvas;
    private CanvasGroup _canvasGroup;

    void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvas.enabled = true;

        helpMenu.SetActive(false);
        fadeTimer.Running = false;
    }

    IEnumerator Start()
    {
        bool showStick = false;
#if TURN_ON_MOBILE_CONTROL_AT_START
        showStick = true;
#endif
#if UNITY_EDITOR
        if (showVirtualStick.Enable)
            showStick = showVirtualStick.Value;
#endif
        mobileControlSwitch.ChangeState(showStick);

        yield return new WaitForEndOfFrame();

#if UNITY_EDITOR
        if (skipMenu)
        {
            // StartButtonPressed();
            GameManager.ins.StartGame();
            _canvasGroup.blocksRaycasts = _canvas.enabled = enabled = false;
        }
#endif
    }

    void Update()
    {
        if (fadeTimer.Running)
        {
            if (fadeTimer.UpdateEnd)
            {
                GameManager.ins.StartGame();
                _canvasGroup.blocksRaycasts = _canvas.enabled = enabled = false;
            }
            _canvasGroup.alpha = 1 - fadeTimer.Progress;
        }
    }

    public void StartButtonPressed()
    {
        _canvasGroup.interactable = false;
        fadeTimer.Reset();
    }

    public void HelpButtonPressed()
    {
        helpMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(helpMenuClose);
    }

    public void CloseHelpMenu()
    {
        helpMenu.SetActive(false);
        eventSystem.SetSelectedGameObject(helpButton);
    }
}
