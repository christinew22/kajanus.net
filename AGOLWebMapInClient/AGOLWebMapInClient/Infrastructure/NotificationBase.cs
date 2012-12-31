namespace Knet.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;

    /// <summary>
    /// Represents an object which can be observed via INotifyPropertyChanged. 
    /// </summary>
    public abstract class NotificationBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        /// Raises the PropertyChanged event for the specified property.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the property.
        /// </typeparam>
        /// <param name="property">
        /// The property.
        /// </param>
        public void NotifyPropertyChanged<T>(Expression<Func<T>> property)
        {
            if (this.PropertyChanged == null)
            {
                return;
            }

            var lambda = (LambdaExpression)property;

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)lambda.Body;
            }

            this.PropertyChanged(this, new PropertyChangedEventArgs(memberExpression.Member.Name));
        }

        /// <summary>
        /// Raises the PropertyChanged event for all properties of the object.
        /// </summary>
        public void NotifyPropertyChanged()
        {
            if (this.PropertyChanged == null)
            {
                return;
            }

            this.PropertyChanged(this, new PropertyChangedEventArgs(null));
        }

        /// <summary>
        /// Helper method to set a property value, typically used in implementing a setter.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the property.
        /// </typeparam>
        /// <param name="backingField">
        /// The backing field used.
        /// </param>
        /// <param name="value">
        /// The new value.
        /// </param>
        /// <param name="property">
        /// The property as an expression like '() => this.UserName'.
        /// </param>
        /// <returns>
        /// Returns true if the property actually changed.
        /// </returns>
        protected bool SetProperty<T>(ref T backingField, T value, Expression<Func<T>> property)
        {
            var changed = !EqualityComparer<T>.Default.Equals(backingField, value);
            if (changed)
            {
                backingField = value;
                this.NotifyPropertyChanged<T>(property);
            }

            return changed;
        }

        /// <summary>
        /// Clears the registered event listeners for PropertyChanged.
        /// </summary>
        protected void ClearPropertyChangedListeners()
        {
            this.PropertyChanged = null;
        }
    }
}
