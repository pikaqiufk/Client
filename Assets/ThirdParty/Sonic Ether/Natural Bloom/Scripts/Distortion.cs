using UnityEngine;

[RequireComponent(typeof (Camera))]
public class Distortion : MonoBehaviour
{
    public Material material;

    private bool isSupported;
    private Camera mainCamera;
    private Camera distortionCamera;

    private RenderTextureFormat format = RenderTextureFormat.R8;

    void Start()
    {
        isSupported = true;
        if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures || !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.R8))
        {
            isSupported = false;
        }

        if (!SystemInfo.SupportsRenderTextureFormat(format))
        {
            format = RenderTextureFormat.Default;
        }

#if UNITY_EDITOR
        if (!material)
        {
            ResourceManager.PrepareResource<Material>(Resource.Material.BloomMaterial, mat =>
            {
                material = new Material(mat);
                material.shader = Shader.Find(material.shader.name);
            });
        }
#else

	    if (!material)
	    {
	        isSupported = false;
	    }
#endif
        if (!isSupported)
        {
            return;
        }

#if UNITY_EDITOR
        if (material)
            material.shader = Shader.Find(material.shader.name);
#endif

        mainCamera = gameObject.GetComponent<Camera>();

        mainCamera.cullingMask &= (int.MaxValue - LayerMask.GetMask("Distortion"));

        var child = gameObject.transform.FindChild("DistortionCamera");
        if (!child)
        {
            var o = new GameObject("DistortionCamera");
            distortionCamera = o.AddComponent<Camera>();

            o.transform.parent = mainCamera.transform;
            o.transform.localPosition = Vector3.zero;
            o.transform.localRotation = Quaternion.Euler(0, 0, 0);
            o.transform.localScale = Vector3.one;

            distortionCamera.cullingMask = LayerMask.GetMask("Distortion");
            distortionCamera.clearFlags = CameraClearFlags.Nothing;
            distortionCamera.backgroundColor = Color.black;
            distortionCamera.fieldOfView = mainCamera.fieldOfView;
            distortionCamera.nearClipPlane = mainCamera.nearClipPlane;
            distortionCamera.farClipPlane = mainCamera.farClipPlane;
            o.SetActive(false);
        }
        else
        {
            child.gameObject.SetActive(false);
        }

    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        RenderTexture distortionRenderTexture = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.R8);

        RenderBuffer colorBuffer = Graphics.activeColorBuffer;
        distortionCamera.SetTargetBuffers(distortionRenderTexture.colorBuffer, Graphics.activeDepthBuffer);
        Graphics.SetRenderTarget(distortionRenderTexture.colorBuffer, Graphics.activeDepthBuffer);
        GL.Clear(false, true, Color.black);
        distortionCamera.Render();
        Graphics.SetRenderTarget(colorBuffer, Graphics.activeDepthBuffer);

        Graphics.Blit(source, destination, material, 4);

        source.filterMode = FilterMode.Point;
        material.SetTexture("_Distortion", distortionRenderTexture);

        RenderTexture.ReleaseTemporary(distortionRenderTexture);
    }
}