using UnityEngine;
using System.Collections.Generic;
using StarSurgeJourney.Models;

namespace StarSurgeJourney.Systems.AI
{
    public enum AIBehaviorType
    {
        Patrol,
        Attack,
        Flee,
        Follow,
        Idle
    }
    
    public class AIController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ShipModel shipModel;
        
        [Header("AI Configuration")]
        [SerializeField] private AIBehaviorType initialBehavior = AIBehaviorType.Patrol;
        [SerializeField] private float detectionRange = 100f;
        [SerializeField] private float attackRange = 50f;
        [SerializeField] private float fleeHealthThreshold = 0.3f;
        [SerializeField] private Transform[] patrolPoints;
        
        private AIStateMachine stateMachine;
        
        private Transform target;
        private int currentPatrolIndex = 0;
        
        public Transform Target => target;
        public float DetectionRange => detectionRange;
        public float AttackRange => attackRange;
        public float FleeHealthThreshold => fleeHealthThreshold;
        
        private void Awake()
        {
            stateMachine = GetComponent<AIStateMachine>();
            if (stateMachine == null)
            {
                stateMachine = gameObject.AddComponent<AIStateMachine>();
            }
            
            if (shipModel == null)
            {
                shipModel = GetComponent<ShipModel>();
            }
            
            InitializeStateMachine();
        }
        
        private void Start()
        {
            switch (initialBehavior)
            {
                case AIBehaviorType.Patrol:
                    stateMachine.SetInitialState("Patrol");
                    break;
                case AIBehaviorType.Attack:
                    stateMachine.SetInitialState("Attack");
                    break;
                case AIBehaviorType.Flee:
                    stateMachine.SetInitialState("Flee");
                    break;
                case AIBehaviorType.Follow:
                    stateMachine.SetInitialState("Follow");
                    break;
                case AIBehaviorType.Idle:
                default:
                    stateMachine.SetInitialState("Idle");
                    break;
            }
        }
        
        private void InitializeStateMachine()
        {
            // Crear y añadir los estados
            stateMachine.AddState("Idle", new IdleState(this, stateMachine));
            stateMachine.AddState("Patrol", new PatrolState(this, stateMachine));
            stateMachine.AddState("Attack", new AttackState(this, stateMachine));
            stateMachine.AddState("Flee", new FleeState(this, stateMachine));
            stateMachine.AddState("Follow", new FollowState(this, stateMachine));
        }
        
        public void MoveTowards(Vector3 targetPosition)
        {
            if (shipModel == null) return;
            
            Vector3 direction = (targetPosition - transform.position).normalized;
            
            shipModel.Move(direction, Time.deltaTime);
            
            Vector3 targetDirection = targetPosition - transform.position;
            if (targetDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                float angle = Quaternion.Angle(transform.rotation, targetRotation);
                float rotationDirection = Mathf.Sign(Vector3.Dot(Vector3.up, Vector3.Cross(transform.forward, targetDirection)));
                
                shipModel.Rotate(rotationDirection * angle * 0.01f, Time.deltaTime);
            }
        }
        
        public void Fire()
        {
            if (shipModel == null) return;
            
            shipModel.Fire();
        }
        
        public Vector3 GetNextPatrolPoint()
        {
            if (patrolPoints == null || patrolPoints.Length == 0)
            {
                return transform.position + Random.insideUnitSphere * 100f;
            }
            
            Vector3 point = patrolPoints[currentPatrolIndex].position;
            
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            
            return point;
        }
        
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            
            if (target != null)
            {
                stateMachine.TriggerEvent(AITriggerType.TargetSpotted, target);
            }
            else
            {
                stateMachine.TriggerEvent(AITriggerType.TargetLost);
            }
        }
        
        public void ScanForTargets()
        {            
            GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag("Player");
            
            float closestDistance = float.MaxValue;
            Transform closestTarget = null;
            
            foreach (var potentialTarget in potentialTargets)
            {
                float distance = Vector3.Distance(transform.position, potentialTarget.transform.position);
                
                if (distance < detectionRange && distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = potentialTarget.transform;
                }
            }
            
            if (closestTarget != null)
            {
                SetTarget(closestTarget);
            }
        }
        
        public void OnDamageReceived()
        {
            stateMachine.TriggerEvent(AITriggerType.TakeDamage);
            
            if (shipModel != null && shipModel.GetStats().currentHealth / shipModel.GetStats().maxHealth < fleeHealthThreshold)
            {
                stateMachine.TriggerEvent(AITriggerType.HealthLow);
            }
        }
        
        private void Update()
        {
            if (Time.frameCount % 30 == 0)
            {
                ScanForTargets();
            }
        }
    }
}