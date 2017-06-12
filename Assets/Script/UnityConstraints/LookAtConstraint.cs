#region using

using UnityEngine;

#endregion

namespace PathologicalGames
{
    /// <summary>
    ///     The base class for all constraints that use a target and mode
    /// </summary>
    [AddComponentMenu("Path-o-logical/UnityConstraints/Constraint - Look At")]
    public class LookAtConstraint : LookAtBaseClass
    {
        /// <summary>
        ///     An optional target just for the upAxis. The upAxis may not point directly
        ///     at this. See the online docs for more info
        /// </summary>
        public Transform upTarget;

        // Get the lookVector
        protected virtual Vector3 lookVect
        {
            get { return target.position - xform.position; }
        }

        // Get the upvector. Factors in any options.
        protected Vector3 upVect
        {
            get
            {
                Vector3 upVect;
                if (upTarget == null)
                {
                    upVect = Vector3.up;
                }
                else
                {
                    upVect = upTarget.position - xform.position;
                }

                return upVect;
            }
        }

        /// <summary>
        ///     Runs each frame while the constraint is active
        /// </summary>
        protected override void OnConstraintUpdate()
        {
            // Note: Do not run base.OnConstraintUpdate. It is not implimented

            var lookRot = Quaternion.LookRotation(lookVect, upVect);
            xform.rotation = GetUserLookRotation(lookRot);
        }
    }
}