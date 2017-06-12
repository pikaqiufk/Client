using System;
#region using

using UnityEngine;

/// <Licensing>
/// � 2011 (Copyright) Path-o-logical Games, LLC
/// If purchased from the Unity Asset Store, the following license is superseded 
/// by the Asset Store license.
/// Licensed under the Unity Asset Package Product License (the "License");
/// You may not use this file except in compliance with the License.
/// You may obtain a copy of the License at: http://licensing.path-o-logical.com
/// </Licensing>

#endregion

namespace PathologicalGames
{
    /// <summary>
    ///     Billboarding is when an object is made to face a camera, so that no matter
    ///     where it is on screen, it looks flat (not simply a "look-at" constraint, it
    ///     keeps the transform looking parallel to the target - usually a camera). This
    ///     is ideal for sprites, flat planes with textures that always face the camera.
    /// </summary>
    [AddComponentMenu("Path-o-logical/UnityConstraints/Constraint - Billboard")]
    public class BillboardConstraint : LookAtBaseClass
    {
        /// <summary>
        ///     If false, the billboard will only rotate along the upAxis.
        /// </summary>
        public bool vertical = true;

        protected override void Awake()
        {
#if !UNITY_EDITOR
try
{
#endif

            base.Awake();

            var cameras = FindObjectsOfType(typeof (Camera)) as Camera[];
            {
                var __array1 = cameras;
                var __arrayLength1 = __array1.Length;
                for (var __i1 = 0; __i1 < __arrayLength1; ++__i1)
                {
                    var cam = __array1[__i1];
                    {
                        if ((cam.cullingMask & 1 << gameObject.layer) > 0)
                        {
                            target = cam.transform;
                            break;
                        }
                    }
                }
            }

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
        }

        /// <summary>
        ///     Runs each frame while the constraint is active
        /// </summary>
        protected override void OnConstraintUpdate()
        {
            // Note: Do not run base.OnConstraintUpdate. It is not implimented

            // This is based on the Unity wiki script which keeps this look-at
            //   vector parrallel with the camera, rather than right at the center
            var lookPos = xform.position + target.rotation*Vector3.back;
            var upVect = Vector3.up;

            // If the billboarding will happen vertically as well, then the upvector needs
            //   to be derrived from the target's up vector to remain parallel
            // If not billboarding vertically, then keep the defaul upvector and set the
            //   lookPos to be level with this object so it doesn't rotate in x or z
            if (vertical)
            {
                upVect = target.rotation*Vector3.up;
            }
            else
            {
                lookPos.y = xform.position.y;
            }

            // Get the final direction to look for processing with user input axis settings
            var lookVect = lookPos - xform.position;
            var lookRot = Quaternion.LookRotation(lookVect, upVect);
            xform.rotation = GetUserLookRotation(lookRot);
        }
    }
}