#region using

using System;
using System.Collections;
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
    ///     The base class for all constraints
    /// </description>
    [ExecuteInEditMode] // WARNING: Runs components in the Editor!!
    [AddComponentMenu("")] // Hides from Unity4 Inspector menu
    public class ConstraintFrameworkBaseClass : MonoBehaviour
    {
        [HideInInspector] public bool mEnabled;
        // Cache...
        [HideInInspector] public Transform xform; // Made public to share the cache when storing references

        /// <summary>
        ///     Cache as much as possible before starting the co-routine
        /// </summary>
        protected virtual void Awake()
        {
#if !UNITY_EDITOR
try
{
#endif

            xform = transform;

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
        }

        /// <summary>
        ///     Runs as long as the component is active.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator Constrain()
        {
            while (true)
            {
                OnConstraintUpdate();
                yield return null;
            }
        }

        /// <summary>
        ///     Activate the constraint again if this object was disabled then enabled.
        ///     Also runs immediatly after Awake()
        /// </summary>
        protected virtual void InitConstraint()
        {
#if !UNITY_EDITOR
try
{
#endif


#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
        }

        private void LateUpdate()
        {
#if !UNITY_EDITOR
try
{
#endif

            if (mEnabled)
            {
                OnConstraintUpdate();
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
        ///     Impliment on child classes
        ///     Runs each frame while the constraint is active
        /// </summary>
        protected virtual void OnConstraintUpdate()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Activate the constraint again if this object was disabled then enabled.
        ///     Also runs immediatly after Awake()
        /// </summary>
        protected virtual void OnDisable()
        {
#if !UNITY_EDITOR
try
{
#endif

            mEnabled = false;

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
        }

        /// <summary>
        ///     Activate the constraint again if this object was disabled then enabled.
        ///     Also runs immediatly after Awake()
        /// </summary>
        protected virtual void OnEnable()
        {
#if !UNITY_EDITOR
try
{
#endif

            InitConstraint();
            mEnabled = true;

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        ///     This class has the ExecuteInEditMode attribute, so this Update() is called
        ///     anytime something is changed in the editor. See:
        ///     http://docs.unity3d.com/Documentation/ScriptReference/ExecuteInEditMode.html
        ///     This function exists in the UNITY_EDITOR preprocessor directive so it
        ///     won't be compiled for the final game. It includes an Application.isPlaying
        ///     check to ensure it is bypassed when in the Unity Editor
        /// </summary>
        protected virtual void Update()
        {
#if !UNITY_EDITOR
try
{
#endif

            if (Application.isPlaying)
            {
                return;
            }

            // The co-routines are started even in Editor mode, but it isn't perfectly 
            //   consistent. They don't always seem to restart when the game is stopped,
            //   for example. So just stop them and run the Update using this Editor-
            //   driven Update()
            StopAllCoroutines();
            OnConstraintUpdate();

#if !UNITY_EDITOR
}
catch (Exception ex)
{
#if !UNITY_EDITOR
try
{
#endif

    Logger.Error(ex.ToString());

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}
#endif
        }
#endif
    }
}