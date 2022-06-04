using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MPack;
using TMPro;


public class StartMenu : AbstractMenu
{
    // [Header("Tutorial")]
    // [SerializeField]
    // private GameObject tutorialButton;
    // [SerializeField]
    // private EventReference openTutorialEvent;
    // [SerializeField]
    // private EventReference tutorialBackEvent;


    // [Header("Setting")]
    // [SerializeField]
    // private GameObject settingButton;
    // [SerializeField]
    // private EventReference openSettingEvent;
    // [SerializeField]
    // private EventReference settingBackEvent;
    
    // [Header("Control")]
    // [SerializeField]
    // private GameObject helpButton;
    // [SerializeField]
    // private GameObject helpMenu;
    // [SerializeField]
    // private GameObject helpMenuClose;

    [Header("Start Countdown")]
    [SerializeField]
    private TextMeshProUGUI startCountDownText;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip beepAudio;
    [SerializeField]
    private AudioClip startAudio;

    [SerializeField]
    private int readyTextLanguageID;
    [SerializeField]
    private int startTextLanguageID;

#if UNITY_EDITOR
    [Header("Editor Only")]
    [SerializeField]
    private bool skipMenu;
    [SerializeField]
    private bool skipStartCountDown;
    [SerializeField]
    private ValueWithEnable<bool> showVirtualStick;
#endif

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

// #if UNITY_EDITOR
//         if (skipMenu)
//         {
//             EventSystem.current.SetSelectedGameObject(null);
//             menuCanvasGroup.interactable = false;
//             fadeTimer.Running = false;
//             StartCoroutine(StartCountDown());
//             menuCanvasGroup.alpha = 0;
//         }
// #endif
    }

    IEnumerator StartCountDown()
    {
#if UNITY_EDITOR
        if (skipStartCountDown)
        {
            CameraManager.ins.SwitchCamera(CameraManager.CameraState.FollowPlayer);
            StartGame();
            yield break;
        }
#endif

        var waitOneSec = new WaitForSeconds(1.2f);

        yield return new WaitForSeconds(0.5f);
        startCountDownText.text = LanguageMgr.GetTextById(readyTextLanguageID);
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
        startCountDownText.text = LanguageMgr.GetTextById(startTextLanguageID);
        audioSource.PlayOneShot(startAudio);

        yield return waitOneSec;
        startCountDownText.gameObject.SetActive(false);
        StartGame();
    }

    void StartGame()
    {
        HUDManager.ins.enabled = true;
        GameManager.ins.enabled = true;
        gameObject.SetActive(false);
    }

    // void Update()
    // {
    //     if (fadeTimer.Running)
    //     {
    //         if (fadeTimer.UpdateEnd)
    //         {
    //             fadeTimer.Running = false;
    //             StartCoroutine(StartCountDown());
    //         }
    //         menuCanvasGroup.alpha = 1 - fadeTimer.Progress;
    //     }
    // }

    public void StartButtonPressed()
    {
        EventSystem.current.SetSelectedGameObject(null);
        _canvasGroup.interactable = false;
        MenuManager.ins.CloseMenu(delegate {
            StartCoroutine(StartCountDown());
        });
    }

    // public void HelpButtonPressed()
    // {
    //     helpMenu.SetActive(true);
    //     EventSystem.current.SetSelectedGameObject(helpMenuClose);
    // }

    // public void CloseHelpMenu()
    // {
    //     helpMenu.SetActive(false);
    //     EventSystem.current.SetSelectedGameObject(helpButton);
    // }

    // public void TutorialButtonPressed()
    // {
    //     _lastButton = EventSystem.current.currentSelectedGameObject;
    //     openTutorialEvent.Invoke();
    // }

    // public void SettingButtonPressed()
    // {
    //     _lastButton = EventSystem.current.currentSelectedGameObject;
    //     openSettingEvent.Invoke();
    // }

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
