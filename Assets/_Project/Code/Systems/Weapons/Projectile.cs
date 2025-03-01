using UnityEngine;

namespace StarSurgeJourney.Systems.Weapons
{
    public interface IDamageable
    {
        void TakeDamage(float amount);
    }
    
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float damage = 10f;
        [SerializeField] private float lifetime = 5f;
        [SerializeField] private ParticleSystem hitEffect;
        [SerializeField] private AudioClip hitSound;
        [SerializeField] private bool destroyOnHit = true;
        
        private Rigidbody rb;
        private bool isInitialized = false;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }
        
        private void Start()
        {
            if (!isInitialized)
            {
                Destroy(gameObject, lifetime);
            }
        }
        
        public void Initialize(float damage, float range, Vector3 velocity)
        {
            this.damage = damage;
            this.lifetime = range / velocity.magnitude;
            
            if (rb != null)
            {
                rb.velocity = velocity;
            }
            
            isInitialized = true;
            Destroy(gameObject, lifetime);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
            
            if (hitEffect != null)
            {
                ParticleSystem effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                effect.Play();
                Destroy(effect.gameObject, effect.main.duration);
            }
            
            if (hitSound != null)
            {
                AudioSource.PlayClipAtPoint(hitSound, transform.position);
            }
            
            if (destroyOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
}