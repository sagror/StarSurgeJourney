using System.Collections.Generic;
using UnityEngine;
using System;

namespace StarSurgeJourney.Systems.Skills
{
    [Serializable]
    public class SkillNodeData
    {
        public string id;
        public string name;
        public string description;
        public Sprite icon;
        public int cost = 1;
        public List<string> requirements = new List<string>();
        public int maxLevel = 1;
    }
    
    // Base class for skill tree nodes
    public class SkillNode
    {
        public SkillNodeData Data { get; private set; }
        public int CurrentLevel { get; private set; }
        public bool IsUnlocked => CurrentLevel > 0;
        public int MaxLevel => Data.cost;
        public bool CanLevelUp => CurrentLevel < MaxLevel;
        
        private List<SkillNode> children = new List<SkillNode>();
        private List<SkillNode> parents = new List<SkillNode>();
        
        // Events
        public event Action<SkillNode> OnNodeUnlocked;
        public event Action<SkillNode, int> OnNodeLevelChanged;
        
        public SkillNode(SkillNodeData data)
        {
            Data = data;
            CurrentLevel = 0;
        }
        
        public void AddChild(SkillNode child)
        {
            if (!children.Contains(child))
            {
                children.Add(child);
                child.AddParent(this);
            }
        }
        
        public void AddParent(SkillNode parent)
        {
            if (!parents.Contains(parent))
            {
                parents.Add(parent);
            }
        }
        
        public List<SkillNode> GetChildren()
        {
            return new List<SkillNode>(children);
        }
        
        public List<SkillNode> GetParents()
        {
            return new List<SkillNode>(parents);
        }
        
        public bool CanUnlock()
        {
            if (IsUnlocked)
                return false;
                
            foreach (var parent in parents)
            {
                if (!parent.IsUnlocked)
                    return false;
            }
            
            return true;
        }
        
        public bool Unlock()
        {
            if (!CanUnlock())
                return false;
                
            CurrentLevel = 1;
            OnNodeUnlocked?.Invoke(this);
            OnNodeLevelChanged?.Invoke(this, CurrentLevel);
            
            return true;
        }
        
        public bool LevelUp()
        {
            if (!IsUnlocked || !CanLevelUp)
                return false;
                
            CurrentLevel++;
            OnNodeLevelChanged?.Invoke(this, CurrentLevel);
            
            return true;
        }
        
        public virtual void ApplyEffects(GameObject target)
        {
            Debug.Log($"Aplying efects of {Data.name} level {CurrentLevel} to {target.name}");
        }
    }
}