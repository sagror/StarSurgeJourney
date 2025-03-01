using UnityEngine;

namespace StarSurgeJourney.Systems.Weapons
{
    public class ProjectileWeapon : BaseWeapon
    {
        [SerializeField] private float projectileSpeed = 20f;
        [SerializeField] private float spreadAngle = 0f;
        [SerializeField] private int projectilesPerShot = 1;
        
        protected override void FireImplementation(Transform firePoint)
        {
            for (int i = 0; i < projectilesPerShot; i++)
            {
                Vector3 direction = firePoint.forward;
                if (spreadAngle > 0 && projectilesPerShot > 1)
                {
                    float angle = Random.Range(-spreadAngle, spreadAngle);
                    direction = Quaternion.Euler(0, angle, 0) * direction;
                }
                
                if (projectilePrefab != null)
                {
                    GameObject projectileObj = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));
                    Projectile projectile = projectileObj.GetComponent<Projectile>();
                    
                    if (projectile != null)
                    {
                        projectile.Initialize(Damage, Range, direction * projectileSpeed);
                    }
                    else
                    {
                        Rigidbody rb = projectileObj.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            rb.velocity = direction * projectileSpeed;
                        }
                        
                        float lifetime = Range / projectileSpeed;
                        Destroy(projectileObj, lifetime);
                    }
                }
            }
        }
    }
}