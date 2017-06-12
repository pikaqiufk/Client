#region using

using UnityEngine;

#endregion

namespace PathologicalGames
{
    /// <summary>
    ///     This is the base class for all look-at based constraints including billboarding.
    /// </summary>
    [AddComponentMenu("")] // Hides from Unity4 Inspector menu
    public class LookAtBaseClass : ConstraintBaseClass
    {
        /// <summary>
        ///     The axis used to point at the target.
        ///     This is public for user input only, should not be used by derrived classes
        /// </summary>
        public Vector3 pointAxis = -Vector3.back;

        /// <summary>
        ///     The axis to point up in world space.
        ///     This is public for user input only, should not be used by derrived classes
        /// </summary>
        public Vector3 upAxis = Vector3.up;

        protected override Transform internalTarget
        {
            get
            {
                // Note: This backing field is in the base class
                if (_internalTarget != null)
                {
                    return _internalTarget;
                }

                var target = base.internalTarget; // Will init internalTarget GO

                // Set the internal target to 1 unit away in the direction of the pointAxis
                // this.target will be the internalTarget due to SetByScript
                target.position = (xform.rotation*pointAxis) + xform.position;

                return _internalTarget;
            }
        }

        /// <summary>
        ///     Processes the user's 'pointAxis' and 'upAxis' to look at the target.
        /// </summary>
        /// <param name="lookVect">The direction in which to look</param>
        /// <param name="upVect">
        ///     The secondary axis will point along a plane in this direction
        /// </param>
        protected Quaternion GetUserLookRotation(Quaternion lookRot)
        {
            // Get the look at rotation and a rotation representing the user input
            var userAxisRot = Quaternion.LookRotation(pointAxis, upAxis);

            // offset the look-at by the user input to get the final result
            return lookRot*Quaternion.Inverse(userAxisRot);
        }

        /// <summary>
        ///     Runs when the noTarget mode is set to ReturnToDefault
        /// </summary>
        protected override void NoTargetDefault()
        {
            xform.rotation = Quaternion.identity;
        }
    }
}