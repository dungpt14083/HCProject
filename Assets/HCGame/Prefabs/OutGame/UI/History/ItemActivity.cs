using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemActivity : MonoBehaviour
{
    #region THANGTODO

    [SerializeField] private TMP_Text txtNameTournament;
    [SerializeField] private Image iconMiniGame;
    [SerializeField] private TMP_Text nameTypeModeTournament;
    [SerializeField] private TMP_Text timeAgo;


    [SerializeField] private TMP_Text statusTitle;
    [SerializeField] private GameObject battle;
    [SerializeField] private GameObject user2;
    [SerializeField] private TMP_Text txtScore1;
    [SerializeField] private TMP_Text txtYouName;
    [SerializeField] private TMP_Text txtScore2;
    [SerializeField] private TMP_Text txtOtherName2;

    [SerializeField] private Image feeImage;
    [SerializeField] private TMP_Text txtFeeImg;

    [SerializeField] private Button bgBtnClaim;
    [SerializeField] private TMP_Text txtBtnClaim;
    [SerializeField] private List<Sprite> listBgBtnClaim;

    [SerializeField] private Button btnItemHistory;

    private bool _isShowHistoryInProgress = true;
    private HcPlayHistoryProto _data;
    private Action _callback;


    public void ShowView(HcPlayHistoryProto data, bool isShowHistoryInProgress, Action callback)
    {
        SetDefault();
        _data = data;
        _callback = callback;
        _isShowHistoryInProgress = isShowHistoryInProgress;
        if (_data == null) return;
        txtNameTournament.text = _data.MiniGameName;
        iconMiniGame.sprite = ResourceManager.Instance.GetIconGame((GameType)_data.MiniGameId);
        nameTypeModeTournament.text = ResourceManager.Instance.GetNameTypeModeTournament(_data.ModeGameId);
        var tmp = (_data.TimeNowServer - _data.TimeStart).ToTimeSpan();
        timeAgo.text = tmp.Days > 0
            ? $"{tmp.Days,0}d ago"
            : $"{(tmp.Hours > 0 ? $"{tmp.Hours}h" : "")}{(tmp.Minutes > 0 ? $"{tmp.Minutes}m" : "1m")} ago";
        feeImage.sprite = ResourceManager.Instance.GetIconFeeMoney((MoneyType)_data.Fee.FeeType);
        txtFeeImg.text = StringUtils.FormatMoneyK(_data.Fee.Fee_);

        switch (_data.ModeGameId)
        {
            case 1: //HEAD TO HEAD
            case 2: //KNOCK OUT
            case 3: //ROUND ROBIN
                ShowDoubleScore();
                break;
            case 4: //ONE TO MANY
                ShowSingleScore();
                break;
            default:
                ShowSingleScore();
                break;
        }

        ShowButtonClaimAndTitle();
    }

    //Khi vào đây thì đã đảm bảo cho vệc có đối th hay chưa từ việc inprocess hay...nếu inprogress thì nếu có đối thủ thì sẽ hiện playnngnow
    private void ShowDoubleScore()
    {
        var infoUser1 = _data.User.ListHcPlayHistoryUserProto_.Count > 0
            ? _data.User.ListHcPlayHistoryUserProto_[0]
            : null;
        var infoUser2 = _data.User.ListHcPlayHistoryUserProto_.Count > 1
            ? _data.User.ListHcPlayHistoryUserProto_[1]
            : null;
        battle.gameObject.SetActive(true);
        user2.gameObject.SetActive(true);
        if (infoUser1 != null)
        {
            txtScore1.text = infoUser1.Score.ToString();
            txtYouName.text = infoUser1.UserName;
        }

        if (infoUser2 != null)
        {
            txtScore2.text = _isShowHistoryInProgress ? "Playing Now" : infoUser2.Score.ToString();
            txtOtherName2.text = infoUser2.UserName;
        }
        else
        {
            txtOtherName2.text = "Opponent Score";
            txtScore2.text = "__";
        }
    }

    private void ShowSingleScore()
    {
        var infoUser1 = _data.User.ListHcPlayHistoryUserProto_.Count > 0
            ? _data.User.ListHcPlayHistoryUserProto_[0]
            : null;
        if (infoUser1 != null)
        {
            txtScore1.text = infoUser1.Score.ToString();
            txtYouName.text = infoUser1.UserName;
        }
    }

    private void SetDefault()
    {
        battle.gameObject.SetActive(false);
        user2.gameObject.SetActive(false);
    }

    private void ShowButtonClaimAndTitle()
    {
        var tmpDateTime = _data.TimeRefund.ToDateTime();
        bgBtnClaim.onClick.RemoveAllListeners();
        btnItemHistory.onClick.RemoveAllListeners();
        switch ((GameModeType)_data.ModeGameId)
        {
            case GameModeType.HeadToHead:
                if (_isShowHistoryInProgress)
                {
                    var span = tmpDateTime - DateTime.UtcNow;
                    if (_data.User.ListHcPlayHistoryUserProto_.Count > 1)
                    {
                        statusTitle.text = "Waiting for result";
                        txtBtnClaim.text = $"__";
                        bgBtnClaim.GetComponent<Image>().sprite = listBgBtnClaim[0];
                    }
                    else
                    {
                        statusTitle.text = "Searching for Opponent";
                        txtBtnClaim.text = span.Days > 0
                            ? $"Refund in {span.Days:0}d"
                            : $"Refund in {span.Hours:0}h{span.Minutes:0}m";
                        bgBtnClaim.GetComponent<Image>().sprite = listBgBtnClaim[0];
                    }

                    btnItemHistory.onClick.AddListener(ShowDetailHistoryScreen);
                }
                else //SẼ BỌC 1 CSAI ĐÃ REFUNT Ở ĐÂY để hiển thị cho refund //BỌC NGOÀI CÙNG//Cho trường hợp ở trong thwafng đã refund
                {
                    //tức là đã nhận refund kéo sang completed
                    if (_data.IsRefund)
                    {
                        if (_data.ReceiveRefund)
                        {
                            statusTitle.text = "Not found Opponent";
                            txtBtnClaim.text = "Refund";
                            bgBtnClaim.GetComponent<Image>().sprite = listBgBtnClaim[0];
                        }
                        else
                        {
                            bgBtnClaim.onClick.AddListener(RefundFee);
                            txtBtnClaim.text = "Refund";
                            bgBtnClaim.GetComponent<Image>().sprite = listBgBtnClaim[1];
                            statusTitle.text = "Not found Opponent";
                        }
                    }
                    else
                    {
                        statusTitle.text = _data.Result == 2 ? "You Won" : "You Lose";

                        //dựa vào list reward có phần tử thì mới cho claim còn k thì k thể
                        if (_data.Reward.Reward.Count > 0)
                        {
                            txtBtnClaim.text = "Claim";
                            if (_data.IsClaim == 1) //Đã claim
                            {
                                bgBtnClaim.GetComponent<Image>().sprite = listBgBtnClaim[0];
                            }
                            else
                            {
                                bgBtnClaim.GetComponent<Image>().sprite = listBgBtnClaim[1];
                                bgBtnClaim.onClick.AddListener(ClaimReward);
                            }
                        }
                        //NẾU KHÔNG THÌ SẼ VÀO ĐÂY VÌ K CÓ QUÀ THÌ COI LÀ TRẬN K THỂ NHẬN CHƯA PHẢI TRẬN CUỐI HOẶC THUA THÌ MỚI HIỆN TRYAGAIN
                        else
                        {
                            //TỨC LÀ THẮNG VÀ CHƯA PHẢI TRẬN CUỐI
                            if (_data.Result == 2)
                            {
                                txtBtnClaim.text = "__";
                                bgBtnClaim.GetComponent<Image>().sprite = listBgBtnClaim[0];
                            }
                            else //TỨC LÀ THUA
                            {
                                if (_data.IsEventTime)
                                {
                                    txtBtnClaim.text = "Try Again";
                                    bgBtnClaim.GetComponent<Image>().sprite = listBgBtnClaim[1];
                                    bgBtnClaim.onClick.AddListener(TryAgain);
                                }
                                else
                                {
                                    txtBtnClaim.text = "__";
                                    bgBtnClaim.GetComponent<Image>().sprite = listBgBtnClaim[0];
                                }
                            }
                        }
                    }

                    btnItemHistory.onClick.AddListener(ShowDetailHistoryScreen);
                }

                break;


            case GameModeType.KnockOutRoundTour:


                if (_isShowHistoryInProgress)
                {
                    var span = tmpDateTime - DateTime.UtcNow;
                    if (_data.User.ListHcPlayHistoryUserProto_.Count > 1)
                    {
                        statusTitle.text = "Waiting for result";
                        txtBtnClaim.text = $"__";
                        bgBtnClaim.GetComponent<Image>().sprite = listBgBtnClaim[0];
                        bgBtnClaim.onClick.AddListener(ShowDetailHistoryScreen);
                    }
                    else
                    {
                        statusTitle.text = "Searching for Opponent";
                        txtBtnClaim.text = span.Days > 0
                            ? $"Refund in {span.Days:0}d"
                            : $"Refund in {span.Hours:0}h{span.Minutes:0}m";
                        bgBtnClaim.GetComponent<Image>().sprite = listBgBtnClaim[0];
                    }

                    //NẾU K REFUND MÀ INPROGRESS THÌ CHO ẤN HIỆN LÊN
                    btnItemHistory.onClick.AddListener(ShowTournament);
                }
                else //SẼ BỌC 1 CSAI ĐÃ REFUNT Ở ĐÂY để hiển thị cho refund //BỌC NGOÀI CÙNG//Cho trường hợp ở trong thwafng đã refund
                {
                    //tức là đã nhận refund kéo sang completed
                    //không hiện thị bấm vào xem gì cả
                    if (_data.IsRefund)
                    {
                        if (_data.ReceiveRefund)
                        {
                            statusTitle.text = "Not found Opponent";
                            txtBtnClaim.text = "Refund";
                            bgBtnClaim.GetComponent<Image>().sprite = listBgBtnClaim[0];
                        }
                        else
                        {
                            bgBtnClaim.onClick.AddListener(RefundFee);
                            txtBtnClaim.text = "Refund";
                            bgBtnClaim.GetComponent<Image>().sprite = listBgBtnClaim[1];
                            statusTitle.text = "Not found Opponent";
                        }
                    }
                    else
                    {
                        statusTitle.text = $"You finished in Top {_data.TournamentTop}";
                        //dựa vào list reward có phần tử thì mới cho claim còn k thì k thể
                        if (_data.Reward.Reward.Count > 0)
                        {
                            txtBtnClaim.text = "Claim";
                            if (_data.IsClaim == 1) //Đã claim
                            {
                                bgBtnClaim.GetComponent<Image>().sprite = listBgBtnClaim[0];
                            }
                            else
                            {
                                bgBtnClaim.GetComponent<Image>().sprite = listBgBtnClaim[1];
                                bgBtnClaim.onClick.AddListener(ClaimReward);
                            }
                        }
                        //NẾU KHÔNG THÌ SẼ VÀO ĐÂY VÌ K CÓ QUÀ THÌ COI LÀ TRẬN K THỂ NHẬN CHƯA PHẢI TRẬN CUỐI HOẶC THUA THÌ MỚI HIỆN TRYAGAIN
                        else
                        {
                            //TRONG KNOCK OUT KHÔNG CÓ TRYAGRAIN
                            txtBtnClaim.text = "__";
                            bgBtnClaim.GetComponent<Image>().sprite = listBgBtnClaim[1];
                            bgBtnClaim.onClick.AddListener(ShowDetailHistoryScreen);
                        }

                        btnItemHistory.onClick.AddListener(ShowTournament);
                    }
                }

                break;


            case GameModeType.OneVsMany:

                if (_isShowHistoryInProgress)
                {
                    var span = tmpDateTime - DateTime.UtcNow;
                    if (_data.User.ListHcPlayHistoryUserProto_.Count > 1)
                    {
                        statusTitle.text = "Waiting for result";
                        txtBtnClaim.text = $"__";
                        bgBtnClaim.GetComponent<Image>().sprite = listBgBtnClaim[0];
                    }
                    else
                    {
                        statusTitle.text = "Searching for Opponent";
                        txtBtnClaim.text = span.Days > 0
                            ? $"Refund in {span.Days:0}d"
                            : $"Refund in {span.Hours:0}h{span.Minutes:0}m";
                        bgBtnClaim.GetComponent<Image>().sprite = listBgBtnClaim[0];
                    }

                    btnItemHistory.onClick.AddListener(ShowTournamentOneToMany);
                }
                else //SẼ BỌC 1 CSAI ĐÃ REFUNT Ở ĐÂY để hiển thị cho refund //BỌC NGOÀI CÙNG//Cho trường hợp ở trong thwafng đã refund
                {
                    //tức là đã nhận refund kéo sang completed
                    if (_data.IsRefund)
                    {
                        if (_data.ReceiveRefund)
                        {
                            statusTitle.text = "Not found Opponent";
                            txtBtnClaim.text = "Refund";
                            bgBtnClaim.GetComponent<Image>().sprite = listBgBtnClaim[0];
                        }
                        else
                        {
                            bgBtnClaim.onClick.AddListener(RefundFee);
                            txtBtnClaim.text = "Refund";
                            bgBtnClaim.GetComponent<Image>().sprite = listBgBtnClaim[1];
                            statusTitle.text = "Not found Opponent";
                        }
                    }
                    else
                    {
                        statusTitle.text = $"You finished in Top {_data.TournamentTop}";
                        //dựa vào list reward có phần tử thì mới cho claim còn k thì k thể
                        if (_data.Reward.Reward.Count > 0)
                        {
                            txtBtnClaim.text = "Claim";
                            if (_data.IsClaim == 1) //Đã claim
                            {
                                bgBtnClaim.GetComponent<Image>().sprite = listBgBtnClaim[0];
                            }
                            else
                            {
                                bgBtnClaim.GetComponent<Image>().sprite = listBgBtnClaim[1];
                                bgBtnClaim.onClick.AddListener(ClaimReward);
                            }
                        }
                        //NẾU KHÔNG THÌ SẼ VÀO ĐÂY VÌ K CÓ QUÀ THÌ COI LÀ TRẬN K THỂ NHẬN CHƯA PHẢI TRẬN CUỐI HOẶC THUA THÌ MỚI HIỆN TRYAGAIN
                        else
                        {
                            //VỚI LOẠI NÀY THÌ SẼ HIỆN LUÔN K PHẢI SOLO NÊN K CÓ THẮNG THUA
                            if (_data.IsEventTime)
                            {
                                txtBtnClaim.text = "Try Again";
                                bgBtnClaim.GetComponent<Image>().sprite = listBgBtnClaim[1];
                                bgBtnClaim.onClick.AddListener(TryAgain);
                            }
                            else
                            {
                                txtBtnClaim.text = "__";
                                bgBtnClaim.GetComponent<Image>().sprite = listBgBtnClaim[0];
                            }
                        }

                        btnItemHistory.onClick.AddListener(ShowTournamentOneToMany);
                    }
                }

                break;
            default:
                break;
        }
    }

    private void ShowDetailHistoryScreen()
    {
        ScreenManagerHC.Instance.ShowDetailHistoryScreen(_data, _isShowHistoryInProgress);
    }

    private Coroutine _sendToRefund;

    private void RefundFee()
    {
        var url = GameConfig.API_URL + GameConfig.API_TAIL_GETREFUND;
        ActivityHistoryRefundRequest request = new ActivityHistoryRefundRequest
        {
            userCodeId = HCAppController.Instance.userInfo.UserCodeId,
            historyId = _data.IdHistory
        };
        _sendToRefund = StartCoroutine(APIUtils.RequestWebApiPost(url, JsonUtility.ToJson(request),
            HCAppController.Instance.GetTokenLogin(), RefundSuccess, RefundError));
    }

    private void RefundError(string error)
    {
        Toast.Show(error);
    }

    private void RefundSuccess(string dataResponse)
    {
        JObject data = JObject.Parse(dataResponse);
        var activityHistoryRefundResponse = new ActivityHistoryRefundResponse(data);
        if (activityHistoryRefundResponse.isSuccess)
        {
            if (activityHistoryRefundResponse != null)
            {
                if (activityHistoryRefundResponse.refund != null)
                {
                    var parser = new MessageParser<ListReward>(() => new ListReward());
                    ListReward listReward = parser.ParseFrom(activityHistoryRefundResponse.refund);
                    CoinFlyAnimation.Instance.SpawnListRewardClaim(listReward, new Vector2(0, 0), null);
                }
            }

            _callback?.Invoke();
        }
        else
        {
            Toast.Show("Có lỗi xảy ra gì đó trong quá trình nhận!");
        }
    }


    private Coroutine _sendToClaim;

    private void ClaimReward()
    {
        if (_sendToClaim != null)
        {
            StopCoroutine(_sendToClaim);
        }

        var url = GameConfig.API_URL + GameConfig.API_TAIL_CLAIMHISTORY;
        ActivityHistoryClaimRequest request = new ActivityHistoryClaimRequest
        {
            id = _data.IdHistory,
            hcGroupRoomId = _data.HcGroupRoomId
        };

        _sendToClaim = StartCoroutine(APIUtils.RequestWebApiPost(url, JsonUtility.ToJson(request),
            HCAppController.Instance.GetTokenLogin(), ClaimSuccess, ClaimError));
    }

    private void ClaimSuccess(string dataResponse)
    {
        JObject data = JObject.Parse(dataResponse);
        var tmpActivityHistoryClaimResponse = new ActivityHistoryClaimResponse(data);
        if (tmpActivityHistoryClaimResponse.isSuccess)
        {
            if (tmpActivityHistoryClaimResponse.listReward != null)
            {
                var parser = new MessageParser<ListReward>(() => new ListReward());
                ListReward listReward = parser.ParseFrom(tmpActivityHistoryClaimResponse.listReward);
                CoinFlyAnimation.Instance.SpawnListRewardClaim(listReward, new Vector2(0, 0), null);
            }

            _callback?.Invoke();
        }
        else
        {
            Toast.Show("Có lỗi xảy ra gì đó trong quá trình nhận!");
        }
    }

    private void ClaimError(string error)
    {
        Toast.Show(error);
    }

    private Coroutine _sendToTakeDetailMiniGameProto;

    [Button]
    private void TryAgain()
    {
        var url = GameConfig.API_URL + GameConfig.API_TAIL_GETDETAILMINIGAMEPROTO + "id=" + $"{_data.MiniGameEventId}";
        if (_sendToTakeDetailMiniGameProto != null)
        {
            StopCoroutine(_sendToTakeDetailMiniGameProto);
        }

        _sendToTakeDetailMiniGameProto =
            StartCoroutine(APIUtils.RequestWebApiGetByte(url, SuccessDetailMiniGameProto, Error));
    }

    private void SuccessDetailMiniGameProto(byte[] data)
    {
        var parser = new MessageParser<DetailMiniGameProto>(() => new DetailMiniGameProto());
        DetailMiniGameProto tmpDetailMiniGameProto = parser.ParseFrom(data);
        if (tmpDetailMiniGameProto != null)
        {
            HcGames.CCData _ccData = new HcGames.CCData();
            _ccData.MiniGameEventId = (ulong)_data.MiniGameEventId;
            _ccData.Token = HCAppController.Instance.GetTokenLogin();
            GameType gameType = (GameType)_data.MiniGameId;
            long miniGameEventId = _data.MiniGameEventId;
            int modeType = _data.ModeGameId;
            int numberPlayer = _data.NumberPlayer;
            HCAppController.Instance.SetCurrentDetailMiniGame(tmpDetailMiniGameProto);
            HCAppController.Instance.ConnectGameAndPlay(gameType, miniGameEventId, modeType, numberPlayer, _ccData,
                HCAppController.Instance.currentDetailMiniGameProto.PrizePool);
        }
    }

    private void Error(string obj)
    {
        Toast.Show(obj);
    }


    private void ShowTournament()
    {
        ScreenManagerHC.Instance.ShowTournament(_data.HcGroupRoomId);
    }

    private void ShowTournamentOneToMany()
    {
        ScreenManagerHC.Instance.ShowResultOneVsMany(_data.HcGroupRoomId);
    }

    private void OnDisable()
    {
        if (_sendToClaim != null)
        {
            StopCoroutine(_sendToClaim);
        }

        if (_sendToRefund != null)
        {
            StopCoroutine(_sendToRefund);
        }
    }

    #endregion
}

public struct ActivityHistoryClaimRequest
{
    public int id;
    public string hcGroupRoomId;
}

public class ActivityHistoryClaimResponse
{
    public string message;
    public bool isSuccess;
    public byte[] listReward;

    public ActivityHistoryClaimResponse(JObject data)
    {
        message = (string)data["message"];
        isSuccess = (bool)data["isSuccess"];
        listReward = (byte[])data["listReward"];
    }
}

public class ActivityHistoryRefundResponse
{
    public string message;
    public bool isSuccess;
    public byte[] refund;

    public ActivityHistoryRefundResponse(JObject data)
    {
        message = (string)data["message"];
        isSuccess = (bool)data["isSuccess"];
        refund = (byte[])data["refund"];
    }
}

public struct ActivityHistoryRefundRequest
{
    public string userCodeId;
    public long historyId;
}