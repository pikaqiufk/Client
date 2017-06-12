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
    ///     Manages a "turntable" component which will rotate the owner in the Y access at
    ///     a constant speed while on. This will start itself and stop itself when enabled
    ///     or disabled as well as directly through the "on" bool property.
    /// </summary>
    [AddComponentMenu("Path-o-logical/UnityConstraints/Constraint - Turntable")]
    public class TurntableConstraint : ConstraintFrameworkBaseClass
    {
        /// <summary>
        ///     The speed at which the turn table will turn
        /// </summary>
        public bool randomStart = false;

        /// <summary>
        ///     The speed at which the turn table will turn
        /// </summary>
        public float speed = 1;

        /// <summary>
        ///     Runs each frame while the constraint is active
        /// </summary>
        protected override void OnConstraintUpdate()
        {
            // Note: Do not run base.OnConstraintUpdate. It is not implimented

#if UNITY_EDITOR
            // It isn't helpful to have this run in the editor.
            //   This if statement won't compile with the game
            if (Application.isPlaying)
            {
#endif
                // Add a rotation == this.speed per frame
                xform.Rotate(0, speed, 0);
#if UNITY_EDITOR
            }
#endif
        }

        protected override void OnEnable()
        {
#if !UNITY_EDITOR
try
{
#endif

            base.OnEnable();

#if UNITY_EDITOR
            if (Application.isPlaying)
            {
#endif
                // It isn't helpful to have this run in the editor.
                //   This if statement won't compile with the game
                if (Application.isPlaying && randomStart)
                {
                    xform.Rotate(0, UnityEngine.Random.value * 360, 0);
                }
#if UNITY_EDITOR
            }
#endif

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
        }
    }
}