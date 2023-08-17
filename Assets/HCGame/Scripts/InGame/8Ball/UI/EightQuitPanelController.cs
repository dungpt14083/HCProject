using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EightQuitPanelController : MonoBehaviour
{
    [SerializeField] GameObject _quitMenu, _quitPopup;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //================Show/hide====================
    public void ShowQuitMenu(bool isShow)
    {
        _quitMenu.SetActive(isShow);
    }

    public void ShowQuitPopup(bool isShow)
    {
        _quitPopup.SetActive(isShow);
    }

    //=====================BUTTON====================
    public void BtnQuit_Click()
    {

    }
  
}
