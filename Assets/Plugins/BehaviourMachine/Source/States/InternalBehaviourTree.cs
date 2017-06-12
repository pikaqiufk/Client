//----------------------------------------------
//            Behaviour Machine
// Copyright © 2014 Anderson Campos Cardoso
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BehaviourMachine
{

    /// <summary>
    /// Visual scripting component.
    /// Create A.I., animations, cutscenes, interactive objects and more without programming.
    /// The BehaviourTree has a set of nodes arranged in a tree struct, you can combine these nodes in a lot of different ways to create unique behaviors.
    /// </summary>
    [AddComponentMenu("")]
    public class InternalBehaviourTree : ParentBehaviour, INodeOwner
    {

        #region Members
        [SerializeField]
        bool m_IgnoreTimeScale = false;

        [SerializeField]
        bool m_ResetStatusOnDisable = true;

        [HideInInspector]
        [SerializeField]
        List<InternalStateBehaviour> m_EnabledStates = new List<InternalStateBehaviour>();

        [HideInInspector]
        [SerializeField]
        NodeSerialization m_NodeSerialization;

        [System.NonSerialized]
        ActionNode[] m_Nodes;

        [System.NonSerialized]
        FunctionNode[] m_FunctionNodes;

        [System.NonSerialized]
        bool m_HierarchyChanged;

        [System.NonSerialized]
        float m_LastRealTime = 0f;

        [System.NonSerialized]
        bool m_NodesEnabled = false;

        bool m_IsPrefab = true;
        #endregion Members


        #region Properties
        /// <summary>
        /// The enabled states in this tree.
        /// </summary>
        public InternalStateBehaviour[] enabledStates { get { return m_EnabledStates.ToArray(); } }

        /// <summary>
        /// The tree is ignoring time scale?
        /// </summary>
        public bool ignoreTimeScale { get { return m_IgnoreTimeScale; } set { m_IgnoreTimeScale = value; } }

        /// <summary>
        /// If False then the tree will keep the execution state when disabled; otherwise the tree's execution will be reset when the tree becomes disabled.
        /// </summary>
        public bool resetStatusOnDisable { get { return m_ResetStatusOnDisable; } set { m_ResetStatusOnDisable = value; } }

        /// <summary>
        /// If ignoreTimeScale is true then return the real delta time; otherwise return Time.deltaTime.
        /// </summary>
        public float deltaTime { get { return m_IgnoreTimeScale ? Time.realtimeSinceStartup - m_LastRealTime : Time.deltaTime; } }
        #endregion Properties


        #region Events
        /// <summary>
        /// Unity timed events.
        /// </summary>
        public event SimpleCallback onEnable, onDisable, start, fixedUpdate, update, lateUpdate, onDestroy;
        #endregion Events


        #region Enabled State
        /// <summary>
        /// Callback called by a child InternalStateBehaviour when it's enabled.
        /// </summary>
        protected override void OnEnableState(InternalStateBehaviour childState)
        {
            // Update the enabled list
            m_EnabledStates.Add(childState);
        }

        /// <summary>
        /// Callback called by a child InternalStateBehaviour when it's disabled.
        /// </summary>
        protected override void OnDisableState(InternalStateBehaviour childState)
        {
            // Update the enabled list
            m_EnabledStates.Remove(childState);
        }

        /// <summary>
        /// Returns true if the supplied child state is enabled; false otherwise.
        /// <param name="childState">A child state of the ParentBehaviour.</param>
        /// <returns>.</returns>
        /// </summary>
        public override bool IsEnabled(InternalStateBehaviour childState)
        {
            return m_EnabledStates.Contains(childState);
        }

        /// <summary>
        /// Returns the enabled state name in this parent.
        /// <returns>The enabled state name.</returns>
        /// </summary>
        public override string GetEnabledStateName()
        {
            if (enabled)
                return this.stateName;
            else
                return "No State";
        }
        #endregion Enabled State


        #region Private Methods
        /// <summary>
        /// Updates the nodes order.
        /// </summary>
        void UpdateNodes()
        {
            var oldNodes = GetNodes();

            // Get all root nodes
            var rootNodes = new List<ActionNode>();
            {
                // foreach(var n in oldNodes)
                var __enumerator1 = (oldNodes).GetEnumerator();
                while (__enumerator1.MoveNext())
                {
                    var n = (ActionNode)__enumerator1.Current;
                    {
                        if (n.isRoot)
                            rootNodes.Add(n);
                    }
                }
            }

            var newNodes = new List<ActionNode>();

            for (int i = 0; i < rootNodes.Count; i++)
            {
                BranchNode branch = rootNodes[i] as BranchNode;
                if (branch != null)
                    newNodes.AddRange(branch.GetHierarchy());
                else
                    newNodes.Add(rootNodes[i]);
            }
            m_Nodes = newNodes.ToArray();
        }

        /// <summary>
        /// Returns a unique node id.
        /// <returns>Unique node id.<returns>
        /// </summary>
        int GetNewUniqueID()
        {
            // Create the list of node id
            var ids = new List<int>();
            {
                // foreach(var node in GetNodes())
                var __enumerator2 = (GetNodes()).GetEnumerator();
                while (__enumerator2.MoveNext())
                {
                    var node = (ActionNode)__enumerator2.Current;
                    {
                        ids.Add(node.instanceID);
                    }
                }
            }

            // The new unique id
            int id = 1;

            while (true)
            {
                if (!ids.Contains(id))
                    return id;
                id++;
            }
        }

        /// <summary>
        /// Returns the function nodes in the tree.
        /// <returns>The function nodes in the tree.</returns>
        /// </summary>
        FunctionNode[] GetFunctionNodes()
        {
            if (m_Nodes == null)
                GetNodes();

            // Create the function list
            var functionList = new List<FunctionNode>();

            // Searching for FunctionNode
            for (int i = 0; i < m_Nodes.Length; i++)
            {
                if (m_Nodes[i].isRoot)
                {
                    // It's a function node?
                    var function = m_Nodes[i] as FunctionNode;
                    if (function != null)
                        functionList.Add(function);
                }
            }

            return functionList.ToArray();
        }


        #region Node Callbacks
        /// <summary>
        /// Call the OnEnable method in nodes.
        /// </summary>
        void OnEnableNodes()
        {
            if (m_Nodes != null)
            {
                for (int i = 0; i < m_Nodes.Length; i++)
                    m_Nodes[i].OnEnable();
            }

            m_NodesEnabled = true;
        }

        /// <summary>
        /// Restart nodes if needed and call the OnDisable method.
        /// </summary>
        void OnDisableNodes()
        {
            // Restart nodes?
            if (m_ResetStatusOnDisable && m_FunctionNodes != null)
            {
                for (int i = 0; i < m_FunctionNodes.Length; i++)
                    m_FunctionNodes[i].ResetStatus();
            }

            if (m_Nodes != null)
            {
                // Call OnDisable in nodes
                for (int i = 0; i < m_Nodes.Length; i++)
                    m_Nodes[i].OnDisable();
            }

            m_NodesEnabled = false;
        }
        #endregion Node Callbacks
        #endregion Private Methods


        #region Unity Callbacks
        /// <summary>
        /// Unity callback called when the script instance is being loaded.
        /// Check link hideFlags. Only used in editor.
        /// </summary>
        void Awake()
        {
            m_IsPrefab = false;

            // Load nodes
            this.LoadNodes();

            // #if UNITY_EDITOR
            // if (!Application.isPlaying)
            //     return;
            // #endif
        }

        /// <summary>
        /// OnEnable is called when the tree becomes enabled and active.
        /// Call OnEnable Nodes.
        /// </summary>
        void OnEnable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif

            // Load nodes
            if (m_Nodes == null)
                this.LoadNodes();

            // Call OnEnable in nodes
            this.OnEnableNodes();

            // Call onEnable event
            if (onEnable != null)
                onEnable();
        }

        /// <summary>
        /// OnDisable is called when the tree becomes disabled or inactive.
        /// Call OnDisable Nodes.
        /// </summary>
        void OnDisable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif

            // Call onDisable event
            if (onDisable != null)
                onDisable();

            this.OnDisableNodes();
        }

        /// <summary>
        /// Start is called just before any of the Update methods are called the first time.
        /// Call Start nodes.
        /// </summary>
        void Start()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif

            if (start != null)
                start();
        }

        /// <summary>
        /// Update is called every frame, if the tree is enabled.
        /// Call Update nodes.
        /// </summary>
        void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif

            if (update != null)
                update();
        }


        /// <summary>
        /// FixedUpdate is called every fixed framerate frame, if the tree is enabled.
        /// Call FixedUpdate nodes.
        /// </summary>
        void FixedUpdate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif

            if (fixedUpdate != null)
                fixedUpdate();
        }

        /// <summary>
        /// FixedUpdate is called every frame just before Update, if the tree is enabled.
        /// Call LateUpdate nodes.
        /// </summary>
        void LateUpdate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif

            if (lateUpdate != null)
                lateUpdate();

            m_LastRealTime = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// Unity callback called when the tree will be destroyed.
        /// Call onDestroy nodes.
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (!Application.isPlaying)
                return;

            if (onDestroy != null)
                onDestroy();
        }

        /// <summary>
        /// Unity callback called when the user hits the Reset button in the Inspector's context menu or when adding the component the first time (Editor only).
        /// Create OnEnable and Update nodes.
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            LoadNodes();

            if (m_Nodes.Length == 0)
            {
                m_Nodes = new ActionNode[0];

                AddNode(typeof(OnEnable));
                AddNode(typeof(Update));

                SaveNodes();
            }
        }
        #endregion Unity Callbacks


        #region Public Methods
        /// <summary> 
        /// Uses the supplied event on the tree, if fails send the event to all enabled states in the tree.
        /// <param name="eventID">The id of the event. The FsmEvent's id in the Blackboard.</param>
        /// <returns>Returns True if the event was used; false otherwise.</returns>
        /// </summary>
        public override bool ProcessEvent(int eventID)
        {
            if (base.ProcessEvent(eventID))
                return true;
            else
            {
                bool eventUsed = false;

                for (int i = 0; i < m_EnabledStates.Count; i++)
                    eventUsed = m_EnabledStates[i].ProcessEvent(eventID) || eventUsed;

                return eventUsed;
            }
        }

        /// <summary>
        /// Marks the tree as changed.
        /// This needs to be called whenever the tree hierarchy has changed.
        /// </summary>
        public void HierarchyChanged()
        {
            m_HierarchyChanged = true;
        }

        /// <summary>
        /// Returns all nodes.
        /// <returns>All nodes in this tree.</returns>
        /// </summary>
        public IList<ActionNode> GetNodes()
        {
            if (m_Nodes == null)
                this.LoadNodes();

            if (m_HierarchyChanged)
            {
                m_HierarchyChanged = false;
                this.UpdateNodes();
            }

            return m_Nodes;
        }

        /// <summary>
        /// Returns the node index.
        /// <param name="node">The taget node.</param>
        /// <returns>The index of the supplied node.</returns>
        /// </summary>
        public int GetIndex(ActionNode node)
        {
            var nodes = GetNodes();
            return nodes != null ? nodes.IndexOf(node) : -1;
        }

        /// <summary>
        /// Returns the node that has the supplied id.
        /// <param name="nodeID">The taget node id.</param>
        /// <returns>The node with id equals to nodeID.</returns>
        /// </summary>
        public ActionNode GetNode(int nodeID)
        {
            var nodes = GetNodes();
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].instanceID == nodeID)
                    return nodes[i];
            }
            return null;
        }

        /// <summary>
        /// Adds a new node to the tree.
        /// <param name="type">The type of the new node.</param>
        /// <returns>The new created node.</returns>
        /// </summary>
        public ActionNode AddNode(System.Type type)
        {
            var node = ActionNode.CreateInstance(type, gameObject, this);
            if (node != null)
            {
                NodeSerialization.idField.SetValue(node, GetNewUniqueID());
                GetNodes();

                // Add node
                var nodes = new List<ActionNode>(m_Nodes);
                nodes.Add(node);
                m_Nodes = nodes.ToArray();

                if (Application.isPlaying)
                {
                    m_FunctionNodes = this.GetFunctionNodes();
                    // The tree is enabled?
                    if (this.enabled)
                        node.OnEnable();
                }

                HierarchyChanged();
            }

            return node;
        }

        /// <summary>
        /// Removes node from tree.
        /// <param name="node">The node to be removed.</param>
        /// <param name="includeHierarchy">If true, the hierarchy will also be removed.</param>
        /// </summary>
        public void RemoveNode(ActionNode node, bool includeHierarchy)
        {
            GetNodes();
            var nodes = new List<ActionNode>(m_Nodes);
            BranchNode branch = node as BranchNode;

            // Remove children
            if (includeHierarchy && branch != null)
            {
                {
                    var __array3 = branch.GetHierarchy();
                    var __arrayLength3 = __array3.Length;
                    for (int __i3 = 0; __i3 < __arrayLength3; ++__i3)
                    {
                        var n = (ActionNode)__array3[__i3];
                        {
                            nodes.Remove(n);
                        }
                    }
                }
            }

            // Remove node
            nodes.Remove(node);
            m_Nodes = nodes.ToArray();

            if (Application.isPlaying && this.enabled)
            {
                // Update function nodes
                m_FunctionNodes = this.GetFunctionNodes();
                // Reset status
                node.ResetStatus();
                // Disable node
                node.OnDisable();
            }

            HierarchyChanged();
        }

        /// <summary>
        /// Moves the node from oldIndex to newIndex in the tree.
        /// <param name="oldIndex">The current index of the node to be moved.</param>
        /// <param name="newIndex">The target index to move the node.</param>
        /// </summary>
        public void MoveNode(int oldIndex, int newIndex)
        {
            GetNodes();

            if (oldIndex != newIndex && oldIndex >= 0 && newIndex >= 0 && oldIndex < m_Nodes.Length && newIndex < m_Nodes.Length)
            {
                var node = m_Nodes[oldIndex];

                // Move node
                var nodes = new List<ActionNode>(m_Nodes);
                nodes.Remove(node);
                nodes.Insert(newIndex, node);
                m_Nodes = nodes.ToArray();

                HierarchyChanged();
            }
        }

        /// <summary>
        /// Saves nodes in m_NodeSerialization.
        /// </summary>
        public void SaveNodes()
        {
            // Debug.Log("Saving Nodes " + name);
            GetNodes();
            m_NodeSerialization.SaveNodes(m_Nodes, gameObject, this);
        }

        /// <summary>
        /// Loads nodes from m_NodeSerialization.
        /// </summary>
        public void LoadNodes()
        {
            // Debug.Log("Loading Nodes " + name);

            // Validate the NodeSerialization object
            if (m_NodeSerialization == null)
                m_NodeSerialization = new NodeSerialization();

            // Is playing and this tree is enabled?
            if (m_NodesEnabled)
            {
                // Force ResetStatus
                if (m_FunctionNodes != null)
                {
                    for (int i = 0; i < m_FunctionNodes.Length; i++)
                        m_FunctionNodes[i].ResetStatus();
                }

                // Call OnDisable in nodes
                if (m_Nodes != null)
                {
                    for (int i = 0; i < m_Nodes.Length; i++)
                        m_Nodes[i].OnDisable();
                }
            }

            // Load nodes
            m_Nodes = m_NodeSerialization.LoadNodes(gameObject, this);

            if (m_HierarchyChanged)
            {
                m_HierarchyChanged = false;
                UpdateNodes();
            }

            if (m_NodeSerialization.resaveNodes)
                SaveNodes();

            m_FunctionNodes = this.GetFunctionNodes();

            // Call Awake in nodes
            if (!m_IsPrefab && Application.isPlaying)
            {
                for (int i = 0; i < m_Nodes.Length; i++)
                    m_Nodes[i].Awake();
            }

            // Reenable nodes
            if (m_NodesEnabled)
                this.OnEnableNodes();
        }


        /// <summary>
        /// Clears all node data.
        /// </summary>
        public void Clear()
        {
            m_NodeSerialization.Clear();
        }
        #endregion Public Methods
    }
}
