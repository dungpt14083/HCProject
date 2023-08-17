using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Executors : SingletonNotAbleNull<Executors>
{
    private readonly List<Coroutine> _savedCoroutines = new List<Coroutine>();

    private Coroutine _RunCoroutineWithReturn(IEnumerator ie)
    {
        var ct = StartCoroutine(ie);
        _savedCoroutines.Add(ct);
        return ct;
    }

    private void _RunCoroutineWithoutReturn(IEnumerator ie)
    {
        StartCoroutine(ie);
    }

    private void _StopIEnumerator(Coroutine ct)
    {
        foreach (var m in _savedCoroutines)
        {
            if (m == ct)
            {
                if (m != null)
                {
                    StopCoroutine(m);
                    _savedCoroutines.Remove(m);
                    break;
                }
            }
        }
    }

    private void _StopAll()
    {
        StopAllCoroutines();
    }

    #region Public API

    public static Coroutine RunCoroutineWithReturn(IEnumerator ie)
    {
        return Instance._RunCoroutineWithReturn(ie);
    }

    public static void RunCoroutineWithoutReturn(IEnumerator ie)
    {
        Instance._RunCoroutineWithoutReturn(ie);
    }
    
    public static void StopIEnumerator(Coroutine ie)
    {
        Instance._StopIEnumerator(ie);
    }

    #endregion
}
