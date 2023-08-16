public class DefaultValueSetter
{
    public static object CreateInstanceWithDefaultValues(Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        object instance = Activator.CreateInstance(type);

        foreach (PropertyInfo property in type.GetProperties())
        {
            if (property.CanWrite)
            {
                object defaultValue = GetDefaultValue(property.PropertyType);
                property.SetValue(instance, defaultValue);
            }
        }

        return instance;
    }

    private static object GetDefaultValue(Type propertyType)
    {
        if (propertyType.IsValueType)
        {
            return Activator.CreateInstance(propertyType);
        }
        else if (propertyType == typeof(string))
        {
            return string.Empty;
        }
        else
        {
            return CreateInstanceWithDefaultValues(propertyType);
        }
    }
}
