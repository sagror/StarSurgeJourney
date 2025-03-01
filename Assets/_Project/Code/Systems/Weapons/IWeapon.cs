using UnityEngine;

namespace StarSurgeJourney.Systems.Weapons
{
    public interface IWeapon
    {
        string Name { get; }
        float Damage { get; }
        float FireRate { get; }
        float Range { get; }
        void Fire(Transform firePoint);
        void Upgrade(float damageMultiplier, float fireRateMultiplier, float rangeMultiplier);
    }
}