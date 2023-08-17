using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Collections;
using UnityEngine;

public class RewardTopTournamentPopup : MonoBehaviour
{
    [SerializeField] ItemRewardTopTournament prefabItemRewardTopTournament;
    [SerializeField] RectTransform holder;


    public void ShowView(RepeatedField<RewardTournament> listData)
    {
        SetDefault();
        for (int i = 0; i < listData.Count; i++)
        {
            var tmp = Instantiate(prefabItemRewardTopTournament,holder);
            tmp.ShowView(listData[i]);
        }
    }

    private void SetDefault()
    {
        for (int i = holder.childCount - 1; i >= 0; i--)
        {
            BonusPool.DeSpawn(holder.GetChild(i));
        }
    }
}
