 
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputCallback
{
    public UnityAction ClickCallBack;
    public UnityAction PressCallBack;
    public UnityAction LongPressCallBack;
    public UnityAction DragCallBack;
    public UnityAction PointEnterCallBack;
    public UnityAction PointExitCallBack;
    public UnityAction DoubleClickCallBack;
    public UnityAction<InputListener.State> CancelCallBack;
}


[AutoCreateSingleton]
public class InputListenerManager : MonoSingleton<InputListenerManager>
{
    public enum PriorityType
    {
        None = 0,
        SceneObj = 1,
        SceneTrigger = 2,
        SceneTop = 3,
        UIBase = 9,
        UI = 10,
        UITigger = 11,
    }

    public readonly List<PriorityType> PrioritySort = new List<PriorityType>()
    {
        PriorityType.UITigger,
        PriorityType.UI,
        PriorityType.UIBase,
        PriorityType.SceneTop,
        PriorityType.SceneTrigger,
        PriorityType.SceneObj,
    };


    public bool IsPressing
    {
        get
        {
            return IsPress;
        }
    }

    public Vector2 CurrentPressPos
    {
        get
        {
            if (Input.touchCount > 0)
            {
                return Input.GetTouch(0).position;
            }

            return Input.mousePosition;
        }
    }

    private GraphicRaycaster _graphicRaycaster;


    public static readonly float LongPressTime = 0.4f;
    public static readonly float DoubleTime = 0.4f;

    /// <summary>
    /// 类型转监听事件字典
    /// </summary>
    private Dictionary<Type, InputListener> _typeToInputListenerDict = new Dictionary<Type, InputListener>();
    /// <summary>
    /// 游戏物体转监听事件字典
    /// </summary>
    private Dictionary<GameObject, InputListener> _objectToInputListenerDict = new Dictionary<GameObject, InputListener>();
    /// <summary>
    /// 类型转移入选择事件字典
    /// </summary>
    private Dictionary<Type, InputListener> _typeToSelectListenerDict = new Dictionary<Type, InputListener>();
    /// <summary>
    /// 游戏物体转移入选择事件字典
    /// </summary>
    private Dictionary<GameObject, InputListener> _objectToSelectListenerDict = new Dictionary<GameObject, InputListener>();
    /// <summary>
    /// 类型转优先级字典
    /// </summary>
    private Dictionary<Type, PriorityType> _typeToPriorityDict = new Dictionary<Type, PriorityType>();
    /// <summary>
    /// 游戏物体转优先级字典
    /// </summary>
    private Dictionary<GameObject, PriorityType> _objectToPriorityDict = new Dictionary<GameObject, PriorityType>();
    /// <summary>
    /// 忽略物体列表
    /// </summary>
    private List<GameObject> _ignoreList = new List<GameObject>();

    private List<RaycastHit> _raycastHitCache = new List<RaycastHit>();

    private Vector2 _lastMousePosition = Vector2.zero;
    private float _detalLenth;
    private float _thresholdLenth = 20;
    public bool IsDrag { private set; get; }
    public bool IsPress { private set; get; }
    public bool IsLongPress { private set; get; }

    private float _pressDownTimeRecord = 0;
    private int _clickCount = 0;
    private Vector2 _doubleClickPos;

    public Type CurSelectObjectType;
    public GameObject CurSelectObject;

    public bool IsOpenAllInput = true;
    public bool IsOpenDrag = true;
    public bool IsOpenLongPress = true;
    public bool IsOpenDouble = true;

    public List<UnityAction> TouchUpEvents = new List<UnityAction>();
    public List<UnityAction> TouchDownEvents = new List<UnityAction>();
    public static UnityAction TouchLongPressCallback;
    public static UnityAction TouchDoubleCallback;
    public static UnityAction<Vector2> TouchDragCallback;

    private bool _isFreshRegisterInfo = false;

    protected override void Initialize()
    {
        _thresholdLenth = 10 * Mathf.Max(1, Mathf.RoundToInt((float)Screen.height / 640));
        RegisterInputEvent(typeof(UIInputBlock), new InputCallback(), PriorityType.UI);

        _touchPhaseData.TouchPhase = TouchPhaseData.ETouchPhase.Up;
        _touchPhaseData.IsAction = false;
    }

    protected override void Uninitialize()
    {
        UnInputRegister(typeof(UIInputBlock));
    }



    public static void RegisterInputEvent(GameObject target, InputCallback callback, PriorityType priority = PriorityType.UI)
    {
        if (Instance != null)
        {
            if (Instance._objectToInputListenerDict.ContainsKey(target))
            {
                Instance._objectToInputListenerDict.Remove(target);
                Instance._objectToPriorityDict.Remove(target);
            }
            Instance._objectToInputListenerDict.Add(target, new InputListener() { Callback = callback });
            Instance._objectToPriorityDict.Add(target, priority);
            Instance._isFreshRegisterInfo = true;

        }
    }

    /// <summary>
    /// 输入事件优先级修改
    /// </summary>
    public static void InputEventChangePriority(GameObject target, PriorityType priority)
    {
        if (Instance != null)
        {
            if (Instance._objectToPriorityDict.ContainsKey(target))
            {
                Instance._objectToPriorityDict[target] = priority;
            }
        }
    }



    public static void RegisterInputEvent(Type target, InputCallback callback, PriorityType priority = PriorityType.UI)
    {
        if (Instance != null)
        {
            if (Instance._typeToInputListenerDict.ContainsKey(target))
            {
                Instance._typeToInputListenerDict.Remove(target);
                Instance._typeToPriorityDict.Remove(target);
            }
            Instance._typeToInputListenerDict.Add(target, new InputListener() { Callback = callback });
            Instance._typeToPriorityDict.Add(target, priority);
        }
    }

    public static void RegisterSelectEvent(GameObject target, InputCallback callback)
    {
        if (Instance != null)
        {
            if (Instance._objectToSelectListenerDict.ContainsKey(target))
            {
                Instance._objectToSelectListenerDict.Remove(target);
            }

            Instance._objectToSelectListenerDict.Add(target, new InputListener() { Callback = callback });

            Instance._isFreshRegisterInfo = true;
        }
    }



    public static void UnInputRegister(Type target)
    {
        if (HasInstance)
        {
            Instance._typeToInputListenerDict.Remove(target);
            Instance._typeToPriorityDict.Remove(target);
        }
    }

    public static void UnInputRegister(GameObject target)
    {
        if (HasInstance)
        {
            Instance._objectToInputListenerDict.Remove(target);
            Instance._objectToPriorityDict.Remove(target);
        }
    }

    public static void UnSelectRegister(GameObject target)
    {
        if (HasInstance)
        {
            Instance._objectToSelectListenerDict.Remove(target);
        }
    }

    public static void RegisterTouchDownEvent(UnityAction callback)
    {
        if (Instance != null)
        {
            if (callback != null && !Instance.TouchDownEvents.Contains(callback))
            {
                Instance.TouchDownEvents.Add(callback);
            }
        }
    }

    public static void UnRegisterTouchDownEvent(UnityAction callback)
    {
        if (HasInstance)
        {
            if (callback != null)
            {
                Instance.TouchDownEvents.Remove(callback);
            }
        }
    }

    public static void RegisterTouchUpEvent(UnityAction callback)
    {
        if (Instance != null)
        {
            if (callback != null && !Instance.TouchUpEvents.Contains(callback))
            {
                Instance.TouchUpEvents.Add(callback);
            }
        }
    }

    public static void UnRegisterTouchUpEvent(UnityAction callback)
    {
        if (HasInstance)
        {
            if (callback != null)
            {
                Instance.TouchUpEvents.Remove(callback);
            }
        }
    }

    public static void RegisterIgnoreObject(GameObject obj)
    {
        if (Instance != null)
        {
            if (obj != null)
            {
                Instance._ignoreList.Add(obj);
            }
        }
    }

    public static void UnRegisterIgnoreObject(GameObject obj)
    {
        if (Instance != null)
        {
            if (obj != null)
            {
                Instance._ignoreList.Remove(obj);
            }
        }
    }

    private Dictionary<GameObject, InputListener> _tmpDict = new Dictionary<GameObject, InputListener>();
    private void CheckVaidObjectDict(Dictionary<GameObject, InputListener> checkDict)
    {
        _tmpDict.Clear();
        foreach (var info in checkDict)
        {
            if (info.Key != null)
            {
                _tmpDict.Add(info.Key, info.Value);
            }
        }
        checkDict.Clear();
        foreach (var info in _tmpDict)
        {
            checkDict.Add(info.Key, info.Value);
        }

        _tmpDict.Clear();
    }


    public Vector3 GetInputMouseWorldPos()
    {
        return UIRoot.Instance.UICamera.ScreenToWorldPoint(CurrentPressPos);
    }


    private const int RaycastHistArrayLength = 64;
    private readonly RaycastHit[] _raycastCache = new RaycastHit[RaycastHistArrayLength];
    private readonly float[] _raycastHitDistCache = new float[RaycastHistArrayLength];
    Collider DoRayCast(Vector2 screenPos, int layerMask = -1, System.Predicate<Collider> filter = null)
    {
        var mainCam = Camera.main;
        if (mainCam == null)
        {
            return null;
        }
        var ray = mainCam.ScreenPointToRay(screenPos);
        if (!Physics.autoSimulation)
        {
            Physics.Simulate(Time.deltaTime);
        }
        int castCnt = Physics.RaycastNonAlloc(ray, _raycastCache, Mathf.Infinity, layerMask);
        if (castCnt > 0)
        {
            int validCnt = 0;
            for (int i = 0; i < castCnt; ++i)
            {
                var hit = _raycastHitCache[i];
                if (filter == null || filter(hit.collider))
                {
                    if (validCnt < i)
                    {
                        _raycastHitCache[validCnt] = _raycastHitCache[i];
                    }
                    _raycastHitDistCache[validCnt] = (ray.origin - _raycastHitCache[i].point).sqrMagnitude;
                    ++validCnt;
                }
            }
            if (validCnt > 0)
            {
                Array.Sort(_raycastHitDistCache, _raycastCache, 0, validCnt);
                return _raycastHitCache[0].collider;
            }
        }
        return null;
    }


    public GameObject GetSelectObject(params Type[] args)
    {
        List<GameObject> targetList = new List<GameObject>();
        var uiList = GetCurSelectUIList();

        if (uiList.Count > 0)
        {
            for (int i = 0; i < uiList.Count; i++)
            {
                if (uiList[i].gameObject != null)
                {
                    targetList.Add(uiList[i].gameObject);
                }
            }
        }

        var argList = args.ToList();


        var pos = Input.mousePosition;
        if (Input.GetMouseButton(0) || Input.touchCount == 0)
        {

        }
        else
        {
            var touch = Input.GetTouch(0);
            pos = touch.position;
        }

        for (int i = 0; i < argList.Count; i++)
        {
            var obj = DoRayCast(pos, -1, (cld) => cld.GetComponent(argList[i]) != null);
            if (obj != null)
            {
                targetList.Add(obj.gameObject);
            }
        }

        //剔除忽略列表
        for (int i = 0; i < _ignoreList.Count; i++)
        {
            var obj = _ignoreList[i];
            if (targetList.Contains(obj))
            {
                targetList.Remove(obj);
            }
        }

        for (int i = 0; i < argList.Count; i++)
        {
            var type = argList[i];

            var find = targetList.Find(o => o != null && o.GetComponent(type) != null);

            if (find != null)
            {
                return find;
            }
        }

        return null;
    }

    private List<GameObject> FindPriorityType(Dictionary<GameObject, PriorityType> objDict, PriorityType tp)
    {
        List<GameObject> newList = new List<GameObject>();
        foreach (var info in objDict)
        {
            if (info.Key != null && info.Value == tp)
            {
                newList.Add(info.Key);
            }
        }
        return newList;
    }

    private List<Type> FindPriorityType(Dictionary<Type, PriorityType> objDict, PriorityType tp)
    {
        List<Type> newList = new List<Type>();
        foreach (var info in objDict)
        {
            if (info.Value == tp)
            {
                newList.Add(info.Key);
            }
        }
        return newList;
    }


    public KeyValuePair<Type, GameObject> GetSelectObject()
    {
        var targetList = GetCurSelectObject();

        for (int i = 0; i < PrioritySort.Count; i++)
        {
            var tp = PrioritySort[i];

            var typeList = FindPriorityType(_typeToPriorityDict, tp);
            var objList = FindPriorityType(_objectToPriorityDict, tp);

            for (int j = 0; j < targetList.Count; j++)
            {
                var targetObj = targetList[j];

                for (int z = 0; z < typeList.Count; z++)
                {
                    var checkType = typeList[z];

                    if (targetObj != null && targetObj.GetComponent(checkType) != null)
                    {
                        return new KeyValuePair<Type, GameObject>(checkType, targetObj);
                    }
                }

                for (int z = 0; z < objList.Count; z++)
                {
                    var checkObj = objList[z];

                    if (checkObj == targetObj)
                    {
                        return new KeyValuePair<Type, GameObject>(null, targetObj);
                    }
                }
            }


        }

        return new KeyValuePair<Type, GameObject>(null, null);
    }


    private float _curPressTime;

    private TouchPhaseData _touchPhaseData;

    public struct TouchPhaseData
    {
        public enum ETouchPhase
        {
            Up,
            Down,
        }
        public ETouchPhase TouchPhase;
        public bool IsAction;
    }

    private void RefreshTouchPhase()
    {
#if UNITY_EDITOR||UNITY_STANDALONE
        if (Input.GetMouseButton(0))
        {
            if (_touchPhaseData.TouchPhase == TouchPhaseData.ETouchPhase.Up)
            {
                _touchPhaseData.IsAction = false;
                _touchPhaseData.TouchPhase = TouchPhaseData.ETouchPhase.Down;
                return;
            }
        }
        else
        {
            if (_touchPhaseData.TouchPhase == TouchPhaseData.ETouchPhase.Down)
            {
                _touchPhaseData.IsAction = false;
                _touchPhaseData.TouchPhase = TouchPhaseData.ETouchPhase.Up;
                return;
            }
        }
#else
        if (Input.touchCount > 0)
        {
            if (_touchPhaseData.TouchPhase == TouchPhaseData.ETouchPhase.Up)
            {
                _touchPhaseData.IsAction = false;
                _touchPhaseData.TouchPhase = TouchPhaseData.ETouchPhase.Down;
                return;
            }
        }
        else
        {
            if (_touchPhaseData.TouchPhase == TouchPhaseData.ETouchPhase.Down)
            {
                _touchPhaseData.IsAction = false;
                _touchPhaseData.TouchPhase = TouchPhaseData.ETouchPhase.Up;
                return;
            }
        }
#endif
    }


    public void Update()
    {
        if (!IsOpenAllInput)
        {
            if (IsPress)
            {
                OnPointerUp(true);
            }
            return;
        }

        UnityEngine.Profiling.Profiler.BeginSample("InputManager.Update 1");
        RefreshTouchPhase();
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("InputManager.Update 2");
        if (_touchPhaseData.TouchPhase == TouchPhaseData.ETouchPhase.Down
            && !_touchPhaseData.IsAction)
        {
            _touchPhaseData.IsAction = true;
            //若点击时,按压事件未清理，则先释放抬起事件
            if (IsPress)
            {
                UnityEngine.Profiling.Profiler.BeginSample("InputManager.Update OnPointerUp");
                OnPointerUp();
                UnityEngine.Profiling.Profiler.EndSample();
            }

            UnityEngine.Profiling.Profiler.BeginSample("InputManager.Update OnPointerDown");
            OnPointerDown();
            UnityEngine.Profiling.Profiler.EndSample();
        }
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("InputManager.Update 3");
        if (_touchPhaseData.TouchPhase == TouchPhaseData.ETouchPhase.Up
            && !_touchPhaseData.IsAction)
        {
            _touchPhaseData.IsAction = true;
            OnPointerUp();
        }
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("InputManager.Update 4");
        if (IsPress)
        {
            OnLongPress();
            CheckPointEnter();
            OnDrag();
        }
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("InputManager.Update 5");
        //当前帧结束后清理射线结果
        _uiCacheRaycastResults.Clear();
        _raycastHitCache.Clear();
        //清理无效字典元素
        if (_isFreshRegisterInfo)
        {
            _isFreshRegisterInfo = false;
            CheckVaidObjectDict(_objectToInputListenerDict);
            CheckVaidObjectDict(_objectToSelectListenerDict);
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }


    public GameObject CurTopClickObj;

    private void FreshCurClickTopObj()
    {
        CurTopClickObj = null;
        if (_uiCacheRaycastResults.Count > 0)
        {
            CurTopClickObj = _uiCacheRaycastResults[0].gameObject;
        }
        else
        {
            if (_raycastHitCache.Count > 0)
            {
                CurTopClickObj = _raycastHitCache[0].collider.gameObject;
            }
        }
    }

    private void OnPointerDown()
    {
        var touchPostion = CurrentPressPos;
        IsPress = true;

        if (_lastMousePosition == Vector2.zero)
        {
            _lastMousePosition = touchPostion;
        }

        UnityEngine.Profiling.Profiler.BeginSample("InputManager.OnPointerDown GetSelectObject");
        var keyValue = GetSelectObject();
        UnityEngine.Profiling.Profiler.EndSample();

        CurSelectObjectType = keyValue.Key;
        CurSelectObject = keyValue.Value;
        UnityEngine.Profiling.Profiler.BeginSample("InputManager.OnPointerDown FreshCurClickTopObj");
        FreshCurClickTopObj();
        UnityEngine.Profiling.Profiler.EndSample();

        for (int i = 0; i < TouchDownEvents.Count; i++)
        {
            try
            {
                if (TouchDownEvents[i] != null)
                {
                    UnityEngine.Profiling.Profiler.BeginSample("InputManager.OnPointerDown TouchDownEvents[" + i + "]()");
                    TouchDownEvents[i]();
                    UnityEngine.Profiling.Profiler.EndSample();
                }
            }
            catch (Exception e)
            {
                LogHandler.Instance.AddException(e);
            }
        }


        UnityEngine.Profiling.Profiler.BeginSample("InputManager.OnPointerDown CheckDoubleClick");
        CheckDoubleClick();
        UnityEngine.Profiling.Profiler.EndSample();


        if (CurSelectObject != null)
        {
            if (_objectToInputListenerDict.TryGetValue(CurSelectObject, out var objectToInputListener))
            {
                try
                {
                    UnityEngine.Profiling.Profiler.BeginSample("InputManager.OnPointerDown objectToInputListener");
                    objectToInputListener.OnPointerDown();
                    UnityEngine.Profiling.Profiler.EndSample();
                }
                catch (Exception e)
                {
                    LogHandler.Instance.AddException(e);
                }
            }

            if (CurSelectObjectType != null && _typeToInputListenerDict.TryGetValue(CurSelectObjectType, out var typeToInputListener))
            {
                try
                {
                    UnityEngine.Profiling.Profiler.BeginSample("InputManager.OnPointerDown typeToInputListener " + CurSelectObjectType);
                    typeToInputListener.OnPointerDown();
                    UnityEngine.Profiling.Profiler.EndSample();
                }
                catch (Exception e)
                {
                    LogHandler.Instance.AddException(e);
                }
            }
        }
    }

    private void OnPointerUp(bool isClear = false)
    {
        if (!isClear)
        {
            OnDoubleClick();
        }

        IsPress = false;
        IsDrag = false;
        IsLongPress = false;

        if (CurSelectObject != null)
        {
            if (_objectToInputListenerDict.ContainsKey(CurSelectObject))
            {
                try
                {
                    _objectToInputListenerDict[CurSelectObject].OnPointerUp();
                }
                catch (Exception e)
                {
                    LogHandler.Instance.AddException(e);
                }
            }

            if (CurSelectObjectType != null && _typeToInputListenerDict.ContainsKey(CurSelectObjectType))
            {
                try
                {
                    _typeToInputListenerDict[CurSelectObjectType].OnPointerUp();
                }
                catch (Exception e)
                {
                    LogHandler.Instance.AddException(e);
                }

            }
        }

        if (!isClear)
        {
            for (int i = 0; i < TouchUpEvents.Count; i++)
            {
                try
                {
                    if (TouchUpEvents[i] != null)
                    {
                        TouchUpEvents[i]();
                    }
                }
                catch (Exception e)
                {
                    LogHandler.Instance.AddException(e);
                }
            }
        }

        _lastMousePosition = Vector2.zero;
        _detalLenth = 0;
        _curPressTime = 0;
        CurSelectObject = null;
        CurSelectObjectType = null;
        ClearSelectInfo();
    }


    private void OnLongPress()
    {
        if (IsOpenLongPress && !IsLongPress && !IsDrag)
        {
            _curPressTime += Time.deltaTime;
            if (_curPressTime > LongPressTime)
            {
                IsLongPress = true;

                if (CurSelectObject != null)
                {
                    if (_objectToInputListenerDict.ContainsKey(CurSelectObject))
                    {
                        try
                        {
                            _objectToInputListenerDict[CurSelectObject].OnLongPress();
                        }
                        catch (Exception e)
                        {
                            LogHandler.Instance.AddException(e);
                        }
                    }

                    if (CurSelectObjectType != null && _typeToInputListenerDict.ContainsKey(CurSelectObjectType))
                    {
                        try
                        {
                            _typeToInputListenerDict[CurSelectObjectType].OnLongPress();
                        }
                        catch (Exception e)
                        {
                            LogHandler.Instance.AddException(e);
                        }
                    }
                }

                if (TouchLongPressCallback != null)
                {
                    try
                    {
                        TouchLongPressCallback();
                    }
                    catch (Exception e)
                    {
                        LogHandler.Instance.AddException(e);
                    }
                }
            }
        }
    }

    private void OnDrag()
    {
        Vector2 touchPostion = CurrentPressPos;
        var movedelta = touchPostion - _lastMousePosition;
        _lastMousePosition = touchPostion;
        if (!IsDrag)
        {
            _detalLenth += movedelta.magnitude;
            if (_detalLenth > _thresholdLenth)
            {
                IsDrag = true;
            }
        }

        if (IsDrag && IsOpenDrag)
        {
            if (CurSelectObject != null)
            {
                if (_objectToInputListenerDict.ContainsKey(CurSelectObject))
                {
                    try
                    {
                        _objectToInputListenerDict[CurSelectObject].OnDrag();
                    }
                    catch (Exception e)
                    {
                        LogHandler.Instance.AddException(e);
                    }

                }

                if (CurSelectObjectType != null && _typeToInputListenerDict.ContainsKey(CurSelectObjectType))
                {
                    try
                    {
                        _typeToInputListenerDict[CurSelectObjectType].OnDrag();
                    }
                    catch (Exception e)
                    {
                        LogHandler.Instance.AddException(e);
                    }
                }
            }

            if (TouchDragCallback != null)
            {
                try
                {
                    TouchDragCallback(movedelta);
                }
                catch (Exception e)
                {
                    LogHandler.Instance.AddException(e);
                }
            }
        }
    }



    private int _doubleCacheInstanceId;
    private bool CheckDoubleClickOverTime()
    {
        return Time.realtimeSinceStartup - _pressDownTimeRecord > DoubleTime;
    }
    private bool CheckDoubleClickOverSize()
    {
        return Vector2.Distance(_doubleClickPos, CurrentPressPos) > _thresholdLenth * 4;
    }
    private void CheckDoubleClick()
    {
        var topInstance = CurTopClickObj == null ? 0 : CurTopClickObj.GetInstanceID();

        if (CheckDoubleClickOverTime()
            || CheckDoubleClickOverSize()
            || _doubleCacheInstanceId != topInstance)
        {
            _clickCount = 1;
            _doubleCacheInstanceId = topInstance;
            _pressDownTimeRecord = Time.realtimeSinceStartup;
            _doubleClickPos = CurrentPressPos;
            return;
        }

        if (_doubleCacheInstanceId == topInstance)
        {
            _clickCount = _clickCount + 1;
        }

        _doubleCacheInstanceId = topInstance;
    }

    private void OnDoubleClick()
    {
        if (IsOpenDouble && _clickCount >= 2 && !CheckDoubleClickOverTime() && !CheckDoubleClickOverSize())
        {
            _clickCount = 0;

            if (CurSelectObject != null)
            {
                if (_objectToInputListenerDict.ContainsKey(CurSelectObject))
                {
                    try
                    {
                        _objectToInputListenerDict[CurSelectObject].OnDoubleClick();
                    }
                    catch (Exception e)
                    {
                        LogHandler.Instance.AddException(e);
                    }

                }

                if (CurSelectObjectType != null && _typeToInputListenerDict.ContainsKey(CurSelectObjectType))
                {
                    try
                    {
                        _typeToInputListenerDict[CurSelectObjectType].OnDoubleClick();
                    }
                    catch (Exception e)
                    {
                        LogHandler.Instance.AddException(e);
                    }
                }
            }

            if (TouchDoubleCallback != null)
            {
                try
                {
                    TouchDoubleCallback();
                }
                catch (Exception e)
                {
                    LogHandler.Instance.AddException(e);
                }
            }
        }
    }

    private List<GameObject> _tmpUIList = new List<GameObject>();
    private void CheckPointEnter()
    {
        _tmpUIList.Clear();
        var curSelectList = GetCurSelectUIList();

        for (int i = 0; i < curSelectList.Count; i++)
        {
            _tmpUIList.Add(curSelectList[i].gameObject);
        }

        foreach (var info in _objectToSelectListenerDict)
        {
            if (info.Key != null)
            {
                if (_tmpUIList.Contains(info.Key) && _tmpUIList.First() == info.Key)
                {
                    //if(info.Key.gameObject.GetComponent<UIHeroSimpleIconElement>()!=null)
                    //Debug.LogError(info.Key.gameObject.GetComponent<UIHeroSimpleIconElement>().Data.Res.Name + "进入");
                    info.Value.OnPointEnter();
                }
                else
                {
                    //if (info.Key.gameObject.GetComponent<UIHeroSimpleIconElement>() != null)
                    //    Debug.LogError(info.Key.gameObject.GetComponent<UIHeroSimpleIconElement>().Data.Res.Name + "退出");
                    info.Value.OnPointExit();
                }
            }

        }
    }

    private void ClearSelectInfo()
    {
        foreach (var info in _objectToSelectListenerDict)
        {
            if (info.Key != null && info.Value != null)
            {
                info.Value.OnPointExit();
            }
        }
    }


    private List<RaycastResult> _uiCacheRaycastResults = new List<RaycastResult>();

    public List<RaycastResult> GetCurSelectUIList()
    {
        if (EventSystem.current == null || !EventSystem.current.enabled)
        {
            return new List<RaycastResult>();
        }

        if (_uiCacheRaycastResults.Count > 0)
        {
            return _uiCacheRaycastResults;

        }

        UnityEngine.Profiling.Profiler.BeginSample("InputManager.GetCurSelectUIList 1");
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = CurrentPressPos;
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("InputManager.GetCurSelectUIList 2");
        EventSystem.current.RaycastAll(pointerEventData, _uiCacheRaycastResults);
        UnityEngine.Profiling.Profiler.EndSample();

        return _uiCacheRaycastResults;
    }


    public List<GameObject> GetCurSelectObject()
    {
        List<GameObject> targetList = new List<GameObject>();

        var uiList = GetCurSelectUIList();

        for (int i = 0; i < uiList.Count; i++)
        {
            if (uiList[i].gameObject != null)
            {
                targetList.Add(uiList[i].gameObject);
            }
        }

        var sceneObjs = GetCurSelectSceneList();

        for (int i = 0; i < sceneObjs.Count; i++)
        {
            if (sceneObjs[i].collider.gameObject != null)
            {
                targetList.Add(sceneObjs[i].collider.gameObject);
            }
        }

        return targetList;
    }



    public List<RaycastHit> GetCurSelectSceneList()
    {
        if (_raycastHitCache.Count > 0)
        {
            return _raycastHitCache;
        }

        if (Camera.main == null)
        {
            return _raycastHitCache;
        }

        var ray = Camera.main.ScreenPointToRay(CurrentPressPos);
        if (!Physics.autoSimulation)
        {
            Physics.Simulate(Time.deltaTime);
        }
        _raycastHitCache = Physics.RaycastAll(ray, Mathf.Infinity).ToList();
        return _raycastHitCache.OrderBy(o => Vector3.Distance(o.point, ray.origin)).ToList();
    }
}

public class InputListener
{
    public bool IsSelect = false;


    public enum State
    {
        None,
        Click,
        LongPress,
        Drag,
        Double,
    }

    public State CurState = State.None;

    public InputCallback Callback;



    public void OnPointerDown()
    {
        CurState = State.None;

        if (Callback.PressCallBack != null)
        {
            Callback.PressCallBack();
        }

    }

    public void OnDoubleClick()
    {
        if (Callback.DoubleClickCallBack != null && CurState == State.None)
        {
            CurState = State.Double;
            Callback.DoubleClickCallBack();
        }
    }

    public void OnPointerUp()
    {
        if (Callback.ClickCallBack != null && CurState == State.None)
        {
            CurState = State.Click;
            Callback.ClickCallBack();
        }



        if (Callback.CancelCallBack != null)
        {
            Callback.CancelCallBack(CurState);
        }
    }



    public void OnDrag()
    {
        if (CurState == State.LongPress)
        {
            if (Callback.CancelCallBack != null)
            {
                Callback.CancelCallBack(CurState);
            }
            CurState = State.None;
        }

        CurState = State.Drag;

        if (Callback.DragCallBack != null)
        {
            Callback.DragCallBack();
        }
    }

    public void OnLongPress()
    {
        if (Callback.LongPressCallBack != null && CurState == State.None)
        {
            CurState = State.LongPress;
            Callback.LongPressCallBack();
        }
    }


    public void OnPointEnter()
    {
        if (!IsSelect)
        {
            IsSelect = true;
            if (Callback.PointEnterCallBack != null)
            {
                Callback.PointEnterCallBack();
            }
        }
    }

    public void OnPointExit()
    {
        if (IsSelect)
        {
            IsSelect = false;
            if (Callback.PointExitCallBack != null)
            {
                Callback.PointExitCallBack();
            }
        }
    }
}