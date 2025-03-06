using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StarSurgeJourney.Models;
using StarSurgeJourney.Systems.Weapons;
using System.Collections.Generic;

namespace StarSurgeJourney.UI
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private ShipModel playerModel;
        
        [Header("Bars and Values")]
        [SerializeField] private Image healthBar;
        [SerializeField] private Image healthBarEffect; 
        [SerializeField] private Image shieldBar;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI shieldText;
        
        [Header("Weapons")]
        [SerializeField] private Image weaponIcon;
        [SerializeField] private TextMeshProUGUI weaponNameText;
        [SerializeField] private Image weaponCooldownOverlay;
        [SerializeField] private TextMeshProUGUI ammoText;
        
        [Header("Radar and Maps")]
        [SerializeField] private RectTransform radarContainer;
        [SerializeField] private GameObject radarBlipPrefab;
        [SerializeField] private float radarRange = 300f;
        
        [Header("Mission Information")]
        [SerializeField] private TextMeshProUGUI objectiveText;
        [SerializeField] private TextMeshProUGUI waveText;
        [SerializeField] private TextMeshProUGUI timeText;
        
        [Header("Notifications")]
        [SerializeField] private GameObject notificationPrefab;
        [SerializeField] private RectTransform notificationContainer;
        
        private Dictionary<Transform, GameObject> radarBlips = new Dictionary<Transform, GameObject>();
        
        private IWeapon currentWeapon;
        
        private List<Transform> enemiesInRange = new List<Transform>();
        
        private void Start()
        {
            if (playerModel == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerModel = player.GetComponent<ShipModel>();
                }
            }
            
            if (playerModel != null)
            {
                playerModel.OnHealthChanged += UpdateHealthUI;
            }
            
            UpdateAllUI();
        }
        
        private void Update()
        {
            UpdateRadar();
            UpdateTimer();
            UpdateWeaponCooldown();
        }
        
        public void UpdateAllUI()
        {
            if (playerModel != null)
            {
                UpdateHealthUI(playerModel.GetStats().currentHealth);
                UpdateShieldUI(playerModel.GetStats().shield);
                UpdateWeaponUI();
            }
            
            UpdateObjectiveUI();
            UpdateWaveUI();
        }
        
        private void UpdateHealthUI(float currentHealth)
        {
            if (healthBar == null || playerModel == null) return;
            
            float maxHealth = playerModel.GetStats().maxHealth;
            float healthRatio = maxHealth > 0 ? currentHealth / maxHealth : 0;
            
            healthBar.fillAmount = healthRatio;
            
            if (healthBarEffect != null)
            {
                if (healthBarEffect.fillAmount > healthRatio)
                {
                    StartCoroutine(AnimateHealthBarEffect(healthRatio));
                }
                else
                {
                    healthBarEffect.fillAmount = healthRatio;
                }
            }
            
            if (healthText != null)
            {
                healthText.text = Mathf.Ceil(currentHealth) + " / " + Mathf.Ceil(maxHealth);
            }
            
            if (healthRatio < 0.3f)
            {
                healthBar.color = Color.red;
            }
            else if (healthRatio < 0.6f)
            {
                healthBar.color = Color.yellow;
            }
            else
            {
                healthBar.color = Color.green;
            }
        }
        
        private System.Collections.IEnumerator AnimateHealthBarEffect(float targetFill)
        {
            float duration = 0.5f;
            float elapsed = 0f;
            float startFill = healthBarEffect.fillAmount;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                healthBarEffect.fillAmount = Mathf.Lerp(startFill, targetFill, t);
                yield return null;
            }
            
            healthBarEffect.fillAmount = targetFill;
        }
        
        private void UpdateShieldUI(float currentShield)
        {
            if (shieldBar == null) return;
            
            float maxShield = 100f;
            float shieldRatio = maxShield > 0 ? currentShield / maxShield : 0;
            
            shieldBar.fillAmount = shieldRatio;
            
            if (shieldText != null)
            {
                shieldText.text = Mathf.Ceil(currentShield) + " / " + Mathf.Ceil(maxShield);
            }
        }
        
        private void UpdateWeaponUI()
        {
            if (weaponIcon == null || weaponNameText == null) return;
                        
            currentWeapon = GetCurrentWeapon();
            
            if (currentWeapon != null)
            {
                weaponNameText.text = currentWeapon.Name;

                UpdateAmmoUI();
            }
            else
            {
                weaponNameText.text = "No Weapon";
            }
        }
        
        private IWeapon GetCurrentWeapon()
        {

            return null;
        }
        
        private void UpdateAmmoUI()
        {
            if (ammoText == null) return;
            
            int currentAmmo = 0;
            int maxAmmo = 0;
                        
            ammoText.text = currentAmmo + " / " + maxAmmo;
        }
        
        private void UpdateWeaponCooldown()
        {
            if (weaponCooldownOverlay == null || currentWeapon == null) return;
        }
        
        private void UpdateObjectiveUI()
        {
            if (objectiveText == null) return;
            
            string objectives = "- Destroy 10 enemies\n- Collect 5 resources";
            objectiveText.text = "Objectives:\n" + objectives;
        }
        
        private void UpdateWaveUI()
        {
            if (waveText == null) return;
            
            int currentWave = 1;
            waveText.text = "Wave: " + currentWave;
        }
        
        private void UpdateTimer()
        {
            if (timeText == null) return;
            
            float gameTime = Time.time;
            int minutes = Mathf.FloorToInt(gameTime / 60);
            int seconds = Mathf.FloorToInt(gameTime % 60);
            
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        
        private void UpdateRadar()
        {
            if (radarContainer == null) return;
            
            CleanupRadarBlips();
            
            FindEnemiesInRange();
            
            foreach (Transform enemy in enemiesInRange)
            {
                if (enemy == null) continue;
                
                Vector3 relativePos = enemy.position - playerModel.transform.position;
                
                float normalizedDistance = relativePos.magnitude / radarRange;
                if (normalizedDistance > 1)
                {
                    continue;
                }
                
                Vector2 radarPos = new Vector2(
                    relativePos.x / radarRange * radarContainer.rect.width * 0.5f,
                    relativePos.z / radarRange * radarContainer.rect.height * 0.5f
                );
                
                GameObject blip;
                if (!radarBlips.TryGetValue(enemy, out blip) || blip == null)
                {
                    blip = Instantiate(radarBlipPrefab, radarContainer);
                    radarBlips[enemy] = blip;
                }
                
                RectTransform blipRect = blip.GetComponent<RectTransform>();
                blipRect.anchoredPosition = radarPos;
            }
        }
        
        private void CleanupRadarBlips()
        {
            List<Transform> keysToRemove = new List<Transform>();
            
            foreach (var kvp in radarBlips)
            {
                if (kvp.Key == null || !kvp.Value)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }
            
            foreach (Transform key in keysToRemove)
            {
                if (radarBlips[key] != null)
                {
                    Destroy(radarBlips[key]);
                }
                radarBlips.Remove(key);
            }
        }
        
        private void FindEnemiesInRange()
        {
            enemiesInRange.Clear();
            
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            
            foreach (GameObject enemy in enemies)
            {
                if (Vector3.Distance(playerModel.transform.position, enemy.transform.position) <= radarRange)
                {
                    enemiesInRange.Add(enemy.transform);
                }
            }
        }
        
        public void ShowNotification(string message, float duration = 3f)
        {
            if (notificationPrefab == null || notificationContainer == null) return;
            
            GameObject notificationObj = Instantiate(notificationPrefab, notificationContainer);
            
            TextMeshProUGUI text = notificationObj.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = message;
            }
            
            CanvasGroup canvasGroup = notificationObj.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                StartCoroutine(AnimateNotification(canvasGroup, duration));
            }
            else
            {
                Destroy(notificationObj, duration);
            }
        }
        
        private System.Collections.IEnumerator AnimateNotification(CanvasGroup group, float duration)
        {
            float fadeInDuration = 0.5f;
            float elapsed = 0f;
            
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                group.alpha = Mathf.Lerp(0, 1, elapsed / fadeInDuration);
                yield return null;
            }
            
            yield return new WaitForSeconds(duration - fadeInDuration - 0.5f);
            
            elapsed = 0f;
            float fadeOutDuration = 0.5f;
            
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                group.alpha = Mathf.Lerp(1, 0, elapsed / fadeOutDuration);
                yield return null;
            }
            
            Destroy(group.gameObject);
        }
        
        private void OnDestroy()
        {
            if (playerModel != null)
            {
                playerModel.OnHealthChanged -= UpdateHealthUI;
            }
        }
    }
}