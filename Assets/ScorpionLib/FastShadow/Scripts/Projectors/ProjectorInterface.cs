#region using

using System;
using UnityEngine;

#endregion

namespace FastShadowReceiver
{
    public struct ClipPlanes
    {
        public Plane[] clipPlanes;
        public float[] maxDistance;
        public int scissorPlaneCount;
        public bool twoSideClipping;

        public int clipPlaneCount
        {
            get { return clipPlanes.Length; }
        }

        public void SetClipPlaneNum(int clipPlaneNum, int scissorePlaneNum, bool twoSideClip)
        {
            if (clipPlanes == null || clipPlanes.Length != clipPlaneNum)
            {
                clipPlanes = new Plane[clipPlaneNum];
                maxDistance = new float[clipPlaneNum];
            }
            scissorPlaneCount = scissorePlaneNum;
            twoSideClipping = twoSideClip;
        }
    }

    public interface IProjector
    {
        Vector3 direction { get; }
        float farClipPlane { get; }
        // returns world -> uv projection matrix. the range of uv(x/w, y/w) is [0, 1]. z is linear depth, i.e. not perspective, and near will be mapped to 0, far will be mapped to 1.
        bool isOrthographic { get; }
        float nearClipPlane { get; }
        Vector3 position { get; }
        Quaternion rotation { get; }
        Matrix4x4 uvProjectionMatrix { get; }
        void GetClipPlanes(ref ClipPlanes clipPlanes, Transform clipPlaneTransform);
        void GetClipPlanes(ref ClipPlanes clipPlanes, Transform clipPlaneTransform, ITransformPredictor predictor);
        void GetPlaneIntersection(Vector3[] vertices, Plane plane);
        void InvokeUpdateTransform();

        event Action updateTransform;
            // this event will be triggered before Shadow Receivers use the transform of the projector.
    }
}