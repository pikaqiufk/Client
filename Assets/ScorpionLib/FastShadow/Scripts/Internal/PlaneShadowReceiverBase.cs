#region using

using UnityEngine;

#endregion

namespace FastShadowReceiver
{
    public abstract class PlaneShadowReceiverBase : ReceiverBase
    {
        private Vector3[] m_normals;
        private Transform m_selfTransform;
        private Vector3[] m_vertices;

        protected override void OnAwake()
        {
            m_vertices = new Vector3[4];
            m_normals = new Vector3[4];
            int[] indices = {0, 1, 2, 2, 1, 3};
            for (var i = 0; i < meshes.Length; ++i)
            {
                meshes[i].vertices = m_vertices;
                meshes[i].triangles = indices;
            }
            m_selfTransform = transform;
        }

        protected void UpdatePlane(Plane plane)
        {
            projector.GetPlaneIntersection(m_vertices, plane);
            m_normals[0] = m_normals[1] = m_normals[2] = m_normals[3] = plane.normal;
            m_selfTransform.position = Vector3.zero;
            m_selfTransform.rotation = Quaternion.identity;
            SwapMesh();
            currentMesh.vertices = m_vertices;
            currentMesh.normals = m_normals;
            currentMesh.RecalculateBounds();
        }
    }
}