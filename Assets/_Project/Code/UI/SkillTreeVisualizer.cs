using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using StarSurgeJourney.Systems.Skills;
using TMPro;

namespace StarSurgeJourney.UI
{
    public class SkillTreeVisualizer : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private float nodeWidth = 120f;
        [SerializeField] private float nodeHeight = 120f;
        [SerializeField] private float horizontalSpacing = 150f;
        [SerializeField] private float verticalSpacing = 100f;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject skillNodePrefab;
        [SerializeField] private GameObject connectionPrefab;
        
        [Header("Containers")]
        [SerializeField] private RectTransform nodesContainer;
        [SerializeField] private RectTransform connectionsContainer;
        
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI skillPointsText;
        [SerializeField] private Button closeButton;
        
        private SkillTree skillTree;
        
        private Dictionary<string, GameObject> nodeUIElements = new Dictionary<string, GameObject>();
        
        private void Start()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnCloseButtonClicked);
            }
        }
        
        public void Initialize(SkillTree tree)
        {
            this.skillTree = tree;
            
            ClearUI();
            
            GenerateVisualTree();
            
            UpdateSkillPointsText();
            
            skillTree.OnSkillPointsChanged += UpdateSkillPointsText;
            skillTree.OnNodeAdded += (node) => UpdateNodeUI(node.Data.id);
        }
        
        private void ClearUI()
        {
            foreach (Transform child in nodesContainer)
            {
                Destroy(child.gameObject);
            }
            
            foreach (Transform child in connectionsContainer)
            {
                Destroy(child.gameObject);
            }
            
            nodeUIElements.Clear();
        }
        
        private void GenerateVisualTree()
        {
            if (skillTree == null) return;
            
            Dictionary<string, Vector2> nodePositions = CalculateNodePositions();
            
            foreach (SkillNode node in skillTree.GetAllNodes())
            {
                CreateNodeUI(node, nodePositions[node.Data.id]);
            }
            
            CreateConnections();
        }
        
        private Dictionary<string, Vector2> CalculateNodePositions()
        {
            Dictionary<string, Vector2> positions = new Dictionary<string, Vector2>();
            Dictionary<string, int> nodeDepths = new Dictionary<string, int>();
            Dictionary<int, List<string>> nodesAtDepth = new Dictionary<int, List<string>>();
            
            CalculateNodeDepths(nodeDepths, nodesAtDepth);
            
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
        
        private void CalculateNodeDepths(Dictionary<string, int> nodeDepths, Dictionary<int, List<string>> nodesAtDepth)
        {
            List<SkillNode> rootNodes = skillTree.GetRootNodes();
            
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
        
        private void CreateNodeUI(SkillNode node, Vector2 position)
        {
            if (skillNodePrefab == null) return;
            
            GameObject nodeUI = Instantiate(skillNodePrefab, nodesContainer);
            
            RectTransform rectTransform = nodeUI.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = position;
            rectTransform.sizeDelta = new Vector2(nodeWidth, nodeHeight);
            
            SkillNodeUI nodeUIComponent = nodeUI.GetComponent<SkillNodeUI>();
            if (nodeUIComponent != null)
            {
                nodeUIComponent.Initialize(node, null);
                
                Button button = nodeUI.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => OnNodeClicked(node));
                }
            }
            
            nodeUIElements[node.Data.id] = nodeUI;
        }
        
        private void CreateConnections()
        {
            foreach (var nodePair in nodeUIElements)
            {
                string nodeId = nodePair.Key;
                GameObject nodeUI = nodePair.Value;
                
                SkillNode node = skillTree.GetNode(nodeId);
                if (node == null) continue;
                
                foreach (var child in node.GetChildren())
                {
                    if (nodeUIElements.TryGetValue(child.Data.id, out GameObject childUI))
                    {
                        CreateConnection(nodeUI.GetComponent<RectTransform>(), childUI.GetComponent<RectTransform>(), node.IsUnlocked && child.IsUnlocked);
                    }
                }
            }
        }
        
        private void CreateConnection(RectTransform fromNode, RectTransform toNode, bool isActive)
        {
            if (connectionPrefab == null) return;
            
            GameObject connectionObj = Instantiate(connectionPrefab, connectionsContainer);
            
            Vector2 fromPos = fromNode.anchoredPosition;
            Vector2 toPos = toNode.anchoredPosition;
            
            Vector2 direction = toPos - fromPos;
            float distance = direction.magnitude;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            RectTransform connectionRect = connectionObj.GetComponent<RectTransform>();
            connectionRect.anchoredPosition = fromPos + direction * 0.5f;
            connectionRect.sizeDelta = new Vector2(distance, 4f);
            connectionRect.localRotation = Quaternion.Euler(0, 0, angle);
            
            Image connectionImage = connectionObj.GetComponent<Image>();
            if (connectionImage != null)
            {
                connectionImage.color = isActive ? Color.green : Color.gray;
            }            
        }
        
        public void UpdateNodeUI(string nodeId)
        {
            if (nodeUIElements.TryGetValue(nodeId, out GameObject nodeUI))
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
            
            UpdateConnections();
        }
        
        private void UpdateConnections()
        {
            foreach (Transform child in connectionsContainer)
            {
                Destroy(child.gameObject);
            }
            
            CreateConnections();
        }
        
        private void UpdateSkillPointsText(int points = -1)
        {
            if (skillPointsText != null)
            {
                int pointsToShow = points >= 0 ? points : skillTree.SkillPoints;
                skillPointsText.text = "Skill Points: " + pointsToShow.ToString();
            }
        }
        
        private void OnNodeClicked(SkillNode node)
        {
            if (node.IsUnlocked)
            {
                if (node.CanLevelUp && skillTree.SkillPoints >= node.Data.cost)
                {
                    skillTree.LevelUpNode(node.Data.id);
                }
            }
            else
            {
                if (node.CanUnlock() && skillTree.SkillPoints >= node.Data.cost)
                {
                    skillTree.UnlockNode(node.Data.id);
                }
            }
            
            UpdateNodeUI(node.Data.id);
        }
        
        private void OnCloseButtonClicked()
        {
            gameObject.SetActive(false);
        }
        
        private void OnDestroy()
        {
            if (skillTree != null)
            {
                skillTree.OnSkillPointsChanged -= UpdateSkillPointsText;
            }
        }
    }
}