using UnityEngine;
using StarSurgeJourney.Core.MVC;
using StarSurgeJourney.Models;

namespace StarSurgeJourney.Views
{
    public class ShipView : BaseView
    {
        [SerializeField] private SpriteRenderer shipSprite;
        [SerializeField] private ParticleSystem engineEffect;
        [SerializeField] private ParticleSystem damageEffect;
        [SerializeField] private ParticleSystem shieldEffect;
        [SerializeField] private Transform firePoint;
        
        [Header("Audio")]
        [SerializeField] private AudioSource engineAudio;
        [SerializeField] private AudioSource fireAudio;
        [SerializeField] private AudioSource damageAudio;
        
        private ShipModel shipModel;
        private Animator animator;
        
        private void Awake()
        {
            animator = GetComponent<Animator>();
            
            if (shipSprite == null)
                shipSprite = GetComponent<SpriteRenderer>();
        }
        
        public override void Initialize(BaseModel model)
        {
            base.Initialize(model);
            shipModel = model as ShipModel;
            
            if (shipModel != null)
            {
                // Subscribe to events
                shipModel.OnPositionChanged += UpdatePosition;
                shipModel.OnRotationChanged += UpdateRotation;
                shipModel.OnFire += PlayFireEffect;
                shipModel.OnHealthChanged += UpdateHealthEffect;
                shipModel.OnDestroyed += PlayDestroyedEffect;
            }
        }
        
        public override void UpdateView()
        {
            // This method is called when the model changes
            // We already have specific events, but could do additional updates here
        }
        
        private void UpdatePosition(Vector2 position)
        {
            transform.position = new Vector3(position.x, position.y, 0);
            
            // Update engine effect based on velocity
            if (engineEffect != null)
            {
                var emission = engineEffect.emission;
                var main = engineEffect.main;
                
                float speedRatio = shipModel.GetVelocity().magnitude / shipModel.GetStats().speed;
                emission.rateOverTime = Mathf.Lerp(5f, 20f, speedRatio);
                main.startSpeed = Mathf.Lerp(1f, 5f, speedRatio);
                
                if (engineAudio != null)
                {
                    engineAudio.pitch = Mathf.Lerp(0.8f, 1.2f, speedRatio);
                    engineAudio.volume = Mathf.Lerp(0.2f, 0.8f, speedRatio);
                }
            }
        }
        
        private void UpdateRotation(float rotation)
        {
            transform.rotation = Quaternion.Euler(0, 0, rotation);
        }
        
        private void PlayFireEffect()
        {
            if (fireAudio != null)
            {
                fireAudio.pitch = Random.Range(0.9f, 1.1f);
                fireAudio.Play();
            }
            
            // Here you would add logic to show firing effect
            // Usually this would involve instantiating a projectile
        }
        
        private void UpdateHealthEffect(float health)
        {
            float healthRatio = health / shipModel.GetStats().maxHealth;
            
            // Show damage effects if health is low
            if (healthRatio < 0.3f && damageEffect != null)
            {
                if (!damageEffect.isPlaying)
                {
                    damageEffect.Play();
                }
            }
            else if (damageEffect != null && damageEffect.isPlaying)
            {
                damageEffect.Stop();
            }
            
            // Activate/deactivate shield effect
            if (shipModel.GetStats().shield > 0 && shieldEffect != null)
            {
                if (!shieldEffect.isPlaying)
                {
                    shieldEffect.Play();
                }
            }
            else if (shieldEffect != null && shieldEffect.isPlaying)
            {
                shieldEffect.Stop();
            }
        }
        
        private void PlayDestroyedEffect()
        {
            // Here you would add logic to show ship explosion
            if (damageAudio != null)
            {
                damageAudio.Play();
            }
            
            // Deactivate ship sprite
            if (shipSprite != null)
            {
                shipSprite.enabled = false;
            }
            
            // You could instantiate an explosion prefab here
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            // Unsubscribe from events
            if (shipModel != null)
            {
                shipModel.OnPositionChanged -= UpdatePosition;
                shipModel.OnRotationChanged -= UpdateRotation;
                shipModel.OnFire -= PlayFireEffect;
                shipModel.OnHealthChanged -= UpdateHealthEffect;
                shipModel.OnDestroyed -= PlayDestroyedEffect;
            }
        }
    }
}