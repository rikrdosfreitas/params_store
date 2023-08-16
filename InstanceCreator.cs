public class InstanceCreator
{
    public static T CreateInstanceWithDefaults<T>() where T : new()
    {
        T instance = new T();
        PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (PropertyInfo property in properties)
        {
            if (property.CanWrite)
            {
                Type propertyType = property.PropertyType;
                object defaultValue = GetDefaultValue(propertyType);
                property.SetValue(instance, defaultValue);
            }
        }

        return instance;
    }
