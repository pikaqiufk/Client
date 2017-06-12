/*! \mainpage Scripting Overview
 *
 * \section sec_scripting Introduction
 *
 * The plugin is MonoBehaviour based and take advantage of the Unity's specific features to create a powerful FSM and Behaviour Tree system.
 *
 * \section sec_customStates Custom States
 * 
 * Inherit from the BehaviourMachine.InternalStateBehaviour class to create your custom state. All the three languages are supported: Boo, C# and UnityScript (Scripts in Boo and UnityScript should not be placed in the "Assets / Plugins", "Assets /  Standard Assets"  and "Assets /  Pro Standard Assets"  folders). 
 *
 * The BehaviourMachine.InternalStateBehaviour inherits from the MonoBehaviour and is added as a component in the same game object as the FSM, it means that the Unity timed callbacks (e.g Update, Awake, Start) will be called directly by the engine when the state is enabled. You also get the OnEnable and OnDisable callbacks when the state is enabled/disable respectively.
 *
 * But your custom state will also get the messages related to other components (eg. OnTrigger, OnCollision, OnMouse), even if it’s not enabled. To not get these callbacks you can check the enabled property:
 * 
 * \code
 *  using UnityEngine;
 *  using System.Collections;
 *  using BehaviourMachine;
 * 
 *  public class CustomState : InternalStateBehaviour { 
 *     void OnTriggerEnter (Collider other) {
 *         if (!enabled)
 *             return;
 *      
 *          // Do stuff
 *      }
 *  }
 * \endcode
 * 
 * Another way to not get these callbacks is to rename these methods and register them in the BehaviourMachine.Blackboard events:
 *
 * \code
 *  using UnityEngine;
 *  using System.Collections;
 *  using BehaviourMachine;
 *  
 *  public class CustomState : InternalStateBehaviour {
 *
 *     // Called when the state is enabled
 *     void OnEnable () {
 *         blackboard.onTriggerEnter += StateOnTriggerEnter;
 *     }
 *     
 *     // Called when the state is disabled
 *     void OnDisable () {
 *         blackboard.onTriggerEnter -= StateOnTriggerEnter;
 *     }
 *     
 *     void StateOnTriggerEnter (Collider other) {
 *         // Do stuff
 *     }
 *  }
 * \endcode
 * 
 *
 * \section sec_changingState Changing State
 *
 * Call the method SendEvent passing the event id/name to execute a state transition:
 *
 * \code
 *  using UnityEngine;
 *  using System.Collections;
 *  using BehaviourMachine;
 *  
 *  public class CustomState : InternalStateBehaviour {
 *
 *     void Update () {
 *          // Call SendEvent in the top most parent (tree or FSM)
 *          root.SendEvent("Event Name");
 *
 *          // Call SendEvent in the parent passing the event id
 *          parent.SendEvent(12345);
 *
 *          // Call SendEvent in this state passing the event name
 *          this.SendEvent("Event Name");
 *      }
 *  }
 * \endcode
 *
 * The SendEvent method returns 'true' if the supplied event was used to perform a transition; 'false' otherwise. 
 *
 *
 * \section sec_accessingVariables Accessing Variables
 *
 * The BehaviourMachine.Blackboard has public methods to get the stored variables: 
 *
 * \code
 *  using UnityEngine;
 *  using System.Collections;
 *  using BehaviourMachine;
 *  
 *  public class CustomState : InternalStateBehaviour {
 *      FloatVar velocity;
 *      GameObjectVar player;
 *      
 *      void Awake () {
 *          // Blackboard variable
 *          velocity = blackboard.GetFloatVar("Velocity");
 *          velocity.Value = 10f;
 *          
 *          // Global variable
 *          player = GlobalBlackboard.Instance.GetGameObjectVar("Player");
 *          Debug.Log(player.Value);
 *      }
 *  }
 * \endcode
 *
 *
 * \section sec_customTreeNodes Custom Node
 *
 * The base classes for nodes are: BehaviourMachine.ActionNode, BehaviourMachine.ConditionNode, BehaviourMachine.CompositeNode and BehaviourMachine.DecoratorNode.
 *
 * To create a simple action node inherit from the ActionNode class and override the virtual methods: 
 *
 * \code
 *  using UnityEngine;
 *  using System.Collections;
 *  using BehaviourMachine;
 *  
 *  public class CustomNode : ActionNode {
 *      
 *      // Called when the owner (BehaviourTree or ActionState) is enabled
 *      public override void OnEnable () {}
 *
 *      // Called when the node starts its execution
 *      public override void Start () {}
 *
 *      // This function is called when the node is in execution
 *      public override void Update () {
 *          // Do stuff
 *          
 *          // Never forget to set the node status
 *          status = Status.Running;
 *      }
 *
 *      // Called when the node ends its execution
 *      public override void End () {}
 *
 *      // Called when the owner (BehaviourTree or ActionState) is disabled
 *      public override void OnDisable () {}
 *
 *      // This function is called to reset the default values of the node
 *      public override void Reset () {}
 *
 *      // Called when the script is loaded or a value is changed in the inspector (Called in the editor only)
 *      public override void OnValidate () {}
 *  }
 * \endcode
 *
 * If you want full control over the node, then you should override the OnTick method. In that case the functions Start, Update and End will never be called.
 *
 * \code
 *  using UnityEngine;
 *  using System.Collections;
 *  using BehaviourMachine;
 *  
 *  public class CustomNode : ActionNode {
 *      
 *      // Called when the node will be ticked
 *      public override void OnTick () {
 *          // Do stuff
 *          
 *          // Never forget to set the node status
 *          status = Status.Success;
 *      }
 *  }
 * \endcode
 *
 * You should always set the execution status of the node in the Update or OnTick methods:
 * 
 * Failure: If the node's execution has failed;
 * Success: if the node's execution has successfully finished;
 * Running: If the node needs more frames to finish its execution;
 * Error: If the node cannot be executed due to an error.
 *
 * The nodes are stored in the NodeSerialization class within the BehaviourTree/ActionState using a very fast custom serialization, only public members are serialized. The supported types are: int, string, float, Enum, bool, Vector2, Vector3, Vector4, Quaternion, Rect, Color, LayerMask, AnimationCurve, UnityEngine.Object and derived types, GameObject, Texture, Material, BehaviourMachine.FsmEvent, BehaviourMachine.Variable, BehaviourMachine.InternalStateBehaviour, BehaviourMachine.FloatVar, BehaviourMachine.IntVar, BehaviourMachine.BoolVar, BehaviourMachine.StringVar, BehaviourMachine.Vector3Var, BehaviourMachine.RectVar, BehaviourMachine.ColorVar, BehaviourMachine.QuaternionVar, BehaviourMachine.GameObjectVar, BehaviourMachine.TextureVar, BehaviourMachine.MaterialVar, BehaviourMachine.ObjectVar and Arrays. Multi-arrays are also supported.
 *
 * The BehaviourTree and the ActionState are the only components that uses reflection. 
 *
 * \section sec_customVariables Custom Variables
 *
 * You can create your own custom variable/constant. You can't share this variable in a Blackboard yet, but you can override the get/set Value property to do what you desire. Below is an example of a custom variable that let you use the position of an object in any node property that you can use a BehaviourMachine.Vector3Var variable: 
 *
 * \code
 * using UnityEngine;
 * using System.Collections;
 * using BehaviourMachine;
 * 
 * [System.Serializable]
 * [CustomVariable("Transform")]
 * public class MyCustomVariable : Vector3Var {
 * 
 *     [Tooltip("The target Transform")]
 *     public Transform transform;
 *  
 *     public override Vector3 Value {
 *         get {return transform.position;}
 *         set {transform.position = value;}
 *     }
 *  
 *     public MyCustomVariable () : base () {}
 *  
 *     public MyCustomVariable (GameObject self) : base () {
 *         this.transform = self.transform;
 *     }
 * }
 * \endcode
 *
 * Here are a list of the base classes for variables: BehaviourMachine.Variable, BehaviourMachine.FloatVar, BehaviourMachine.IntVar, BehaviourMachine.BoolVar, BehaviourMachine.StringVar, BehaviourMachine.Vector3Var, BehaviourMachine.RectVar, BehaviourMachine.ColorVar, BehaviourMachine.QuaternionVar, BehaviourMachine.GameObjectVar, BehaviourMachine.TextureVar, BehaviourMachine.MaterialVar, BehaviourMachine.ObjectVar.
 *
 */
