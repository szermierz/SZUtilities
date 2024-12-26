using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SZUtilities
{
    public static class Reflection
    {
        public const BindingFlags AllInstanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static Dictionary<Type, FieldInfo[]> s_fieldsCache = new Dictionary<Type, FieldInfo[]>();

        private static Dictionary<Type, MethodInfo[]> s_methodsCache = new Dictionary<Type, MethodInfo[]>();

        public static MethodInfo[] GetAllMethods(Type type)
        {
            if (s_methodsCache.ContainsKey(type))
                return s_methodsCache[type];

            var methods = type.GetMethods(AllInstanceFlags);
            if (type.BaseType is Type baseType)
            {
                methods = methods
                    .Concat(GetAllMethods(baseType))
                    .Distinct()
                    .ToArray();
            }

            s_methodsCache.Add(type, methods);
            return methods;
        }

        public static MethodInfo GetMethod(Type type, string methodName)
        {
            var methods = GetAllMethods(type);
            foreach (var methodInfo in methods)
            {
                if (methodInfo.Name == methodName)
                    return methodInfo;
            }
            return null;
        }

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

        public static List<Type> FindDerived(Type baseType, params Assembly[] assemblies)
        {
            return assemblies
                .SelectMany(_assembly => _assembly.GetTypes())
                .Where(_type => _type != baseType && baseType.IsAssignableFrom(_type))
                .ToList();
        }

        public static List<Type> FindDerived<T>()=> FindDerived<T>(AppDomain.CurrentDomain.GetAssemblies());

        public static List<Type> FindDerived<T>(params Assembly[] assemblies) => FindDerived(typeof(T), assemblies);

        public static IEnumerable<KeyValuePair<Type, Attribute>> FindAllTypesWithAttribute(Type attributeType, Assembly[] assemblies = null)
        {
            if (assemblies == null)
                assemblies = AppDomain.CurrentDomain.GetAssemblies();

            return assemblies
                .SelectMany(_assembly => _assembly.GetTypes())
                .Select(_type => new KeyValuePair<Type, Attribute>(_type, Attribute.GetCustomAttribute(_type, attributeType)))
                .Where(pair => pair.Value != null);
        }

        public static IEnumerable<KeyValuePair<Type, AttribType>> FindAllTypesWithAttribute<AttribType>(Assembly[] assemblies = null)
            where AttribType : Attribute
        {
            return FindAllTypesWithAttribute(typeof(AttribType), assemblies)
                .Where(pair => pair.Value is AttribType)
                .Select(pair => new KeyValuePair<Type, AttribType>(pair.Key, (AttribType)pair.Value));
        }

        public static object CreateObject(Type type)
        {
#if UNITY_5_3_OR_NEWER
            if (type.IsSubclassOf(typeof(UnityEngine.ScriptableObject)))
                return UnityEngine.ScriptableObject.CreateInstance(type);
#endif
            return Activator.CreateInstance(type);
        }

        public static ObjectType CreateObject<ObjectType>(Type type)
            where ObjectType : class
        {
            if (!typeof(ObjectType).IsAssignableFrom(type))
                throw new Exception();

            return (ObjectType)CreateObject(type);
        }
    }
}

