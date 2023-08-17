using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameOverBubblesShotData
{
    public int GameOverType;//0 -> time up(het gio), 1 -> quit, 2-> success(choi het van)
    public int Score;
    public int ClearBoard;
    public int TimeBonus;
    public int FinalScores;
}

public class GameOverBubblesShot : GameOverPopupBase
{
    [SerializeField] private TMP_Text _score;
    [SerializeField] private TMP_Text _clearBoard;
    [SerializeField] private TMP_Text _bonusTime;
    [SerializeField] private TMP_Text _totalScore;
    [SerializeField] private Image _titleImg;
    public Sprite[] sprGameOverType;
    public void Show(GameOverBubblesShotData data, Action actionSubmit)
    {
        Debug.Log("GameOverBubblesShot ");
        base.Show();
        btSubmit.onClick.AddListener(() => {
            actionSubmit?.Invoke();
        });
        LoadData(data);
    }
    public void LoadData(GameOverBubblesShotData data)
    {
        if (data == null)
        {
            _score.text = "0";
            _clearBoard.text = "0";
            _bonusTime.text = "0";
            _totalScore.text = "0";
            if (sprGameOverType.Length > 1) _titleImg.sprite = sprGameOverType[0];
            return;
        }
        switch (data.GameOverType)
        {
            case 0://0 -> quit
                if (sprGameOverType.Length > 0) _titleImg.sprite = sprGameOverType[0];
                break;
            case 1://success(choi het van)
                if (sprGameOverType.Length > 1) _titleImg.sprite = sprGameOverType[1];
                break;
            case 2:// time up(het gio)
                if (sprGameOverType.Length > 2) _titleImg.sprite = sprGameOverType[2];
                break;
        }
        HCHelper.NumberIncreasesEff(_score, 0, data.Score, 1, false);
        HCHelper.NumberIncreasesEff(_clearBoard, 0, data.ClearBoard, 1, false);
        HCHelper.NumberIncreasesEff(_bonusTime, 0, data.TimeBonus, 1, false);
        HCHelper.NumberIncreasesEff(_totalScore, 0, data.FinalScores, 1, false);
    }
    public override void Submit()
    {
        base.Submit();
    }
    public override void ClearData()
    {
        _score.text = "0";
        _clearBoard.text = "0";
        _bonusTime.text = "0";
        _totalScore.text = "0";
    }
}
