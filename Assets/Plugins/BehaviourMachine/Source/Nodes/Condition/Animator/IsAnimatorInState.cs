//----------------------------------------------
//            Behaviour Machine
// Copyright © 2014 Anderson Campos Cardoso
//----------------------------------------------

using UnityEngine;
using System.Collections;

namespace BehaviourMachine {
    /// <summary>
    /// Returns Success if the Animatior component is in the supplied state; Failure otherwise.
    /// </summary>
    [NodeInfo(  category = "Condition/Animator/",
                icon = "Animator",
                description = "Returns Success if the Animatior component is in the supplied state; Failure otherwise",
                url = "http://docs.unity3d.com/ScriptReference/Animator.GetCurrentAnimatorStateInfo.html")]
    public class IsAnimatorInState : ConditionNode {

    	/// <summary>
        /// The game object that has an Animation component.
        /// </summary>
        [VariableInfo(requiredField = false, nullLabel = "Use Self", tooltip = "The game object that has an Animation component")]
        public GameObjectVar gameObject;

        /// <summary>
        /// The layer's index.
        /// </summary>
        [VariableInfo(tooltip = "The layer's index")]
        public IntVar layerIndex;

        /// <summary>
        /// The state's name.
        /// </summary>
        [VariableInfo(tooltip = "The state's name")]
        public StringVar stateName;

        /// <summary>
        /// If True then it will returns Failure if the Animator is in transition.
        /// </summary>
        [Tooltip("If True then it will returns Failure if the Animator is in transition")]
        public bool considerTransitions;

        [System.NonSerialized]
        Animator m_Animator = null;

        public override void Reset () {
            gameObject = new ConcreteGameObjectVar(this.self);
            layerIndex = 0;
            stateName = "State Name";
            considerTransitions = true;
        }

        public override Status Update () {
            // Get the animation
            if (m_Animator == null || m_Animator.gameObject != gameObject.Value)
                m_Animator = gameObject.Value != null ? gameObject.Value.GetComponent<Animator>() : null;

            // Validate members
            if (m_Animator == null)
                return Status.Error;

            if ((!considerTransitions || !m_Animator.IsInTransition(0)) && m_Animator.GetCurrentAnimatorStateInfo(layerIndex.Value).IsName(stateName.Value)) {
                 // Send event?
                if (onSuccess.id != 0)
                    owner.root.SendEvent(onSuccess.id);
                return Status.Success;
            }
            else
                return Status.Failure;
        }
    }
}