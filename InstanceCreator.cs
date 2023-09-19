89public class DefaultValueSetter
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

class Program
{
    static void Main()
    {
        string textoOriginal = "Lorem ipsum dolor sit amet... (255 caracteres)";

        // Convertendo o texto para um GUID
        Guid guid = Guid.NewGuid();
        string guidString = Convert.ToBase64String(GuidToBytes(guid));

        Console.WriteLine("Texto Original: " + textoOriginal);
        Console.WriteLine("GUID: " + guidString);

        // Convertendo de volta para o texto original
        Guid novoGuid = new Guid(Convert.FromBase64String(guidString));
        string novoTexto = BytesToString(GuidToBytes(novoGuid));

        Console.WriteLine("Novo Texto: " + novoTexto);
    }

    // Método para converter um GUID em um array de bytes
    static byte[] GuidToBytes(Guid guid)
    {
        return guid.ToByteArray();
    }

    // Método para converter um array de bytes em uma string
    static string BytesToString(byte[] bytes)
    {
        return System.Text.Encoding.UTF8.GetString(bytes);
    }
}

public class TextToGuidConverter
{
    public static Guid ConvertTextToGuid(string text)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(text.PadRight(255)); // Garante que o texto tenha exatamente 255 caracteres
        return new Guid(bytes);
    }

    public static string ConvertGuidToText(Guid guid)
    {
        byte[] bytes = guid.ToByteArray();
        string text = Encoding.UTF8.GetString(bytes);
        return text.TrimEnd('\0'); // Remove os caracteres nulos que podem ter sido adicionados no processo de conversão.
    }
}

string textoOriginal = "Este é um texto de exemplo com 255 caracteres."; // 47 caracteres
Guid guid = TextToGuidConverter.ConvertTextToGuid(textoOriginal);

string textoDeVolta = TextToGuidConverter.ConvertGuidToText(guid);

Console.WriteLine(textoDeVolta);
ok 
