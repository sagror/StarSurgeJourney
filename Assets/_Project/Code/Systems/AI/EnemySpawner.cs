using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace StarSurgeJourney.Systems.AI
{
    public enum EnemyType
    {
        Fighter,
        Bomber,
        Scout,
        Elite,
        Boss
    }
    
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Spawn configuration")]
        [SerializeField] private float initialSpawnDelay = 3f;
        [SerializeField] private float minSpawnInterval = 5f;
        [SerializeField] private float maxSpawnInterval = 15f;
        [SerializeField] private int maxEnemiesAlive = 10;
        [SerializeField] private float spawnDistance = 200f;
        [SerializeField] private bool spawnOnStart = true;
        
        [Header("Enemies Prefabs")]
        [SerializeField] private GameObject fighterPrefab;
        [SerializeField] private GameObject bomberPrefab;
        [SerializeField] private GameObject scoutPrefab;
        [SerializeField] private GameObject elitePrefab;
        [SerializeField] private GameObject bossPrefab;
        
        [Header("Probabilities")]
        [SerializeField] private float fighterProbability = 0.5f;
        [SerializeField] private float bomberProbability = 0.3f;
        [SerializeField] private float scoutProbability = 0.15f;
        [SerializeField] private float eliteProbability = 0.05f;
        
        private List<GameObject> activeEnemies = new List<GameObject>();
        
        private Transform playerTransform;
        
        private Coroutine spawnCoroutine;
        
        private void Start()
        {
            // Busca al jugador
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            
            if (spawnOnStart)
            {
                StartSpawning();
            }
        }
        
        public void StartSpawning()
        {
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
            }
            
            spawnCoroutine = StartCoroutine(SpawnRoutine());
        }
        
        public void StopSpawning()
        {
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }
        }
        
        private IEnumerator SpawnRoutine()
        {
            yield return new WaitForSeconds(initialSpawnDelay);
            
            while (true)
            {
                activeEnemies.RemoveAll(enemy => enemy == null);
                
                if (activeEnemies.Count < maxEnemiesAlive)
                {
                    SpawnEnemy();
                }
                
                float interval = Random.Range(minSpawnInterval, maxSpawnInterval);
                yield return new WaitForSeconds(interval);
            }
        }
        
        private void SpawnEnemy()
        {
            if (playerTransform == null)
                return;
                
            EnemyType enemyType = DetermineEnemyType();
            
            Vector3 spawnPosition = CalculateSpawnPosition();
            
            GameObject enemyPrefab = GetEnemyPrefab(enemyType);
            if (enemyPrefab != null)
            {
                GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                activeEnemies.Add(enemy);
                
                AIController aiController = enemy.GetComponent<AIController>();
                if (aiController != null)
                {
                    aiController.SetTarget(playerTransform);
                }
            }
        }
        
        private Vector3 CalculateSpawnPosition()
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnDistance;
            Vector3 offset = new Vector3(randomCircle.x, 0, randomCircle.y);
            
            return playerTransform.position + offset;
        }
        
        private EnemyType DetermineEnemyType()
        {
            float roll = Random.value;
            float cumulativeProbability = 0f;
            
            cumulativeProbability += fighterProbability;
            if (roll < cumulativeProbability)
                return EnemyType.Fighter;
                
            cumulativeProbability += bomberProbability;
            if (roll < cumulativeProbability)
                return EnemyType.Bomber;
                
            cumulativeProbability += scoutProbability;
            if (roll < cumulativeProbability)
                return EnemyType.Scout;
                
            cumulativeProbability += eliteProbability;
            if (roll < cumulativeProbability)
                return EnemyType.Elite;
                
            return EnemyType.Boss;
        }
        
        private GameObject GetEnemyPrefab(EnemyType type)
        {
            switch (type)
            {
                case EnemyType.Fighter:
                    return fighterPrefab;
                case EnemyType.Bomber:
                    return bomberPrefab;
                case EnemyType.Scout:
                    return scoutPrefab;
                case EnemyType.Elite:
                    return elitePrefab;
                case EnemyType.Boss:
                    return bossPrefab;
                default:
                    return fighterPrefab;
            }
        }
        
        public GameObject SpawnBoss(Vector3 position)
        {
            if (bossPrefab != null)
            {
                GameObject boss = Instantiate(bossPrefab, position, Quaternion.identity);
                activeEnemies.Add(boss);
                
                // Configuración especial para el jefe
                // ...
                
                return boss;
            }
            
            return null;
        }
        
        public void SpawnWave(int numFighters, int numBombers, int numScouts, Vector3 centerPosition, float radius)
        {
            for (int i = 0; i < numFighters; i++)
            {
                SpawnSpecificEnemy(EnemyType.Fighter, CalculatePositionInRadius(centerPosition, radius));
            }
            
            for (int i = 0; i < numBombers; i++)
            {
                SpawnSpecificEnemy(EnemyType.Bomber, CalculatePositionInRadius(centerPosition, radius));
            }
            
            for (int i = 0; i < numScouts; i++)
            {
                SpawnSpecificEnemy(EnemyType.Scout, CalculatePositionInRadius(centerPosition, radius));
            }
        }
        
        private Vector3 CalculatePositionInRadius(Vector3 center, float radius)
        {
            Vector2 randomCircle = Random.insideUnitCircle * radius;
            return center + new Vector3(randomCircle.x, 0, randomCircle.y);
        }
        
        private GameObject SpawnSpecificEnemy(EnemyType type, Vector3 position)
        {
            GameObject enemyPrefab = GetEnemyPrefab(type);
            if (enemyPrefab != null)
            {
                GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
                activeEnemies.Add(enemy);
                
                AIController aiController = enemy.GetComponent<AIController>();
                if (aiController != null && playerTransform != null)
                {
                    aiController.SetTarget(playerTransform);
                }
                
                return enemy;
            }
            
            return null;
        }
    }
}