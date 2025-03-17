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
                // En 2D, usamos firePoint.up en lugar de firePoint.forward
                Vector2 direction = firePoint.up;
                
                if (spreadAngle > 0 && projectilesPerShot > 1)
                {
                    // Para 2D, rotamos alrededor del eje Z
                    float angle = Random.Range(-spreadAngle, spreadAngle);
                    direction = (Vector2)(Quaternion.Euler(0, 0, angle) * direction);
                }
                
                if (projectilePrefab != null)
                {
                    // Para 2D, usamos Quaternion.Euler para rotar alrededor del eje Z
                    Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90);
                    GameObject projectileObj = Instantiate(projectilePrefab, firePoint.position, rotation);
                    Projectile projectile = projectileObj.GetComponent<Projectile>();
                    
                    if (projectile != null)
                    {
                        // La dirección que pasamos es un Vector2, compatible con Initialize
                        projectile.Initialize(Damage, direction, projectileSpeed);
                    }
                    else
                    {
                        // Si el proyectil no tiene el componente Projectile, intentamos usar Rigidbody2D
                        Rigidbody2D rb = projectileObj.GetComponent<Rigidbody2D>();
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