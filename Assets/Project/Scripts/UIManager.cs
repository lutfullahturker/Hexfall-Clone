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

        public void RestartGame()
        {
            SceneManager.LoadSceneAsync(1);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    
        private void OnScoreUpdated(int score)
        {
            _score = score;
            scoreText.text = "Score: " + score;
        }

        private void OnGameOver()
        {
            gameOverPanel.transform.localScale = Vector3.zero;
            gameOverPanel.SetActive(true);
            gameOverPanel.transform.DOScale(Vector3.one, 2);
            scoreText.transform.DOScale(Vector3.zero, 2).OnComplete(() => scoreText.gameObject.SetActive(false));
            gameOverScoreText.text += _score;
        }
    }
}
