using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Purchasing;

namespace Bingo
{
    public class Bingo_Bot_Booster : MonoBehaviour
    {


        #region singleton
        public static Bingo_Bot_Booster instance;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Debug.LogError("MULTIINSTANCE" + name);
                Destroy(this);
            }
        }

        #endregion



        public BingoGameData _data;
        public GameObject btnBooster1;
        public GameObject btnBooster2;



        [Button]
        public void OnHandleNewData(BingoGameData data)
        {
            if (data != null) _data = data;
            Color colorDefult = boostrEndable1.color;
            Color colorBootser = boostrEndable1.color;
            colorDefult.a = 0f;
            colorBootser.a = 255f;



            if (_data.boosterBalls[0].booster == "None" && _data.boosterBalls[1].booster == "None")
            {
                boosterText_1.text = "--";
                boosterText_2.text = "--";
                boostrEndable1.color = colorDefult;
                boostrEndable2.color = colorDefult;
                boostrEndable1.sprite = _spritebooter;
                boostrEndable2.sprite = _spritebooter;
                btnBooster1.SetActive(false);
                btnBooster2.SetActive(false);
            }
            

            if (_data.boosterBalls[0].booster != "None")
            {
                boosterText_1.text = _data.boosterBalls[0].booster;
                boosterText_2.text = "--";
                btnBooster1.SetActive(true);
                btnBooster2.SetActive(false);
                string booster = _data.boosterBalls[0].booster;
                switch (booster)
                {
                    case "Bonus time":
                        boostrEndable1.color = colorBootser;
                        boostrEndable1.sprite = _spriteBonusTime;
                        break;
                    case "Double score" :
                        boostrEndable1.color = colorBootser;
                        boostrEndable1.sprite = _spriteDoubleScore;
                        break;
                    case "Wild daub":
                        boostrEndable1.color = colorBootser;
                        boostrEndable1.sprite = _spritewildaub;
                        break;
                    case "Pick a ball":
                        boostrEndable1.color = colorBootser;
                        boostrEndable1.sprite = _spritepickAball;
                        break;
                }

            }
            if (_data.boosterBalls[1].booster != "None")
            {
                boosterText_1.text = _data.boosterBalls[0].booster;
                boosterText_2.text = _data.boosterBalls[1].booster;
                btnBooster1.SetActive(true);
                btnBooster2.SetActive(true);
                string booster = _data.boosterBalls[0].booster;
                switch (booster)
                {
                    case "Bonus time":
                        boostrEndable1.color = colorBootser;
                        boostrEndable1.sprite = _spriteBonusTime;
                        break;
                    case "Double score" :
                        boostrEndable1.color = colorBootser;
                        boostrEndable1.sprite = _spriteDoubleScore;
                        break;
                    case "Wild daub":
                        boostrEndable1.color = colorBootser;
                        boostrEndable1.sprite = _spritewildaub;
                        break;
                    case "Pick a ball":
                        boostrEndable1.color = colorBootser;
                        boostrEndable1.sprite = _spritepickAball;
                        break;
                }
                string booster2 = _data.boosterBalls[1].booster;
                switch (booster2)
                {
                    case "Bonus time":
                        boostrEndable2.color = colorBootser;
                        boostrEndable2.sprite = _spriteBonusTime;
                        break;
                    case "Double score" :
                        boostrEndable2.color = colorBootser;
                        boostrEndable2.sprite = _spriteDoubleScore;
                        break;
                    case "Wild daub":
                        boostrEndable2.color = colorBootser;
                        boostrEndable2.sprite = _spritewildaub;
                        break;
                    case "Pick a ball":
                        boostrEndable2.color = colorBootser;
                        boostrEndable2.sprite = _spritepickAball;
                        break;
                }
            }
            
            if (_data.boosterBalls[0].loading != 0)
            {
                btnBooster1.SetActive(true);
            }
            else
            {
                btnBooster1.SetActive(false);
            }
            if (_data.boosterBalls[1].loading != 0)
            {
                btnBooster2.SetActive(true);
            }
            else
            {
                btnBooster2.SetActive(false);
            }
            IncreaseProgressBar((float)_data.boosterBalls[0].loading, _progress1, _progress1_Shadow);
            IncreaseProgressBar((float)_data.boosterBalls[1].loading, _progress2, _progress2_Shadow);



        }

        public TextMeshProUGUI boosterText_1, boosterText_2;
        public float _currentProgress;
        public float _progressMax = 100;
        public Image _progress1, _progress1_Shadow, _progress2, _progress2_Shadow;
        public Image boostrEndable1, boostrEndable2;
        [SerializeField] private Sprite _spriteBonusTime, _spriteDoubleScore, _spritewildaub, _spritepickAball,_spritebooter;
        [Button]
        public void IncreaseProgressBar(float value, Image progress, Image shadow)
        {

            _currentProgress = value / _progressMax;


            var from = progress.fillAmount;
            var to = _currentProgress;
            DOTween.To(() => from, x => from = x, to, 1)
                .OnUpdate(() =>
                {
                    progress.fillAmount = from;
                    shadow.fillAmount = from;
                });
        }

        public GameObject settingGr, posSetting_In, posSetting_Out;
        public Ease _ease;
        public float timeMoveSettting;
        public bool isMoving;
        [Button]
        public void OnClickSetting()
        {
            if (isMoving) return;

            isMoving = true;
            settingGr.SetActive(true);
            settingGr.transform.position = posSetting_Out.transform.position;
            settingGr.transform.DOMove(posSetting_In.transform.position, timeMoveSettting).SetEase(_ease).OnComplete(() =>
            {
                isMoving = false;
            });
        }


        [Button]
        public void OnCloseSetting()
        {
            if (isMoving) return;

            isMoving = true;
            settingGr.transform.DOMove(posSetting_Out.transform.position, timeMoveSettting).SetEase(_ease).OnComplete(() =>
            {
                settingGr.SetActive(false);
                isMoving = false;
            });
        }

        public void OnClickQuit()
        {
            Bingo_Notice.instance.OnClickShowNotice("End the match with your current score ? ");
        }



        public void OnClickUseBooster(int idx)
        {

            if (_data.boosterBalls.Count == 0) return;
            Debug.LogError("CLICK  BOOSTER IDX " + idx + "__name: " + _data.boosterBalls[idx]);
            Color colorBootser = boostrEndable1.color;
            colorBootser.a = 0f;
            if (idx == 0)
            {
                boostrEndable1.color = colorBootser;
                boostrEndable1.sprite = _spritebooter;
                btnBooster1.SetActive(false);
               
            }
            else if (idx == 1)
            {
                boostrEndable2.color = colorBootser;
                boostrEndable2.sprite = _spritebooter;
                btnBooster2.SetActive(false);
            }

            if (_data.boosterBalls[idx].booster.Contains("Bonus time")) OnClickBonusTime(idx);
            if (_data.boosterBalls[idx].booster.Contains("Double score")) OnClickDoubleScore(idx);
            if (_data.boosterBalls[idx].booster.Contains("Wild daub")) OnClickWildDaub(idx);
            if (_data.boosterBalls[idx].booster.Contains("Pick a ball")) OnClickPickABall(idx);

        }
        public GameObject _wildDaub;
        public bool _isSelectingWildDaub;


        public TMP_Text Bonus_Time;
        public GameObject pos1,pos2;

        public void OnClickBonusTime(int index)
        {
            Bonus_Time.text = "+10";
            Bonus_Time.gameObject.SetActive(true);
            EffectBonusTime();
            Bingo_NetworkManager.instance.SendMessage_BonusTime(index);
        }

        public void EffectBonusTime()
        {
            Bonus_Time.transform.position = pos1.transform.position;
            Bonus_Time.transform.DOMove(pos2.transform.position, 1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                Bonus_Time.gameObject.SetActive(false);
            });
        }
       
        public void OnClickWildDaub(int index)
        {
            Bingo_NetworkManager.instance.SendMessage_PauseToPick_WildDaub(index);
            SetSelecting_WildDaub(true);
        }
        [Button]
        public void SetSelecting_WildDaub(bool value)
        {
            _isSelectingWildDaub = value;
            if(value)
            {
                _wildDaub.GetComponent<Bingo_AutoClosePanelBosster>().ShowPopup(()=>SetSelecting_WildDaub(false));
            }
            else
            {
                _wildDaub.GetComponent<Bingo_AutoClosePanelBosster>().HidePopup();
            }
            //_wildDaub.gameObject.SetActive(value);
            Bingo_GameTargetSpawner.instance.contents.gameObject.SetActive(!value);
        }

        public GameObject _double;
        public bool _isSelectingDoubleScore;
        public void OnClickDoubleScore(int index)
        {
            Bingo_NetworkManager.instance.SendMessage_DoubleScore(index);
            SetselectingDoubleScore(true);
        }
        [Button]
        public void SetselectingDoubleScore(bool value)
        {
            _isSelectingDoubleScore = value;
            if(value)
            {
                _double.GetComponent<Bingo_AutoClosePanelBosster>().ShowPopup(()=>SetselectingDoubleScore(false));
            }
            else
            {
                _double.GetComponent<Bingo_AutoClosePanelBosster>().HidePopup();
            }
            //_wildDaub.gameObject.SetActive(value);
         //   Bingo_GameTargetSpawner.instance.contents.gameObject.SetActive(!value);
        }


        public GameObject _pickABallUI, _pickBallList;
        public List<int> _numLeft, _valueToSpawn;
        public bool _isPickABall;
        public Bingo_GameBoardSingleItemPickABall BoardSingleItemPickABall;
        public List<Bingo_GameBoardSingleItemPickABall> ListPickaBall = new List<Bingo_GameBoardSingleItemPickABall>();
        public Sprite SpriteB, SpriteI, SpriteN, SpriteO;
        [Button]
        public void OnClickPickABall(int index)
        {
            _numLeft.Clear();
            _valueToSpawn.Clear();
            deleteItemGrid();
            Bingo_NetworkManager.instance.SendMessage_PauseToPick_PickABall(index);

            SetSelecting_PickABall(true);

            int i = 0;
            Bingo_GameBoard.instance.boardList.ForEach(board =>
            {
                if (board._boardItemType == BoardItemTypeEn.toClick)
                {
                    _numLeft.Add(board.myValue);
                    i++;
                }
                    
            });
            if (_numLeft.Count == 1)
            {
                var rd = Random.Range(0, _numLeft.Count);
                _valueToSpawn.Add(_numLeft[rd]);
                _numLeft.RemoveAt(rd);
            }
            else if (_numLeft.Count == 2)
            {

                var rd = Random.Range(0, _numLeft.Count);
                _valueToSpawn.Add(_numLeft[rd]);
                _numLeft.RemoveAt(rd);

                rd = Random.Range(0, _numLeft.Count);
                _valueToSpawn.Add(_numLeft[rd]);
                _numLeft.RemoveAt(rd);


            }


            else if (_numLeft.Count == 3)
            {

                var rd = Random.Range(0, _numLeft.Count);
                _valueToSpawn.Add(_numLeft[rd]);
                _numLeft.RemoveAt(rd);

                rd = Random.Range(0, _numLeft.Count);
                _valueToSpawn.Add(_numLeft[rd]);
                _numLeft.RemoveAt(rd);


                rd = Random.Range(0, _numLeft.Count);
                _valueToSpawn.Add(_numLeft[rd]);
                _numLeft.RemoveAt(rd);

            }
            else if(_numLeft.Count>3)
            {
                var rd = Random.Range(0, _numLeft.Count);
                _valueToSpawn.Add(_numLeft[rd]);
                _numLeft.RemoveAt(rd);

                rd = Random.Range(0, _numLeft.Count);
                _valueToSpawn.Add(_numLeft[rd]);
                _numLeft.RemoveAt(rd);


                rd = Random.Range(0, _numLeft.Count);
                _valueToSpawn.Add(_numLeft[rd]);
                _numLeft.RemoveAt(rd);

                rd = Random.Range(0, _numLeft.Count);
                _valueToSpawn.Add(_numLeft[rd]);
                _numLeft.RemoveAt(rd);

            }

            int j = 0;
            _valueToSpawn.ForEach(num =>
            {
                Bingo_GameBoardSingleItemPickABall pickABallObj = Instantiate(BoardSingleItemPickABall, _pickBallList.transform);
                pickABallObj.GetComponent<Bingo_GameBoardSingleItemPickABall>().SetValue(num,0,0);
                pickABallObj.gameObject.SetActive(true);
                ListPickaBall.Add(pickABallObj);
                switch (j)
                {
                    case 0:
                        pickABallObj.rightBG = SpriteB;
                        pickABallObj.bingoBG = SpriteB;
                        pickABallObj.transform.GetChild(1).GetComponent<Image>().sprite = SpriteB;
                        break;
                    case 1:
                        pickABallObj.rightBG = SpriteI;
                        pickABallObj.bingoBG = SpriteI;
                        pickABallObj.transform.GetChild(1).GetComponent<Image>().sprite = SpriteI;
                        break;
                    case 2:
                        pickABallObj.rightBG = SpriteN;
                        pickABallObj.bingoBG = SpriteN;
                        pickABallObj.transform.GetChild(1).GetComponent<Image>().sprite = SpriteN;
                        break;
                    case 3:
                        pickABallObj.rightBG = SpriteO;
                        pickABallObj.bingoBG = SpriteO;
                        pickABallObj.transform.GetChild(1).GetComponent<Image>().sprite = SpriteO;
                        break;
                    default:
                        pickABallObj.rightBG = SpriteB;
                        pickABallObj.bingoBG = SpriteB;
                        pickABallObj.transform.GetChild(1).GetComponent<Image>().sprite = SpriteB;
                        break;
                }
                j++;
            });
            


        }
        void deleteItemGrid()
        {
            foreach (Bingo_GameBoardSingleItemPickABall item in ListPickaBall)
            {
                Destroy(item.gameObject);
            }

          
            ListPickaBall.Clear();
        }
        public void SetSelecting_PickABall(bool value)
        {
            _isPickABall = value;
            //_pickABallUI.SetActive(value);
            if (value)
            {
                _pickABallUI.GetComponent<Bingo_AutoClosePanelBosster>().ShowPopup(()=>SetSelecting_PickABall(false));
            }
            else
            {
                _pickABallUI.GetComponent<Bingo_AutoClosePanelBosster>().HidePopup();
            }
        //   Bingo_GameTargetSpawner.instance.contents.gameObject.SetActive(!value); 
           //Bingo_PickABallTaget.instance.contents.gameObject.SetActive(value);
        }

        [SerializeField] GameObject _btnBingoActive, _btnBingoDeactive;
        [SerializeField] Button _btnBingo;
        
        public void ShowButonBingo(bool isShow)
        {
            _btnBingoActive.SetActive(isShow);
            _btnBingoDeactive.SetActive(!isShow);
            _btnBingo.interactable = isShow ? true : false;
        }

    }
    public enum Booster
    {
        none,
        WildDaub,
        PickABall,
        DoubleScore,
        BonusTime

    }
}