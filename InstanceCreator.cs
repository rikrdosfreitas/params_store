public class InstanceCreator
{
    public static object CreateInstanceWithDefaults(Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        // Check if the type has a default (parameterless) constructor
        if (type.GetConstructor(Type.EmptyTypes) == null)
        {
            throw new InvalidOperationException($"Type {type.Name} does not have a parameterless constructor.");
        }

        object instance = Activator.CreateInstance(type);

        // Set default values for properties with private setters
        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (PropertyInfo property in properties)
        {
            if (property.CanWrite)
            {
                // If property has a private setter
                MethodInfo setter = property.GetSetMethod(nonPublic: true);
                if (setter != null)
                {
                    object defaultValue = GetDefaultValue(property.PropertyType);
                    property.SetValue(instance, defaultValue);
                }
            }
        }

        return instance;
    }

    private static object GetDefaultValue(Type type)
    {
        if (type.IsValueType)
        {
            return Activator.CreateInstance(type);
        }
        else
        {
            return null;
        }
    }
}
