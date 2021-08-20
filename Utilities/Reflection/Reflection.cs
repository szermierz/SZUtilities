using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SZUtilities
{
    public static class Reflection
    {
        public const BindingFlags AllInstanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        
        private static Dictionary<Type, FieldInfo[]> s_fieldsCache = new Dictionary<Type, FieldInfo[]>();

        public static FieldInfo[] GetAllFields(Type type)
        {
            if (s_fieldsCache.ContainsKey(type))
                return s_fieldsCache[type];
            
            var fields = type.GetFields(AllInstanceFlags);
            if (type.BaseType is Type baseType)
            {
                fields = fields
                    .Concat(GetAllFields(baseType))
                    .Distinct()
                    .ToArray();
            }
            
            s_fieldsCache.Add(type, fields);
            return fields;
        }
        
        public static FieldInfo GetField(Type type, string fieldName)
        {
            var fields = GetAllFields(type);
            foreach (var fieldInfo in fields)
            {
                if (fieldInfo.Name == fieldName)
                    return fieldInfo;
            }
            return null;
        }
        
        public static Type FindType(string name) => AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(_assembly => _assembly.GetTypes())
            .FirstOrDefault(_type => _type.FullName == name);

        public static List<Type> FindDerived<T>()=> FindDerived<T>(AppDomain.CurrentDomain.GetAssemblies());

        public static List<Type> FindDerived<T>(Assembly assembly) => assembly
            .GetTypes()
            .Where(_type => _type != typeof(T) && _type.IsSubclassOf(typeof(T)))
            .ToList();

        public static List<Type> FindDerived<T>(Assembly[] assemblies) => assemblies
            .SelectMany(_assembly => _assembly.GetTypes())
            .Where(_type => _type != typeof(T) && _type.IsSubclassOf(typeof(T)))
            .ToList();

        public static IEnumerable<KeyValuePair<Type, AttribType>> FindAllTypesWithAttribute<AttribType>(Assembly[] assemblies = null)
            where AttribType : Attribute
        {
            if (assemblies == null)
                assemblies = AppDomain.CurrentDomain.GetAssemblies();

            return assemblies
                .SelectMany(_assembly => _assembly.GetTypes())
                .Select(_type => new KeyValuePair<Type, AttribType>(_type, Attribute.GetCustomAttribute(_type, typeof(AttribType)) as AttribType))
                .Where(pair => pair.Value != null);
        }

        public static object CreatetObject<T>()
            where T : class
        {
            return CreateObject(typeof(T));
        }

        public static object CreateObject(Type type)
        {
            if (type.IsSubclassOf(typeof(ScriptableObject)))
                return ScriptableObject.CreateInstance(type);

            return Activator.CreateInstance(type);
        }
    }
}

