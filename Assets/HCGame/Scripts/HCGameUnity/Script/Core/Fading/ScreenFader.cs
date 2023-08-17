using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using NBCore;

public class ScreenFader : SingletonMono<ScreenFader> {
   
    [SerializeField]
	private Image FadeImg;
    [SerializeField]
    private Color colorFade;

    private void FadeToClear(float timer, System.Action complete = null)
	{
        FadeImg.DOColor(Color.clear, timer).OnComplete(()=> {
            FadeImg.enabled = false;
            complete?.Invoke();
        });
    }

    private void FadeToBlack(float timer, System.Action complete = null)
    {
       
        FadeImg.DOColor(colorFade, timer).OnComplete(() => {
            complete?.Invoke();
        });
    }
	
	private void StartFadeInOut(float time, System.Action complete = null)
	{
        FadeToBlack(time, ()=> {
            FadeToClear(time, ()=> {
                Debug.Log("FadeInOut Complete");
                complete?.Invoke();
            });
        });
       
    }

    public void FadeOut(float time, System.Action complete = null)
    {
        FadeImg.color = colorFade;
        FadeImg.enabled = true;
        FadeToClear(time, () => {
            Debug.Log("FadeInOut Complete");
            complete?.Invoke();
        });
    }

    public void FadeInOut(float time, System.Action complete = null)
	{
        FadeImg.color = Color.clear;
        FadeImg.enabled = true;
        StartFadeInOut(time, ()=> {
            complete?.Invoke();
        });
    }

    public void FadeIn(float time, System.Action complete = null)
    {
        FadeImg.color = Color.clear;
        FadeImg.enabled = true;
        FadeToBlack(time, () => {
            complete?.Invoke();
        });
    }
    
}
