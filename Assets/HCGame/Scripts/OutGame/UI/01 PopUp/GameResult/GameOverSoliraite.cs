using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverSoliraiteAndBilliardData
{
    public int gameOverType;//0 -> time up(het gio), 1 -> quit, 2-> success(choi het van)
    public int score;
    public int timeBonus;
    public int finalScore;
}

public class GameOverSoliraite : GameOverPopupBase
{
    //LIST LOẠI TITLE
    public Sprite[] sprGameOverType;
    public Image imgStatusGameOver;
    public TMP_Text txtScore;
    public TMP_Text txtTimeBonus;
    public TMP_Text txtFinalScore;

    public void Show(GameOverSoliraiteAndBilliardData data, Action actionSubmit)
    {
        base.Show();
        btSubmit.onClick.AddListener(()=> {
            actionSubmit?.Invoke();
        });
        if (data != null) LoadData(data);
    }
    
    
    public void LoadData(GameOverSoliraiteAndBilliardData data)
    {
        switch (data.gameOverType)
        {
            case 0://0 -> time up(het gio)
                if(sprGameOverType.Length > 0) imgStatusGameOver.sprite = sprGameOverType[0];
                break;
            case 1://quit
                if (sprGameOverType.Length > 1) imgStatusGameOver.sprite = sprGameOverType[1];
                break;
            case 2://success(choi het van)
                if (sprGameOverType.Length > 2) imgStatusGameOver.sprite = sprGameOverType[2];
                break;
            default:
                break;
        }
        imgStatusGameOver.SetNativeSize();

        HCHelper.NumberIncreasesEff(txtScore, 0, data.score, 1, false);
        HCHelper.NumberIncreasesEff(txtTimeBonus, 0, data.timeBonus, 1, false);
        HCHelper.NumberIncreasesEff(txtFinalScore, 0, data.finalScore, 1, false);
    }
    public override void Submit()
    {
        base.Submit();
    }
    public override void ClearData()
    {
        txtScore.text = "0";
        txtTimeBonus.text = "0";
        txtFinalScore.text = "0";
    }
}