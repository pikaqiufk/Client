using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(Camera))]
public class DistortionAndBloom : MonoBehaviour
{
    [Range(0.0f, 0.4f)]
    public float bloomIntensity = 0.02f;

    public Material material;

    private bool isSupported;

    private float blurSize = 1.2013f;

    public float Brightnesss = 0.3f;

    private Camera mainCamera;
    private Camera distortionCamera;

    public List<float> Intensity = new List<float>();

    private RenderTextureFormat format = RenderTextureFormat.R8;

    public bool Blur
    {
        set
        {
            if (value)
            {
                mBlur = value;
                contrast = 0;
            }
            else
            {
                mBlur = value;
                contrast = Brightnesss;
            }
        }
    }

    private bool mBlur = false;
    private float contrast = 0.3f;

    void Start()
    {
        isSupported = true;
        if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
        {
            isSupported = false;
        }


        format = GetSmallestRenderTextureFormat();


        Intensity.Add(1.0f);
        Intensity.Add(1.1f);
        Intensity.Add(1.5f);
        Intensity.Add(2.0f);
        Intensity.Add(2.0f);

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

#if UNITY_EDITOR
        mainCamera.hdr = true;
#else
	    mainCamera.hdr = GameSetting.Instance.EnableHDR;
#endif
        mainCamera.cullingMask &= (int.MaxValue - LayerMask.GetMask("Distortion"));
        bloomIntensity = 0.02f;

        var child = gameObject.transform.FindChild("DistortionCamera");
        if (!child)
        {
            var o = new GameObject("DistortionCamera");
            distortionCamera = o.AddComponent<Camera>();

            o.transform.parent = mainCamera.transform;
            o.transform.localPosition = Vector3.zero;
            o.transform.localRotation = Quaternion.Euler(0, 0, 0);
            o.transform.localScale = Vector3.one;

            o.SetActive(false);
        }
        else
        {
            child.gameObject.SetActive(false);
            distortionCamera = child.GetComponent<Camera>();
        }

        distortionCamera.cullingMask = LayerMask.GetMask("Distortion");
        distortionCamera.clearFlags = CameraClearFlags.Nothing;
        distortionCamera.backgroundColor = Color.black;
        distortionCamera.fieldOfView = mainCamera.fieldOfView;
        distortionCamera.nearClipPlane = mainCamera.nearClipPlane;
        distortionCamera.farClipPlane = mainCamera.farClipPlane;

        if (GameLogic.Instance && GameLogic.Instance.Scene)
        {
            Brightnesss = GameLogic.Instance.Scene.Brightness;
            contrast = Brightnesss;
        }
    }

    public static RenderTextureFormat GetSmallestRenderTextureFormat()
    {
        if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.R8))
        {
            return RenderTextureFormat.R8;
        }
        else if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RHalf))
        {
            return RenderTextureFormat.RHalf;
        }
        else if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RFloat))
        {
            return RenderTextureFormat.RFloat;
        }
        else if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RInt))
        {
            return RenderTextureFormat.RInt;
        }
        else if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
        {
            return RenderTextureFormat.ARGBHalf;
        }
        else
        {
            return RenderTextureFormat.Default;
        }
    }

    [ImageEffectTransformsToLDR]
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!isSupported || !material)
        {
            return;
        }

        RenderTexture distortionRenderTexture = RenderTexture.GetTemporary(source.width, source.height, 0, format);

        RenderBuffer colorBuffer = Graphics.activeColorBuffer;
        distortionCamera.SetTargetBuffers(distortionRenderTexture.colorBuffer, Graphics.activeDepthBuffer);
        Graphics.SetRenderTarget(distortionRenderTexture.colorBuffer, Graphics.activeDepthBuffer);
        GL.Clear(false, true, Color.grey);
        distortionCamera.Render();
        Graphics.SetRenderTarget(colorBuffer, Graphics.activeDepthBuffer);

        source.filterMode = FilterMode.Bilinear;

        int rtWidth = source.width / 4;
        int rtHeight = source.height / 4;

        RenderTexture downsampled;
        downsampled = source;

        float spread = 1.0f;
        int iterations = 1;

        RenderTexture blur1 = null, blur2 = null;

        //float[] intensity = {1.0f, 4, 4, 4, 4};
        for (int i = 0; i < 2; i++)
        {
            RenderTexture rt = RenderTexture.GetTemporary(rtWidth, rtHeight, 0, RenderTextureFormat.Default);
            rt.filterMode = FilterMode.Bilinear;

            material.SetFloat("_Contrast", contrast);
            Graphics.Blit(downsampled, rt, material, i == 0 ? 1 : 5);

            if (i != 0)
                RenderTexture.ReleaseTemporary(downsampled);

            downsampled = rt;

            for (int j = 0; j < iterations; j++)
            {
                material.SetFloat("_BlurSize", blurSize);
                material.SetFloat("_BlurIntensity", Intensity[i]);

                //vertical blur
                RenderTexture rt2 = RenderTexture.GetTemporary(rtWidth, rtHeight, 0, RenderTextureFormat.Default);
                rt2.filterMode = FilterMode.Bilinear;
                Graphics.Blit(rt, rt2, material, 2);
                rt = rt2;

                rt2 = RenderTexture.GetTemporary(rtWidth, rtHeight, 0, RenderTextureFormat.Default);
                rt2.filterMode = FilterMode.Bilinear;
                Graphics.Blit(rt, rt2, material, 3);
                RenderTexture.ReleaseTemporary(rt);
                rt = rt2;
            }

            switch (i)
            {
                case 0:
                    material.SetTexture("_Bloom0", rt);
                    blur1 = rt;
                    break;
                case 1:
                    material.SetTexture("_Bloom1", rt);
                    blur2 = rt;
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

            rtWidth /= 4;
            rtHeight /= 4;
        }

        RenderTexture.ReleaseTemporary(downsampled);

        source.filterMode = FilterMode.Point;
        material.SetFloat("_BloomIntensity", (Mathf.Exp(bloomIntensity) - 1.0f) * 10.0f);
        material.SetTexture("_Distortion", distortionRenderTexture);
        material.SetTexture("_Bloom0", blur1);
        material.SetTexture("_Bloom1", blur2);

        if (mBlur)
        {
            Graphics.Blit(source, destination, material, 6);
        }
        else
        {
            Graphics.Blit(source, destination, material, 0);
        }

        RenderTexture.ReleaseTemporary(distortionRenderTexture);
        if (blur1 != null)
            RenderTexture.ReleaseTemporary(blur1);
        if (blur2 != null)
            RenderTexture.ReleaseTemporary(blur2);
    }
}