using UnityEngine;
using System.Collections;

namespace StarSurgeJourney.Systems.Weapons
{
    public class LaserWeapon : BaseWeapon
    {
        [SerializeField] private LineRenderer laserRenderer;
        [SerializeField] private float laserDuration = 0.1f;
        [SerializeField] private float laserWidth = 0.1f;
        [SerializeField] private Color laserColor = Color.red;
        
        protected override void Awake()
        {
            base.Awake();
            
            if (laserRenderer == null)
            {
                laserRenderer = GetComponent<LineRenderer>();
                if (laserRenderer == null)
                {
                    laserRenderer = gameObject.AddComponent<LineRenderer>();
                }
            }
            
            laserRenderer.startWidth = laserWidth;
            laserRenderer.endWidth = laserWidth;
            laserRenderer.material = new Material(Shader.Find("Sprites/Default"));
            laserRenderer.startColor = laserColor;
            laserRenderer.endColor = laserColor;
            laserRenderer.enabled = false;
        }
        
        protected override void FireImplementation(Transform firePoint)
        {
            laserRenderer.SetPosition(0, firePoint.position);
            
            RaycastHit hit;
            if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, Range))
            {
                laserRenderer.SetPosition(1, hit.point);
                
                var damageable = hit.collider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(Damage);
                }
            }
            else
            {
                laserRenderer.SetPosition(1, firePoint.position + firePoint.forward * Range);
            }
            
            laserRenderer.enabled = true;
            StartCoroutine(DisableLaserAfterDelay());
        }
        
        private IEnumerator DisableLaserAfterDelay()
        {
            yield return new WaitForSeconds(laserDuration);
            laserRenderer.enabled = false;
        }
    }
}