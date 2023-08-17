using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dispatcher : MonoBehaviour 
{
    private static Dispatcher instance;

    private List<Action> pending = new List<Action>();

    private List<Action> executeList = new List<Action>();
    public static Dispatcher Instance {
        get {
            return instance;
        }
    }

    public void Invoke (Action fn) {
        lock (this.pending) {
            this.pending.Add(fn);
        }
    }

    private void InvokePending () {
        lock (this.pending) {
            //Debug.LogWarning("pending: " + this.pending.Count);
            if (this.pending.Count <= 0) {
                return;
            } // if

            int maxcount = pending.Count / 100;
            int count = maxcount > 1 ? maxcount : 1;
            //int count = this.pending.Count > 10 ? 10 : this.pending.Count;
            for(int i = 0; i<count; i++) {
                this.pending[0]();
                this.pending.RemoveAt(0);
            } // for
        }
    }

    IEnumerator executePending () {
        lock (this.executeList) {
            foreach (Action action in this.executeList) {
                yield return null;
                action();
            }
            
            this.executeList.Clear();

        }
    }

    private void Awake () {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
    }

    private void Update () {
        this.InvokePending();
    }
}