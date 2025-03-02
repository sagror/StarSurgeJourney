using System.Collections.Generic;
using UnityEngine;
using StarSurgeJourney.Systems.Weapons;

namespace StarSurgeJourney.Systems.Skills
{
    // Factory to create settled skill trees
    public static class SkillTreeFactory
    {
        public static SkillTree CreateBasicShipSkillTree()
        {
            SkillTree tree = new SkillTree();
            
            SkillNodeData speedData = new SkillNodeData
            {
                id = "speed_1",
                name = "Improved Thrusters I",
                description = "Increase ship velocity by 10%.",
                cost = 1,
                maxLevel = 3
            };
            SpeedSkillNode speedNode = new SpeedSkillNode(speedData, 10f);
            tree.AddNode(speedNode, true);
            
            SkillNodeData healthData = new SkillNodeData
            {
                id = "health_1",
                name = "Hull Reinforced I",
                description = "Increase ship max health by 15%.",
                cost = 1,
                maxLevel = 3
            };
            HealthSkillNode healthNode = new HealthSkillNode(healthData, 15f);
            tree.AddNode(healthNode, true);
            
            SkillNodeData damageData = new SkillNodeData
            {
                id = "damage_1",
                name = "Weapons booster I",
                description = "Increase weapons damage by 10%.",
                cost = 1,
                maxLevel = 3
            };
            WeaponDamageSkillNode damageNode = new WeaponDamageSkillNode(damageData, 10f);
            tree.AddNode(damageNode, true);
                        
            SkillNodeData speedData2 = new SkillNodeData
            {
                id = "speed_2",
                name = "Improved Thrusters II",
                description = "ncrease ship velocity by 20%.",
                cost = 2,
                maxLevel = 2,
                requirements = new List<string> { "speed_1" }
            };
            SpeedSkillNode speedNode2 = new SpeedSkillNode(speedData2, 20f);
            tree.AddNode(speedNode2);
            tree.ConnectNodes("speed_1", "speed_2");
            
            SkillNodeData healthData2 = new SkillNodeData
            {
                id = "health_2",
                name = "Hull Reinforced II",
                description = "Increase ship max health by 25%.",
                cost = 2,
                maxLevel = 2,
                requirements = new List<string> { "health_1" }
            };
            HealthSkillNode healthNode2 = new HealthSkillNode(healthData2, 25f);
            tree.AddNode(healthNode2);
            tree.ConnectNodes("health_1", "health_2");

            SkillNodeData damageData2 = new SkillNodeData
            {
                id = "damage_2",
                name = "Weapons booster II",
                description = "Increase weapons damage by 20%.",
                cost = 2,
                maxLevel = 2,
                requirements = new List<string> { "damage_1" }
            };
            WeaponDamageSkillNode damageNode2 = new WeaponDamageSkillNode(damageData2, 20f);
            tree.AddNode(damageNode2);
            tree.ConnectNodes("damage_1", "damage_2");
            
            SkillNodeData laserData = new SkillNodeData
            {
                id = "weapon_laser",
                name = "Precision Laser",
                description = "Unlocks Precision Laser.",
                cost = 3,
                maxLevel = 1,
                requirements = new List<string> { "damage_2" }
            };
            WeaponUnlockSkillNode laserNode = new WeaponUnlockSkillNode(laserData, WeaponType.LaserBeam);
            tree.AddNode(laserNode);
            tree.ConnectNodes("damage_2", "weapon_laser");
            
            SkillNodeData missileData = new SkillNodeData
            {
                id = "weapon_missile",
                name = "Missile Launcher",
                description = "Unlocks AOE Missile launcher.",
                cost = 3,
                maxLevel = 1,
                requirements = new List<string> { "damage_2" }
            };
            WeaponUnlockSkillNode missileNode = new WeaponUnlockSkillNode(missileData, WeaponType.MissileLauncher);
            tree.AddNode(missileNode);
            tree.ConnectNodes("damage_2", "weapon_missile");
            
            SkillNodeData shotgunData = new SkillNodeData
            {
                id = "weapon_shotgun",
                name = "Spread Cannon",
                description = "Unlocks spread Cannon.",
                cost = 3,
                maxLevel = 1,
                requirements = new List<string> { "damage_2" }
            };
            WeaponUnlockSkillNode shotgunNode = new WeaponUnlockSkillNode(shotgunData, WeaponType.Shotgun);
            tree.AddNode(shotgunNode);
            tree.ConnectNodes("damage_2", "weapon_shotgun");
            
            SkillNodeData hyperSpeedData = new SkillNodeData
            {
                id = "speed_hyper",
                name = "Hyper-Propulsion",
                description = "Highly increase ship's velocity by 50%.",
                cost = 5,
                maxLevel = 1,
                requirements = new List<string> { "speed_2", "health_2" }
            };
            SpeedSkillNode hyperSpeedNode = new SpeedSkillNode(hyperSpeedData, 50f);
            tree.AddNode(hyperSpeedNode);
            tree.ConnectNodes("speed_2", "speed_hyper");
            tree.ConnectNodes("health_2", "speed_hyper");
            
            SkillNodeData superWeaponData = new SkillNodeData
            {
                id = "weapon_plasma",
                name = "Plasma Cannon",
                description = "Unlocks most powerful weapon: Devastating plasma cannon.",
                cost = 5,
                maxLevel = 1,
                requirements = new List<string> { "weapon_laser", "weapon_missile" }
            };
            WeaponUnlockSkillNode superWeaponNode = new WeaponUnlockSkillNode(superWeaponData, WeaponType.PlasmaCannon);
            tree.AddNode(superWeaponNode);
            tree.ConnectNodes("weapon_laser", "weapon_plasma");
            tree.ConnectNodes("weapon_missile", "weapon_plasma");
            
            return tree;
        }
    }
}