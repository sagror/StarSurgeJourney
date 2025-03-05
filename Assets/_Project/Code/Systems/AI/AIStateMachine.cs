using UnityEngine;
using System.Collections.Generic;

namespace StarSurgeJourney.Systems.AI
{
    public class AIStateMachine : MonoBehaviour
    {
        private Dictionary<string, IState> states = new Dictionary<string, IState>();
        private IState currentState;
        private string currentStateName;
        
        public System.Action<string, string> OnStateChanged;
        
        public string CurrentState => currentStateName;
        
        public void AddState(string stateName, IState state)
        {
            if (!states.ContainsKey(stateName))
            {
                states.Add(stateName, state);
            }
        }
        
        public void SetInitialState(string stateName)
        {
            if (states.TryGetValue(stateName, out IState state))
            {
                currentState = state;
                currentStateName = stateName;
                currentState.Enter();
            }
        }
        
        public void ChangeState(string stateName)
        {
            if (currentState == null || !states.ContainsKey(stateName))
                return;
                
            string previousState = currentStateName;
            
            currentState.Exit();
            
            states.TryGetValue(stateName, out currentState);
            currentStateName = stateName;
            currentState.Enter();
            
            OnStateChanged?.Invoke(previousState, currentStateName);
        }
        
        public void UpdateState()
        {
            if (currentState != null)
            {
                currentState.Update();
            }
        }
        
        public void TriggerEvent(AITriggerType triggerType, object context = null)
        {
            if (currentState != null)
            {
                currentState.OnTriggerEvent(triggerType, context);
            }
        }
        
        private void Update()
        {
            UpdateState();
        }
    }
}