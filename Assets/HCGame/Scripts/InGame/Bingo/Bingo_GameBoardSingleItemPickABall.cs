using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
namespace Bingo
{
    [Serializable]

    public class Bingo_GameBoardSingleItemPickABall : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

            SetupSelf();
        }


        public void SetupSelf()
        {
            gameObject.GetComponent<Button>().onClick.AddListener(OnClick);
            mySlotType = 0;
            GetComponent<Button>().targetGraphic = transform.GetChild(2).GetComponent<Image>();
            _boardItemType = BoardItemTypeEn.toClick;

        }

        [TitleGroup("___________  Show only __________")]
        public string baseName;
        public string myKey;
        public int myValue;
        public int mySlotType;
        public int myIndex;



        [TitleGroup("___________  Reference  Obj __________")]
        public TextMeshProUGUI myText;
        public Sprite rightCircle, bingoStar, rightBG, bingoBG;
        //0 là màu trắng, chạm vào để gọi số
        //1 là màu xanh, đã được chạm vào và đúng
        //2 là kim cương
        //3 là ngôi sao(bingo)

        [TitleGroup("___________  Reference Var  __________")]
        public float _timeEffCircle;
        public Vector3 _circleSize;

        public Material outlineGreen;
        [Button]
        public void SetValue(int value, int slotType, int index )
        {
            if (mySlotType != slotType)
            {
                if (slotType == 1)  // RIGHT green circle eff
                {
                    //transform.GetChild(3).gameObject.SetActive(true); // circle color change
                    transform.GetChild(3).transform.localScale = Vector3.zero;
                    transform.GetChild(3).transform.DOScale(_circleSize, _timeEffCircle).SetEase(Ease.OutBack);
                    transform.GetChild(3).GetComponent<Image>().sprite = rightCircle;



                    transform.GetChild(2).gameObject.SetActive(true);//new color bg
                    transform.GetChild(2).GetComponent<Image>().color = new Color(1, 1, 1, 0);
                    transform.GetChild(2).GetComponent<Image>().DOFade(1, _timeEffCircle);
                    transform.GetChild(2).GetComponent<Image>().sprite = rightBG;

                    transform.GetChild(4).gameObject.SetActive(true);//  number
                    transform.GetChild(4).GetComponent<TextMeshProUGUI>().color = Color.white;
                    transform.GetChild(4).GetComponent<TextMeshProUGUI>().fontSharedMaterial = outlineGreen;
                    _boardItemType = BoardItemTypeEn.right;

                }

                if (slotType == 2)// diamond eff // remove that case
                {
                    return;
                    //var blueCircleEff = transform.GetChild(0).gameObject;
                    //blueCircleEff.SetActive(true);
                    //blueCircleEff.transform.localScale = Vector3.zero;
                    //blueCircleEff.transform.DOScale(Vector3.one, .7f).SetEase(Ease.OutBack);


                    //transform.GetChild(1).gameObject.SetActive(false);// number
                    //transform.GetChild(2).gameObject.SetActive(true);//diamond and bingoStar
                    //transform.GetChild(2).transform.localEulerAngles = new Vector3(0, 0, 90);
                    //transform.GetChild(2).GetComponent<Image>().sprite = diamond;
                    //transform.GetChild(2).GetComponent<Image>().color = Color.white;
                    //transform.GetChild(2).transform.localScale = Vector3.zero;
                    //transform.GetChild(2).transform.DOScale(Vector3.one, .7f).SetEase(Ease.OutBack);
                }

                if (slotType == 3)// bingo star
                {

                    transform.GetChild(4).gameObject.SetActive(false);//  number
                    transform.GetChild(2).gameObject.SetActive(true);//   color background
                    transform.GetChild(3).gameObject.SetActive(true);//   circle color change



                    transform.GetChild(3).GetComponent<Image>().sprite = bingoStar;
                    transform.GetChild(3).transform.localScale = Vector3.zero;
                    transform.GetChild(3).transform.DOScale(_circleSize, _timeEffCircle).SetEase(Ease.OutBack);

                    transform.GetChild(2).GetComponent<Image>().color = new Color(1, 1, 1, 0);
                    transform.GetChild(2).GetComponent<Image>().DOFade(1, _timeEffCircle);
                    transform.GetChild(2).GetComponent<Image>().sprite = bingoBG;

                    _boardItemType = BoardItemTypeEn.bingo;
                }
            }
            
            name = myKey + "__Type: " + mySlotType + "__Value: " + value;
            myValue = value;
            myIndex= index;
            mySlotType = slotType;
            myText.text = "" + value;
            
        }
        public BoardItemTypeEn _boardItemType;
        public void OnClick()
        {
           if (Bingo_GameManager.instance.currentGameState != GameState.playing) return;


          //  Bingo_NetworkManager.instance.SendMessageString(messageSend + myValue + "}");
          Bingo_NetworkManager.instance.SendMessageClick(myValue , myIndex);
           Bingo_SoundManager.instance.PlaySound_TapButton();
           Bingo_GameTargetSpawner.instance.contents.gameObject.SetActive(false); 
           Bingo_PickABallTaget.instance.contents.gameObject.SetActive(true);
           Bingo_PickABallTaget.instance.SpawnPickABallTarget(myKey.ToString()+"_"+myValue.ToString(), 3);
           BingoGameData bingoGameData = new BingoGameData();
           bingoGameData.choseTarget = myKey.ToString() + "_" + myValue.ToString();
           bingoGameData.choseTargetSecondTime = 0;
           Bingo_GameTargetSpawner.instance.SpawnNewTarget(bingoGameData);
           Bingo_Bot_Booster.instance.SetSelecting_PickABall(false);
        }
    }
}
