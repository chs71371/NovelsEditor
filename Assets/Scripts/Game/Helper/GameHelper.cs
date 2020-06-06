using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameHelper 
{
    public static T Alloc<T>(string path) where T : Object
    {
        var res = Resources.Load<T>(path);

        if (res == null)
        {
            return null;
        }

        return GameObject.Instantiate(res) as T;
    }

    public static void Recycle(Object o)
    {
        GameObject.DestroyImmediate(o);
    }

    public static void RestTransform(this Transform trans, Transform parent=null)
    {
        if (parent != null)
        {
            trans.SetParent(parent,false);
        }

        trans.localPosition = Vector3.zero;
        trans.rotation = Quaternion.identity;
        trans.localScale = Vector3.one;
    }
}
