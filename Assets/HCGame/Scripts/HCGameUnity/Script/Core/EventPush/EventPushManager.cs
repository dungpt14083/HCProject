using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IHandle<T> {
    void Handle (T message);
}

public class EventPushManager {

    private static IList<EventMono> listeners = new List<EventMono>();

    public static void PushEvent<T> (T message) {
        lock (listeners)
        {
            for(int i =0; i < listeners.Count; i++)
            {
                if(listeners[i] != null)
                {
                    var handle = listeners[i].GetComponent<IHandle<T>>();
                    if (handle != null)
                    {
                        handle.Handle(message);
                    } // if
                } // if
            }// for

        } // lock
    } // PushEvent


    public static void AddListener(EventMono listener) {
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        } // if
    } //AddListener ()


    public static void RemoveListener(EventMono listener) {
        if (listeners.Contains(listener)) {
            listeners.Remove(listener);
        } // if
    } // RemoveListener ()



}

