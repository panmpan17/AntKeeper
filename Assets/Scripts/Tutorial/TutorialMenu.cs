using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using MPack;

public class TutorialMenu : MonoBehaviour
{
    [SerializeField]
    private Button previousButton;
    [SerializeField]
    private Button nextButton;
    [SerializeField]
    private GameObject closeIndicate;
    [SerializeField]
    private GameObject[] tutorialSections;
    private int currentTutorialIndex;

    [SerializeField]
    private EventReference closeEvent;

#if UNITY_EDITOR
    [Header("Editor Only")]
    [SerializeField]
    private bool openOnStart;
#endif

    private Canvas _canvas;
    private Navigation nextNullNavigation;
    private Navigation nextNormalNavigation;

    void Awake()
    {
        _canvas = GetComponent<Canvas>();

        previousButton.onClick.AddListener(PreviousTutorial);
        nextButton.onClick.AddListener(NextTutorial);
        closeIndicate.SetActive(false);

        enabled = _canvas.enabled = false;


        nextNullNavigation = nextButton.navigation;
        nextNullNavigation.selectOnLeft = null;
        nextNormalNavigation = nextButton.navigation;

#if UNITY_EDITOR
        if (openOnStart)
            Open();
#endif
    }

    public void PreviousTutorial()
    {
        tutorialSections[currentTutorialIndex].SetActive(false);

        if (--currentTutorialIndex < 0)
        {
            currentTutorialIndex = 0;
        }

        if (currentTutorialIndex == 0)
        {
            previousButton.interactable = false;
            EventSystem.current.SetSelectedGameObject(nextButton.gameObject);
            nextButton.navigation = nextNullNavigation;
        }

        tutorialSections[currentTutorialIndex].SetActive(true);
    }

    public void NextTutorial()
    {
        tutorialSections[currentTutorialIndex].SetActive(false);

        if (++currentTutorialIndex >= tutorialSections.Length)
        {
            CloseWindow();
            return;
        }

        tutorialSections[currentTutorialIndex].SetActive(true);
        previousButton.interactable = true;
        nextButton.navigation = nextNormalNavigation;
        closeIndicate.SetActive(currentTutorialIndex == tutorialSections.Length - 1);
    }

    public void Open()
    {
        currentTutorialIndex = 0;
        tutorialSections[0].SetActive(true);
        for (int i = 1; i < tutorialSections.Length; i++)
        {
            tutorialSections[i].SetActive(false);
        }

        previousButton.interactable = false;
        nextButton.navigation = nextNullNavigation;

        EventSystem.current.SetSelectedGameObject(nextButton.gameObject);
        enabled = _canvas.enabled = true;
    }

    void CloseWindow()
    {
        enabled = _canvas.enabled = false;
        closeEvent.Invoke();
    }
}
