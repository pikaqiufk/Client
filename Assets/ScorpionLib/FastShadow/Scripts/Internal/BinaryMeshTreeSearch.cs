#region using

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace FastShadowReceiver
{
    public class BinaryMeshTreeSearch : MeshTreeSearch
    {
        private List<int[]> m_triangleList;
        private List<int[]> m_triangleListToScissor;

        internal void AddTriangles(int[] trianglesToAdd, Bounds bounds, bool isPartial)
        {
            if (m_bScissor && isPartial)
            {
                m_triangleListToScissor.Add(trianglesToAdd);
            }
            else
            {
                var min = bounds.min;
                var max = bounds.max;
                if (m_triangleList.Count == 0)
                {
                    m_minBounds = min;
                    m_maxBounds = max;
                }
                else
                {
                    m_minBounds.x = Mathf.Min(m_minBounds.x, min.x);
                    m_minBounds.y = Mathf.Min(m_minBounds.y, min.y);
                    m_minBounds.z = Mathf.Min(m_minBounds.z, min.z);
                    m_maxBounds.x = Mathf.Max(m_maxBounds.x, max.x);
                    m_maxBounds.y = Mathf.Max(m_maxBounds.y, max.y);
                    m_maxBounds.z = Mathf.Max(m_maxBounds.z, max.z);
                }
                m_triangleList.Add(trianglesToAdd);
            }
        }

        internal void Finalize(Vector3[] vertices, int[] indices)
        {
            var scissoredTriangleCount = new ScissoredTriangleCount();
            // scissor triangles if any
            if (m_bScissor && 0 < m_triangleListToScissor.Count)
            {
                var numTrianglesToScissor = 0;
                for (var i = 0; i < m_triangleListToScissor.Count; ++i)
                {
                    numTrianglesToScissor += m_triangleListToScissor[i].Length;
                }
                InitScissorBuffer(numTrianglesToScissor);
                if (m_bBackfaceCulling)
                {
                    for (var i = 0; i < m_triangleListToScissor.Count; ++i)
                    {
                        var triangles = m_triangleListToScissor[i];
                        for (var tri = 0; tri < triangles.Length; ++tri)
                        {
                            var index = triangles[tri];
                            var v0 = vertices[indices[index++]];
                            var v1 = vertices[indices[index++]];
                            var v2 = vertices[indices[index++]];
                            if (isFrontFaceTriangle(v0, v1, v2))
                            {
                                ScissorTriangle(v0, v1, v2, ref scissoredTriangleCount, m_triangleList.Count == 0);
                            }
                        }
                    }
                }
                else
                {
                    for (var i = 0; i < m_triangleListToScissor.Count; ++i)
                    {
                        var triangles = m_triangleListToScissor[i];
                        for (var tri = 0; tri < triangles.Length; ++tri)
                        {
                            var index = triangles[tri];
                            var v0 = vertices[indices[index++]];
                            var v1 = vertices[indices[index++]];
                            var v2 = vertices[indices[index++]];
                            ScissorTriangle(v0, v1, v2, ref scissoredTriangleCount, m_triangleList.Count == 0);
                        }
                    }
                }
            }
            // count vertices and indices
            var numVertexCount = 0;
            for (var i = 0; i < m_triangleList.Count; ++i)
            {
                numVertexCount += m_triangleList[i].Length*3;
            }
            var numIndexCount = numVertexCount;
            numVertexCount += scissoredTriangleCount.m_nVertexCount;
            numIndexCount += scissoredTriangleCount.m_nIndexCount;
            // create result buffer
            InitResultBuffer(numVertexCount, numIndexCount, false);
            m_bOutputNormals = false;
            // fill result buffer
            var nVertex = 0;
            var nIndex = 0;
            if (m_bBackfaceCulling)
            {
                for (var i = 0; i < m_triangleList.Count; ++i)
                {
                    var triangles = m_triangleList[i];
                    for (var tri = 0; tri < triangles.Length; ++tri)
                    {
                        var index = triangles[tri];
                        var v0 = vertices[indices[index++]];
                        var v1 = vertices[indices[index++]];
                        var v2 = vertices[indices[index++]];
                        if (isFrontFaceTriangle(v0, v1, v2))
                        {
                            m_resultIndices[nIndex++] = nVertex;
                            m_result[nVertex++] = v0;
                            m_resultIndices[nIndex++] = nVertex;
                            m_result[nVertex++] = v1;
                            m_resultIndices[nIndex++] = nVertex;
                            m_result[nVertex++] = v2;
                        }
                    }
                }
            }
            else
            {
                for (var i = 0; i < m_triangleList.Count; ++i)
                {
                    var triangles = m_triangleList[i];
                    for (var tri = 0; tri < triangles.Length; ++tri)
                    {
                        var index = triangles[tri];
                        m_resultIndices[nIndex++] = nVertex;
                        m_result[nVertex++] = vertices[indices[index++]];
                        m_resultIndices[nIndex++] = nVertex;
                        m_result[nVertex++] = vertices[indices[index++]];
                        m_resultIndices[nIndex++] = nVertex;
                        m_result[nVertex++] = vertices[indices[index++]];
                    }
                }
            }
            FillResultWithScissoredTriangles(scissoredTriangleCount.m_nTriangleCount, ref nVertex, ref nIndex);
            var lastIndex = 0 < nIndex ? m_resultIndices[nIndex - 1] : 0;
            while (nIndex < m_resultIndices.Length)
            {
                m_resultIndices[nIndex++] = lastIndex;
            }
        }

        internal void Finalize(Vector3[] vertices, int[] indices, Vector3[] normals)
        {
            var scissoredTriangleCount = new ScissoredTriangleCount();
            // scissor triangles if any
            if (m_bScissor && 0 < m_triangleListToScissor.Count)
            {
                var numTrianglesToScissor = 0;
                for (var i = 0; i < m_triangleListToScissor.Count; ++i)
                {
                    numTrianglesToScissor += m_triangleListToScissor[i].Length;
                }
                InitScissorBuffer(numTrianglesToScissor);
                InitScissorNormalBuffer(numTrianglesToScissor);
                if (m_bBackfaceCulling)
                {
                    for (var i = 0; i < m_triangleListToScissor.Count; ++i)
                    {
                        var triangles = m_triangleListToScissor[i];
                        for (var tri = 0; tri < triangles.Length; ++tri)
                        {
                            var index = triangles[tri];
                            var v0 = vertices[indices[index]];
                            var v1 = vertices[indices[index + 1]];
                            var v2 = vertices[indices[index + 2]];
                            if (isFrontFaceTriangle(v0, v1, v2))
                            {
                                var n0 = normals[indices[index++]];
                                var n1 = normals[indices[index++]];
                                var n2 = normals[indices[index++]];
                                ScissorTriangle(v0, v1, v2, n0, n1, n2, ref scissoredTriangleCount,
                                    m_triangleList.Count == 0);
                            }
                        }
                    }
                }
                else
                {
                    for (var i = 0; i < m_triangleListToScissor.Count; ++i)
                    {
                        var triangles = m_triangleListToScissor[i];
                        for (var tri = 0; tri < triangles.Length; ++tri)
                        {
                            var index = triangles[tri];
                            var v0 = vertices[indices[index]];
                            var n0 = normals[indices[index++]];
                            var v1 = vertices[indices[index]];
                            var n1 = normals[indices[index++]];
                            var v2 = vertices[indices[index]];
                            var n2 = normals[indices[index++]];
                            ScissorTriangle(v0, v1, v2, n0, n1, n2, ref scissoredTriangleCount,
                                m_triangleList.Count == 0);
                        }
                    }
                }
            }
            // count vertices and indices
            var numVertexCount = 0;
            for (var i = 0; i < m_triangleList.Count; ++i)
            {
                numVertexCount += m_triangleList[i].Length*3;
            }
            var numIndexCount = numVertexCount;
            numVertexCount += scissoredTriangleCount.m_nVertexCount;
            numIndexCount += scissoredTriangleCount.m_nIndexCount;
            // create result buffer
            InitResultBuffer(numVertexCount, numIndexCount, true);
            // fill result buffer
            var nVertex = 0;
            var nIndex = 0;
            if (m_bBackfaceCulling)
            {
                for (var i = 0; i < m_triangleList.Count; ++i)
                {
                    var triangles = m_triangleList[i];
                    for (var tri = 0; tri < triangles.Length; ++tri)
                    {
                        var index = triangles[tri];
                        var v0 = vertices[indices[index]];
                        var v1 = vertices[indices[index + 1]];
                        var v2 = vertices[indices[index + 2]];
                        if (isFrontFaceTriangle(v0, v1, v2))
                        {
                            m_resultIndices[nIndex++] = nVertex;
                            m_result[nVertex] = v0;
                            m_resultNormal[nVertex++] = normals[indices[index++]];
                            m_resultIndices[nIndex++] = nVertex;
                            m_result[nVertex] = v1;
                            m_resultNormal[nVertex++] = normals[indices[index++]];
                            m_resultIndices[nIndex++] = nVertex;
                            m_result[nVertex] = v2;
                            m_resultNormal[nVertex++] = normals[indices[index++]];
                        }
                    }
                }
            }
            else
            {
                for (var i = 0; i < m_triangleList.Count; ++i)
                {
                    var triangles = m_triangleList[i];
                    for (var tri = 0; tri < triangles.Length; ++tri)
                    {
                        var index = triangles[tri];
                        m_resultIndices[nIndex++] = nVertex;
                        m_result[nVertex] = vertices[indices[index]];
                        m_resultNormal[nVertex++] = normals[indices[index++]];
                        m_resultIndices[nIndex++] = nVertex;
                        m_result[nVertex] = vertices[indices[index]];
                        m_resultNormal[nVertex++] = normals[indices[index++]];
                        m_resultIndices[nIndex++] = nVertex;
                        m_result[nVertex] = vertices[indices[index]];
                        m_resultNormal[nVertex++] = normals[indices[index++]];
                    }
                }
            }
            FillResultWithScissoredTrianglesWithNormals(scissoredTriangleCount.m_nTriangleCount, ref nVertex, ref nIndex);
            var lastIndex = 0 < nIndex ? m_resultIndices[nIndex - 1] : 0;
            while (nIndex < m_resultIndices.Length)
            {
                m_resultIndices[nIndex++] = lastIndex;
            }
        }

        internal void Initialize()
        {
            InitClipMetrics();
            if (m_triangleList == null)
            {
                m_triangleList = new List<int[]>();
            }
            m_triangleList.Clear();
            if (m_bScissor)
            {
                if (m_triangleListToScissor == null)
                {
                    m_triangleListToScissor = new List<int[]>();
                }
                m_triangleListToScissor.Clear();
            }
            m_minBounds = Vector3.zero;
            m_maxBounds = Vector3.zero;
        }

        internal bool IsInView(Bounds bounds, out float scissorDistance)
        {
            var numClipPlanes = m_clipPlanes.clipPlaneCount;
            scissorDistance = 0.0f;
            for (var i = 0; i < numClipPlanes; ++i)
            {
                var distance = m_clipPlanes.clipPlanes[i].GetDistanceToPoint(bounds.center);
                var extent = Vector3.Dot(bounds.extents, m_clipMetric[i]);
                var maxDistance = distance + extent;
                var minDistance = distance - extent;
                if (maxDistance < 0 || m_clipPlanes.maxDistance[i] < minDistance)
                {
                    return false;
                }
                scissorDistance = Mathf.Min(Mathf.Min(scissorDistance, minDistance),
                    m_clipPlanes.maxDistance[i] - maxDistance);
            }
            return true;
        }
    }
}