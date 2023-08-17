using System;
using DG.Tweening;
using UnityEngine;

public interface IAnimation
{
    IAnimation OnStart();
    
    IAnimation OnReverse();
    IAnimation OnStop();
    IAnimation SetStartCompletedCallback(Action action);
    IAnimation SetReverseCompletedCallBack(Action action);
}
