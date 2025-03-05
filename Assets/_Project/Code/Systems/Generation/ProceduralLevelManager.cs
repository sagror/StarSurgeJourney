using UnityEngine;
using System.Collections.Generic;

namespace StarSurgeJourney.Systems.Generation
{
    public class ProceduralLevelManager : MonoBehaviour
    {
        [Header("Configuración")]
        [SerializeField] private bool generateOnStart = true;
        [SerializeField] private int initialLevelSeed = 0;
        [SerializeField] private float levelDifficulty = 1.0f;
        [SerializeField] private float difficultyIncrease = 0.2f;
        
        [Header("Referencias")]
        [SerializeField] private ProceduralGenerator generator;
        
        private int currentLevelIndex = 0;
        private int currentLevelSeed;
        
        private List<LevelData> generatedLevels = new List<LevelData>();
        
        private LevelObjectiveManager objectiveManager;
        
        private void Awake()
        {
            if (generator == null)
            {
                generator = GetComponent<ProceduralGenerator>();
                if (generator == null)
                {
                    generator = gameObject.AddComponent<ProceduralGenerator>();
                }
            }
            
            objectiveManager = GetComponent<LevelObjectiveManager>();
            if (objectiveManager == null)
            {
                objectiveManager = gameObject.AddComponent<LevelObjectiveManager>();
            }
        }
        
        private void Start()
        {
            if (generateOnStart)
            {
                GenerateInitialLevel();
            }
        }
        
        public void GenerateInitialLevel()
        {
            currentLevelIndex = 0;
            currentLevelSeed = initialLevelSeed;
            levelDifficulty = 1.0f;
            
            GenerateLevel();
        }
        
        public void GenerateNextLevel()
        {
            currentLevelIndex++;
            currentLevelSeed = GenerateNextSeed();
            levelDifficulty += difficultyIncrease;
            
            GenerateLevel();
        }
        
        public void ReturnToPreviousLevel()
        {
            if (currentLevelIndex > 0)
            {
                currentLevelIndex--;
                LevelData previousLevel = generatedLevels[currentLevelIndex];
                currentLevelSeed = previousLevel.seed;
                levelDifficulty = previousLevel.difficulty;
                
                GenerateLevel(false);
            }
        }
        
        private void GenerateLevel(bool isNewLevel = true)
        {
            //generator.SetSeed(currentLevelSeed); TBD
            
            generator.GenerateSystem();
            
            objectiveManager.GenerateObjectives(levelDifficulty);
            
            if (isNewLevel)
            {
                LevelData levelData = new LevelData
                {
                    index = currentLevelIndex,
                    seed = currentLevelSeed,
                    difficulty = levelDifficulty
                };
                
                if (currentLevelIndex < generatedLevels.Count)
                {
                    generatedLevels.RemoveRange(currentLevelIndex, generatedLevels.Count - currentLevelIndex);
                }
                
                generatedLevels.Add(levelData);
            }
        }
        
        private int GenerateNextSeed()
        {
            System.Random random = new System.Random(currentLevelSeed);
            return random.Next(0, int.MaxValue);
        }
        
        private class LevelData
        {
            public int index;
            public int seed;
            public float difficulty;
        }
    }
}