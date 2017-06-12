//----------------------------------------------
//            Behaviour Machine
// Copyright © 2014 Anderson Campos Cardoso
//----------------------------------------------

using UnityEngine;
using System.Collections;

namespace BehaviourMachine {

    /// <summary> 
    /// A FunctionNode waits for a callback to tick their children.
    /// </summary>
    public abstract class FunctionNode : CompositeNode {

        #region Members
        /// <summary>
        /// The function is enabled? Set this value to false to disable the function.
        /// </summary>
        [HideInInspector]
        public bool enabled = true;

        [System.NonSerialized]
        protected bool m_Registered = false;
        #endregion Members

        
        #region ActionNode Callbacks
        public override void Reset () {
            enabled = true;
        }

        public override Status Update () {
            var childStatus = Status.Error;

            // Tick children
            for (int i = 0; i < children.Length; i++) {
                children[i].OnTick();
                childStatus = children[i].status;
            }

            return childStatus;
        }

        public override void OnValidate () {
            if (Application.isPlaying) {
                if (this.enabled && owner.enabled && !m_Registered)
                    this.OnEnable();
                else if (!this.enabled && owner.enabled && m_Registered)
                    this.OnDisable();
            }
        }
        #endregion ActionNode Callbacks
    }
}