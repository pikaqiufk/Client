#region using

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace FastShadowReceiver
{
    /// <summary>
    ///     Attach this component to a caster object which will be lit by light projector.
    /// </summary>
    public class LightProjectorShadowCaster : MonoBehaviour
    {
        private static readonly List<LightProjectorShadowCaster> s_listCasters = new List<LightProjectorShadowCaster>();
        [SerializeField] private Vector2 m_extension = Vector2.one;
        [SerializeField] private bool m_manualUpdate;
        [SerializeField] private float m_nearClipPlane;
        [SerializeField] private float m_nearClipSharpness = 100.0f;
        [SerializeField] private ProjectionType m_projectionType = ProjectionType.Sprite;
        [SerializeField] private Texture m_shadowTexture;

        public enum ProjectionType
        {
            Sprite,
            Billboard,
            Plane
        }

        public Vector2 extension
        {
            get { return m_extension; }
            set { m_extension = value; }
        }

        public bool manualUpdate
        {
            get { return m_manualUpdate; }
            set { m_manualUpdate = value; }
        }

        public float nearClipPlane
        {
            get { return m_nearClipPlane; }
            set { m_nearClipPlane = value; }
        }

        public float nearClipSharpness
        {
            get { return m_nearClipSharpness; }
            set { m_nearClipSharpness = value; }
        }

        public ProjectionType projectionType
        {
            get { return m_projectionType; }
            set { m_projectionType = value; }
        }

        public Texture shadowTexture
        {
            get { return m_shadowTexture; }
            set { m_shadowTexture = value; }
        }

        public new Transform transform { get; private set; }

        private void Awake()
        {
            transform = base.transform;
        }

        public static List<LightProjectorShadowCaster> GetAllCasters()
        {
            return s_listCasters;
        }

        public Matrix4x4 GetOrthoProjectionMatrix(Vector3 lightDir)
        {
            var pos = transform.position;
            var x = transform.right;
            var y = transform.up;
            var z = lightDir;
            var epsilon = 0.001f*transform.forward;
            switch (m_projectionType)
            {
                case ProjectionType.Sprite:
                    x = x - Vector3.Dot(x, z)*(z + epsilon); // add epsilon to make sure that x never become zero.
                    x.Normalize();
                    y = y - Vector3.Dot(y, z)*(z + epsilon);
                    y.Normalize();
                    break;
                case ProjectionType.Billboard:
                    x = x - Vector3.Dot(x, z)*(z + epsilon);
                    x.Normalize();
                {
                    var n = Vector3.Cross(x, y);
                    y = y - (Vector3.Dot(y, z)/Vector3.Dot(z, n))*n;
                }
                    break;
                case ProjectionType.Plane:
                {
                    var n = Vector3.Cross(x, y);
                    var a = 1.0f/Vector3.Dot(z, n);
                    x = x - (a*Vector3.Dot(x, z))*n;
                    y = y - (a*Vector3.Dot(y, z))*n;
                }
                    break;
            }
            Vector4 rowX = x;
            Vector4 rowY = y;
            Vector4 rowZ = z;
            Vector4 rowW;
            rowX.w = -Vector3.Dot(x, pos);
            rowY.w = -Vector3.Dot(y, pos);
            rowZ.w = -Vector3.Dot(z, pos);
            rowW = Vector4.zero;
            rowW.w = 1.0f;
            rowX *= 0.5f/m_extension.x;
            rowY *= 0.5f/m_extension.y;
            rowX.w += 0.5f;
            rowY.w += 0.5f;
            var mat = Matrix4x4.zero;
            mat.SetRow(0, rowX);
            mat.SetRow(1, rowY);
            mat.SetRow(2, rowZ);
            mat.SetRow(3, rowW);
            return mat;
        }

        public Matrix4x4 GetProjectionMatrix(Vector3 lightPos)
        {
            var pos = transform.position;
            var x = transform.right;
            var y = transform.up;
            var z = transform.forward;
            var epsilon = 0.001f*z;
            switch (m_projectionType)
            {
                case ProjectionType.Sprite:
                    z = (pos - lightPos).normalized;
                    x = x - Vector3.Dot(x, z)*(z + epsilon); // add epsilon to make sure that x never become zero.
                    x.Normalize();
                    y = y - Vector3.Dot(y, z)*(z + epsilon);
                    y.Normalize();
                    break;
                case ProjectionType.Billboard:
                    z = pos - lightPos;
                    z = (z - Vector3.Dot(y, z)*y) + epsilon;
                    z.Normalize();
                    x = x - Vector3.Dot(x, z)*(z + epsilon);
                    x.Normalize();
                    break;
                case ProjectionType.Plane:
                    break;
            }
            Vector4 rowX = x;
            Vector4 rowY = y;
            Vector4 rowZ = z;
            Vector4 rowW;
            rowX.w = -Vector3.Dot(x, lightPos);
            rowY.w = -Vector3.Dot(y, lightPos);
            rowZ.w = -Vector3.Dot(z, pos) - m_nearClipPlane;
            rowZ *= m_nearClipSharpness;
            rowW = z;
            rowW.w = -Vector3.Dot(z, lightPos);
            Vector4 origin = pos;
            origin.w = 1.0f;
            var ow = Vector4.Dot(rowW, origin);
            rowX *= 0.5f*ow/m_extension.x;
            rowY *= 0.5f*ow/m_extension.y;
            var ox = Vector4.Dot(rowX, origin);
            var oy = Vector4.Dot(rowY, origin);
            var invW = 1.0f/ow;
            var transX = 0.5f - ox*invW;
            var transY = 0.5f - oy*invW;
            rowX += transX*rowW;
            rowY += transY*rowW;
            var mat = Matrix4x4.zero;
            mat.SetRow(0, rowX);
            mat.SetRow(1, rowY);
            mat.SetRow(2, rowZ);
            mat.SetRow(3, rowW);
            return mat;
        }

        public void GetShadowPlaneAxes(Vector3 lightDir, out Vector3 x, out Vector3 y)
        {
            x = transform.right;
            y = transform.up;
            var epsilon = 0.001f*transform.forward;
            switch (m_projectionType)
            {
                case ProjectionType.Sprite:
                    x = x - Vector3.Dot(x, lightDir)*(lightDir + epsilon);
                        // add epsilon to make sure that x never become zero.
                    x.Normalize();
                    y = y - Vector3.Dot(y, lightDir)*(lightDir + epsilon);
                    y.Normalize();
                    break;
                case ProjectionType.Billboard:
                    lightDir = (lightDir - Vector3.Dot(y, lightDir)*y) + epsilon;
                    lightDir.Normalize();
                    x = x - Vector3.Dot(x, lightDir)*(lightDir + epsilon);
                    x.Normalize();
                    break;
                case ProjectionType.Plane:
                    break;
            }
        }

        private void OnDisable()
        {
            s_listCasters.Remove(this);
        }

        private void OnEnable()
        {
            if (!s_listCasters.Contains(this))
            {
                s_listCasters.Add(this);
            }
        }
    }
}