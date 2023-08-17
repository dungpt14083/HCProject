using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bingo_PopupScoreEffect : MonoBehaviour
{
    [SerializeField] GameObject[] _listScoreEffect;
    [SerializeField] Transform _content;
    [SerializeField] AnimationCurve _aniCurve;
    [SerializeField] float _timeEff;

     
    int _currentID = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayScoreEffect(int id)
    {
        _listScoreEffect[_currentID].SetActive(false);
        _currentID = id;
        _listScoreEffect[_currentID].SetActive(true);
        _content.gameObject.SetActive(true);
        _content.DOKill();
        _content.localScale = Vector3.one;
        _content.DOScale(1, 1f).SetEase(_aniCurve).OnComplete(() =>
        {
            _content.gameObject.SetActive(false);
        });

    }
}
