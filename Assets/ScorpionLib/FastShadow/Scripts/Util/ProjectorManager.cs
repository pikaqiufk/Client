#region using

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace FastShadowReceiver
{
    [RequireComponent(typeof (MeshRenderer))]
    public class ProjectorManager : MonoBehaviour
    {
        private const int BUFFER_COUNT = 2;

        static ProjectorManager()
        {
            Instance = null;
        }

        public ProjectorManager()
        {
            Instance = this;
            if (m_receivers == null)
            {
                m_receivers = new List<ReceiverBase>();
            }
            if (m_projectors == null)
            {
                m_projectors = new List<AutoProjector>();
            }
            m_receiverInstances = new List<List<ReceiverBase>>();
            m_freeReceiverInstances = new Stack<List<ReceiverBase>>();
        }

        [SerializeField] private Rect[] m_blobShadowTextureRects;
        [SerializeField] private Texture2D[] m_blobShadowTextures;
        private Plane[] m_cameraClipPlanes;
        private Transform m_cameraTransform;
        private Color32[] m_colors;
        [SerializeField] private LayerMask m_environmentLayers = 0;
        [SerializeField] private float m_fadeDuration = 0.5f;
        private int m_firstReceiverLayer;
        private Stack<List<ReceiverBase>> m_freeReceiverInstances;
        [SerializeField] private float m_infinitePlaneHeight;
        [SerializeField] private Vector3 m_infinitePlaneNormal = Vector3.up;
        [SerializeField] private Transform m_infinitePlaneTransform;
        private int m_lastReceiverLayer;
        [SerializeField] private Camera m_mainCamera;
        [SerializeField] private bool m_manualUpdate;
        private Mesh[] m_meshes;
        private MeshFilter m_meshFilter;
        private int m_nCurrentBuffer;
        [SerializeField] private Texture2D m_packedBlobShadowTexture;
        [SerializeField] private readonly int m_packedTextureMaxSize = 1024;
        [SerializeField] private int m_packedTexturePadding;
        [SerializeField] private float m_projectorFadeoutDistance = 20.0f;
        // private fields
        private readonly List<AutoProjector> m_projectors;
        [SerializeField] private LayerMask m_raycastPlaneMask = 0;
        private readonly List<List<ReceiverBase>> m_receiverInstances;
        // serialize field
        [SerializeField] private LayerMask m_receiverLayerMask;
        [SerializeField] private readonly List<ReceiverBase> m_receivers;
        private Renderer m_renderer;
        [SerializeField] private readonly string m_shadowTexName = "_ShadowTex";
        private int[] m_triangles;
        [SerializeField] private bool m_useInfinitePlane;
        private Vector2[] m_uvs;
        private Vector3[] m_vertices;

        public int activeProjectorCount
        {
            get
            {
                var count = 0;
                for (var i = 0; i < m_projectors.Count; ++i)
                {
                    if (m_projectors[i].projector.projector.enabled)
                    {
                        ++count;
                    }
                }
                return count;
            }
        }

        public string blobShadowTextureName
        {
            get { return m_shadowTexName; }
        }

        public Rect[] blobShadowTextureRects
        {
            get { return m_blobShadowTextureRects; }
        }

        public Texture2D[] blobShadowTextures
        {
            get { return m_blobShadowTextures; }
        }

        public LayerMask environmentLayers
        {
            get { return m_environmentLayers; }
            set { m_environmentLayers = value; }
        }

        public float fadeDuration
        {
            get { return m_fadeDuration; }
            set { m_fadeDuration = value; }
        }

        public float infinitePlaneHeight
        {
            get { return m_infinitePlaneHeight; }
            set { m_infinitePlaneHeight = value; }
        }

        public Vector3 infinitePlaneNormal
        {
            get { return m_infinitePlaneNormal; }
            set { m_infinitePlaneNormal = value; }
        }

        public Transform infinitPlaneTransform
        {
            get { return m_infinitePlaneTransform; }
            set { m_infinitePlaneTransform = value; }
        }

        // singleton instance
        public static ProjectorManager Instance { get; private set; }

        public Camera mainCamera
        {
            get { return m_mainCamera; }
            set { m_mainCamera = value; }
        }

        public bool manualUpdate
        {
            get { return m_manualUpdate; }
            set { m_manualUpdate = value; }
        }

        public Texture2D packedBlobShadowTexture
        {
            get { return m_packedBlobShadowTexture; }
        }

        public int projectorCount
        {
            get { return m_projectors.Count; }
        }

        public float projectorFadeoutDistance
        {
            get { return m_projectorFadeoutDistance; }
            set { m_projectorFadeoutDistance = value; }
        }

        public LayerMask raycastPlaneMask
        {
            get { return m_raycastPlaneMask; }
            set { m_raycastPlaneMask = value; }
        }

        // public properties
        public LayerMask receiverLayerMask
        {
            get { return m_receiverLayerMask; }
            set
            {
                if (m_receiverLayerMask.value != value.value)
                {
                    m_receiverLayerMask = value;
                    UpdateReceiverLayer();
                }
            }
        }

        public List<ReceiverBase> receivers
        {
            get { return m_receivers; }
        }

        public bool useInfinitPlane
        {
            get { return m_useInfinitePlane; }
            set { m_useInfinitePlane = value; }
        }

        // this function is public for Editor
        public void AddBlobShadowTextureIfNotExist(Texture2D tex)
        {
            var texCount = 0;
            if (m_blobShadowTextures != null)
            {
                texCount = m_blobShadowTextures.Length;
                for (var i = 0; i < texCount; ++i)
                {
                    if (m_blobShadowTextures[i] == tex)
                    {
                        return;
                    }
                }
            }
            var textures = new Texture2D[texCount + 1];
            for (var i = 0; i < texCount; ++i)
            {
                textures[i] = m_blobShadowTextures[i];
            }
            textures[texCount] = tex;
            m_blobShadowTextures = textures;
        }

        // public methods
        public void AddProjector(AutoProjector projector)
        {
            m_projectors.Add(projector);
            projector.uvIndex = 0;
            UpdateUVIndex(projector);
            projector.projector.projector.ignoreLayers |= m_environmentLayers;
        }

        public void AddReceiver(ReceiverBase receiver)
        {
            if (m_receivers.Contains(receiver))
            {
                return;
            }
            receiver.gameObject.SetActive(false);
            m_receivers.Add(receiver);
            for (var i = 0; i < m_receiverInstances.Count; ++i)
            {
                var clone = Instantiate(receiver) as ReceiverBase;
                clone.transform.parent = transform;
                if (0 < m_receiverInstances[i].Count)
                {
                    clone.gameObject.layer = m_receiverInstances[i][0].gameObject.layer;
                    clone.gameObject.SetActive(m_receiverInstances[i][0].gameObject.activeSelf);
                }
                else
                {
                    clone.gameObject.SetActive(false);
                }
                m_receiverInstances[i].Add(clone);
            }
        }

        // private methods
        private void Awake()
        {
            UpdateReceiverLayer();
            if (m_mainCamera == null)
            {
                m_mainCamera = Camera.main;
            }
            m_cameraTransform = m_mainCamera.transform;
            m_cameraClipPlanes = new Plane[4];
            for (var i = 0; i < m_receivers.Count; ++i)
            {
                m_receivers[i].gameObject.SetActive(false);
                m_receivers[i].manualUpdate = true;
            }
            m_renderer = GetComponent<Renderer>();
            if (m_blobShadowTextures != null && 0 < m_blobShadowTextures.Length &&
                (m_blobShadowTextures.Length == 1 || m_packedBlobShadowTexture != null))
            {
                m_meshes = new Mesh[BUFFER_COUNT];
                for (var i = 0; i < BUFFER_COUNT; ++i)
                {
                    m_meshes[i] = new Mesh();
                }
                m_meshFilter = GetComponent<MeshFilter>();
                if (m_meshFilter == null)
                {
                    m_meshFilter = gameObject.AddComponent<MeshFilter>();
                }
                m_nCurrentBuffer = 0;
                if (m_packedBlobShadowTexture != null)
                {
                    m_renderer.material.SetTexture(m_shadowTexName, m_packedBlobShadowTexture);
                }
                else
                {
                    m_renderer.material.SetTexture(m_shadowTexName, m_blobShadowTextures[0]);
                }
            }
            else
            {
                m_meshes = null;
                m_renderer.enabled = false;
            }
            if (m_blobShadowTextureRects == null || m_blobShadowTextureRects.Length == 0 ||
                m_blobShadowTextures.Length == 1)
            {
                m_blobShadowTextureRects = new Rect[1];
                m_blobShadowTextureRects[0] = new Rect(0, 0, 1, 1);
            }
            else if ((m_blobShadowTextureRects.Length != m_blobShadowTextures.Length) &&
                     (Debug.isDebugBuild || Application.isEditor))
            {
                Debug.LogError(
                    "Combined Blob Shadow Texture has not been updated since the array of Blob Shadow Textures was changed.");
            }
            m_infinitePlaneNormal.Normalize();
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }

        private void FreeReceivers(List<ReceiverBase> receivers)
        {
            for (var j = 0; j < m_receivers.Count; ++j)
            {
                receivers[j].gameObject.SetActive(false);
            }
            if (m_freeReceiverInstances != null)
            {
                m_freeReceiverInstances.Push(receivers);
            }
            else
            {
                for (var j = 0; j < m_receivers.Count; ++j)
                {
                    if (receivers[j] != null)
                    {
                        Destroy(receivers[j].gameObject);
                    }
                }
            }
        }

        private List<ReceiverBase> GetFreeReceivers()
        {
            List<ReceiverBase> receivers;
            if (0 < m_freeReceiverInstances.Count)
            {
                receivers = m_freeReceiverInstances.Pop();
                for (var j = 0; j < m_receivers.Count; ++j)
                {
                    receivers[j].gameObject.SetActive(true);
                }
            }
            else
            {
                receivers = new List<ReceiverBase>(m_receivers.Count);
                for (var j = 0; j < m_receivers.Count; ++j)
                {
                    var clone = Instantiate(m_receivers[j]) as ReceiverBase;
                    clone.transform.parent = transform;
                    clone.gameObject.SetActive(true);
                    receivers.Add(clone);
                }
                m_receiverInstances.Add(receivers);
            }
            return receivers;
        }

        private int GetNextReceiverLayer(int layer)
        {
            if (layer < m_firstReceiverLayer)
            {
                return m_firstReceiverLayer;
            }
            while (++layer <= m_lastReceiverLayer)
            {
                if ((m_receiverLayerMask & (1 << layer)) != 0)
                {
                    return layer;
                }
            }
            return m_firstReceiverLayer;
        }

        private void LateUpdate()
        {
            if (!m_manualUpdate)
            {
                UpdateScene();
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void OnDisable()
        {
            for (var i = 0; i < m_projectors.Count; ++i)
            {
                var receivers = m_projectors[i].GetReceivers();
                if (receivers != null)
                {
                    m_projectors[i].ClearReceivers();
                    FreeReceivers(receivers);
                }
            }
            if (m_freeReceiverInstances != null)
            {
                while (0 < m_freeReceiverInstances.Count)
                {
                    var receivers = m_freeReceiverInstances.Pop();
                    for (var i = 0; i < receivers.Count; ++i)
                    {
                        if (receivers[i] != null)
                        {
                            Destroy(receivers[i].gameObject);
                        }
                    }
                }
                m_freeReceiverInstances.Clear();
                m_freeReceiverInstances = null;
            }
            if (m_receiverInstances != null)
            {
                for (var j = 0; j < m_receiverInstances.Count; ++j)
                {
                    var receivers = m_receiverInstances[j];
                    for (var i = 0; i < receivers.Count; ++i)
                    {
                        if (receivers[i] != null)
                        {
                            Destroy(receivers[i].gameObject);
                        }
                    }
                }
                m_receiverInstances.Clear();
            }
        }

        private void OnEnable()
        {
            if (m_freeReceiverInstances == null)
            {
                m_freeReceiverInstances = new Stack<List<ReceiverBase>>();
            }
        }

        private void OnValidate()
        {
            if (m_cameraClipPlanes == null || m_cameraClipPlanes.Length != 4)
            {
                m_cameraClipPlanes = new Plane[4];
            }
        }

        public void PackBlobShadowTextures(Texture2D packedTexture)
        {
            m_blobShadowTextureRects = packedTexture.PackTextures(m_blobShadowTextures, m_packedTexturePadding,
                m_packedTextureMaxSize, false);
            m_packedBlobShadowTexture = packedTexture;
            /*if (0 < m_packedBlobShadowTexturePadding)*/
            {
                // fill padding area with white color
                var w = m_packedBlobShadowTexture.width;
                var h = m_packedBlobShadowTexture.height;
                var bits = new BitArray(w*h, false);
                for (var i = 0; i < m_blobShadowTextureRects.Length; ++i)
                {
                    var rect = m_blobShadowTextureRects[i];
                    var startX = Mathf.RoundToInt(rect.x*w);
                    var startY = Mathf.RoundToInt(rect.y*h);
                    var endX = Mathf.RoundToInt((rect.x + rect.width)*w);
                    var endY = Mathf.RoundToInt((rect.y + rect.height)*h);
                    for (var y = startY; y < endY; ++y)
                    {
                        for (var x = startX; x < endX; ++x)
                        {
                            bits[x + y*w] = true;
                        }
                    }
                }
                var pixels = m_packedBlobShadowTexture.GetPixels32();
                var white = new Color32(255, 255, 255, 255);
                var index = 0;
                for (var y = 0; y < h; ++y)
                {
                    for (var x = 0; x < w; ++x)
                    {
                        if (!bits[index])
                        {
                            pixels[index] = white;
                        }
                        ++index;
                    }
                }
                m_packedBlobShadowTexture.SetPixels32(pixels);
            }
        }

        public void RemoveProjector(AutoProjector projector)
        {
            var receivers = projector.GetReceivers();
            if (receivers != null)
            {
                projector.ClearReceivers();
                FreeReceivers(receivers);
            }
            m_projectors.Remove(projector);
        }

        public void RemoveReceiver(ReceiverBase receiver)
        {
            var index = m_receivers.IndexOf(receiver);
            if (0 <= index)
            {
                m_receivers.RemoveAt(index);
            }
            for (var i = 0; i < m_receiverInstances.Count; ++i)
            {
                Destroy(m_receiverInstances[i][index].gameObject);
                m_receiverInstances[i].RemoveAt(index);
            }
        }

        private void UpdateReceiverLayer()
        {
            if (m_receiverLayerMask == 0)
            {
                for (var i = 0; i < 32; ++i)
                {
                    if (string.IsNullOrEmpty(LayerMask.LayerToName(i)))
                    {
                        m_receiverLayerMask |= 1 << i;
                    }
                }
            }
            m_firstReceiverLayer = -1;
            m_lastReceiverLayer = -1;
            for (var i = 0; i < 32; ++i)
            {
                if ((m_receiverLayerMask & (1 << i)) != 0)
                {
                    if (m_firstReceiverLayer == -1)
                    {
                        m_firstReceiverLayer = i;
                    }
                    m_lastReceiverLayer = i;
                }
            }
        }

        public void UpdateScene()
        {
            // setup camera clip planes
            var cameraZ = m_cameraTransform.forward;
            var cameraPos = m_cameraTransform.position;
            var cameraZPlane = new Plane(cameraZ, cameraPos);
            var cameraX = m_cameraTransform.right;
            var cameraY = m_cameraTransform.up;
            if (m_mainCamera.orthographic)
            {
                var ySize = m_mainCamera.orthographicSize;
                var xSize = m_mainCamera.aspect*ySize;
                m_cameraClipPlanes[0] = new Plane(cameraX, cameraPos);
                m_cameraClipPlanes[0].distance += xSize;
                m_cameraClipPlanes[1] = new Plane(-cameraX, cameraPos);
                m_cameraClipPlanes[1].distance += xSize;
                m_cameraClipPlanes[2] = new Plane(cameraY, cameraPos);
                m_cameraClipPlanes[2].distance += ySize;
                m_cameraClipPlanes[3] = new Plane(-cameraY, cameraPos);
                m_cameraClipPlanes[3].distance += ySize;
            }
            else
            {
                var ySize = Mathf.Tan(0.5f*Mathf.Deg2Rad*m_mainCamera.fieldOfView);
                var xSize = m_mainCamera.aspect*ySize;
                var x0 = (cameraX + xSize*cameraZ).normalized;
                var x1 = (-cameraX + xSize*cameraZ).normalized;
                var y0 = (cameraY + ySize*cameraZ).normalized;
                var y1 = (-cameraY + ySize*cameraZ).normalized;
                m_cameraClipPlanes[0] = new Plane(x0, cameraPos);
                m_cameraClipPlanes[1] = new Plane(y0, cameraPos);
                m_cameraClipPlanes[2] = new Plane(x1, cameraPos);
                m_cameraClipPlanes[3] = new Plane(y1, cameraPos);
            }
            // check visibility of each projector
            var receiverLayer = -1;
            var quadShadowCount = 0;
            var fadeStep = 0.0f < m_fadeDuration ? Time.deltaTime/m_fadeDuration : 1;
            var bAlwaysShowQuadShadows = m_receivers == null || m_receivers.Count == 0;
            for (var i = 0; i < m_projectors.Count; ++i)
            {
                var projector = m_projectors[i];
                projector.projector.InvokeUpdateTransform();
                // find a plane on which the shadow of the projector is projected.
                var foundPlane = false;
                var plane = new Plane();
                if (m_raycastPlaneMask != 0)
                {
                    var origin = projector.projector.position;
                    var dir = projector.projector.direction;
                    RaycastHit hit;
                    if (Physics.Raycast(origin, dir, out hit, projector.projector.farClipPlane, m_raycastPlaneMask))
                    {
                        plane = new Plane(hit.normal, hit.point);
                        foundPlane = true;
                    }
                }
                if (!foundPlane)
                {
                    if (m_useInfinitePlane)
                    {
                        if (m_infinitePlaneTransform != null)
                        {
                            plane =
                                new Plane(
                                    m_infinitePlaneTransform.TransformDirection(m_infinitePlaneNormal).normalized,
                                    m_infinitePlaneTransform.position);
                        }
                        else
                        {
                            plane = new Plane(m_infinitePlaneNormal, Vector3.zero);
                        }
                        plane.distance -= m_infinitePlaneHeight;
                    }
                    else
                    {
                        plane = m_cameraClipPlanes[2];
                        plane.distance += 0.01f;
                    }
                }
                // check visibility and create a quadrangle shadow polygon on the plane.
                var receivers = projector.GetReceivers();
                if (projector.UpdateVisibility(plane, cameraZPlane, m_cameraClipPlanes, m_mainCamera.nearClipPlane,
                    m_mainCamera.farClipPlane))
                {
                    // if projector is far from the main camera, disable projection.
                    if (projector.distanceFromCamera < m_projectorFadeoutDistance)
                    {
                        projector.EnableProjector();
                    }
                    else
                    {
                        projector.DisableProjector();
                    }
                    projector.UpdateWeights(fadeStep, bAlwaysShowQuadShadows);
                    if (projector.isProjectorActive && 0 < m_receivers.Count)
                    {
                        // if projection is enabled, assign a set of shadow receivers.
                        if (receivers == null)
                        {
                            receivers = GetFreeReceivers();
                            projector.SetReceivers(receivers);
                        }
                        receiverLayer = GetNextReceiverLayer(receiverLayer);
                        projector.SetReceiverLayer(receiverLayer, m_receiverLayerMask);
                        for (var j = 0; j < receivers.Count; ++j)
                        {
                            receivers[j].UpdateReceiver();
                        }
                    }
                    else if (receivers != null)
                    {
                        projector.ClearReceivers();
                        FreeReceivers(receivers);
                    }
                    if (0.0f < projector.quadShadowWeight)
                    {
                        ++quadShadowCount;
                    }
                }
                else if (receivers != null)
                {
                    projector.ClearReceivers();
                    FreeReceivers(receivers);
                }
            }
            if (m_meshes != null)
            {
                // update quad shadows mesh
                var numVertices = 4*quadShadowCount;
                var index = 0;
                var indexCount = 0;
                var color = new Color32(255, 255, 255, 255);
                if (0 < quadShadowCount)
                {
                    if (m_vertices == null || m_vertices.Length < numVertices)
                    {
                        m_vertices = new Vector3[numVertices];
                        m_colors = new Color32[numVertices];
                        m_uvs = new Vector2[numVertices];
                        m_triangles = new int[6*quadShadowCount];
                    }
                    for (var i = 0; i < m_projectors.Count; ++i)
                    {
                        var projector = m_projectors[i];
                        if (projector.isVisible && 0.0f < projector.quadShadowWeight)
                        {
                            m_triangles[indexCount++] = index;
                            m_triangles[indexCount++] = index + 1;
                            m_triangles[indexCount++] = index + 2;
                            m_triangles[indexCount++] = index + 2;
                            m_triangles[indexCount++] = index + 1;
                            m_triangles[indexCount++] = index + 3;
                            projector.GetQuadShadowVertices(m_vertices, index);
                            color.a = (byte) Mathf.FloorToInt(255*projector.quadShadowAlpha);
                            var rect = m_blobShadowTextureRects[projector.uvIndex];
                            m_colors[index] = color;
                            m_uvs[index] = new Vector2(rect.x, rect.y);
                            index++;
                            m_colors[index] = color;
                            m_uvs[index] = new Vector2(rect.x, rect.y + rect.height);
                            index++;
                            m_colors[index] = color;
                            m_uvs[index] = new Vector2(rect.x + rect.width, rect.y);
                            index++;
                            m_colors[index] = color;
                            m_uvs[index] = new Vector2(rect.x + rect.width, rect.y + rect.height);
                            index++;
                        }
                    }
                    while (indexCount < m_triangles.Length)
                    {
                        m_triangles[indexCount++] = 0;
                    }
                    m_nCurrentBuffer = (m_nCurrentBuffer + 1)%BUFFER_COUNT;
                    var currentMesh = m_meshes[m_nCurrentBuffer];
                    currentMesh.Clear();
                    currentMesh.vertices = m_vertices;
                    currentMesh.colors32 = m_colors;
                    currentMesh.uv = m_uvs;
                    currentMesh.triangles = m_triangles;
                    m_meshFilter.mesh = currentMesh;
                    m_renderer.enabled = true;
                }
                else
                {
                    m_renderer.enabled = false;
                }
            }
        }

        public void UpdateUVIndex(AutoProjector projector)
        {
            if (m_blobShadowTextures != null && 1 < m_blobShadowTextures.Length &&
                projector.projector.projector.material.HasProperty(m_shadowTexName))
            {
                var tex = projector.projector.projector.material.GetTexture(m_shadowTexName) as Texture2D;
                for (var i = 0; i < m_blobShadowTextures.Length; ++i)
                {
                    if (tex == m_blobShadowTextures[i])
                    {
                        projector.uvIndex = i;
                        break;
                    }
                }
            }
        }
    }
}