using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName="MPack/Event Reference", order=0)]
public class EventReference : ScriptableObject
{
    [System.NonSerialized]
    private List<EventDispatcher> eventDispatchers = new List<EventDispatcher>();

    public void Invoke()
    {
        for (int i = eventDispatchers.Count - 1; i >= 0; i--)
            eventDispatchers[i].DispatchEvent();
    }
    public void Invoke(bool booleanValue)
    {
        for (int i = eventDispatchers.Count - 1; i >= 0; i--)
            eventDispatchers[i].DispatchEvent(booleanValue);
    }

    public void RegisterEvent(EventDispatcher dispatcher)
    {
        eventDispatchers.Add(dispatcher);
    }
    public void UnregisterEvent(EventDispatcher dispatcher)
    {
        eventDispatchers.Remove(dispatcher);
    }
}
