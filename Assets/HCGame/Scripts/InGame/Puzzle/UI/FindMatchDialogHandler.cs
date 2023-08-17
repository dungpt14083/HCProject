using Cysharp.Threading.Tasks;
using Google.Protobuf;
using MiniGame.MatchThree.Scripts.Network;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MiniGame.MatchThree.Scripts.UI
{
    public class FindMatchDialogHandler : MonoBehaviour
    {
        [SerializeField] private TMP_InputField urlInput;
        [SerializeField] private TextMeshProUGUI txtStatus;
        [SerializeField] private Button btnFindMatch;

        private string URL_KEY = "TETRIS_URL";
        private GameStatus currentStatus = GameStatus.Disconneced;

        CompositeDisposable disposables = new CompositeDisposable();

        private void Start()
        {
            //string defaultUrl = "ws://192.168.2.99:8080";//local nghia server
            string defaultUrl = "ws://18.141.169.208:8086"; //ws match 3 game server


            string initUrl = PlayerPrefs.GetString(URL_KEY, defaultUrl);
            urlInput.text = initUrl;

            MatchThreeNetworkManager.Instance.OnConnectedSocketCallback += OnSocketConnected;
            MatchThreeNetworkManager.Instance.OnDisconnectedSocketCallback += onSocketDisconnected;

            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    txtStatus.text = $"Status: {currentStatus}";
                    btnFindMatch.interactable = currentStatus == GameStatus.Disconneced;
                }).AddTo(disposables);
        }

        #region Event

        public void OnClickFindRoom()
        {
            OnConnect();
        }

        #endregion

        #region Socket

        private void OnSocketConnected()
        {
            ReadyToPlay();
        }

        private void onSocketDisconnected(string msg)
        {
            currentStatus = GameStatus.Disconneced;
        }

        #endregion

        #region Utils

        private void OnConnect()
        {
            currentStatus = GameStatus.Connecting;
            MatchThreeGameSystem.Instance.PlayGame();
            MatchThreeNetworkManager.Instance.Connect(urlInput.text, null);
        }

        private async void ReadyToPlay()
        {
            currentStatus = GameStatus.Ready;
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            //SceneManager.LoadScene("M3_NetworkGamePlay");
            HcPopupManager.Instance.ShowEightGameLoading(true, GameType.Puzzle);
        }

        #endregion

        private void OnDisable()
        {
            disposables.Clear();

            MatchThreeNetworkManager.Instance.OnConnectedSocketCallback -= OnSocketConnected;
            MatchThreeNetworkManager.Instance.OnDisconnectedSocketCallback -= onSocketDisconnected;
        }
    }
}