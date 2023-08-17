using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;

namespace Bingo
{


    public class BingoGame_AnimationEffect : MonoBehaviour
    {
        public static BingoGame_AnimationEffect instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.LogError("MULTIINSTANCE" + name);
                Destroy(this);
            }
        }




        public GameObject startGameGroup;

        [Button]
        public void PlayStartGameEff(Action callback)
        {
            Debug.Log("PlayStartGameEff 11111111111111");
            StartCoroutine(PlayStartGameEff_IE());
            IEnumerator PlayStartGameEff_IE()
            {
                //myOpacityBG.gameObject.SetActive(true);
                myContent.gameObject.SetActive(true);
                //endGameGroup.gameObject.SetActive(false);
                startGameGroup.gameObject.SetActive(true);
                myImage.color = Color.clear;

                //opacity fade in
                //myOpacityBG.color = Color.clear;
                //myOpacityBG.DOFade(.5f, 0.5f);


                yield return new WaitForSeconds(1f);
                PlaySound();
                yield return new WaitForSeconds(.3f);
                DisplaySprite(sprite_1);
                yield return new WaitForSeconds(1f);
                PlaySound();
                yield return new WaitForSeconds(.3f);
                DisplaySprite(sprite_2);
                yield return new WaitForSeconds(1f);
                PlaySound();
                yield return new WaitForSeconds(.3f);
                DisplaySprite(sprite_3);
                yield return new WaitForSeconds(1f);
                PlaySound();
                yield return new WaitForSeconds(.3f);
                DisplaySprite(sprite_Go);

                yield return new WaitForSeconds(.6f);
               
                //myOpacityBG.DOFade(.5f, 0.5f).OnComplete(() =>
                //{
                //    myContent.gameObject.SetActive(false);
                //});
                myContent.gameObject.SetActive(false);
                count = 0;
                callback?.Invoke();
            }


        }

        public void PlaySound()
        {
            if (count != 3)
            {
                Bingo_SoundManager.instance.PlaySound_Ting();
            }
            else
            {
                Bingo_SoundManager.instance.PlaySound_Go();
            }
            count++;
        }

        int count = 0;
        public float time1, time2, time3;
        public void DisplaySprite(Sprite sprite)
        {
            myImage.sprite = sprite;
            myImage.SetNativeSize();
            myImage.color = Color.white;
            myImage.transform.localScale = Vector3.zero;
            myImage.transform.DOScale(Vector3.one, time1).OnComplete(() =>
            {
                myImage.transform.DOScale(Vector3.one, time2).OnComplete(() =>
                {
                    myImage.DOFade(0, time3);

                });// wait
            });





        }

        [TitleGroup("___________  Reference Obj  __________")]

        public GameObject myContent;
        public Image myImage, myOpacityBG;
        public Sprite sprite_1, sprite_2, sprite_3, sprite_Go, opacityBG;
        public GameObject bubbleEffGr, scoreBoard, endGameGroup;
        public GameObject posScoreBoardStart, posScoreBoardEnd, scoreGroup;
        public List<GameObject> scoreAppearList;




        public bool _isDisplayEndGame;
        [Button]
        public void PlayEndGameEff()
        {
            if (_isDisplayEndGame) return;
            _isDisplayEndGame = true;


            myContent.SetActive(true);
            //myOpacityBG.gameObject.SetActive(false);

            //bingo board fade out
            Bingo_GameTargetSpawner.instance.contents.GetComponent<CanvasGroup>().DOFade(0, 1);
            Bingo_GameBoard.instance.contents.GetComponent<CanvasGroup>().DOFade(0, 1).OnComplete(() =>
            {
                //particle appears
                Bingo_GameBoard.instance.contents.SetActive(false);
                bubbleEffGr.SetActive(true);

                Bingo_GameBoard.instance.contents.GetComponent<CanvasGroup>().DOFade(0, 3).OnComplete(() =>
                { //wait 2 seconds
                    //endGameGroup.SetActive(true);
                    //myOpacityBG.gameObject.SetActive(true);
                    //myOpacityBG.color = new Color(0, 0, 0, 0);
                    //myOpacityBG.DOFade(.7f, 1);


                    scoreAppearList.ForEach(gameObject =>
                    {
                        gameObject.GetComponent<CanvasGroup>().alpha = 0;
                        if (gameObject.name != "#_[Button]__ReplayBtn")
                        {
                            gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "0";
                        }

                    });

                    //scoreboard appear
                    //scoreBoard.transform.position = posScoreBoardStart.transform.position;
                    //scoreBoard.transform.DOMove(posScoreBoardEnd.transform.position, 1.6f).SetEase(Ease.InOutBack).OnComplete(() =>
                    //{

                    //    //scoreAppear List appears  total 5 item: doubs/ bingos/ multi bingos/ penaties/ finalscore

                    //    var index = 0;
                    //    var timePerItemAppear = 1f;
                    //    scoreAppearList.ForEach(gameObject =>
                    //    {
                    //        Debug.Log("Fade" + gameObject);
                    //        Bingo_GameManager.instance.DelayedCall(index * timePerItemAppear, () =>
                    //        {
                    //            gameObject.GetComponent<CanvasGroup>().DOFade(1, .7f);

                    //            if (gameObject.name != "#_[Button]__ReplayBtn")
                    //            {
                    //                var text = gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

                    //                var valueEnd = 0;

                    //                if (gameObject.name.Contains("#_[Group]__Score_Doubs")) valueEnd = Bingo_NetworkManager.instance.doubs;
                    //                if (gameObject.name.Contains("#_[Group]__Score_Bingos")) valueEnd = Bingo_NetworkManager.instance.bingos;
                    //                if (gameObject.name.Contains("#_[Group]__Score_Multi Bingos")) valueEnd = Bingo_NetworkManager.instance.multiBingos;
                    //                if (gameObject.name.Contains("#_[Group]__Score_Penalties")) valueEnd = Bingo_NetworkManager.instance.penaties;
                    //                if (gameObject.name.Contains("#_[Group]__Score_Final"))
                    //                {
                    //                    valueEnd = Bingo_NetworkManager.instance.penaties
                    //                    + Bingo_NetworkManager.instance.doubs
                    //                    + Bingo_NetworkManager.instance.bingos
                    //                    + Bingo_NetworkManager.instance.multiBingos;
                    //                }

                    //                Bingo_GameManager.instance.DelayedCall(.5f, () =>
                    //                {
                    //                    NumberIncreasesEff(text, valueEnd);
                    //                });
                    //            }
                    //        });
                    //        index++;
                    //    });






                    //});
                });
            });
        }

        [Button]
        public void NumberIncreasesEff(TextMeshProUGUI text, int valueEnd)
        {

            var currentValue = Int32.Parse(text.text);
            int valueIncreasePerTick = (int)(valueEnd * 0.01f);

            StartCoroutine(_NumberIncreasesEff());
            IEnumerator _NumberIncreasesEff()
            {
                while (currentValue < valueEnd)
                {
                    yield return new WaitForSeconds(Time.deltaTime);
                    currentValue += valueIncreasePerTick;

                    text.text = "" + currentValue;
                }

                text.text = "" + valueEnd;
            }
        }



        public void OnClickBackToHCApp()
        {
            Bingo_NetworkManager.instance.Disconnect();
            SceneManager.LoadSceneAsync("Home");
        }


    }
}