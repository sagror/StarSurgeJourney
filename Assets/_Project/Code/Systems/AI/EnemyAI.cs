using UnityEngine;
using StarSurgeJourney.Models;
using StarSurgeJourney.Systems.Weapons;

namespace StarSurgeJourney.Systems.AI
{
    public class EnemyAI : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float rotationSpeed = 120f;
        [SerializeField] private float detectionRange = 10f;
        [SerializeField] private float shootingRange = 7f;
        [SerializeField] private float retreatDistance = 5f;
        
        [Header("Attack")]
        [SerializeField] private float fireRate = 1f;
        [SerializeField] private Transform firePoint;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float projectileSpeed = 5f;
        [SerializeField] private float projectileDamage = 10f;
        
        [Header("References")]
        [SerializeField] private ShipModel shipModel;
        
        private Transform playerTransform;
        private float lastFireTime;
        
        private void Start()
        {
            // Find player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            
            // Get ShipModel if not assigned
            if (shipModel == null)
            {
                shipModel = GetComponent<ShipModel>();
            }
        }
        
        private void Update()
        {
            if (playerTransform == null || shipModel == null)
                return;
                
            // Calculate distance to player
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            
            // Determine behavior based on distance
            if (distanceToPlayer <= detectionRange)
            {
                // Look at player
                Vector3 directionToPlayer = playerTransform.position - transform.position;
                float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90;
                Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                
                // If within shooting range, shoot
                if (distanceToPlayer <= shootingRange)
                {
                    // If too close, back up
                    if (distanceToPlayer < retreatDistance)
                    {
                        // Move away from player
                        Vector2 moveDirection = -directionToPlayer.normalized;
                        shipModel.Move(moveDirection, Time.deltaTime);
                    }
                    
                    Shoot();
                }
                else
                {
                    // Move towards player
                    Vector2 moveDirection = directionToPlayer.normalized;
                    shipModel.Move(moveDirection, Time.deltaTime);
                }
            }
            else
            {
                // Patrol or idle behavior when player is not detected
                // For simplicity, we'll just stay in place
            }
        }
        
        private void Shoot()
        {
            // Check if enough time has passed since last shot
            if (Time.time - lastFireTime < fireRate)
                return;
                
            lastFireTime = Time.time;
            
            // Create projectile
            if (projectilePrefab != null && firePoint != null)
            {
                GameObject projectileObj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
                Projectile projectile = projectileObj.GetComponent<Projectile>();
                
                if (projectile != null)
                {
                    // Initialize projectile
                    projectile.Initialize(projectileDamage, firePoint.up, projectileSpeed);
                }
            }
        }
        
        // Optional: visualization of detection range in the editor
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, shootingRange);
        }
    }
}