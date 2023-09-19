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


using System;
using System.Security.Cryptography;
using System.Text;

public class Program
{
    public static long TextToLong(string texto)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(texto));
            return BitConverter.ToInt64(hashBytes, 0);
        }
    }

    public static void Main()
    {
        string texto = "SeuTextoAqui";
        long valorConvertido = TextToLong(texto);
        Console.WriteLine(valorConvertido);
    }
}
