#if UNITY_IOS || UNITY_ANDROID || UNITY_WEBGL
#define TURN_ON_MOBILE_CONTROL_AT_START
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MPack;
using TMPro;


public class StartMenu : MonoBehaviour
{
    [SerializeField]
    [ShortTimer]
    private Timer fadeTimer;

    [SerializeField]
    private CanvasGroup menuCanvasGroup;
    [SerializeField]
    private GameObject helpButton;
    [SerializeField]
    private GameObject helpMenu;
    [SerializeField]
    private GameObject helpMenuClose;

    [SerializeField]
    private ToggleSwitch mobileControlSwitch;

    [Header("Start Countdown")]
    [SerializeField]
    private TextMeshProUGUI startCountDownText;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip beepAudio;
    [SerializeField]
    private AudioClip startAudio;

#if UNITY_EDITOR
    [Header("Editor Only")]
    [SerializeField]
    private bool skipMenu;
    [SerializeField]
    private bool skipStartCountDown;
    [SerializeField]
    private ValueWithEnable<bool> showVirtualStick;
#endif

    private Canvas _canvas;

    void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = true;

        helpMenu.SetActive(false);
        fadeTimer.Running = false;

        startCountDownText.text = "";
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
            fadeTimer.Running = false;
            StartCoroutine(StartCountDown());
            menuCanvasGroup.alpha = 0;
        }
#endif
    }

    IEnumerator StartCountDown()
    {
        if (skipStartCountDown)
        {
            CameraManager.ins.SwitchCamera(CameraManager.CameraState.FollowPlayer);
            StartGame();
            yield break;
        }

        var waitOneSec = new WaitForSeconds(1.2f);

        yield return new WaitForSeconds(0.5f);
        startCountDownText.text = "Ready?";
        yield return waitOneSec;
        CameraManager.ins.SwitchCamera(CameraManager.CameraState.FollowPlayer);
        audioSource.PlayOneShot(beepAudio);
        startCountDownText.text = "3";

        yield return waitOneSec;
        audioSource.PlayOneShot(beepAudio);
        startCountDownText.text = "2";

        yield return waitOneSec;
        audioSource.PlayOneShot(beepAudio);
        startCountDownText.text = "1";

        yield return waitOneSec;
        startCountDownText.text = "Start!";
        audioSource.PlayOneShot(startAudio);

        yield return waitOneSec;
        startCountDownText.gameObject.SetActive(false);
        StartGame();
    }

    void StartGame()
    {
        GameManager.ins.enabled = true;
        HUDManager.ins.enabled = true;
        _canvas.enabled = enabled = false;
    }

    void Update()
    {
        if (fadeTimer.Running)
        {
            if (fadeTimer.UpdateEnd)
            {
                fadeTimer.Running = false;
                StartCoroutine(StartCountDown());
            }
            menuCanvasGroup.alpha = 1 - fadeTimer.Progress;
        }
    }

    public void StartButtonPressed()
    {
        menuCanvasGroup.interactable = false;
        fadeTimer.Reset();
    }

    public void HelpButtonPressed()
    {
        helpMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(helpMenuClose);
    }

    public void CloseHelpMenu()
    {
        helpMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(helpButton);
    }
}
