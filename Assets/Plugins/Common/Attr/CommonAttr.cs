using System;
using System.Reflection;
using UnityEngine;

public static class AttributeExt
{
    public static T GetFirstAttr<T>(this MemberInfo m, bool inherit = true) where T : Attribute
    {
        var attrs = m.GetCustomAttributes(typeof(T), inherit);
        if (attrs.Length == 0)
        {
            return null;
        }
        return attrs[0] as T;
    }
    public static bool HasAttr<T>(this MemberInfo m, bool inherit = true) where T : Attribute
    {
        return m.GetCustomAttributes(typeof(T), inherit).Length > 0;
    }
}

public class AutoCreateSingletonAttribute : Attribute
{
    public static bool Check<T>()
    {
        return typeof(T).GetFirstAttr<AutoCreateSingletonAttribute>() != null;
    }
}