 using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UpdateProfile : PageBase
{
    public static UpdateProfile Ins;
    public Button btBack;
    public Button btSetting;
    public Action actionBack;
    public Image avatar;
    public Image Background;
    public Image BoderImage;
    public TMP_InputField input;
    public Button btnUpdate;
    public Button btnChooseAvatar;
    public Button btnChooseBackground;
    public Button btnChooseFrame;
    public Button Close;
    public GameObject PanelChooseAvatar;
    public GameObject PanelChooseBackground;
     
    public SettingController settingController;
    
    public GridItemElement _GridItemElement; //opengarrly
    public GridItemElement prefabItemGridLayout;
    public GridLayoutGroup GridLayoutGroupChooseAvatar;
    public string filenameAvatar;
    public int ButtonIDAvatar;

    [Header("grid lay out background")] 
    public GridItemBackGround _GridItemElementBackGround;//opengarrly
    public GridItemBackGround prefabItemGridLayoutBackGround;
    public GridLayoutGroup GridLayoutGroupChooseAvatarBackGround;
    public Sprite defaultSprite, slectedSprite;
    public Texture2D Texture2D;
    public Image selectBackGround;
    public String filenameBackground;
    public int ButtonIDBackground;
    public Texture2D Texture2DBackGround;
    public int currentSelection = -1;
    public int currentSelectionback = -1;
    private int index = 0;

    [Header("GridLayout FrameAvatar")] 
    public GridItemElementChooseFrameAvatar _GridItemElementChooseFrameAvatar;

    public GridItemElementChooseFrameAvatar PrefabItemElementChooseFrameAvatar;
    public GridLayoutGroup GridLayoutGroupFrameAvatar;
    public Texture2D Texture2DFrameAvatar;
    public Image selectedFrameAvatar;
    public String filenameFrameAvatar;
    public int ButtonIDFrameAvatar;
    public int currentSelectionFrame = -1;
    

    [Header("ButtonTab")]
    public Button tabFrameAvatar;
    public Button tabAvatar;
    public GameObject PanalFrameAvatar;
    public GameObject PanelAvatar;

    private void Awake()
    {
        Ins = this;
        btBack.onClick.AddListener(Back);
        btnUpdate.onClick.AddListener(updateUser);
        btnChooseAvatar.onClick.AddListener(startChooseeAvatar);
        btnChooseBackground.onClick.AddListener(startChooseeBackground1);
        input.onValueChanged.AddListener(OnInputValueChanged);
        btnChooseFrame.onClick.AddListener(startChooseFrame);
        tabFrameAvatar.onClick.AddListener(showFrameAvatar);
        tabAvatar.onClick.AddListener(showAvatar);
        btSetting.onClick.AddListener(showSetting);
    }

    public void showFrameAvatar()
    {
        PanalFrameAvatar.gameObject.SetActive(true);
        PanelAvatar.gameObject.SetActive(false);
    }

    public void showAvatar()
    {
        PanalFrameAvatar.gameObject.SetActive(false);
        PanelAvatar.gameObject.SetActive(true);
    }

    public void showSetting()
    {
        DisableDisplay();
        settingController.Show();
    }
    public void startChooseeAvatar()
    {
        StartCoroutine(chooseeAvatar());
    }

    public void startChooseeBackground1()
    {
        StartCoroutine(chooseeBackGround());
    }

    public void startChooseFrame()
    {
        StartCoroutine(chooseeFrameAvatar());
    }

    public static IEnumerator LoadBackGround(string url, Image background)
    {
        Debug.Log("Update background " + url);
        if (url == "")
        {
            if (background != null) background.sprite = null;
            yield return null;
        }
        else
        {
            WWW www = new WWW(url);
            yield return www;
            Texture2D profilePic = www.texture;
            if (www == null || www.texture == null)
            {
                Debug.Log("www == null || www.texture == null");
                if (background != null) background.sprite = null;
                yield return null;
            }
            else
            {
                var spr = Sprite.Create(profilePic, new Rect(0, 0, www.texture.width, www.texture.height),
                    new Vector2(0, 0));
                if (background != null) background.sprite = spr;
                HCAppController.Instance.myBackground = background.sprite;
            }
        }
    }

    #region callAPi

   
    public IEnumerator chooseeBackGround()
    {
        String token = HCAppController.Instance.GetTokenLogin();
        String userId = HCAppController.Instance.userInfo.UserCodeId;
        Debug.Log("token login " + token);
        Debug.Log("userid login " + HCAppController.Instance.userInfo.UserCodeId);
        byte[] imageData = Texture2DBackGround.EncodeToPNG();
        string base64String = Convert.ToBase64String(imageData);
        yield return null;
        Debug.Log(filenameBackground + " filename back ground");
        if (ButtonIDBackground == 1)
        {
            EditUserBackground request = new EditUserBackground()
            {
                userId = HCAppController.Instance.userInfo.UserCodeId,
                coverPhoto = filenameBackground
            };
            StartCoroutine(APIUtils.RequestWebApiPost(HCAppController.Instance.GetUrlEditUser(),
                JsonUtility.ToJson(request), token,
                (data) =>
                {
                    Sprite sprite = Sprite.Create(Texture2DBackGround,
                        new Rect(0, 0, Texture2DBackGround.width, Texture2DBackGround.height), Vector2.zero);
                    selectBackGround.sprite = sprite;
                    Debug.Log("RequestWebApiPost EditUserRequest data " + data);
                    PanelChooseBackground.SetActive((false));
                }));
        }

        if (ButtonIDBackground == 0)
        {
            EditUserBackgroundUpload request = new EditUserBackgroundUpload()
            {
                userId = HCAppController.Instance.userInfo.UserCodeId,
                coverPhotoUpload = base64String,
                filenameCoverPhoto = filenameBackground
            };
            StartCoroutine(APIUtils.RequestWebApiPost(HCAppController.Instance.GetUrlEditUser(),
                JsonUtility.ToJson(request), token,
                (data) =>
                {
                    Sprite sprite = Sprite.Create(Texture2DBackGround,
                        new Rect(0, 0, Texture2DBackGround.width, Texture2DBackGround.height), Vector2.zero);
                    selectBackGround.sprite = sprite;
                    Debug.Log("RequestWebApiPost EditUserRequest data " + data);
                    PanelChooseBackground.SetActive((false));
                }));
        }
    }
     public IEnumerator chooseeAvatar()
    {
        String token = HCAppController.Instance.GetTokenLogin();
        String userId = HCAppController.Instance.userInfo.UserCodeId;
        Debug.Log(filenameAvatar + " filename back ground");
        Debug.Log("token login " + token);
        Debug.Log("userid login " + HCAppController.Instance.userInfo.UserCodeId);
        byte[] imageData = Texture2D.EncodeToPNG();
        string base64String = Convert.ToBase64String(imageData);
        yield return null;
        if (ButtonIDAvatar == 1)
        {
            EditUserAvatar request = new EditUserAvatar()
            {
                userId = HCAppController.Instance.userInfo.UserCodeId,
                avatar = filenameAvatar
            };
            StartCoroutine(APIUtils.RequestWebApiPost(HCAppController.Instance.GetUrlEditUser(),
                JsonUtility.ToJson(request), token,
                (data) =>
                {
                    Debug.Log("RequestWebApiPost EditUserRequest data " + data);
                    Sprite sprite = Sprite.Create(Texture2D,
                        new Rect(0, 0, Texture2D.width, Texture2D.height), Vector2.zero);
                    HCAppController.Instance.myAvatar = sprite;
                    avatar.sprite = HCAppController.Instance?.myAvatar;
                    PanelChooseAvatar.SetActive((false));
                }));
        }

        if (ButtonIDAvatar == 0)
        {
            EditUserAvatarUpload request = new EditUserAvatarUpload()
            {
                userId = HCAppController.Instance.userInfo.UserCodeId,
                avatarUpload = base64String,
                filename = filenameAvatar
            };
            StartCoroutine(APIUtils.RequestWebApiPost(HCAppController.Instance.GetUrlEditUser(),
                JsonUtility.ToJson(request), token,
                (data) =>
                {
                    Debug.Log("RequestWebApiPost EditUserRequest data " + data);
                    Sprite sprite = Sprite.Create(Texture2D,
                        new Rect(0, 0, Texture2D.width, Texture2D.height), Vector2.zero);
                    HCAppController.Instance.myAvatar = sprite;
                    avatar.sprite = HCAppController.Instance?.myAvatar;
                    PanelChooseAvatar.SetActive((false));
                }));
        }
    }
    public IEnumerator chooseeFrameAvatar()
    {
        String token = HCAppController.Instance.GetTokenLogin();
        String userId = HCAppController.Instance.userInfo.UserCodeId;
        Debug.Log(filenameFrameAvatar + " filename back ground");
        Debug.Log("token login " + token);
        Debug.Log("userid login " + HCAppController.Instance.userInfo.UserCodeId);
        yield return null;
        EditUserFrameAvatar request = new EditUserFrameAvatar()
            {
                userId = HCAppController.Instance.userInfo.UserCodeId,
                frame = filenameFrameAvatar
            };
            StartCoroutine(APIUtils.RequestWebApiPost(HCAppController.Instance.GetUrlEditUser(),
                JsonUtility.ToJson(request), token,
                (data) =>
                {
                    Debug.Log("RequestWebApiPost EditUserRequest data " + data);
                    Sprite sprite = Sprite.Create(Texture2D,
                        new Rect(0, 0, Texture2D.width, Texture2D.height), Vector2.zero);
                    HCAppController.Instance.myBoder = sprite;
                    BoderImage.sprite = HCAppController.Instance?.myBoder;
                    PanelChooseAvatar.SetActive((false));
                    // Sprite sprite = Sprite.Create(Texture2D,
                    //     new Rect(0, 0, Texture2D.width, Texture2D.height), Vector2.zero);
                    // HCAppController.Instance.myAvatar = sprite;
                    // avatar.sprite = HCAppController.Instance?.myAvatar;
                    // PanelChooseAvatar.SetActive((false));
                }));
    }

    #endregion

    private void OnEnable()
    {
        ShowElementGridlayout();
        ShowElementBackGroundGridlayout();
        ShowGridElementFrameAvatar();
        input.text = HCAppController.Instance.userInfo.UserName;
        currentSelectionFrame = -1;
        currentSelectionback = -1;
        currentSelection = -1;
    }

    private void Update()
    {
        if (input.text.Length <= 0)
        {
            btnUpdate.interactable = false;
        }

        if (input.text.Length > 0)
        {
            btnUpdate.interactable = true;
        }
    }


    #region AvatarFrame

    private List<GridItemElementChooseFrameAvatar> _gridItemElementChooseFrameAvatars = new List<GridItemElementChooseFrameAvatar>();

    private void ShowGridElementFrameAvatar()  
    {
        _gridItemElementChooseFrameAvatars.Clear();
        
        Debug.Log("Log e Frame"+ HCAppController.Instance.lisSpriteFrameAvatar.Count);
        foreach (KeyValuePair<int, HCAppController.SpriteAvatarFrame> pair in
                 HCAppController.Instance.lisSpriteFrameAvatar)
        {
            GridItemElementChooseFrameAvatar imageObject = Instantiate(PrefabItemElementChooseFrameAvatar,
                GridLayoutGroupFrameAvatar.transform);
            imageObject.ShowView(pair,SelectFrameAvatar);
            _gridItemElementChooseFrameAvatars.Add(imageObject);
            Debug.Log("show view thành cong");
            imageObject.gameObject.SetActive(true);
        }
        GridLayoutGroupChooseAvatarBackGround.transform.parent.gameObject.SetActive(false);
        GridLayoutGroupChooseAvatarBackGround.transform.parent.gameObject.SetActive(true);
    }

    public void SelectFrameAvatar(int index)
    {
        currentSelectionFrame = index;
        for (int i = 0; i < _gridItemElementChooseFrameAvatars.Count; i++)
        {
            _gridItemElementChooseFrameAvatars[i].SetSelected(index);
        }
    }

    public void InvokeCurrentSelectFramaAvatar()
    {
        if (currentSelectionFrame >= 0)
        {
            // Do something with selected avatar index
            Debug.Log("Selected avatar index: " + currentSelectionFrame);
        }
    }

    private void OnSelectItemElementFrameAvatar(Texture2D texture)
    {
        // Lấy được texture trong item element đã được selected
        Debug.Log("Selected texture: " + texture.name);
    }
    

    #endregion
    #region background

    private List<GridItemBackGround> _listItemBackGrounds = new List<GridItemBackGround>();

    private void ShowElementBackGroundGridlayout()  
    {
        _listItemBackGrounds.Clear();
        foreach (KeyValuePair<int, HCAppController.SpriteBackGround> pair in
                 HCAppController.Instance.lisSpriteBackround)
        {
            GridItemBackGround imageObject = Instantiate(prefabItemGridLayoutBackGround,
                GridLayoutGroupChooseAvatarBackGround.transform);
            imageObject.ShowView(pair, SelectBackGround);
            _listItemBackGrounds.Add(imageObject);
            Debug.Log("show view thành cong");
            imageObject.gameObject.SetActive(true);
        }
        GridLayoutGroupChooseAvatarBackGround.transform.parent.gameObject.SetActive(false);
        GridLayoutGroupChooseAvatarBackGround.transform.parent.gameObject.SetActive(true);
    }

    public void SelectBackGround(int index)
    {
        currentSelectionback = index;
        Debug.Log(currentSelectionback+"UpdateSelectBackground");
        for (int i = 0; i < _listItemBackGrounds.Count; i++)
        {
            _listItemBackGrounds[i].SetSelected(index);
        }
    }

    public void InvokeCurrentSelectBackGround()
    {
        if (currentSelectionback >= 0)
        {
            // Do something with selected avatar index
            Debug.Log("Selected avatar index: " + currentSelectionback);
        }
    }

    private void OnSelectItemElementBackGround(Texture2D texture)
    {
        // Lấy được texture trong item element đã được selected
        Debug.Log("Selected texture: " + texture.name);
    }

    #endregion

    private List<GridItemElement> _listItem = new List<GridItemElement>();

    private void ShowElementGridlayout()
    {
        _listItem.Clear();
        // Debug.Log("show element");
        // GridItemElement imageObj =  Instantiate(_GridItemElement, GridLayoutGroupChooseAvatar.transform);
        // imageObj.gameObject.SetActive(true);
        // _listItem.Add(imageObj);
        foreach (KeyValuePair<int, HCAppController.SpriteAvatar> pair in HCAppController.Instance.lisSpriteAvatar)
        {
            GridItemElement imageObject = Instantiate(prefabItemGridLayout, GridLayoutGroupChooseAvatar.transform);

            imageObject.ShowView(pair, SelectAvatar);
            _listItem.Add(imageObject);
            Debug.Log("show view thành cong");
            imageObject.gameObject.SetActive(true);
        }
        //selectButton.onClick.AddListener(() => InvokeCurrentSelect())

        // Cập nhật lại layout để hiển thị các image đúng cách
        GridLayoutGroupChooseAvatar.transform.parent.gameObject.SetActive(false);
        GridLayoutGroupChooseAvatar.transform.parent.gameObject.SetActive(true);
    }

    void deleteItemGrid()
    {
        foreach (GridItemElement item in _listItem)
        {
            Destroy(item.gameObject);
        }

        foreach ( GridItemBackGround itemBackGround in _listItemBackGrounds)
        {
            Destroy(itemBackGround.gameObject);
            
        }
        foreach ( GridItemElementChooseFrameAvatar itemElementChooseFrame in _gridItemElementChooseFrameAvatars )
        {
            Destroy(itemElementChooseFrame.gameObject);
            
        }
        _listItemBackGrounds.Clear();
        _listItem.Clear();
    }
    public void SelectAvatar(int index)
    {
        currentSelection = index;
        // Highlight selected avatar item
        for (int i = 0; i < _listItem.Count; i++)
        {
            _listItem[i].SetSelected(index);
        }
    }

    public void InvokeCurrentSelect()
    {
        if (currentSelection >= 0)
        {
            // Do something with selected avatar index
            Debug.Log("Selected avatar index: " + currentSelection);
        }
    }

    private void OnSelectItemElement(Texture2D texture)
    {
        // Lấy được texture trong item element đã được selected
        Debug.Log("Selected texture: " + texture.name);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        avatar.sprite = HCAppController.Instance?.myAvatar;
        Background.sprite = HCAppController.Instance?.myBackground;
        BoderImage.sprite = HCAppController.Instance?.myBoder;
        //  Debug.Log("my background: "+HCAppController.Instance?.myBackground.texture);
        StartCoroutine(LoadBackGround(HCAppController.Instance.myurlbackround, Background));
        StartCoroutine(LoadBoder(HCAppController.Instance.myUrlBoder, BoderImage));
        FadeIn();
    }
    public IEnumerator LoadBoder(string url, Image Boder)
    {
        yield return new WaitForSeconds(1);

        if (url == "")
        {
            if (Boder != null) Boder.sprite = null;
            yield return null;
        }
        else
        {
            WWW www = new WWW(url);
            yield return www;
            Texture2D profilePic = www.texture;
            if (www == null || www.texture == null)
            {
                Debug.Log("www == null || www.texture == null");
                if (Boder != null) Boder.sprite = null;
                yield return null;
            }
            else
            {

                var spr = Sprite.Create(profilePic, new Rect(0, 0, www.texture.width, www.texture.height),
                    new Vector2(0, 0));
                if (Boder != null) Boder.sprite = spr;
                BoderImage.sprite = Boder.sprite;
                Debug.Log("đã load sprte");
                HCAppController.Instance.myBoder = Boder.sprite;
            }
        }
    }
    private void OnInputValueChanged(string value)
    {
        // Remove any characters that are not letters or digits
        string filteredValue = Regex.Replace(value, @"[^a-zA-Z0-9]", "");

        // Update the text field with the filtered value
        input.text = filteredValue;
    }
    public void updateUser()
    {
        string token = HCAppController.Instance.GetTokenLogin();
        var value = input.text.Trim();
        EditUserRequest request = new EditUserRequest
        {
            userId = HCAppController.Instance.userInfo.UserCodeId,
            username = value
        };
        StartCoroutine(APIUtils.RequestWebApiPost(HCAppController.Instance.GetUrlEditUser(),
            JsonUtility.ToJson(request), token,
            (data) =>
            {
                Debug.Log("Update Profile EditUserRequest data " + data);
                HcPopupManager.Instance.ShowNotifyPopup("Update Profile Name User Succes");
                input.text = "";
            }));
    }

    public void Hide()
    {
        DisableDisplay();
    }

    private void OnDisable()
    {
        deleteItemGrid();
    }

    public void Back()
    {
        actionBack?.Invoke();
    }
    //public string GetTitle()
    //{
    //    return "Update Profile";
    //}
}