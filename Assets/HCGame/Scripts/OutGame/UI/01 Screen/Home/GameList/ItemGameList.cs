using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemGameList : MonoBehaviour
{
    [SerializeField]
    private GameType gameType;
    public Image iconGame;
    public TextMeshProUGUI nameGame;
    public Button btSelect;

    private void Awake()
    {
        btSelect.onClick.AddListener(SelectGame);
    }
    public void Show(GameType _gameType, string name)
    {
        gameObject.SetActive(true);
        this.gameType = _gameType;
        nameGame.text = name;
        iconGame.sprite = ResourceManager.Instance.GetIconGame(_gameType);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void SelectGame()
    {
        ScreenManagerHC.Instance.ShowGameModeUI(this.gameType);
    }
}
