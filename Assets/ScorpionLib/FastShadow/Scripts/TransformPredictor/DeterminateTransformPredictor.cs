#region using

using UnityEngine;

#endregion

namespace FastShadowReceiver
{
    public class DeterminateTransformPredictor : MonoBehaviour, ITransformPredictor
    {
        public Vector3 averageEulerAngle;
        public Vector3 averageMove;
        public Vector3 eulerAngleRange;
        public Vector3 moveRange;

        public Bounds PredictNextFramePositionChanges()
        {
            var bounds = new Bounds();
            bounds.center = averageMove;
            bounds.extents = moveRange;
            return bounds;
        }

        public Bounds PredictNextFrameEulerAngleChanges()
        {
            var bounds = new Bounds();
            bounds.center = averageEulerAngle;
            bounds.extents = eulerAngleRange;
            return bounds;
        }
    }
}