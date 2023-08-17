using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using AsyncOperation = UnityEngine.AsyncOperation;

public class EightLoadingPanelController : MonoBehaviour
{
    [SerializeField] AnimationCurve _loadingCurve;
    [SerializeField] Image _loadingImage;
    [SerializeField] TextMeshProUGUI _loadingText;
    [SerializeField] float _timeLoading;
    [SerializeField] float _loadingProgress;
    bool _isLoadSceneFinish, _isRunFakeLoadingCompleted;
    [SerializeField] GameObject[] backgrounds;
    
    public void LoadScene(GameType gameType)
    {
        ScreenManagerHC.Instance.CloseAllScreen();
        ScreenManagerHC.Instance.groupBottomHomeController.gameObject.SetActive(false);
        if (backgrounds.Length <= 0) return;
        int indexScene = (int)gameType - 1;
        indexScene = indexScene < 0 ? 0 : indexScene;
        indexScene = indexScene >= backgrounds.Length ? backgrounds.Length - 1 : indexScene;
        backgrounds[indexScene].SetActive(true);
        Executors.Instance.StartCoroutine(LoadGameAsync(HCAppController.Instance.GetNameScene(gameType)));
    }

    IEnumerator LoadGameAsync(string sceneName)
    {
        _loadingProgress = 0f;
        _isLoadSceneFinish = false;
        _isRunFakeLoadingCompleted = false;
        DOTween.To(() => _loadingProgress, x => _loadingProgress = x, 1f, _timeLoading).SetEase(_loadingCurve)
        .OnUpdate(() =>
        {
            _loadingText.SetText("{0}%", Mathf.RoundToInt(_loadingProgress * 100));
            _loadingImage.fillAmount = _loadingProgress;
        }).OnComplete(() =>
        {
            _isRunFakeLoadingCompleted = true;
        });
        Debug.Log("LoadGameAsync sceneName " + sceneName);
        AsyncOperation loadSceneOperator = SceneManager.LoadSceneAsync(sceneName);
        loadSceneOperator.allowSceneActivation = false;
        while (!loadSceneOperator.isDone)
        {
            if (loadSceneOperator.progress >= 0.9f)
            {
                _isLoadSceneFinish = true;
                break;
            }
            yield return null;
        }
        yield return new WaitUntil(() => _isRunFakeLoadingCompleted);
        yield return new WaitUntil(() => _isLoadSceneFinish);
        _loadingText.SetText("{0}%", 100);
        _loadingImage.fillAmount = 1;
        yield return new WaitForSeconds(0.1f);
        loadSceneOperator.allowSceneActivation = true;
    }
}

