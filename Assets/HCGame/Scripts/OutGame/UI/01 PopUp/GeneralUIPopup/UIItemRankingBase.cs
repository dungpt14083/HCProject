using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItemRankingBase<TData> : MonoBehaviour
{
    [SerializeField] protected Image avatar;
    [SerializeField] protected TMP_Text playerName;
    [SerializeField] protected Image iconMoney;
    [SerializeField] protected TMP_Text valueMoney;
    [SerializeField] protected GameObject highLight;
    [SerializeField] protected Image bgItem;
    [SerializeField] protected List<Sprite> listSpriteBgItem;

    protected TData _data;
    protected string _urlAvatar;
    protected int _indexRank = 0;

    public virtual void ShowView(TData data, int indexRank)
    {
        SetDefault();
        _data = data;
        _indexRank = indexRank;
    }


    protected virtual void SetEmptyItem()
    {
        highLight.gameObject.SetActive(false);
        avatar.sprite = ResourceManager.Instance.GetAvatarDefault();
        playerName.text = "--";
        valueMoney.text = "--";
    }

    public virtual void SetDefault()
    {
        highLight.gameObject.SetActive(false);
        avatar.sprite = ResourceManager.Instance.GetAvatarDefault();
        playerName.text = "--";
        valueMoney.text = "--";
    }
}