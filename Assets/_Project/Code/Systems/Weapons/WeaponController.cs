using UnityEngine;
using StarSurgeJourney.Systems.Weapons;

namespace StarSurgeJourney.Controllers
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float projectileSpeed = 10f;
        [SerializeField] private float fireRate = 0.5f;
        [SerializeField] private float damage = 10f;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip fireSound;
        [SerializeField] private MuzzleFlash muzzleFlash;
        
        private float lastFireTime;
        
        private void Start()
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
        }
        
        public void Fire()
        {
            // Check if enough time has passed since last fire
            if (Time.time - lastFireTime < fireRate)
                return;
                
            lastFireTime = Time.time;
            
            // Play sound effect
            if (audioSource != null && fireSound != null)
            {
                audioSource.PlayOneShot(fireSound);
            }
            
            // Flash the muzzle
            if (muzzleFlash != null)
            {
                muzzleFlash.Flash();
            }
            
            // Create projectile
            if (projectilePrefab != null && firePoint != null)
            {
                GameObject projectileObj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
                Projectile projectile = projectileObj.GetComponent<Projectile>();
                
                if (projectile != null)
                {
                    projectile.Initialize(damage, firePoint.up, projectileSpeed);
                }
            }
        }
    }
}