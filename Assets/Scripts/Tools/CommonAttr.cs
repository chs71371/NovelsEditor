using System;
using UnityEngine;

public class ResourcePathAttribute : PropertyAttribute
{
    public Type Type;

    public ResourcePathAttribute(Type t = null)
    {
        Type = t ?? typeof(GameObject);
    }
}
