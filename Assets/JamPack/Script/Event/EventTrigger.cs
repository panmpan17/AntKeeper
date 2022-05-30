using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class EventTrigger : MonoBehaviour
{
    [SerializeField]
    private EventReference eventReference;

    public void TriggerEvent() => eventReference.Invoke();
    public void TriggerEvent(float floatValue) => eventReference.Invoke(floatValue);
    public void TriggerEvent(bool booleanValue) => eventReference.Invoke(booleanValue);
}
