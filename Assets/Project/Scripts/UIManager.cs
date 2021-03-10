using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Scripts
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Text scoreText;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Text gameOverScoreText;
        private int _score;

        #region Unity Methods
        
        // Start is called before the first frame update
        void Start()
        {
            EventManager.Current.onScoreUpdated += OnScoreUpdated;
            EventManager.Current.onGameOver += OnGameOver;
        }

        private void OnDestroy()
        {
            EventManager.Current.onScoreUpdated -= OnScoreUpdated;
            EventManager.Current.onGameOver -= OnGameOver;
        }

        #endregion

        #region OnClick Callbacks

        /// <summary>
        /// Called when replay button clicked.
        /// </summary>
        public void RestartGame()
        {
            SceneManager.LoadSceneAsync(1);
            Time.timeScale = 1;
        }

        /// <summary>
        /// Called when quit game button clicked.
        /// </summary>
        public void QuitGame()
        {
            Application.Quit();
        }

        #endregion

        #region Event Callbacks

        /// <summary>
        /// Called when the score is updated. Updates score text in UI.
        /// </summary>
        /// <param name="score"></param>
        private void OnScoreUpdated(int score)
        {
            _score = score;
            scoreText.text = "Score: " + score;
        }

        /// <summary>
        /// Called when the game ends. Opens game over panel.
        /// </summary>
        private void OnGameOver()
        {
            gameOverPanel.transform.localScale = Vector3.zero;
            gameOverPanel.SetActive(true);
            gameOverPanel.transform.DOScale(Vector3.one, 2);
            scoreText.transform.DOScale(Vector3.zero, 2).OnComplete(() => scoreText.gameObject.SetActive(false));
            gameOverScoreText.text += _score;
        }

        #endregion
    }
}