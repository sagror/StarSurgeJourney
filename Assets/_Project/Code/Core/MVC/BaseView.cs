using UnityEngine;

namespace StarSurgeJourney.Core.MVC
{
    public abstract class BaseView : MonoBehaviour
    {
        protected BaseModel model;
        
        public virtual void Initialize(BaseModel model)
        {
            this.model = model;
            model.RegisterView(this);
        }
        
        public abstract void UpdateView();
        
        protected virtual void OnDestroy()
        {
            if (model != null)
            {
                model.UnregisterView(this);
            }
        }
    }
}