using System;
 
public class Singleton<T> where T : Singleton<T>, new()
{
    private static T _instance;
    private static readonly bool AutoCreate = AutoCreateSingletonAttribute.Check<T>();

    protected Singleton()
    {

    }

    public static T Instance
    {
        get
        {
            if (_instance == null && AutoCreate)
            {
                CreateInstance();
            }
            return _instance;
        }
    }

    public static bool HasInstance
    {
        get
        {
            return _instance != null;
        }
    }

    public static implicit operator bool(Singleton<T> t)
    {
        return t != null;
    }

    public static T CreateInstance()
    {
        if (_instance == null)
        {
#if UNITY_EDITOR
            SingletonDestroyHelper.DestroyList += DestroyInstance;
#endif
            _instance = new T();
            (_instance).Initialize();
        }
        return _instance;
    }

    public static void DestroyInstance()
    {
        if (_instance != null)
        {
            (_instance).Uninitialize();
            _instance = null;
#if UNITY_EDITOR
            SingletonDestroyHelper.DestroyList -= DestroyInstance;
#endif
        }
    }

    protected virtual void Initialize()
    {

    }

    protected virtual void Uninitialize()
    {

    }
}

#if UNITY_EDITOR
public static class SingletonDestroyHelper
{
    public static Action DestroyList;

    public static void Destroy()
    {
        if (DestroyList != null)
        {
            DestroyList();
        }
    }
}
#endif
