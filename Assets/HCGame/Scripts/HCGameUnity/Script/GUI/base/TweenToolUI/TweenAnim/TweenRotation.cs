using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Tweens.Plugins
{
    public class TweenRotation : TweenToolUI
    {
        public T_TypePosPath typePath;
        public RotateMode rotateMode = RotateMode.Fast;

        [Space(5)]
        public Vector3 rotate;

        [Space(10)]
        public UnityEvent OnStart;
        public UnityEvent OnComplete;

        private Vector3 next;
        private Vector3 previous;

        private bool isAwake = true;

        public override void OnBegin()
        {
            base.OnBegin();

            next = rotate;

            if (isAwake)
            {
                switch (typePath)
                {
                    case T_TypePosPath.DoPositionPath:
                        previous = target.rotation.eulerAngles;
                        break;
                    case T_TypePosPath.DoLocalPositionPath:
                        previous = target.localRotation.eulerAngles;
                        break;
                }

                isAwake = false;
            }
        }

        public override Tween OnPlay()
        {
            base.OnPlay();

            OnBegin();

            switch (typePath)
            {
                case T_TypePosPath.DoPositionPath:
                    tween = target.DORotate(next, duration, rotateMode)
                                  .SetDelay(delay)
                                  .SetLoops(loop, loopType)
                                  .SetEase(ease)
                                  .OnStart(() => { OnStart.Invoke(); })
                                  .OnComplete(() => { OnComplete.Invoke(); });
                    break;
                case T_TypePosPath.DoLocalPositionPath:
                    tween = target.DOLocalRotate(next, duration, rotateMode)
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
                    target.DORotate(previous, duration, rotateMode)
                                  .SetDelay(delay)
                                  .SetLoops(loop, loopType)
                                  .SetEase(ease)
                                  .OnStart(() => { OnStart.Invoke(); })
                                  .OnComplete(() => { OnComplete.Invoke(); });
                    break;
                case T_TypePosPath.DoLocalPositionPath:
                     target.DOLocalRotate(previous, duration, rotateMode)
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

        private void Update()
        {

        }
    }
}


