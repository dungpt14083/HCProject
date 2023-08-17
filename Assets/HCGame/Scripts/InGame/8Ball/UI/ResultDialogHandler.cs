using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultDialogHandler : MonoBehaviour
{
    [SerializeField] private Text resultText;
    [SerializeField] GameObject _resultWinBg, resultLoseBg;
    [SerializeField] TextMeshProUGUI _txtShotTotal, _txtTimeLet, _txtFinalScore;
    public void Show(bool isWin)
    {
        resultText.text = isWin ? "You Win!" : "You Lose!";
        this.gameObject.SetActive(true);
        _resultWinBg.SetActive(isWin);
        resultLoseBg.SetActive(!isWin);
    }
    
    public void SetPointShow(uint points)
    {
        resultText.text = $"Your Points: {points}";
        //this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void OnOkClicked()
    {
        SceneManager.LoadScene("Home");

    }

    public void SetDataResult(int shottotal, int timeLeft, int finalScore)
    {
        _txtShotTotal.SetText(shottotal.ToString());
        _txtTimeLet.SetText(timeLeft.ToString());
        _txtFinalScore.SetText(finalScore.ToString());
    }
}