using UnityEngine;

namespace StarSurgeJourney.Systems.Weapons
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float damage = 10f;
        [SerializeField] private float lifetime = 3f;
        [SerializeField] private bool destroyOnHit = true;
        [SerializeField] private GameObject hitEffectPrefab;
        
        private Rigidbody2D rb;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            Destroy(gameObject, lifetime); // Auto-destroy after lifetime
        }
        
        public void Initialize(float damageAmount, Vector2 direction, float speed)
        {
            damage = damageAmount;
            rb.velocity = direction * speed;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Check if the other object can be damaged
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
            
            // Play hit effect if available
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }
            
            // Destroy this projectile if configured to do so
            if (destroyOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
}