
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.Extensions.DependencyInjection;

namespace Aws.ssm.init

{
    public class GenericInstanceWithTTL<T>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _ttl;
        private T _instance;
        private DateTime _lastRefreshed;

        public GenericInstanceWithTTL(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _ttl = TimeSpan.FromMinutes(1);
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
            _instance = DynamicClassGenerator.GenerateClassFromInterface<T>();
            LoadValuesFromSource().GetAwaiter().GetResult();
            _lastRefreshed = DateTime.Now;
        }

        private async Task LoadValuesFromSource()
        {
            var ssmClient = _serviceProvider.GetRequiredService<IAmazonSimpleSystemsManagement>();

            PropertyInfo[] properties = typeof(T).GetProperties();

            var itemList = properties.Select(property =>
            {
                var customAttribute = property.GetCustomAttribute<CustomAttribute>();
                return (property, customAttribute);
            }).ToList();

            var tasks = new List<Task<GetParametersResponse>>();

            int batchSize = 1; // Number of elements to take in each iteration
            int skipCount = 0;  // Initial skip count

            do
            {
                var batch = itemList.Skip(skipCount).Take(batchSize).ToList();

                if (batch.Any())
                {
                    var task = GetValueFromSource(batch.Select(x => x.customAttribute.CustomValue).ToList(), ssmClient);
                    tasks.Add(task);
                }

                skipCount += batchSize;

            } while (skipCount < itemList.Count);


            await Task.WhenAll(tasks);

            var list = tasks.SelectMany(x => x.Result.Parameters);

            foreach (var item in list)
            {
                var a = itemList.FirstOrDefault(x => x.customAttribute.CustomValue == item.Name);

                a.property.SetValue(_instance, item.Value);
            }




            //foreach (PropertyInfo property in properties)
            //{
            //    var customAttribute = property.GetCustomAttribute<CustomAttribute>();
            //    //object valueFromSource = GetValueFromSource(property.Name, ssmClient);
            //    //property.SetValue(_instance, valueFromSource);
            //}
        }

        private Task<GetParametersResponse> GetValueFromSource(List<string> parameterName, IAmazonSimpleSystemsManagement ssmClient)
        {
            var request = new GetParametersRequest
            {
                Names = parameterName,
                WithDecryption = false
            };

            return ssmClient.GetParametersAsync(request);
        }


    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CustomAttribute : Attribute
    {
        public string CustomValue { get; }

        public CustomAttribute(string customValue)
        {
            CustomValue = customValue;
        }
    }

    public class DynamicClassGenerator
    {
        public static T GenerateClassFromInterface<T>()
        {

            Type interfaceType = typeof(T);
            var className = interfaceType.Name;

            AssemblyName assemblyName = new AssemblyName(Guid.NewGuid().ToString());
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

            TypeBuilder typeBuilder = moduleBuilder.DefineType(
                className,
                TypeAttributes.Public | TypeAttributes.Class,
                typeof(object),
                new[] { interfaceType }
            );

            foreach (var property in interfaceType.GetProperties())
            {
                FieldBuilder fieldBuilder = typeBuilder.DefineField(
                    "_" + property.Name,
                    property.PropertyType,
                    FieldAttributes.Private
                );

                PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(
                    property.Name,
                    PropertyAttributes.HasDefault,
                    property.PropertyType,
                    null
                );

                MethodBuilder getMethodBuilder = typeBuilder.DefineMethod(
                    "get_" + property.Name,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                    property.PropertyType,
                    Type.EmptyTypes
                );

                ILGenerator getIL = getMethodBuilder.GetILGenerator();
                getIL.Emit(OpCodes.Ldarg_0);
                getIL.Emit(OpCodes.Ldfld, fieldBuilder);
                getIL.Emit(OpCodes.Ret);

                MethodBuilder setMethodBuilder = typeBuilder.DefineMethod(
                    "set_" + property.Name,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                    null,
                    new[] { property.PropertyType }
                );

                ILGenerator setIL = setMethodBuilder.GetILGenerator();
                setIL.Emit(OpCodes.Ldarg_0);
                setIL.Emit(OpCodes.Ldarg_1);
                setIL.Emit(OpCodes.Stfld, fieldBuilder);
                setIL.Emit(OpCodes.Ret);

                propertyBuilder.SetGetMethod(getMethodBuilder);
                propertyBuilder.SetSetMethod(setMethodBuilder);
            }

            return (T)Activator.CreateInstance(typeBuilder.CreateType());
        }
    }

}
