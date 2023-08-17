using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Tweens.Plugins
{
    public class TweenMove : TweenToolUI
    {
        public T_TypePosPath typePath;

        [Space(5)]
        public Transform from;
        public Transform to;

        [Space(10)]
        public UnityEvent OnStart;
        public UnityEvent OnComplete;

        private Vector3 next;
        private Vector3 previous;


        public override void OnBegin()
        {
            base.OnBegin();

            if (from == null)
                from = target;

            if (to == null)
                to = target;

            switch (typePath)
            {
                case T_TypePosPath.DoPositionPath:
                    next = to.position;
                    previous = from.position;
                    break;
                case T_TypePosPath.DoLocalPositionPath:
                    next = to.localPosition;
                    previous = from.localPosition;
                    break;
            }
        }

        public void OnPlayMove()
        {
            OnPlay();
        }

        public override Tween OnPlay()
        {
            base.OnPlay();
            OnBegin();

            switch (typePath)
            {
                case T_TypePosPath.DoPositionPath:
                    tween = target.DOMove(next, duration)
                            .SetDelay(delay)
                            .SetLoops(loop, loopType)
                            .SetEase(ease)
                            .OnStart(() => { OnStart.Invoke(); })
                            .OnComplete(() => { OnComplete.Invoke(); });
                    break;
                case T_TypePosPath.DoLocalPositionPath:
                    tween = target.DOLocalMove(next, duration)
                          .SetDelay(delay)
                          .SetLoops(loop, loopType)
                          .SetEase(ease)
                          .OnStart(() => { OnStart.Invoke(); })
                          .OnComplete(() => { OnComplete.Invoke(); });

                    break;
            }

            return tween;
        }

        public override void OnBack()
        {
            base.OnBack();

            switch (typePath)
            {
                case T_TypePosPath.DoPositionPath:
                     target.DOMove(previous, duration)
                          .SetDelay(delay)
                          .SetLoops(loop, loopType)
                          .SetEase(ease)
                          .OnStart(() => { OnStart.Invoke(); })
                          .OnComplete(() => { OnComplete.Invoke(); });//
                    break;
                case T_TypePosPath.DoLocalPositionPath:
                    target.DOLocalMove(previous, duration)
                       .SetDelay(delay)
                       .SetLoops(loop, loopType)
                       .SetEase(ease)
                       .OnStart(() => { OnStart.Invoke(); })
                       .OnComplete(() => { OnComplete.Invoke(); });

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
