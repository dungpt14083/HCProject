using AssemblyCSharp.GameNetwork;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.X9;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HcGames;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class EightBallUIController : SingletonMono<EightBallUIController>
{
    public EightPopupController EightPopupCtrl;
    [SerializeField] private PlayerInfor[] playerInfors;
    private PlayerInfor mainInfor;
    private PlayerInfor otherInfor;
    [SerializeField] private ResultDialogHandler _resultDialogHandler;
    [SerializeField] private Sprite[] ballIconSprite;

    [SerializeField] private Sprite eightBallSprite;
    [SerializeField] private GameObject p2PGUI;

    [SerializeField] private GameObject sampleMessage;
    
    //Single UI
    [Header("Single")]
    [SerializeField] private PlayerInfor singlePlayerInfo;
    [SerializeField] private GameObject trickShotGUI;
    [SerializeField] private Transform[] remainShootCountImages;
    [SerializeField] private TMP_Text remainTimeText;
    [SerializeField] private TMP_Text remainPointText;
    [SerializeField] private Image iconRewardImg;
    [SerializeField] private TMP_Text valueRewardTxt;
    

    // [SerializeField] private 

    //private bool isMainFirst = false;

    private BallGroup newUpdateGroup = BallGroup.BgtNone;
    bool isFirstShot, _isShowMessGetGroup;



    void Start()
    {
        //Debug.LogError("==============CHAY 2 LAN HAM START=================");
        if(EightBallGameSystem.Instance.CurrentMode == PlayMode.Online)
        {
            if (playerInfors != null)
            {
                mainInfor = playerInfors[0];
                otherInfor = playerInfors[1];
            }
        }
        else
        {
            mainInfor = singlePlayerInfo;
            if (playerInfors != null)
            {
                otherInfor = playerInfors[1];
            }
        }
        
        mainInfor.ClearActiveImgSpot();
        otherInfor.ClearActiveImgSpot();
        isFirstShot = true;
        EightBallGameSystem.Instance.OnBallOnPotCallback += BallOnPot;
        EightBallGameSystem.Instance.OnEndGameCallback += ShowResult;
        EightBallGameSystem.Instance.OnGetAllBallsGroupCallback += UpdateBallGroup;
        EightBallGameSystem.Instance.OnEndBallCallback += OnEndBall;
        EightBallGameSystem.Instance.OnTimeUpdateCallback += updateTime;

        EightBallGameSystem.Instance.OnGetBallGroupCallback += ShowGetGroupMessage;

        //Single
        EightBallGameSystem.Instance.OnRemainShootUpdateCallback += UpdateRemainShoot;
        EightBallGameSystem.Instance.OnSinglePointUpdateCallback += UpdateSingePoint;
        EightBallGameSystem.Instance.OnSingleRemainTimeUpdate += UpdateRemainTimeout;
        EightBallGameSystem.Instance.OnSingleEndGameCallback += ShowSingleResult;
        EightBallGameSystem.Instance.OnSingleFailedPocketBallCallback += ShowSingleFailedPocketBall;

    }
    public void StartGame()
    {
        //ResetTime();
        //isMainFirst = EightBallGameSystem.Instance.IsMyTurn;

        //enable UI by PlayMode
        if (EightBallGameSystem.Instance.CurrentMode == PlayMode.Online)
        {
            //mainInfor.UpdatePlayerData(HCAppController.Instance.userInfo.UserName, HCAppController.Instance.myAvatar);
            //otherInfor.UpdatePlayerData(EightBallNetworkManager.Instance.MatchInformationData.SecondUserName, EightBallNetworkManager.Instance.MatchInformationData.AvatarSecondUser);
            p2PGUI.SetActive(true);
            trickShotGUI.SetActive(false);
            if (EightBallGameSystem.Instance.IsMyTurn && isFirstShot)
            {
                EightPopupCtrl.ShowPopupPlayerBreaking();
            }
            iconRewardImg.sprite = ResourceManager.Instance.GetIconReward((RewardType)EightBallGameSystem.Instance.reward.RewardType);
            valueRewardTxt.text = EightBallGameSystem.Instance.reward.Reward_.ToString();
        }
        else
        {
            //singlePlayerInfo.UpdatePlayerData(HCAppController.Instance.userInfo.UserName, HCAppController.Instance.myAvatar);
            p2PGUI.SetActive(false);
            trickShotGUI.SetActive(true);
        }
        if (HCAppController.Instance.findingRoomResponse != null)
        {
            Debug.Log("EightBallGameSystem HCAppController.Instance.findingRoomResponse.Mode " + HCAppController.Instance.findingRoomResponse.Mode);
            if (EightBallGameSystem.Instance.CurrentMode == PlayMode.Online)//dong bo
            {
                HCAppController.Instance.LoadUserInfoInGame(HCAppController.Instance.findingRoomResponse, mainInfor.txtName, mainInfor.imgAvatar, otherInfor.txtName, otherInfor.imgAvatar);
            }
            else
            {
                HCAppController.Instance.LoadUserInfoInGame(HCAppController.Instance.findingRoomResponse, singlePlayerInfo.txtName, singlePlayerInfo.imgAvatar, null, null);
            }
        }
        else
        {
            if (EightBallGameSystem.Instance.CurrentMode == PlayMode.Practice)//dong bo
            {
                
                HCAppController.Instance.LoadUserInfoInGame(null, singlePlayerInfo.txtName, singlePlayerInfo.imgAvatar, null, null);
            }
        }
        
    }
    public void OnDestroy()
    {
        EightBallGameSystem.Instance.OnBallOnPotCallback -= BallOnPot;
        EightBallGameSystem.Instance.OnEndGameCallback -= ShowResult;
        EightBallGameSystem.Instance.OnGetAllBallsGroupCallback -= UpdateBallGroup;
        EightBallGameSystem.Instance.OnEndBallCallback -= OnEndBall;
        EightBallGameSystem.Instance.OnTimeUpdateCallback -= updateTime;
        EightBallGameSystem.Instance.OnGetBallGroupCallback += ShowGetGroupMessage;
        
        //Single
        EightBallGameSystem.Instance.OnRemainShootUpdateCallback -= UpdateRemainShoot;
        EightBallGameSystem.Instance.OnSinglePointUpdateCallback -= UpdateSingePoint;
        EightBallGameSystem.Instance.OnSingleRemainTimeUpdate -= UpdateRemainTimeout;
        EightBallGameSystem.Instance.OnSingleEndGameCallback -= ShowSingleResult;
        EightBallGameSystem.Instance.OnSingleFailedPocketBallCallback -= ShowSingleFailedPocketBall;

    }

    public void UpdateBallGroup(List<int> myBallIds, List<int> otherBallIds)
    {
        string allballs = "";
        foreach (var ball in myBallIds)
        {
            allballs += ball.ToString() + ",";
        }

        allballs += "====";
        foreach (var ball in otherBallIds)
        {
            allballs += ball.ToString() + ",";
        }
        
        Debug.Log("Update ball group : " + allballs);
        mainInfor.ClearActiveImgSpot();
        mainInfor.AddBalls(myBallIds);
        //foreach (var myBall in myBallIds)
        //{
        //    if(EightBallGameSystem.Instance.CurrentMode == PlayMode.Online)
        //    {
        //        mainInfor.AddBalls(myBall);
        //    }
        //    else
        //    {
        //        var ballIndex = myBall == 8 ? 5 : 8;
        //        mainInfor.AddBall(myBall, GetSpriteByBallID(ballIndex));
        //    }
        //}
        if(EightBallGameSystem.Instance.CurrentMode == PlayMode.Online)
        {
            //otherInfor.ClearActiveImgSpot();
            //foreach (var otherBall in otherBallIds)
            //{
            //    otherInfor.AddBall(otherBall, GetSpriteByBallID(otherBall));
            //}
            otherInfor.ClearActiveImgSpot();
            otherInfor.AddBalls(otherBallIds);
        }
    }

    public async void BallOnPot(int ballId, bool isMyBall)
    {
        
        bool isMyTurn = EightBallGameSystem.Instance.IsMyTurn;
        await UniTask.Delay(1000);
        if (EightBallGameSystem.Instance.CurrentMode == PlayMode.Online)
        {
            //if ball 8 or white ball , not remove
            //if (ballId != 8 && ballId != EightBallGameSystem.WHITE_BALL_ID)
            //{
            //    var playerInfo = isMyBall ? mainInfor : otherInfor;
            //    Debug.Log("Xoa bal ne: " + ballId);
            //    playerInfo.RemoveBall(ballId);
            //}
            var playerInfo = isMyBall ? mainInfor : otherInfor;
            Debug.Log("Xoa bal ne: " + ballId);
            playerInfo.RemoveBall(ballId);
        }
        else
        {
            //if white ball , not remove
            if (ballId != EightBallGameSystem.WHITE_BALL_ID)
            {               
                //Debug.Log("Xoa bal ne: " + ballId);
                //singlePlayerInfo.RemoveBall(ballId);
            }
            singlePlayerInfo.RemoveBall(ballId);
        }

        if (isMyTurn&& ballId == EightBallGameSystem.WHITE_BALL_ID)
        {
            ShowBallInPotMessage();
        }

    }

    private void OnEndBall(EightBallShootStatus status)
    {
        
        if(EightBallGameSystem.Instance.CurrentMode != PlayMode.Online)
            return;
        
        ResetTime();

        if (newUpdateGroup != BallGroup.BgtNone)
        {           
            //ShowMessage("Get Group  : " + newUpdateGroup);
            newUpdateGroup = BallGroup.BgtNone;
        }
        
        if (EightBallGameSystem.Instance.IsMyTurn && EightBallGameSystem.Instance.CurrentMode == PlayMode.Online)
        {
            //ShowInTurnMessage();
            EightPopupCtrl.ShowPopupYourTurn();
        }

        if (isFirstShot)
        {
            isFirstShot = false;
        }
        //ShowPopupEndShot(status);
    }

    public void ShowPopupEndShot(EightBallShootStatus status)
    {
        if (EightBallGameSystem.Instance.CurrentMode != PlayMode.Online) return;
        switch (status)
        {            
            case EightBallShootStatus.SecWhiteBallNotImpactAnything:
                EightPopupCtrl.ShowMesageDialog("The cue ball did not Strike another ball",0, () =>
                {
                    ShowUserHandWhiteBall(false);
                });
                break;
            case EightBallShootStatus.InvalidBallGroup:
                //EightPopupCtrl.ShowPopupHitBallAnotherGroup(isMainFirst);
                EightPopupCtrl.ShowPopupHitBallAnotherGroup(EightBallGameSystem.Instance.MyBallGroup == BallGroup.Bgt17, 0, () =>
                {
                    ShowUserHandWhiteBall(false);
                });
                break;
            case EightBallShootStatus.WhiteBallNotImpactBall:
                if(EightBallGameSystem.Instance.FirstBallIDCollection >= 0)
                {

                    EightPopupCtrl.ShowPopupHitBallAnotherGroup(newUpdateGroup == BallGroup.Bgt17, 0, () =>
                    {
                        ShowUserHandWhiteBall(false);
                    });
                }
                else
                {
                    EightPopupCtrl.ShowMesageDialog("The cue ball did not Strike another ball", 0, () =>
                    {
                        ShowUserHandWhiteBall(false);
                    });
                }
                
                
                break;
        }
    }
    private void ResetTime()
    {
        if(mainInfor!= null) mainInfor.SetTimer(0);
        if(otherInfor != null) otherInfor.SetTimer(0);
    }
    private void updateTime(bool isMyTime, float value)
    {
        var userInfo = isMyTime ? mainInfor : otherInfor;
        if(userInfo != null) userInfo.SetTimer(value);
    }

    public Sprite GetSpriteByBallID(int ballID)
    {
        if (ballID == 8)
        {
            return eightBallSprite;
        }
        int ballIndex = ballID > 8 ? ballID - 2 : ballID - 1;
        return ballIconSprite[ballIndex];
    }
    
    public void ShowResult(bool isWin)
    {
        //_resultDialogHandler.Show(isWin);
         return;
        //_resultDialogHandler.Show(isWin);
        if(EightBallGameSystem.Instance.CurrentMode == PlayMode.Single)
        {
            EightPopupCtrl.ShowEndGamePopup(isWin);
        }
        else
        {
            _resultDialogHandler.Show(isWin);
        }
    }

    private void ShowSingleResult(uint points)
    {
        // return;
        bool isWin = EightBallGameSystem.Instance.AllBallIDsCount <= 7;
        _resultDialogHandler.SetPointShow(points);
        EightPopupCtrl.ShowTimeupPopup(0,() =>
        {
            ShowEndGameResult(isWin, 0, () =>
            {
                //SceneManager.LoadScene("Home");
                ShowResult(isWin);
            });
        });
        //_resultDialogHandler.Show(points);
    }

    void ShowEndGameResult(bool isWin, float delay = 0, Action hideSuccessed = null)
    {
        EightPopupCtrl.ShowEndGamePopup(isWin, delay, hideSuccessed);
    }
    private void ShowBallInPotMessage()
    {
        ShowMessage("You potted the cue ball.", () => { ShowUserHandWhiteBall(false); });
    }

    
    private void ShowGetGroupMessage(BallGroup groupType)
    {
        newUpdateGroup = groupType;
        Debug.LogWarning("Get Group  : " + newUpdateGroup);
        if (EightBallGameSystem.Instance.CurrentMode != PlayMode.Online) return;
        if(!_isShowMessGetGroup)
        {
            switch (newUpdateGroup)
            {
                case BallGroup.Bgt17:
                    EightPopupCtrl.ShowMesageDialog("You are solids!", 1.5f);
                    break;
                case BallGroup.Bgt915:
                    EightPopupCtrl.ShowMesageDialog("You are Stripes!", 1.5f);
                    break;
            }
            _isShowMessGetGroup = true;
        }
        
    }

    private void ShowInTurnMessage()
    {
        ShowMessage("It's your turn!" );

    }

    private void ShowMessage(string message, Action action = null)
    {
        //var newMessageInstance = GameObject.Instantiate(sampleMessage, transform);
        //newMessageInstance.GetComponent<EightBallMessageDialogHandler>().Show(message);
        EightPopupCtrl.ShowMesageDialog(message, 0, action);
    }
    
    public void LoadMyData()
    {

    }

    public void LoadOtherPlayerData()
    {

    }
    public void ShowUserHandWhiteBall(bool isUser)
    {
        if (EightBallGameSystem.Instance.CurrentMode != PlayMode.Online) return;
        var user = isUser ? mainInfor : otherInfor;
        EightPopupCtrl.ShowMesageDialog($"{user.name} has the ball in hand.");
    }

    #region SINGLE
    
    private void UpdateRemainShoot(int remainTimes)
    {
        for (int i = 0; i < remainShootCountImages.Length; i++)
        {
            remainShootCountImages[i].gameObject.SetActive(i<remainTimes);
        }
    }
    
    private void ShowSingleFailedPocketBall(bool isTrigger)
    {
        if (EightBallGameSystem.Instance.CurrentMode != PlayMode.Online) return;
        string mess = isTrigger ? "You failed to pot a ball." : "The cue ball did not Strike another ball.";
        ShowMessage(mess);
    }

    private void UpdateRemainTimeout(float remainTime)
    {
        var timespan = TimeSpan.FromSeconds(remainTime);
        remainTimeText.text = timespan.ToString(@"mm\:ss");
    }

    private void UpdateSingePoint(uint point)
    {
        remainPointText.text = point.ToString();
    }
    
    #endregion
}