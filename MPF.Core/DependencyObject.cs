﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPF
{
    public class DependencyObject
    {
        private readonly ConcurrentDictionary<DependencyProperty, object> _localValueStore = new ConcurrentDictionary<DependencyProperty, object>();

        public DependencyObject()
        {
        }

        public void SetValue<T>(DependencyProperty<T> property, T value)
        {
            var oldValue = GetValue(property);
            if (!EqualityComparer<T>.Default.Equals(oldValue, value))
            {
                _localValueStore[property] = value;
                property.InvokePropertyChangedHandlers(this, new PropertyChangedEventArgs<T>(property, oldValue, value));
            }
        }

        public T GetValue<T>(DependencyProperty<T> property)
        {
            object oldValue;
            if (!_localValueStore.TryGetValue(property, out oldValue))
                return property.DefaultValue;
            return (T)oldValue;
        }
    }

    public class PropertyChangedEventArgs<T> : EventArgs
    {
        public DependencyProperty<T> Property { get; }
        public T OldValue { get; }
        public T NewValue { get; }

        public PropertyChangedEventArgs(DependencyProperty<T> property, T oldValue, T newValue)
        {
            Property = property;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
