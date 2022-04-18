using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

#if UNITY_EDITOR
    [Header("Editor Only")]
    [SerializeField]
    private bool skipMenu;
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
