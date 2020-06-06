using UnityEngine;
 using Sirenix.OdinInspector;
public class MonoSingleton<T> : SerializedMonoBehaviour where T : Component
{
    private static T _instance;
    private static readonly bool AutoCreate = AutoCreateSingletonAttribute.Check<T>();

    public static T Instance
    {
        get
        {
            if (!HasInstance && AutoCreate)
            {
                CreateInstance();
            }
            return _instance;
        }
    }

#if UNITY_EDITOR
    private static bool _refindInstance = false;
#endif
    public static bool HasInstance
    {
        get
        {
#if UNITY_EDITOR // 编辑器下，代码修改后，重编译会导致静态变量重置，这里从新寻找一次instance
            if (_instance == null && !_refindInstance)
            {
                _refindInstance = true;
                _instance = FindObjectOfType(typeof(T)) as T;
            }
#endif
            return _instance != null;
        }
    }

    protected virtual void Awake()
    {
        _instance = this as T;
        Initialize();
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            Uninitialize();
            _instance = null;
        }
    }

    public static void CreateInstance()
    {
        if (!HasInstance)
        {
            var bootObj = GameObject.Find("Boot");
            var go = new GameObject(typeof(T).Name);
            go.transform.SetParent(bootObj == null ? null : bootObj.transform);
            go.AddComponent<T>();
        }
    }

    public static void CreateInstance(System.Type tp)
    {
        if (!HasInstance)
        {
            var bootObj = GameObject.Find("Boot");
            var go = new GameObject(tp.Name);
            go.transform.SetParent(bootObj == null ? null : bootObj.transform);
            go.AddComponent(tp);
        }
    }

    public static void DestroyInstance()
    {
        if (HasInstance)
        {
            Object.DestroyImmediate(_instance.gameObject);
        }
    }

    protected virtual void Initialize()
    {

    }

    protected virtual void Uninitialize()
    {

    }


}