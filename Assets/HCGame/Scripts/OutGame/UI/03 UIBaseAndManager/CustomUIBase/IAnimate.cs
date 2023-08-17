using System;

public interface IAnimate
{
    void OnOpen();
    void OnStop();
    void OnClose(Action onComplete = null);
}