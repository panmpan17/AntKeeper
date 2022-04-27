using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventDispatcher : MonoBehaviour
{
    [SerializeField]
    private EventReference eventReference;

    public UnityEvent Event;
    public UnityEventWithBoolean BooleanEvent;

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
    public void DispatchEvent(bool booleanValue)
    {
        BooleanEvent.Invoke(booleanValue);
    }

    [System.Serializable]
    public class UnityEventWithBoolean : UnityEvent<bool> {}
}
