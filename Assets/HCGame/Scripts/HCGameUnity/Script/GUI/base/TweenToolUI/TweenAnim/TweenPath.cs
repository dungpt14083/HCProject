using DG.Tweening;
using DG.Tweening.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Tweens.Plugins
{
    public class TweenPath : TweenToolUI
    {
        public T_TypePosPath typePath;
        public PathType pathType = PathType.CatmullRom;
        public PathMode pathMode = PathMode.Ignore;

        [Space(5)]
        public Transform[] paths;

        [Space(10)]
        public UnityEvent OnStart;
        public UnityEvent OnComplete;

        private Vector3[] WayPathNext;
        private Vector3[] WayPathPrevious;

        public override void OnBegin()
        {
            base.OnBegin();

            WayPathNext = new Vector3[paths.Length];
            WayPathPrevious = new Vector3[paths.Length];

            for (int i = 0; i < paths.Length; i++)
            {
                switch (typePath)
                {
                    case T_TypePosPath.DoPositionPath:
                        WayPathNext[i] = paths[i].position;
                        WayPathPrevious[i] = paths[paths.Length - (i + 1)].position;
                        break;
                    case T_TypePosPath.DoLocalPositionPath:
                        WayPathNext[i] = paths[i].localPosition;
                        WayPathPrevious[i] = paths[paths.Length - (i + 1)].localPosition;
                        break;
                }
            }
        }

        public override Tween OnPlay()
        {
            base.OnPlay();

            switch (typePath)
            {
                case T_TypePosPath.DoPositionPath:
                    tween = target.DOPath(WayPathNext, duration, pathType, pathMode)
                          .SetDelay(delay)
                          .SetLoops(loop, loopType)
                          .SetEase(ease)
                          .OnStart(() => { OnStart.Invoke(); })
                          .OnComplete(() => { OnComplete.Invoke(); });
                    break;
                case T_TypePosPath.DoLocalPositionPath:
                    tween = target.DOLocalPath(WayPathNext, duration, pathType, pathMode)
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
                    target.DOPath(WayPathPrevious, duration, pathType, pathMode)
                          .SetDelay(delay)
                          .SetLoops(loop, loopType)
                          .SetEase(ease)
                          .OnStart(() => { OnStart.Invoke(); })
                          .OnComplete(() => { OnComplete.Invoke(); });
                    break;
                case T_TypePosPath.DoLocalPositionPath:
                    target.DOLocalPath(WayPathPrevious, duration, pathType, pathMode)
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


