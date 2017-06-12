using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlurTextureCreator : MonoBehaviour
{
    private bool init = false;
    public Material BlurMaterial;
    private bool isSupported;
    private float blurSize = 1.2013f;
    private float[] Intensity = new float[] { 1.0f, 1.1f, 1.5f, 2.0f, 2.0f };

    public static RenderTexture BlurredTexture
    {
        get
        {
            var blur = UICamera.currentCamera.GetComponent<BlurTextureCreator>();
            if (blur == null)
            {
                blur = UICamera.currentCamera.gameObject.AddComponent<BlurTextureCreator>();
            }

            if (!blur.init)
            {
                blur.isSupported = SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures;
#if UNITY_EDITOR
                if (!blur.BlurMaterial)
                {
                    ResourceManager.PrepareResource<Material>(Resource.Material.BlurMaterial, mat =>
                    {
                        blur.BlurMaterial = new Material(mat);
                        blur.BlurMaterial.shader = Shader.Find(blur.BlurMaterial.shader.name);
                    });
                }
#else

                if (!blur.BlurMaterial)
                {
                    blur.isSupported = false;
                }
#endif

#if UNITY_EDITOR
                if (blur.BlurMaterial)
                    blur.BlurMaterial.shader = Shader.Find(blur.BlurMaterial.shader.name);
#endif

                blur.init = true;
            }


            if (!blur.isSupported || !blur.BlurMaterial)
            {
                return null;
            }

            int rtWidth = (int) (NGUITools.screenSize.x/4);
            int rtHeight = (int) (NGUITools.screenSize.y/4);

            RenderTexture downsampled = RenderTexture.GetTemporary(rtWidth, rtHeight, 8, RenderTextureFormat.Default);
            Graphics.SetRenderTarget(downsampled.colorBuffer, downsampled.depthBuffer);
            GL.Clear(true, true, Color.black);

	        Camera mainCamera = null;
	        if (GameLogic.Instance != null)
	        {
		        mainCamera = GameLogic.Instance.MainCamera;
	        }
	        if (null == mainCamera)
	        {
		        mainCamera = Camera.main;
	        }
            if ( null!=mainCamera)
            {
                var bloom = mainCamera.GetComponent<DistortionAndBloom>();
                var post = true;
                if (bloom)
                {
                    post = bloom.enabled;
                    bloom.enabled = false;
                }
                mainCamera.targetTexture = downsampled;
				mainCamera.Render();
                mainCamera.targetTexture = null;
                if (bloom)
                {
                    bloom.enabled = post;
                }
            }

            UICamera.currentCamera.targetTexture = downsampled;
            UICamera.currentCamera.Render();
            UICamera.currentCamera.targetTexture = null;

            downsampled.filterMode = FilterMode.Bilinear;
            
            float spread = 1.0f;
            int iterations = 1;

            RenderTexture rt = null;
            for (int i = 0; i < 2; i++)
            {
                if (i != 0)
                {
                    downsampled = RenderTexture.GetTemporary(rtWidth, rtHeight, 0, RenderTextureFormat.Default);
                    downsampled.filterMode = FilterMode.Bilinear;

                    Graphics.Blit(rt, downsampled, blur.BlurMaterial, 5);
                    RenderTexture.ReleaseTemporary(rt);
                }
            
                rt = downsampled;

                for (int j = 0; j < iterations; j++)
                {
                    blur.BlurMaterial.SetFloat("_BlurSize", blur.blurSize);
                    blur.BlurMaterial.SetFloat("_BlurIntensity", blur.Intensity[i]);

                    //vertical blur
                    RenderTexture rt2 = RenderTexture.GetTemporary(rtWidth, rtHeight, 0, RenderTextureFormat.Default);
                    rt2.filterMode = FilterMode.Bilinear;
                    Graphics.Blit(rt, rt2, blur.BlurMaterial, 2);
                    RenderTexture.ReleaseTemporary(rt);
                    rt = rt2;

                    rt2 = RenderTexture.GetTemporary(rtWidth, rtHeight, 0, RenderTextureFormat.Default);
                    rt2.filterMode = FilterMode.Bilinear;
                    Graphics.Blit(rt, rt2, blur.BlurMaterial, 3);
                    RenderTexture.ReleaseTemporary(rt);
                    rt = rt2;
                }

                switch (i)
                {
                    case 0:
                        blur.BlurMaterial.SetTexture("_Bloom0", rt);
                        break;
                    case 1:
                        blur.BlurMaterial.SetTexture("_Bloom1", rt);
                        break;
                    default:
                        break;
                }

                rtWidth /= 2;
                rtHeight /= 2;
            }

            return rt;
        }
    }
}
