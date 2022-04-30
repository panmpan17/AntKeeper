using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;


public class PauseMenu : MonoBehaviour
{
    public static PauseMenu ins;

    [SerializeField]
    private GameObject resumeButton;

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
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("TempMainMenu");
    }

    void OnCancelPerformed(InputAction.CallbackContext callbackContext)
    {
        Resume();
    }
}
