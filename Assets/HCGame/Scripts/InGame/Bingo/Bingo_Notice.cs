using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace Bingo {
    public class Bingo_Notice : MonoBehaviour
    {
        public static Bingo_Notice instance;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.LogError("MULTI INS");
                Destroy(gameObject);
            }
        }






        // Start is called before the first frame update
        [TitleGroup("___________  Reference  __________")]
        [SerializeField] private GameObject _contents;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _text;




        [TitleGroup("___________  Config  __________")]
        [SerializeField]
        private float fadeTime = .3f;





        [Button]
        public void DisplaySelf()
        {
            _contents.gameObject.SetActive(true);
            _canvasGroup.alpha = 0;
            DOTween.To(() => _canvasGroup.alpha, v => _canvasGroup.alpha = v, 1, fadeTime).Play().OnComplete(() =>
            {

            });
        }





        [Button]
        public void OnClickShowNotice(string notice)
        {
            _text.text = notice;
            DisplaySelf();
        }

        public void OnClickClose()
        {
            DOTween.To(() => _canvasGroup.alpha, v => _canvasGroup.alpha = v, 0, fadeTime).Play().OnComplete(() =>
            {
                _contents.SetActive(false);
            });
        }


        public void OnClickEndMatch()
        {
            //BingoGame_AnimationEffect.instance.OnClickBackToHCApp();
            Bingo_NetworkManager.instance.SendMessageQuit();
            OnClickClose();
            //Bingo_NetworkManager.instance.Disconnect();
        }


    }
}