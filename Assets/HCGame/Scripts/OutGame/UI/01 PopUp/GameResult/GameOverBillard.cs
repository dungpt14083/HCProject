using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameOverBilliardData
{
    public int gameOverType;//0 -> time up(het gio), 1 -> quit, 2-> success(choi het van)
    public int score;
    public int timeBonus;
    public int finalScore;
    public int result;
    public int modeSyncAndUnSync;//1->bat dong bo, 2-> dong bo
}
public class GameOverBillard : GameOverPopupBase
{
    public Sprite[] sprGameOverType;
    public Image imgStatusGameOver;
    public Sprite sprYouWin;
    public Sprite sprYouLose;
    public Sprite[] sprBgGameOver;
    public Image bgGameOver;

    public TMP_Text score;
    public TMP_Text timeBonus;
    public TMP_Text finalScore;
    public GameObject groupScore;
    public void Show(GameOverBilliardData data, Action actionSubmit)
    {
        base.Show();
        btSubmit.onClick.AddListener(() => { actionSubmit?.Invoke(); });
        if (data != null) LoadData(data);
    }

    public void LoadData(GameOverBilliardData data)
    {
        if (data.modeSyncAndUnSync == 1)
        {
            groupScore.gameObject.SetActive(true);
            switch (data.gameOverType)
            {
                case 0: //0 -> time up(het gio)
                    if (sprGameOverType.Length > 0) imgStatusGameOver.sprite = sprGameOverType[0];
                    bgGameOver.sprite = sprBgGameOver[1];
                    break;
                case 1: //quit
                    if (sprGameOverType.Length > 1) imgStatusGameOver.sprite = sprGameOverType[1];
                    bgGameOver.sprite = sprBgGameOver[1];
                    break;
                case 2: //success(choi het van)
                    if (sprGameOverType.Length > 2) imgStatusGameOver.sprite = sprGameOverType[2];
                    bgGameOver.sprite = sprBgGameOver[0];
                    break;
                default:
                    break;
            }

            imgStatusGameOver.SetNativeSize();

            HCHelper.NumberIncreasesEff(score, 0, data.score, 1, false);
            HCHelper.NumberIncreasesEff(timeBonus, 0, data.timeBonus, 1, false);
            HCHelper.NumberIncreasesEff(finalScore, 0, data.finalScore, 1, false);
        }
        else if (data.modeSyncAndUnSync == 2)
        {
            groupScore.gameObject.SetActive(false);
            if(data.result == 1)//thua
            {
                imgStatusGameOver.sprite = sprYouLose;
                bgGameOver.sprite = sprBgGameOver[1];
            }
            else if (data.result == 2)//thang
            {
                imgStatusGameOver.sprite = sprYouWin;
                bgGameOver.sprite = sprBgGameOver[0];
            }
            imgStatusGameOver.SetNativeSize();
        }
    }

    public override void Submit()
    {
        base.Submit();
    }
    public override void ClearData()
    {
        score.text = "0";
        timeBonus.text = "0";
        finalScore.text = "0";
    }
}