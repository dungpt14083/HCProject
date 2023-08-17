using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Tweens.Plugins
{
    public class TweenFloat : TweenToolUI
    {
        [Space(5)]
        public float from = 0;
        public float to = 0;
        public bool isShow = true;
        public E_TypeFloat type;

        [Space(10)]
        public UnityEvent OnStart;
        public UnityEvent OnComplete;

        private float next;
        private float previous;

        public override void OnBegin()
        {
            base.OnBegin();

            next = to;
            previous = from;
        }

        public void Play()
        {
            OnPlay();
        }

        public void PlayBack()
        {
            OnBack();
        }
        public override Tween OnPlay()
        {
            base.OnPlay();

            tweener = DOVirtual.Float(from, to, duration, (value) =>
            {
                if (value >= to)
                {
                    switch (type)
                    {
                        case E_TypeFloat.ActiveObject:
                            target.gameObject.SetActive(isShow);
                            break;
                    }
                }
            })
                           .SetDelay(delay)
                           .SetLoops(loop, loopType)
                           .SetEase(ease)
                           .OnStart(() => { OnStart.Invoke(); })
                           .OnComplete(() => { OnComplete.Invoke(); });

            return tween;
        }

        public override void OnBack()
        {
            base.OnBack();

            tweener = DOVirtual.Float(from, to, duration, (value) =>
            {
                if (value >= to)
                {
                    switch (type)
                    {
                        case E_TypeFloat.ActiveObject:
                            target.gameObject.SetActive(!isShow);
                            break;
                    }
                }      
            })
                          .SetDelay(delay)
                          .SetLoops(loop, loopType)
                          .SetEase(ease)
                          .OnStart(() => { OnStart.Invoke(); })
                          .OnComplete(() => { OnComplete.Invoke(); });
        }

        public override void OnStop()
        {
            base.OnStop();

            base.target.DOKill();
        }
    }

    [System.Serializable]
    public class ParticleItem
    {
        public float startsize1;
        public float startsize2;
        public float emissionRateOverTime;
    }

    public enum E_TypeFloat
    {
        ActiveObject,
    }
}


