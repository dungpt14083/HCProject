
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GridItemElement : MonoBehaviour
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
   private string filenameavatar;

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

   public void onClickButton()
   {
      UpdateProfile.Ins.ButtonIDAvatar = btnId;
   }

   public void ShowView( KeyValuePair<int, HCAppController.SpriteAvatar> data,Action<int> action)
   {
      _id = data.Key;
      image.sprite = data.Value.SpriteImage;
      filenameavatar = data.Value.nameImage;
      _action = action;
      if(_id == HCAppController.Instance.chooseCurrentAvatar)
      {
         backgroundImage.sprite=selectedSprite;
         backgroundImage.color = new Color(255,255,255,255);
         UpdateProfile.Ins.Texture2D = image.sprite.texture;
         UpdateProfile.Ins.filenameAvatar = filenameavatar;
      }
   }
   public void SetSelected(int id)
   {
      if (_id==id)
      {
         Debug.Log("log selected"+ id);
         //hien len gameobject vien
        backgroundImage.sprite=selectedSprite;
        backgroundImage.color = new Color(255,255,255,255);
        UpdateProfile.Ins.Texture2D = image.sprite.texture;
        UpdateProfile.Ins.filenameAvatar = filenameavatar;
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
