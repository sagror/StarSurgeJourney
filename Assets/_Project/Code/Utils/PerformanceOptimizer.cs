using UnityEngine;
using System.Collections.Generic;

namespace StarSurgeJourney.Utils
{
    // Class to manage performance optimizations
    public class PerformanceOptimizer : MonoBehaviour
    {
        [Header("Pooling")]
        [SerializeField] private bool useObjectPooling = true;
        [SerializeField] private List<PooledObjectInfo> objectsToPool;
        
        [Header("LOD")]
        [SerializeField] private bool useDynamicLOD = true;
        [SerializeField] private float lodDistance = 100f;
        [SerializeField] private float cullingDistance = 200f;
        
        [Header("Optimization")]
        [SerializeField] private bool limitFrameRate = true;
        [SerializeField] private int targetFrameRate = 60;
        [SerializeField] private bool useGPUInstancing = true;
        
        // Dictionary for object pooling
        private Dictionary<string, Queue<GameObject>> objectPools = new Dictionary<string, Queue<GameObject>>();
        
        // Singleton
        private static PerformanceOptimizer instance;
        
        public static PerformanceOptimizer Instance
        {
            get { return instance; }
        }
        
        private void Awake()
        {
            // Singleton configuration
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            
            // Apply optimization settings
            ApplyOptimizationSettings();
            
            // Initialize object pooling
            if (useObjectPooling)
            {
                InitializeObjectPools();
            }
        }
        
        // Apply optimization settings
        private void ApplyOptimizationSettings()
        {
            if (limitFrameRate)
            {
                Application.targetFrameRate = targetFrameRate;
            }
            
            if (useGPUInstancing)
            {
                // Configure materials to use instancing
                SetupGPUInstancing();
            }
        }
        
        // Configure GPU Instancing for materials
        private void SetupGPUInstancing()
        {
            Renderer[] renderers = FindObjectsOfType<Renderer>();
            
            foreach (Renderer renderer in renderers)
            {
                Material[] materials = renderer.materials;
                
                foreach (Material material in materials)
                {
                    if (material.shader.isSupported && material.enableInstancing == false)
                    {
                        material.enableInstancing = true;
                    }
                }
            }
        }
        
        // Initialize object pools
        private void InitializeObjectPools()
        {
            foreach (PooledObjectInfo info in objectsToPool)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();
                
                for (int i = 0; i < info.initialPoolSize; i++)
                {
                    GameObject obj = CreatePooledObject(info.prefab);
                    objectPool.Enqueue(obj);
                }
                
                objectPools.Add(info.tag, objectPool);
            }
        }
        
        // Create an object for the pool
        private GameObject CreatePooledObject(GameObject prefab)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            return obj;
        }
        
        // Get an object from the pool
        public GameObject GetPooledObject(string tag)
        {
            if (!objectPools.ContainsKey(tag))
            {
                Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
                return null;
            }
            
            if (objectPools[tag].Count > 0)
            {
                GameObject obj = objectPools[tag].Dequeue();
                obj.SetActive(true);
                obj.transform.SetParent(null);
                return obj;
            }
            else
            {
                // If the pool is empty, find the corresponding prefab
                PooledObjectInfo info = objectsToPool.Find(x => x.tag == tag);
                if (info != null)
                {
                    GameObject obj = CreatePooledObject(info.prefab);
                    obj.SetActive(true);
                    obj.transform.SetParent(null);
                    return obj;
                }
            }
            
            return null;
        }
        
        // Return an object to the pool
        public void ReturnToPool(string tag, GameObject obj)
        {
            if (!objectPools.ContainsKey(tag))
            {
                Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
                Destroy(obj);
                return;
            }
            
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            objectPools[tag].Enqueue(obj);
        }
        
        // Apply dynamic LOD
        public void ApplyDynamicLOD(GameObject obj, float distance)
        {
            if (!useDynamicLOD || obj == null)
                return;
                
            LODGroup lodGroup = obj.GetComponent<LODGroup>();
            
            if (lodGroup != null)
            {
                // LOD is already handled by the component
                return;
            }
            
            // Apply manual LOD
            if (distance > cullingDistance)
            {
                // Hide completely
                obj.SetActive(false);
            }
            else if (distance > lodDistance)
            {
                // Low LOD
                Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    // Reduce quality
                    renderer.receiveShadows = false;
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
                
                // Disable non-essential components
                Behaviour[] behaviours = obj.GetComponentsInChildren<Behaviour>();
                foreach (Behaviour behaviour in behaviours)
                {
                    // Don't disable critical components
                    System.Type transformType = typeof(Transform);
                    System.Type meshRendererType = typeof(MeshRenderer);
                    
                    if (behaviour.GetType() != transformType && behaviour.GetType() != meshRendererType)
                    {
                        behaviour.enabled = false;
                    }
                }
            }
            else
            {
                // High LOD (normal)
                obj.SetActive(true);
                
                Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    // Full quality
                    renderer.receiveShadows = true;
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                }
                
                // Enable components
                Behaviour[] behaviours = obj.GetComponentsInChildren<Behaviour>();
                foreach (Behaviour behaviour in behaviours)
                {
                    behaviour.enabled = true;
                }
            }
        }
        
        // Structure for pooled object information
        [System.Serializable]
        public class PooledObjectInfo
        {
            public string tag;
            public GameObject prefab;
            public int initialPoolSize = 10;
        }
    }
}