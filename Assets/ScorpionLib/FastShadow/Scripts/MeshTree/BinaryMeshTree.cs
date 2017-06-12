﻿#region using

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace FastShadowReceiver
{
    public class BinaryMeshTree : MeshTree
    {
        private const int LEAFNODE_TRIANGLE_COUNT = 2;
        [SerializeField] [HideInInspector] private Vector3[] m_nodeBoundsCenter;
        [SerializeField] [HideInInspector] private Vector3[] m_nodeBoundsExtent;
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3)
        [SerializeField] [HideInInspector] // from Unity 4.5, struct can be serialized!
#endif
            private TreeNode[] m_treeNodes;

        private void AddAllTrianglesInChildren(BinaryMeshTreeSearch search, ref TreeNode node)
        {
            if (node.m_triangles != null && 0 < node.m_triangles.Length)
            {
                search.AddTriangles(node.m_triangles, node.m_bounds, false);
            }
            else if (0 <= node.m_childIndex)
            {
                AddAllTrianglesInChildren(search, ref m_treeNodes[node.m_childIndex]);
                AddAllTrianglesInChildren(search, ref m_treeNodes[node.m_childIndex + 1]);
            }
        }

        protected override void Build()
        {
            var numTriangles = m_indices.Length/3;
            var triangleBounds = new Bounds[numTriangles];
            var index = 0;
            var nodeList = new List<TreeNode>();
            TreeNode rootNode;
            rootNode.m_bounds = m_bounds;
            rootNode.m_childIndex = -1;
            rootNode.m_triangles = null;
            var triangleList = new List<int>();
            for (var i = 0; i < numTriangles; ++i)
            {
                triangleList.Add(index);
                var v0 = m_vertices[m_indices[index++]];
                var v1 = m_vertices[m_indices[index++]];
                var v2 = m_vertices[m_indices[index++]];
                Vector3 min, max;
                min.x = Mathf.Min(Mathf.Min(v0.x, v1.x), v2.x);
                min.y = Mathf.Min(Mathf.Min(v0.y, v1.y), v2.y);
                min.z = Mathf.Min(Mathf.Min(v0.z, v1.z), v2.z);
                max.x = Mathf.Max(Mathf.Max(v0.x, v1.x), v2.x);
                max.y = Mathf.Max(Mathf.Max(v0.y, v1.y), v2.y);
                max.z = Mathf.Max(Mathf.Max(v0.z, v1.z), v2.z);
                triangleBounds[i].SetMinMax(min, max);
            }
            BuildMeshTree(nodeList, ref rootNode, triangleList, triangleBounds);
            nodeList.Add(rootNode);
#if UNITY_EDITOR && (UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3)
			int numNodes = nodeList.Count;
			int[] childNodes = new int[numNodes];
			TriangleList[] triangleLists = new TriangleList[numNodes];
			Vector3[] nodeBoundsCenter = new Vector3[numNodes];
			Vector3[] nodeBoundsExtent = new Vector3[numNodes];
			for (int i = 0; i < numNodes; ++i) {
				TreeNode node = nodeList[i];
				nodeBoundsCenter[i] = node.m_bounds.center;
				nodeBoundsExtent[i] = node.m_bounds.extents;
				childNodes[i] = node.m_childIndex;
				if (node.m_triangles != null) {
					triangleLists[i] = new TriangleList();
					triangleLists[i].triangles = node.m_triangles;
				}
				else {
					triangleLists[i] = null;
				}
			}
			m_childNodes = childNodes;
			m_triangleLists = triangleLists;
			m_nodeBoundsCenter = nodeBoundsCenter;
			m_nodeBoundsExtent = nodeBoundsExtent;
#endif
            m_treeNodes = nodeList.ToArray();
        }

        public override void BuildFromPrebuiltData()
        {
            if (m_treeNodes == null || m_treeNodes.Length == 0)
            {
                var numNodes = m_childNodes.Length;
                var treeNodes = new TreeNode[numNodes];
                for (var i = 0; i < numNodes; ++i)
                {
                    treeNodes[i].m_bounds.center = m_nodeBoundsCenter[i];
                    treeNodes[i].m_bounds.extents = m_nodeBoundsExtent[i];
                    treeNodes[i].m_childIndex = m_childNodes[i];
                    if (m_triangleLists[i] != null && m_triangleLists[i].triangles != null &&
                        0 < m_triangleLists[i].triangles.Length)
                    {
                        treeNodes[i].m_triangles = m_triangleLists[i].triangles;
                    }
                    else
                    {
                        treeNodes[i].m_triangles = null;
                    }
                }
                m_treeNodes = treeNodes;
            }
#if !(UNITY_EDITOR || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3)
    // release memory
			m_nodeBoundsCenter = null;
			m_nodeBoundsExtent = null;
			m_childNodes = null;
			m_triangleLists = null;
#endif
        }

        private void BuildMeshTree(List<TreeNode> nodeList,
                                   ref TreeNode parentNode,
                                   List<int> triangleList,
                                   Bounds[] triangleBounds)
        {
            var numTriangles = triangleList.Count;
            if (numTriangles <= 16)
            {
                // to save memory usage, TreeNode hold triangles only when triangle count <= 16.
                parentNode.m_triangles = triangleList.ToArray();
            }
            if (numTriangles <= LEAFNODE_TRIANGLE_COUNT)
            {
#if UNITY_EDITOR
                m_triangleCountInLeafNodes += parentNode.m_triangles.Length;
#endif
                parentNode.m_childIndex = -1;
                return;
            }
            var extents = parentNode.m_bounds.extents;
            var center = parentNode.m_bounds.center;
            var list1 = new List<int>();
            var list2 = new List<int>();
            if (extents.y <= extents.x && extents.z <= extents.x)
            {
                DivideTrianglesByXAxis(triangleList, list1, list2, center, triangleBounds);
                if (list1.Count == 0 || list2.Count == 0)
                {
                    list1.Clear();
                    list2.Clear();
                    if (extents.z <= extents.y)
                    {
                        DivideTrianglesByYAxis(triangleList, list1, list2, center, triangleBounds);
                        if (list1.Count == 0 || list2.Count == 0)
                        {
                            list1.Clear();
                            list2.Clear();
                            DivideTrianglesByZAxis(triangleList, list1, list2, center, triangleBounds);
                        }
                    }
                    else
                    {
                        DivideTrianglesByZAxis(triangleList, list1, list2, center, triangleBounds);
                        if (list1.Count == 0 || list2.Count == 0)
                        {
                            list1.Clear();
                            list2.Clear();
                            DivideTrianglesByYAxis(triangleList, list1, list2, center, triangleBounds);
                        }
                    }
                }
            }
            else if (extents.x <= extents.y && extents.z <= extents.y)
            {
                DivideTrianglesByYAxis(triangleList, list1, list2, center, triangleBounds);
                if (list1.Count == 0 || list2.Count == 0)
                {
                    list1.Clear();
                    list2.Clear();
                    if (extents.z <= extents.x)
                    {
                        DivideTrianglesByXAxis(triangleList, list1, list2, center, triangleBounds);
                        if (list1.Count == 0 || list2.Count == 0)
                        {
                            list1.Clear();
                            list2.Clear();
                            DivideTrianglesByZAxis(triangleList, list1, list2, center, triangleBounds);
                        }
                    }
                    else
                    {
                        DivideTrianglesByZAxis(triangleList, list1, list2, center, triangleBounds);
                        if (list1.Count == 0 || list2.Count == 0)
                        {
                            list1.Clear();
                            list2.Clear();
                            DivideTrianglesByXAxis(triangleList, list1, list2, center, triangleBounds);
                        }
                    }
                }
            }
            else
            {
                DivideTrianglesByZAxis(triangleList, list1, list2, center, triangleBounds);
                if (list1.Count == 0 || list2.Count == 0)
                {
                    list1.Clear();
                    list2.Clear();
                    if (extents.y <= extents.x)
                    {
                        DivideTrianglesByXAxis(triangleList, list1, list2, center, triangleBounds);
                        if (list1.Count == 0 || list2.Count == 0)
                        {
                            list1.Clear();
                            list2.Clear();
                            DivideTrianglesByYAxis(triangleList, list1, list2, center, triangleBounds);
                        }
                    }
                    else
                    {
                        DivideTrianglesByYAxis(triangleList, list1, list2, center, triangleBounds);
                        if (list1.Count == 0 || list2.Count == 0)
                        {
                            list1.Clear();
                            list2.Clear();
                            DivideTrianglesByXAxis(triangleList, list1, list2, center, triangleBounds);
                        }
                    }
                }
            }
            if (list1.Count == 0 || list2.Count == 0)
            {
                if (parentNode.m_triangles == null)
                {
                    parentNode.m_triangles = triangleList.ToArray();
                }
#if UNITY_EDITOR
                m_triangleCountInLeafNodes += parentNode.m_triangles.Length;
#endif
                parentNode.m_childIndex = -1;
                return;
            }
            triangleList.Clear();
            triangleList.Capacity = 0;
            var child1 = new TreeNode();
            var child2 = new TreeNode();
            child1.m_triangles = null;
            child2.m_triangles = null;
            CalculateBounds(ref child1, list1, triangleBounds);
            CalculateBounds(ref child2, list2, triangleBounds);
            BuildMeshTree(nodeList, ref child1, list1, triangleBounds);
            BuildMeshTree(nodeList, ref child2, list2, triangleBounds);
            parentNode.m_childIndex = nodeList.Count;
            nodeList.Add(child1);
            nodeList.Add(child2);
        }

        private void CalculateBounds(ref TreeNode node, List<int> triangleList, Bounds[] triangleBounds)
        {
            if (triangleList.Count == 0)
            {
                node.m_bounds = new Bounds(Vector3.zero, Vector3.zero);
                return;
            }
            var first = triangleList[0]/3;
            var min = triangleBounds[first].min;
            var max = triangleBounds[first].max;
            for (var i = 1; i < triangleList.Count; ++i)
            {
                var index = triangleList[i]/3;
                var m = triangleBounds[index].min;
                var M = triangleBounds[index].max;
                min.x = Mathf.Min(min.x, m.x);
                min.y = Mathf.Min(min.y, m.y);
                min.z = Mathf.Min(min.z, m.z);
                max.x = Mathf.Max(max.x, M.x);
                max.y = Mathf.Max(max.y, M.y);
                max.z = Mathf.Max(max.z, M.z);
            }
            node.m_bounds.SetMinMax(min, max);
        }

        protected override void Clear()
        {
            base.Clear();
            m_nodeBoundsCenter = null;
            m_nodeBoundsExtent = null;
            m_treeNodes = null;
        }

        public override MeshTreeSearch CreateSearch()
        {
            return new BinaryMeshTreeSearch();
        }

        private static void DivideTrianglesByXAxis(List<int> triangleList,
                                                   List<int> list1,
                                                   List<int> list2,
                                                   Vector3 center,
                                                   Bounds[] triangleBounds)
        {
            var numTriangles = triangleList.Count;
            for (var i = 0; i < numTriangles; ++i)
            {
                var tri = triangleList[i];
                if (triangleBounds[tri/3].center.x < center.x)
                {
                    list1.Add(tri);
                }
                else
                {
                    list2.Add(tri);
                }
            }
        }

        private static void DivideTrianglesByYAxis(List<int> triangleList,
                                                   List<int> list1,
                                                   List<int> list2,
                                                   Vector3 center,
                                                   Bounds[] triangleBounds)
        {
            var numTriangles = triangleList.Count;
            for (var i = 0; i < numTriangles; ++i)
            {
                var tri = triangleList[i];
                if (triangleBounds[tri/3].center.y < center.y)
                {
                    list1.Add(tri);
                }
                else
                {
                    list2.Add(tri);
                }
            }
        }

        private static void DivideTrianglesByZAxis(List<int> triangleList,
                                                   List<int> list1,
                                                   List<int> list2,
                                                   Vector3 center,
                                                   Bounds[] triangleBounds)
        {
            var numTriangles = triangleList.Count;
            for (var i = 0; i < numTriangles; ++i)
            {
                var tri = triangleList[i];
                if (triangleBounds[tri/3].center.z < center.z)
                {
                    list1.Add(tri);
                }
                else
                {
                    list2.Add(tri);
                }
            }
        }

        public override Type GetSearchType()
        {
            return typeof (BinaryMeshTreeSearch);
        }

        public override bool IsBuildFinished()
        {
            return m_treeNodes != null && 0 < m_treeNodes.Length;
        }

        protected override void PrepareForBuild()
        {
            base.PrepareForBuild();
            m_treeNodes = null;
#if UNITY_EDITOR
            m_totalTriangleCount = m_indices.Length/3;
            m_triangleCountInLeafNodes = 0;
#endif
        }

        public override void Raycast(MeshTreeRaycast raycast)
        {
            var param = raycast.CreateTemporaryParam();
            Raycast(raycast, ref m_treeNodes[m_treeNodes.Length - 1], ref param);
        }

        private bool Raycast(MeshTreeRaycast raycast, ref TreeNode node, ref MeshTreeRaycast.TemporaryParam param)
        {
            if (node.m_childIndex < 0)
            {
                if (node.m_triangles == null || node.m_triangles.Length == 0)
                {
                    return false;
                }
                var hit = false;
                for (var i = 0; i < node.m_triangles.Length; ++i)
                {
                    var tri = node.m_triangles[i];
                    if (raycast.TriangleHitTest(m_vertices[m_indices[tri]], m_vertices[m_indices[tri + 1]],
                        m_vertices[m_indices[tri + 2]]))
                    {
                        hit = true;
                    }
                }
                return hit;
            }
            float distance1, distance2;
            var hit1 = raycast.BoundsHitTest(m_treeNodes[node.m_childIndex].m_bounds.center,
                m_treeNodes[node.m_childIndex].m_bounds.extents, param, out distance1);
            var hit2 = raycast.BoundsHitTest(m_treeNodes[node.m_childIndex + 1].m_bounds.center,
                m_treeNodes[node.m_childIndex + 1].m_bounds.extents, param, out distance2);
            if (hit1)
            {
                if (hit2)
                {
                    if (distance1 < distance2)
                    {
                        if (Raycast(raycast, ref m_treeNodes[node.m_childIndex], ref param))
                        {
                            // there is a chance that the other node has the nearest hit point, since bounding boxes of nodes in a binary mesh trees are overlapping each other.
                            if (distance2 < raycast.hitDistance)
                            {
                                Raycast(raycast, ref m_treeNodes[node.m_childIndex + 1], ref param);
                            }
                            return true;
                        }
                        return Raycast(raycast, ref m_treeNodes[node.m_childIndex + 1], ref param);
                    }
                    if (Raycast(raycast, ref m_treeNodes[node.m_childIndex + 1], ref param))
                    {
                        // there is a chance that the other node has the nearest hit point, since bounding boxes of nodes in a binary mesh trees are overlapping each other.
                        if (distance1 < raycast.hitDistance)
                        {
                            Raycast(raycast, ref m_treeNodes[node.m_childIndex], ref param);
                        }
                        return true;
                    }
                    return Raycast(raycast, ref m_treeNodes[node.m_childIndex], ref param);
                }
                return Raycast(raycast, ref m_treeNodes[node.m_childIndex], ref param);
            }
            if (hit2)
            {
                return Raycast(raycast, ref m_treeNodes[node.m_childIndex + 1], ref param);
            }
            return false;
        }

        public override void Search(MeshTreeSearch search)
        {
            if (!(search is BinaryMeshTreeSearch))
            {
                Debug.LogError("Invalid MeshTreeSearch class!");
                return;
            }
            var binSearch = (BinaryMeshTreeSearch) search;
            binSearch.Initialize();
            Search(binSearch, ref m_treeNodes[m_treeNodes.Length - 1]);
            if (search.m_bOutputNormals && m_normals != null && 0 < m_normals.Length)
            {
                binSearch.Finalize(m_vertices, m_indices, m_normals);
            }
            else
            {
                binSearch.Finalize(m_vertices, m_indices);
            }
        }

        private void Search(BinaryMeshTreeSearch search, ref TreeNode node)
        {
            var noTriangles = (node.m_triangles == null || node.m_triangles.Length == 0);
            if (node.m_childIndex < 0 && noTriangles)
            {
                return;
            }
            float scissorDistance;
            if (!search.IsInView(node.m_bounds, out scissorDistance))
            {
                return;
            }
            if (scissorDistance < 0 && 0 <= node.m_childIndex)
            {
                Search(search, ref m_treeNodes[node.m_childIndex]);
                Search(search, ref m_treeNodes[node.m_childIndex + 1]);
            }
            else
            {
                if (noTriangles)
                {
                    AddAllTrianglesInChildren(search, ref m_treeNodes[node.m_childIndex]);
                    AddAllTrianglesInChildren(search, ref m_treeNodes[node.m_childIndex + 1]);
                }
                else
                {
                    search.AddTriangles(node.m_triangles, node.m_bounds, scissorDistance < -search.m_scissorMargin);
                }
            }
        }

        [Serializable]
        private struct TreeNode
        {
            public Bounds m_bounds;
            public int m_childIndex;
            public int[] m_triangles;
        }

#if UNITY_EDITOR
        public override int GetMemoryUsage()
        {
            var size = base.GetMemoryUsage();
            if (m_nodeBoundsCenter != null)
            {
                size += m_nodeBoundsCenter.Length*sizeof (float)*6;
            }
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3)
            if (m_treeNodes != null && m_treeNodes.Length != 0 &&
                (m_nodeBoundsCenter == null || m_nodeBoundsCenter.Length == 0))
            {
                size += m_treeNodes.Length*(sizeof (float)*6 + sizeof (int) + IntPtr.Size);
                for (var i = 0; i < m_treeNodes.Length; ++i)
                {
                    if (m_treeNodes[i].m_triangles != null)
                    {
                        size += m_treeNodes[i].m_triangles.Length*sizeof (int) + sizeof (int);
                    }
                }
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
            return base.GetNodeCount();
        }

        private int m_totalTriangleCount;
        private int m_triangleCountInLeafNodes;

        public override float GetBuildProgress()
        {
            if (m_treeNodes != null)
            {
                return 1.0f;
            }
            if (m_totalTriangleCount == 0)
            {
                return 0;
            }
            return m_triangleCountInLeafNodes/(float) m_totalTriangleCount;
        }
#endif
    }
}