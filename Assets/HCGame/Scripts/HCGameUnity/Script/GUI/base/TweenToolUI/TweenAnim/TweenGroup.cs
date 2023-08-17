using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace Tweens.Plugins
{
    [System.Serializable]
    public class ListTweenItem
    {
        public TweenToolUI itemObj;
        public T_TypeSequences itemType;
    }

    [System.Serializable]
    public class GroupTweenItem
    {
        public ListTweenItem[] groupObj;
        [Space(10)]
        public T_TypeSequences groupType;
    }

    public class TweenGroup : MonoBehaviour
    {
        public bool isStart;
        public bool isEnable;
        public float delay;
        public int loop;
        public LoopType loopType = LoopType.Yoyo;

        [Space(5)]
        public GroupTweenItem[] listItem;

        [Space(10)]
        public UnityEvent OnStart;
        public UnityEvent OnComplete;

        public Sequence mySequence;
        Sequence sequence;
        private bool isAnimation = false;

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

        public void OnPlay()
        {
            Debug.Log("Tween Play");

            if (isAnimation)
            {
                mySequence.Restart();
                return;
            }

            isAnimation = true;

            InitSequence();

            foreach (var group in listItem)
            {
                sequence = DOTween.Sequence();

                foreach (var item in group.groupObj)
                {
                    switch (item.itemType)
                    {
                        case T_TypeSequences.Append:
                            sequence.Append(item.itemObj.OnPlay());
                            break;
                        case T_TypeSequences.Join:
                            sequence.Join(item.itemObj.OnPlay());
                            break;
                    }
                }

                switch (group.groupType)
                {
                    case T_TypeSequences.Append:
                        mySequence.Append(sequence);
                        break;
                    case T_TypeSequences.Join:
                        mySequence.Join(sequence);
                        break;
                }
            }

            mySequence.Play();
        }

        public void OnStop()
        {
            Debug.Log("Tween Stop");

            mySequence.Kill();
        }

        public void OnBack()
        {
            Debug.Log("Tween Back");

            foreach (var group in listItem)
            {
                foreach (var item in group.groupObj)
                {
                    item.itemObj.OnBack();
                }
            }
        }

        private void InitSequence()
        {
            mySequence = DOTween.Sequence();
            mySequence.OnStart(() => { OnStart.Invoke(); });
            mySequence.OnComplete(() => { OnComplete.Invoke(); isAnimation = false; });
            mySequence.SetLoops(loop, loopType);
            mySequence.SetDelay(delay);
            mySequence.Pause();
        }

    }

}

