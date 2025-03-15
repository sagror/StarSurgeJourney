using UnityEngine;
using System.Collections;
using StarSurgeJourney.Systems.Generation;
using StarSurgeJourney.Systems.Skills;
using StarSurgeJourney.UI;

namespace StarSurgeJourney
{
    // Game states enumeration
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver,
        Victory
    }
    
    // Main game manager
    public class GameManager : MonoBehaviour
    {
        [Header("Systems")]
        [SerializeField] private ProceduralLevelManager levelManager;
        [SerializeField] private UIManager uiManager;
        
        [Header("Player")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform playerSpawnPoint;
        
        [Header("Configuration")]
        [SerializeField] private int initialSkillPoints = 3;
        [SerializeField] private int skillPointsPerLevel = 2;
        
        // Current game state
        private GameState currentState;
        
        // Player reference
        private GameObject playerInstance;
        
        // Skill tree
        private SkillTree skillTree;
        
        // Game data
        private int currentScore = 0;
        private int highScore = 0;
        private int currentLevel = 1;
        
        // Singleton
        private static GameManager instance;
        
        public static GameManager Instance
        {
            get { return instance; }
        }
        
        // Properties
        public GameState CurrentState => currentState;
        public int CurrentScore => currentScore;
        public int CurrentLevel => currentLevel;
        public SkillTree PlayerSkillTree => skillTree;
        
        private void Awake()
        {
            // Singleton setup
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            
            // Load high score
            highScore = PlayerPrefs.GetInt("HighScore", 0);
            
            // Initialize skill tree
            InitializeSkillTree();
        }
        
        private void Start()
        {
            // Start with main menu
            ChangeState(GameState.MainMenu);
        }
        
        // Initialize skill tree
        private void InitializeSkillTree()
        {
            skillTree = SkillTreeFactory.CreateBasicShipSkillTree();
            skillTree.SkillPoints = initialSkillPoints;
        }
        
        // Method to start a new game
        public void StartNewGame()
        {
            // Reset game data
            currentScore = 0;
            currentLevel = 1;
            
            // Create player
            SpawnPlayer();
            
            // Generate first level
            if (levelManager != null)
            {
                levelManager.GenerateInitialLevel();
            }
            
            // Apply skills to player
            ApplySkillTreeToPlayer();
            
            // Change state
            ChangeState(GameState.Playing);
            
            // Show game UI
            if (uiManager != null)
            {
                uiManager.ShowHUD();
            }
        }
        
        // Player spawning
        private void SpawnPlayer()
        {
            if (playerInstance != null)
            {
                Destroy(playerInstance);
            }
            
            if (playerPrefab != null)
            {
                Vector3 spawnPosition = playerSpawnPoint != null ? playerSpawnPoint.position : Vector3.zero;
                playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            }
        }
        
        // Apply skill tree effects to player
        private void ApplySkillTreeToPlayer()
        {
            if (skillTree != null && playerInstance != null)
            {
                skillTree.ApplyAllEffects(playerInstance);
            }
        }
        
        // Change game state
        public void ChangeState(GameState newState)
        {
            // If state doesn't change, do nothing
            if (currentState == newState)
                return;
                
            // Exit current state
            ExitCurrentState();
            
            // Change to new state
            currentState = newState;
            
            // Enter new state
            EnterNewState();
        }
        
        // Exit current state
        private void ExitCurrentState()
        {
            switch (currentState)
            {
                case GameState.MainMenu:
                    // Clear main menu
                    break;
                    
                case GameState.Playing:
                    // Pause active systems
                    break;
                    
                case GameState.Paused:
                    // Close pause menu
                    Time.timeScale = 1f;
                    break;
                    
                case GameState.GameOver:
                case GameState.Victory:
                    // Clear results screen
                    break;
            }
        }
        
        // Enter new state
        private void EnterNewState()
        {
            switch (currentState)
            {
                case GameState.MainMenu:
                    // Show main menu
                    if (uiManager != null)
                    {
                        uiManager.ShowMainMenu();
                    }
                    break;
                    
                case GameState.Playing:
                    // Activate systems
                    if (uiManager != null)
                    {
                        uiManager.ShowHUD();
                    }
                    break;
                    
                case GameState.Paused:
                    // Show pause menu
                    Time.timeScale = 0f;
                    if (uiManager != null)
                    {
                        uiManager.ShowPauseMenu();
                    }
                    break;
                    
                case GameState.GameOver:
                    // Show game over screen
                    Time.timeScale = 0f;
                    
                    // Update high score if necessary
                    if (currentScore > highScore)
                    {
                        highScore = currentScore;
                        PlayerPrefs.SetInt("HighScore", highScore);
                        PlayerPrefs.Save();
                    }
                    
                    if (uiManager != null)
                    {
                        uiManager.ShowGameOver(currentScore, highScore);
                    }
                    break;
                    
                case GameState.Victory:
                    // Show victory screen
                    Time.timeScale = 0f;
                    
                    // Update high score
                    if (currentScore > highScore)
                    {
                        highScore = currentScore;
                        PlayerPrefs.SetInt("HighScore", highScore);
                        PlayerPrefs.Save();
                    }
                    
                    if (uiManager != null)
                    {
                        // Could have a specific victory method
                        uiManager.ShowGameOver(currentScore, highScore);
                    }
                    break;
            }
        }
        
        // Pause/resume game
        public void TogglePause()
        {
            if (currentState == GameState.Playing)
            {
                ChangeState(GameState.Paused);
            }
            else if (currentState == GameState.Paused)
            {
                ChangeState(GameState.Playing);
            }
        }
        
        // Add points
        public void AddScore(int points)
        {
            currentScore += points;
            
            // Update UI
            if (uiManager != null)
            {
                uiManager.UpdateScore(currentScore);
            }
        }
        
        // Go to next level
        public void GoToNextLevel()
        {
            currentLevel++;
            
            // Add skill points
            skillTree.SkillPoints += skillPointsPerLevel;
            
            // Generate new level
            if (levelManager != null)
            {
                levelManager.GenerateNextLevel();
            }
            
            // Show notification
            if (uiManager != null)
            {
                HUDController hudController = FindObjectOfType<HUDController>();
                if (hudController != null)
                {
                    hudController.ShowNotification("Level " + currentLevel + " - Skill Points: +" + skillPointsPerLevel);
                }
                
                uiManager.UpdateWaveText(currentLevel);
            }
        }
        
        // Game Over
        public void GameOver(bool isVictory = false)
        {
            ChangeState(isVictory ? GameState.Victory : GameState.GameOver);
        }
        
        // Show skill tree
        public void ShowSkillTree()
        {
            if (uiManager != null)
            {
                uiManager.ShowSkillTree(skillTree);
            }
        }
        
        // Update
        private void Update()
        {
            // Global input handling
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (currentState == GameState.Playing)
                {
                    TogglePause();
                }
                else if (currentState == GameState.Paused)
                {
                    TogglePause();
                }
            }
            
            // Key to show skill tree
            if (Input.GetKeyDown(KeyCode.K) && currentState == GameState.Playing)
            {
                ShowSkillTree();
            }
        }
    }
}