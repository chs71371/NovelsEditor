using System;
using System.Collections.Generic;
 
using System.Threading;

 
    public interface IVersionObject
    {
        uint PoolVersion { get; set; }
    }
    public interface IPoolObject : IVersionObject
    {
        void OnAlloc();
        void OnRecycle();
    }

    public class PoolObject : IPoolObject
    {
        public uint PoolVersion { get; set; }
        public virtual void OnAlloc()
        {
        }

        public virtual void OnRecycle()
        {
        }
    }

    public interface IObjectPool
    {
        void Clear();
        object AllocObject();
        void RecycleObject(object obj);
    }

    public static class ObjectPoolManager
    {
        private static readonly List<IObjectPool> _poolList = new List<IObjectPool>(); 
        internal static void AddPool(IObjectPool pool)
        {
            _poolList.Add(pool);
        }

        public static void Clear()
        {
            for (int i = 0; i < _poolList.Count; i++)
            {
                _poolList[i].Clear();
            }
        }
    }

    public class ObjectPool<T> : IObjectPool where T : class ,new()
    {
        private const int InitQueueCapicity = 16;
        private static readonly bool IsPoolObj = typeof(IPoolObject).IsAssignableFrom(typeof (T));
        private static ObjectPool<T> _defaultPool;      
		public static bool ThreadSafe { get; set; }

        public static ObjectPool<T> Default
        {
            get
            {
                if (_defaultPool == null)
                {
                    _defaultPool = new ObjectPool<T>();
                }
                return _defaultPool;
            }
        }
		private T[] _freeObjQueue = new T[16];
		private int _freeObjCnt = 0;

        public ObjectPool()
        {
            ObjectPoolManager.AddPool(this);
        }

        public void Clear()
        {
            Array.Clear(_freeObjQueue, 0, _freeObjCnt);
            _freeObjCnt = 0;
        }

        public object AllocObject()
        {
            return Alloc();
        }

        public void RecycleObject(object obj)
        {
            Recycle(obj as T);
        }
              
        public T Alloc()
        {
            T obj = null;
			if (ThreadSafe)
			{
				if (Monitor.TryEnter(this))
				{
                    try
                    {
                        if (_freeObjCnt > 0)
                        {
                            obj = _freeObjQueue[--_freeObjCnt];
                        }
                    }
                    finally
                    {
                        Monitor.Exit(this);
                    }
				}
			}
			else if (_freeObjCnt > 0)
            {
				obj = _freeObjQueue[--_freeObjCnt];
            }

            if (obj == null)
			{
				obj = new T();
			}

            if (IsPoolObj)
            {
                IPoolObject poolObj = obj as IPoolObject;
                poolObj.OnAlloc();
            }
            return obj;
        }

        public void Recycle(T obj)
        {
            if (obj == null)
            {
                return;
            }
            if (IsPoolObj)
            {
                IPoolObject poolObj = obj as IPoolObject;
                poolObj.OnRecycle();
                ++poolObj.PoolVersion;
            }
			if (ThreadSafe)
			{     
				if (Monitor.TryEnter(this))
				{               
					try
					{
						if (_freeObjCnt >= _freeObjQueue.Length)
						{
							Array.Resize(ref _freeObjQueue, Math.Max(_freeObjCnt * 2, 64));
						}
						_freeObjQueue[_freeObjCnt++] = obj;
					}
                    finally
                    {
                        Monitor.Exit(this);
                    }
				}
			}
			else
			{            
                if (_freeObjCnt >= _freeObjQueue.Length)
                {
                    Array.Resize(ref _freeObjQueue, Math.Max(_freeObjCnt * 2, 64));
                }
                _freeObjQueue[_freeObjCnt++] = obj;
			}
 
        }

		public void RecycleList(IList<T> objList)
		{
			for (int i = 0; i < objList.Count; ++i)
			{
				Recycle(objList[i]);
			}
			objList.Clear();
		}

        public  void RecycleSet(HashSet<T> objSet)
        {
			var itor = objSet.GetEnumerator();
			while(itor.MoveNext())
			{
				Recycle(itor.Current);
			}
			objSet.Clear();
        }

        public static T DefaultAlloc()
        {
            return Default.Alloc();
        }

        public static void DefaultRecycle(T obj)
        {
            Default.Recycle(obj);
        }

        public static void DefaultRecycleList(IList<T> objList)
        {
            Default.RecycleList(objList);
		}
		public static void DefaultRecycleSet(HashSet<T> objList)
        {
            Default.RecycleSet(objList);
        }
    }

    public static class PoolObjectExt
    {
        public static void RecycleToObjectPool<T>(this T obj, ObjectPool<T> pool = null) where T : class, new()
        {
            if (pool == null)
            {
                ObjectPool<T>.DefaultRecycle(obj);
            }
            else
            {
                pool.RecycleObject(obj);
            }
        }
    }
 