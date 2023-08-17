using System;
using System.Collections.Generic;
using Google.Protobuf;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Google.Protobuf.WellKnownTypes;

public class DetailHistoryScreen : UIView<DetailHistoryScreen>
{
    [SerializeField] private GameObject effectTitle;
    [SerializeField] private TMP_Text txtTitleBattle;
    [SerializeField] private Image titleBattle;
    [SerializeField] private List<Sprite> listSpriteTitleBattle;
    [SerializeField] private Image bgInfoBattle;
    [SerializeField] private List<Sprite> listBgInfoBattle;
    [SerializeField] private Image bgContentInfoBattle;
    [SerializeField] private List<Sprite> listContentBattle;

    [SerializeField] private TMP_Text txtNameGame;
    [SerializeField] private Image iconGame;

    [SerializeField] private TMP_Text txtNameTournament;
    [SerializeField] private TMP_Text txtTime;
    [SerializeField] private TMP_Text txtEntryFee;
    [SerializeField] private Image iconEntryFee;

    [SerializeField] private TMP_Text txtStatusUser1;
    [SerializeField] private Image iconUser1;
    [SerializeField] private TMP_Text txtNameUser1;
    [SerializeField] private TMP_Text txtScoreUser1;
    [SerializeField] private GameObject bgEffectUser1;
    [SerializeField] private Image imgBorderUser1;

    [SerializeField] private TMP_Text txtStatusUser2;
    [SerializeField] private Image iconUser2;
    [SerializeField] private TMP_Text txtNameUser2;
    [SerializeField] private TMP_Text txtScoreUser2;
    [SerializeField] private GameObject scoreUser2;
    [SerializeField] private TMP_Text txtStatusScore;
    [SerializeField] private GameObject bgEffectUser2;
    [SerializeField] private Image imgBorderUser2;

    [SerializeField] private List<Sprite> listSpriteBorder;

    [SerializeField] private ItemReward prefabItemReward;
    [SerializeField] private Transform holderItemReward;

    [SerializeField] private Button btnClaim;
    [SerializeField] private List<Sprite> listSpriteBgBtn;

    private List<ItemReward> _listItemReward = new List<ItemReward>();
    private HcPlayHistoryProto _data;
    private bool _isShowHistoryInProgress = true;
    private string _urlAvatar;

    private Coroutine _sendToClaim;


    public void ShowView(HcPlayHistoryProto data, bool isShowHistoryInProgress)
    {
        SetDefault();
        _isShowHistoryInProgress = isShowHistoryInProgress;
        _data = data;
        if (_isShowHistoryInProgress)
        {
            txtTitleBattle.text = "RESULT PENDING";
            titleBattle.sprite = listSpriteTitleBattle[2];
        }
        else
        {
            txtTitleBattle.text = _data.Result == 2 ? "BATTLE WIN" : "BATTLE LOSE";
            effectTitle.gameObject.SetActive(_data.Result == 2);
            titleBattle.sprite = _data.Result == 2 ? listSpriteTitleBattle[0] : listSpriteTitleBattle[1];
        }

        bgInfoBattle.sprite = _data.Result == 2 ? listBgInfoBattle[0] : listBgInfoBattle[1];
        bgContentInfoBattle.sprite = _data.Result == 2 ? listContentBattle[0] : listContentBattle[1];
        txtNameGame.text = ResourceManager.Instance.GetNameGame((GameType)_data.MiniGameId);
        iconGame.sprite = ResourceManager.Instance.GetIconGame((GameType)_data.MiniGameId);
        txtNameTournament.text = _data.MiniGameName;
        txtTime.text = _data.TimeStart.ToDateTime().ToLocalTime().ToString();
        txtEntryFee.text = StringUtils.FormatMoneyK(_data.Fee.Fee_);
        iconEntryFee.sprite = ResourceManager.Instance.GetIconFeeMoney((MoneyType)_data.Fee.FeeType);
        HcPlayHistoryUserProto userInfo1 = null;
        HcPlayHistoryUserProto userInfo2 = null;
        if (_data.User.ListHcPlayHistoryUserProto_.Count > 0)
        {
            userInfo1 = _data.User.ListHcPlayHistoryUserProto_[0];
        }

        if (_data.User.ListHcPlayHistoryUserProto_.Count > 1)
        {
            userInfo2 = _data.User.ListHcPlayHistoryUserProto_[1];
        }

        iconUser1.sprite = HCAppController.Instance.myAvatar;
        txtNameUser1.text = HCAppController.Instance.userInfo.UserName;
        bgEffectUser1.gameObject.SetActive(_data.Result == 2);
        imgBorderUser1.GetComponent<Image>().sprite = _data.Result == 2 ? listSpriteBorder[0] : listSpriteBorder[1];
        if (userInfo1 != null)
        {
            txtScoreUser1.text = userInfo1.Score.ToString();
        }

        if (_isShowHistoryInProgress)
        {
            txtStatusUser1.gameObject.SetActive(false);
            txtStatusUser2.gameObject.SetActive(false);
            if (userInfo2 != null)
            {
                txtStatusScore.gameObject.Show();
                txtStatusScore.text = "Playing Now";
                txtNameUser2.text = userInfo2.UserName;
                if (userInfo2.AvatarUser != _urlAvatar)
                {
                    _urlAvatar = userInfo2.AvatarUser;
                    StartCoroutine(HCHelper.LoadAvatar(_urlAvatar, iconUser2));
                }
            }
            else //BẰNG NULL THÌ SẼ SHOW KIỂU SEACHOPPONENT
            {
                txtStatusScore.gameObject.Show();
                txtStatusScore.text = "Searching Opponent";
                iconUser2.sprite = ResourceManager.Instance.GetAvatarDefault();
                txtNameUser2.text = "__";
            }
        }
        else //SHOW FULL FUNC CÓ THỂ CLAIM
        {
            txtStatusUser1.gameObject.SetActive(true);
            txtStatusUser1.text = _data.Result == 2 ? "WIN" : "LOSE";
            txtStatusUser2.gameObject.SetActive(true);
            txtStatusUser2.text = _data.Result == 1 ? "WIN" : "LOSE";
            bgEffectUser2.SetActive(_data.Result == 1);
            if (userInfo2 != null)
            {
                scoreUser2.Show();
                if (userInfo2.AvatarUser != _urlAvatar)
                {
                    _urlAvatar = userInfo2.AvatarUser;
                    StartCoroutine(HCHelper.LoadAvatar(_urlAvatar, iconUser2));
                }

                txtNameUser2.text = userInfo2.UserName;
                txtScoreUser2.text = userInfo2.Score.ToString();
                imgBorderUser2.GetComponent<Image>().sprite =
                    _data.Result == 1 ? listSpriteBorder[0] : listSpriteBorder[1];
            }

            if (_data.Reward.Reward.Count > 0)
            {
                for (int i = 0; i < _data.Reward.Reward.Count; i++)
                {
                    var tmpReward = BonusPool.Spawn(prefabItemReward, holderItemReward);
                    tmpReward.ShowView(_data.Reward.Reward[i]);
                }
            }
        }

        ShowButtonClaim();
    }

    private void ShowButtonClaim()
    {
        /*
         if (_data.Reward.Reward.Count > 0)
            {
                if (_data.IsClaim == 0)
                {
                    btnClaim.GetComponent<Image>().sprite = listSpriteBgBtn[0];
                }
                else
                {
                    btnClaim.GetComponent<Image>().sprite = listSpriteBgBtn[1];
                }
            }
            else
            {
                btnClaim.GetComponent<Image>().sprite = listSpriteBgBtn[1];
            } 
         */
        if (_isShowHistoryInProgress)
        {
            btnClaim.GetComponent<Image>().sprite = listSpriteBgBtn[1];
        }
        else //PHẢI BỌC NGOÀI CÁI CÓ THỂ NHẬN HAY K NỮA
        {
            if (_data.Reward.Reward.Count > 0)
            {
                if (_data.IsClaim == 0)
                {
                    btnClaim.GetComponent<Image>().sprite = listSpriteBgBtn[0];
                    btnClaim.onClick.AddListener(Claim);
                }
                else
                {
                    btnClaim.GetComponent<Image>().sprite = listSpriteBgBtn[1];
                }
            }
            else
            {
                btnClaim.GetComponent<Image>().sprite = listSpriteBgBtn[1];
            }
        }
    }

    private void DisableBtnClaim()
    {
        btnClaim.GetComponent<Image>().sprite = listSpriteBgBtn[1];
        btnClaim.onClick.RemoveAllListeners();
    }

    private void Claim()
    {
        //Sau khi nhận xong thì disable nút đi:::
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
        }
        else
        {
            Toast.Show("Có lỗi xảy ra gì đó trong quá trình nhận!");
        }

        DisableBtnClaim();
    }

    private void ClaimError(string error)
    {
        Toast.Show(error);
    }

    private void SetDefault()
    {
        btnClaim.onClick.RemoveAllListeners();
        effectTitle.Hide();
        _listItemReward.Clear();
        scoreUser2.Hide();
        txtStatusScore.gameObject.Hide();
        bgEffectUser1.Hide();
        bgEffectUser2.Hide();
        for (int i = holderItemReward.childCount - 1; i >= 0; i--)
        {
            BonusPool.DeSpawn(holderItemReward.GetChild(i));
        }
    }
}