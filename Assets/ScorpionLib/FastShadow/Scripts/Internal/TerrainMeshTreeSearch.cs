﻿#region using

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace FastShadowReceiver
{
    public class TerrainMeshTreeSearch : MeshTreeSearch
    {
        private const uint CLIP_FLAGS_NXNZ = 7 | (3 << 4) | (5 << 8) | (1 << 12);
        private const uint CLIP_FLAGS_NXPZ = 13 | (12 << 4) | (5 << 8) | (4 << 12);
        private const uint CLIP_FLAGS_PXNZ = 11 | (3 << 4) | (10 << 8) | (2 << 12);
        private const uint CLIP_FLAGS_PXPZ = 14 | (12 << 4) | (10 << 8) | (8 << 12);
        private uint[] m_clipFlags;
        private int m_indexCount;
        private int m_minX, m_maxX;
        private int m_minZ, m_maxZ;
        private List<Patch> m_patchList;
        private List<Patch> m_patchListToScissor;

        public void AddPatch(int posX, int posZ, int width, float minY, float maxY, bool isPartial)
        {
            if (isPartial && m_bScissor)
            {
                m_patchListToScissor.Add(new Patch(posX, posZ, width));
                return;
            }
            if (m_patchList.Count == 0)
            {
                m_minX = posX;
                m_minBounds.y = minY;
                m_minZ = posZ;
                m_maxX = posX + width;
                m_maxBounds.y = maxY;
                m_maxZ = posZ + width;
            }
            else
            {
                m_minX = Mathf.Min(m_minX, posX);
                m_minZ = Mathf.Min(m_minZ, posZ);
                m_maxX = Mathf.Max(m_maxX, posX + width);
                m_maxZ = Mathf.Max(m_maxZ, posZ + width);
                m_minBounds.y = Mathf.Min(m_minBounds.y, minY);
                m_maxBounds.y = Mathf.Max(m_maxBounds.y, maxY);
            }
            m_patchList.Add(new Patch(posX, posZ, width));
            m_indexCount += 6*width*width;
        }

        private Vector3 CalculateNormal(float[,] heightMap, float invScaleX, float invScaleZ, int z, int x)
        {
            float dx, dz;
            if (0 < x)
            {
                dx = invScaleX*(heightMap[z, x + 1] - heightMap[z, x - 1]);
            }
            else
            {
                dx = 2.0f*invScaleX*(heightMap[z, x + 1] - heightMap[z, 0]);
            }
            if (0 < z)
            {
                dz = invScaleZ*(heightMap[z + 1, x] - heightMap[z - 1, x]);
            }
            else
            {
                dz = 2.0f*invScaleZ*(heightMap[z + 1, x] - heightMap[0, x]);
            }
            return new Vector3(dx, 1.0f, dz).normalized;
        }

        internal void Finalize(float[,] heightMap, float scaleX, float scaleZ)
        {
            m_minBounds.x = scaleX*m_minX;
            m_minBounds.z = scaleZ*m_minZ;
            m_maxBounds.x = scaleX*m_maxX;
            m_maxBounds.z = scaleZ*m_maxZ;

            var scissoredTriangleCount = new ScissoredTriangleCount();
            // scissor triangles if any
            if (m_bScissor && 0 < m_patchListToScissor.Count)
            {
                var numTrianglesToScissor = 0;
                for (var i = 0; i < m_patchListToScissor.Count; ++i)
                {
                    var w = m_patchListToScissor[i].m_width;
                    numTrianglesToScissor += 2*w*w;
                }
                InitScissorBuffer(numTrianglesToScissor);
                for (var i = 0; i < m_patchListToScissor.Count; ++i)
                {
                    var patch = m_patchListToScissor[i];
                    for (int z = patch.m_posZ, zEnd = patch.m_posZ + patch.m_width; z < zEnd; ++z)
                    {
                        for (int x = patch.m_posX, xEnd = patch.m_posX + patch.m_width; x < xEnd; ++x)
                        {
                            var v0 = new Vector3(scaleX*x, heightMap[z, x], scaleZ*z);
                            var v1 = new Vector3(scaleX*(x + 1), heightMap[z, x + 1], scaleZ*z);
                            var v2 = new Vector3(scaleX*x, heightMap[z + 1, x], scaleZ*(z + 1));
                            var v3 = new Vector3(scaleX*(x + 1), heightMap[z + 1, x + 1], scaleZ*(z + 1));
                            ScissorTriangle(v0, v2, v3, ref scissoredTriangleCount, m_patchList.Count == 0);
                            ScissorTriangle(v0, v3, v1, ref scissoredTriangleCount, m_patchList.Count == 0);
                        }
                    }
                }
            }
            // create result buffer
            var vertexCount = scissoredTriangleCount.m_nVertexCount + (m_maxX - m_minX + 1)*(m_maxZ - m_minZ + 1);
            var indexCount = scissoredTriangleCount.m_nIndexCount + m_indexCount;
            InitResultBuffer(vertexCount, indexCount, false);
            m_bOutputNormals = false;
            // fill result buffer
            var nVertex = 0;
            var nIndex = 0;
            for (var z = m_minZ; z <= m_maxZ; ++z)
            {
                for (var x = m_minX; x <= m_maxX; ++x)
                {
                    m_result[nVertex++] = new Vector3(scaleX*x, heightMap[z, x], scaleZ*z);
                }
            }
            var width = m_maxX - m_minX + 1;
            for (var i = 0; i < m_patchList.Count; ++i)
            {
                var patch = m_patchList[i];
                var offset = (patch.m_posZ - m_minZ)*width + (patch.m_posX - m_minX);
                for (var z = 0; z < patch.m_width; ++z)
                {
                    var vtx = offset;
                    for (var x = 0; x < patch.m_width; ++x)
                    {
                        m_resultIndices[nIndex++] = vtx;
                        m_resultIndices[nIndex++] = vtx + width;
                        m_resultIndices[nIndex++] = vtx + width + 1;
                        m_resultIndices[nIndex++] = vtx;
                        m_resultIndices[nIndex++] = vtx + width + 1;
                        m_resultIndices[nIndex++] = ++vtx;
                    }
                    offset += width;
                }
            }
            FillResultWithScissoredTriangles(scissoredTriangleCount.m_nTriangleCount, ref nVertex, ref nIndex);
            var lastIndex = 0 < nIndex ? m_resultIndices[nIndex - 1] : 0;
            while (nIndex < m_resultIndices.Length)
            {
                m_resultIndices[nIndex++] = lastIndex;
            }
        }

        internal void FinalizeWithNormal(float[,] heightMap, float scaleX, float scaleZ)
        {
            m_minBounds.x = scaleX*m_minX;
            m_minBounds.z = scaleZ*m_minZ;
            m_maxBounds.x = scaleX*m_maxX;
            m_maxBounds.z = scaleZ*m_maxZ;

            var invScaleX = 0.5f/scaleX;
            var invScaleZ = 0.5f/scaleZ;
            var scissoredTriangleCount = new ScissoredTriangleCount();
            // scissor triangles if any
            if (m_bScissor && 0 < m_patchListToScissor.Count)
            {
                var numTrianglesToScissor = 0;
                for (var i = 0; i < m_patchListToScissor.Count; ++i)
                {
                    var w = m_patchListToScissor[i].m_width;
                    numTrianglesToScissor += 2*w*w;
                }
                InitScissorBuffer(numTrianglesToScissor);
                InitScissorNormalBuffer(numTrianglesToScissor);
                for (var i = 0; i < m_patchListToScissor.Count; ++i)
                {
                    var patch = m_patchListToScissor[i];
                    for (int z = patch.m_posZ, zEnd = patch.m_posZ + patch.m_width; z < zEnd; ++z)
                    {
                        for (int x = patch.m_posX, xEnd = patch.m_posX + patch.m_width; x < xEnd; ++x)
                        {
                            var v0 = new Vector3(scaleX*x, heightMap[z, x], scaleZ*z);
                            var n0 = CalculateNormal(heightMap, invScaleX, invScaleZ, z, x);
                            var v1 = new Vector3(scaleX*(x + 1), heightMap[z, x + 1], scaleZ*z);
                            var n1 = CalculateNormal(heightMap, invScaleX, invScaleZ, z, x + 1);
                            var v2 = new Vector3(scaleX*x, heightMap[z + 1, x], scaleZ*(z + 1));
                            var n2 = CalculateNormal(heightMap, invScaleX, invScaleZ, z + 1, x);
                            var v3 = new Vector3(scaleX*(x + 1), heightMap[z + 1, x + 1], scaleZ*(z + 1));
                            var n3 = CalculateNormal(heightMap, invScaleX, invScaleZ, z + 1, x + 1);
                            ScissorTriangle(v0, v2, v3, n0, n2, n3, ref scissoredTriangleCount, m_patchList.Count == 0);
                            ScissorTriangle(v0, v3, v1, n0, n3, n1, ref scissoredTriangleCount, m_patchList.Count == 0);
                        }
                    }
                }
            }
            // create result buffer
            var vertexCount = scissoredTriangleCount.m_nVertexCount + (m_maxX - m_minX + 1)*(m_maxZ - m_minZ + 1);
            var indexCount = scissoredTriangleCount.m_nIndexCount + m_indexCount;
            InitResultBuffer(vertexCount, indexCount, true);
            // fill result buffer
            var nVertex = 0;
            var nIndex = 0;
            for (var z = m_minZ; z <= m_maxZ; ++z)
            {
                for (var x = m_minX; x <= m_maxX; ++x)
                {
                    m_resultNormal[nVertex] = CalculateNormal(heightMap, invScaleX, invScaleZ, z, x);
                    m_result[nVertex++] = new Vector3(scaleX*x, heightMap[z, x], scaleZ*z);
                }
            }
            var width = m_maxX - m_minX + 1;
            for (var i = 0; i < m_patchList.Count; ++i)
            {
                var patch = m_patchList[i];
                var offset = (patch.m_posZ - m_minZ)*width + (patch.m_posX - m_minX);
                for (var z = 0; z < patch.m_width; ++z)
                {
                    var vtx = offset;
                    for (var x = 0; x < patch.m_width; ++x)
                    {
                        m_resultIndices[nIndex++] = vtx;
                        m_resultIndices[nIndex++] = vtx + width;
                        m_resultIndices[nIndex++] = vtx + width + 1;
                        m_resultIndices[nIndex++] = vtx;
                        m_resultIndices[nIndex++] = vtx + width + 1;
                        m_resultIndices[nIndex++] = ++vtx;
                    }
                    offset += width;
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
            var numClipPlanes = m_clipPlanes.clipPlaneCount;
            if (m_clipMetric == null || m_clipMetric.Length < numClipPlanes)
            {
                m_clipMetric = new Vector3[numClipPlanes];
            }
            if (m_clipFlags == null || m_clipFlags.Length < numClipPlanes)
            {
                m_clipFlags = new uint[numClipPlanes];
            }
            for (var i = 0; i < numClipPlanes; ++i)
            {
                var clipPlane = m_clipPlanes.clipPlanes[i];
                m_clipMetric[i].x = Mathf.Abs(clipPlane.normal.x);
                m_clipMetric[i].y = Mathf.Abs(clipPlane.normal.y);
                m_clipMetric[i].z = Mathf.Abs(clipPlane.normal.z);
                if (clipPlane.normal.x < 0)
                {
                    if (clipPlane.normal.z < 0)
                    {
                        m_clipFlags[i] = CLIP_FLAGS_NXNZ | (CLIP_FLAGS_PXPZ << 16);
                    }
                    else
                    {
                        m_clipFlags[i] = CLIP_FLAGS_NXPZ | (CLIP_FLAGS_PXNZ << 16);
                    }
                }
                else
                {
                    if (clipPlane.normal.z < 0)
                    {
                        m_clipFlags[i] = CLIP_FLAGS_PXNZ | (CLIP_FLAGS_NXPZ << 16);
                    }
                    else
                    {
                        m_clipFlags[i] = CLIP_FLAGS_PXPZ | (CLIP_FLAGS_NXNZ << 16);
                    }
                }
            }

            if (m_patchList == null)
            {
                m_patchList = new List<Patch>();
            }
            m_patchList.Clear();
            if (m_bScissor)
            {
                if (m_patchListToScissor == null)
                {
                    m_patchListToScissor = new List<Patch>();
                }
                m_patchListToScissor.Clear();
            }
            m_indexCount = 0;
            m_minX = m_maxX = 0;
            m_minZ = m_maxZ = 0;
            m_minBounds = Vector3.zero;
            m_maxBounds = Vector3.zero;
        }

        internal uint IsInView(Vector3 center, Vector3 extents, out bool isPartial)
        {
            var numClipPlanes = m_clipPlanes.clipPlaneCount;
            uint flags = 0xf;
            isPartial = false;
            for (var i = 0; i < numClipPlanes; ++i)
            {
                var distance = m_clipPlanes.clipPlanes[i].GetDistanceToPoint(center);
                var xExtent = extents.x*m_clipMetric[i].x;
                var yExtent = extents.y*m_clipMetric[i].y;
                var zExtent = extents.z*m_clipMetric[i].z;
                var extent = xExtent + yExtent + zExtent;
                var maxDistance = distance + extent;
                var minDistance = distance - extent;
                var clipDistance = m_clipPlanes.maxDistance[i];
                var clipFlags = m_clipFlags[i];
                if (maxDistance < 0 || clipDistance < minDistance)
                {
                    return 0U;
                }
                if (minDistance < 0)
                {
                    isPartial = true;
                    var d = distance + yExtent;
                    if (d < 0)
                    {
                        flags &= clipFlags;
                        if (d + xExtent < 0)
                        {
                            flags &= (clipFlags >> 4);
                        }
                        if (d + zExtent < 0)
                        {
                            flags &= (clipFlags >> 8);
                        }
                    }
                }
                if (m_clipPlanes.twoSideClipping && clipDistance < maxDistance)
                {
                    isPartial = true;
                    var d = clipDistance - distance + yExtent;
                    if (d < 0)
                    {
                        flags &= (clipFlags >> 16);
                        if (d + xExtent < 0)
                        {
                            flags &= (clipFlags >> 20);
                        }
                        if (d + zExtent < 0)
                        {
                            flags &= (clipFlags >> 24);
                        }
                    }
                }
            }
            return flags;
        }

        internal uint IsInView(Vector3 center, Vector3 extents)
        {
            var numClipPlanes = m_clipPlanes.clipPlaneCount;
            uint flags = 0xf00f; // clipFlags | (partialFlags << 12)
            for (var i = 0; i < numClipPlanes; ++i)
            {
                var distance = m_clipPlanes.clipPlanes[i].GetDistanceToPoint(center);
                var xExtent = extents.x*m_clipMetric[i].x;
                var yExtent = extents.y*m_clipMetric[i].y;
                var zExtent = extents.z*m_clipMetric[i].z;
                var extent = xExtent + yExtent + zExtent;
                var maxDistance = distance + extent;
                var minDistance = distance - extent;
                var clipDistance = m_clipPlanes.maxDistance[i];
                if (maxDistance < 0 || clipDistance < minDistance)
                {
                    return 0U;
                }
                if (minDistance < 0)
                {
                    flags = UpdateClipFlags(flags, m_clipFlags[i], distance, xExtent, yExtent, zExtent);
                }
                if (m_clipPlanes.twoSideClipping && clipDistance < maxDistance)
                {
                    flags = UpdateClipFlags(flags, m_clipFlags[i] >> 16, clipDistance - distance, xExtent, yExtent,
                        zExtent);
                }
            }
            return flags;
        }

        private static uint UpdateClipFlags(uint flags,
                                            uint clipFlags,
                                            float distance,
                                            float xExtent,
                                            float yExtent,
                                            float zExtent)
        {
            var d1 = distance + yExtent;
            var d2 = distance - yExtent;
            if (d1 < 0)
            {
                flags &= clipFlags & 0xf;
                if (d1 + xExtent < 0)
                {
                    flags &= (clipFlags >> 4);
                }
                if (d1 + zExtent < 0)
                {
                    flags &= (clipFlags >> 8);
                }
            }
            else if (0 < d2)
            {
                var f = clipFlags | 0xf;
                if (0 < d2 - xExtent)
                {
                    f |= (clipFlags << 8);
                }
                if (0 < d2 - zExtent)
                {
                    f |= (clipFlags << 4);
                }
                flags &= f;
            }
            else
            {
                flags &= 0xf;
            }
            return flags;
        }

        private struct Patch
        {
            public Patch(int posX, int posZ, int width)
            {
                m_posX = posX;
                m_posZ = posZ;
                m_width = width;
            }

            public readonly int m_posX;
            public readonly int m_posZ;
            public readonly int m_width;
        }
    }
}