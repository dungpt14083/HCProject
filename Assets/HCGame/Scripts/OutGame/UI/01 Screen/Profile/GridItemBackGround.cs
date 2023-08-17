using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridItemBackGround : MonoBehaviour
{
    public Image image;
    public Image backgroundImage;
    public Sprite normalSprite;
    public Sprite selectedSprite;
    public Button btnselected;
    private bool isSelected = false;
    public int _id;
    public int btnId;
    public Texture2D Texture2DGrid;
    private string filenameback;

    private Action<int> _action;

    private void Awake()
    {
        btnselected.onClick.AddListener((() =>
        {
            _action?.Invoke(_id);
        }));
    }

    private void OnEnable()
    {
        backgroundImage.color = new Color(255,255,255,0);
    }

    public void onclickButton()
    {
        UpdateProfile.Ins.ButtonIDBackground = btnId;
    }

    public void ShowView( KeyValuePair<int, HCAppController.SpriteBackGround> data,Action<int> action)
    {
        _id = data.Key;
        image.sprite = data.Value.spriteimage;
        filenameback =data.Value.nameImage;
        
        _action = action;
    }
    public void SetSelected(int id)
    {
        if (_id==id)
        {
            //hien len gameobject vien
            Debug.Log("select back ground");
            backgroundImage.sprite=selectedSprite;
            backgroundImage.color = new Color(255,255,255,255);
            UpdateProfile.Ins.Texture2DBackGround = image.sprite.texture;
            UpdateProfile.Ins.filenameBackground = filenameback;
            Debug.Log(filenameback+ "check log filename");
        }
        else
        {
            //an di
            backgroundImage.sprite = normalSprite;
            backgroundImage.color = new Color(255,255,255,0);
        }
    }

    public bool IsSelected()
    {
        return isSelected;
    }
}
