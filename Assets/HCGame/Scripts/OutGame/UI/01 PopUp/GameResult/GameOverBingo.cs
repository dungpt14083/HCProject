using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverBingoData
{
    public int pointDoubs;
    public int numberDoubs;
    public int pointBingos;
    public int numberBingos;
    public int pointMultiBingos;
    public int numberMultiBingos;
    public int pointDoubleScore;
    public int numberDoubleScore;
    public int pointPenalties;
    public int numberPenalties;
    public int gameOverType;//0 -> time up(het gio), 1 -> quit, 2-> success(choi het van)
    public int finalScore;
}
public class GameOverBingo : GameOverPopupBase
{
    public TMP_Text txtPointDoubs;
    public TMP_Text txtNumberDoubs;
    public TMP_Text txtPointBingos;
    public TMP_Text txtNumberBingos;
    public TMP_Text txtPointMultiBingos;
    public TMP_Text txtNumberMultiBingos;
    public TMP_Text txtPointDoubleScore;
    public TMP_Text txtNumberDoubleScore;
    public TMP_Text txtPointPenalties;
    public TMP_Text txtNumberPenalties;
    public TMP_Text txtFinalScore;
    public GameObject[] gameOverType;

    public void Show(GameOverBingoData data, Action actionSubmit)
    {
        base.Show();
        btSubmit.onClick.AddListener(() => {
            actionSubmit?.Invoke();
        });
        if (data != null) LoadData(data);
    }
    public void LoadData(GameOverBingoData data)
    {
        switch (data.gameOverType)
        {
            case 0:// time up(het gio)
                if (gameOverType.Length > 2) gameOverType[2].SetActive(true);
                break;
            case 1://0 -> quit
                if (gameOverType.Length > 0) gameOverType[0].SetActive(true);
                break;
            case 2://success(choi het van)
                if (gameOverType.Length > 1) gameOverType[1].SetActive(true);
                break;
        }
        HCHelper.NumberIncreasesEff(txtPointDoubs, 0, data.pointDoubs,1, false);
        txtNumberDoubs.text = data.numberDoubs.ToString();

        HCHelper.NumberIncreasesEff(txtPointBingos, 0, data.pointBingos,1, false);
        txtNumberBingos.text = data.numberBingos.ToString();

        HCHelper.NumberIncreasesEff(txtPointMultiBingos, 0, data.pointMultiBingos,1, false);
        txtNumberMultiBingos.text = data.numberMultiBingos.ToString();

        HCHelper.NumberIncreasesEff(txtPointDoubleScore, 0, data.pointDoubleScore,1, false);
        txtNumberDoubleScore.text = data.numberDoubleScore.ToString();

        HCHelper.NumberIncreasesEff(txtPointPenalties, 0, data.pointPenalties,1, false);
        txtNumberPenalties.text = data.numberPenalties.ToString();

        HCHelper.NumberIncreasesEff(txtFinalScore, 0, data.finalScore,1, false);
    }
    public override void Submit()
    {
        base.Submit();
    }
    public override void ClearData()
    {
        txtPointDoubs.text = "0";
        txtNumberDoubs.text = "0";
        txtPointBingos.text = "0";
        txtNumberBingos.text = "0";
        txtPointMultiBingos.text = "0";
        txtNumberMultiBingos.text = "0";
        txtPointDoubleScore.text = "0";
        txtNumberDoubleScore.text = "0";
        txtPointPenalties.text = "0";
        txtNumberPenalties.text = "0";
        txtFinalScore.text = "0";
        foreach (var type in gameOverType)
        {
            type.SetActive(false);
        }
    }
}
