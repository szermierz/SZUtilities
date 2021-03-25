using System;
using UnityEngine;

public class SerializeInterfaceAttribute : PropertyAttribute
{
    public System.Type requiredType { get; private set; }
    
    public SerializeInterfaceAttribute(Type type)
    {
        requiredType = type;
    }
}

