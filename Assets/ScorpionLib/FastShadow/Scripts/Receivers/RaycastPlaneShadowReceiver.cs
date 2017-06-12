#region using

using UnityEngine;

#endregion

namespace FastShadowReceiver
{
    public class RaycastPlaneShadowReceiver : PlaneShadowReceiverBase
    {
        public LayerMask m_raycastMask = -1;

        protected override void OnUpdate()
        {
            if (projector == null)
            {
                Hide(true);
                return;
            }
            var origin = projector.position;
            var dir = projector.direction;
            RaycastHit hit;
            if (Physics.Raycast(origin, dir, out hit, projector.farClipPlane, m_raycastMask))
            {
                Hide(false);
                var plane = new Plane(hit.normal, hit.point);
                UpdatePlane(plane);
            }
            else
            {
                Hide(true);
            }
        }
    }
}