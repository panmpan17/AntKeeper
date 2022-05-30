using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using DigitalRuby.Tween;


public class PauseMenu : MonoBehaviour
{
    public static PauseMenu ins;

    [SerializeField]
    private GameObject resumeButton;
    [SerializeField]
    private GameObject settingButton;

    private InputSystemUIInputModule uIInputModule;

    public event System.Action OnPaused;
    public event System.Action OnResumed;

    private Canvas _canvas;
    private CanvasGroup _canvasGroup;


    void Awake()
    {
        ins = this;
        _canvas = GetComponent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.interactable = false;
        _canvas.enabled = false;

        uIInputModule = FindObjectOfType<InputSystemUIInputModule>();
    }

    void OnDisable()
    {
        uIInputModule.cancel.action.performed -= OnCancelPerformed;
    }

    public void Pause()
    {
        EventSystem.current.SetSelectedGameObject(resumeButton);
        Time.timeScale = 0;
        _canvas.enabled = true;
        OnPaused?.Invoke();
        _canvasGroup.interactable = true;

        uIInputModule.cancel.action.performed += OnCancelPerformed;
    }

    public void Resume()
    {
        Time.timeScale = 1;
        _canvas.enabled = false;
        OnResumed?.Invoke();
        _canvasGroup.interactable = false;
        EventSystem.current.SetSelectedGameObject(null);

        uIInputModule.cancel.action.performed -= OnCancelPerformed;
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        // SceneManager.LoadScene("TempMainMenu");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnCancelPerformed(InputAction.CallbackContext callbackContext)
    {
        Resume();
    }

    public void OnSettingClose()
    {
        if (_canvas.enabled)
            EventSystem.current.SetSelectedGameObject(settingButton);
    }
}
