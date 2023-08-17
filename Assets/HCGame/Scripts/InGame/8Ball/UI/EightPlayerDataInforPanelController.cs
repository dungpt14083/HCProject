using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EightPlayerDataInforPanelController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _txtPlayerName, _txtBestScore, _txtWinRate;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetPlayerDataInfo(string playerName, int bestScore, int winRate)
    {
        _txtPlayerName.SetText(playerName);
        _txtBestScore.SetText(bestScore.ToString());
        _txtWinRate.SetText("{0}%",winRate);
    }
}
