using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Camera))]
public class SENaturalBloomPlus : MonoBehaviour
{
	[Range(0.0f, 10.4f)]
	public float bloomIntensity = 0.05f;

	public Material material;

	private bool isSupported;

	private float blurSize = 4.0f;

	public bool inputIsHDR;

    private Camera mainCamera;
    private Camera distortionCamera;

    public RenderTexture mainRenderTexture;
    public RenderTexture secondRenderTexture;

    public RenderBuffer[] mBuffers;

    public RenderTexture aRenderTexture;

	void Start() 
	{
        isSupported = true;
        if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures || !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.R8) || SystemInfo.supportedRenderTargetCount < 2)
        {
            isSupported = false;
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

	    mainRenderTexture = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.Default);
        secondRenderTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.R8);

        //aRenderTexture = new RenderTexture(Screen.width / 2, Screen.height / 2, 0, RenderTextureFormat.Default);

	    mainCamera = gameObject.GetComponent<Camera>();
	    mBuffers = new[] {mainRenderTexture.colorBuffer, secondRenderTexture.colorBuffer};
	    mainCamera.SetTargetBuffers(mBuffers, mainRenderTexture.depthBuffer);
        mainCamera.backgroundColor = Color.black;

	    mainCamera.hdr = true;
        mainCamera.cullingMask -= LayerMask.GetMask("Distortion");
        bloomIntensity = 1.5f;

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

    void Update()
    {
        if (!isSupported)
        {
            return;
        }

        if (mainRenderTexture.width != Screen.width || mainRenderTexture.height != Screen.height)
        {
            Destroy(mainRenderTexture);
            Destroy(secondRenderTexture);

            mainRenderTexture = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.Default);
            secondRenderTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.R8);

            mBuffers = new[] { mainRenderTexture.colorBuffer, secondRenderTexture.colorBuffer };
            mainCamera.SetTargetBuffers(mBuffers, mainRenderTexture.depthBuffer);
        }

        Graphics.Blit(Texture2D.blackTexture, secondRenderTexture);
    }

    void OnDestroy()
    {
        if (!isSupported)
        {
            return;
        }

        if (mainRenderTexture)
        {
            Destroy(mainRenderTexture);
        }

        if (secondRenderTexture)
        {
            Destroy(secondRenderTexture);
        }
    }

    void OnPostRender()
    {
        if (!isSupported || !material)
        {
            return;
        }

        var source = mainRenderTexture;

        RenderTexture distortionRenderTexture = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.R8);

        RenderBuffer colorBuffer = Graphics.activeColorBuffer;
        distortionCamera.SetTargetBuffers(distortionRenderTexture.colorBuffer, Graphics.activeDepthBuffer);
        Graphics.SetRenderTarget(distortionRenderTexture.colorBuffer, Graphics.activeDepthBuffer);
        GL.Clear(false, true, Color.black);
        distortionCamera.Render();
        Graphics.SetRenderTarget(colorBuffer, Graphics.activeDepthBuffer);

#if UNITY_EDITOR
        if (source.format == RenderTextureFormat.ARGBHalf)
            inputIsHDR = true;
        else
            inputIsHDR = false;
#endif

        material.SetFloat("_BloomIntensity", Mathf.Exp(bloomIntensity) - 1.0f);
        source.filterMode = FilterMode.Bilinear;

        int rtWidth = source.width / 2;
        int rtHeight = source.height / 2;

        RenderTexture downsampled;
        downsampled = source;

        float spread = 1.0f;
        int iterations = 1;

        for (int i = 0; i < 4; i++)
        {
            RenderTexture rt = RenderTexture.GetTemporary(rtWidth, rtHeight, 0, source.format);
            rt.filterMode = FilterMode.Bilinear;

            if (i == 0)
            {
                material.SetTexture("_HighLight", secondRenderTexture);
                Graphics.Blit(downsampled, rt, material, 4);
            }
            else
            {
                Graphics.Blit(downsampled, rt, material, 1);
            }

            downsampled = rt;

            if (i > 1)
                spread = 1.0f;
            else
                spread = 0.5f;

            if (i == 2)
                spread = 0.75f;


            for (int j = 0; j < iterations; j++)
            {
                material.SetFloat("_BlurSize", (blurSize * 0.5f + j) * spread);

                //vertical blur
                RenderTexture rt2 = RenderTexture.GetTemporary(rtWidth, rtHeight, 0, source.format);
                rt2.filterMode = FilterMode.Bilinear;
                Graphics.Blit(rt, rt2, material, 2);
                RenderTexture.ReleaseTemporary(rt);
                rt = rt2;

                rt2 = RenderTexture.GetTemporary(rtWidth, rtHeight, 0, source.format);
                rt2.filterMode = FilterMode.Bilinear;
                Graphics.Blit(rt, rt2, material, 3);
                RenderTexture.ReleaseTemporary(rt);
                rt = rt2;
            }

            switch (i)
            {
                case 0:
                    material.SetTexture("_Bloom0", rt);
                    aRenderTexture = rt;
                    break;
                case 1:
                    material.SetTexture("_Bloom1", rt);
                    break;
                case 2:
                    material.SetTexture("_Bloom2", rt);
                    break;
                case 3:
                    material.SetTexture("_Bloom3", rt);
                    break;
                case 4:
                    material.SetTexture("_Bloom4", rt);
                    break;
                case 5:
                    material.SetTexture("_Bloom5", rt);
                    break;
                default:
                    break;
            }

            RenderTexture.ReleaseTemporary(rt);

            rtWidth /= 2;
            rtHeight /= 2;
        }

        source.filterMode = FilterMode.Point;
        material.SetTexture("_Distortion", distortionRenderTexture);

        Graphics.SetRenderTarget(Display.main.colorBuffer, Display.main.depthBuffer);
        Graphics.Blit(source, null, material, 0);

        RenderTexture.ReleaseTemporary(distortionRenderTexture);


    }
}