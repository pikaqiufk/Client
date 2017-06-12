#region using

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace FastShadowReceiver
{
    public class TerrainMeshTree : MeshTreeBase
    {
        [SerializeField] [HideInInspector] protected int[] m_childNodes;
        private float[,] m_heightMap;
        private int m_heightMapWidth;
        private Vector3 m_scale;
        [SerializeField] [HideInInspector] private TerrainData m_terrainData;
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3)
        [SerializeField] [HideInInspector] // from Unity 4.5, struct can be serialized!
#endif
            private TreeNode[] m_treeNodes;

        [SerializeField] [HideInInspector] private float[] m_yCenters;
        [SerializeField] [HideInInspector] private float[] m_yExtents;

        public TerrainData terrainData
        {
            get { return m_terrainData; }
            set
            {
                if (m_terrainData != value)
                {
                    Clear();
                    m_terrainData = value;
                }
            }
        }

        protected override void Build()
        {
            for (var z = 0; z <= m_heightMapWidth; ++z)
            {
                for (var x = 0; x <= m_heightMapWidth; ++x)
                {
                    m_heightMap[z, x] *= m_scale.y;
                }
            }
            var nodeList = new List<TreeNode>();
            TreeNode rootNode;
            rootNode.m_yCenter = 0.0f;
            rootNode.m_yExtent = 0.0f;
            rootNode.m_childIndex = -1;
            BuildMeshTree(nodeList, ref rootNode, 0, 0, m_heightMapWidth);
            m_bounds.center = new Vector3(0.5f*m_heightMapWidth*m_scale.x, rootNode.m_yCenter,
                0.5f*m_heightMapWidth*m_scale.z);
            m_bounds.extents = new Vector3(m_bounds.center.x, rootNode.m_yExtent, m_bounds.center.z);
            nodeList.Add(rootNode);
#if UNITY_EDITOR && (UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3)
			int numNodes = nodeList.Count;
			m_childNodes = new int[numNodes];
			m_yCenters = new float[numNodes];
			m_yExtents = new float[numNodes];
			for (int i = 0; i < numNodes; ++i) {
				m_childNodes[i] = nodeList[i].m_childIndex;
				m_yCenters[i] = nodeList[i].m_yCenter;
				m_yExtents[i] = nodeList[i].m_yExtent;
			}
#endif
            m_treeNodes = nodeList.ToArray();
        }

        public override void BuildFromPrebuiltData()
        {
            m_heightMapWidth = m_terrainData.heightmapWidth - 1;
            m_heightMap = m_terrainData.GetHeights(0, 0, m_terrainData.heightmapWidth, m_terrainData.heightmapWidth);
            m_scale = m_terrainData.heightmapScale;
            for (var z = 0; z <= m_heightMapWidth; ++z)
            {
                for (var x = 0; x <= m_heightMapWidth; ++x)
                {
                    m_heightMap[z, x] *= m_scale.y;
                }
            }
            if (m_treeNodes == null || m_treeNodes.Length == 0)
            {
                var numNodes = m_childNodes.Length;
                var treeNodes = new TreeNode[numNodes];
                for (var i = 0; i < numNodes; ++i)
                {
                    treeNodes[i].m_yCenter = m_yCenters[i];
                    treeNodes[i].m_yExtent = m_yExtents[i];
                    treeNodes[i].m_childIndex = m_childNodes[i];
                }
                var rootNode = treeNodes[numNodes - 1];
                m_bounds.center = new Vector3(0.5f*m_heightMapWidth*m_scale.x, rootNode.m_yCenter,
                    0.5f*m_heightMapWidth*m_scale.z);
                m_bounds.extents = new Vector3(m_bounds.center.x, rootNode.m_yExtent, m_bounds.center.z);
                m_treeNodes = treeNodes;
            }
#if !(UNITY_EDITOR || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3)
    // release memory
			m_yCenters = null;
			m_yExtents = null;
			m_childNodes = null;
#endif
        }

        private void BuildMeshTree(List<TreeNode> nodeList, ref TreeNode parentNode, int posX, int posZ, int width)
        {
            if (width <= 2)
            {
                UpdateNodeBounds(ref parentNode, posX, posZ, width);
                parentNode.m_childIndex = -1;
#if UNITY_EDITOR
                m_leafNodesCellCount += width*width;
#endif
                return;
            }
            width /= 2;
            var child0 = new TreeNode();
            BuildMeshTree(nodeList, ref child0, posX, posZ, width);
            var child1 = new TreeNode();
            BuildMeshTree(nodeList, ref child1, posX + width, posZ, width);
            var child2 = new TreeNode();
            BuildMeshTree(nodeList, ref child2, posX, posZ + width, width);
            var child3 = new TreeNode();
            BuildMeshTree(nodeList, ref child3, posX + width, posZ + width, width);
            var minY = child0.m_yCenter - child0.m_yExtent;
            var maxY = child0.m_yCenter + child0.m_yExtent;
            minY = Mathf.Min(minY, child1.m_yCenter - child1.m_yExtent);
            maxY = Mathf.Max(maxY, child1.m_yCenter + child1.m_yExtent);
            minY = Mathf.Min(minY, child2.m_yCenter - child2.m_yExtent);
            maxY = Mathf.Max(maxY, child2.m_yCenter + child2.m_yExtent);
            minY = Mathf.Min(minY, child3.m_yCenter - child3.m_yExtent);
            maxY = Mathf.Max(maxY, child3.m_yCenter + child3.m_yExtent);
            parentNode.m_yCenter = 0.5f*(minY + maxY);
            parentNode.m_yExtent = 0.5f*(maxY - minY);
            parentNode.m_childIndex = nodeList.Count;
            nodeList.Add(child0);
            nodeList.Add(child1);
            nodeList.Add(child2);
            nodeList.Add(child3);
        }

        public override string CheckError(GameObject rootObject)
        {
            var terrain = rootObject.GetComponent<Terrain>();
            if (terrain == null)
            {
                return rootObject.name + " is not a Terrain object.";
            }
            if (terrain.terrainData != m_terrainData)
            {
                return "This mesh tree was not built from " + rootObject.name + ".";
            }
            var nodeList = new List<TreeNode>();
            TreeNode rootNode;
            rootNode.m_yCenter = 0.0f;
            rootNode.m_yExtent = 0.0f;
            rootNode.m_childIndex = -1;
            BuildMeshTree(nodeList, ref rootNode, 0, 0, m_heightMapWidth);
            nodeList.Add(rootNode);
            var errorString = "Terrain data has been modified since the mesh tree was built.";
            if (nodeList.Count != m_treeNodes.Length)
            {
                return errorString;
            }
            var epsilon = 0.001f*m_bounds.extents.y;
            for (var i = 0; i < m_treeNodes.Length; ++i)
            {
                if (epsilon < Mathf.Abs(m_treeNodes[i].m_yCenter - nodeList[i].m_yCenter))
                {
                    return errorString;
                }
                if (epsilon < Mathf.Abs(m_treeNodes[i].m_yExtent - nodeList[i].m_yExtent))
                {
                    return errorString;
                }
            }
            return null;
        }

        protected virtual void Clear()
        {
            WaitForBuild();
            m_yCenters = null;
            m_yExtents = null;
            m_childNodes = null;
            m_treeNodes = null;
        }

        public override MeshTreeSearch CreateSearch()
        {
            return new TerrainMeshTreeSearch();
        }

        public override Type GetSearchType()
        {
            return typeof (TerrainMeshTreeSearch);
        }

        public override bool IsBuildFinished()
        {
            return m_heightMap != null && m_treeNodes != null && 0 < m_treeNodes.Length;
        }

        public override bool IsPrebuilt()
        {
#if (UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3)
			return m_childNodes != null && 0 < m_childNodes.Length;
#else
            return (m_treeNodes != null && 0 < m_treeNodes.Length) || (m_childNodes != null && 0 < m_childNodes.Length);
#endif
        }

        public override bool IsReadyToBuild()
        {
            return m_terrainData != null;
        }

        protected override void PrepareForBuild()
        {
            m_heightMapWidth = m_terrainData.heightmapWidth - 1;
            m_heightMap = m_terrainData.GetHeights(0, 0, m_terrainData.heightmapWidth, m_terrainData.heightmapWidth);
            m_scale = m_terrainData.heightmapScale;
            m_treeNodes = null;
#if UNITY_EDITOR
            m_totalCellCount = m_heightMapWidth*m_heightMapWidth;
            m_leafNodesCellCount = 0;
#endif
        }

        public override void Raycast(MeshTreeRaycast raycast)
        {
            var param = raycast.CreateTemporaryParam();
            var order = 0x3210u;
            if (raycast.direction.x < 0.0f)
            {
                order ^= 0x1111u;
            }
            if (raycast.direction.z < 0.0f)
            {
                order ^= 0x2222u;
            }
            Raycast(raycast, ref m_treeNodes[m_treeNodes.Length - 1], m_bounds.extents, 0, 0, m_heightMapWidth,
                ref param, order);
        }

        private bool Raycast(MeshTreeRaycast raycast,
                             ref TreeNode node,
                             Vector3 extents,
                             int posX,
                             int posZ,
                             int width,
                             ref MeshTreeRaycast.TemporaryParam param,
                             uint order)
        {
            extents.y = node.m_yExtent;
            var halfWidth = 0.5f*width;
            float distance;
            if (
                !raycast.BoundsHitTest(
                    new Vector3(m_scale.x*(posX + halfWidth), node.m_yCenter, m_scale.z*(posZ + halfWidth)), extents,
                    param, out distance))
            {
                return false;
            }
            if (node.m_childIndex < 0)
            {
                var hit = false;
                for (int z = posZ, zEnd = posZ + width; z < zEnd; ++z)
                {
                    for (int x = posX, xEnd = posX + width; x < xEnd; ++x)
                    {
                        var v0 = new Vector3(m_scale.x*x, m_heightMap[z, x], m_scale.z*z);
                        var v1 = new Vector3(m_scale.x*(x + 1), m_heightMap[z, x + 1], m_scale.z*z);
                        var v2 = new Vector3(m_scale.x*x, m_heightMap[z + 1, x], m_scale.z*(z + 1));
                        var v3 = new Vector3(m_scale.x*(x + 1), m_heightMap[z + 1, x + 1], m_scale.z*(z + 1));
                        if (raycast.TriangleHitTest(v0, v2, v3))
                        {
                            hit = true;
                        }
                        if (raycast.TriangleHitTest(v0, v3, v1))
                        {
                            hit = true;
                        }
                    }
                }
                return hit;
            }
            width /= 2;
            extents.x *= 0.5f;
            extents.z *= 0.5f;
            for (var i = 0; i < 4; ++i)
            {
                var n = (order >> (i*4)) & 0xF;
                var x = posX;
                var z = posZ;
                if ((n & 1) != 0)
                {
                    x += width;
                }
                if ((n & 2) != 0)
                {
                    z += width;
                }
                if (Raycast(raycast, ref m_treeNodes[node.m_childIndex + n], extents, x, z, width, ref param, order))
                {
                    return true;
                }
            }
            return false;
        }

        public override void Search(MeshTreeSearch search)
        {
            if (!(search is TerrainMeshTreeSearch))
            {
                if (Debug.isDebugBuild || Application.isEditor)
                {
                    Debug.LogError("Invalid MeshTreeSearch class!");
                }
                return;
            }
            var terrainSearch = (TerrainMeshTreeSearch) search;
            terrainSearch.Initialize();
            Search(terrainSearch, ref m_treeNodes[m_treeNodes.Length - 1], m_bounds.extents, 0, 0, m_heightMapWidth);
            if (search.m_bOutputNormals)
            {
                terrainSearch.FinalizeWithNormal(m_heightMap, m_scale.x, m_scale.z);
            }
            else
            {
                terrainSearch.Finalize(m_heightMap, m_scale.x, m_scale.z);
            }
        }

        private void Search(TerrainMeshTreeSearch search,
                            ref TreeNode node,
                            Vector3 extents,
                            int posX,
                            int posZ,
                            int width)
        {
            bool isPartial;
            extents.y = node.m_yExtent;
            var halfWidth = 0.5f*width;
            if (0 <= node.m_childIndex)
            {
                var flags =
                    search.IsInView(
                        new Vector3(m_scale.x*(posX + halfWidth), node.m_yCenter, m_scale.z*(posZ + halfWidth)), extents,
                        out isPartial);
                if (flags == 0)
                {
                    return;
                }
                if (isPartial)
                {
                    extents.x *= 0.5f;
                    extents.z *= 0.5f;
                    width /= 2;
                    if ((flags & 1) != 0)
                    {
                        Search(search, ref m_treeNodes[node.m_childIndex], extents, posX, posZ, width);
                    }
                    if ((flags & 2) != 0)
                    {
                        Search(search, ref m_treeNodes[node.m_childIndex + 1], extents, posX + width, posZ, width);
                    }
                    if ((flags & 4) != 0)
                    {
                        Search(search, ref m_treeNodes[node.m_childIndex + 2], extents, posX, posZ + width, width);
                    }
                    if ((flags & 8) != 0)
                    {
                        Search(search, ref m_treeNodes[node.m_childIndex + 3], extents, posX + width, posZ + width,
                            width);
                    }
                }
                else
                {
                    search.AddPatch(posX, posZ, width, node.m_yCenter - node.m_yExtent, node.m_yCenter + node.m_yExtent,
                        false);
                }
            }
            else
            {
                var flags =
                    search.IsInView(
                        new Vector3(m_scale.x*(posX + halfWidth), node.m_yCenter, m_scale.z*(posZ + halfWidth)), extents);
                if (flags == 0)
                {
                }
                else if (flags != 0xf000f)
                {
                    width /= 2;
                    var minY = node.m_yCenter - node.m_yExtent;
                    var maxY = node.m_yCenter + node.m_yExtent;
                    if ((flags & 1) != 0)
                    {
                        search.AddPatch(posX, posZ, width, minY, maxY, (flags & (1 << 12)) == 0);
                    }
                    if ((flags & 2) != 0)
                    {
                        search.AddPatch(posX + width, posZ, width, minY, maxY, (flags & (2 << 12)) == 0);
                    }
                    if ((flags & 4) != 0)
                    {
                        search.AddPatch(posX, posZ + width, width, minY, maxY, (flags & (4 << 12)) == 0);
                    }
                    if ((flags & 8) != 0)
                    {
                        search.AddPatch(posX + width, posZ + width, width, minY, maxY, (flags & (8 << 12)) == 0);
                    }
                }
                else
                {
                    search.AddPatch(posX, posZ, width, node.m_yCenter - node.m_yExtent, node.m_yCenter + node.m_yExtent,
                        false);
                }
            }
        }

        private void UpdateNodeBounds(ref TreeNode node, int posX, int posZ, int width)
        {
            var minY = m_heightMap[posZ, posX];
            var maxY = minY;
            for (int x = posX, endX = posX + width; x <= endX; ++x)
            {
                for (int z = posZ, endZ = posZ + width; z <= endZ; ++z)
                {
                    var h = m_heightMap[z, x];
                    maxY = Mathf.Max(maxY, h);
                    minY = Mathf.Min(minY, h);
                }
            }
            node.m_yCenter = 0.5f*(minY + maxY);
            node.m_yExtent = 0.5f*(maxY - minY);
        }

        [Serializable]
        private struct TreeNode
        {
            public int m_childIndex;
            public float m_yCenter;
            public float m_yExtent;
        }

#if UNITY_EDITOR
        public override int GetMemoryUsage()
        {
            var size = 0;
            if (m_terrainData != null)
            {
                var w = m_terrainData.heightmapWidth;
                size += w*w*sizeof (float);
            }
            if (m_yExtents != null)
            {
                size += m_yExtents.Length*sizeof (float);
            }
            if (m_yCenters != null)
            {
                size += m_yCenters.Length*sizeof (float);
            }
            if (m_childNodes != null)
            {
                size += m_childNodes.Length*sizeof (int);
            }
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3)
            if (m_treeNodes != null && m_treeNodes.Length == 0 && (m_childNodes == null || m_childNodes.Length == 0))
            {
                size += m_treeNodes.Length*(2*sizeof (float) + sizeof (int));
            }
#endif
            return size;
        }

        public override int GetNodeCount()
        {
            if (m_treeNodes != null && m_treeNodes.Length != 0)
            {
                return m_treeNodes.Length;
            }
            return m_childNodes == null ? 0 : m_childNodes.Length;
        }

        private int m_totalCellCount;
        private int m_leafNodesCellCount;

        public override float GetBuildProgress()
        {
            if (m_treeNodes != null)
            {
                return 1.0f;
            }
            if (m_totalCellCount == 0)
            {
                return 0;
            }
            return m_leafNodesCellCount/(float) m_totalCellCount;
        }
#endif
    }
}