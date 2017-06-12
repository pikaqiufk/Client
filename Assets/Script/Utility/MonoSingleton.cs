#region using

using System;
using UnityEngine;

#endregion

namespace Assets.Script.Utility
{
    public class MonoSingleton<T> : BaseCallBackMonoBehavior
        where T : BaseCallBackMonoBehavior, new()
    {
        public static T Instance;

        public static void Create(string name, Action callBack = null)
        {
            var go = GameObject.Find(name);
            if (go == null)
            {
                go = new GameObject(name);
                DontDestroyOnLoad(go);
                var t = go.AddComponent<T>();
                Instance = t;
                t.SetCallBack(callBack);
            }
        }
    }

    public class BaseCallBackMonoBehavior : MonoBehaviour
    {
        private Action mOnCallBack;

        public void DoCallBack()
        {
            if (mOnCallBack != null)
            {
                mOnCallBack();
            }
        }

        public void SetCallBack(Action callBack)
        {
            mOnCallBack = callBack;
        }
    }
}