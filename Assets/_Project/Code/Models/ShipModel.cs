using UnityEngine;
using System;
using System.Collections.Generic;
using StarSurgeJourney.Core.MVC;
using StarSurgeJourney.Systems.Weapons;

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
    
    public class ShipModel : BaseModel, IDamageable
    {
        [SerializeField] private ShipStats stats = new ShipStats();
        
        // Current position
        private Vector2 position;
        
        // Current rotation (2D uses a single angle)
        private float rotation;
        
        // Current velocity
        private Vector2 velocity;
        
        // Firing state
        private bool isFiring = false;
        private float lastFireTime = 0f;
        
        // Events
        public event Action<float> OnHealthChanged;
        public event Action<Vector2> OnPositionChanged;
        public event Action<float> OnRotationChanged;
        public event Action OnFire;
        public event Action OnDestroyed;
        
        private void Awake()
        {
            stats.Initialize();
            position = transform.position;
            rotation = transform.eulerAngles.z;
            velocity = Vector2.zero;
        }
        
        // Public methods for controllers
        
        public void Move(Vector2 direction, float deltaTime)
        {
            // Calculate new velocity
            velocity = direction * stats.speed;
            
            // Update position
            position += velocity * deltaTime;
            
            // Apply position to transform (AÑADIR ESTA LÍNEA)
            transform.position = new Vector3(position.x, position.y, transform.position.z);
            
            // Notify observers
            OnPositionChanged?.Invoke(position);
            
            // Notify views
            NotifyViews();
        }

        public void Rotate(float amount, float deltaTime)
        {
            // Calculate new rotation (2D only needs z-axis rotation)
            rotation += amount * stats.rotationSpeed * deltaTime;
            
            // Apply rotation to transform (AÑADIR ESTA LÍNEA)
            transform.rotation = Quaternion.Euler(0, 0, rotation);
            
            // Notify observers
            OnRotationChanged?.Invoke(rotation);
            
            // Notify views
            NotifyViews();
        }
        
        public void Fire()
        {
            if (Time.time - lastFireTime < stats.fireRate)
                return;
                
            isFiring = true;
            lastFireTime = Time.time;
            
            // Notify observers
            OnFire?.Invoke();
            
            // Notify views
            NotifyViews();
            
            // Reset after firing
            isFiring = false;
        }
        
        public void TakeDamage(float amount)
        {
            // Apply damage
            stats.currentHealth -= amount;
            
            // Notify
            OnHealthChanged?.Invoke(stats.currentHealth);
            
            // Check for destruction
            if (stats.currentHealth <= 0)
            {
                // Trigger destroyed event
                OnDestroyed?.Invoke();
                
                // You might want to play an explosion effect here
                
                // Destroy the game object
                Destroy(gameObject);
            }
        }
        
        // Getters and setters
        
        public Vector2 GetPosition() => position;
        public float GetRotation() => rotation;
        public Vector2 GetVelocity() => velocity;
        public bool IsFiring() => isFiring;
        public ShipStats GetStats() => stats;
        
        public void SetPosition(Vector2 newPosition)
        {
            position = newPosition;
            OnPositionChanged?.Invoke(position);
            NotifyViews();
        }
        
        public void SetRotation(float newRotation)
        {
            rotation = newRotation;
            OnRotationChanged?.Invoke(rotation);
            NotifyViews();
        }
    }
}