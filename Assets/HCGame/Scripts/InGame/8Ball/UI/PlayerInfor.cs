using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerInfor : MonoBehaviour
{
    
    [SerializeField] public Image imgAvatar;
    [SerializeField] public TextMeshProUGUI txtName;
    [SerializeField] private Image[] lstImgSpot;
    [Header("Timer")]
    [SerializeField] private GameObject imgTimerParent;
    [SerializeField] private Image imgTimer;
    [SerializeField] private TextMeshProUGUI txtTimer;
    private Dictionary<int, int> indexDictionary = new Dictionary<int, int>();
    public List<int> listBallId = new List<int>();
    public void SetTimer(float value)
    {
        if (EightBallGameSystem.Instance.CurrentMode == PlayMode.Practice)
            return;
        imgTimer.fillAmount = value;
        txtTimer.SetText("{0}s", Mathf.RoundToInt(value*EightBallGameSystem.TURN_TIME));
        imgTimerParent.SetActive(value > 0);
    }
    private void Awake()
    {
        //indexDictionary.Clear();
        //indexDictionary.Add(1, 0);
        //indexDictionary.Add(2, 1);
        //indexDictionary.Add(3, 2);
        //indexDictionary.Add(4, 3);
        //indexDictionary.Add(5, 4);
        //indexDictionary.Add(6, 5);
        //indexDictionary.Add(7, 6);
        //indexDictionary.Add(8, 7);
    }

    public float GetTimer()
        => imgTimer.fillAmount;

    public void SetActiveImgSpot(bool active, int index)
    {
        try
        {
            Debug.Log("set active : " + active + "===" + index);
            lstImgSpot[index].enabled = active;
        }catch(Exception e)
        {
            Debug.Log("set active error: " + active + "===" + index);
        }
    }

    public void ClearActiveImgSpot()
    {
        listBallId.Clear();
        for (int i = 0; i < lstImgSpot.Length; i++)
        {
            SetActiveImgSpot(false, i);
        }
    }
    
    public void AddBalls(List<int> ballIds)
    {
        try
        {
            listBallId = ballIds;
            int index = lstImgSpot.Length-1;
            foreach(var ballId in ballIds)
            {
                if(index > -1)
                {
                    Sprite spr;

                    if (EightBallGameSystem.Instance.CurrentMode == PlayMode.Online)
                    {
                        spr = EightBallUIController.Instance.GetSpriteByBallID(ballId);
                    }
                    else
                    {
                        var ballIndex = ballId == 8 ? 5 : 8;
                        spr = EightBallUIController.Instance.GetSpriteByBallID(ballIndex);
                    }

                    lstImgSpot[index].sprite = spr;
                    SetActiveImgSpot(true, index);
                    indexDictionary[ballId] = index;
                }
                index -= 1;
            }
        }
        catch(Exception e)
        {
            Debug.Log("AddBall Error ballID = ");
        }
        
        // currentIndex++;
    }

    public void RemoveBall(int ballID)
    {
        Debug.Log("remove ball : " + ballID + "===");
        foreach(var ball in indexDictionary)
        {
            SetActiveImgSpot(false, ball.Value);
        }
        if (indexDictionary.TryGetValue(ballID, out var index))
        {
            indexDictionary.Remove(ballID);
            listBallId.Remove(ballID);
            Debug.Log("remove ball : " + ballID +"===" + index);
            SetActiveImgSpot(false, index);
            if(listBallId.Count > 0)
            {
                AddBalls(listBallId);
            }
            else
            {
                if(EightBallGameSystem.Instance.CurrentMode == PlayMode.Online)
                {
                    //add ball 8 sprite
                    var ball8Sprite = EightBallUIController.Instance.GetSpriteByBallID(8);
                    lstImgSpot[lstImgSpot.Length - 1].sprite = ball8Sprite;
                    lstImgSpot[lstImgSpot.Length - 1].enabled = true;
                }
            }
            //for (int i = 0; i < lstImgSpot.Length; i++)
            //{
            //    if (lstImgSpot[i].isActiveAndEnabled)
            //    {
            //        return;
            //    }
            //}
            
        }

    }
    public void UpdatePlayerData(string name, string urlAvatar)
    {
        txtName.SetText(name);
        StartCoroutine(HCHelper.LoadAvatar(urlAvatar, imgAvatar));
    }
    public void UpdatePlayerData(string name, Sprite avatar)
    {
        txtName.SetText(name);
        imgAvatar.sprite = avatar;
    }
}
