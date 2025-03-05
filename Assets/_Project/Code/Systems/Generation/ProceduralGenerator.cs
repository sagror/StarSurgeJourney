using UnityEngine;
using System.Collections.Generic;

namespace StarSurgeJourney.Systems.Generation
{
    public class ProceduralGenerator : MonoBehaviour
    {
        [Header("Generación de Sistema")]
        [SerializeField] private int seed = 0;
        [SerializeField] private bool useRandomSeed = true;
        [SerializeField] private float systemRadius = 1000f;
        [SerializeField] private int minPlanets = 3;
        [SerializeField] private int maxPlanets = 8;
        [SerializeField] private int minAsteroids = 10;
        [SerializeField] private int maxAsteroids = 30;
        [SerializeField] private int minStations = 1;
        [SerializeField] private int maxStations = 3;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject sunPrefab;
        [SerializeField] private List<GameObject> planetPrefabs;
        [SerializeField] private List<GameObject> asteroidPrefabs;
        [SerializeField] private List<GameObject> stationPrefabs;
        
        private Transform systemRoot;
        private System.Random random;
        
        private void Awake()
        {
            if (useRandomSeed)
            {
                seed = Random.Range(0, int.MaxValue);
            }
            
            random = new System.Random(seed);
            
            GameObject rootObj = new GameObject("GeneratedSystem");
            systemRoot = rootObj.transform;
        }
        
        public void GenerateSystem()
        {
            ClearSystem();
            
            GenerateSun();
            
            int planetCount = random.Next(minPlanets, maxPlanets + 1);
            GeneratePlanets(planetCount);
            
            int asteroidCount = random.Next(minAsteroids, maxAsteroids + 1);
            GenerateAsteroids(asteroidCount);
            
            int stationCount = random.Next(minStations, maxStations + 1);
            GenerateStations(stationCount);
        }
        
        private void ClearSystem()
        {
            while (systemRoot.childCount > 0)
            {
                DestroyImmediate(systemRoot.GetChild(0).gameObject);
            }
        }
        
        private void GenerateSun()
        {
            if (sunPrefab != null)
            {
                GameObject sun = Instantiate(sunPrefab, Vector3.zero, Quaternion.identity);
                sun.transform.SetParent(systemRoot);
                sun.name = "Sun";
                
                if (sun.GetComponent<Light>() == null)
                {
                    Light light = sun.AddComponent<Light>();
                    light.type = LightType.Point;
                    light.color = Color.yellow;
                    light.intensity = 2f;
                    light.range = systemRadius * 2f;
                }
            }
        }
        
        private void GeneratePlanets(int count)
        {
            if (planetPrefabs.Count == 0) return;
            
            for (int i = 0; i < count; i++)
            {
                GameObject prefab = planetPrefabs[random.Next(0, planetPrefabs.Count)];
                
                float distance = RandomRange(50f, systemRadius * 0.8f);
                float angle = RandomRange(0f, 360f);
                Vector3 position = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;
                
                GameObject planet = Instantiate(prefab, position, Quaternion.identity);
                planet.transform.SetParent(systemRoot);
                planet.name = $"Planet_{i+1}";

                float scale = RandomRange(5f, 20f);
                planet.transform.localScale = new Vector3(scale, scale, scale);
            }
        }
        
        private void GenerateAsteroids(int count)
        {
            if (asteroidPrefabs.Count == 0) return;
            
            int numBelts = random.Next(1, 4);
            List<Vector2> beltRanges = new List<Vector2>();
            
            for (int i = 0; i < numBelts; i++)
            {
                float minRadius = RandomRange(systemRadius * 0.3f, systemRadius * 0.7f);
                float beltWidth = RandomRange(20f, 80f);
                beltRanges.Add(new Vector2(minRadius, minRadius + beltWidth));
            }
            
            for (int i = 0; i < count; i++)
            {
                GameObject prefab = asteroidPrefabs[random.Next(0, asteroidPrefabs.Count)];
                
                Vector2 beltRange = beltRanges[random.Next(0, beltRanges.Count)];
                
                float distance = RandomRange(beltRange.x, beltRange.y);
                float angle = RandomRange(0f, 360f);
                float height = RandomRange(-10f, 10f);
                Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
                Vector3 position = direction * distance + Vector3.up * height;
                
                GameObject asteroid = Instantiate(prefab, position, Random.rotation);
                asteroid.transform.SetParent(systemRoot);
                asteroid.name = $"Asteroid_{i+1}";
                
                float scale = RandomRange(1f, 5f);
                asteroid.transform.localScale = new Vector3(scale, scale, scale);
            }
        }
        
        private void GenerateStations(int count)
        {
            if (stationPrefabs.Count == 0) return;
            
            for (int i = 0; i < count; i++)
            {
                GameObject prefab = stationPrefabs[random.Next(0, stationPrefabs.Count)];
                
                float distance = RandomRange(100f, systemRadius * 0.6f);
                float angle = RandomRange(0f, 360f);
                Vector3 position = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;
                
                GameObject station = Instantiate(prefab, position, Quaternion.identity);
                station.transform.SetParent(systemRoot);
                station.name = $"Station_{i+1}";
            }
        }
        
        public void GeneratePlanetTerrain(GameObject planet, int depth, int maxDepth)
        {
            if (depth >= maxDepth) return;
                        
            MeshFilter meshFilter = planet.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.mesh != null)
            {
                Mesh mesh = meshFilter.mesh;
                
                Vector3[] vertices = mesh.vertices;
                int[] triangles = mesh.triangles;
                
                List<Vector3> newVertices = new List<Vector3>();
                List<int> newTriangles = new List<int>();
                
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    Vector3 v1 = vertices[triangles[i]];
                    Vector3 v2 = vertices[triangles[i+1]];
                    Vector3 v3 = vertices[triangles[i+2]];
                    
                    Vector3 m1 = (v1 + v2) * 0.5f;
                    Vector3 m2 = (v2 + v3) * 0.5f;
                    Vector3 m3 = (v3 + v1) * 0.5f;
                    
                    m1 += Vector3.Normalize(m1) * RandomRange(-0.1f, 0.1f);
                    m2 += Vector3.Normalize(m2) * RandomRange(-0.1f, 0.1f);
                    m3 += Vector3.Normalize(m3) * RandomRange(-0.1f, 0.1f);                    
                }
            }
            
            for (int i = 0; i < planet.transform.childCount; i++)
            {
                GeneratePlanetTerrain(planet.transform.GetChild(i).gameObject, depth + 1, maxDepth);
            }
        }
        
        private float RandomRange(float min, float max)
        {
            return (float)(random.NextDouble() * (max - min) + min);
        }
    }
}