using UnityEngine;
using System.Collections.Generic;

namespace StarSurgeJourney.Core.MVC
{
    public abstract class BaseModel : MonoBehaviour
    {
        protected List<BaseView> views = new List<BaseView>();
        
        public virtual void RegisterView(BaseView view)
        {
            if (!views.Contains(view))
            {
                views.Add(view);
            }
        }
        
        public virtual void UnregisterView(BaseView view)
        {
            if (views.Contains(view))
            {
                views.Remove(view);
            }
        }
        
        protected virtual void NotifyViews()
        {
            foreach (var view in views)
            {
                view.UpdateView();
            }
        }
    }
}