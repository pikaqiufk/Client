#region using

using UnityEngine;

#endregion

namespace FastShadowReceiver
{
    public interface ITransformPredictor
    {
        /// <summary>
        ///     Predicts the next frame euler angle changes.
        /// </summary>
        /// <returns>The bounds of next frame euler angle changes in local coordinates.</returns>
        Bounds PredictNextFrameEulerAngleChanges();

        /// <summary>
        ///     Predicts the next frame position changes.
        /// </summary>
        /// <returns>The bounds of next frame move vector in local coordinates.</returns>
        Bounds PredictNextFramePositionChanges();
    }
}