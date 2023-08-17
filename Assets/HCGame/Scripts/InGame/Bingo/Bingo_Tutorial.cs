using System.Collections;
using System.Collections.Generic;
using Bingo;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Bingo_Tutorial : SingletonMono<Bingo_Tutorial>
{
  [Header("content")]
  public GameObject contents;
  public RectTransform hand;
  [Header("PopupEndTheTutorial")]
  public GameObject popupEndTheTutorial;
  [Header("Text")]
  public GameObject txtGoodJob;
  public GameObject txtTapItDauBit;
  public GameObject txtLockForTheCalled;
  public GameObject txtTaptoBinGo;
    [Header("Button")]
  public Button btn;
  public Button btnBingo;
  public Button btnTryAgain;
  public Button btnPlayNow;
  [SerializeField] private List<Button> buttons;
  [Header("TargetBar")]
  public GameObject bgOverlay;
  public GameObject contentsTarget;
  public GameObject itemTarget;
 


  
  private void Awake()
  {
    btn.onClick.AddListener(StepBingo2);
    btnBingo.onClick.AddListener(StepBingo3);
    btnTryAgain.onClick.AddListener(TryAgain);
    btnPlayNow.onClick.AddListener(PlayNow);
//  StartTutorial();
  }

  public void StartTutorial()
  {
    contents.SetActive(true);
    txtLockForTheCalled.gameObject.SetActive(true);
    hand.gameObject.SetActive(true);
    txtGoodJob.gameObject.SetActive(false);
    Bingo_GameBoard.instance.contents.SetActive(false);
    Bingo_GameTargetSpawner.instance.contents.SetActive(false);
    ChangeObjectAlpha(btn.gameObject.transform.GetChild(2).gameObject, 0);
    StepBingo1();
    // itemTarget.transform.SetParent(contents.transform);
    // SwapObjectPositions(itemTarget,contentsTarget);



  }

  private void StepBingo1()
  {
    contents.SetActive(true);
    txtLockForTheCalled.gameObject.SetActive(true);
    txtTapItDauBit.gameObject.SetActive(true);
    hand.gameObject.SetActive(true);
    txtGoodJob.gameObject.SetActive(false);
  }

  private void StepBingo2()
  {
    contents.SetActive(true);
    txtLockForTheCalled.gameObject.SetActive(false);
    txtTapItDauBit.gameObject.SetActive(false);
    hand.gameObject.SetActive(true);
    txtGoodJob.gameObject.SetActive(false);
    hand.DOLocalMoveX(510, 1);
    hand.DOLocalMoveY(-852, 1);
    SwapObjectPositions(btn.gameObject,btnBingo.gameObject);
    txtTaptoBinGo.gameObject.SetActive(true);
    itemTarget.transform.SetParent(contentsTarget.transform);
    SetupBtnBingo();
    ChangeObjectAlpha(btn.gameObject.transform.GetChild(2).gameObject, 255);
  }

  private void StepBingo3()
  {
    contents.SetActive(true);
    txtGoodJob.gameObject.SetActive(true);
    txtTaptoBinGo.gameObject.SetActive(false);
    foreach (var button in buttons)
    {
      button.transform.GetChild(4).gameObject.GetComponent<TMP_Text>().text = "";
      button.transform.GetChild(3).gameObject.SetActive(true);
    }

    StartCoroutine(ShowEndTutorial());
  }

  IEnumerator ShowEndTutorial()
  {
    yield  return new  WaitForSeconds(1f);
    txtGoodJob.gameObject.SetActive(false);
    popupEndTheTutorial.gameObject.SetActive(true);
  }

  private void SetupBtnBingo()
  {
    btnBingo.gameObject.transform.GetChild(0).gameObject.SetActive(false);
    btnBingo.gameObject.transform.GetChild(1).gameObject.SetActive(true);

  }
  private void SwapObjectPositions(GameObject obj1, GameObject obj2)
  {
    int obj1Index = obj1.transform.GetSiblingIndex();
    int obj2Index = obj2.transform.GetSiblingIndex();

    obj1.transform.SetSiblingIndex(obj2Index);
    obj2.transform.SetSiblingIndex(obj1Index);
  }
  private void ChangeObjectAlpha(GameObject obj, float alpha)
  {
    Image image = obj.GetComponent<Image>();
    Color color = image.material.color;
    color.a = alpha;
    image.color = color;
  }

  private void TryAgain()
  {
    Bingo_NetworkManager.instance.StartPractice(HCAppController.Instance.GetBingoWs(), HCAppController.Instance.userInfo.UserCodeId);
  }

  private void PlayNow()
  {
    ScreenManagerHC.Instance.GoToScreenViewWithFull(() => ScreenManagerHC.Instance.ShowGameModeUI(GameType.Bingo));
    ScreenManagerHC.Instance.groupBottomHomeController.gameObject.SetActive(true);
  }
}
