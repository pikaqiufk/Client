#region using

using UnityEngine;

#endregion

namespace PathologicalGames
{
    /// <summary>
    ///     Billboarding is when an object is made to face a camera, so that no matter
    ///     where it is on screen, it looks flat (not simply a "look-at" constraint, it
    ///     keeps the transform looking parallel to the target - usually a camera). This
    ///     is ideal for sprites, flat planes with textures that always face the camera.
    /// </summary>
    [AddComponentMenu("Path-o-logical/UnityConstraints/Constraint - Look At - Smooth")]
    public class SmoothLookAtConstraint : LookAtConstraint
    {
        private Vector3 angles;
        private Quaternion curRot;

        /// <summary>
        ///     The rotation interpolation solution to use.
        /// </summary>
        public UnityConstraints.INTERP_OPTIONS interpolation =
            UnityConstraints.INTERP_OPTIONS.Spherical;

        // Reused every frame (probably overkill, but can't hurt...)
        private Quaternion lookRot;
        private Vector3 lookVectCache;

        /// <summary>
        ///     What axes and space to output the result to.
        /// </summary>
        public UnityConstraints.OUTPUT_ROT_OPTIONS output =
            UnityConstraints.OUTPUT_ROT_OPTIONS.WorldAll;

        /// <summary>
        ///     How fast the constrant can rotate. The result depends on the interpolation chosen.
        /// </summary>
        public float speed = 1;

        private Quaternion usrLookRot;

        /// <summary>
        ///     Runs when the noTarget mode is set to ReturnToDefault
        /// </summary>
        protected override void NoTargetDefault()
        {
            UnityConstraints.InterpolateLocalRotationTo
                (
                    xform,
                    Quaternion.identity,
                    interpolation,
                    speed
                );
        }

        /// <summary>
        ///     Runs each frame while the constraint is active
        /// </summary>
        protected override void OnConstraintUpdate()
        {
            // Note: Do not run base.OnConstraintUpdate. It is not implimented

            lookVectCache = Vector3.zero;
            lookVectCache = lookVect; // Property, so cache result
            if (lookVectCache == Vector3.zero)
            {
                return;
            }

            lookRot = Quaternion.LookRotation(lookVectCache, upVect);
            usrLookRot = GetUserLookRotation(lookRot);


            OutputTowards(usrLookRot);
        }

        /// <summary>
        ///     Runs when the constraint is active or when the noTarget mode is set to
        ///     ReturnToDefault
        /// </summary>
        private void OutputTowards(Quaternion destRot)
        {
            UnityConstraints.InterpolateRotationTo
                (
                    xform,
                    destRot,
                    interpolation,
                    speed
                );

            UnityConstraints.MaskOutputRotations(xform, output);
        }
    }
}