#region using

using System;
using UnityEngine;

#endregion

namespace FastShadowReceiver
{
    public abstract class ProjectorBase : MonoBehaviour, IProjector
    {
        public static void DrawFrustum(IProjector projector, Color color)
        {
            var mat = projector.uvProjectionMatrix;
            var dir = projector.direction;
            var near = projector.nearClipPlane;
            var far = projector.farClipPlane;
            Vector4 z = dir;
            z.w = -near - Vector3.Dot(projector.position, dir);
            z /= (far - near);
            Vector4 p = projector.position + far*dir;
            p.w = 1.0f;
            z *= (mat*p).w;
            mat.SetRow(2, z);
            mat = mat.inverse;
            var p0 = mat*new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            var p1 = mat*new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
            var p2 = mat*new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
            var p3 = mat*new Vector4(1.0f, 1.0f, 0.0f, 1.0f);
            var p4 = mat*new Vector4(0.0f, 0.0f, 1.0f, 1.0f);
            var p5 = mat*new Vector4(1.0f, 0.0f, 1.0f, 1.0f);
            var p6 = mat*new Vector4(0.0f, 1.0f, 1.0f, 1.0f);
            var p7 = mat*new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            Vector3 v0 = p0/p0.w;
            Vector3 v1 = p1/p1.w;
            Vector3 v2 = p2/p2.w;
            Vector3 v3 = p3/p3.w;
            Vector3 v4 = p4/p4.w;
            Vector3 v5 = p5/p5.w;
            Vector3 v6 = p6/p6.w;
            Vector3 v7 = p7/p7.w;
            Gizmos.color = color;
            Gizmos.DrawLine(v0, v1);
            Gizmos.DrawLine(v0, v2);
            Gizmos.DrawLine(v2, v3);
            Gizmos.DrawLine(v1, v3);
            Gizmos.DrawLine(v0, v4);
            Gizmos.DrawLine(v1, v5);
            Gizmos.DrawLine(v2, v6);
            Gizmos.DrawLine(v3, v7);
            Gizmos.DrawLine(v4, v5);
            Gizmos.DrawLine(v4, v6);
            Gizmos.DrawLine(v6, v7);
            Gizmos.DrawLine(v5, v7);
        }

        protected virtual void OnDrawGizmosSelected()
        {
            DrawFrustum(this, new Color(1.0f, 1.0f, 1.0f, 111.0f/255.0f));
        }

        public event Action updateTransform;
            // this event will be triggered before Shadow Receivers use the transform of the projector.

        public abstract void GetPlaneIntersection(Vector3[] vertices, Plane plane);
        public abstract void GetClipPlanes(ref ClipPlanes clipPlanes, Transform clipPlaneTransform);

        public abstract void GetClipPlanes(ref ClipPlanes clipPlanes,
                                           Transform clipPlaneTransform,
                                           ITransformPredictor predictor);

        public void InvokeUpdateTransform()
        {
            if (updateTransform != null)
            {
                updateTransform();
            }
        }

        public abstract bool isOrthographic { get; }
        public abstract Vector3 position { get; }
        public abstract Vector3 direction { get; }
        public abstract Quaternion rotation { get; }
        public abstract Matrix4x4 uvProjectionMatrix { get; }
        // returns world -> uv projection matrix. the range of uv(x/w, y/w) is [0, 1]. z is linear depth, i.e. not perspective, and near will be mapped to 0, far will be mapped to 1.
        public abstract float nearClipPlane { get; }
        public abstract float farClipPlane { get; }
    }
}