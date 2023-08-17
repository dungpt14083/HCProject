using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventMono : MonoBehaviour
{ // use to check mono behavior will listener event
    public virtual void Awake()
    {
        EventPushManager.AddListener(this);
    }

    public virtual void OnDestroy()
    {
        EventPushManager.RemoveListener(this);
    }
} // EventMono
