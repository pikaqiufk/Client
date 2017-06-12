using System;
#region using

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

public class CGBurnout : ICGPlayable
{
    private IEnumerator BurnoutCoroutine(List<SkinnedMeshRenderer> renderers, float time)
    {
        var totalTime = time;
        while (time > 0)
        {
            time -= Time.deltaTime;
            var x = (totalTime - time)/totalTime;
            foreach (var renderer in renderers)
            {
                renderer.material.SetFloat("_Cutoff", x*x*x);
            }
            yield return null;
        }
        gameObject.SetActive(false);
    }

    public override void Play(float time)
    {
        ResourceManager.PrepareResource<Material>(Resource.Dir.Material + "Dead.mat", material =>
        {
            var list = new List<SkinnedMeshRenderer>();
            GetComponentsInChildren(list);
            var renderers = list;
            if (renderers.Count > 0)
            {
                var __array5 = renderers;
                var __arrayLength5 = __array5.Count;
                for (var __i5 = 0; __i5 < __arrayLength5; ++__i5)
                {
                    var skinnedMeshRenderer = __array5[__i5];
                    {
                        var texture = skinnedMeshRenderer.sharedMaterial.GetTexture("_MainTex");
                        var mat = new Material(material);
                        mat.SetTexture("_MainTex", texture);
                        skinnedMeshRenderer.material = mat;
                    }
                }
                StartCoroutine(BurnoutCoroutine(renderers, time));
            }
        });
    }

    // Use this for initialization
    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif


#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    public override void Stop()
    {
    }
}