using UnityEngine;

namespace StarSurgeJourney.Core.MVC
{
    public abstract class BaseController : MonoBehaviour
    {
        protected BaseModel model;
        
        public virtual void Initialize(BaseModel model)
        {
            this.model = model;
        }
        
        public abstract void ProcessInput();
        
        protected abstract void UpdateModel();
    }
}