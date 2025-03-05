using UnityEngine;
using System.Collections.Generic;

namespace StarSurgeJourney.Systems.Generation
{
    public enum ObjectiveType
    {
        DestroyEnemies,
        CollectItems,
        SurviveTime,
        ReachLocation,
        DefendTarget,
        EscortAlly
    }
    
    [System.Serializable]
    public class LevelObjective
    {
        public ObjectiveType type;
        public string description;
        public int targetAmount;
        public int currentAmount;
        public bool isCompleted;
        public float timeLimit;
        
        public Vector3 targetLocation;
        public float reachDistance;
        
        public GameObject targetObject;
        
        public bool UpdateProgress(int amount)
        {
            currentAmount = Mathf.Min(currentAmount + amount, targetAmount);
            isCompleted = currentAmount >= targetAmount;
            return isCompleted;
        }
    }
    
    public class LevelObjectiveManager : MonoBehaviour
    {
        [SerializeField] private List<LevelObjective> currentObjectives = new List<LevelObjective>();
        
        public System.Action<LevelObjective> OnObjectiveUpdated;
        public System.Action<LevelObjective> OnObjectiveCompleted;
        public System.Action OnAllObjectivesCompleted;
        
        public void GenerateObjectives(float difficulty)
        {
            currentObjectives.Clear();
            
            int numObjectives = Mathf.FloorToInt(1 + difficulty / 2);
            numObjectives = Mathf.Clamp(numObjectives, 1, 4);
            
            for (int i = 0; i < numObjectives; i++)
            {
                LevelObjective objective = GenerateRandomObjective(difficulty);
                currentObjectives.Add(objective);
            }
        }
        
        private LevelObjective GenerateRandomObjective(float difficulty, int depth = 0, int maxDepth = 2)
        {
            if (depth >= maxDepth)
            {
                return GenerateSimpleObjective(difficulty);
            }
            
            if (Random.value < 0.3f && depth < maxDepth - 1)
            {
                LevelObjective objective = GenerateSimpleObjective(difficulty * 1.2f);
                
                return objective;
            }
            else
            {
                return GenerateSimpleObjective(difficulty);
            }
        }
        
        private LevelObjective GenerateSimpleObjective(float difficulty)
        {
            ObjectiveType type = (ObjectiveType)Random.Range(0, System.Enum.GetValues(typeof(ObjectiveType)).Length);
            
            LevelObjective objective = new LevelObjective();
            objective.type = type;
            
            switch (type)
            {
                case ObjectiveType.DestroyEnemies:
                    int enemyCount = Mathf.RoundToInt(5 * difficulty);
                    objective.targetAmount = Mathf.Clamp(enemyCount, 3, 30);
                    objective.description = $"Destroy {objective.targetAmount} enemies";
                    break;
                    
                case ObjectiveType.CollectItems:
                    int itemCount = Mathf.RoundToInt(7 * difficulty);
                    objective.targetAmount = Mathf.Clamp(itemCount, 5, 20);
                    objective.description = $"Collect {objective.targetAmount} resources";
                    break;
                    
                case ObjectiveType.SurviveTime:
                    float minutes = 1f + difficulty * 0.5f;
                    minutes = Mathf.Clamp(minutes, 1f, 5f);
                    objective.timeLimit = minutes * 60f;
                    objective.description = $"Survive {minutes:0.0} minutes";
                    break;
                    
                case ObjectiveType.ReachLocation:
                    objective.targetLocation = Random.insideUnitSphere * 500f;
                    objective.reachDistance = 50f;
                    objective.description = "Reach marked navigation point";
                    break;
                    
                case ObjectiveType.DefendTarget:
                    float defenseTime = 2f + difficulty * 0.3f;
                    defenseTime = Mathf.Clamp(defenseTime, 2f, 4f);
                    objective.timeLimit = defenseTime * 60f;
                    objective.description = $"Defend the station for {defenseTime:0.0} minutes";
                    break;
                    
                case ObjectiveType.EscortAlly:
                    objective.description = "Escort ally ship to its destination";
                    break;
            }
            
            objective.currentAmount = 0;
            objective.isCompleted = false;
            
            return objective;
        }
        
        public void UpdateObjective(ObjectiveType type, int amount = 1)
        {
            bool allCompleted = true;
            
            foreach (var objective in currentObjectives)
            {
                if (objective.type == type && !objective.isCompleted)
                {
                    if (objective.UpdateProgress(amount))
                    {
                        OnObjectiveCompleted?.Invoke(objective);
                    }
                    
                    OnObjectiveUpdated?.Invoke(objective);
                }
                
                allCompleted = allCompleted && objective.isCompleted;
            }
            
            if (allCompleted && currentObjectives.Count > 0)
            {
                OnAllObjectivesCompleted?.Invoke();
            }
        }
        
        public void ReachLocation(Vector3 playerPosition)
        {
            foreach (var objective in currentObjectives)
            {
                if (objective.type == ObjectiveType.ReachLocation && !objective.isCompleted)
                {
                    float distance = Vector3.Distance(playerPosition, objective.targetLocation);
                    if (distance <= objective.reachDistance)
                    {
                        objective.isCompleted = true;
                        OnObjectiveCompleted?.Invoke(objective);
                        OnObjectiveUpdated?.Invoke(objective);
                    }
                }
            }
            
            CheckAllObjectivesCompleted();
        }
        
        public void UpdateTimeBasedObjectives(float deltaTime)
        {
            foreach (var objective in currentObjectives)
            {
                if ((objective.type == ObjectiveType.SurviveTime || 
                     objective.type == ObjectiveType.DefendTarget) && 
                    !objective.isCompleted)
                {
                    objective.currentAmount += Mathf.FloorToInt(deltaTime);
                    if (objective.currentAmount >= objective.timeLimit)
                    {
                        objective.isCompleted = true;
                        OnObjectiveCompleted?.Invoke(objective);
                    }
                    
                    OnObjectiveUpdated?.Invoke(objective);
                }
            }
            
            CheckAllObjectivesCompleted();
        }
        
        private void CheckAllObjectivesCompleted()
        {
            bool allCompleted = currentObjectives.Count > 0;
            
            foreach (var objective in currentObjectives)
            {
                allCompleted = allCompleted && objective.isCompleted;
            }
            
            if (allCompleted)
            {
                OnAllObjectivesCompleted?.Invoke();
            }
        }
        
        public List<LevelObjective> GetCurrentObjectives()
        {
            return new List<LevelObjective>(currentObjectives);
        }
    }
}