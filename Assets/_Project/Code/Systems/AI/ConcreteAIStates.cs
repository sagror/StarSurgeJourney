using UnityEngine;
using System.Collections;

namespace StarSurgeJourney.Systems.AI
{
    public class IdleState : BaseAIState
    {
        private float idleTime = 0f;
        private float maxIdleTime = 5f;
        
        public IdleState(AIController controller, AIStateMachine stateMachine) : base(controller, stateMachine)
        {
        }
        
        protected override void ConfigureTransitions()
        {
            transitions.Add(AITriggerType.TargetSpotted, "Attack");
            transitions.Add(AITriggerType.TakeDamage, "Flee");
            transitions.Add(AITriggerType.CommandReceived, "Patrol");
        }
        
        public override void Enter()
        {
            idleTime = 0f;
            maxIdleTime = Random.Range(3f, 7f);
            
            Debug.Log("AI Entering to state: Idle");
        }
        
        public override void Update()
        {
            idleTime += Time.deltaTime;
            
            if (idleTime >= maxIdleTime)
            {
                stateMachine.ChangeState("Patrol");
            }
        }
        
        public override void Exit()
        {
            Debug.Log("AI Exiting from state: Idle");
        }
    }
    
    public class PatrolState : BaseAIState
    {
        private Vector3 currentPatrolPoint;
        private float nextPatrolTime = 0f;
        private bool hasReachedPoint = false;
        
        public PatrolState(AIController controller, AIStateMachine stateMachine) : base(controller, stateMachine)
        {
        }
        
        protected override void ConfigureTransitions()
        {
            transitions.Add(AITriggerType.TargetSpotted, "Attack");
            transitions.Add(AITriggerType.TakeDamage, "Attack");
            transitions.Add(AITriggerType.HealthLow, "Flee");
        }
        
        public override void Enter()
        {
            currentPatrolPoint = aiController.GetNextPatrolPoint();
            hasReachedPoint = false;
            
            Debug.Log("AI Entering to state: Patrol");
        }
        
        public override void Update()
        {
            float distanceToPoint = Vector3.Distance(aiController.transform.position, currentPatrolPoint);
            
            if (distanceToPoint < 5f)
            {
                if (!hasReachedPoint)
                {
                    hasReachedPoint = true;
                    nextPatrolTime = Time.time + Random.Range(1f, 3f);
                }
                
                if (Time.time > nextPatrolTime)
                {
                    currentPatrolPoint = aiController.GetNextPatrolPoint();
                    hasReachedPoint = false;
                }
            }
            else
            {
                aiController.MoveTowards(currentPatrolPoint);
            }
        }
        
        public override void Exit()
        {
            Debug.Log("AI Exiting from state: Patrol");
        }
    }
    
    public class AttackState : BaseAIState
    {
        private float lastFireTime = 0f;
        private float fireRate = 0.5f; 
        
        public AttackState(AIController controller, AIStateMachine stateMachine) : base(controller, stateMachine)
        {
        }
        
        protected override void ConfigureTransitions()
        {
            transitions.Add(AITriggerType.TargetLost, "Patrol");
            transitions.Add(AITriggerType.HealthLow, "Flee");
            transitions.Add(AITriggerType.NoAmmo, "Flee");
        }
        
        public override void Enter()
        {
            Debug.Log("AI Entering to state: Attack");
        }
        
        public override void Update()
        {
            if (aiController.Target == null)
            {
                stateMachine.TriggerEvent(AITriggerType.TargetLost);
                return;
            }
            
            float distanceToTarget = Vector3.Distance(aiController.transform.position, aiController.Target.position);
            
            if (distanceToTarget <= aiController.AttackRange)
            {
                Vector3 positionToAim = aiController.Target.position;
                aiController.MoveTowards(positionToAim);
                
                if (Time.time - lastFireTime > fireRate)
                {
                    aiController.Fire();
                    lastFireTime = Time.time;
                }
            }
            else if (distanceToTarget <= aiController.DetectionRange)
            {
                aiController.MoveTowards(aiController.Target.position);
            }
            else
            {
                stateMachine.TriggerEvent(AITriggerType.TargetLost);
            }
        }
        
        public override void Exit()
        {
            Debug.Log("AI Exiting from state: Attack");
        }
    }
    
    public class FleeState : BaseAIState
    {
        private Vector3 fleeDirection;
        private float fleeTime = 0f;
        private float maxFleeTime = 5f;
        
        public FleeState(AIController controller, AIStateMachine stateMachine) : base(controller, stateMachine)
        {
        }
        
        protected override void ConfigureTransitions()
        {
            transitions.Add(AITriggerType.ReachedDestination, "Patrol");
        }
        
        public override void Enter()
        {
            if (aiController.Target != null)
            {
                fleeDirection = (aiController.transform.position - aiController.Target.position).normalized;
            }
            else
            {
                fleeDirection = Random.insideUnitSphere.normalized;
                fleeDirection.y = 0;
            }
            
            fleeTime = 0f;
            maxFleeTime = Random.Range(3f, 7f);
            
            Debug.Log("AI Entering to state: Flee");
        }
        
        public override void Update()
        {
            fleeTime += Time.deltaTime;
            
            if (fleeTime >= maxFleeTime)
            {
                stateMachine.TriggerEvent(AITriggerType.ReachedDestination);
                return;
            }
            
            if (aiController.Target != null)
            {
                fleeDirection = (aiController.transform.position - aiController.Target.position).normalized;
            }
            
            Vector3 destination = aiController.transform.position + fleeDirection * 100f;
            
            aiController.MoveTowards(destination);
        }
        
        public override void Exit()
        {
            Debug.Log("AI Exiting from state: Flee");
        }
    }
    
    // Estado de seguimiento
    public class FollowState : BaseAIState
    {
        private Transform targetToFollow;
        private float followDistance = 15f;
        
        public FollowState(AIController controller, AIStateMachine stateMachine) : base(controller, stateMachine)
        {
        }
        
        protected override void ConfigureTransitions()
        {
            transitions.Add(AITriggerType.TargetLost, "Patrol");
            transitions.Add(AITriggerType.CommandReceived, "Idle");
            transitions.Add(AITriggerType.TakeDamage, "Attack");
        }
        
        public override void Enter()
        {
            targetToFollow = aiController.Target;
            
            Debug.Log("AI Entering to state: Follow");
        }
        
        public override void Update()
        {
            // Verifica si aún tiene un objetivo para seguir
            if (targetToFollow == null)
            {
                stateMachine.TriggerEvent(AITriggerType.TargetLost);
                return;
            }
            
            float distanceToTarget = Vector3.Distance(aiController.transform.position, targetToFollow.position);
            
            if (distanceToTarget > followDistance)
            {
                aiController.MoveTowards(targetToFollow.position);
            }
            else
            {
                Vector3 directionToTarget = (targetToFollow.position - aiController.transform.position).normalized;
                Vector3 targetPosition = targetToFollow.position - directionToTarget * followDistance;
                
                aiController.MoveTowards(targetPosition);
            }
        }
        
        public override void Exit()
        {
            Debug.Log("AI Exiting from state: Follow");
        }
    }
}