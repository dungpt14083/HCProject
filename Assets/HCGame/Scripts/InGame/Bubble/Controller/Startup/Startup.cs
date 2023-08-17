using InitScriptName;
using System.Collections;
using System.Collections.Generic;
using BubblesShot;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace BubblesShot
{
    public class Startup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _per;
        [SerializeField] private Slider _loading;

        UserData _userData;
        float _timeCountdown = 0;
        const float _maxTimeCountdown = 5f;
        // Start is called before the first frame update
        void Start()
        {
            _userData = Commons.GetUserData();
            //WebSocket - Request server matching room here

        }

        // Update is called once per frame
        void Update()
        {
            //Check if request success a room then change to gameplay
            if (_timeCountdown >= 0 && _timeCountdown <= _maxTimeCountdown)
            {
                _timeCountdown += Time.deltaTime;
                _per.text = (int)(_timeCountdown * 100 / _maxTimeCountdown) + "%";
                _loading.value = _timeCountdown / _maxTimeCountdown;
            }
            else
            {
                SceneFlowManager.Instance.LoadScene(Scenes.BubbleGameplay);
            }
        }
    }
}
