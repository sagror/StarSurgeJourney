using UnityEngine;
using System.Collections;

namespace StarSurgeJourney.Systems.Weapons
{
    public abstract class BaseWeapon : MonoBehaviour, IWeapon
    {
        [SerializeField] protected string weaponName = "Base Weapon";
        [SerializeField] protected float damage = 10f;
        [SerializeField] protected float fireRate = 0.5f;
        [SerializeField] protected float range = 20f;
        [SerializeField] protected GameObject projectilePrefab;
        [SerializeField] protected AudioClip fireSound;
        [SerializeField] protected ParticleSystem muzzleFlash;
        
        protected float lastFireTime = -100f;
        protected AudioSource audioSource;
        
        public string Name => weaponName;
        public float Damage => damage;
        public float FireRate => fireRate;
        public float Range => range;
        
        protected virtual void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        public virtual void Fire(Transform firePoint)
        {
            if (Time.time - lastFireTime < 1f / fireRate)
                return;
                
            lastFireTime = Time.time;
            
            if (fireSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(fireSound);
            }
            
            if (muzzleFlash != null)
            {
                muzzleFlash.Play();
            }
            
            FireImplementation(firePoint);
        }
        
        protected abstract void FireImplementation(Transform firePoint);
        
        public virtual void Upgrade(float damageMultiplier, float fireRateMultiplier, float rangeMultiplier)
        {
            damage *= damageMultiplier;
            fireRate *= fireRateMultiplier;
            range *= rangeMultiplier;
        }
    }
}