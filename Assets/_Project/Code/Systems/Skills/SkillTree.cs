using System.Collections.Generic;
using UnityEngine;
using System;

namespace StarSurgeJourney.Systems.Skills
{
    // Class to manage the entire skill tree
    public class SkillTree
    {
        private Dictionary<string, SkillNode> nodes = new Dictionary<string, SkillNode>();
        private List<SkillNode> rootNodes = new List<SkillNode>();
        private int skillPoints = 0;
        
        // Events
        public event Action<int> OnSkillPointsChanged;
        public event Action<SkillNode> OnNodeAdded;
        
        public int SkillPoints
        {
            get => skillPoints;
            set
            {
                if (skillPoints != value)
                {
                    skillPoints = value;
                    OnSkillPointsChanged?.Invoke(skillPoints);
                }
            }
        }
        
        public SkillTree()
        {

        }
        
        public void AddNode(SkillNode node, bool isRoot = false)
        {
            if (!nodes.ContainsKey(node.Data.id))
            {
                nodes.Add(node.Data.id, node);
                OnNodeAdded?.Invoke(node);
                
                if (isRoot)
                {
                    rootNodes.Add(node);
                }
            }
        }
        
        public void ConnectNodes(string parentId, string childId)
        {
            if (nodes.TryGetValue(parentId, out SkillNode parent) && 
                nodes.TryGetValue(childId, out SkillNode child))
            {
                parent.AddChild(child);
            }
        }
        
        public bool UnlockNode(string nodeId)
        {
            if (nodes.TryGetValue(nodeId, out SkillNode node))
            {
                if (node.CanUnlock() && skillPoints >= node.Data.cost)
                {
                    bool success = node.Unlock();
                    if (success)
                    {
                        SkillPoints -= node.Data.cost;
                        return true;
                    }
                }
            }
            
            return false;
        }
        
        public bool LevelUpNode(string nodeId)
        {
            if (nodes.TryGetValue(nodeId, out SkillNode node))
            {
                if (node.CanLevelUp && skillPoints >= node.Data.cost)
                {
                    bool success = node.LevelUp();
                    if (success)
                    {
                        SkillPoints -= node.Data.cost;
                        return true;
                    }
                }
            }
            
            return false;
        }
        
        public SkillNode GetNode(string nodeId)
        {
            nodes.TryGetValue(nodeId, out SkillNode node);
            return node;
        }
        
        public List<SkillNode> GetAllNodes()
        {
            return new List<SkillNode>(nodes.Values);
        }
        
        public List<SkillNode> GetRootNodes()
        {
            return new List<SkillNode>(rootNodes);
        }
        
        public void ApplyAllEffects(GameObject target)
        {
            foreach (var node in nodes.Values)
            {
                if (node.IsUnlocked)
                {
                    node.ApplyEffects(target);
                }
            }
        }
        
        public void TraverseTree(Action<SkillNode, int> action, bool unlockedOnly = false)
        {
            foreach (var rootNode in rootNodes)
            {
                TraverseNode(rootNode, action, 0, unlockedOnly);
            }
        }
        
        private void TraverseNode(SkillNode node, Action<SkillNode, int> action, int depth, bool unlockedOnly)
        {
            if (!unlockedOnly || node.IsUnlocked)
            {
                action(node, depth);
            }
            
            foreach (var child in node.GetChildren())
            {
                TraverseNode(child, action, depth + 1, unlockedOnly);
            }
        }
    }
}