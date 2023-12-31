99public class DefaultValueSetter
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

using System;
using System.Text;

public class Program
{
    public static void Main()
    {
        string textoOriginal = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas vestibulum.";
        
        // Convertendo o texto para Int64
        long numeroConvertido = ConverterTextoParaInt64(textoOriginal);
        Console.WriteLine("Texto convertido para Int64: " + numeroConvertido);
        
        // Convertendo de volta para texto
        string textoRevertido = ConverterInt64ParaTexto(numeroConvertido);
        Console.WriteLine("Int64 revertido para texto: " + textoRevertido);
    }

    public static long ConverterTextoParaInt64(string texto)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(texto);
        long resultado = BitConverter.ToInt64(bytes, 0);
        return resultado;
    }

    public static string ConverterInt64ParaTexto(long numero)
    {
        byte[] bytes = BitConverter.GetBytes(numero);
        string texto = Encoding.UTF8.GetString(bytes);
        return texto;
    }
}


using System;
using System.Security.Cryptography;
using System.Text;

public class Program
{
    public static Guid TextToGuid(string input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return new Guid(hashBytes);
        }
    }

    public static string GuidToText(Guid guid)
    {
        return guid.ToString();
    }

    public static void Main()
    {
        string textoOriginal = "Exemplo de texto com até 255 caracteres";
        Guid guidGerado = TextToGuid(textoOriginal);
        string textoRecuperado = GuidToText(guidGerado);

        Console.WriteLine("Texto Original: " + textoOriginal);
        Console.WriteLine("GUID Gerado: " + guidGerado);
        Console.WriteLine("Texto Recuperado: " + textoRecuperado);
    }
}

