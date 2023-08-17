using System.Collections;
using System.Collections.Generic;
using BonusGame;
using UnityEngine;

public class AnimationSpawner : MonoBehaviour
{
    [SerializeField] List<GameObject> playerPrefab;
    [SerializeField] [Range(0, 50)] int poolSize = 3;
    [SerializeField] [Range(0.1f, 30f)] float spawnTimer = 0.2f;

    List<GameObject> _poolObjects = new List<GameObject>();

    public static AnimationSpawner ins;

    void Awake()
    {
        if (ins != null)
        {
            Debug.LogError("Multi ins" + gameObject.name);
            Destroy(this);
        }

        ins = this;
        _poolObjects.Clear();
        PopulatePool(poolSize);
    }

    void PopulatePool(int amount)
    {
        for (int i = 0; i < poolSize; i++)
        {
            var tmp = Instantiate(playerPrefab[0], transform.position, Quaternion.identity);
            _poolObjects.Add(tmp);
            tmp.transform.SetParent(this.transform);
            tmp.transform.localScale = new Vector3(1, 1, 1);
            tmp.SetActive(false);
        }
    }


    public void Spawn(List<MsgItem> listMsgItem)
    {
        SpawnPri(listMsgItem);
    }

    private void SpawnPri(List<MsgItem> listMsgItem)
    {
        var listAvailable = new List<GameObject>();

        for (int i = 0; i < _poolObjects.Count; i++)
        {
            if (!_poolObjects[i].activeSelf)
            {
                listAvailable.Add(_poolObjects[i]);
            }
        }

        for (int i = 0; i < listMsgItem.Count && i < listAvailable.Count; i++)
        {
            listAvailable[i].SetActive(true);
            listAvailable[i].GetComponent<Transform>().position = listMsgItem[i].GetComponent<Transform>().position;
            listAvailable[i].transform.SetAsLastSibling();
            //yield return new WaitForSeconds(spawnTimer);
            if (listAvailable[i].TryGetComponent<RewardAnimation>(out var comp))
            {
                comp.StartAnimation(listMsgItem[i].Type);
            }
        }
    }


    #region SHOWANIMATIONREWARDTMP

    
    public void Spawn(List<ListRewardAndPosition> listReward)
    {
        SpawnTmp(listReward);
    }

    private void SpawnTmp(List<ListRewardAndPosition> listReward)
    {
        var listAvailable = new List<GameObject>();

        for (int i = 0; i < _poolObjects.Count; i++)
        {
            if (!_poolObjects[i].activeSelf)
            {
                listAvailable.Add(_poolObjects[i]);
            }
        }

        for (int i = 0; i < listReward.Count && i < listAvailable.Count; i++)
        {
            listAvailable[i].SetActive(true);
            listAvailable[i].GetComponent<Transform>().position = listReward[i].position;
            listAvailable[i].transform.SetAsLastSibling();
            //yield return new WaitForSeconds(spawnTimer);
            if (listAvailable[i].TryGetComponent<RewardAnimation>(out var comp))
            {
                comp.StartAnimationTmp((RewardType)listReward[i].reward.RewardType);
            }
        }
    }
    

    #endregion
    
}