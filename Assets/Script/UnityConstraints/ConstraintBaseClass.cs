using System;
#region using

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
    [AddComponentMenu("")] // Hides from Unity4 Inspector menu
    public class ConstraintBaseClass : ConstraintFrameworkBaseClass
    {
        protected Transform _internalTarget;

        /// <summary>
        ///     The current mode of the constraint.
        ///     Setting the mode will start or stop the constraint coroutine, so if
        ///     'alignOnce' is chosen, the constraint will align once then go to sleep.
        /// </summary>
        // Public backing field for the inspector (The underscore will not show up)
        public UnityConstraints.MODE_OPTIONS _mode = UnityConstraints.MODE_OPTIONS.Constrain;

        /// <summary>
        ///     Determines the behavior if no target is available
        /// </summary>
        // Public backing field for the inspector (The underscore will not show up)
        public UnityConstraints.NO_TARGET_OPTIONS _noTargetMode =
            UnityConstraints.NO_TARGET_OPTIONS.DoNothing;

        /// <summary>
        ///     The object to constrain to
        /// </summary>
        public Transform _target; // For the inspector

        protected Vector3 pos; // Reused for temp calculations
        protected Quaternion rot; // Reused for temp calculations
        protected Vector3 scl; // Reused for temp calculations
        // Used if the noTargetMode is set to SetByScript
        protected virtual Transform internalTarget
        {
            get
            {
                if (_internalTarget != null)
                {
                    return _internalTarget;
                }

                // Create a GameObject to use as the target for the constraint
                // Setting the name is really only for debugging and unexpected errors
                var go = new GameObject(name + "_InternalConstraintTarget");
                go.hideFlags = HideFlags.HideInHierarchy; // Hide from the hierarchy
                _internalTarget = go.transform;


                // Make a sibling so this object and the reference exist in the same space
                //   and reset
                _internalTarget.position = xform.position;
                _internalTarget.rotation = xform.rotation;
                _internalTarget.localScale = xform.localScale;

                return _internalTarget;
            }
        }

        public UnityConstraints.MODE_OPTIONS mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                InitConstraint();
            }
        }

        public UnityConstraints.NO_TARGET_OPTIONS noTargetMode
        {
            get { return _noTargetMode; }
            set { _noTargetMode = value; }
        }

        /// <summary>
        ///     Set the position this constraint will use if the noTargetMode is set to
        ///     SetByScript. Usage is determined by script.
        /// </summary>
        public Vector3 position
        {
            get { return internalTarget.position; }
            set { internalTarget.position = value; }
        }

        /// <summary>
        ///     Set the rotaion this constraint will use if the noTargetMode is set to
        ///     SetByScript. Usage is determined by script.
        /// </summary>
        public Quaternion rotation
        {
            get { return internalTarget.rotation; }
            set { internalTarget.rotation = value; }
        }

        /// <summary>
        ///     Set the localScale this constraint will use if the noTargetMode is set to
        ///     SetByScript. Usage is determined by script.
        /// </summary>
        public Vector3 scale
        {
            get { return internalTarget.localScale; }
            set { internalTarget.localScale = value; }
        }

        public Transform target
        {
            get
            {
                if (noTargetMode == UnityConstraints.NO_TARGET_OPTIONS.SetByScript)
                {
                    return internalTarget;
                }

                return _target;
            }

            set { _target = value; }
        }

        /// <summary>
        ///     Runs as long as the component is active.
        /// </summary>
        /// <returns></returns>
        protected override sealed IEnumerator Constrain()
        {
            // Start on the next frame incase some init still needs to occur.
            //yield return null;

            // While in Constrain mode handle this.target even if null.
            while (mode == UnityConstraints.MODE_OPTIONS.Constrain)
            {
                // If null, handle then continue to the next frame to test again.
                if (target == null)
                {
                    // While the target is null, handle using the noTargetMode options
                    switch (noTargetMode)
                    {
                        case UnityConstraints.NO_TARGET_OPTIONS.Error:
                            var msg = string.Format("No target provided. \n{0} on {1}",
                                GetType().Name,
                                xform.name);

                            Debug.LogError(msg); // Spams like Unity
                            yield return null;
                            continue; // All done this frame, try again next.

                        case UnityConstraints.NO_TARGET_OPTIONS.DoNothing:
                            yield return null;
                            continue; // All done this frame, try again next.

                        case UnityConstraints.NO_TARGET_OPTIONS.ReturnToDefault:
                            NoTargetDefault();
                            yield return null;
                            continue; // All done this frame, try again next.

                            // Could omit. Kept for completeness
                            //case UnityConstraints.NO_TARGET_OPTIONS.SetByScript:
                            //    // Handled as normal. Pass-through.
                            //    break;
                    }
                }

                // Attempt the constraint...

                // Just incase the target is lost during the frame prior to this point.
                if (target == null)
                {
                    continue;
                }

                // CONSTRAIN...
                OnConstraintUpdate();

                yield return null;
            }
        }

        /// <summary>
        ///     Activate the constraint again if this object was disabled then enabled.
        ///     Also runs immediatly after Awake()
        /// </summary>
        protected override sealed void InitConstraint()
        {
#if !UNITY_EDITOR
try
{
#endif

            switch (mode)
            {
                case UnityConstraints.MODE_OPTIONS.Align:
                    OnOnce();
                    break;

                case UnityConstraints.MODE_OPTIONS.Constrain:
                    break;
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
        ///     Runs when the noTarget mode is set to ReturnToDefault.
        ///     Derrived constraints should just override and not run this
        /// </summary>
        protected virtual void NoTargetDefault()
        {
            // Use in child classes to do something when there is no target
        }

        // Clean-up
        private void OnDestroy()
        {
#if !UNITY_EDITOR
try
{
#endif

            // Destroy the internalTarget if one was created.

            if (_internalTarget != null)
            {
                if (!Application.isPlaying)
                {
                    DestroyImmediate(_internalTarget.gameObject);
                }
                else
                {
                    Destroy(_internalTarget.gameObject);
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
        ///     Runs when the "once" option is chosen
        /// </summary>
        private void OnOnce()
        {
            OnConstraintUpdate();
        }

#if UNITY_EDITOR
        /// <summary>
        ///     The base class has the ExecuteInEditMode attribute, so this is Update() called
        ///     anytime something is changed in the editor. See:
        ///     http://docs.unity3d.com/Documentation/ScriptReference/ExecuteInEditMode.html
        ///     This function exists in the UNITY_EDITOR preprocessor directive so it
        ///     won't be compiled for the final game. It includes an Application.isPlaying
        ///     check to ensure it is bypassed when in the Unity Editor
        /// </summary>
        protected override void Update()
        {
#if !UNITY_EDITOR
try
{
#endif

            if (target == null)
            {
                return;
            }

            base.Update();

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
}