using MiniGame.MatchThree.Scripts.Network;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
namespace MiniGame.MatchThree.Scripts.UI
{
    public class GameplayDialogHandler : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI txtScore;
        [SerializeField] private TextMeshProUGUI txtSwap;
        [SerializeField] private Text txtDebugLog;
        [SerializeField] private GameObject loading;
        [SerializeField] private RectTransform parentText;

        private int m_currentScore = 0;
        private int m_counterValue = 0;
        private int m_increment = 5;

        private Text targetText;

        private void Awake()
        {
            targetText = txtDebugLog;
        }

        private void OnPLaying(RoyalMatch.DataGame data)
        {
            AddSwap(data.NumberSwap);
            UpdateScoreText(data.Points);
        }

        public void UpdateScoreText(int scoreValue)
        {
            if (txtScore != null)
            {
                txtScore.text = ""+scoreValue;
            }
        }

        public void AddScore(int value)
        {
            m_currentScore += value;
            StartCoroutine(CountScoreRoutine());
        }

        IEnumerator CountScoreRoutine()
        {
            int iterations = 0;
            while (m_counterValue < m_currentScore && iterations < 10000)
            {
                m_counterValue += m_increment;
                UpdateScoreText(m_counterValue);
                iterations++;
                yield return null;
            }
            m_counterValue = m_currentScore;
            UpdateScoreText(m_currentScore);
        }

        public void AddSwap(int value)
        {
            txtSwap.text = value.ToString();
        }


        public void OnStartGame()
        {
            UpdateScoreText(m_currentScore);
        }

        public void OnGameOver()
        {
            SceneManager.LoadScene("M3_ResultResult");
        }

        public void OnLoading(bool isShow)
        {
            loading.SetActive(isShow);
        }

        public void AddDebugLog(string value)
        {
            if (GetMaxLineCount(targetText) >= 250)
            {
                GameObject objText = Instantiate(txtDebugLog.gameObject, Vector3.zero, Quaternion.identity);
                objText.transform.parent = parentText;
                targetText = objText.GetComponent<Text>();
                targetText.text = string.Empty;
            }

            targetText.text += "\n" + value;
        }

        #region Event

        public void OnRestartGame()
        {
            OnStartGame();
        }
        public void TogPauseGame(Toggle tog)
        {
            if (tog.isOn)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
        }

        public void TogRemove(Toggle tog)
        {
            if (tog.isOn)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
        }

        #endregion

        private void OnEnable()
        {
            MatchThreeNetworkManager.Instance.OnDebugLog += AddDebugLog;
            MatchThreeGameSystem.Instance.ActStartGame += OnPLaying;
        }

        private void OnDisable()
        {
            MatchThreeNetworkManager.Instance.OnDebugLog -= AddDebugLog;
            MatchThreeGameSystem.Instance.ActStartGame -= OnPLaying;
        }

        private int GetMaxLineCount(Text text)
        {
            var textGenerator = new TextGenerator();
            var generationSettings = text.GetGenerationSettings(text.rectTransform.rect.size);
            var lineCount = 0;
            var s = new StringBuilder();
            while (true)
            {
                s.Append("\n");
                textGenerator.Populate(s.ToString(), generationSettings);
                var nextLineCount = textGenerator.lineCount;
                if (lineCount == nextLineCount) break;
                lineCount = nextLineCount;
            }
            return lineCount;
        }
    }
}