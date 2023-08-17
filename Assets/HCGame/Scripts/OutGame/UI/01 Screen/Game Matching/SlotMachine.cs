using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RoyalMatch;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SlotMachine : MonoBehaviour
{

    public RectTransform content;
    public RectTransform DefultTransform;
    private float targetPosY;
    public List<RectTransform> _listPos = new List<RectTransform>();
    private float delay = 0.1f; 
    public float duration = 0.5f;
    private Vector2 defaultPos;
    public Image imageToRotate;
    public List<Image> listObjSprite= new List<Image>();
    private List<Sprite> _sprites = new List<Sprite>();
    private Vector3 originalPos;
    public bool isScrollListRunning = false;
    void Start () {
        targetPosY = content.anchoredPosition.y;
        DefultTransform.anchoredPosition = content.anchoredPosition;
        defaultPos = new Vector2(46,155);
        StartCoroutine(ScrollList());
        loadSprite();
        imageToRotate.rectTransform.DORotate(new Vector3(0, 0, 360), duration, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart);
         originalPos = _listPos[0].localPosition;
    }
    
    void loadSprite()
    {
       LoadSpritesInList();
       setSprites();
    }
    void setSprites()
    {
        foreach (var VARIABLE in listObjSprite)
        {
            VARIABLE.sprite = _sprites[Random.Range(0, _sprites.Count)];
        }
    }
    void LoadSpritesInList()
    {
        foreach (KeyValuePair<int, HCAppController.SpriteAvatar> pair in HCAppController.Instance.lisSpriteAvatar)
        {
            _sprites.Add(pair.Value.SpriteImage);
        }
    }
   public IEnumerator ScrollList()
   {
       isScrollListRunning = true;
        while (isScrollListRunning)
        {
            foreach (RectTransform rt in _listPos) {
                rt.DOLocalMoveY(-200, duration) // Di chuyển object xuống dưới
                    .OnComplete(() =>
                    {
                        rt.localPosition =originalPos;
                    });
                yield return new WaitForSeconds(delay); 

            }
        }
     
    }
}
