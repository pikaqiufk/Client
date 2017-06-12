#region using

using System;
using System.Threading;
using UnityEngine;
using ThreadPool = Nyahoon.ThreadPool;

#endregion

namespace FastShadowReceiver
{
    public abstract class MeshTreeBase : ScriptableObject
    {
        [SerializeField] [HideInInspector] protected Bounds m_bounds;
        private ManualResetEvent m_event;

        public Bounds bounds
        {
            get { return m_bounds; }
        }

        public void AsyncBuild()
        {
            if (!IsBuildFinished() && m_event != null)
            {
                // already start building
                return;
            }
            if (m_event == null)
            {
                m_event = new ManualResetEvent(false);
            }
            m_event.Reset();
            PrepareForBuild();
            ThreadPool.InitInstance();
            ThreadPool.QueueUserWorkItem(arg => ((MeshTreeBase) arg).BuildStart(), this);
        }

        protected abstract void Build();
        public abstract void BuildFromPrebuiltData();

        private void BuildStart()
        {
            try
            {
                Build();
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
            finally
            {
                m_event.Set();
            }
        }

        public abstract string CheckError(GameObject rootObject);
        public abstract MeshTreeSearch CreateSearch();
        public abstract Type GetSearchType();
        public abstract bool IsBuildFinished();

        public bool IsBuilding()
        {
            return (!IsBuildFinished() && m_event != null);
        }

        public abstract bool IsPrebuilt();
        public abstract bool IsReadyToBuild();
        protected abstract void PrepareForBuild();
        public abstract void Raycast(MeshTreeRaycast raycast);
        public abstract void Search(MeshTreeSearch search);

        public void WaitForBuild()
        {
            if (m_event != null)
            {
                m_event.WaitOne();
                m_event = null;
            }
        }

#if UNITY_EDITOR
        public abstract int GetMemoryUsage();
        public abstract int GetNodeCount();
        public abstract float GetBuildProgress();
#endif
    }
}