using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StarSurgeJourney.Systems.Skills;

namespace StarSurgeJourney.UI
{
    public class SkillNodeUI : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Button nodeButton;
        
        [Header("Visual states")]
        [SerializeField] private Color lockedColor = Color.gray;
        [SerializeField] private Color unlockedColor = Color.green;
        [SerializeField] private Color maxLevelColor = Color.yellow;
        [SerializeField] private Color unavailableColor = Color.red;
        
        private SkillNode node;
        private UIManager uiManager;
        
        public void Initialize(SkillNode node, UIManager uiManager)
        {
            this.node = node;
            this.uiManager = uiManager;
            
            if (nameText != null)
            {
                nameText.text = node.Data.name;
            }
            
            if (iconImage != null && node.Data.icon != null)
            {
                iconImage.sprite = node.Data.icon;
            }
            
            if (nodeButton != null)
            {
                nodeButton.onClick.AddListener(OnNodeClicked);
            }
            
            UpdateUI();
        }
        
        public void UpdateUI()
        {
            if (node == null) return;
            
            if (levelText != null)
            {
                levelText.text = node.CurrentLevel + " / " + node.MaxLevel;
            }
            
            if (backgroundImage != null)
            {
                Color color;
                
                if (node.IsUnlocked)
                {
                    if (node.CurrentLevel >= node.MaxLevel)
                    {
                        color = maxLevelColor;
                    }
                    else
                    {
                        color = unlockedColor;
                    }
                }
                else if (node.CanUnlock())
                {
                    color = unlockedColor * 0.7f;
                }
                else
                {
                    color = lockedColor;
                }
                
                backgroundImage.color = color;
            }
            
            if (nodeButton != null)
            {
                bool canInteract = !node.IsUnlocked || (node.IsUnlocked && node.CanLevelUp);
                nodeButton.interactable = canInteract;
            }
        }
        
        private void OnNodeClicked()
        {
            if (uiManager != null)
            {
                uiManager.TryUnlockSkillNode(node.Data.id);
            }
        }
        
        public void ShowTooltip()
        {

        }
        
        public void HideTooltip()
        {

        }
    }
}