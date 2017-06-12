#region using

using System;
using System.Threading;
using UnityEngine;
using ThreadPool = Nyahoon.ThreadPool;

#endregion

namespace FastShadowReceiver
{
    public class MeshTreeRaycast
    {
        private bool m_cullBackFace;
        private float m_distance;
        private ManualResetEvent m_event;
        private MeshTreeBase m_tree;
        public Vector3 direction { get; private set; }
        public float hitDistance { get; private set; }

        /// <summary>
        ///     Gets the normal vector of the surface where the raycast hit.
        /// </summary>
        /// <value>The hit normal.</value>
        public Vector3 hitNormal { get; private set; }

        /// <summary>
        ///     Gets the position where the raycast hit.
        /// </summary>
        /// <value>The hit position.</value>
        public Vector3 hitPosition
        {
            get { return origin + hitDistance*direction; }
        }

        /// <summary>
        ///     Gets a value indicating whether the raycast hit the mesh object.
        /// </summary>
        /// <value><c>true</c> if raycast hit; otherwise, <c>false</c>.</value>
        public bool isHit
        {
            get { return hitDistance < m_distance; }
        }

        public Vector3 origin { get; private set; }

        /// <summary>
        ///     Cast a ray against a mesh tree object in a background thread.
        ///     You can wait for the raycast to be done by calling Wait() function.
        ///     Also, you can check if the raycast is done or not by calling IsDone() function.
        ///     Please be noted that "isHit", "hitPosition" and "hitNormal" properties are invalid until the raycast is done
        /// </summary>
        /// <param name="tree">A MeshTree object.</param>
        /// <param name="origin">The origin point of the ray in the local space of the mesh object.</param>
        /// <param name="direction">The direction of the ray in the local space of the mesh object.</param>
        /// <param name="distance">The length of the ray.</param>
        /// <param name="cullBackFace">If set to <c>true</c> cull back face.</param>
        public void AsyncRaycast(MeshTreeBase tree, Vector3 origin, Vector3 direction, float distance, bool cullBackFace)
        {
            if (m_event == null)
            {
                m_event = new ManualResetEvent(false);
            }
            m_event.Reset();
            m_tree = tree;
            distance *= direction.magnitude;
            direction.Normalize();
            this.origin = origin;
            this.direction = direction;
            m_distance = distance;
            hitDistance = distance;
            m_cullBackFace = cullBackFace;
            ThreadPool.QueueUserWorkItem(s_raycastCallback, this);
        }

        public bool BoundsHitTest(Vector3 center, Vector3 extents, TemporaryParam param, out float distance)
        {
            center -= origin;
            extents = Vector3.Scale(param.m_dirsign, extents);
            var min = Vector3.Scale(param.m_invdir, center - extents);
            var max = Vector3.Scale(param.m_invdir, center + extents);
            var m = Mathf.Max(Mathf.Max(min.x, min.y), min.z);
            var M = Mathf.Min(Mathf.Min(max.x, max.y), max.z);
            distance = m;
            return m <= M && 0.0f <= M && m < hitDistance;
        }

        public TemporaryParam CreateTemporaryParam()
        {
            TemporaryParam param;
            param.m_dirsign.x = direction.x < 0.0f ? -1.0f : 1.0f;
            param.m_dirsign.y = direction.y < 0.0f ? -1.0f : 1.0f;
            param.m_dirsign.z = direction.z < 0.0f ? -1.0f : 1.0f;
            param.m_invdir.x = Mathf.Epsilon < Mathf.Abs(direction.x)
                ? 1.0f/direction.x
                : direction.x < 0.0f ? -1.0f/Mathf.Epsilon : 1.0f/Mathf.Epsilon;
            param.m_invdir.y = Mathf.Epsilon < Mathf.Abs(direction.y)
                ? 1.0f/direction.y
                : direction.y < 0.0f ? -1.0f/Mathf.Epsilon : 1.0f/Mathf.Epsilon;
            param.m_invdir.z = Mathf.Epsilon < Mathf.Abs(direction.z)
                ? 1.0f/direction.z
                : direction.z < 0.0f ? -1.0f/Mathf.Epsilon : 1.0f/Mathf.Epsilon;
            return param;
        }

        public bool IsDone()
        {
            if (m_event != null)
            {
                return m_event.WaitOne(0);
            }
            return false;
        }

        /// <summary>
        ///     Cast a ray against a mesh tree object.
        ///     The return value indicates whether the raycast hit or not.
        ///     After this function is called, you can access "isHit", "hitPosition" and "hitNormal" properties.
        /// </summary>
        /// <param name="tree">A MeshTree object.</param>
        /// <param name="origin">The origin point of the ray in the local space of the mesh object.</param>
        /// <param name="direction">The direction of the ray in the local space of the mesh object.</param>
        /// <param name="distance">The length of the ray.</param>
        /// <param name="cullBackFace">If set to <c>true</c> cull back face.</param>
        public bool Raycast(MeshTreeBase tree, Vector3 origin, Vector3 direction, float distance, bool cullBackFace)
        {
            this.origin = origin;
            distance *= direction.magnitude;
            direction.Normalize();
            this.direction = direction;
            m_distance = distance;
            hitDistance = distance;
            m_cullBackFace = cullBackFace;
            tree.Raycast(this);
            return isHit;
        }

        private void Raycast()
        {
            try
            {
                m_tree.Raycast(this);
                m_tree = null;
            }
            catch (Exception e)
            {
                if (Debug.isDebugBuild || Application.isEditor)
                {
                    Debug.LogException(e);
                }
                throw e;
            }
            finally
            {
                m_event.Set();
            }
        }

        public bool TriangleHitTest(Vector3 v0, Vector3 v1, Vector3 v2)
        {
            var p = origin - v0;
            var a = v1 - v0;
            var b = v2 - v0;
            var axb = Vector3.Cross(a, b);
            var dot = Vector3.Dot(direction, axb);
            if (-Mathf.Epsilon < dot && (dot < Mathf.Epsilon || m_cullBackFace))
            {
                return false;
            }
            var rdot = 1.0f/dot;
            var distance = -Vector3.Dot(p, axb)*rdot;
            if (distance < 0.0f || hitDistance <= distance)
            {
                return false;
            }
            var u = Vector3.Dot(p, Vector3.Cross(b, direction))*rdot;
            var v = Vector3.Dot(p, Vector3.Cross(direction, a))*rdot;
            if (u < 0.0f || v < 0.0f || 1.0f < u + v)
            {
                return false;
            }
            hitDistance = distance;
            hitNormal = axb.normalized;
            return true;
        }

        public void Wait()
        {
            if (m_event != null)
            {
                m_event.WaitOne();
            }
        }

        public struct TemporaryParam
        {
            public Vector3 m_dirsign;
            public Vector3 m_invdir;
        }

        private static readonly WaitCallback s_raycastCallback = (arg => ((MeshTreeRaycast) arg).Raycast());
    }
}