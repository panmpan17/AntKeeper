using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class EventDispatcher : MonoBehaviour
{
    [SerializeField]
    private EventReference eventReference;

    public UnityEvent Event;
    public UnityEventWithBoolean BooleanEvent;
    [FormerlySerializedAs("FLoatEvent")]
    public UnityEventWithFloat FloatEvent;

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
    public void DispatchEvent(float floatValue)
    {
        FloatEvent.Invoke(floatValue);
    }

    [System.Serializable]
    public class UnityEventWithBoolean : UnityEvent<bool> {}
    [System.Serializable]
    public class UnityEventWithFloat : UnityEvent<float> { }
}
