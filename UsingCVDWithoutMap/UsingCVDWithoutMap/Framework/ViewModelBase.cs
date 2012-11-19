using System;

namespace UsingCVDWithoutMap.Framework
{
    using Caliburn.Micro;

    public class ViewModelBase : Screen
    {
        private string contextKey;

        public string ContextKey
        {
            get
            {
                return this.contextKey;
            }
            set
            {
                if (value.Equals(this.contextKey))
                {
                    return;
                }
                this.contextKey = value;
                this.NotifyOfPropertyChange(() => this.ContextKey);
            }
        }

        protected override void OnInitialize()
        {
            if (!string.IsNullOrEmpty(this.ContextKey))
            {
                this.LoadNavigationContext(ContextKey);                
            }
            base.OnInitialize();
        }

        protected virtual void LoadNavigationContext(string key)
        {
            // template

        }
    }
}
