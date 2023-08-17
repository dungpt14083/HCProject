using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXItem : MonoBehaviour
{
    [SerializeField] private Vector3 offsetPosition = new Vector3(0, -5.5f, 0);
    public Vector3 OffsetPosition
    {
        get
        {
            return offsetPosition;
        }
    }

    private ParticleSystem particleSystem;

    public Action FinishFX_Callback = null;

    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ParticleSystem.MainModule mainModule = particleSystem.main;
        mainModule.stopAction = ParticleSystemStopAction.Callback;
    }

    public void Init(Vector3 fxPosition)
    {
        transform.position = fxPosition + offsetPosition;
        particleSystem?.Play(true);
    }

    private void OnParticleSystemStopped()
    {
        Debug.Log("OnParticleSystemStopped!!!");
        FinishFX_Callback?.Invoke();
        PoolManager.Instance.releaseObject(gameObject);
        FinishFX_Callback = null;
    }
}
