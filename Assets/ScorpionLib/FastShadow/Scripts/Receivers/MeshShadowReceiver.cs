#region using

using Nyahoon;
using UnityEngine;

#endregion

namespace FastShadowReceiver
{
    public class MeshShadowReceiver : ReceiverBase
    {
        [SerializeField] private bool m_cullBackFace;
        [SerializeField] [HideInInspector] private bool m_enablePrediction;
        [SerializeField] private bool m_hasNormals;
        [SerializeField] [HideInInspector] private float m_margin;
        private MeshState m_meshState;
        [SerializeField] private Transform m_meshTransform;
        [SerializeField] private MeshTreeBase m_meshTree;
        private bool m_multiThreading;
        [SerializeField] [HideInInspector] private Component m_predictor;
        private float[] m_prevClipDistance;
        private Plane[] m_prevClipPlanes;
        private Vector3 m_projectorPositionWhenMeshCreated;
        private Quaternion m_projectorRotationWhenMeshCreated;
        [SerializeField] private bool m_scissor = true;
        [SerializeField] private float m_scissorMargin; // applicable if m_meshTree is BinaryMeshTree
        private MeshTreeSearch m_search;
        private Transform m_selfTransform;
        [SerializeField] [HideInInspector] private bool m_updateOnlyWhenProjectorMoved;

        public bool cullBackFace
        {
            get { return m_cullBackFace; }
            set { m_cullBackFace = value; }
        }

        public bool hasNormals
        {
            get { return m_hasNormals; }
            set { m_hasNormals = value; }
        }

        public float margin
        {
            get { return m_margin; }
            set { m_margin = value; }
        }

        public Transform meshTransform
        {
            get { return m_meshTransform; }
            set { m_meshTransform = value; }
        }

        public MeshTreeBase meshTree
        {
            get { return m_meshTree; }
            set
            {
                if (m_meshTree != value)
                {
                    if (m_meshTree != null)
                    {
                        if (!m_meshTree.IsBuildFinished())
                        {
                            m_meshTree.WaitForBuild();
                        }
                        if (m_search != null)
                        {
                            SyncAndSwap();
                        }
                    }
                    m_meshTree = value;
                    if (value != null)
                    {
                        if (!m_meshTree.IsBuildFinished() && !m_meshTree.IsBuilding())
                        {
                            if (m_meshTree.IsPrebuilt())
                            {
                                m_meshTree.BuildFromPrebuiltData();
                                if (Debug.isDebugBuild || Application.isEditor)
                                {
                                    CheckError();
                                }
                            }
                            else
                            {
                                m_meshTree.AsyncBuild();
                            }
                        }
                        m_search = value.CreateSearch();
                    }
                }
            }
        }

        public bool multiThreadEnabled
        {
            get { return m_multiThreading; }
            set
            {
                if (m_multiThreading != value)
                {
                    if (m_multiThreading && m_search != null)
                    {
                        m_search.Wait();
                    }
                    m_multiThreading = value;
                    if (value)
                    {
                        ThreadPool.InitInstance();
                    }
                }
            }
        }

        public bool predictEnabled
        {
            get { return m_enablePrediction && m_predictor is ITransformPredictor && m_multiThreading; }
            set { m_enablePrediction = value; }
        }

        public ITransformPredictor predictor
        {
            get { return m_predictor as ITransformPredictor; }
            set
            {
                if (value == null || value is Component)
                {
                    m_predictor = value as Component;
                }
                else if (Debug.isDebugBuild || Application.isEditor)
                {
                    Debug.LogError("predictor must be a Component!");
                }
            }
        }

        public bool scissorEnabled
        {
            get { return m_scissor; }
            set { m_scissor = value; }
        }

        public float scissorMargin
        {
            get { return m_scissorMargin; }
            set { m_scissorMargin = value; }
        }

        public bool updateOnlyWhenProjectorMoved
        {
            get { return m_updateOnlyWhenProjectorMoved; }
            set { m_updateOnlyWhenProjectorMoved = value; }
        }

        private void CheckError()
        {
            if (m_meshTree != null && m_meshTransform != null)
            {
                var error = m_meshTree.CheckError(m_meshTransform.gameObject);
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogWarning(error, this);
                }
            }
        }

        protected override bool IsReady()
        {
            if (m_meshTransform == null || m_meshTree == null || !base.IsReady())
            {
                return false;
            }
            if (m_meshTree.IsBuildFinished() || m_meshTree.IsPrebuilt())
            {
                return true;
            }
            return (Application.isPlaying && m_meshTree.IsReadyToBuild());
        }

        protected override void OnAwake()
        {
            m_multiThreading = 1 < SystemInfo.processorCount;
            if (m_multiThreading)
            {
                ThreadPool.InitInstance();
            }
            base.OnAwake();
            m_selfTransform = transform;
            m_meshState = MeshState.Uncreated;
            if (m_meshTree != null)
            {
                if (!m_meshTree.IsBuildFinished())
                {
                    if (m_meshTree.IsPrebuilt())
                    {
                        m_meshTree.BuildFromPrebuiltData();
                        if (Debug.isDebugBuild || Application.isEditor)
                        {
                            CheckError();
                        }
                    }
                    else
                    {
                        m_meshTree.AsyncBuild();
                    }
                }
                m_search = m_meshTree.CreateSearch();
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            if (m_meshTree != null && !m_meshTree.IsBuildFinished())
            {
                m_meshTree.WaitForBuild();
            }
            if (Debug.isDebugBuild || Application.isEditor)
            {
                CheckError();
            }
        }

        protected override void OnUpdate()
        {
            UpdateMesh();
        }

        private void OnWillRenderObject()
        {
            if (!predictEnabled)
            {
                SyncAndSwap();
            }
        }

        public void SyncAndSwap()
        {
#if UNITY_EDITOR
            if (m_search == null)
            {
                return;
            }
#endif
            if (m_meshState == MeshState.Creating)
            {
                m_search.Wait();
                if (m_search.bounds.extents == Vector3.zero)
                {
                    Hide(true);
                }
                else
                {
                    Hide(false);
                    SwapMesh();
                    currentMesh.Clear();
                    currentMesh.vertices = m_search.vertices;
                    if (m_search.m_bOutputNormals)
                    {
                        currentMesh.normals = m_search.normals;
                    }
                    currentMesh.triangles = m_search.triangles;
                    currentMesh.bounds = m_search.bounds;
                }
                m_meshState = MeshState.Created;
            }
        }

        public void UpdateMesh()
        {
#if UNITY_EDITOR
            if (m_meshTree == null || m_search == null || m_search.GetType() != m_meshTree.GetSearchType())
            {
                if (!IsReady())
                {
                    return;
                }
                OnAwake();
                OnStart();
            }
            if (m_meshTree == null || m_meshTransform == null)
            {
                return;
            }
#else
			if (m_meshTree == null || m_meshTransform == null) {
				Hide(true);
				return;
			}
#endif
            if (!m_meshTree.IsBuildFinished() || projector == null)
            {
                Hide(true);
                return;
            }
            m_selfTransform.localScale = m_meshTransform.localScale;
            m_selfTransform.position = m_meshTransform.position;
            m_selfTransform.rotation = m_meshTransform.rotation;
            if (predictEnabled)
            {
                SyncAndSwap();
                if (!UpdateSearchCondition(true))
                {
                    return;
                }
                m_meshState = MeshState.Creating;
                m_search.AsyncStart(m_meshTree);
            }
            else if (m_multiThreading)
            {
                m_search.Wait();
                if (!UpdateSearchCondition(false))
                {
                    SyncAndSwap();
                    return;
                }
                currentMesh.bounds = m_meshTree.bounds;
                m_meshState = MeshState.Creating;
                m_search.AsyncStart(m_meshTree);
                Hide(false);
            }
            else
            {
                if (!UpdateSearchCondition(false))
                {
                    SyncAndSwap();
                    return;
                }
                m_meshTree.Search(m_search);
                if (m_search.bounds.extents == Vector3.zero)
                {
                    Hide(true);
                }
                else
                {
                    Hide(false);
                    SwapMesh();
                    currentMesh.Clear();
                    currentMesh.vertices = m_search.vertices;
                    if (m_search.m_bOutputNormals)
                    {
                        currentMesh.normals = m_search.normals;
                    }
                    currentMesh.triangles = m_search.triangles;
                    currentMesh.bounds = m_search.bounds;
                }
                m_meshState = MeshState.Created;
            }
        }

        private bool UpdateSearchCondition(bool bPredict)
        {
            var bSmallMove = false;
            var projectorPosition = Vector3.zero;
            var projectorRotation = Quaternion.identity;
            Plane[] prevClipPlanes = null;
            float[] prevClipDistance = null;
            var prevClipPlaneCount = 0;
            if (m_updateOnlyWhenProjectorMoved)
            {
                projectorPosition = m_meshTransform.InverseTransformPoint(projector.position);
                projectorRotation = projector.rotation;
                // check how much projector moved since last time mesh created
                if (m_meshState != MeshState.Uncreated)
                {
                    var move = projectorPosition - m_projectorPositionWhenMeshCreated;
                    var sqDistance = move.sqrMagnitude;
                    var limit = m_margin*m_margin + Mathf.Epsilon;
                    bSmallMove = (sqDistance <= limit);
                    if (!bSmallMove && sqDistance <= 3*limit)
                    {
                        move = Quaternion.Inverse(m_projectorRotationWhenMeshCreated)*move;
                        bSmallMove = (Mathf.Abs(move.x) <= m_margin && Mathf.Abs(move.y) <= m_margin &&
                                      Mathf.Abs(move.z) <= m_margin);
                    }
                    if (bSmallMove)
                    {
                        if (m_projectorRotationWhenMeshCreated == projectorRotation)
                        {
                            // We don't check other properties such as fieldOfView, farClipPlane, aspectRatio, and so on...
                            return false;
                        }
                        var rot = Quaternion.Inverse(m_projectorRotationWhenMeshCreated)*projectorRotation;
                        if (0.9999f < Mathf.Abs(rot.w))
                        {
                            // We don't check other properties such as fieldOfView, farClipPlane, aspectRatio, and so on...
                            return false;
                        }
                        var z = (rot*Vector3.forward);
                        var far = projector.farClipPlane;
                        bSmallMove = (far*far*(z.x*z.x + z.y*z.y) <= limit);
                        if (bSmallMove && 0.0f < m_margin)
                        {
                            // swap clip plane buffers so that we can check intersection later.
                            prevClipPlaneCount = m_search.m_clipPlanes.clipPlaneCount;
                            prevClipPlanes = m_search.m_clipPlanes.clipPlanes;
                            m_search.m_clipPlanes.clipPlanes = m_prevClipPlanes;
                            prevClipDistance = m_search.m_clipPlanes.maxDistance;
                            m_search.m_clipPlanes.maxDistance = m_prevClipDistance;
                        }
                    }
                }
            }
            m_search.m_bOutputNormals = m_hasNormals;
            m_search.m_bBackfaceCulling = m_cullBackFace;
            m_search.m_bScissor = m_scissor;
            m_search.m_scissorMargin = m_scissorMargin;
            m_search.SetProjectionDir(projector.isOrthographic,
                m_meshTransform.InverseTransformDirection(projector.direction),
                m_meshTransform.InverseTransformPoint(projector.position));
            if (bPredict)
            {
                projector.GetClipPlanes(ref m_search.m_clipPlanes, m_meshTransform, predictor);
            }
            else
            {
                projector.GetClipPlanes(ref m_search.m_clipPlanes, m_meshTransform);
            }
            if (m_margin != 0)
            {
                if (bSmallMove)
                {
                    // check whether the new projection volume intersects with the previous projection volume or not.
                    // this check is not robust, only check far clip plane. if you got a problem whereby the mesh was not updated at a right timing, please don't use m_margin.
                    var z = projector.direction;
                    var far = projector.farClipPlane;
                    if (m_search.m_clipPlanes.twoSideClipping)
                    {
                        far += Vector3.Dot(projectorPosition, z);
                        for (var i = 0; i < m_search.m_clipPlanes.scissorPlaneCount - 1 && bSmallMove; ++i)
                        {
                            var p0 = m_search.m_clipPlanes.clipPlanes[i];
                            var p1 = m_search.m_clipPlanes.clipPlanes[i + 1];
                            var l = Vector3.Cross(p0.normal, p1.normal);
                            var ldotz = Vector3.Dot(l, z);
                            if (Mathf.Abs(ldotz) < 0.001f)
                            {
                                continue;
                            }
                            Vector3 x0, x1, x2, x3;
                            // project arbitrary position onto plane p0 and the other side of p0
                            x0 = projectorPosition - p0.GetDistanceToPoint(projectorPosition)*p0.normal;
                            x1 = x0 + m_search.m_clipPlanes.maxDistance[i]*p0.normal;
                            // project x0, x1 onto plane p1 along p0
                            var n = Vector3.Cross(l, p0.normal);
                            var a = 1.0f/Vector3.Dot(n, p1.normal);
                            x0 = x0 - a*p1.GetDistanceToPoint(x0)*n;
                            x1 = x1 - a*p1.GetDistanceToPoint(x1)*n;
                            n = a*m_search.m_clipPlanes.maxDistance[i + 1]*n;
                            x2 = x0 + n;
                            x3 = x1 + n;
                            // project x0, x1, x2, x3 onto far clip plane along l.
                            a = 1.0f/ldotz;
                            x0 = x0 + (a*(far - Vector3.Dot(x0, z)))*l;
                            x1 = x1 + (a*(far - Vector3.Dot(x1, z)))*l;
                            x2 = x2 + (a*(far - Vector3.Dot(x2, z)))*l;
                            x3 = x3 + (a*(far - Vector3.Dot(x3, z)))*l;
                            for (var j = 0; j < prevClipPlaneCount; ++j)
                            {
                                var d = prevClipPlanes[j].GetDistanceToPoint(x0);
                                if (d < 0.0f || prevClipDistance[j] < d)
                                {
                                    bSmallMove = false;
                                    break;
                                }
                                d = prevClipPlanes[j].GetDistanceToPoint(x1);
                                if (d < 0.0f || prevClipDistance[j] < d)
                                {
                                    bSmallMove = false;
                                    break;
                                }
                                d = prevClipPlanes[j].GetDistanceToPoint(x2);
                                if (d < 0.0f || prevClipDistance[j] < d)
                                {
                                    bSmallMove = false;
                                    break;
                                }
                                d = prevClipPlanes[j].GetDistanceToPoint(x3);
                                if (d < 0.0f || prevClipDistance[j] < d)
                                {
                                    bSmallMove = false;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (var i = 0; i < m_search.m_clipPlanes.scissorPlaneCount && bSmallMove; ++i)
                        {
                            // assuming that projectorPosition is located on each clip plane.
                            var p0 = m_search.m_clipPlanes.clipPlanes[i];
                            var d0 = p0.GetDistanceToPoint(projectorPosition);
                            if (0.0001f*far < Mathf.Abs(d0))
                            {
                                continue;
                            }
                            var j = (i < m_search.m_clipPlanes.scissorPlaneCount - 1) ? i + 1 : 0;
                            var p1 = m_search.m_clipPlanes.clipPlanes[j];
                            var d1 = p1.GetDistanceToPoint(projectorPosition);
                            if (0.0001f*far < Mathf.Abs(d1))
                            {
                                ++i;
                                continue;
                            }
                            var l = Vector3.Cross(p0.normal, p1.normal);
                            var ldotz = Vector3.Dot(l, z);
                            if (Mathf.Abs(ldotz) < 0.001f)
                            {
                                continue;
                            }
                            var x = projectorPosition + (far/ldotz)*l;
                            for (var k = 0; k < prevClipPlaneCount; ++k)
                            {
                                if (prevClipPlanes[k].GetDistanceToPoint(x) < 0.0f)
                                {
                                    bSmallMove = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (bSmallMove)
                    {
                        m_search.m_clipPlanes.clipPlanes = prevClipPlanes;
                        m_search.m_clipPlanes.maxDistance = prevClipDistance;
                        return false;
                    }
                    m_prevClipPlanes = prevClipPlanes;
                    m_prevClipDistance = prevClipDistance;
                }
                var clipPlanes = m_search.m_clipPlanes;
                var doubleMargin = 2*m_margin;
                for (var i = 0; i < clipPlanes.clipPlaneCount; ++i)
                {
                    clipPlanes.clipPlanes[i].distance += m_margin;
                    if (clipPlanes.twoSideClipping)
                    {
                        clipPlanes.maxDistance[i] += doubleMargin;
                    }
                }
            }
            if (m_updateOnlyWhenProjectorMoved)
            {
                m_projectorPositionWhenMeshCreated = projectorPosition;
                m_projectorRotationWhenMeshCreated = projectorRotation;
            }
            return true;
        }

        private enum MeshState
        {
            Uncreated,
            Creating,
            Created
        }
    }
}