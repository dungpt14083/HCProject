using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Tweens.Plugins
{
    public class TweenToolUI : MonoBehaviour
    {
        public bool isStart;
        public bool isEnable;
        public Transform target;
        public float duration = 1;
        public float delay = 0;
        public int loop = 0;
        public LoopType loopType = LoopType.Yoyo;
        public Ease ease = Ease.Linear;

        public Tween tween;
        public Tweener tweener;

        private void Awake()
        {
            OnBegin();
        }

        private void OnValidate()
        {
            OnBegin();
        }

        private void Start()
        {
            if (isStart)
            {
                OnPlay();
            }
        }

        private void OnEnable()
        {
            if (isEnable)
            {
                OnPlay();
            }
        }

        public virtual void OnBegin()
        {
            if (target == null)
                target = transform;
        }

        public virtual Tween OnPlay()
        {
            //Debug.Log("Tween Play");
            tween.Rewind();
            return tween;
        }

        public virtual void OnStop()
        {
            //Debug.Log("Tween Stop");
        }

        public virtual void OnBack()
        {
            //Debug.Log("Tween Back");
        }
    }
}


