using AssemblyCSharp.GameNetwork;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMode : PageBase
{
    public Button btClose;
    public ModeInfo modeInfo;
    public GameModeUI gameModeUI;
    public GameType gameType;

    private void Awake()
    {
        btClose.onClick.AddListener(Hide);
    }

    #region TESTTTTTTTT

    //GAMENAYCHICHOMUCDICHTEST MOI SHOW THẰNG CHỌN MODE CÒN K THÌ SẼ DỰA VÀO VIỆC CÁC ITEMGAMEMODE HÌ MỚI VÀO

    [Button]
    public void TestShow()
    {
        Show(GameType.Billard);
    }

    public void Show(GameType _gameType)
    {
        this.gameType = _gameType;
        var data = HCAppController.Instance.GetListDetailMiniGameProto(_gameType);
        StartCoroutine(gameModeUI.Show(_gameType, data, true));
        //modeInfo.Hide();
        FadeIn();
    }

    #endregion


    public void Hide()
    {
        FadeOut();
        this.gameType = GameType.None;
        //modeInfo.Hide();
        //GameModeUI.Instance.O.Hide();
    }

    public void ShowModeInfo(DetailMiniGameProto data)
    {
        ModeInfo.OpenViewWithCallBack(info => info.ShowView(data));
        // gameModeUI.FadeOut();
    }
    //public void ShowGameModeUI()
    //{
    //    gameModeUI.Show(this.gameType,true);
    //}
}