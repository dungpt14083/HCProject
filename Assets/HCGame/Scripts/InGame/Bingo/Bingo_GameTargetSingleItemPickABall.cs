using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

namespace Bingo
{
    public class Bingo_GameTargetSingleItemPickABall : MonoBehaviour
    {



        public TextMeshProUGUI myValueText;
        public Image myImage;
        public GameObject myCooldownBar;
        public GameObject myCooldownBar_value;
        public float offsetCooldown, scale1;

        public Color _B, _I, _N, _G, _O;
        public void SetupSelf(string data, int timeSpawnOther)
        {

            transform.localScale = Vector3.zero;
            transform.DOScale(scale1, 1).OnComplete(() =>
            {
                myCooldownBar.gameObject.SetActive(true);
                myCooldownBar_value.transform.DOScaleX(0, timeSpawnOther - offsetCooldown).SetEase(Ease.Linear).OnComplete(() =>
                {
                    myCooldownBar.gameObject.SetActive(false);

                });

                // enable cooldown

            });

            Bingo_GameTargetSpawner.instance.OnSpawnCallback += MoveLeft;

            var myValue = int.Parse(data.Split("_")[1]);
            myValueText.text = "" + myValue;


            var myType = data.Split("_")[0];
            if      (myType == "B") { myImage.sprite = b; myValueText.color = _B; }
            else if (myType == "I") { myImage.sprite = i; myValueText.color = _I; }
            else if (myType == "N") { myImage.sprite = n; myValueText.color = _N; }
            else if (myType == "G") { myImage.sprite = g; myValueText.color = _G; }
            else if (myType == "O") { myImage.sprite = o; myValueText.color = _O; };


        }

        int moveCount = 0;
        public Sprite b, i, n, g, o;
        public float scaleValue;
        public void MoveLeft()
        {
            if (moveCount >= 10)
            {
                Bingo_GameTargetSpawner.instance.OnSpawnCallback -= MoveLeft;
                Destroy(gameObject);
                return;
            }
            moveCount++;
            transform.DOMoveX(gameObject.transform.position.x - Bingo_GameTargetSpawner.instance.distanceTargetMove, 1);
            transform.DOScale(scaleValue, 1);

        }



















    }
}