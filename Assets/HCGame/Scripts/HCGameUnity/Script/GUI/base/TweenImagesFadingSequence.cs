using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenImagesFadingSequence : MonoBehaviour
{
    [SerializeField] private List<TweenImageFade> imageFadeList;

    private int _currentIndex = 0;
    private bool _isPlaying = false;

    private void FadeImage()
    {
        imageFadeList[_currentIndex].ResetFade();
        imageFadeList[_currentIndex].OnFadeToEndAlpha(() =>
        {
            _currentIndex++;
            if(_currentIndex < imageFadeList.Count)
            {
                FadeImage();
            }
        });
    }

    public void StartPlayFadingSequence()
    {
        if(_currentIndex >= imageFadeList.Count)
        {
            return;
        }
        if(_isPlaying == true)
        {
            return;
        }

        _isPlaying = true;
        _currentIndex = 0;
        gameObject.SetActive(true);

        FadeImage();
    }
}
