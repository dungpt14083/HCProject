using System;
using System.Collections;
using System.Collections.Generic;
using BonusGame;
using TMPro;
using UnityEngine;

public class BonusGameRankPopup : PageBase
{
    [SerializeField] GameObject bgTop;
    [SerializeField] List<BonusGameRankItem> listBonusGameRankItem;

    [SerializeField] TMP_Text txtTime;


    private List<ResBonusGameRankDTO> _listBonusGameRankItem = new List<ResBonusGameRankDTO>();

    private long _timeCount = 0;
    private Coroutine _coroutineTime;


    #region ANIMATIONSHOWHIDE

    private void OnEnable()
    {
        //Init();
    }

    private void Init()
    {
        for (int i = 0; i < _listBonusGameRankItem.Count && i < listBonusGameRankItem.Count; i++)
        {
            listBonusGameRankItem[i].ShowView(_listBonusGameRankItem[i],i);
        }

        if (_coroutineTime != null)
        {
            StopCoroutine(_coroutineTime);
        }

        _coroutineTime = StartCoroutine(ShowTime());
    }

    private IEnumerator ShowTime()
    {
        while (true)
        {
            _timeCount -= 1;
            if (_timeCount < 0)
            {
                RefreshTopUserSignals.RefreshTop.Dispatch();
                yield break;
            }

            yield return new WaitForSeconds(1.0f);
            var tmp = TimeSpan.FromSeconds(_timeCount);
            txtTime.text = $"Reset in {tmp.Days}d {tmp.Hours:00}h {tmp.Minutes:00}m";
        }
    }

    public void ShowView(GuideBonusGameType type, ListResponseRank data)
    {
        _listBonusGameRankItem.Clear();
        _listBonusGameRankItem = data.list;
        _timeCount = data.countTime;
        bgTop.gameObject.SetActive(true);
        FadeIn(Init);
    }

    public void Hide()
    {
        bgTop.gameObject.SetActive(false);
        FadeOut();
    }

    #endregion
}