using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MPack;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private LanguageData languageData;
    [SerializeField]
    private GameObject[] tutorialSections;
    private int currentTutorialIndex;

    private Canvas _canvas;

    public event System.Action OnClose;

    void Awake()
    {
        _canvas = GetComponent<Canvas>();
        LanguageMgr.AssignLanguageData(languageData);
    }

    void Start()
    {
        tutorialSections[0].SetActive(true);
        for (int i = 1; i < tutorialSections.Length; i++)
        {
            tutorialSections[i].SetActive(false);
        }
    }

    public void PreviousTutorial()
    {
        tutorialSections[currentTutorialIndex].SetActive(false);

        if (--currentTutorialIndex < 0)
        {
            currentTutorialIndex = 0;
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
    }

    void CloseWindow()
    {
        enabled = _canvas.enabled = false;
        OnClose?.Invoke();
    }
}
