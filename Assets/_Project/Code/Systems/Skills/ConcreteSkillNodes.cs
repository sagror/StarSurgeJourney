using UnityEngine;
using StarSurgeJourney.Models;
using StarSurgeJourney.Systems.Weapons;

namespace StarSurgeJourney.Systems.Skills
{
    public class SpeedSkillNode : SkillNode
    {
        private float speedBoostPercentage;
        
        public SpeedSkillNode(SkillNodeData data, float speedBoostPercentage) : base(data)
        {
            this.speedBoostPercentage = speedBoostPercentage;
        }
        
        public override void ApplyEffects(GameObject target)
        {
            ShipModel shipModel = target.GetComponent<ShipModel>();
            if (shipModel != null)
            {
                float boostMultiplier = 1f + (speedBoostPercentage * CurrentLevel / 100f);
                                
                Debug.Log($"Velocity improved in {speedBoostPercentage * CurrentLevel}% for {target.name}");
            }
        }
    }
    
    public class WeaponDamageSkillNode : SkillNode
    {
        private float damageBoostPercentage;
        
        public WeaponDamageSkillNode(SkillNodeData data, float damageBoostPercentage) : base(data)
        {
            this.damageBoostPercentage = damageBoostPercentage;
        }
        
        public override void ApplyEffects(GameObject target)
        {
            IWeapon[] weapons = target.GetComponentsInChildren<IWeapon>();
            
            foreach (var weapon in weapons)
            {
                if (weapon is BaseWeapon baseWeapon)
                {
                    float boostMultiplier = 1f + (damageBoostPercentage * CurrentLevel / 100f);
                    baseWeapon.Upgrade(boostMultiplier, 1f, 1f);
                    
                    Debug.Log($"Damage of {weapon.Name} improved in {damageBoostPercentage * CurrentLevel}% for {target.name}");
                }
            }
        }
    }
    
    public class HealthSkillNode : SkillNode
    {
        private float healthBoostPercentage;
        
        public HealthSkillNode(SkillNodeData data, float healthBoostPercentage) : base(data)
        {
            this.healthBoostPercentage = healthBoostPercentage;
        }
        
        public override void ApplyEffects(GameObject target)
        {
            ShipModel shipModel = target.GetComponent<ShipModel>();
            if (shipModel != null)
            {
                float boostMultiplier = 1f + (healthBoostPercentage * CurrentLevel / 100f);
                
                Debug.Log($"Health improved in {healthBoostPercentage * CurrentLevel}% for {target.name}");
            }
        }
    }
    
    public class WeaponUnlockSkillNode : SkillNode
    {
        private WeaponType weaponType;
        
        public WeaponUnlockSkillNode(SkillNodeData data, WeaponType weaponType) : base(data)
        {
            this.weaponType = weaponType;
        }
        
        public override void ApplyEffects(GameObject target)
        {
            if (CurrentLevel > 0)
            {
   
                Debug.Log($"Weapon {weaponType} unlocked for {target.name}");
                
                IWeapon newWeapon = WeaponFactory.CreateWeapon(weaponType, target);
            }
        }
    }
}