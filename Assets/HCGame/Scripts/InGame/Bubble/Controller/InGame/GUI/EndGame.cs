using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BubblesShot
{
    public class EndGame : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public void OnClickOKButton()
        {
            UIManager.Instance.HidePopup();
            SessionPref.ResetSessionPref();
            Application.Quit();
        }
    }
}
