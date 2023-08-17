using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using MiniGame.MatchThree.Scripts.Network;

[System.Serializable]
public class GameOverPuzzleData
{
    public int gameOverType; //0 -> time up(het gio), 1 -> quit gameover, 2-> success(choi het van)  4 final ROUND1
    public int round1Final;
    public int round2Final;
    public int round1Score;
    public int round2Score;
    public int moveBonus;
    public int allClearBonus;
    public int finalScore;
}

public class GameOverPuzzle : GameOverPopupBase
{
    public Sprite[] sprGameOverType;
    public Image imgStatusGameOver;
    public GameObject subStatusGameOver;

    public TMP_Text round1Score;
    public TMP_Text round2Score;
    public TMP_Text moveBonus;
    public TMP_Text round1Final;
    public TMP_Text round2Final;
    public TMP_Text allClearBonus;
    public TMP_Text finalScore;


    public static GameOverPuzzle instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("MULTI INS" + name);
            Destroy(gameObject);
        }
    }
    public void Show(GameOverPuzzleData data, Action actionSubmit)
    {
        Debug.Log("Puzzle game over show");
        base.Show();
        btSubmit.onClick.AddListener(() => { actionSubmit?.Invoke(); });
        if (data != null) LoadData(data);
    }

    public void LoadData(GameOverPuzzleData data)
    {
        SetDefault();
        switch (data.gameOverType)
        {
            case 0: //0 -> time up(het gio)
                if (sprGameOverType.Length > 0) imgStatusGameOver.sprite = sprGameOverType[0];
                round1Final.gameObject.SetActive(true);
                HCHelper.NumberIncreasesEff(round1Final, 0, data.round1Final, 1, false);
                round2Final.gameObject.SetActive(true);
                HCHelper.NumberIncreasesEff(round2Final, 0, data.round2Final, 1, false);
                break;
            case 1: //quit gameover
                if (sprGameOverType.Length > 1) imgStatusGameOver.sprite = sprGameOverType[1];
                round1Final.gameObject.SetActive(true);
                HCHelper.NumberIncreasesEff(round1Final, 0, data.round1Final, 1, false);
                round2Final.gameObject.SetActive(true);
                HCHelper.NumberIncreasesEff(round2Final, 0, data.round2Final, 1, false);
                break;
            case 2: //success(choi het van)
                if (sprGameOverType.Length > 2) imgStatusGameOver.sprite = sprGameOverType[2];
                round1Score.gameObject.SetActive(true);
                HCHelper.NumberIncreasesEff(round1Score, 0, data.round1Score, 1, false);
                moveBonus.gameObject.SetActive(true);
                HCHelper.NumberIncreasesEff(moveBonus, 0, data.moveBonus, 1, false);
                round1Final.gameObject.SetActive(true);
                HCHelper.NumberIncreasesEff(round1Final, 0, data.round1Final, 1, false);
                allClearBonus.gameObject.SetActive(true);
                HCHelper.NumberIncreasesEff(allClearBonus, 0, data.allClearBonus, 1, false);
                break;
            case 3: //final Round1
                if (sprGameOverType.Length > 2) imgStatusGameOver.sprite = sprGameOverType[3];
                gameObject.transform.GetChild(0).gameObject.SetActive(true);
                subStatusGameOver.gameObject.SetActive(true);
                round1Score.gameObject.SetActive(true);
                HCHelper.NumberIncreasesEff(round1Score, 0, data.round1Score, 1, false);
                moveBonus.gameObject.SetActive(true);
                HCHelper.NumberIncreasesEff(moveBonus, 0, data.moveBonus, 1, false);
                break;
            default:
                break;
        }

        imgStatusGameOver.SetNativeSize();
        HCHelper.NumberIncreasesEff(finalScore, 0, data.finalScore, 1, false);
    }

    public GameOverPuzzleData _data;
    [Button]
    public void LoadDataFake()
    {
        LoadData(_data);
    }


    public void OnClickRound2()
    {
        NetWorkBoard.instance.SendMessageContinueRound2();
        Match3_BotBar.Instance.SetTextRound2();
        transform.GetChild(0).gameObject.SetActive(false);
    }










    private void SetDefault()
    {
        round1Score.text = "0";
        round2Score.text = "0";
        moveBonus.text = "0";
        round1Final.text = "0";
        round2Final.text = "0";
        round2Final.text = "0";
        allClearBonus.text = "0";
        finalScore.text = "0";
        round1Score.gameObject.SetActive(false);
        round2Score.gameObject.SetActive(false);
        moveBonus.gameObject.SetActive(false);
        round1Final.gameObject.SetActive(false);
        round2Final.gameObject.SetActive(false);
        round2Final.gameObject.SetActive(false);
        allClearBonus.gameObject.SetActive(false);
        subStatusGameOver.gameObject.SetActive(false);
    }

    public override void Submit()
    {
        base.Submit();
    }
}

