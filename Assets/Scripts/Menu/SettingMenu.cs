using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MPack;

public class SettingMenu : MonoBehaviour
{
    [SerializeField]
    private Selectable firstSeletable;
    [SerializeField]
    private EventReference closeEvent;

    private Canvas _canvas;

    void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;
    }

    public void Open()
    {
        _canvas.enabled = true;
        EventSystem.current.SetSelectedGameObject(firstSeletable.gameObject);
    }

    public void Back()
    {
        _canvas.enabled = false;
        closeEvent.Invoke();
    }
}
