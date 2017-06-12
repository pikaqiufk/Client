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
    /// <summary>
    ///     Makes this Transform, being rendered from an orthographic (2D) camera, appear to
    ///     follow a target being rendered by another camera, in another space.
    ///     This is perfect for use with most sprite-based GUI plugins to make a GUI element
    ///     follow a 3D object. For example, a life bar can be setup to stay above the head of
    ///     enemies.
    /// </summary>
    [AddComponentMenu("Path-o-logical/UnityConstraints/Constraint- World To 2D Camera")]
    public class WorldTo2DCameraConstraint : TransformConstraint
    {
        /// <summary>
        ///     Determines what happens when the target moves offscreen
        /// </summary>
        public OFFSCREEN_MODE offScreenMode = OFFSCREEN_MODE.Constrain;

        /// <summary>
        ///     Offsets the behavior determined by offscreenMode.
        ///     e.g. 0.1 to 0.9 would stop 0.1 from the edge of the screen on both sides
        /// </summary>
        public Vector2 offscreenThreasholdH = new Vector2(0, 1);

        /// <summary>
        ///     Offsets the behavior determined by offscreenMode.
        ///     e.g. 0.1 to 0.9 would stop 0.1 from the edge of the screen on both sides
        /// </summary>
        public Vector2 offscreenThreasholdW = new Vector2(0, 1);

        /// <summary>
        ///     Offset the screen position. The z value is depth from the camera.
        /// </summary>
        public Vector3 offset;

        /// <summary>
        ///     Determines when to apply the offset. In world-space or view-port space
        /// </summary>
        public OFFSET_MODE offsetMode = OFFSET_MODE.WorldSpace;

        /// <summary>
        ///     The camera the output object will be constrained to.
        /// </summary>
        public Camera orthoCamera;

        /// <summary>
        ///     The camera used to render the target
        /// </summary>
        public Camera targetCamera;

        /// <summary>
        ///     If false, the billboard will only rotate along the upAxis.
        /// </summary>
        public bool vertical = true;

        /// <summary>
        ///     Determines what happens when the target moves offscreen
        ///     Constraint - Continues to track the target the same as it does on-screen
        ///     Limit - Stops at the edge of the frame but continues to track the target
        ///     DoNothing - Stops calculating the constraint
        /// </summary>
        public enum OFFSCREEN_MODE
        {
            Constrain,
            Limit,
            DoNothing
        };

        /// <summary>
        ///     Determines when to apply the offset.
        ///     WorldSpace - Applies the X and Y offset relative to the target object, in world-
        ///     space, before converting to the ortheCamera's viewport-space. The
        ///     offset values are used for world units
        ///     ViewportSpace - Applies the X and Y offset in the ortheCamera's viewport-space.
        ///     The offset values are in viewport space, which is normalized 0-1.
        /// </summary>
        public enum OFFSET_MODE
        {
            WorldSpace,
            ViewportSpace
        };

        protected override void Awake()
        {
#if !UNITY_EDITOR
try
{
#endif

            base.Awake();

            orthoCamera = UIManager.Instance.UICamera;

            if (target != null)
            {
                targetCamera = GameLogic.Instance.MainCamera;
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
            // Ignore position work in the base class
            var constrainPositionSetting = constrainPosition;
            constrainPosition = false;

            base.OnConstraintUpdate(); // Handles rotation and scale

            // Set back to the user setting
            constrainPosition = constrainPositionSetting;

            if (!constrainPosition)
            {
                return;
            }

            //wang androidbug
            if (!target)
            {
                return;
            }

            // Note: pos is reused and never destroyed
            pos = target.position;

            if (offsetMode == OFFSET_MODE.WorldSpace)
            {
                pos.x += offset.x;
                pos.y += offset.y;
            }

            pos = targetCamera.WorldToViewportPoint(pos);

            if (offsetMode == OFFSET_MODE.ViewportSpace)
            {
                pos.x += offset.x;
                pos.y += offset.y;
            }

            switch (offScreenMode)
            {
                case OFFSCREEN_MODE.DoNothing:
                    // The pos is normalized so if it is between 0 and 1 in x and y, it is on screen.
                    var isOnScreen = (
                        pos.z > 0f &&
                        pos.x > offscreenThreasholdW.x &&
                        pos.x < offscreenThreasholdW.y &&
                        pos.y > offscreenThreasholdH.x &&
                        pos.y < offscreenThreasholdH.y
                        );

                    if (!isOnScreen)
                    {
                        return; // Quit
                    }
                    break; // It is on screen, so continue...

                case OFFSCREEN_MODE.Constrain:
                    break; // Nothing modified. Continue...

                case OFFSCREEN_MODE.Limit:
                    // pos is normalized so if it is between 0 and 1 in x and y, it is on screen.
                    // Clamp will do nothing unless the target is offscreen
                    pos.x = Mathf.Clamp
                        (
                            pos.x,
                            offscreenThreasholdW.x,
                            offscreenThreasholdW.y
                        );

                    pos.y = Mathf.Clamp
                        (
                            pos.y,
                            offscreenThreasholdH.x,
                            offscreenThreasholdH.y
                        );
                    break;
            }

            pos = orthoCamera.ViewportToWorldPoint(pos);
            //this.pos.z = this.offset.z;

            // Output only if wanted, otherwise use the transform's current value
            if (!outputPosX)
            {
                pos.x = position.x;
            }
            if (!outputPosY)
            {
                pos.y = position.y;
            }
            //if (!this.outputPosZ) this.pos.z = this.position.z;

            xform.position = new Vector3(pos.x, pos.y, 0);
            xform.localPosition = new Vector3(xform.localPosition.x, xform.localPosition.y, 0);
        }
    }
}