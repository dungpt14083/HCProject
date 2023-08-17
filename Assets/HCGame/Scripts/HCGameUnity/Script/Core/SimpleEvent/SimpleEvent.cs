using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISimpleEvent
{
    public void HandleEvent(string eventName, object param);
}

public class SimpleEvent
{
    public static Dictionary<string, List<ISimpleEvent>> internalEventHandlers = new Dictionary<string, List<ISimpleEvent>>();

    public static void AddListener(string eventName, ISimpleEvent handler)
    {
        if (!internalEventHandlers.ContainsKey(eventName))
        {
            internalEventHandlers.Add(eventName, new List<ISimpleEvent>());
        }
        internalEventHandlers[eventName].Add(handler);
    }

    public static void Fire(string eventName, object param = null)
    {
        if (internalEventHandlers.ContainsKey(eventName))
        {
            foreach (var handle in internalEventHandlers[eventName])
            {
                handle.HandleEvent(eventName, param);
            }

        }
    }

    public static void RemoveEvent(string eventName, ISimpleEvent handler)
    {
        if (internalEventHandlers.ContainsKey(eventName) && internalEventHandlers[eventName].Contains(handler))
        {
            internalEventHandlers[eventName].Remove(handler);
        }
    }

    public static bool HasListener(string eventName)
    {
        if (internalEventHandlers.ContainsKey(eventName) && internalEventHandlers[eventName].Count > 0)
        {
            return true;
        }
        return false;
    }
}
