#region using

using UnityEngine;

#endregion

namespace PathologicalGames
{
    /// <description>
    ///     Constrain this transform to a target's scale, rotation and/or translation.
    /// </description>
    [AddComponentMenu("Path-o-logical/UnityConstraints/Constraint - Transform - Smooth")]
    public class SmoothTransformConstraint : TransformConstraint
    {
        private Vector3 curDampVelocity = Vector3.zero;

        public UnityConstraints.INTERP_OPTIONS interpolation =
            UnityConstraints.INTERP_OPTIONS.Spherical;

        public INTERP_OPTIONS_POS position_interpolation = INTERP_OPTIONS_POS.Linear;
        public float positionMaxSpeed = 0.1f;
        public float positionSpeed = 0.1f;
        public float rotationSpeed = 1;
        public float scaleSpeed = 0.1f;

        public enum INTERP_OPTIONS_POS
        {
            Linear,
            Damp,
            DampLimited
        }

        /// <summary>
        ///     Runs when the noTarget mode is set to ReturnToDefault
        /// </summary>
        protected override void NoTargetDefault()
        {
            if (constrainScale)
            {
                xform.localScale = Vector3.one;
            }

            OutputRotationTowards(Quaternion.identity);
            OutputPositionTowards(target.position);
        }

        /// <summary>
        ///     Runs each frame while the constraint is active
        /// </summary>
        protected override void OnConstraintUpdate()
        {
            if (constrainScale)
            {
                SetWorldScale(target);
            }

            OutputRotationTowards(target.rotation);
            OutputPositionTowards(target.position);
        }

        /// <summary>
        ///     Runs when the constraint is active or when the noTarget mode is set to
        ///     ReturnToDefault
        /// </summary>
        private void OutputPositionTowards(Vector3 destPos)
        {
            // Faster exit if there is nothing to do.
            if (!constrainPosition)
            {
                return;
            }

            switch (position_interpolation)
            {
                case INTERP_OPTIONS_POS.Linear:
                    pos = Vector3.Lerp(xform.position, destPos, positionSpeed);
                    break;

                case INTERP_OPTIONS_POS.Damp:
                    pos = Vector3.SmoothDamp
                        (
                            xform.position,
                            destPos,
                            ref curDampVelocity,
                            positionSpeed
                        );
                    break;

                case INTERP_OPTIONS_POS.DampLimited:
                    pos = Vector3.SmoothDamp
                        (
                            xform.position,
                            destPos,
                            ref curDampVelocity,
                            positionSpeed,
                            positionMaxSpeed
                        );
                    break;
            }

            // Output only if wanted - faster to invert and set back to current.
            if (!outputPosX)
            {
                pos.x = xform.position.x;
            }
            if (!outputPosY)
            {
                pos.y = xform.position.y;
            }
            if (!outputPosZ)
            {
                pos.z = xform.position.z;
            }

            xform.position = pos;
        }

        /// <summary>
        ///     Runs when the constraint is active or when the noTarget mode is set to
        ///     ReturnToDefault
        /// </summary>
        private void OutputRotationTowards(Quaternion destRot)
        {
            // Faster exit if nothing to do.
            if (!constrainRotation)
            {
                return;
            }

            UnityConstraints.InterpolateRotationTo
                (
                    xform,
                    destRot,
                    interpolation,
                    rotationSpeed
                );

            UnityConstraints.MaskOutputRotations(xform, output);
        }

        /// <summary>
        ///     Sets this transform's scale to equal the target in world space.
        /// </summary>
        /// <param name="sourceXform"></param>
        public override void SetWorldScale(Transform sourceXform)
        {
            // Set the scale now that both Transforms are in the same space
            xform.localScale = Vector3.Lerp
                (
                    xform.localScale,
                    GetTargetLocalScale(sourceXform),
                    scaleSpeed
                );
        }
    }
}