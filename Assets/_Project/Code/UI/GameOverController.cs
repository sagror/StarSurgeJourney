using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace StarSurgeJourney.UI
{
    public class GameOverController : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private CanvasGroup statsGroup;
        
        [Header("Animation")]
        [SerializeField] private float titleAnimDuration = 1.0f;
        [SerializeField] private float statsAnimDelay = 0.5f;
        [SerializeField] private float statsAnimDuration = 0.5f;
        [SerializeField] private float buttonsAnimDelay = 1.0f;
        [SerializeField] private float buttonsAnimDuration = 0.5f;
        
        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip gameOverSound;
        [SerializeField] private AudioClip buttonClickSound;
        
        private void OnEnable()
        {
            if (retryButton != null)
                retryButton.onClick.AddListener(OnRetryClicked);
                
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(OnMainMenuClicked);
                
            AnimateGameOver();
            
            if (audioSource != null && gameOverSound != null)
            {
                audioSource.PlayOneShot(gameOverSound);
            }
        }
        
        private void OnDisable()
        {
            if (retryButton != null)
                retryButton.onClick.RemoveListener(OnRetryClicked);
                
            if (mainMenuButton != null)
                mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
        }
        
        public void SetGameOverData(int score, int highScore, bool isVictory = false)
        {
            if (titleText != null)
            {
                titleText.text = isVictory ? "Victory!" : "Game Over";
                titleText.color = isVictory ? Color.green : Color.red;
            }
            
            if (scoreText != null)
                scoreText.text = "Score: " + score.ToString();
                
            if (highScoreText != null)
                highScoreText.text = "High Score: " + highScore.ToString();
        }
        
        private void AnimateGameOver()
        {
            if (titleText != null)
            {
                titleText.transform.localScale = Vector3.zero;
                titleText.transform.DOScale(1, titleAnimDuration).SetEase(Ease.OutBack);
            }
            
            if (statsGroup != null)
            {
                statsGroup.alpha = 0;
                statsGroup.DOFade(1, statsAnimDuration).SetDelay(statsAnimDelay);
            }
            
            Button[] buttons = { retryButton, mainMenuButton };
            
            foreach (var button in buttons)
            {
                if (button != null)
                {
                    button.transform.localScale = Vector3.zero;
                    button.transform.DOScale(1, buttonsAnimDuration)
                        .SetEase(Ease.OutBack)
                        .SetDelay(buttonsAnimDelay);
                }
            }
        }
                
        private void OnRetryClicked()
        {
            PlayButtonSound();
            
            // Reload current scene
            string currentScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentScene);
        }
        
        private void OnMainMenuClicked()
        {
            PlayButtonSound();
            
            // Load main menu scene
            SceneManager.LoadScene("MainMenu");
        }
        
        private void PlayButtonSound()
        {
            if (audioSource != null && buttonClickSound != null)
            {
                audioSource.PlayOneShot(buttonClickSound);
            }
        }
    }
}