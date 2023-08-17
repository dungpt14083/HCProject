using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class GameOverProcessPopup : UIPopupView<GameOverProcessPopup>
{
    //DO SEVER TRẢ VỀ GỘP CHUNG GAMEOVER VÀO ENDUSER NÊN KHI ENDROOOM VẪN CÓ ENDUSER
    private EndUserWebsocketProto _endUserWebsocketProto;
    private EndRoomWebsocketProto _endRoomWebsocketProto;

    private Coroutine _waitEndUser;
    private Coroutine _waitEndRoomShowButton;
    private Coroutine _autoSubmit;

    private bool _isShowGameOver = false;
    private bool _isShowEndRoom = false;

    public GameOverBingo gameOverBingo;
    public GameOverSoliraite gameOverSoliraite;
    public GameOverBubblesShot gameOverBubblesShot;
    public GameOverPuzzle gameOverPuzzle;
    public GameOverBillard gameOverBillard;

    private void OnEnable()
    {
        //
    }

    private void OnDisable()
    {
        SetDefaultData();
    }

    #region TODOENDROOM

    public void ShowViewEndRoom(EndRoomWebsocketProto dataEndRoom)
    {
        _isShowEndRoom = true;
        _endRoomWebsocketProto = dataEndRoom;
        if (_waitEndUser != null) Executors.Instance.StopCoroutine(_waitEndUser);
        if (_endUserWebsocketProto == null)
            _waitEndUser = Executors.Instance.StartCoroutine(WaitEndUserDataAndShowEndRoom());
        if (!_isShowGameOver)
        {
            ShowView(true);
        }
    }

    private IEnumerator WaitEndUserDataAndShowEndRoom()
    {
        yield return new WaitUntil((() => _endUserWebsocketProto != null));
        CloseView();
        ShowView(true);
    }

    #endregion


    #region TODOENDUSER

    public void ShowViewEndUser(EndUserWebsocketProto dataEndUser)
    {
        //if(submitBtn != null)
        //    submitBtn.interactable = false;
        _isShowEndRoom = false;
        _endUserWebsocketProto = dataEndUser;
        ShowView(false);
    }

    #endregion


    #region ShowViewAndFucntionButton

    private void ShowView(bool isEndRoom)
    {
        gameOverBingo.gameObject.SetActive(false);
        gameOverSoliraite.gameObject.SetActive(false);
        gameOverBubblesShot.gameObject.SetActive(false);
        gameOverPuzzle.gameObject.SetActive(false);
        gameOverBillard.gameObject.SetActive(false);
        _isShowGameOver = true;
        if (_endUserWebsocketProto != null)
        {
            switch ((GameType)_endUserWebsocketProto.MiniGameId)
            {
                case GameType.Bingo:
                    Debug.Log("_endUserWebsocketProto.PointGameLogicBingo " + _endUserWebsocketProto.PointGameLogic);
                    GameOverBingoData dataBingo =
                        JsonUtility.FromJson<GameOverBingoData>(_endUserWebsocketProto.PointGameLogic);
                    gameOverBingo.Show(dataBingo, SubmitScore);
                    break;
                case GameType.Solitaire:
                    Debug.Log("_endUserWebsocketProto.PointGameLogicSolitaire " +
                              _endUserWebsocketProto.PointGameLogic);
                    GameOverSoliraiteAndBilliardData dataSoli =
                        JsonUtility.FromJson<GameOverSoliraiteAndBilliardData>(_endUserWebsocketProto.PointGameLogic);
                    gameOverSoliraite.Show(dataSoli, SubmitScore);
                    break;
                case GameType.Bubble:
                    Debug.Log("_endUserWebsocketProto.PointGameLogicBubble " + _endUserWebsocketProto.PointGameLogic);
                    GameOverBubblesShotData dataBubblesShot =
                        JsonUtility.FromJson<GameOverBubblesShotData>(_endUserWebsocketProto.PointGameLogic);
                    gameOverBubblesShot.Show(dataBubblesShot, SubmitScore);
                    break;
                case GameType.Puzzle:
                    Debug.Log("_endUserWebsocketProto.PointGameLogicPuzzle " + _endUserWebsocketProto.PointGameLogic);
                    GameOverPuzzleData dataPuzzle =
                        JsonUtility.FromJson<GameOverPuzzleData>(_endUserWebsocketProto.PointGameLogic);
                    gameOverPuzzle.Show(dataPuzzle, SubmitScore);
                    break;
                case GameType.Billard:
                    Debug.Log("_endUserWebsocketProto.PointGameLogicBillard " + _endUserWebsocketProto);
                    GameOverBilliardData dataBilliard =
                        JsonUtility.FromJson<GameOverBilliardData>(_endUserWebsocketProto.PointGameLogic);
                    gameOverBillard.Show(dataBilliard, SubmitScore);
                    break;
            }
        }

        if (_waitEndRoomShowButton != null) StopCoroutine(_waitEndRoomShowButton);
        if (!isEndRoom)
        {
            _waitEndRoomShowButton = StartCoroutine(ShowButtonSubmit());
        }
        else
        {
            SetFuncForButtonSubmit();
        }

        if (_autoSubmit != null) StopCoroutine(_autoSubmit);
        _autoSubmit = StartCoroutine(AutoSubmitScore());
    }


    private IEnumerator ShowButtonSubmit()
    {
        yield return new WaitForSeconds(3.0f);
        SetFuncForButtonSubmit();
    }

    private void SetFuncForButtonSubmit()
    {
    }

    private void ShowOverTest()
    {
        //FadeIn();
    }

    private void SubmitScore()
    {
        if (_autoSubmit != null)
        {
            StopCoroutine(_autoSubmit);
        }

        ShowViewResultOrRanking();
        CloseView();
    }

    private void ShowViewResultOrRanking()
    {
        if (_endUserWebsocketProto == null) return;
        var tmpMode = _endUserWebsocketProto.ModeType;
        if (_isShowEndRoom)
        {
            tmpMode = _endRoomWebsocketProto.ModeType;
        }

        switch (tmpMode)
        {
            case 4:
                SendToShowTournamentRanking();
                break;
            case 2:
                ShowResultNormal();
                break;
            case 3:
                ShowResultNormal();
                break;
            case 1:
                ShowResultNormal();
                break;
            default:
                //Log
                break;
        }
    }

    
    #region TODOSHOWTOURNAMENTRANKING TYPE1VSMANY

    private Coroutine _sendToServerToShowResult;

    private void SendToShowTournamentRanking()
    {
        if (_sendToServerToShowResult != null)
        {
            StopCoroutine(_sendToServerToShowResult);
        }

        var tmpgroupRoomId = _endUserWebsocketProto.GroupRoomId;
        if (_isShowEndRoom)
        {
            tmpgroupRoomId = _endRoomWebsocketProto.GroupRoomId;
        }

        string url = GameConfig.API_URL + GameConfig.API_TAIL_TOURNAMENTRANKING +
                     $"userId={HCAppController.Instance.userInfo.UserCodeId}" + "&" +
                     $"groupRoomId={tmpgroupRoomId}";
        _sendToServerToShowResult = StartCoroutine(APIUtils.RequestWebApiGetByte(url,
            ShowTournamentSameTopPlayerPopup, ShowTournamentSameTopPlayerError));
    }

    private byte[] _tmpData;

    private void ShowTournamentSameTopPlayerPopup(byte[] data)
    {
        CloseView();
        _tmpData = data;
        ScreenManagerHC.Instance.GoToScreenViewOnlyShowButton(CallBackLoadScene);
    }

    private void CallBackLoadScene()
    {
        Executors.Instance.StartCoroutine(CallBackShowTournament(_tmpData));
    }

    private IEnumerator CallBackShowTournament(byte[] data)
    {
        var parser = new MessageParser<ListUserTournament>(() => new ListUserTournament());
        ListUserTournament listUserTournament = parser.ParseFrom(data);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        ScreenManagerHC.Instance.ShowResultTopPlayer(listUserTournament);
    }

    private void ShowTournamentSameTopPlayerError(string error)
    {
        ScreenManagerHC.Instance.GotoHomeByDelay(error);
    }

    #endregion


    #region TODOSHOWRESULT

    private void ShowResultNormal()
    {
        CloseView();
        if (_isShowEndRoom)
        {
            HcPopupManager.Instance.ShowGameResultEndRoom(_endRoomWebsocketProto);
        }
        else
        {
            HcPopupManager.Instance.ShowGameResultEndUser(_endUserWebsocketProto);
        }
    }

    #endregion


    private IEnumerator AutoSubmitScore()
    {
        var tmpTimeLeft = 5.0f;
        while (true)
        {
            if (tmpTimeLeft <= 0)
            {
                SubmitScore();
                yield break;
            }

            yield return new WaitForSeconds(1.0f);
            tmpTimeLeft--;
        }
    }

    #endregion

    private void SetDefaultData()
    {
        _endUserWebsocketProto = null;
        _endRoomWebsocketProto = null;
        _isShowGameOver = false;
        _isShowEndRoom = false;
    }
}