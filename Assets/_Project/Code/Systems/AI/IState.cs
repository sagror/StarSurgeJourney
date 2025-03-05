using UnityEngine;

namespace StarSurgeJourney.Systems.AI
{
    public interface IState
    {
        void Enter();
        void Update();
        void Exit();
        void OnTriggerEvent(AITriggerType triggerType, object context = null);
    }
    
    public enum AITriggerType
    {
        TargetSpotted,
        TargetLost,
        HealthLow,
        TakeDamage,
        AllyNeedsHelp,
        ReachedDestination,
        CommandReceived,
        NoAmmo
    }
}