using AssemblyCSharp.GameNetwork;
using MiniGame.MatchThree.Scripts.Network;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchingControler : MonoBehaviour
{
    public Image iconMiniGame;
    public Image avatarMyPlayer;
    public Image avatarOtherPlayer;
    public TextMeshProUGUI txtMyNamePlayer;
    public TextMeshProUGUI txtOtherNamePlayer;
    public TextMeshProUGUI txtGameMachine;
    public Button btCancelMatching;
    [SerializeField] private GameObject findingObject;
    [SerializeField] private GameObject opponentObject;
    [SerializeField] private SlotMachine SlotMachine;
    
    void Start()
    {
        btCancelMatching.onClick.AddListener(CancelMatching);
        GameType gameType = (GameType)HCAppController.Instance.currentGameType;
        
        iconMiniGame.sprite = ResourceManager.Instance.GetIconGame(gameType);
        txtGameMachine.text = gameType.ToString();
        LoadMyData(HCAppController.Instance.userInfo.UserName, HCAppController.Instance.myAvatar);
        HCAppController.Instance.actionMatching = LoadData;
        HCAppController.Instance.actionMatching += EightBallNetworkManager.Instance.SetMatchInformation;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnDisable()
    {
        HCAppController.Instance.actionMatching = null;
        HCAppController.Instance.currentGameType = GameType.None;
    }
    private void OnDestroy()
    {
        HCAppController.Instance.actionMatching -= EightBallNetworkManager.Instance.SetMatchInformation;
        HCAppController.Instance.actionMatching = null;
        HCAppController.Instance.currentGameType = GameType.None;
    }
    
    public void LoadData(MatchInformation matchInfo)
    {
        StartCoroutine(waitAndLoadSlotMachine());
        findingObject.SetActive(false);
        opponentObject.SetActive(true);
        //LoadMyData(matchInfo.FirstUserName, matchInfo.AvatarFirstUser);
        LoadOtherData(matchInfo.SecondUserName, matchInfo.AvatarSecondUser);
    }
    public void LoadMyData(string name, Sprite sprAvatar)
    {
        txtMyNamePlayer.text = name;
        avatarMyPlayer.sprite = sprAvatar;
    }
    public void LoadMyData(string name, string urlAvatar)
    {
        txtMyNamePlayer.text = name;
        StartCoroutine(HCHelper.LoadAvatar(urlAvatar, avatarMyPlayer));
    }
    public void LoadOtherData(string name, string urlAvatar)
    {
        txtOtherNamePlayer.text = name;
        StartCoroutine(HCHelper.LoadAvatar(urlAvatar, avatarOtherPlayer));
    }

    IEnumerator waitAndLoadSlotMachine()
    {
        SlotMachine.duration = 2f;
        yield return new WaitForSeconds(2f);
        SlotMachine.isScrollListRunning = false;
        yield return new WaitForSeconds(0.5f);
        SlotMachine.gameObject.SetActive(false);
    }
    public void CancelMatching()
    {
        Debug.Log("CancelMatching HCAppController.Instance.currentGameType " + HCAppController.Instance.currentGameType);
        var gameType = HCAppController.Instance.currentGameType;
        switch (gameType)
        {
            case GameType.Billard:
                EightBallGameSystem.Instance.CancelMatching();
                break;
            case GameType.Solitaire:
                Solitaire.SGameManager.Instance.CancelMatching();
                break;
            case GameType.Bingo:
                Bingo.Bingo_NetworkManager.instance.CancelMatching();
                break;
            case GameType.Puzzle:
                MatchThreeNetworkManager.Instance.CancelMatching();
                break;
            case GameType.Bubble:
                BubbesShotManager.Instance.CancelMatching();
                break;
        }
    }
}
