using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaseEventBanner : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] protected Image imgBg;
    [SerializeField] protected Sprite[] listSprite;
    [SerializeField] protected Button btnToLink;

    [SerializeField] protected List<GameObject> listBtnNextPrevious;
    [SerializeField] protected Transform parent;
    [SerializeField] protected NotButton prefab;

    [SerializeField] protected bool isHome = false;
    [SerializeField] protected int numPage = 0;

    [SerializeField] protected GameObject title;


    protected readonly List<NotButton> _listNotButton = new List<NotButton>();
    protected int _currentPage = 0;

    protected void OnEnable()
    {
        ClearButton();
        _listNotButton.Clear();
        _currentPage = 0;
        title.gameObject.SetActive(!isHome);
        if (numPage > 0)
        {
            for (int i = 0; i < listBtnNextPrevious.Count; i++)
            {
                listBtnNextPrevious[i].SetActive(!isHome);
            }

            parent.gameObject.SetActive(true);
            ShowGroupButtonToggle();
            ShowToggle(_currentPage);
        }
        else
        {
            for (int i = 0; i < listBtnNextPrevious.Count; i++)
            {
                listBtnNextPrevious[i].SetActive(!isHome);
            }

            parent.gameObject.SetActive(false);
        }

        btnToLink.onClick.RemoveAllListeners();
        OpenEventButton();

        StartCoroutine(AutoSlider());
    }

    private IEnumerator AutoSlider()
    {
        while (true)
        {
            yield return new WaitForSeconds(5.0f);
            NextPage();
            if (gameObject.activeSelf)
            {
                yield break;
            }
        }
    }

    public void NextPage()
    {
        _currentPage += 1;
        if (_currentPage > numPage)
        {
            _currentPage = 0;
        }

        OpenEventButton();
        ShowToggle(_currentPage);
    }

    public void PreviousPage()
    {
        _currentPage -= 1;
        if (_currentPage < 0)
        {
            _currentPage = numPage;
        }

        OpenEventButton();
        ShowToggle(_currentPage);
    }

    protected void ShowGroupButtonToggle()
    {
        for (int i = 0; i < numPage + 1; i++)
        {
            var notButton = BonusPool.Spawn(prefab, parent);
            notButton.index = i;
            _listNotButton.Add(notButton);
        }
    }

    private void ShowToggle(int index)
    {
        for (int i = 0; i < _listNotButton.Count; i++)
        {
            _listNotButton[i].ShowToggle(index);
        }
    }

    private void ClearButton()
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            BonusPool.DeSpawn(parent.GetChild(i));
        }
    }

    protected virtual void OpenEventButton()
    {
        btnToLink.onClick.RemoveAllListeners();
        if (_currentPage < listSprite.Length)
        {
            imgBg.sprite = listSprite[_currentPage];
        }
    }

    #region TODOANIMATION

    [SerializeField] protected float distanceToSlide;
    protected Vector2 _swipeStartPos;


    public void OnPointerDown(PointerEventData eventData)
    {
        _swipeStartPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //_isDragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        var distance = _swipeStartPos.x - eventData.position.x;
        if (distance > distanceToSlide)
        {
            if (Mathf.Abs(distance) >= distanceToSlide)
            {
                if (distance < 0)
                {
                    NextPage();
                }
                else
                {
                    PreviousPage();
                }
            }
        }
    }

    #endregion
}