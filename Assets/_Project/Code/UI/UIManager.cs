using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using StarSurgeJourney.Models;
using StarSurgeJourney.Systems.Skills;
using DG.Tweening;

namespace StarSurgeJourney.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Panels references")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject hudPanel;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject skillTreePanel;
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private GameObject settingsPanel;
        
        [Header("HUD References")]
        [SerializeField] private Image healthBar;
        [SerializeField] private Image shieldBar;
        [SerializeField] private Image energyBar;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI ammoText;
        [SerializeField] private TextMeshProUGUI waveText;
        [SerializeField] private Image weaponIcon;
        [SerializeField] private TextMeshProUGUI objectivesText;
        
        [Header("Skill Tree References")]
        [SerializeField] private Transform skillTreeContainer;
        [SerializeField] private GameObject skillNodePrefab;
        [SerializeField] private TextMeshProUGUI skillPointsText;
        
        [Header("Referencias Game Over")]
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        
        private ShipModel playerModel;
        
        private SkillTree skillTree;
        
        private Dictionary<string, GameObject> skillNodeUIElements = new Dictionary<string, GameObject>();
        
        private void Awake()
        {
            if (DOTween.instance == null)
            {
                DOTween.Init();
            }
        }
        
        private void Start()
        {
            if (playerModel == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerModel = player.GetComponent<ShipModel>();
                    
                    if (playerModel != null)
                    {
                        playerModel.OnHealthChanged += UpdateHealthBar;
                    }
                }
            }
            
            ShowMainMenu();
        }
        
        
        public void ShowMainMenu()
        {
            HideAllPanels();
            mainMenuPanel.SetActive(true);
            
            Time.timeScale = 0f;
        }
        
        public void ShowHUD()
        {
            HideAllPanels();
            hudPanel.SetActive(true);
            
            Time.timeScale = 1f;
            
            UpdateHUD();
        }
        
        public void ShowPauseMenu()
        {
            pausePanel.SetActive(true);
            
            Time.timeScale = 0f;
        }
        
        public void HidePauseMenu()
        {
            pausePanel.SetActive(false);
            
            Time.timeScale = 1f;
        }
        
        public void ShowGameOver(int finalScore, int highScore)
        {
            HideAllPanels();
            gameOverPanel.SetActive(true);
            
            finalScoreText.text = "Score: " + finalScore.ToString();
            highScoreText.text = "High Score: " + highScore.ToString();
            
            Time.timeScale = 0f;
        }
        
        public void ShowSkillTree(SkillTree skillTree)
        {
            this.skillTree = skillTree;
            skillTreePanel.SetActive(true);
            
            Time.timeScale = 0f;
            
            GenerateSkillTreeUI();
        }
        
        public void HideSkillTree()
        {
            skillTreePanel.SetActive(false);
            
            if (!pausePanel.activeSelf && !gameOverPanel.activeSelf && !mainMenuPanel.activeSelf && !inventoryPanel.activeSelf && !settingsPanel.activeSelf)
            {
                Time.timeScale = 1f;
            }
        }
        
        public void ShowInventory()
        {
            inventoryPanel.SetActive(true);
            
            Time.timeScale = 0f;
        }
        
        public void HideInventory()
        {
            inventoryPanel.SetActive(false);
            
            if (!pausePanel.activeSelf && !gameOverPanel.activeSelf && !mainMenuPanel.activeSelf && !skillTreePanel.activeSelf && !settingsPanel.activeSelf)
            {
                Time.timeScale = 1f;
            }
        }
        
        public void ShowSettings()
        {
            settingsPanel.SetActive(true);
            
            Time.timeScale = 0f;
        }
        
        public void HideSettings()
        {
            settingsPanel.SetActive(false);
            
            if (!pausePanel.activeSelf && !gameOverPanel.activeSelf && !mainMenuPanel.activeSelf && !skillTreePanel.activeSelf && !inventoryPanel.activeSelf)
            {
                Time.timeScale = 1f;
            }
        }
        
        private void HideAllPanels()
        {
            mainMenuPanel.SetActive(false);
            hudPanel.SetActive(false);
            pausePanel.SetActive(false);
            gameOverPanel.SetActive(false);
            skillTreePanel.SetActive(false);
            inventoryPanel.SetActive(false);
            settingsPanel.SetActive(false);
        }
                
        public void UpdateHUD()
        {
            if (playerModel != null)
            {
                UpdateHealthBar(playerModel.GetStats().currentHealth);
            }
        }
        
        private void UpdateHealthBar(float currentHealth)
        {
            if (healthBar != null && playerModel != null)
            {
                float healthRatio = currentHealth / playerModel.GetStats().maxHealth;
                healthBar.fillAmount = healthRatio;
                healthBar.DOColor(Color.Lerp(Color.red, Color.green, healthRatio), 0.3f);
            }
        }
        
        public void UpdateShieldBar(float currentShield, float maxShield)
        {
            if (shieldBar != null)
            {
                shieldBar.fillAmount = maxShield > 0 ? currentShield / maxShield : 0;
            }
        }
        
        public void UpdateEnergyBar(float currentEnergy, float maxEnergy)
        {
            if (energyBar != null)
            {
                energyBar.fillAmount = maxEnergy > 0 ? currentEnergy / maxEnergy : 0;
            }
        }
        
        public void UpdateScore(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = "Score: " + score.ToString();
                
                scoreText.transform.DOScale(1.2f, 0.1f).OnComplete(() => {
                    scoreText.transform.DOScale(1f, 0.1f);
                });
            }
        }
        
        public void UpdateAmmo(int currentAmmo, int maxAmmo)
        {
            if (ammoText != null)
            {
                ammoText.text = currentAmmo + " / " + maxAmmo;
            }
        }
        
        public void UpdateWaveText(int currentWave)
        {
            if (waveText != null)
            {
                waveText.text = "Wave: " + currentWave.ToString();
                
                waveText.transform.DOScale(1.5f, 0.3f).OnComplete(() => {
                    waveText.transform.DOScale(1f, 0.3f);
                });
            }
        }
        
        public void UpdateWeaponIcon(Sprite icon)
        {
            if (weaponIcon != null)
            {
                weaponIcon.sprite = icon;
                
                weaponIcon.transform.DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360).From();
            }
        }
        
        public void UpdateObjectivesText(string objectives)
        {
            if (objectivesText != null)
            {
                objectivesText.text = objectives;
            }
        }
        
        
        private void GenerateSkillTreeUI()
        {
            if (skillTree == null || skillTreeContainer == null || skillNodePrefab == null)
                return;
                
            foreach (Transform child in skillTreeContainer)
            {
                Destroy(child.gameObject);
            }
            skillNodeUIElements.Clear();
            
            if (skillPointsText != null)
            {
                skillPointsText.text = "Skill Points: " + skillTree.SkillPoints.ToString();
            }
            
            List<SkillNode> allNodes = skillTree.GetAllNodes();
            
            Dictionary<string, Vector2> nodePositions = CalculateNodePositions(skillTree);
            
            foreach (SkillNode node in allNodes)
            {
                GameObject nodeUI = Instantiate(skillNodePrefab, skillTreeContainer);
                
                if (nodePositions.TryGetValue(node.Data.id, out Vector2 position))
                {
                    nodeUI.GetComponent<RectTransform>().anchoredPosition = position;
                }
                
                ConfigureSkillNodeUI(nodeUI, node);
                
                skillNodeUIElements[node.Data.id] = nodeUI;
            }
            
            DrawNodeConnections();
        }
        
        private Dictionary<string, Vector2> CalculateNodePositions(SkillTree tree)
        {
            Dictionary<string, Vector2> positions = new Dictionary<string, Vector2>();
            Dictionary<string, int> nodeDepths = new Dictionary<string, int>();
            Dictionary<int, List<string>> nodesAtDepth = new Dictionary<int, List<string>>();
            
            CalculateNodeDepths(tree, nodeDepths, nodesAtDepth);
            
            int maxDepth = nodesAtDepth.Count > 0 ? nodesAtDepth.Count - 1 : 0;
            
            float verticalSpacing = 100f;
            float horizontalSpacing = 150f;
            
            foreach (var depthPair in nodesAtDepth)
            {
                int depth = depthPair.Key;
                List<string> nodeIds = depthPair.Value;
                
                float yPos = -depth * verticalSpacing;
                
                for (int i = 0; i < nodeIds.Count; i++)
                {
                    float xOffset = (nodeIds.Count - 1) * horizontalSpacing * -0.5f;
                    float xPos = xOffset + i * horizontalSpacing;
                    
                    positions[nodeIds[i]] = new Vector2(xPos, yPos);
                }
            }
            
            return positions;
        }
        
        private void CalculateNodeDepths(SkillTree tree, Dictionary<string, int> nodeDepths, Dictionary<int, List<string>> nodesAtDepth)
        {
            List<SkillNode> rootNodes = tree.GetRootNodes();
            
            foreach (var node in rootNodes)
            {
                CalculateNodeDepthRecursive(node, 0, nodeDepths, nodesAtDepth);
            }
        }
        
        private void CalculateNodeDepthRecursive(SkillNode node, int depth, Dictionary<string, int> nodeDepths, Dictionary<int, List<string>> nodesAtDepth)
        {
            string nodeId = node.Data.id;
            
            if (nodeDepths.TryGetValue(nodeId, out int existingDepth) && existingDepth >= depth)
            {
                return;
            }
            
            nodeDepths[nodeId] = depth;
            
            if (!nodesAtDepth.ContainsKey(depth))
            {
                nodesAtDepth[depth] = new List<string>();
            }
            
            if (!nodesAtDepth[depth].Contains(nodeId))
            {
                nodesAtDepth[depth].Add(nodeId);
            }
            
            foreach (var child in node.GetChildren())
            {
                CalculateNodeDepthRecursive(child, depth + 1, nodeDepths, nodesAtDepth);
            }
        }
        
        private void ConfigureSkillNodeUI(GameObject nodeUI, SkillNode node)
        {
            SkillNodeUI ui = nodeUI.GetComponent<SkillNodeUI>();
            if (ui != null)
            {
                ui.Initialize(node, this);
            }
        }
        
        private void DrawNodeConnections()
        {
            foreach (var nodePair in skillNodeUIElements)
            {
                string nodeId = nodePair.Key;
                GameObject nodeUI = nodePair.Value;
                
                SkillNode node = skillTree.GetNode(nodeId);
                if (node == null) continue;
                
                List<SkillNode> children = node.GetChildren();
                
                foreach (var child in children)
                {
                    if (skillNodeUIElements.TryGetValue(child.Data.id, out GameObject childUI))
                    {
                        DrawConnection(nodeUI.GetComponent<RectTransform>(), childUI.GetComponent<RectTransform>(), node.IsUnlocked && child.IsUnlocked);
                    }
                }
            }
        }
        
        private void DrawConnection(RectTransform from, RectTransform to, bool isActive)
        {
            GameObject lineObj = new GameObject("Connection");
            lineObj.transform.SetParent(skillTreeContainer);
            
            Image line = lineObj.AddComponent<Image>();
            line.color = isActive ? Color.green : Color.gray;
            
            Vector2 fromPos = from.anchoredPosition;
            Vector2 toPos = to.anchoredPosition;
            
            Vector2 direction = toPos - fromPos;
            float distance = direction.magnitude;
            
            line.rectTransform.anchoredPosition = fromPos + direction * 0.5f;
            line.rectTransform.sizeDelta = new Vector2(distance, 2f);
            line.rectTransform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }
        
        public void TryUnlockSkillNode(string nodeId)
        {
            if (skillTree.UnlockNode(nodeId))
            {
                UpdateSkillNodeUI(nodeId);
                UpdateSkillPointsText();
                
                DrawNodeConnections();
            }
        }
        
        private void UpdateSkillNodeUI(string nodeId)
        {
            if (skillNodeUIElements.TryGetValue(nodeId, out GameObject nodeUI))
            {
                SkillNode node = skillTree.GetNode(nodeId);
                if (node != null)
                {
                    SkillNodeUI ui = nodeUI.GetComponent<SkillNodeUI>();
                    if (ui != null)
                    {
                        ui.UpdateUI();
                    }
                }
            }
        }
        
        private void UpdateSkillPointsText()
        {
            if (skillPointsText != null && skillTree != null)
            {
                skillPointsText.text = "Skill Points: " + skillTree.SkillPoints.ToString();
            }
        }
        
        
        public void OnStartButtonClicked()
        {
            ShowHUD();
        }
        
        public void OnResumeButtonClicked()
        {
            HidePauseMenu();
        }
        
        public void OnPauseButtonClicked()
        {
            ShowPauseMenu();
        }
        
        public void OnSettingsButtonClicked()
        {
            ShowSettings();
        }
        
        public void OnBackToMainMenuButtonClicked()
        {
            ShowMainMenu();
        }
        
        public void OnRestartButtonClicked()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
        
        public void OnQuitButtonClicked()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
        
        private void OnDestroy()
        {
            if (playerModel != null)
            {
                playerModel.OnHealthChanged -= UpdateHealthBar;
            }
        }
    }
}