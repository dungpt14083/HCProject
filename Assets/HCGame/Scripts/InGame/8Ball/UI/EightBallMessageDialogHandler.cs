using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EightBallMessageDialogHandler : MonoBehaviour
{
    [SerializeField] private Text resultText;
    public void Show(string message)
    {
        resultText.text = message;
        var color = resultText.color;
        color.a = 1.0f;
        resultText.color = color;
        this.gameObject.SetActive(true);
    }

    public void onFinishedShow()
    {
        GameObject.Destroy(gameObject);
    }
    
}