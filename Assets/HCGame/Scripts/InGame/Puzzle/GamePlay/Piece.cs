using DG.Tweening;
using TMPro;
using UnityEngine;
using MiniGame.MatchThree.Scripts.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using MiniGame.MatchThree.Scripts.Network;
namespace MiniGame.MatchThree.Scripts.GamePlay
{
    public class Piece : MonoBehaviour
    {
        [SerializeField] public string name;
        [SerializeField] public int xIndex;
        [SerializeField] public int yIndex;
        [SerializeField] public TextMeshPro txtNumber;

        private SingleBoard m_board;
        private bool isMove = false;

        public void AddNumber(int value)
        {
            Debug.Log("AddNumnber " + value);
            txtNumber.text = value.ToString();
        }

        public void OnInit(SingleBoard board, string name)
        {
            m_board = board;
            this.name = name;
            transform.name = name;
            baseScale = transform.localScale.x;

        }

        public void SetCoord(int x, int y)
        {
            xIndex = x;
            yIndex = y;
        }

        public void Move(int destX, int destY, float timeToMove)
        {
            if (isMove)
                return;
            transform.DOMove(new Vector3(destX, destY, 0), timeToMove)
                     .SetEase(Ease.OutExpo)
                     .OnStart(() => OnStart())
                     .OnComplete(() => OnComplete(destX, destY));
        }

        public float slowMoveTime;
        public float slowMoveCollapseTime;
        public void SlowMove(int destX, int destY)
        {
            if (isMove)
                return;
            transform.DOMove(new Vector3(destX, destY, 0), slowMoveTime)
                     .OnStart(() => OnStart())
                     .OnComplete(() => OnComplete(destX, destY));
        }


        public void SlowMoveColapse(int destX, int destY, float timeToMove)
        {
            if (isMove)
                return;
            transform.DOMove(new Vector3(destX, destY, 0), timeToMove * slowMoveCollapseTime)
                     .OnStart(() => OnStart())
                     .OnComplete(() => OnComplete(destX, destY));
        }






        private void OnStart()
        {
            isMove = true;
        }

        private void OnComplete(int destX, int destY)
        {
            isMove = false;
            m_board.PlacePiece(this, destX, destY);
            //MatchThreeGameSystem.Instance.veryfiAll();
            //MatchThreeNetworkManager.Instance.SendRequestClearMassive();
        }

        public bool IsSpecialPiece()
        {
            if (txtNumber.text == "41" ||
                txtNumber.text == "42" ||
                txtNumber.text == "44" ||
                txtNumber.text == "51" ||
                txtNumber.text == "55")
                return true;
            return false;
        }


        public float offsetScale, baseScale;
        public float offsetScaleTime;
        public bool showHintFlag;
        [Button]
        public void ScaleHint()
        {
            showHintFlag = true;
            StartCoroutine(scale());

            IEnumerator scale()
            {
                while (true)
                {
                    transform.DOScale(transform.localScale * offsetScale, offsetScaleTime);
                    yield return new WaitForSeconds(offsetScaleTime);
                    transform.DOScale(baseScale, offsetScaleTime);
                    yield return new WaitForSeconds(offsetScaleTime);
                }

            }
        }

        public Vector3 posToMove;
        public float timeMove;
        public Vector3 basePos;
        [Button]
        public void MoveHint(int dir)
        {
            if (dir == 5) posToMove = new Vector3(0, .5f, 0);
            if (dir == 6) posToMove = new Vector3(0, -.5f, 0);
            if (dir == 7) posToMove = new Vector3(-.5f, 0, 0);
            if (dir == 8) posToMove = new Vector3(.5f, 0, 0);

            //        SWAP_UP = 5,
            //SWAP_DOWN = 6,
            //SWAP_LEFT = 7,
            //SWAP_RIGHT = 8,
            basePos = transform.position;
            StartCoroutine(Move());

            IEnumerator Move()
            {
                while (true)
                {
                    transform.DOMove(transform.position + posToMove, timeMove);
                    yield return new WaitForSeconds(timeMove);
                    transform.DOMove(basePos, timeMove);
                    yield return new WaitForSeconds(timeMove);


                    transform.DOMove(transform.position + posToMove, timeMove);
                    yield return new WaitForSeconds(timeMove);
                    transform.DOMove(basePos, timeMove);
                    yield return new WaitForSeconds(timeMove);



                    yield return new WaitForSeconds(2);
                }

            }
        }

        public void ResetHint()
        {
            if (showHintFlag == false) return;
            StopAllCoroutines();
            transform.localScale = new Vector3(baseScale, baseScale);
            transform.position = basePos;
        }
    }
}