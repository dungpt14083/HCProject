using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
public class OnWebsocketMessageEvent : MonoBehaviour
{


    public static OnWebsocketMessageEvent ins;

    private void Awake()
    {
        if (ins == null)
        {
            ins = this;

        }
        else
        {
            Debug.LogError("MULTIINSTANCE" + name);
            Destroy(this);
        }
    }


    public Action updateUserData_Type_1;
    public Event eventTest;
    public string data;
    public GameObject thisGO;


    public void RegisterOnMessageEvent(int eventID, Action cb)
    {



    }
    public SocketMessageEvent updateUserDataUIEvent;





    [Serializable]
    public class SocketMessageEvent : UnityEvent<string> { }





    [Button]
    public void CallEvent()
    {
        updateUserDataUIEvent.Invoke(data);
    }



}
