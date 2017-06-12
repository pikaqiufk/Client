#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

public class ComplexObjectPool
{
    private static bool Active;
    private static readonly Queue<Action> mActionQueue = new Queue<Action>();

    private static readonly Dictionary<string, ComplexObjectPoolImpl> mDictionary =
        new Dictionary<string, ComplexObjectPoolImpl>();

    private static GameObject mGameObject;

    private static readonly Dictionary<int, ComplexObjectPoolImpl> mObjectIndex =
        new Dictionary<int, ComplexObjectPoolImpl>();

    private static Transform mTransform;
    private static GameObject mUIGameObject;
    private static Transform mUITransform;
#if UNITY_EDITOR
    private static bool Quit;
#endif

    public static Transform Holder
    {
        get
        {
            if (mTransform)
            {
                return mTransform;
            }

            if (mGameObject)
            {
                mTransform = mGameObject.transform;
                return mTransform;
            }

            mGameObject = new GameObject("ComplexObjectPoolHolder");
            mGameObject.AddComponent<Helper>();
            mTransform = mGameObject.transform;
            mTransform.localPosition = new Vector3(0, -10000, 0);
            return mTransform;
        }
    }

    public static Transform UIHolder
    {
        get
        {
            if (mUITransform)
            {
                return mUITransform;
            }

            if (mUIGameObject)
            {
                mUITransform = mUIGameObject.transform;
                return mUITransform;
            }

            mUIGameObject = new GameObject("UIComplexObjectPoolHolder");
            mUIGameObject.AddComponent<Helper>();
            mUITransform = mUIGameObject.transform;
            if (UIManager.Instance.UIRoot == null)
            {
                return null;
            }
            mUITransform.parent = UIManager.Instance.UIRoot.transform;
            mUITransform.localPosition = new Vector3(0, 10000, 0);
            return mUITransform;
        }
    }

    private static IEnumerator CheckRelease()
    {
        // 1分钟以后开始回收资源
        yield return new WaitForSeconds(60.0f);
        while (true)
        {
            var array = mDictionary.Values.ToArray();
            {
                var __array1 = array;
                var __arrayLength1 = __array1.Length;
                for (var __i1 = 0; __i1 < __arrayLength1; ++__i1)
                {
                    var i = __array1[__i1];
                    {
                        i.CheckRelease();
                        if (i.Count == 0)
                        {
                            i.Destroy();
                            mDictionary.Remove(i.Name);
                            OptList.ClearAll();
                            yield return new WaitForSeconds(10.0f);
                        }
                    }
                }
            }
            yield return new WaitForSeconds(30.0f);
        }
        yield break;
    }

    public static void Destroy()
    {
        {
            // foreach(var pool in mDictionary)
            var __enumerator2 = (mDictionary).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var pool = __enumerator2.Current;
                {
                    pool.Value.Destroy();
                }
            }
        }

        mObjectIndex.Clear();
    }

    public static void DestroyUnusedObject()
    {
        foreach (var kv in mDictionary)
        {
            kv.Value.DestroyUnusedObject();
        }
    }

    /// <summary>
    ///     Create a object prefab from pool, the object retain from this pool should not be INSTANTIATED. just use it.
    ///     NOTICE: you should always call Release to give the object back to pool, Destroy should NEVER called by yourself.
    /// </summary>
    /// <param name="res">the path of prefab you want to instinate</param>
    /// <param name="callback">
    ///     when the object is created for you, this function will be called, NOTICE, this function may
    ///     called several frame later.
    /// </param>
    /// <param name="callbackIfReuse">
    ///     when a object is reused from the pool, this function will be called, callback will call
    ///     just after this one.
    /// </param>
    /// <param name="callbackIfNew">
    ///     when a new object is INSTANTIATED from resource, this function will be called, callback
    ///     will call just after this one.
    /// </param>
    public static void NewObject(string res,
                                 Action<GameObject> callback,
                                 Action<GameObject> callbackIfReuse = null,
                                 Action<GameObject> callbackIfNew = null,
                                 bool sync = false,
                                 bool firstPriority = false,
                                 bool active = true,
                                 string changedKey = null)
    {
#if UNITY_EDITOR
        if (Quit)
        {
            return;
        }
#endif
        if (!Active)
        {
            Active = true;
            Holder.GetComponent<Helper>().StartCoroutine(CheckRelease());
        }

        ComplexObjectPoolImpl pool;
        string key;
        if (changedKey == null)
        {
            key = res;
        }
        else
        {
            key = res + changedKey;
        }

        if (!mDictionary.TryGetValue(key, out pool))
        {
            pool = new ComplexObjectPoolImpl(res);
            mDictionary.Add(key, pool);
        }

        pool.NewObject(obj =>
        {
            if (obj)
            {
                mObjectIndex[obj.GetInstanceID()] = pool;
            }
            try
            {
                if (callback != null)
                {
                    callback(obj);
                }
            }
            catch
            {
                // ignored
            }
        }, callbackIfReuse, callbackIfNew, sync, firstPriority, active);
    }

    /// <summary>
    ///     Create a object prefab from pool immediately, the object retain from this pool should not be INSTANTIATED. just use
    ///     it.
    ///     NOTICE: you should always call Release to give the object back to pool, Destroy should NEVER called by yourself.
    /// </summary>
    /// <param name="res">the path of prefab you want to instinate</param>
    /// <param name="callbackIfReuse">
    ///     when a object is reused from the pool, this function will be called, callback will call
    ///     just after this one.
    /// </param>
    /// <param name="callbackIfNew">
    ///     when a new object is INSTANTIATED from resource, this function will be called, callback
    ///     will call just after this one.
    /// </param>
    public static GameObject NewObjectSync(string res,
                                           Action<GameObject> callbackIfReuse = null,
                                           Action<GameObject> callbackIfNew = null)
    {
#if UNITY_EDITOR
        if (Quit)
        {
            return null;
        }
#endif

        if (!Active)
        {
            Active = true;
            Holder.GetComponent<Helper>().StartCoroutine(CheckRelease());
        }

        ComplexObjectPoolImpl pool;
        if (!mDictionary.TryGetValue(res, out pool))
        {
            pool = new ComplexObjectPoolImpl(res);
            mDictionary.Add(res, pool);
        }

        var go = pool.NewObjectSync(callbackIfReuse, callbackIfNew);
        mObjectIndex[go.GetInstanceID()] = pool;
        return go;
    }

    public static void NotifyDestroied(GameObject go)
    {
#if UNITY_EDITOR
        if (Quit)
        {
            return;
        }
#endif
        if (!go)
        {
            return;
        }

        ComplexObjectPoolImpl pool;
        if (mObjectIndex.TryGetValue(go.GetInstanceID(), out pool))
        {
            if (mObjectIndex.Remove(go.GetInstanceID()))
            {
                pool.NotifyDestroied(go);
            }
        }
    }

    /// <summary>
    ///     NOTICE, Relase is not a SYNC operation, the go may not deactived immediate
    ///     so, take care of this.
    /// </summary>
    /// <param name="go">the game object about to put back to pool.</param>
    public static void Release(GameObject go, bool forceDestory = false, bool deactive = true)
    {
#if UNITY_EDITOR
        if (Quit)
        {
            return;
        }
#endif
        if (!go)
        {
            return;
        }

        ComplexObjectPoolImpl pool;
        if (!forceDestory && mObjectIndex.TryGetValue(go.GetInstanceID(), out pool))
        {
            if (mObjectIndex.Remove(go.GetInstanceID()))
            {
                pool.Release(go, deactive);
            }
        }
        else
        {
            Object.Destroy(go);
        }
    }

    public static void SetActive(GameObject go, bool active)
    {
        ComplexObjectPoolImpl.SetTransformActiveRecursively(go.transform, active);
    }

    public class Helper : MonoBehaviour
    {
#if UNITY_EDITOR
        private void OnApplicationQuit()
        {
            Quit = true;
        }
#endif

        private void OnDisable()
        {
#if !UNITY_EDITOR
try
{
#endif

            Active = false;

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
        }

        private void Update()
        {
#if !UNITY_EDITOR
try
{
#endif

            if (mActionQueue.Count > 0)
            {
                var act = mActionQueue.Dequeue();
                try
                {
                    act();
                }
                catch
                {
                    // do nothing...
                }
            }

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
        }
    }

    private class ComplexObjectPoolImpl
    {
        // Flags used to find the 'enabled' property on Unity components that don't expose it.
        private const BindingFlags flags =
            BindingFlags.GetField |
            BindingFlags.SetField |
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Static |
            BindingFlags.Instance;

        public ComplexObjectPoolImpl(string prefab)
        {
            Name = prefab;
        }

        private int mClearFlag;
        private readonly HashSet<GameObject> mDictionary = new HashSet<GameObject>();
        private bool mEnumerating;
        private readonly Stack<GameObject> mFreeGameObjects = new Stack<GameObject>();
        private GameObject mGameObject;
        private int mTotalUsed;
        private Transform mTransform;

        public int Count
        {
            get { return mDictionary.Count + mFreeGameObjects.Count; }
        }

        public string Name { get; private set; }

        public void CheckRelease()
        {
            mEnumerating = true;
            if (mTotalUsed < mFreeGameObjects.Count)
            {
                var least = mFreeGameObjects.Count/2;
                while (mFreeGameObjects.Count > least)
                {
                    var go = mFreeGameObjects.Pop();
                    if (go)
                    {
#if UNITY_EDITOR
                        Object.DestroyImmediate(go);
#else
                        GameObject.Destroy(go);
#endif
                    }
                }
            }

            mTotalUsed = 0;
            mEnumerating = false;
        }

        public void Destroy()
        {
            mEnumerating = true;
            {
                // foreach(var pair in mDictionary)
                var __enumerator3 = (mDictionary).GetEnumerator();
                while (__enumerator3.MoveNext())
                {
                    var pair = __enumerator3.Current;
                    if (pair)
                    {
#if UNITY_EDITOR
                        Object.DestroyImmediate(pair);
#else
						GameObject.Destroy(pair);
#endif
                    }
                }
            }
            {
                // foreach(var pair in mFreeGameObjects)
                var __enumerator4 = (mFreeGameObjects).GetEnumerator();
                while (__enumerator4.MoveNext())
                {
                    var pair = __enumerator4.Current;
                    if (pair)
                    {
#if UNITY_EDITOR
                        Object.DestroyImmediate(pair);
#else
						GameObject.Destroy(pair);
#endif
                    }
                }
            }

            if (mGameObject)
            {
#if UNITY_EDITOR
                Object.DestroyImmediate(mGameObject);
#else
			    GameObject.Destroy(mGameObject);
#endif
            }

            mEnumerating = false;

            mDictionary.Clear();
            mFreeGameObjects.Clear();
            mTotalUsed = 0;
            mClearFlag++;
        }

        public void DestroyUnusedObject()
        {
            while (mFreeGameObjects.Count > 0)
            {
                var go = mFreeGameObjects.Pop();
                if (go)
                {
#if UNITY_EDITOR
                    Object.DestroyImmediate(go);
#else
                    GameObject.Destroy(go);
#endif
                }
            }
        }

        private Transform GetTransform(bool ui = false)
        {
            if (mTransform)
            {
                return mTransform;
            }

            if (mGameObject)
            {
                mTransform = mGameObject.transform;
                return mTransform;
            }

            mGameObject = new GameObject(Name);
            mTransform = mGameObject.transform;
            if (ui)
            {
                mTransform.parent = UIHolder;
                if (mTransform.parent == null)
                {
                    return null;
                }
            }
            else
            {
                mTransform.parent = Holder;
            }
            mTransform.localPosition = Vector3.zero;
            return mTransform;
        }

        public void NewObject(Action<GameObject> callback,
                              Action<GameObject> reuseCallback,
                              Action<GameObject> newCallback,
                              bool sync,
                              bool firstPriority,
                              bool active)
        {
            mTotalUsed++;
            if (mFreeGameObjects.Count > 0)
            {
                var go = mFreeGameObjects.Pop();

                while (go == null && mFreeGameObjects.Count > 0)
                {
                    Logger.Warn("{0} in pool has been destroyed.", Name);
                    go = mFreeGameObjects.Pop();
                }

                if (go == null)
                {
                    var clearFlag = mClearFlag;
                    ResourceManager.PrepareResource<GameObject>(Name, res =>
                    {
                        if (clearFlag != mClearFlag)
                        {
                            return;
                        }
                        if (res == null)
                        {
                            Logger.Error("Load resource {0} from pool failed.", Name);
                            try
                            {
                                callback(null);
                            }
                            catch
                            {
                                // ignored
                            }
                            return;
                        }

                        var goRes = Object.Instantiate(res);
                        go = goRes as GameObject;
                        if (go == null)
                        {
                            Logger.Error("NewObject PrepareResource failed. Prefab = {0}, type = {1}", Name,
                                goRes.GetType());
                            return;
                        }

                        mDictionary.Add(go);

                        try
                        {
                            if (newCallback != null)
                            {
                                newCallback(go);
                            }

                            if (callback != null)
                            {
                                callback(go);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("Create object from pool failed." + ex.ToString());
                        }
                    }, true, true, sync, firstPriority);

                    return;
                }

                var trans = go.transform;
                trans.parent = null;
                trans.localPosition = Vector3.zero;
                trans.localRotation = Quaternion.identity;
                trans.localScale = Vector3.one;
                //go.SetActive(active);

                if (active)
                {
                    SetTransformActiveRecursively(trans, true);
                }

                mDictionary.Add(go);
                try
                {
                    if (reuseCallback != null)
                    {
                        reuseCallback(go);
                    }

                    if (callback != null)
                    {
                        callback(go);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Reuse object from pool failed." + ex);
                }
            }
            else
            {
                var clearFlag = mClearFlag;
                ResourceManager.PrepareResource<GameObject>(Name, res =>
                {
                    if (clearFlag != mClearFlag)
                    {
                        return;
                    }
                    if (res == null)
                    {
                        Logger.Error("Load resource {0} from pool failed.", Name);
                        try
                        {
                            callback(null);
                        }
                        catch
                        {
                            // ignored
                        }
                        return;
                    }

                    var goRes = Object.Instantiate(res);
                    var go = goRes as GameObject;
                    if (go == null)
                    {
                        Logger.Error("NewObject PrepareResource failed. Prefab = {0}, type = {1}", Name, goRes.GetType());
                        return;
                    }

                    mDictionary.Add(go);

                    try
                    {
                        if (newCallback != null)
                        {
                            newCallback(go);
                        }

                        if (callback != null)
                        {
                            callback(go);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Create object from pool failed." + ex.ToString());
                    }
                }, true, true, sync, firstPriority);
            }
        }

        public GameObject NewObjectSync(Action<GameObject> callbackIfReuse, Action<GameObject> callbackIfNew)
        {
            mTotalUsed++;
            if (mFreeGameObjects.Count > 0)
            {
                var go = mFreeGameObjects.Pop();

                while (go == null && mFreeGameObjects.Count > 0)
                {
                    Logger.Info("{0} in pool has been destroyed in sync.", Name);
                    go = mFreeGameObjects.Pop();
                }

                if (go == null)
                {
                    var res = ResourceManager.PrepareResourceSync<GameObject>(Name);
                    if (res == null)
                    {
                        Logger.Error("NewObjectSync PrepareResourceSync failed. Prefab = {0}", Name);
                        return null;
                    }

                    go = Object.Instantiate(res) as GameObject;
                    mDictionary.Add(go);
                    try
                    {
                        if (callbackIfNew != null)
                        {
                            callbackIfNew(go);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Create object from pool failed." + ex);
                    }

                    return go;
                }

                var trans = go.transform;
                trans.parent = null;
                trans.localPosition = Vector3.zero;
                trans.localRotation = Quaternion.identity;
                trans.localScale = Vector3.one;
                //go.SetActive(true);
                mDictionary.Add(go);

                try
                {
                    if (callbackIfReuse != null)
                    {
                        callbackIfReuse(go);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Create object from pool failed." + ex);
                }
                //go.SetActive(true);
                return go;
            }
            else
            {
                var res = ResourceManager.PrepareResourceSync<GameObject>(Name);
                var go = Object.Instantiate(res) as GameObject;
                mDictionary.Add(go);
                try
                {
                    if (callbackIfNew != null)
                    {
                        callbackIfNew(go);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Create object from pool failed." + ex);
                }
                return go;
            }
        }

        internal void NotifyDestroied(GameObject go)
        {
            if (go == null)
            {
                return;
            }
            if (mEnumerating)
            {
                return;
            }
            mDictionary.Remove(go);
        }

        public void Release(GameObject go, bool deactive = true)
        {
            if (go == null)
            {
                return;
            }

            if (mEnumerating)
            {
                return;
            }

            if (!mDictionary.Remove(go))
            {
                return;
            }

            var parent = GetTransform(go.layer == LayerMask.NameToLayer("UI"));
            if (parent == null)
            {
                if (go)
                {
#if UNITY_EDITOR
                    Object.DestroyImmediate(go);
#else
                    GameObject.Destroy(go);
#endif
                }
                return;
            }

            mFreeGameObjects.Push(go);
            var goTransform = go.transform;
            goTransform.parent = parent;
            goTransform.localPosition = Vector3.zero;

            Action act = () =>
            {
                if (go)
                {
                    // check if the resource is reused.
                    if (!mDictionary.Contains(go))
                    {
                        if(deactive)
                            SetTransformActiveRecursively(goTransform, false);
                    }
                }
            };

            mActionQueue.Enqueue(act);
        }

        /// <summary>
        ///     Set all the components in this xform to act except Animation to avoid RebuildInternalState.
        ///     if the xform do not contains a Animation component, it will set to act directly
        ///     else all the component will set to act and keep the xform active.
        /// </summary>
        /// <param name="transform">xform to be set</param>
        /// <param name="act"></param>
        /// <returns>is the xform still active</returns>
        private static bool SetTransformActive(Transform transform, bool act)
        {
            var animations = transform.GetComponents<Animation>();
            if (animations.Length > 0)
            {
                Animation anim;
                for (var i = 0; i < animations.Length; i++)
                {
                    anim = animations[i];
                    if (!act)
                    {
                        anim.Stop();
                    }
                    else
                    {
                        if (anim.clip != null)
                        {
                            var state = anim[anim.clip.name];
                            if (state != null)
                            {
                                state.time = 0;
                                anim.Sample();
                            }

                            anim.Play(PlayMode.StopAll);
                        }
                    }
                }

                var components = transform.GetComponents<Component>();
                Component component;
                Type type;
                PropertyInfo property;
                for (var j = 0; j < components.Length; j++)
                {
                    component = components[j];
                    if (null == component)
                    {
//增加安全判断
                        continue;
                    }
                    type = component.GetType();
                    if (type != typeof (Animation))
                    {
                        property = type.GetProperty("enabled", flags);
                        if (property != null)
                        {
                            property.SetValue(component, act, null);
                        }
                    }
                }

                return true;
            }
            transform.gameObject.active = act;
            return false;
        }

        public static void SetTransformActiveRecursively(Transform transform, bool active)
        {
            if (SetTransformActive(transform, active))
            {
                if (transform.childCount > 0)
                {
                    for (var i = 0; i < transform.childCount; i++)
                    {
                        SetTransformActiveRecursively(transform.GetChild(i), active);
                    }
                }
            }
        }
    }
}