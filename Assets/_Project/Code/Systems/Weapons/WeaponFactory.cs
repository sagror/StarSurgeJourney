using UnityEngine;

namespace StarSurgeJourney.Systems.Weapons
{
    public enum WeaponType
    {
        LaserBeam,
        MachineGun,
        Shotgun,
        MissileLauncher,
        PlasmaCannon
    }
    
    public static class WeaponFactory
    {
        public static IWeapon CreateWeapon(WeaponType type, GameObject parent)
        {
            GameObject weaponObj = new GameObject(type.ToString());
            weaponObj.transform.SetParent(parent.transform);
            
            BaseWeapon weapon = null;
            
            switch (type)
            {
                case WeaponType.LaserBeam:
                    weapon = weaponObj.AddComponent<LaserWeapon>();
                    // Config specifics  LaserWeapon
                    break;
                    
                case WeaponType.MachineGun:
                    weapon = weaponObj.AddComponent<ProjectileWeapon>();
                    if (weapon is ProjectileWeapon machineGun)
                    {
                        // Config specifics  machineGun
                        // machineGun.projectileSpeed = 30f;
                        // machineGun.fireRate = 10f;
                        // machineGun.damage = 5f;
                    }
                    break;
                    
                case WeaponType.Shotgun:
                    weapon = weaponObj.AddComponent<ProjectileWeapon>();
                    if (weapon is ProjectileWeapon shotgun)
                    {
                        // Config shotgun specifics
                        // shotgun.projectilesPerShot = 5;
                        // shotgun.spreadAngle = 15f;
                        // shotgun.damage = 8f;
                    }
                    break;
                    
                case WeaponType.MissileLauncher:
                    weapon = weaponObj.AddComponent<ProjectileWeapon>();
                    break;
                    
                case WeaponType.PlasmaCannon:
                    weapon = weaponObj.AddComponent<LaserWeapon>();
                    break;
            }
            
            return weapon;
        }
    }
}