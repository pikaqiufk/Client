using System;
using UnityEngine;

namespace SignalChain
{
    public class SignalChain
    {
        private Transform _root;

        public SignalChain(Transform root)
        {
            _root = root;
        }

        public void Send<T>(T notification)
        {
            Send(typeof(T), notification);
        }

        public void Send<T>()
        {
            Send(typeof(T));
        }

        void Send(Type notification)
        {
            Send(notification, null);
        }

        public void Send(Type notificationType, object notification)
        {
            MonoBehaviour[] behaviours = _root.GetComponents<MonoBehaviour>();
            {
                var __array1 = behaviours;
                var __arrayLength1 = __array1.Length;
                for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
                {
                    var behaviour = (MonoBehaviour)__array1[__i1];
                    if (behaviour is IChainListener)
                        (behaviour as IChainListener).Listen(notification != null ? notification : notificationType);
                }
            }
        }

        public void Broadcast<T>()
        {
            Broadcast(typeof(T));
        }

        public void Broadcast<T>(T notification)
        {
            Broadcast(typeof(T), notification, false);
        }

        void Broadcast(Type notification)
        {
            Broadcast(notification, null, false);
        }

        public void Broadcast<T>(T notification, bool notifyDisabled)
        {
            Broadcast(typeof(T), notification, notifyDisabled);
        }

        public void Broadcast(Type notification, bool notifyDisabled)
        {
            Broadcast(notification, null, notifyDisabled);
        }

        private void Broadcast(Type notificationType, object notification, bool notifyDisabled)
        {
            {
                var __array2 = _root.GetComponentsInChildren<MonoBehaviour>(notifyDisabled);
                var __arrayLength2 = __array2.Length;
                for (int __i2 = 0; __i2 < __arrayLength2; ++__i2)
                {
                    var behaviour = (MonoBehaviour)__array2[__i2];
                    if (behaviour is IChainListener)
                        (behaviour as IChainListener).Listen(notification != null ? notification : notificationType);
                }
            }
        }

        public void DeepBroadcast<T>()
        {
            DeepBroadcast(typeof(T));
        }

        public void DeepBroadcast<T>(T notification)
        {
            DeepBroadcast(typeof(T), notification);
        }

        void DeepBroadcast(Type notification)
        {
            DeepBroadcast(notification, null);
        }

        private void DeepBroadcast(Type notificationType, object notification)
        {
            Broadcast(notificationType, notification, true);
        }
    }
}

