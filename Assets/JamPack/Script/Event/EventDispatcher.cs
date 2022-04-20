using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventDispatcher : MonoBehaviour
{
    [SerializeField]
    private EventReference eventReference;

    public UnityEvent Event;

    void OnEnable()
    {
        eventReference.RegisterEvent(this);
    }
    void OnDisable()
    {
        eventReference.UnregisterEvent(this);
    }

    public void DispatchEvent()
    {
        Event.Invoke();
    }
}
