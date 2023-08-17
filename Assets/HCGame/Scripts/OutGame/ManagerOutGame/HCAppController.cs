using AssemblyCSharp.GameNetwork;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Bingo;
using DG.Tweening;
using MiniGame.MatchThree.Scripts.Network;
using Newtonsoft.Json.Linq;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Networking;
using CCData = HcGames.CCData;
using PackageData = HcGames.PackageData;
using TMPro;
using UnityEngine.UI;

public class HCAppController : MonoBehaviour
{
    #region HC_TechAndLocal

    public static HCAppController Instance;

    string defaultUrl_EIGHTBALL = "ws://18.141.169.208:8086"; //cloud
    //string defaultUrl_EIGHTBALL = "ws://192.168.2.38:8080";//local quang

    string defaultUrl_SOLIRAITE = "ws://18.141.169.208:8086"; //cloud
    //string defaultUrl_SOLIRAITE = "ws://192.168.2.38:8080";//quang
    //   string defaultUrl_SOLIRAITE = "ws://192.168.2.99:8080";//local pc nghia
    // string defaultUrl_SOLIRAITE = "ws://192.168.3.219:8080";//local pc nghia2

  //  private string bingoWS = "ws://103.171.26.189:8000";//Hoang
     private string bingoWS = "ws://18.141.169.208:8086"; //Cloud
   // private string bingoWS = "ws://192.168.3.185:8080"; //Hung
   //   private string bingoWS = "ws://192.168.2.99:8080";//nghia
    //private string bingoWS = "ws://127.0.0.1:8080";//nghia 127
    //private string bingoWS = "ws://192.168.2.38:8080";//local quang

    // private string match3WS = "ws://192.168.2.99:8080"; //Nghia
    private string match3WS = "ws://18.141.169.208:8086"; //Cloud

    private string urlBubblesShot = "ws://18.141.169.208:8086";
    //private string urlBubblesShot = "ws://192.168.3.185:8080"; //local hung

    //   private string url_HCApp = "ws://27.72.59.161:8022/api/v1/websocket";//Cloud
    private string url_HCApp = "ws://192.168.2.68:8022/api/v1/websocket"; //techasians
    //private string url_HCApp = "ws://192.168.2.113:8080/api/v1/websocket";//local pc Hai
    //   private string url_HCApp = "ws://192.168.2.6:8080/api/v1/websocket";//local pc Hiep
    //private string url_HCApp = "ws://192.168.2.250:8080/api/v1/websocket";//local pc Huy
    //private string url_HCApp = "ws://192.168.2.30:8080/api/v1/websocket";//local pc long

    //private string url_HCAppWebApi = "http://27.72.59.161:8055/api/login-with-guest";//Cloud
    private string url_HCAppWebApi = "http://192.168.2.68:8055/api/login-with-guest"; //techasians
    //private string url_HCAppWebApi = "http://192.168.2.6:8055/api/login-with-guest"; //hiep
    //private string url_HCAppWebApi = "http://192.168.3.195:8888/api/login-with-guest"; //hai

    private string urlDailyMission = "http://192.168.2.68:8022/api/get-daily-mission"; //cloud
    //private string urlDailyMission = "http://192.168.3.195:8080/api/get-daily-mission";//local hai

    private string url_DeleteAccount = "http://192.168.2.68:8022/api/delete-account"; //techasians
    private string urlReferenceCode = "http://192.168.2.68:8022/api/reference-code"; //techasians
    private string urlCouponCode = "http://192.168.2.68:8022/api/coupon-code"; //techasians
    private string urlInvitedFriend = "http://192.168.2.113:8080/api/claim/invited-friend";//hai

    private string urlEditUser = "http://192.168.2.68:8022/api/edit-user"; //techasians

    //private string urlEditUser = "http://192.168.3.195:8080/api/edit-user"; //local hai
    private string urlClaimAllHistory = "http://192.168.2.68:8022/api/claim-all"; //techasians
    private string urlClaimItemHistory = "http://192.168.2.68:8022/api/claim"; //techasians

    private string urlBonusGame = "ws://18.141.169.208:8086";
    //private string urlBonusGame = "ws://192.168.2.38:8080";//quanglocal


    private string urlDailyReward = "http://192.168.2.68:8022/api/get-daily-reward";

    //private string urlDailyReward = "http://192.168.3.195:8080/api/get-daily-reward";//local hai
    private string urlFreeGold = "http://192.168.2.68:8022/api/free-gold";

    #endregion HC_TechAndLocal

    // #region HC_Korean
    //
    // public static HCAppController Instance;
    // string defaultUrl_EIGHTBALL = "ws://18.141.169.208:8090"; //cloud
    //
    // string defaultUrl_SOLIRAITE = "ws://18.141.169.208:8089"; //cloud
    //
    // private string bingoWS = "ws://18.141.169.208:8089"; //Cloud
    //
    // private string match3WS = "ws://18.141.169.208:8089"; //Cloud
    //
    // private string urlBubblesShot = "ws://18.141.169.208:8089";
    // private string urlBonusGame = "ws://18.141.169.208:8089";
    // private string url_HCApp = "ws://hcoutgamestg.techasians.com/api/v1/websocket"; // server stg
    //
    // private string url_HCAppWebApi = "http://hcloginstg.techasians.com/api/login-with-guest"; //stg
    //
    // private string urlDailyMission = "http://hcoutgamestg.techasians.com/api/get-daily-mission"; // stg
    //
    // private string url_DeleteAccount = "http://hcoutgamestg.techasians.com/api/delete-account"; //stg
    // private string urlReferenceCode = "http://hcoutgamestg.techasians.com/api/reference-code"; //stg
    // private string urlCouponCode = "http://hcoutgamestg.techasians.com/api/coupon-code"; //stg
    //
    // private string urlEditUser = "http://hcoutgamestg.techasians.com/api/edit-user"; //stg
    // private string urlClaimAllHistory = "http://hcoutgamestg.techasians.com/api/claim-all"; //stg
    // private string urlClaimItemHistory = "http://hcoutgamestg.techasians.com/api/claim"; //stg
    //
    // private string urlDailyReward = "http://hcoutgamestg.techasians.com/api/get-daily-reward"; //stg
    // private string urlFreeGold = "http://hcoutgamestg.techasians.com/api/free-gold"; //stg
    //
    // #endregion HC_Korean

    private GameStatus currentStatus = GameStatus.Disconneced;
    public string currentDeviceId;

    public Sprite myAvatar;
    public Sprite myBoder;
    public Sprite myBackground;
    public string myurlbackround;
    public string myUrlBoder;

    public NetworkControlerHCApp networkControlerHCApp = new NetworkControlerHCApp();
    public UserDataProto userInfo;
    public ListMiniGameProto listMiniGame;


    public ListDetailMiniGameProto listDetailMiniGame;


    public ListHcPlayHistoryProto listActivityHistory;
    public ListHcDailyRewardProto listHcDailyReward;
    public ListHcDailyMission listHcDailyMission;
    public Action<MatchInformation> actionMatching;
    //public ListUserRankingProto listUserRanking;
    public long totalJackpot;
    public ListTopJackpotProto listTopJackpot;

    public Dictionary<long, HcNotificationProto> listHcNotification;
    public GameType currentGameType;

    public DetailMiniGameProto currentDetailMiniGameProto;
    public ListMiniGameEventProto ListMiniGameEventProto;
    public List<LevelUp> listLevelUp;
    public TimeSpan timeGoldGift;
    public HcGames.FindingRoomResponse findingRoomResponse;

    float pingInterval = 5.0f;
    float timeSinceLastPing = 0.0f;

    #region AvatarFrameUpdate

    public class SpriteAvatarFrame
    {
        public string nameImage;
        public Sprite spriteimage;

        public SpriteAvatarFrame(string nameImage, Sprite spriteimage)
        {
            this.nameImage = nameImage;
            this.spriteimage = spriteimage;
        }
    }

    public List<String> ListUrlSpriteFrameAvatar = new List<string>();
    public Dictionary<int, SpriteAvatarFrame> lisSpriteFrameAvatar = new Dictionary<int, SpriteAvatarFrame>();
    public ListFrameAvatar _ListFrameAvatar;

    public void startLoadSpriteFrameAvatar()
    {
        StartCoroutine(LoadSpriteFrameAvatar());
    }

    public IEnumerator LoadSpriteFrameAvatar()
    {
        lisSpriteFrameAvatar = new Dictionary<int, SpriteAvatarFrame>();
        lisSpriteFrameAvatar.Clear();

        for (int i = 0; i < _ListFrameAvatar.FrameAvatar.Count; i++)
        {
            UnityWebRequest www =
                UnityWebRequestTexture.GetTexture(_ListFrameAvatar.FrameAvatar[i].FrameAvatar_.ToString());
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));
                if (!lisSpriteFrameAvatar.ContainsKey(i))
                {
                    lisSpriteFrameAvatar.Add(i,
                        new SpriteAvatarFrame(_ListFrameAvatar.FrameAvatar[i].FrameAvatar_, sprite));
                }
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                //Debug.LogError("Lỗi not success: "+www.error);
            }
            //Debug.Log("listavat: 4");
        }
    }

    #endregion

    #region background

    public class SpriteBackGround
    {
        public string nameImage;
        public Sprite spriteimage;

        public SpriteBackGround(string nameImage, Sprite spriteimage)
        {
            this.nameImage = nameImage;
            this.spriteimage = spriteimage;
        }
    }

    public List<String> ListUrlSpriteBackround = new List<string>();
    public Dictionary<int, SpriteBackGround> lisSpriteBackround = new Dictionary<int, SpriteBackGround>();
    public ListBackground ListBackground;
    public int chooseCurrentBackground;

    public void startLoadSpriteBackground()
    {
        StartCoroutine(LoadSpriteBackground());
    }

    public IEnumerator LoadSpriteBackground()
    {
        lisSpriteBackround = new Dictionary<int, SpriteBackGround>();
        lisSpriteBackround.Clear();

        for (int i = 0; i < ListBackground.Background.Count; i++)
        {
            UnityWebRequest www =
                UnityWebRequestTexture.GetTexture(ListBackground.Background[i].Background_.ToString());
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));
                if (!lisSpriteBackround.ContainsKey(i))
                {
                    lisSpriteBackround.Add(i, new SpriteBackGround(ListBackground.Background[i].Background_, sprite));
                }
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                //Debug.LogError("Lỗi not success: "+www.error);
            }
            //Debug.Log("listavat: 4");
        }
    }

    #endregion

    #region AvatarUpload

    public class SpriteAvatar
    {
        public string nameImage;
        public Sprite SpriteImage;

        public SpriteAvatar(string nameImage, Sprite spriteImage)
        {
            this.nameImage = nameImage;
            SpriteImage = spriteImage;
        }
    }

    public List<String> ListUrlSpriteAvatar = new List<string>();
    public Dictionary<int, SpriteAvatar> lisSpriteAvatar = new Dictionary<int, SpriteAvatar>();
    public ListAvatar ListAvatar;
    public int chooseCurrentAvatar;

    public void startLoadSpriteCCC()
    {
        StartCoroutine(LoadSpriteCCC());
    }

    public IEnumerator LoadSpriteCCC()
    {
        lisSpriteAvatar = new Dictionary<int, SpriteAvatar>();
        lisSpriteAvatar.Clear();
        Debug.Log("listavat" + ListAvatar.Avatar.Count());
        for (int i = 0; i < ListAvatar.Avatar.Count; i++)
        {
            // Debug.LogError("name Avatar: "+i+" : "+ListAvatar.Avatar[i].Avatar.ToString());
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(ListAvatar.Avatar[i].Avatar_.ToString());
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));
                if (!lisSpriteAvatar.ContainsKey(i))
                {
                    lisSpriteAvatar.Add(i, new SpriteAvatar(ListAvatar.Avatar[i].Avatar_.ToString(), sprite));
                }
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                //Debug.LogError("Lỗi not success: "+www.error);
            }
            //Debug.Log("listavat: 4");
        }
    }

    #endregion

    Dictionary<GameType, string> ListSceneName = new Dictionary<GameType, string>();

    //FAKEDATA
    public TournamentProto tournamentProto;

    private string tokenLogin;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Debug.LogError("MULTIINSTANCE" + name);
            Destroy(this);
        }

        //fake data daily mission
        List<HcDailyRewardProto> fake = new List<HcDailyRewardProto>();
        for (int i = 0; i < 10; i++)
        {
        }

        ListSceneName.Add(GameType.Billard, "GameScene");
        ListSceneName.Add(GameType.Puzzle, "M3_NetworkGamePlay");
        ListSceneName.Add(GameType.Solitaire, "Solitaire_GameScene");
        ListSceneName.Add(GameType.Bingo, "Game Bingo");
        ListSceneName.Add(GameType.Bubble, "BubbleGameplay");
        ////////////////////////
    }

    private void Update()
    {
        timeSinceLastPing += Time.deltaTime;
        if (timeSinceLastPing >= pingInterval)
        {
            Debug.Log("HCAPP ping");
            HCAppController.Instance?.networkControlerHCApp?.SendPing();
            timeSinceLastPing = 0.0f;
        }

        //HCSocketNetworkManager.Instance?.SendPing();
        //BubbesShotManager.Instance?.networkBubblesShot?.SendPing();
    }

    /*
    private void UpdateMoneyToUserInfo(UpdateMoney updateMoney)
    {
        HCAppController.Instance.userInfo.UserGold += updateMoney.Gold;
        HCAppController.Instance.userInfo.UserTicket += updateMoney.Ticket;
        HCAppController.Instance.userInfo.UserToken += updateMoney.Token;
    }
    */

    // Start is called before the first frame update
    public void DisconnectAllMiniGame()
    {
        if (EightBallNetworkManager.Instance.GetIsConnected()) EightBallNetworkManager.Instance.Disconnect(true);
    }

    public void SetCurrentDetailMiniGame(DetailMiniGameProto detailMiniGameProto)
    {
        currentDetailMiniGameProto = detailMiniGameProto;
    }

    public DetailMiniGameProto GetCurrentDetailMiniGame()
    {
        return currentDetailMiniGameProto;
    }

    public List<DetailMiniGameProto> GetListDetailMiniGameProto(GameType gameType)
    {
        if (listDetailMiniGame == null) return new();
        List<DetailMiniGameProto> result = new();
        result = listDetailMiniGame.ListDetailMiniGameProto_.Where(x => x.MiniGameId == (long)gameType).ToList();
        return result;
    }

    public void ConnectGameAndPlay(GameType gameType, long miniGameEventId, int modeType, int numberPlayer, CCData data,
        Reward reward)
    {
        switch (gameType)
        {
            case GameType.Billard:
                HCAppController.Instance.InitNetworkEightBallNew(miniGameEventId, modeType, numberPlayer, 0, data,
                    reward);
                break;
            case GameType.Solitaire:
                HCAppController.Instance.InitNetworkSoliraite(miniGameEventId, modeType, numberPlayer, 0, data);
                break;

            case GameType.Bingo: //mmr get from HCAppController.Instance.userInfo;
                Bingo_NetworkManager.instance.SendRequestFindRoomFromHCGameList(miniGameEventId, modeType, numberPlayer,
                    bingoWS, data);
                break;

            case GameType.Puzzle: //mmr get from HCAppController.Instance.userInfo;
                Debug.Log("ConnectGameByGameType Puzzle");
                MatchThreeNetworkManager.Instance.SendRequestFindRoomFromHCGameList(miniGameEventId, modeType,
                    numberPlayer, match3WS, data);
                break;
            case GameType.Bubble:
                BubbesShotManager.Instance.StartFindRoom(miniGameEventId, modeType, numberPlayer, GetUrlBubblesShot(),
                    data);
                break;
        }
    }

    public void LoadUserInfoInGame(HcGames.FindingRoomResponse findingRoomResponse, TMP_Text userName1,
        Image userAvatar1, TMP_Text userName2, Image userAvatar2)
    {
        //load data user1
        if (userName1 != null) userName1.text = userInfo.UserName;
        if (userAvatar1 != null) StartCoroutine(HCHelper.LoadAvatar(userInfo.UserAvatar, userAvatar1));
        //load data usser 2
        if (findingRoomResponse != null && findingRoomResponse.Mode == 2)
        {
            if (userName2 != null) userName2.text = findingRoomResponse.OtherPlayerName;
            if (userAvatar2 != null)
                StartCoroutine(HCHelper.LoadAvatar(findingRoomResponse.OtherPlayerAvatar, userAvatar2));
        }
    }

    public string GetNameScene(GameType gameType)
    {
        if (ListSceneName.ContainsKey(gameType))
        {
            return ListSceneName[gameType];
        }

        return string.Empty;
    }

    public int GetMmrByGameType(GameType gameType)
    {
        if (userInfo == null || userInfo.Mmr == null) return 0;
        var mmrGame = userInfo.Mmr.FirstOrDefault(x => x.MiniGameCode == (int)gameType);
        if (mmrGame == null) return 0;
        else return mmrGame.Mmr;
    }

    public ListHcPlayHistoryProto GetActivityHistory()
    {
        return listActivityHistory;
    }

    public void SetTokenLogin(string token)
    {
        tokenLogin = token;
        Debug.Log("HCAppController SetTokenLogin " + HCAppController.Instance.tokenLogin);
    }

    public string GetTokenLogin()
    {
        return tokenLogin;
    }

    #region Url

    public void SetUrlHCAppWebSocket(string url)
    {
        url_HCApp = url;
    }

    public string GetUrlHCAppWebSocket()
    {
        return url_HCApp;
    }

    public void SetUrlFreeGold(string url)
    {
        urlFreeGold = url;
    }

    public string GetUrlFreeGold()
    {
        return urlFreeGold;
    }

    public void SetUrlClaimItemHistory(string url)
    {
        urlClaimItemHistory = url;
    }

    public string GetUrlClaimItemHistory()
    {
        return urlClaimItemHistory;
    }

    public void SetUrlClaimAllHistory(string url)
    {
        urlClaimAllHistory = url;
    }

    public string GetUrlClaimAllHistory()
    {
        return urlClaimAllHistory;
    }

    public void SetUrlHCAppWebApi(string url)
    {
        url_HCAppWebApi = url;
    }

    public string GetUrlHCAppWebApi()
    {
        return url_HCAppWebApi;
    }

    public void SetUrlEightBall(string url)
    {
        defaultUrl_EIGHTBALL = url;
    }

    public string GetUrlEightBall()
    {
        return defaultUrl_EIGHTBALL;
    }

    public string GetUrlBubblesShot()
    {
        return urlBubblesShot;
    }

    public void SetUrlBubblesShot()
    {
    }

    public string GetUrlInvitedFriend()
    {
        return urlInvitedFriend;
    }
    public void SetUrlCouponCode(string url)
    {
        urlCouponCode = url;
    }

    public string GetUrlCouponCode()
    {
        return urlCouponCode;
    }

    public void SetUrlReferenceCode(string url)
    {
        urlReferenceCode = url;
    }

    public string GetUrlReferenceCode()
    {
        return urlReferenceCode;
    }

    public void SetUrlEditUser(string url)
    {
        urlEditUser = url;
    }

    public string GetUrlEditUser()
    {
        return urlEditUser;
    }

    public void SetUrl_DeleteAccount(string url)
    {
        url_DeleteAccount = url;
    }

    public string GetUrl_DeleteAccount()
    {
        return url_DeleteAccount;
    }

    public void SetBingoWs(string url)
    {
        bingoWS = url;
    }

    public string GetBingoWs()
    {
        return bingoWS;
    }

    public void SetMatch3Ws(string url)
    {
        match3WS = url;
    }

    public string GetMatch3Ws()
    {
        return match3WS;
    }

    public void SetUrlBonusGame(string url)
    {
        urlBonusGame = url;
    }

    public string GetUrlBonusGame()
    {
        return urlBonusGame;
    }

    public void SetUrlDailyReward(string url)
    {
        urlDailyReward = url;
    }

    public string GetUrlDailyReward()
    {
        return urlDailyReward;
    }

    public void SetUrlDailyMission(string url)
    {
        urlDailyMission = url;
    }

    public string GetUrlDailyMission()
    {
        return urlDailyMission;
    }

    #endregion Url

    public void ApplyEditYourProfile(string newUserName)
    {
        EditUserRequest request = new EditUserRequest
        {
            userId = userInfo.UserCodeId,
            username = newUserName
        };
        StartCoroutine(APIUtils.RequestWebApiPost(GetUrlEditUser(), JsonUtility.ToJson(request), tokenLogin,
            (data) =>
            {
                Debug.Log("RequestWebApiPost EditUserRequest data " + data);
                Debug.Log("RequestWebApiPost EditUserRequest data " + HCAppController.Instance.userInfo.UserAvatar);
                //HCAppController.Instance.userInfo.UserName = data.

                JObject json = JObject.Parse(data);
                bool issucsse = (bool)json["isSuccess"];
                string message = (string)json["message"];
                if (issucsse)
                {
                    HcPopupManager.Instance.ShowNotifyPopup(message.ToString(),"Notification");
                    EditYourProfilePopup.CloseAllPopup();
                   
                }
                else
                {
                    HcPopupManager.Instance.ShowNotifyPopup(message.ToString(),"Notification");
                    StartCoroutine(ShowEditYourProfile());
                }
                
            }));
    }

    IEnumerator ShowEditYourProfile()
    {
        EditYourProfilePopup.CloseAllPopup();
        yield return  new WaitForSeconds(1f);
        HcPopupManager.Instance.ShowEditYourProfile();
    }

    public void ClaimFreeGold(Action<string> claimFreeGoldSuccess)
    {
        ClaimFreeGoldRequest data = new ClaimFreeGoldRequest
        {
            userCodeId = userInfo.UserCodeId
        };
        Debug.Log("ClaimFreeGold " + data.userCodeId);
        StartCoroutine(APIUtils.RequestWebApiPost(GetUrlFreeGold(), JsonUtility.ToJson(data), tokenLogin,
            (data) => { claimFreeGoldSuccess?.Invoke(data); }));
    }

    public void ClaimDailyReward(int day, Action claimSuccess)
    {
        DailyRewardRequest request = new DailyRewardRequest
        {
            day = day,
            userCodeId = userInfo.UserCodeId
        };

        Debug.Log("AcceptClick token " + tokenLogin);
        StartCoroutine(APIUtils.RequestWebApiPost(GetUrlDailyReward(), JsonUtility.ToJson(request), tokenLogin,
            (data) =>
            {
                claimSuccess?.Invoke();
                DailyRewardClaimSuccess(data);
            }));
    }

    public void DailyRewardClaimSuccess(string dataRespon)
    {
        var data = JsonUtility.FromJson<DailyRewardRespon>(dataRespon);
        if (!data.isSuccess)
        {
            //show noti
            return;
        }

        listHcDailyReward.CountCheck = data.totalDayClaim;
        /*
        if (PopupManager.ins.dailyReward.gameObject.activeInHierarchy)
        {
            PopupManager.ins.dailyReward.ChangeCountCheck(data.totalDayClaim);
        }*/
    }

    public void ConnectToServer(string url)
    {
        networkControlerHCApp.InitNetwork(url);
    }

    #region 8Ball

    public void InitNetworkEightBallNew(long miniGameEventId, int modeType, int numberPlayer, int mmr,
        CCData _data = null, Reward reward = null)
    {
        currentGameType = GameType.Billard;
        Debug.Log("-----------InitNetworkEightBall-------> " + miniGameEventId);
        string initUrl = defaultUrl_EIGHTBALL;
        CCData ccData = null;
        if (_data == null)
        {
            ccData = new CCData
            {
                MiniGameEventId = (ulong)miniGameEventId,
                Token = tokenLogin,
                WaitingTimeId = 2111,
                GameMode = modeType,
                NumberInMiniGame = numberPlayer
            };
        }
        else
        {
            ccData = _data;
        }

        EightBallGameSystem.Instance.reward = reward;
        EightBallGameSystem.Instance.StartFindRoomNew(initUrl, userInfo.UserCodeId, mmr, ccData, userInfo.Level);
    }

    public async void InitNetworkEightBall(long miniGameEventId, int modeType, int numberPlayer, int mmr,
        CCData _data = null)
    {
        currentGameType = GameType.Billard;
        Debug.Log("-----------InitNetworkEightBall-------> " + miniGameEventId);
        string initUrl = defaultUrl_EIGHTBALL;
        CCData ccData = null;
        if (_data == null)
        {
            ccData = new CCData
            {
                MiniGameEventId = (ulong)miniGameEventId,
                Token = tokenLogin,
                WaitingTimeId = 2111,
                GameMode = modeType,
                NumberInMiniGame = numberPlayer
            };
        }
        else
        {
            ccData = _data;
        }

        var findRoomResult =
            await EightBallGameSystem.Instance.StartFindRoom(initUrl, userInfo.UserCodeId, mmr, ccData,
                userInfo.Level);

        Debug.Log($"[HCAPPController] : findroom result : {findRoomResult}");
        if (findRoomResult)
        {
            readyToPlayEightBall();
        }
        else
        {
            SceneManager.LoadScene("Home");
        }
    }

    public void UpdateTimeGoldGift(DateTime time)
    {
        if (time >= DateTime.UtcNow)
        {
            timeGoldGift = TimeSpan.FromSeconds(0);
        }
        else
        {
            timeGoldGift = DateTime.UtcNow - time;
        }
    }

    private void readyToPlayEightBall()
    {
        Debug.Log("-----------readyToPlayEightBall");
        currentStatus = GameStatus.Ready;
        //await UniTask.Delay(TimeSpan.FromSeconds(1));
        //SceneManager.LoadScene("GameScene");
        HcPopupManager.Instance.ShowEightGameLoading(true, GameType.Billard);
    }

    #endregion 8Ball

    #region Solitaire

    public void InitNetworkSoliraite(long miniGameEventId, int modeType, int numberPlayer, int mmr,
        HcGames.CCData _data)
    {
        currentGameType = GameType.Solitaire;
        Debug.Log("-----------InitNetworkSoliraite-------> " + miniGameEventId);
        Debug.Log("-----------InitNetworkSoliraite-------tokenLogin> " + tokenLogin);
        string initUrl = defaultUrl_SOLIRAITE;
        CCData ccData = null;
        if (_data == null)
        {
            ccData = new CCData
            {
                MiniGameEventId = (ulong)miniGameEventId,
                Token = tokenLogin,
                WaitingTimeId = 2111,
                GameMode = modeType,
                NumberInMiniGame = numberPlayer
            };
        }
        else
        {
            ccData = _data;
        }

        Solitaire.SGameManager.Instance.StartFindRoom(initUrl, userInfo.UserCodeId, mmr, ccData,
            userInfo.Level);
    }

    #endregion Solitaire


    #region GetUserCurrency

    public long GetCurrencyByType(RewardType type)
    {
        long tmp = 0;
        switch (type)
        {
            case RewardType.Ticket:
                tmp = userInfo.UserTicket;
                break;
            case RewardType.Token:
                tmp = userInfo.UserToken;
                break;
            case RewardType.Gold:
                tmp = userInfo.UserGold;
                break;
            default:
                break;
        }

        return tmp;
    }

    #endregion


    #region GOTOSCENE

    public void LoadScene(string sceneName, System.Action callback)
    {
        StartCoroutine(LoadSceneAsync(sceneName, callback));
    }

    private IEnumerator LoadSceneAsync(string sceneName, System.Action callback)
    {
        var tmp = SceneManager.GetActiveScene();
        SceneManager.UnloadSceneAsync(tmp);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        if (callback != null)
        {
            callback();
        }
    }

    public void GotoHome()
    {
        SceneManager.LoadScene("Home");
    }

    #endregion
}