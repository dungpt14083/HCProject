using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Tweens.Plugins
{
    public class TweenTransparent : TweenToolUI
    {
        [Space(5)]
        public Color color = Color.white;

        [Space(10)]
        public UnityEvent OnStart;
        public UnityEvent OnComplete;

        private Color next;
        private Color previous;

        private bool isAwake = true;

        public override void OnBegin()
        {
            base.OnBegin();

            next = color;

            if (isAwake)
            {
                if (target.GetComponent<Image>() == null)
                {
                    Debug.LogError("Object not found Image !!!");
                    return;
                }

                previous = target.GetComponent<Image>().color;
                isAwake = false;
            }
        }

        public void OnPlayTransparent()
        {
            OnPlay();
        }

        public override Tween OnPlay()
        {
            base.OnPlay();

            if (target.GetComponent<Image>() == null)
            {
                Debug.LogError("Object not found Image !!!");
                return tween;
            }

            tween = target.GetComponent<Image>().DOColor(next, duration)
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

            if (target.GetComponent<Image>() == null)
            {
                Debug.LogError("Object not found Image !!!");
                return;
            }

            target.GetComponent<Image>().DOColor(previous, duration)
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
