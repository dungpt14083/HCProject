using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ConformDialogHandler : MonoBehaviour
{
    public enum EConfirmDialogResult
    {
        Yes,
        No,
        Cancel
    }

    [SerializeField]
    private TMP_Text titleText;
    [SerializeField]
    private TMP_Text contentText;
    [SerializeField]private ScaleOnClick okButton;
    [SerializeField]private TMP_Text okText;

    [SerializeField]private ScaleOnClick cancelButton;
    [SerializeField]private TMP_Text cancelText;

    private UniTaskCompletionSource<EConfirmDialogResult> finishedTaskCompletion;

    public async UniTask<EConfirmDialogResult> Show(string title, string content, string okay, string cancel)
    {
        finishedTaskCompletion = new UniTaskCompletionSource<EConfirmDialogResult>();
        gameObject.SetActive(true);
        titleText.text = title;
        contentText.text = content;
        okText.text = okay;
        cancelText.text = cancel;
        cancelButton.OnDelayedClick.RemoveAllListeners();
        cancelButton.OnDelayedClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            finishedTaskCompletion?.TrySetResult(EConfirmDialogResult.Cancel);
        });

        okButton.OnDelayedClick.RemoveAllListeners();
        okButton.OnDelayedClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            finishedTaskCompletion?.TrySetResult(EConfirmDialogResult.Yes);
        });
        return await finishedTaskCompletion.Task;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        finishedTaskCompletion?.TrySetResult(EConfirmDialogResult.Cancel);
    }

}

