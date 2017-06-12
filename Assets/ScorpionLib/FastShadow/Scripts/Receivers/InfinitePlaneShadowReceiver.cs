#region using

using UnityEngine;

#endregion

namespace FastShadowReceiver
{
    public class InfinitePlaneShadowReceiver : PlaneShadowReceiverBase
    {
        public float m_height = 0.0f;
        public Vector3 m_normal = Vector3.up;
        public Transform m_target;

        protected override void OnUpdate()
        {
            if (projector == null)
            {
                Hide(true);
                return;
            }
            Hide(false);
            Plane plane;
            if (m_target != null)
            {
                plane = new Plane(m_target.TransformDirection(m_normal).normalized, m_target.position);
                plane.distance -= m_height;
            }
            else
            {
                plane = new Plane(m_normal, -m_height);
            }
            UpdatePlane(plane);
        }
    }
}