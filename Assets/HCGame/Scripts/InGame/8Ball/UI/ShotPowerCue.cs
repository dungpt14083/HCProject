using System;
using System.Collections;
using System.Collections.Generic;
using Tweens.Plugins;
using UnityEngine;
using UnityEngine.UI;

public class ShotPowerCue : MonoBehaviour
{
    [SerializeReference] private TweenMove tweenMove;
    [SerializeReference] private RectTransform cue;
    [SerializeReference] private RectTransform posEndCue;
    public Image shotColorIndicator;
    private float initXPos;
    private Vector3 initcue;
    private Color initPowerColor;
    GameController gameController;
    public bool mouseDown = false;
    public Action<float> OnPowerChanged;
    public Action<float> OnPowerFire;
    float cacheDuration;
    float power;
    public TweenMove TweenMove
    {
        get { return tweenMove; }
        set { tweenMove = value; }
    }

    private void Awake()
    {
        initcue = cue.anchoredPosition;
    }
    private void Start()
    {
        cacheDuration = tweenMove.duration;
        initPowerColor = shotColorIndicator.color;
        if (gameController == null)
        {
            gameController = GameController.Instance;
            gameController.OnBlockShot += OnBlockShot;
        }
    }
    public void OnMouseDown()
    {
        mouseDown = true;
        if (EightBallGameSystem.Instance.CurrentMode == PlayMode.Practice)
        {
            GameController.Instance.tutorialController.CompleteStep2();
        }
        initXPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
    }

    private void OnDestroy()
    {
        if (gameController != null)
            gameController.OnBlockShot -= OnBlockShot;
    }

    void OnBlockShot(bool isBlock)
    {
        Debug.Log("OnBlockShot isBlock " + isBlock);
        if (isBlock)
        {           
            tweenMove.OnBack();
        }
        else
        {         
            tweenMove.OnPlay();
        }
    }

    public void OnMouseUp()
    {
        shotColorIndicator.color = initPowerColor;  
        var powerChanged = Mathf.InverseLerp(initcue.y, posEndCue.anchoredPosition.y, cue.anchoredPosition.y);
        
        OnPowerFire?.Invoke(powerChanged);
        
        cue.anchoredPosition = initcue;
        //
        // if (!GameManager.Instance.stopTimer)
        // {
        //     cue.anchoredPosition = initcue;
        // }
        mouseDown = false;
    }

    private void Update()
    {
        OnUpdate();
    }

    public void OnUpdate()
    {
        if (mouseDown)
        {
            //Chuyển Sprite renderer Cây gậy(cue) thành UGUI
            Vector3 cuePos2D = cue.anchoredPosition;
            var currentX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
            var diff = currentX - initXPos;
            //Debug.Log($"initXPos = {initXPos} ---- current x = {currentX} --- diff = {diff}");
            float newYPos2D = initcue.y - diff * 105;

            if (newYPos2D < initcue.y && newYPos2D > posEndCue.anchoredPosition.y)
            {
                cuePos2D.y = newYPos2D;
                cue.anchoredPosition = cuePos2D;
            }
            else
            {
                if (newYPos2D > initcue.y)
                    cue.anchoredPosition = initcue;
                else if (newYPos2D < posEndCue.anchoredPosition.y)
                {
                    Vector3 pos = cue.anchoredPosition;
                    pos.y = posEndCue.anchoredPosition.y;
                    cue.anchoredPosition = pos;
                }
            }
            power = Mathf.InverseLerp(initcue.y, posEndCue.anchoredPosition.y, cue.anchoredPosition.y);
            OnPowerChanged?.Invoke(power);
            setIndicatorColor();
        }
    }
    public float startC = 0.25f;
    public float endC = 1f;
    // Sets indicator color when cue is moving
    private void setIndicatorColor()
    {
        float add = Mathf.Lerp(startC, endC, power);
        Color color = shotColorIndicator.color;
        color.r = add;
        color.g = add;
        color.b = add;
        shotColorIndicator.color = color;
    }
}
