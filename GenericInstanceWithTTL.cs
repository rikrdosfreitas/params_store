using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace GenericInstanceWithTTL
{
    public class GenericInstanceWithTTL<T>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _ttl;
        private T _instance;
        private DateTime _lastRefreshed;

        public GenericInstanceWithTTL(IServiceProvider serviceProvider, TimeSpan ttl)
        {
            _serviceProvider = serviceProvider;
            _ttl = ttl;
            RefreshInstance();
        }

        public T Instance
        {
            get
            {
                if (DateTime.Now - _lastRefreshed > _ttl)
                {
                    RefreshInstance();
                }
                return _instance;
            }
        }

        private void RefreshInstance()
        {
            _instance = _serviceProvider.GetRequiredService<T>();
            LoadValuesFromSource();
            _lastRefreshed = DateTime.Now;
        }

        private void LoadValuesFromSource()
        {
            // Simulate loading values from a source and setting properties
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                // Simulate getting values from a source and setting properties
                // Replace this with actual logic to get and set values from your source
                object valueFromSource = GetValueFromSource(property.Name);
                property.SetValue(_instance, valueFromSource);
            }
        }

        private object GetValueFromSource(string propertyName)
        {
            // Simulate getting values from a source
            // Replace this with actual logic to get values from your source
            return null;
        }
    }
}
