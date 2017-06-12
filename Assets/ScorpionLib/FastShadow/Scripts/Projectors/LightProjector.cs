#region using

using UnityEngine;

#endregion

namespace FastShadowReceiver
{
    /// <summary>
    ///     The name "LightProjector" is a bit confusing. This component is not a "light projector", unlike
    ///     UnityEngine.Projector.
    ///     This component calculates shadow projection volume of m_target object which is lit by m_light. Typically used with
    ///     shadowmap.
    /// </summary>
    public class LightProjector : ProjectorBase
    {
        public Light m_light;
        public Bounds m_targetBounds;
        public Transform m_target;
        public float m_nearClipPlane = 0.1f;
        public float m_farClipPlane = 50.0f;

        public override bool isOrthographic
        {
            get { return m_light.type == LightType.Directional; }
        }

        public override Vector3 position
        {
            get
            {
                if (m_light.type == LightType.Directional)
                {
                    return m_target.TransformPoint(m_targetBounds.center);
                }
                return lightTransform.position;
            }
        }

        public override Vector3 direction
        {
            get
            {
                if (m_light.type == LightType.Directional)
                {
                    return lightTransform.forward;
                }
                return (m_target.TransformPoint(m_targetBounds.center) - lightTransform.position).normalized;
            }
        }

        public override Quaternion rotation
        {
            get
            {
                var z = direction;
                var right = m_target.right;
                var up = m_target.up;
                var forward = m_target.forward;
                var xDepth = Mathf.Abs(Vector3.Dot(right, z));
                var sqExtents = m_targetBounds.extents*m_targetBounds.extents.x;
                var xSize = sqExtents.x*(1.0f - xDepth*xDepth);
                var yDepth = Mathf.Abs(Vector3.Dot(up, z));
                var ySize = sqExtents.y*(1.0f - yDepth*yDepth);
                var zDepth = Mathf.Abs(Vector3.Dot(forward, z));
                var zSize = sqExtents.z*(1.0f - zDepth*zDepth);
                Vector3 x;
                if (ySize <= xSize && zSize <= xSize)
                {
                    x = m_target.right;
                }
                else if (xSize <= ySize && zSize <= ySize)
                {
                    x = m_target.up;
                }
                else
                {
                    x = m_target.forward;
                }
                var y = Vector3.Cross(z, x).normalized;
                return Quaternion.LookRotation(z, y);
            }
        }

        public override Matrix4x4 uvProjectionMatrix
        {
            get
            {
                var z = direction;
                var right = m_target.right;
                var up = m_target.up;
                var forward = m_target.forward;
                var xDepth = Mathf.Abs(Vector3.Dot(right, z));
                var xSize = m_targetBounds.extents.x*Mathf.Sqrt(Mathf.Max(0.0f, 1.0f - xDepth*xDepth));
                var yDepth = Mathf.Abs(Vector3.Dot(up, z));
                var ySize = m_targetBounds.extents.y*Mathf.Sqrt(Mathf.Max(0.0f, 1.0f - yDepth*yDepth));
                var zDepth = Mathf.Abs(Vector3.Dot(forward, z));
                var zSize = m_targetBounds.extents.z*Mathf.Sqrt(Mathf.Max(0.0f, 1.0f - zDepth*zDepth));
                Vector3 x;
                if (ySize <= xSize && zSize <= xSize)
                {
                    x = m_target.right;
                }
                else if (xSize <= ySize && zSize <= ySize)
                {
                    x = m_target.up;
                }
                else
                {
                    x = m_target.forward;
                }
                var y = Vector3.Cross(z, x).normalized;
                x = Vector3.Cross(y, z);
                xSize = m_targetBounds.extents.x*Mathf.Abs(Vector3.Dot(right, x))
                        + m_targetBounds.extents.y*Mathf.Abs(Vector3.Dot(up, x))
                        + m_targetBounds.extents.z*Mathf.Abs(Vector3.Dot(forward, x));
                ySize = m_targetBounds.extents.x*Mathf.Abs(Vector3.Dot(right, y))
                        + m_targetBounds.extents.y*Mathf.Abs(Vector3.Dot(up, y))
                        + m_targetBounds.extents.z*Mathf.Abs(Vector3.Dot(forward, y));
                if (m_light.type == LightType.Directional)
                {
                    x *= 0.5f/xSize;
                    y *= 0.5f/ySize;
                    Vector4 rowX = x;
                    Vector4 rowY = y;
                    Vector4 rowZ = z;
                    Vector4 rowW;
                    var o = position;
                    rowX.w = -Vector3.Dot(x, o);
                    rowY.w = -Vector3.Dot(y, o);
                    rowZ.w = -Vector3.Dot(z, o) - m_nearClipPlane;
                    rowZ /= m_farClipPlane - m_nearClipPlane;
                    rowW = Vector4.zero;
                    rowW.w = 1.0f;
                    rowX.w += 0.5f;
                    rowY.w += 0.5f;
                    var matProjection = Matrix4x4.zero;
                    matProjection.SetRow(0, rowX);
                    matProjection.SetRow(1, rowY);
                    matProjection.SetRow(2, rowZ);
                    matProjection.SetRow(3, rowW);
                    return matProjection;
                }
                else
                {
                    var pos = m_target.position;
                    var lightPos = lightTransform.position;
                    var depth = Vector3.Dot(z, (pos - lightPos)) - xDepth - yDepth - zDepth;
                    if (depth < m_nearClipPlane)
                    {
                        depth = m_nearClipPlane;
                    }
                    x *= 0.5f*depth/xSize;
                    y *= 0.5f*depth/ySize;
                    Vector4 rowX = x;
                    Vector4 rowY = y;
                    Vector4 rowZ = z;
                    Vector4 rowW;
                    rowX.w = -Vector3.Dot(x, pos);
                    rowY.w = -Vector3.Dot(y, pos);
                    rowZ.w = -Vector3.Dot(z, pos) - m_nearClipPlane;
                    rowZ /= m_farClipPlane - m_nearClipPlane;
                    rowW = z;
                    rowW.w = -Vector3.Dot(z, lightPos);
                    rowX += 0.5f*rowW;
                    rowY += 0.5f*rowW;
                    var matProjection = Matrix4x4.zero;
                    matProjection.SetRow(0, rowX);
                    matProjection.SetRow(1, rowY);
                    matProjection.SetRow(2, rowZ);
                    matProjection.SetRow(3, rowW);
                    return matProjection;
                }
            }
        }

        public override void GetPlaneIntersection(Vector3[] vertices, Plane plane)
        {
            var z = direction;
            if (0 <= Vector3.Dot(z, plane.normal))
            {
                vertices[0] = vertices[1] = vertices[2] = vertices[3] = Vector3.zero;
                return;
            }
            var right = m_target.right;
            var up = m_target.up;
            var forward = m_target.forward;
            var xDepth = Mathf.Abs(Vector3.Dot(right, z));
            var xSize = m_targetBounds.extents.x*Mathf.Sqrt(Mathf.Max(0.0f, 1.0f - xDepth*xDepth));
            var yDepth = Mathf.Abs(Vector3.Dot(up, z));
            var ySize = m_targetBounds.extents.y*Mathf.Sqrt(Mathf.Max(0.0f, 1.0f - yDepth*yDepth));
            var zDepth = Mathf.Abs(Vector3.Dot(forward, z));
            var zSize = m_targetBounds.extents.z*Mathf.Sqrt(Mathf.Max(0.0f, 1.0f - zDepth*zDepth));
            Vector3 x;
            if (ySize <= xSize && zSize <= xSize)
            {
                x = m_target.right;
            }
            else if (xSize <= ySize && zSize <= ySize)
            {
                x = m_target.up;
            }
            else
            {
                x = m_target.forward;
            }
            var y = Vector3.Cross(z, x).normalized;
            x = Vector3.Cross(y, z);
            xSize = m_targetBounds.extents.x*Mathf.Abs(Vector3.Dot(right, x))
                    + m_targetBounds.extents.y*Mathf.Abs(Vector3.Dot(up, x))
                    + m_targetBounds.extents.z*Mathf.Abs(Vector3.Dot(forward, x));
            ySize = m_targetBounds.extents.x*Mathf.Abs(Vector3.Dot(right, y))
                    + m_targetBounds.extents.y*Mathf.Abs(Vector3.Dot(up, y))
                    + m_targetBounds.extents.z*Mathf.Abs(Vector3.Dot(forward, y));
            if (m_light.type == LightType.Directional)
            {
                var o = position;
                x *= xSize;
                y *= ySize;
                var v0 = o - x - y;
                var v1 = o - x + y;
                var v2 = o + x - y;
                var v3 = o + x + y;
                var invZdotN = 1.0f/Vector3.Dot(z, plane.normal);
                vertices[0] = v0 - (invZdotN*(plane.distance + Vector3.Dot(v0, plane.normal)))*z;
                vertices[1] = v1 - (invZdotN*(plane.distance + Vector3.Dot(v1, plane.normal)))*z;
                vertices[2] = v2 - (invZdotN*(plane.distance + Vector3.Dot(v2, plane.normal)))*z;
                vertices[3] = v3 - (invZdotN*(plane.distance + Vector3.Dot(v3, plane.normal)))*z;
            }
            else
            {
                x *= xSize;
                y *= ySize;
                var depth = Vector3.Dot(z, (m_target.position - lightTransform.position)) - xDepth - yDepth - zDepth;
                if (depth < m_nearClipPlane)
                {
                    depth = m_nearClipPlane;
                }
                z *= depth;
                var o = lightTransform.position;
                var v0 = z - x - y;
                var v1 = z - x + y;
                var v2 = z + x - y;
                var v3 = z + x + y;
                var d = Vector3.Dot(plane.normal, o) + plane.distance;
                vertices[0] = o - (d/Vector3.Dot(plane.normal, v0))*v0;
                vertices[1] = o - (d/Vector3.Dot(plane.normal, v1))*v1;
                vertices[2] = o - (d/Vector3.Dot(plane.normal, v2))*v2;
                vertices[3] = o - (d/Vector3.Dot(plane.normal, v3))*v3;
            }
        }

        public override void GetClipPlanes(ref ClipPlanes clipPlanes, Transform clipPlaneTransform)
        {
            var z = direction;
            var right = m_target.right;
            var up = m_target.up;
            var forward = m_target.forward;
            var xDepth = Mathf.Abs(Vector3.Dot(right, z));
            var xSize = m_targetBounds.extents.x*Mathf.Sqrt(Mathf.Max(0.0f, 1.0f - xDepth*xDepth));
            var yDepth = Mathf.Abs(Vector3.Dot(up, z));
            var ySize = m_targetBounds.extents.y*Mathf.Sqrt(Mathf.Max(0.0f, 1.0f - yDepth*yDepth));
            var zDepth = Mathf.Abs(Vector3.Dot(forward, z));
            var zSize = m_targetBounds.extents.z*Mathf.Sqrt(Mathf.Max(0.0f, 1.0f - zDepth*zDepth));
            Vector3 x;
            if (ySize <= xSize && zSize <= xSize)
            {
                x = m_target.right;
            }
            else if (xSize <= ySize && zSize <= ySize)
            {
                x = m_target.up;
            }
            else
            {
                x = m_target.forward;
            }
            var y = Vector3.Cross(z, x).normalized;
            x = Vector3.Cross(y, z);
            xSize = m_targetBounds.extents.x*Mathf.Abs(Vector3.Dot(right, x))
                    + m_targetBounds.extents.y*Mathf.Abs(Vector3.Dot(up, x))
                    + m_targetBounds.extents.z*Mathf.Abs(Vector3.Dot(forward, x));
            ySize = m_targetBounds.extents.x*Mathf.Abs(Vector3.Dot(right, y))
                    + m_targetBounds.extents.y*Mathf.Abs(Vector3.Dot(up, y))
                    + m_targetBounds.extents.z*Mathf.Abs(Vector3.Dot(forward, y));
            if (m_light.type == LightType.Directional)
            {
                var o = position;
                clipPlanes.SetClipPlaneNum(3, 2, true);
                clipPlanes.clipPlanes[0] = new Plane(x, o);
                clipPlanes.clipPlanes[0].distance += xSize;
                clipPlanes.maxDistance[0] = 2*xSize;
                clipPlanes.clipPlanes[1] = new Plane(y, o);
                clipPlanes.clipPlanes[1].distance += ySize;
                clipPlanes.maxDistance[1] = 2*ySize;
                clipPlanes.clipPlanes[2] = new Plane(z, o);
                clipPlanes.clipPlanes[2].distance -= m_nearClipPlane;
                clipPlanes.maxDistance[2] = m_farClipPlane - m_nearClipPlane;
            }
            else
            {
                var zPos = Vector3.Dot(z, (m_target.position - lightTransform.position));
                var o = lightTransform.position;
                clipPlanes.SetClipPlaneNum(5, 4, false);
                var depth = zPos - xDepth - yDepth - zDepth;
                if (depth < m_nearClipPlane)
                {
                    depth = m_nearClipPlane;
                }
                depth = 1.0f/depth;
                xSize *= depth;
                ySize *= depth;
                var x0 = (x + xSize*z).normalized;
                var x1 = (-x + xSize*z).normalized;
                var y0 = (y + ySize*z).normalized;
                var y1 = (-y + ySize*z).normalized;
                clipPlanes.clipPlanes[0] = new Plane(x0, o);
                clipPlanes.clipPlanes[1] = new Plane(y0, o);
                clipPlanes.clipPlanes[2] = new Plane(x1, o);
                clipPlanes.clipPlanes[3] = new Plane(y1, o);
                clipPlanes.clipPlanes[4] = new Plane(-z, o);
                clipPlanes.clipPlanes[4].distance += m_farClipPlane + zPos;
                clipPlanes.maxDistance[0] = clipPlanes.maxDistance[2] = 2.0f*xSize*m_farClipPlane;
                clipPlanes.maxDistance[1] = clipPlanes.maxDistance[3] = 2.0f*ySize*m_farClipPlane;
                clipPlanes.maxDistance[4] = m_farClipPlane - m_nearClipPlane;
            }
            if (clipPlaneTransform != null)
            {
                var m = clipPlaneTransform.localToWorldMatrix.transpose;
                Vector3 t = m.GetRow(3);
                for (var i = 0; i < clipPlanes.clipPlaneCount; ++i)
                {
                    var d = Vector3.Dot(clipPlanes.clipPlanes[i].normal, t);
                    clipPlanes.clipPlanes[i].distance += d;
                    clipPlanes.clipPlanes[i].normal = m.MultiplyVector(clipPlanes.clipPlanes[i].normal);
                }
            }
        }

        public override void GetClipPlanes(ref ClipPlanes clipPlanes,
                                           Transform clipPlaneTransform,
                                           ITransformPredictor predictor)
        {
            Vector3 z;
            var moveBounds = predictor.PredictNextFramePositionChanges();
            if (m_light.type == LightType.Directional)
            {
                z = lightTransform.forward;
            }
            else
            {
                z =
                    (m_target.TransformPoint(m_targetBounds.center + moveBounds.center) - lightTransform.position)
                        .normalized;
            }
            var right = m_target.right;
            var up = m_target.up;
            var forward = m_target.forward;
            var angleBounds = predictor.PredictNextFrameEulerAngleChanges();
            if (angleBounds.center != Vector3.zero)
            {
                var rot = m_target.rotation*Quaternion.Euler(angleBounds.center)*Quaternion.Inverse(m_target.rotation);
                right = rot*right;
                up = rot*up;
                forward = rot*forward;
            }
            var xExtent = m_targetBounds.extents.x + moveBounds.extents.x;
            var yExtent = m_targetBounds.extents.y + moveBounds.extents.y;
            var zExtent = m_targetBounds.extents.z + moveBounds.extents.z;
            if (angleBounds.extents.x != 0.0f)
            {
                var cos = Mathf.Cos(Mathf.Deg2Rad*Mathf.Min(90, angleBounds.extents.x));
                var rcpLen = 1.0f/Mathf.Sqrt(yExtent*yExtent + zExtent*zExtent);
                var cos_y = Mathf.Max(cos, yExtent*rcpLen);
                var cos_z = Mathf.Max(cos, zExtent*rcpLen);
                var ySin = Mathf.Sqrt(1.0f - cos_z*cos_z)*yExtent;
                var zSin = Mathf.Sqrt(1.0f - cos_y*cos_y)*zExtent;
                yExtent = cos_y*yExtent + zSin;
                zExtent = cos_z*zExtent + ySin;
            }
            if (angleBounds.extents.y != 0.0f)
            {
                var cos = Mathf.Cos(Mathf.Deg2Rad*Mathf.Min(90, angleBounds.extents.y));
                var rcpLen = 1.0f/Mathf.Sqrt(zExtent*zExtent + xExtent*xExtent);
                var cos_z = Mathf.Max(cos, zExtent*rcpLen);
                var cos_x = Mathf.Max(cos, xExtent*rcpLen);
                var zSin = Mathf.Sqrt(1.0f - cos_x*cos_x)*zExtent;
                var xSin = Mathf.Sqrt(1.0f - cos_z*cos_z)*xExtent;
                zExtent = cos_z*zExtent + xSin;
                xExtent = cos_x*xExtent + zSin;
            }
            if (angleBounds.extents.z != 0.0f)
            {
                var cos = Mathf.Cos(Mathf.Deg2Rad*Mathf.Min(90, angleBounds.extents.z));
                var rcpLen = 1.0f/Mathf.Sqrt(xExtent*xExtent + yExtent*yExtent);
                var cos_x = Mathf.Max(cos, xExtent*rcpLen);
                var cos_y = Mathf.Max(cos, yExtent*rcpLen);
                var xSin = Mathf.Sqrt(1.0f - cos_y*cos_y)*xExtent;
                var ySin = Mathf.Sqrt(1.0f - cos_x*cos_x)*yExtent;
                xExtent = cos_x*xExtent + ySin;
                yExtent = cos_y*yExtent + xSin;
            }
            var xDepth = Mathf.Abs(Vector3.Dot(right, z));
            var xSize = xExtent*Mathf.Sqrt(Mathf.Max(0.0f, 1.0f - xDepth*xDepth));
            var yDepth = Mathf.Abs(Vector3.Dot(up, z));
            var ySize = yExtent*Mathf.Sqrt(Mathf.Max(0.0f, 1.0f - yDepth*yDepth));
            var zDepth = Mathf.Abs(Vector3.Dot(forward, z));
            var zSize = zExtent*Mathf.Sqrt(Mathf.Max(0.0f, 1.0f - zDepth*zDepth));
            Vector3 x;
            if (ySize <= xSize && zSize <= xSize)
            {
                x = right;
            }
            else if (xSize <= ySize && zSize <= ySize)
            {
                x = up;
            }
            else
            {
                x = forward;
            }
            var y = Vector3.Cross(z, x).normalized;
            x = Vector3.Cross(y, z);
            xSize = xExtent*Mathf.Abs(Vector3.Dot(right, x)) + yExtent*Mathf.Abs(Vector3.Dot(up, x)) +
                    zExtent*Mathf.Abs(Vector3.Dot(forward, x));
            ySize = xExtent*Mathf.Abs(Vector3.Dot(right, y)) + yExtent*Mathf.Abs(Vector3.Dot(up, y)) +
                    zExtent*Mathf.Abs(Vector3.Dot(forward, y));
            if (m_light.type == LightType.Directional)
            {
                var o = m_target.TransformPoint(m_targetBounds.center + moveBounds.center);
                clipPlanes.SetClipPlaneNum(3, 2, true);
                clipPlanes.clipPlanes[0] = new Plane(x, o);
                clipPlanes.clipPlanes[0].distance += xSize;
                clipPlanes.maxDistance[0] = 2*xSize;
                clipPlanes.clipPlanes[1] = new Plane(y, o);
                clipPlanes.clipPlanes[1].distance += ySize;
                clipPlanes.maxDistance[1] = 2*ySize;
                clipPlanes.clipPlanes[2] = new Plane(z, o);
                clipPlanes.clipPlanes[2].distance -= m_nearClipPlane;
                clipPlanes.maxDistance[2] = m_farClipPlane - m_nearClipPlane;
            }
            else
            {
                var zPos = Vector3.Dot(z, (m_target.position - lightTransform.position));
                var o = lightTransform.position;
                clipPlanes.SetClipPlaneNum(5, 4, false);
                var depth = zPos - xDepth - yDepth - zDepth;
                if (depth < m_nearClipPlane)
                {
                    depth = m_nearClipPlane;
                }
                depth = 1.0f/depth;
                xSize *= depth;
                ySize *= depth;
                var x0 = (x + xSize*z).normalized;
                var x1 = (-x + xSize*z).normalized;
                var y0 = (y + ySize*z).normalized;
                var y1 = (-y + ySize*z).normalized;
                clipPlanes.clipPlanes[0] = new Plane(x0, o);
                clipPlanes.clipPlanes[1] = new Plane(y0, o);
                clipPlanes.clipPlanes[2] = new Plane(x1, o);
                clipPlanes.clipPlanes[3] = new Plane(y1, o);
                clipPlanes.clipPlanes[4] = new Plane(-z, o);
                clipPlanes.clipPlanes[4].distance += m_farClipPlane + zPos;
                clipPlanes.maxDistance[0] = clipPlanes.maxDistance[2] = 2.0f*xSize*m_farClipPlane;
                clipPlanes.maxDistance[1] = clipPlanes.maxDistance[3] = 2.0f*ySize*m_farClipPlane;
                clipPlanes.maxDistance[4] = m_farClipPlane - m_nearClipPlane;
            }
            if (clipPlaneTransform != null)
            {
                var m = clipPlaneTransform.localToWorldMatrix.transpose;
                Vector3 t = m.GetRow(3);
                for (var i = 0; i < clipPlanes.clipPlaneCount; ++i)
                {
                    var d = Vector3.Dot(clipPlanes.clipPlanes[i].normal, t);
                    clipPlanes.clipPlanes[i].distance += d;
                    clipPlanes.clipPlanes[i].normal = m.MultiplyVector(clipPlanes.clipPlanes[i].normal);
                }
            }
        }

        public override float nearClipPlane
        {
            get
            {
                if (m_light.type == LightType.Directional)
                {
                    return m_nearClipPlane;
                }
                var zPos = Vector3.Dot(direction, (m_target.position - lightTransform.position));
                return m_nearClipPlane + zPos;
            }
        }

        public override float farClipPlane
        {
            get
            {
                if (m_light.type == LightType.Directional)
                {
                    return m_farClipPlane;
                }
                var zPos = Vector3.Dot(direction, (m_target.position - lightTransform.position));
                return m_farClipPlane + zPos;
            }
        }

        protected override void OnDrawGizmosSelected()
        {
            if (m_target != null)
            {
                base.OnDrawGizmosSelected();
            }
        }

#if !UNITY_EDITOR
		private Transform m_lightTransform;
		void Awake()
		{
			m_lightTransform = m_light.transform;
		}
#endif

        private Transform lightTransform
        {
            get
            {
#if UNITY_EDITOR
                return m_light.transform;
#else
				return m_lightTransform;
#endif
            }
        }
    }
}