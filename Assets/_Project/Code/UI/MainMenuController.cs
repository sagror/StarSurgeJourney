using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using DG.Tweening;

namespace StarSurgeJourney.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject creditsPanel;
        
        [Header("UI Elements")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private TextMeshProUGUI titleText;
        
        [Header("Animations")]
        [SerializeField] private float buttonAnimationDelay = 0.2f;
        [SerializeField] private float titleAnimationDuration = 1.0f;
        
        [Header("Audio")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioClip buttonClickSound;
        [SerializeField] private AudioClip buttonHoverSound;
        
        [Header("Escenes")]
        [SerializeField] private string gameSceneName = "Game";
        
        private void Start()
        {
            if (startButton != null)
                startButton.onClick.AddListener(OnStartButtonClicked);
                
            if (settingsButton != null)
                settingsButton.onClick.AddListener(OnSettingsButtonClicked);
                
            if (creditsButton != null)
                creditsButton.onClick.AddListener(OnCreditsButtonClicked);
                
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitButtonClicked);
            
            ShowMainPanel();
            
            StartCoroutine(AnimateMenuElements());
        }
        
        private IEnumerator AnimateMenuElements()
        {
            if (titleText != null)
            {
                titleText.transform.localScale = Vector3.zero;
                titleText.transform.DOScale(1, titleAnimationDuration).SetEase(Ease.OutBack);
            }
            
            Button[] buttons = { startButton, settingsButton, creditsButton, quitButton };
            
            foreach (Button button in buttons)
            {
                if (button != null)
                {
                    button.transform.localScale = Vector3.zero;
                    button.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);
                    
                    yield return new WaitForSeconds(buttonAnimationDelay);
                }
            }
        }
                
        public void ShowMainPanel()
        {
            mainPanel.SetActive(true);
            settingsPanel.SetActive(false);
            creditsPanel.SetActive(false);
        }
        
        public void ShowSettingsPanel()
        {
            mainPanel.SetActive(false);
            settingsPanel.SetActive(true);
            creditsPanel.SetActive(false);
        }
        
        public void ShowCreditsPanel()
        {
            mainPanel.SetActive(false);
            settingsPanel.SetActive(false);
            creditsPanel.SetActive(true);
        }
        
        
        private void OnStartButtonClicked()
        {
            PlayButtonClickSound();
            
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.DOFade(0, 1f).OnComplete(() => {
                    LoadGameScene();
                });
            }
            else
            {
                LoadGameScene();
            }
        }
        
        private void LoadGameScene()
        {
            SceneManager.LoadScene(gameSceneName);
        }
        
        private void OnSettingsButtonClicked()
        {
            PlayButtonClickSound();
            ShowSettingsPanel();
        }
        
        private void OnCreditsButtonClicked()
        {
            PlayButtonClickSound();
            ShowCreditsPanel();
        }
        
        private void OnQuitButtonClicked()
        {
            PlayButtonClickSound();
            
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.DOFade(0, 1f).OnComplete(() => {
                    QuitGame();
                });
            }
            else
            {
                QuitGame();
            }
        }
        
        private void QuitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
        
        
        public void OnBackButtonClicked()
        {
            PlayButtonClickSound();
            ShowMainPanel();
        }
        
        
        private void PlayButtonClickSound()
        {
            if (sfxSource != null && buttonClickSound != null)
            {
                sfxSource.PlayOneShot(buttonClickSound);
            }
        }
        
        public void OnButtonHover()
        {
            if (sfxSource != null && buttonHoverSound != null)
            {
                sfxSource.PlayOneShot(buttonHoverSound);
            }
        }
    }
}