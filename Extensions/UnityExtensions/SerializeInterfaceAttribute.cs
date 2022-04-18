using System;
using UnityEngine;

namespace SZUtilities
{
    public class SerializeInterfaceAttribute : PropertyAttribute
    {
        public Type requiredType { get; private set; }

        public SerializeInterfaceAttribute(Type type)
        {
            requiredType = type;
        }
    }
}

