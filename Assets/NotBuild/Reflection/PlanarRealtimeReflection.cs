#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections;

[ExecuteInEditMode()]
public class PlanarRealtimeReflection : MonoBehaviour
{
    public bool m_DisablePixelLights = true;
    public int m_TextureResolution = 1024;
    public float m_clipPlaneOffset = 0.07f;
    private float m_finalClipPlaneOffset = 0.0f;
    public bool m_NormalsFromMesh = false;
    public bool m_BaseClipOffsetFromMesh = false;
    public bool m_BaseClipOffsetFromMeshInverted = false;
    private Vector3 m_calculatedNormal = Vector3.zero;

    public LayerMask m_ReflectLayers = -1;

    private Hashtable m_ReflectionCameras = new Hashtable(); //Camera -> Camera table

    private RenderTexture m_ReflectionTexture = null;
    private int m_OldReflectionTextureSize = 0;

    private static bool s_InsideRendering = false;

    private bool firstTime = true;
    public float z = 0;
    public float w = 1;
    public float scalex = 1.0f;
    public float scaley = 1.0f;

    //This is called when it's known that the object will be rendered by some
    //camera. We render reflections and do other updates here.
    //Because the script executes in edit mode, reflections for the scene view
    //camera will just work!
    public void OnWillRenderObject()
    {
        if (!enabled || !renderer || !renderer.sharedMaterial || !renderer.enabled)
            return;

        Camera cam = Camera.current;
        if (!cam)
            return;

        if (m_NormalsFromMesh && GetComponent<MeshFilter>() != null)
            m_calculatedNormal = transform.TransformDirection(GetComponent<MeshFilter>().sharedMesh.normals[0]);

        if (m_BaseClipOffsetFromMesh && GetComponent<MeshFilter>() != null)
            m_finalClipPlaneOffset = (transform.position - transform.TransformPoint(GetComponent<MeshFilter>().sharedMesh.vertices[0])).magnitude + m_clipPlaneOffset;
        else if (m_BaseClipOffsetFromMeshInverted && GetComponent<MeshFilter>() != null)
            m_finalClipPlaneOffset = -(transform.position - transform.TransformPoint(GetComponent<MeshFilter>().sharedMesh.vertices[0])).magnitude + m_clipPlaneOffset;
        else
            m_finalClipPlaneOffset = m_clipPlaneOffset;

        //Safeguard from recursive reflections.        
        if (s_InsideRendering)
            return;
        s_InsideRendering = true;

        Camera reflectionCamera;
        CreateSurfaceObjects(cam, out reflectionCamera);

        //Find out the reflection plane: position and normal in world space
        Vector3 pos = transform.position;
        Vector3 normal = m_NormalsFromMesh && GetComponent<MeshFilter>() != null ? m_calculatedNormal : transform.up;

        //Optionally disable pixel lights for reflection
        int oldPixelLightCount = QualitySettings.pixelLightCount;
        if (m_DisablePixelLights)
            QualitySettings.pixelLightCount = 0;

        reflectionCamera.cullingMask = m_ReflectLayers.value;

        UpdateCameraModes(cam, reflectionCamera);

        //Render reflection
        //Reflect camera around reflection plane
        float d = -Vector3.Dot(normal, pos) - m_finalClipPlaneOffset;
        Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

        Matrix4x4 reflection = Matrix4x4.zero;
        CalculateReflectionMatrix(ref reflection, reflectionPlane);
        Vector3 oldpos = cam.transform.position;
        Vector3 newpos = reflection.MultiplyPoint(oldpos);
        reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;

        //Setup oblique projection matrix so that near plane is our reflection plane.
        //This way we clip everything below/above it for free.
        Vector4 clipPlane = CameraSpacePlane(reflectionCamera, pos, normal, 1.0f);
        Matrix4x4 projection = cam.projectionMatrix;
        CalculateObliqueMatrix(ref projection, clipPlane);
        reflectionCamera.projectionMatrix = projection;

        //reflectionCamera.cullingMask = ~(1 << 4) & m_ReflectLayers.value; //never render water layer
        reflectionCamera.targetTexture = m_ReflectionTexture;
        GL.SetRevertBackfacing(false);
        reflectionCamera.transform.position = newpos;
        Vector3 euler = cam.transform.eulerAngles;
        reflectionCamera.transform.eulerAngles = new Vector3(0, euler.y, euler.z);
        reflectionCamera.Render();
        Texture2D tex = new Texture2D(m_ReflectionTexture.width, m_ReflectionTexture.height, TextureFormat.ARGB32, false);
        RenderTexture.active = m_ReflectionTexture;
        reflectionCamera.transform.position = oldpos;

        tex.ReadPixels(new Rect(0, 0, m_ReflectionTexture.width, m_ReflectionTexture.height), 0, 0);
        tex.Apply(false);

        GL.SetRevertBackfacing(false);
        Material[] materials = renderer.sharedMaterials;


        var planeTex = new Texture2D(m_TextureResolution, m_TextureResolution, TextureFormat.ARGB32, false);
        //Set matrix on the shader that transforms UVs from object space into screen
        //space. We want to just project reflection texture on screen.
        Matrix4x4 scaleOffset = Matrix4x4.TRS(
            new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, new Vector3(0.5f, 0.5f, 0.5f));
        Vector3 scale = transform.lossyScale;
        Matrix4x4 mtx = transform.localToWorldMatrix;// *Matrix4x4.Scale(new Vector3(1.0f / scale.x, 1.0f / scale.y, 1.0f / scale.z));
        mtx = scaleOffset * cam.projectionMatrix * cam.worldToCameraMatrix * mtx;
        {
            var __array1 = materials;
            var __arrayLength1 = __array1.Length;
            for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
            {
                var mat = (Material)__array1[__i1];
                mat.SetMatrix("_ProjMatrix", mtx);
            }
        }
        var mesh = gameObject.GetComponent<MeshFilter>();

        for (int i = 0; i < m_TextureResolution; i++)
        {
            for (int j = 0; j < m_TextureResolution; j++)
            {
                float x = (float)i / m_TextureResolution * 10 - 5;
                float y = (float)j / m_TextureResolution * 10 - 5;

                var uvw = mtx * (new Vector4(x, y, z, w));
                var u = uvw.x / uvw.w;
                var v = uvw.y / uvw.w;
                if (u < 0 || u > 1 || v < 0 || v > 1)
                {
                    planeTex.SetPixel(m_TextureResolution - i, j, Color.clear);
                }
                else
                {
                    var c = tex.GetPixel((int)(u * tex.width), (int)(v * tex.height));
                    if (c.a == 0 && (c.r != 0 || c.g != 0 || c.b != 0))
                    {
                        c.a = 1;
                    }
                    planeTex.SetPixel(m_TextureResolution - i, j, c);
                }
            }
        }

        planeTex.Apply(false);


        {
            if (!Directory.Exists(Application.dataPath + "/WaterReflection"))
            {
                Directory.CreateDirectory(Application.dataPath + "/WaterReflection");
            }
            var bytes = planeTex.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/WaterReflection/" + gameObject.name + "_reflect.png", bytes);
        }

        AssetDatabase.ImportAsset("Assets/WaterReflection/" + gameObject.name + "_reflect.png");
        var texture = AssetDatabase.LoadAssetAtPath("Assets/WaterReflection/" + gameObject.name + "_reflect.png", typeof(Texture2D)) as Texture2D;
        {
            var __array2 = materials;
            var __arrayLength2 = __array2.Length;
            for (int __i2 = 0; __i2 < __arrayLength2; ++__i2)
            {
                var mat = (Material)__array2[__i2];
                {
                    if (mat.HasProperty("_MainTex"))
                        mat.SetTexture("_MainTex", texture);
                }
            }
        }
        //Restore pixel light count
        if (m_DisablePixelLights)
            QualitySettings.pixelLightCount = oldPixelLightCount;

        s_InsideRendering = false;
    }


    //Cleanup all the objects we possibly have created
    void OnDisable()
    {
        if (m_ReflectionTexture)
        {
            DestroyImmediate(m_ReflectionTexture);
            m_ReflectionTexture = null;
        }
        {
            // foreach(var kvp in m_ReflectionCameras)
            var __enumerator3 = (m_ReflectionCameras).GetEnumerator();
            while (__enumerator3.MoveNext())
            {
                var kvp = (DictionaryEntry)__enumerator3.Current;
                DestroyImmediate(((Camera)kvp.Value).gameObject);
            }
        }
        m_ReflectionCameras.Clear();
    }


    private void UpdateCameraModes(Camera src, Camera dest)
    {
        if (dest == null)
            return;
        //set camera to clear the same way as current camera
        dest.clearFlags = CameraClearFlags.Depth;
        dest.backgroundColor = Color.black;
        //update other values to match current camera.
        //even if we are supplying custom camera&projection matrices,
        //some of values are used elsewhere (e.g. skybox uses far plane)
        dest.farClipPlane = src.farClipPlane;
        dest.nearClipPlane = src.nearClipPlane;
        dest.orthographic = src.orthographic;
        dest.fieldOfView = src.fieldOfView;
        dest.aspect = src.aspect;
        dest.orthographicSize = src.orthographicSize;
    }

    //On-demand create any objects we need
    private void CreateSurfaceObjects(Camera currentCamera, out Camera reflectionCamera)
    {
        reflectionCamera = null;

        //Reflection render texture
        if (!m_ReflectionTexture || m_OldReflectionTextureSize != m_TextureResolution)
        {
            if (m_ReflectionTexture)
                DestroyImmediate(m_ReflectionTexture);
            m_ReflectionTexture = new RenderTexture(m_TextureResolution, m_TextureResolution, 32);
            m_ReflectionTexture.name = "__SurfaceReflection" + GetInstanceID();
            m_ReflectionTexture.isPowerOfTwo = true;
            m_ReflectionTexture.hideFlags = HideFlags.DontSave;
            m_OldReflectionTextureSize = m_TextureResolution;
        }

        //Camera for reflection
        reflectionCamera = m_ReflectionCameras[currentCamera] as Camera;
        if (!reflectionCamera) //catch both not-in-dictionary and in-dictionary-but-deleted-GO
        {
            GameObject go = new GameObject("Surface Refl Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof(Camera), typeof(Skybox));
            reflectionCamera = go.camera;
            reflectionCamera.enabled = false;
            reflectionCamera.transform.position = transform.position;
            reflectionCamera.transform.rotation = transform.rotation;
            reflectionCamera.gameObject.AddComponent("FlareLayer");
            go.hideFlags = HideFlags.HideAndDontSave;
            m_ReflectionCameras[currentCamera] = reflectionCamera;
        }
    }

    //Extended sign: returns -1, 0 or 1 based on sign of a
    private static float sgn(float a)
    {
        if (a > 0.0f) return 1.0f;
        if (a < 0.0f) return -1.0f;
        return 0.0f;
    }

    //Given position/normal of the plane, calculates plane in camera space.
    private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
    {
        Vector3 offsetPos = pos + normal * m_finalClipPlaneOffset;
        Matrix4x4 m = cam.worldToCameraMatrix;
        Vector3 cpos = m.MultiplyPoint(offsetPos);
        Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }

    //Adjusts the given projection matrix so that near plane is the given clipPlane
    //clipPlane is given in camera space. See article in Game Programming Gems 5 and
    //http://aras-p.info/texts/obliqueortho.html
    private static void CalculateObliqueMatrix(ref Matrix4x4 projection, Vector4 clipPlane)
    {
        Vector4 q = projection.inverse * new Vector4(
            sgn(clipPlane.x),
            sgn(clipPlane.y),
            1.0f,
            1.0f
            );
        Vector4 c = clipPlane * (2.0F / (Vector4.Dot(clipPlane, q)));
        //third row = clip plane - fourth row
        projection[2] = c.x - projection[3];
        projection[6] = c.y - projection[7];
        projection[10] = c.z - projection[11];
        projection[14] = c.w - projection[15];
    }

    //Calculates reflection matrix around the given plane
    private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
    {
        reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
        reflectionMat.m01 = (-2F * plane[0] * plane[1]);
        reflectionMat.m02 = (-2F * plane[0] * plane[2]);
        reflectionMat.m03 = (-2F * plane[3] * plane[0]);

        reflectionMat.m10 = (-2F * plane[1] * plane[0]);
        reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
        reflectionMat.m12 = (-2F * plane[1] * plane[2]);
        reflectionMat.m13 = (-2F * plane[3] * plane[1]);

        reflectionMat.m20 = (-2F * plane[2] * plane[0]);
        reflectionMat.m21 = (-2F * plane[2] * plane[1]);
        reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
        reflectionMat.m23 = (-2F * plane[3] * plane[2]);

        reflectionMat.m30 = 0F;
        reflectionMat.m31 = 0F;
        reflectionMat.m32 = 0F;
        reflectionMat.m33 = 1F;
    }
}


#endif