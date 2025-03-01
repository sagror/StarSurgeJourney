using StarSurgeJourney.Core.MVC;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StarSurgeJourney.Models
{
    [Serializable]
    public class ShipStats
    {
        public float maxHealth = 100f;
        public float currentHealth;
        public float speed = 5f;
        public float rotationSpeed = 180f;
        public float fireRate = 0.5f;
        public float damage = 10f;
        public float shield = 0f;
        public float shieldRechargeRate = 0f;
        
        public void Initialize()
        {
            currentHealth = maxHealth;
        }
    }
    
    public class ShipModel : BaseModel
    {
        [SerializeField] private ShipStats stats = new ShipStats();
        
        // Current Position
        private Vector3 position;
        
        // Current Rotation
        private Quaternion rotation;
        
        // Current Velocity
        private Vector3 velocity;
        
        // Firing state
        private bool isFiring = false;
        private float lastFireTime = 0f;
        
        // Events
        public event Action<float> OnHealthChanged;
        public event Action<Vector3> OnPositionChanged;
        public event Action<Quaternion> OnRotationChanged;
        public event Action OnFire;
        public event Action OnDestroyed;
        
        private void Awake()
        {
            stats.Initialize();
            position = transform.position;
            rotation = transform.rotation;
            velocity = Vector3.zero;
        }
        
        // Public Methods for controllers
        
        public void Move(Vector3 direction, float deltaTime)
        {
            velocity = direction * stats.speed;
            
            position += velocity * deltaTime;
            
            OnPositionChanged?.Invoke(position);
            
            NotifyViews();
        }
        
        public void Rotate(float amount, float deltaTime)
        {
            Quaternion deltaRotation = Quaternion.Euler(0f, amount * stats.rotationSpeed * deltaTime, 0f);
            rotation *= deltaRotation;
            
            OnRotationChanged?.Invoke(rotation);
            
            NotifyViews();
        }
        
        public void Fire()
        {
            if (Time.time - lastFireTime < stats.fireRate)
                return;
                
            isFiring = true;
            lastFireTime = Time.time;
            
            OnFire?.Invoke();
            
            NotifyViews();
            
            isFiring = false;
        }
        
        public void TakeDamage(float amount)
        {
            float damageAfterShield = Mathf.Max(0, amount - stats.shield);
            stats.currentHealth -= damageAfterShield;
            
            OnHealthChanged?.Invoke(stats.currentHealth);
            
            if (stats.currentHealth <= 0)
            {
                OnDestroyed?.Invoke();
            }
            
            NotifyViews();
        }
        
        // Getters and setters
        
        public Vector3 GetPosition() => position;
        public Quaternion GetRotation() => rotation;
        public Vector3 GetVelocity() => velocity;
        public bool IsFiring() => isFiring;
        public ShipStats GetStats() => stats;
        
        public void SetPosition(Vector3 newPosition)
        {
            position = newPosition;
            OnPositionChanged?.Invoke(position);
            NotifyViews();
        }
        
        public void SetRotation(Quaternion newRotation)
        {
            rotation = newRotation;
            OnRotationChanged?.Invoke(rotation);
            NotifyViews();
        }
    }
}