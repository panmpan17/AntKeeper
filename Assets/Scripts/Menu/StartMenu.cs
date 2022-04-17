using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{

#if UNITY_EDITOR
    [Header("Editor Only")]
    [SerializeField]
    private bool skipMenu;
#endif

    private Canvas _canvas;

    void Awake()
    {
        _canvas = GetComponent<Canvas>();
    }

    void Start()
    {
        if (skipMenu)
        {
            StartButtonPressed();
        }
    }

    public void StartButtonPressed()
    {
        GameManager.ins.StartGame();
        _canvas.enabled = enabled = false;
    }

    public void HelpButtonPressed()
    {}
}
