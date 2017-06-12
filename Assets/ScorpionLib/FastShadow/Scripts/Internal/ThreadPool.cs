﻿#region using

using System.Threading;
using UnityEngine;

#endregion

namespace Nyahoon
{
    /// <summary>
    ///     Thread pool.
    ///     This class itself is not thread safe. Only a single thread can call QueueUserWorkItem safely.
    /// </summary>
    public class ThreadPool
    {
        static ThreadPool()
        {
            Instance = null;
        }

        private ThreadPool(int queueSize, int threadNum)
        {
#if UNITY_WEBPLAYER
			threadNum = 1;
#else
            if (threadNum == 0)
            {
                threadNum = SystemInfo.processorCount;
            }
#endif
            m_threadPool = new Thread[threadNum];
            m_taskQueue = new TaskInfo[queueSize];
            m_nPutPointer = 0;
            m_nGetPointer = 0;
            m_numTasks = 0;
            m_putNotification = new AutoResetEvent(false);
            m_getNotification = new AutoResetEvent(false);
#if !UNITY_WEBPLAYER
            if (1 < threadNum)
            {
                m_semaphore = new Semaphore(0, queueSize);
                for (var i = 0; i < threadNum; ++i)
                {
                    m_threadPool[i] = new Thread(ThreadFunc);
                    m_threadPool[i].Start();
                }
            }
            else
#endif
            {
                m_threadPool[0] = new Thread(SingleThreadFunc);
                m_threadPool[0].Start();
            }
        }

        private readonly AutoResetEvent m_getNotification;
        private int m_nGetPointer;
        private int m_nPutPointer;
        private int m_numTasks;
        private readonly AutoResetEvent m_putNotification;
#if !UNITY_WEBPLAYER
        // according to this page (https://docs.unity3d.com/401/Documentation/ScriptReference/MonoCompatibility.html),
        // Semaphore is not available on web player.
        private readonly Semaphore m_semaphore;
#endif
        private readonly TaskInfo[] m_taskQueue;
        private readonly Thread[] m_threadPool;
        public static ThreadPool Instance { get; private set; }

        private void EnqueueTask(WaitCallback callback, object state)
        {
            while (m_numTasks == m_taskQueue.Length)
            {
                m_getNotification.WaitOne();
            }
            m_taskQueue[m_nPutPointer].callback = callback;
            m_taskQueue[m_nPutPointer].args = state;
            ++m_nPutPointer;
            if (m_nPutPointer == m_taskQueue.Length)
            {
                m_nPutPointer = 0;
            }
#if !UNITY_WEBPLAYER
            if (m_threadPool.Length == 1)
            {
#endif
                if (Interlocked.Increment(ref m_numTasks) == 1)
                {
                    m_putNotification.Set();
                }
#if !UNITY_WEBPLAYER
            }
            else
            {
                Interlocked.Increment(ref m_numTasks);
                m_semaphore.Release();
            }
#endif
        }

        public static void InitInstance()
        {
            if (Instance == null)
            {
                InitInstance(128, 0);
            }
        }

        public static bool InitInstance(int queueSize, int threadNum)
        {
            if (Instance != null)
            {
                if (Debug.isDebugBuild || Application.isEditor)
                {
                    Debug.LogWarning("TreadPool instance is already created.");
                }
                return false;
            }
            Instance = new ThreadPool(queueSize, threadNum);
            return true;
        }

        public static void QueueUserWorkItem(WaitCallback callback, object state)
        {
            Instance.EnqueueTask(callback, state);
        }

        private void SingleThreadFunc()
        {
            for (;;)
            {
                while (m_numTasks == 0)
                {
                    m_putNotification.WaitOne();
                }
                var task = m_taskQueue[m_nGetPointer++];
                if (m_nGetPointer == m_taskQueue.Length)
                {
                    m_nGetPointer = 0;
                }
                if (Interlocked.Decrement(ref m_numTasks) == m_taskQueue.Length - 1)
                {
                    m_getNotification.Set();
                }
                task.callback(task.args);
            }
        }

#if !UNITY_WEBPLAYER
        private void ThreadFunc()
        {
            for (;;)
            {
                m_semaphore.WaitOne();
                int nCurrentPointer, nNextPointer;
                do
                {
                    nCurrentPointer = m_nGetPointer;
                    nNextPointer = nCurrentPointer + 1;
                    if (nNextPointer == m_taskQueue.Length)
                    {
                        nNextPointer = 0;
                    }
                } while (Interlocked.CompareExchange(ref m_nGetPointer, nNextPointer, nCurrentPointer) !=
                         nCurrentPointer);
                var task = m_taskQueue[nCurrentPointer];
                if (Interlocked.Decrement(ref m_numTasks) == m_taskQueue.Length - 1)
                {
                    m_getNotification.Set();
                }
                task.callback(task.args);
            }
        }
#endif

        private struct TaskInfo
        {
            public object args;
            public WaitCallback callback;
        }
    }
}