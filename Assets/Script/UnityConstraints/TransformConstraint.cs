using System;
#region using

using UnityEngine;

/// <Licensing>
/// ?2011 (Copyright) Path-o-logical Games, LLC
/// If purchased from the Unity Asset Store, the following license is superseded 
/// by the Asset Store license.
/// Licensed under the Unity Asset Package Product License (the "License");
/// You may not use this file except in compliance with the License.
/// You may obtain a copy of the License at: http://licensing.path-o-logical.com
/// </Licensing>

#endregion

namespace PathologicalGames
{
    /// <description>
    ///     Constrain this transform to a target's scale, rotation and/or translation.
    /// </description>
    [AddComponentMenu("Path-o-logical/UnityConstraints/Constraint - Transform (Postion, Rotation, Scale)")]
    public class TransformConstraint : ConstraintBaseClass
    {
        /// <summary>
        ///     A transform used for all Perimeter Gizmos to calculate the final
        ///     position and rotation of the drawn gizmo
        /// </summary>
        internal static Transform scaleCalculator;

        /// <summary>
        ///     Option to match the target's position
        /// </summary>
        public bool constrainPosition = true;

        /// <summary>
        ///     Option to match the target's rotation
        /// </summary>
        public bool constrainRotation = false;

        /// <summary>
        ///     Option to match the target's scale. This is a little more expensive performance
        ///     wise and not needed very often so the default is false.
        /// </summary>
        public bool constrainScale = false;

        /// <summary>
        ///     Used to alter the way the rotations are set
        /// </summary>
        public UnityConstraints.OUTPUT_ROT_OPTIONS output =
            UnityConstraints.OUTPUT_ROT_OPTIONS.WorldAll;

        /// <summary>
        ///     If false, the rotation in this axis will not be affected
        /// </summary>
        public bool outputPosX = true;

        /// <summary>
        ///     If false, the rotation in this axis will not be affected
        /// </summary>
        public bool outputPosY = true;

        /// <summary>
        ///     If false, the rotation in this axis will not be affected
        /// </summary>
        public bool outputPosZ = true;

        // Cache...
        internal Transform parXform;

        /// <summary>
        ///     position offset of this constraint.
        /// </summary>
        public Vector3 positionOffset;

        /// <summary>
        ///     Cache as much as possible before starting the co-routine
        /// </summary>
        protected override void Awake()
        {
#if !UNITY_EDITOR
try
{
#endif

            base.Awake();

            parXform = xform.parent;

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
        }

        /// <summary>
        ///     Sets this transform's scale to equal the target in world space.
        /// </summary>
        /// <param name="sourceXform"></param>
        internal Vector3 GetTargetLocalScale(Transform sourceXform)
        {
            // Singleton: Create a hidden empty gameobject to use for scale calculations
            //   All instances will this. A single empty game object is so small it will
            //   never be an issue. Ever.
            if (scaleCalculator == null)
            {
                var name = "TransformConstraint_spaceCalculator";

                // When the game starts and stops the reference can be lost but the transform
                //   still exists. Re-establish the reference if it is found.
                var found = GameObject.Find(name);
                if (found != null)
                {
                    scaleCalculator = found.transform;
                }
                else
                {
                    var scaleCalc = new GameObject(name);
                    scaleCalc.gameObject.hideFlags = HideFlags.HideAndDontSave;
                    scaleCalculator = scaleCalc.transform;
                }
            }

            // Store the source's lossyScale, which is Unity's estimate of "world scale", 
            //   to this seperate Transform which doesn't have a parent (it is in world space) 
            var refXform = scaleCalculator;

            // Cast the reference transform in to the space of the source Xform using
            //   Parenting, then cast it back to set.
            refXform.parent = sourceXform;
            refXform.localRotation = Quaternion.identity; // Stablizes this solution
            refXform.localScale = Vector3.one;

            // Parent the reference transform to this object so they are in the same
            //   space, now we have a local scale to use.
            refXform.parent = parXform;

            return refXform.localScale;
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

            if (constrainRotation)
            {
                xform.rotation = Quaternion.identity;
            }

            if (constrainPosition)
            {
                xform.position = Vector3.zero;
            }
        }

        /// <summary>
        ///     Runs each frame while the constraint is active
        /// </summary>
        protected override void OnConstraintUpdate()
        {
            // Note: Do not run base.OnConstraintUpdate. It is not implimented
            if (!target)
            {
                return;
            }

            if (constrainScale)
            {
                SetWorldScale(target);
            }

            if (constrainRotation)
            {
                xform.rotation = target.rotation;
                UnityConstraints.MaskOutputRotations(xform, output);
            }

            if (constrainPosition)
            {
                pos = xform.position + positionOffset;

                // Output only if wanted
                if (outputPosX)
                {
                    pos.x = target.position.x + positionOffset.x;
                }
                if (outputPosY)
                {
                    pos.y = target.position.y + positionOffset.y;
                }
                if (outputPosZ)
                {
                    pos.z = target.position.z + positionOffset.z;
                }

                xform.position = pos;
            }
        }

        /// <summary>
        ///     Sets this transform's scale to equal the target in world space.
        /// </summary>
        /// <param name="sourceXform"></param>
        public virtual void SetWorldScale(Transform sourceXform)
        {
            // Set the scale now that both Transforms are in the same space
            xform.localScale = GetTargetLocalScale(sourceXform);
        }
    }
}