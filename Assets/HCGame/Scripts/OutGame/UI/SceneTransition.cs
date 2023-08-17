using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using MoreMountains.Feedbacks;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
	[SerializeField] MMF_Player fadeIn;
	[SerializeField] MMF_Player fadeOut;
	[SerializeField] MMF_Player flash;
	readonly CompositeDisposable disposables = new CompositeDisposable();

	private void SetUpCanvas()
	{
		try
		{
			var canvas = transform.Find("Canvas").GetComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingLayerName = "Scene Transition";
			canvas.sortingOrder = 100;
		}
		catch (System.Exception e)
		{
			Debug.LogError(e);
		}
	}

	public double FadeIn()
	{
		fadeIn.PlayFeedbacks();
		return fadeIn.TotalDuration;
	}

	public double FadeOut()
	{
		fadeOut.PlayFeedbacks();
		return fadeIn.TotalDuration;
	}

	public double Flash()
	{
		flash.PlayFeedbacks();
		return fadeIn.TotalDuration;
	}

	public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
	{
		try
		{
			double delay = FadeIn();
			MainThreadDispatcher.StartCoroutine(DelayForNavigate(sceneName, mode, (float)delay));
		}
		catch (Exception e)
		{
			Debug.LogError(e);
		}
	}

	private IEnumerator DelayForNavigate(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, float delay = 0.3f)
	{
		yield return new WaitForSeconds(delay);
		SceneManager.LoadSceneAsync(sceneName, mode);
	}

	public void ReloadScence()
	{
		double delay = Flash();
		SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
	}

	public void FlashLoadScene(string sceneName)
	{
		double delay = Flash();
		SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
	}


	private void OnDestroy()
	{
		disposables.Dispose();
	}
}