#region using

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace FastShadowReceiver
{
    [RequireComponent(typeof (Projector))]
    public class AutoProjector : MonoBehaviour
    {
        public const string SHADOW_ALPHA_PROPERTY_NAME = "_Alpha";

        public AutoProjector()
        {
            uvIndex = 0;
            quadShadowWeight = 0.0f;
            isVisible = false;
        }

        private bool m_canFadeout;
        private bool m_enableProjector;
        private float m_originalAlpha;
        [SerializeField] [HideInInspector] private Component m_predictor;
        private UnityProjector m_projector;
        private float m_projectorWeight;
        private int m_receiverLayer = -1;
        private List<ReceiverBase> m_receivers;
        private Transform m_transform;
        private Vector3[] m_vertices;
        public float distanceFromCamera { get; private set; }

        public bool isProjectorActive
        {
            get { return m_projector.projector.enabled; }
        }

        public bool isVisible { get; private set; }

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

        // called from ProjectorManager
        public UnityProjector projector
        {
            get
            {
#if UNITY_EDITOR
                if (m_projector == null)
                {
                    m_projector = new UnityProjector(GetComponent<Projector>());
                }
#endif
                return m_projector;
            }
        }

        public float projectorShadowAlpha
        {
            get { return m_originalAlpha*m_projectorWeight; }
        }

        public float quadShadowAlpha
        {
            get { return m_originalAlpha*quadShadowWeight; }
        }

        public float quadShadowWeight { get; private set; }
        public int uvIndex { get; set; }

        private void Awake()
        {
            m_projector = new UnityProjector(GetComponent<Projector>());
            // check if the components of this game object have UpdateTransform methods
            var components = gameObject.GetComponents<Component>();
            for (var i = 0; i < components.Length; ++i)
            {
                var type = components[i].GetType();
                var method = type.GetMethod("UpdateTransform");
                if (method != null)
                {
                    m_projector.updateTransform +=
                        Delegate.CreateDelegate(typeof (Action), components[i], method) as Action;
                }
            }
            m_transform = transform;
            m_vertices = new Vector3[4];
            if (m_projector.projector.material.HasProperty(SHADOW_ALPHA_PROPERTY_NAME))
            {
                m_projector.projector.material = new Material(m_projector.projector.material);
                    // make a copy of material so that we can change alpha later.
                m_originalAlpha = m_projector.projector.material.GetFloat(SHADOW_ALPHA_PROPERTY_NAME);
                m_canFadeout = true;
            }
            else
            {
                m_originalAlpha = 1.0f;
                m_canFadeout = false;
            }
        }

        public void ClearReceivers()
        {
            m_receivers = null;
        }

        public void DisableProjector()
        {
            m_enableProjector = false;
        }

        public void EnableProjector()
        {
            m_projector.projector.enabled = true;
            m_enableProjector = true;
        }

        public void GetQuadShadowVertices(Vector3[] vertices, int offset)
        {
            vertices[offset++] = m_vertices[0];
            vertices[offset++] = m_vertices[1];
            vertices[offset++] = m_vertices[2];
            vertices[offset++] = m_vertices[3];
        }

        public List<ReceiverBase> GetReceivers()
        {
            return m_receivers;
        }

        private void OnDisable()
        {
            // check if ProjectorManager.Instance is null or not, in case that ProjectorManager is already destroyed.
            if (ProjectorManager.Instance != null)
            {
                ProjectorManager.Instance.RemoveProjector(this);
            }
        }

        private void OnEnable()
        {
            // register projector to ProjectorManager, and reset visibility.
            ProjectorManager.Instance.AddProjector(this);
            m_projectorWeight = 0.0f;
            quadShadowWeight = 0.0f;
            m_projector.projector.enabled = false;
            m_enableProjector = false;
        }

        // call this function manually when blob shadow texture is changed
        public void OnTextureChanged()
        {
            ProjectorManager.Instance.UpdateUVIndex(this);
        }

        private void OnValidate()
        {
            // when script files are rebuilt while scene running, unity tries to restore m_receivers and make it empty list rather than null.
            if (m_receivers != null && m_receivers.Count == 0)
            {
                m_receivers = null;
            }
        }

        public void SetReceiverLayer(int receiverLayer, int receiverLayerMask)
        {
            if (m_receiverLayer != receiverLayer && 0 <= receiverLayer)
            {
                m_projector.projector.ignoreLayers |= receiverLayerMask;
                m_projector.projector.ignoreLayers &= ~(1 << receiverLayer);
            }
            m_receiverLayer = receiverLayer;
            if (0 <= receiverLayer)
            {
                for (var j = 0; j < m_receivers.Count; ++j)
                {
                    m_receivers[j].gameObject.layer = receiverLayer;
                }
            }
        }

        public void SetReceivers(List<ReceiverBase> receivers)
        {
            m_receivers = receivers;
            for (var j = 0; j < receivers.Count; ++j)
            {
                var receiver = receivers[j];
                receiver.projectorInterface = m_projector;
                if (m_predictor != null)
                {
                    var meshReceiver = receiver as MeshShadowReceiver;
                    if (meshReceiver != null)
                    {
                        meshReceiver.predictor = predictor;
                    }
                }
                receiver.gameObject.SetActive(true);
            }
        }

        public bool UpdateVisibility(Plane targetPlane,
                                     Plane zPlane,
                                     Plane[] cameraClipPlanes,
                                     float cameraNear,
                                     float cameraFar)
        {
            // calculate quadrangle shadow vertex positions, and check visibility.
            m_projector.GetPlaneIntersection(m_vertices, targetPlane);
            var o = m_transform.position;
            isVisible = true;
            var z0 = zPlane.GetDistanceToPoint(o);
            var z1 = zPlane.GetDistanceToPoint(m_vertices[0]);
            var z2 = zPlane.GetDistanceToPoint(m_vertices[1]);
            var z3 = zPlane.GetDistanceToPoint(m_vertices[2]);
            var z4 = zPlane.GetDistanceToPoint(m_vertices[3]);
            distanceFromCamera = 0.5f*(z1 + z3);
            if (z0 < cameraNear && z1 < cameraNear && z3 < cameraNear && z4 < cameraNear)
            {
                isVisible = false;
            }
            else if (cameraFar < z0 && cameraFar < z1 && cameraFar < z2 && cameraFar < z4)
            {
                isVisible = false;
            }
            else
            {
                for (var i = 0; i < cameraClipPlanes.Length; ++i)
                {
                    if (0.0f < cameraClipPlanes[i].GetDistanceToPoint(o))
                    {
                        continue;
                    }
                    if (0.0f < cameraClipPlanes[i].GetDistanceToPoint(m_vertices[0]))
                    {
                        continue;
                    }
                    if (0.0f < cameraClipPlanes[i].GetDistanceToPoint(m_vertices[1]))
                    {
                        continue;
                    }
                    if (0.0f < cameraClipPlanes[i].GetDistanceToPoint(m_vertices[2]))
                    {
                        continue;
                    }
                    if (0.0f < cameraClipPlanes[i].GetDistanceToPoint(m_vertices[3]))
                    {
                        continue;
                    }
                    isVisible = false;
                    break;
                }
            }
            if (!isVisible)
            {
                m_projectorWeight = 0.0f;
                quadShadowWeight = 0.0f;
                m_projector.projector.enabled = false;
                m_enableProjector = false;
            }
            return isVisible;
        }

        public void UpdateWeights(float fadeStep, bool alwaysShowQuadShadow)
        {
            if (isVisible)
            {
                var bShowQuadShadow = alwaysShowQuadShadow || !m_enableProjector;
                var wasVisible = (0.0f < quadShadowWeight || 0.0f < m_projectorWeight);
                if (m_enableProjector)
                {
                    if (m_projectorWeight < 1.0f)
                    {
                        if (m_canFadeout && wasVisible)
                        {
                            m_projectorWeight = Mathf.Min(1.0f, m_projectorWeight + fadeStep);
                        }
                        else
                        {
                            m_projectorWeight = 1.0f;
                        }
                        m_projector.projector.material.SetFloat(SHADOW_ALPHA_PROPERTY_NAME, projectorShadowAlpha);
                    }
                }
                else
                {
                    if (0.0f < m_projectorWeight)
                    {
                        m_projectorWeight = Mathf.Max(0.0f, m_projectorWeight - fadeStep);
                        m_projector.projector.material.SetFloat(SHADOW_ALPHA_PROPERTY_NAME, projectorShadowAlpha);
                        if (m_projectorWeight <= 0.0f)
                        {
                            m_projector.projector.enabled = false;
                        }
                    }
                }
                if (bShowQuadShadow)
                {
                    if (m_canFadeout && wasVisible)
                    {
                        quadShadowWeight = Mathf.Min(1.0f, quadShadowWeight + fadeStep);
                    }
                    else
                    {
                        quadShadowWeight = 1.0f;
                    }
                }
                else
                {
                    quadShadowWeight = Mathf.Max(0.0f, quadShadowWeight - fadeStep);
                }
            }
        }
    }
}