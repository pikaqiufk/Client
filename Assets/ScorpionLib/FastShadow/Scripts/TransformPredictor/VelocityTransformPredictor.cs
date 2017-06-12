#region using

using UnityEngine;

#endregion

namespace FastShadowReceiver
{
    public class VelocityTransformPredictor : MonoBehaviour, ITransformPredictor
    {
        public Vector3 averageEulerAngleVelocity;
        public Vector3 averageVelocity;
        public Vector3 eulerAngleVelocityRange;
        public Vector3 velocityRange;

        public Bounds PredictNextFramePositionChanges()
        {
            var bounds = new Bounds();
            bounds.center = Time.deltaTime*averageVelocity;
            bounds.extents = Time.deltaTime*velocityRange;
            return bounds;
        }

        public Bounds PredictNextFrameEulerAngleChanges()
        {
            var bounds = new Bounds();
            bounds.center = Time.deltaTime*averageEulerAngleVelocity;
            bounds.extents = Time.deltaTime*eulerAngleVelocityRange;
            return bounds;
        }
    }
}