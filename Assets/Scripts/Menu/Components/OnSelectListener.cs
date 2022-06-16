using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class OnSelectListener : MonoBehaviour, ISelectHandler
{
    [SerializeField]
    private UnityEvent onSelect;

    public void OnSelect(BaseEventData eventData)
    {
        onSelect.Invoke();
    }
}
