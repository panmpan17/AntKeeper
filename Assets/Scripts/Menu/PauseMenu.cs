using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;


public class PauseMenu : AbstractMenu
{
    private InputSystemUIInputModule uIInputModule;

    [SerializeField]
    private EventReference pauseEvent;
    [SerializeField]
    private EventReference resumeEvent;


    protected override void Awake()
    {
        uIInputModule = FindObjectOfType<InputSystemUIInputModule>();
        base.Awake();
    }

    void OnDisable()
    {
        uIInputModule.cancel.action.performed -= OnCancelPerformed;
    }

    public void Pause()
    {
        Time.timeScale = 0;

        pauseEvent.Invoke();
        // uIInputModule.cancel.action.performed += OnCancelPerformed;
        MenuManager.ins.OpenMenu(this);
    }

    public void Resume()
    {
        Time.timeScale = 1;

        uIInputModule.cancel.action.performed -= OnCancelPerformed;
        MenuManager.ins.CloseMenu(resumeEvent.Invoke);
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnCancelPerformed(InputAction.CallbackContext callbackContext)
    {
        Resume();
    }
}
