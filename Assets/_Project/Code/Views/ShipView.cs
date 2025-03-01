using StarSurgeJourney.Core.MVC;
using StarSurgeJourney.Models;
using UnityEngine;

namespace StarSurgeJourney.Views
{
    public class ShipView : BaseView
    {
        //[SerializeField] private GameObject shipModel;
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
        }
        
        public override void Initialize(BaseModel model)
        {
            base.Initialize(model);
            shipModel = model as ShipModel;
            
            if (shipModel != null)
            {
                shipModel.OnPositionChanged += UpdatePosition;
                shipModel.OnRotationChanged += UpdateRotation;
                shipModel.OnFire += PlayFireEffect;
                shipModel.OnHealthChanged += UpdateHealthEffect;
                shipModel.OnDestroyed += PlayDestroyedEffect;
            }
        }
        
        public override void UpdateView()
        {
           
        }
        
        private void UpdatePosition(Vector3 position)
        {
            transform.position = position;
            
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
        
        private void UpdateRotation(Quaternion rotation)
        {
            transform.rotation = rotation;
        }
        
        private void PlayFireEffect()
        {
            if (fireAudio != null)
            {
                fireAudio.pitch = Random.Range(0.9f, 1.1f);
                fireAudio.Play();
            }
        }
        
        private void UpdateHealthEffect(float health)
        {
            float healthRatio = health / shipModel.GetStats().maxHealth;
            
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
            if (damageAudio != null)
            {
                damageAudio.Play();
            }
            
            if (shipModel != null)
            {
                shipModel.gameObject.SetActive(false);
            }
            
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
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