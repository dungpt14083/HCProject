using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Tweens.Plugins
{
    [RequireComponent(typeof(CanvasGroup))]
    public class TweenFade : TweenToolUI
    {
        [Space(5)]
        public CanvasGroup targetCanvas;
        public float value = 0;

        [Space(10)]
        public UnityEvent OnStart;
        public UnityEvent OnComplete;

        private float next;
        private float previous;

        public override void OnBegin()
        {
            base.OnBegin();

            if (targetCanvas == null)
                targetCanvas = GetComponent<CanvasGroup>();

            next = value;
            previous = targetCanvas.alpha;
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

            tween = targetCanvas.DOFade(next, duration)
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

            targetCanvas.DOFade(previous, duration)
                          .SetDelay(delay)
                          .SetLoops(loop, loopType)
                          .SetEase(ease)
                          .OnStart(() => { OnStart.Invoke(); })
                          .OnComplete(() => { OnComplete.Invoke(); });
        }

        public override void OnStop()
        {
            base.OnStop();

            target.DOKill();
        }
    }
}


