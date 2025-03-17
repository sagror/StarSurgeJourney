using UnityEngine;

namespace StarSurgeJourney.Systems.Weapons
{
    public class MuzzleFlash : MonoBehaviour
    {
        [SerializeField] private float flashDuration = 0.05f;
        private SpriteRenderer spriteRenderer;
        
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = false;
        }
        
        public void Flash()
        {
            spriteRenderer.enabled = true;
            Invoke("Hide", flashDuration);
        }
        
        private void Hide()
        {
            spriteRenderer.enabled = false;
        }
    }
}