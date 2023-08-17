using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Tweens.Plugins
{
    public class TweenScale : TweenToolUI
    {
        [Space(5)]
        public T_TypeScale typeScale = T_TypeScale.Normal;
        public int vibrate = 10;
        public Vector3 scaleTarget = Vector3.one;

        [Space(10)]
        public UnityEvent OnStart;
        public UnityEvent OnComplete;

        private Vector3 next;
        private Vector3 previous;

        private bool isAwake = true;

        public override void OnBegin()
        {
            base.OnBegin();

            next = scaleTarget;

            if (isAwake)
            {
                previous = target.localScale;
                isAwake = false;
            }
        }

        public void OnPlayScale()
        {
            OnPlay();
        }

        public override Tween OnPlay()
        {
            base.OnPlay();

            switch (typeScale)
            {
                case T_TypeScale.Normal:
                    tween = target.DOScale(next, duration)
                          .SetDelay(delay)
                          .SetLoops(loop, loopType)
                          .SetEase(ease)
                          .OnStart(() => { OnStart.Invoke(); })
                          .OnComplete(() => { OnComplete.Invoke(); });
                    break;
                case T_TypeScale.Punch:
                    tween = target.DOPunchScale(next, duration, vibrate)
                        .SetDelay(delay)
                        .SetLoops(loop, loopType)
                        .SetEase(ease)
                        .OnStart(() => { OnStart.Invoke(); })
                        .OnComplete(() => { OnComplete.Invoke(); transform.localScale = scaleTarget; });
                    break;
                case T_TypeScale.Shake:
                    tween = target.DOShakeScale(duration)
                       .SetDelay(delay)
                       .SetLoops(loop, loopType)
                       .SetEase(ease)
                       .OnStart(() => { OnStart.Invoke(); })
                       .OnComplete(() => { OnComplete.Invoke(); transform.localScale = scaleTarget; });
                    break;
            }

            return tween;
        }

        public override void OnBack()
        {
            base.OnBack();

            switch (typeScale)
            {
                case T_TypeScale.Normal:
                    tween = target.DOScale(previous, duration)
                          .SetDelay(delay)
                          .SetLoops(loop, loopType)
                          .SetEase(ease)
                          .OnStart(() => { OnStart.Invoke(); })
                          .OnComplete(() => { OnComplete.Invoke(); });
                    break;
                case T_TypeScale.Punch:
                    tween = target.DOPunchScale(previous, duration, vibrate, 0)
                        .SetDelay(delay)
                        .SetLoops(loop, loopType)
                        .SetEase(ease)
                        .OnStart(() => { OnStart.Invoke(); })
                        .OnComplete(() => { OnComplete.Invoke(); transform.localScale = scaleTarget; });
                    break;
                case T_TypeScale.Shake:
                    tween = target.DOShakeScale(duration, 1, vibrate)
                       .SetDelay(delay)
                       .SetLoops(loop, loopType)
                       .SetEase(ease)
                       .OnStart(() => { OnStart.Invoke(); })
                       .OnComplete(() => { OnComplete.Invoke(); transform.localScale = scaleTarget; });
                    break;
            }
        }

        public override void OnStop()
        {
            base.OnStop();

            target.DOKill();
        }
    }
}


