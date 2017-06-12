#region using

using System;
using UnityEngine;

#endregion

[RequireComponent(typeof (UIPanel))]
public class ParticleSystemClipper : MonoBehaviour
{
    private const float ClipInterval = 1f;
    private const string ShaderName = "Bleach/Particles Additive Area Clip";
    private Vector4 clipArea;
    private Shader m_shader;
    private UIPanel m_targetPanel;

    private Vector4 CalcClipArea()
    {
        var nguiArea = new Vector4();
        var clipRegion = m_targetPanel.finalClipRegion;
        nguiArea.x = clipRegion.x - clipRegion.z/2;
        nguiArea.y = clipRegion.y - clipRegion.w/2;
        nguiArea.z = clipRegion.x + clipRegion.z/2;
        nguiArea.w = clipRegion.y + clipRegion.w/2;
        UIRoot uiRoot = null;
        if (UIManager.Instance != null && UIManager.Instance.UIRoot != null)
        {
            uiRoot = UIManager.Instance.UIRoot.GetComponent<UIRoot>();
        }
        if (uiRoot == null)
        {
            uiRoot = NGUITools.FindInParents<UIRoot>(transform);
        }
        if (uiRoot == null)
        {
            return Vector4.zero;
        }
        var ah = (float) uiRoot.activeHeight;
        var s = (float) uiRoot.activeHeight/uiRoot.manualHeight;
        var h = 2/s;
        var w = h;
        //float h = 2 ;
        //float w = h * Screen.width / Screen.height ;

        var clipArea = new Vector4();
        var pos = m_targetPanel.transform.position - uiRoot.transform.position;
        clipArea.x = pos.x + nguiArea.x*w/uiRoot.manualWidth;
        clipArea.y = pos.y + nguiArea.y*h/uiRoot.manualHeight;
        clipArea.z = pos.x + nguiArea.z*w/uiRoot.manualWidth;
        clipArea.w = pos.y + nguiArea.w*h/uiRoot.manualHeight;


        return clipArea;
    }

    private void Clip()
    {
        if (enabled == false)
        {
            return;
        }
        var particleSystems = GetComponentsInChildren<ParticleSystem>();

        for (var i = 0; i < particleSystems.Length; i++)
        {
            var ps = particleSystems[i];
            var mat = ps.renderer.material;

            if (mat.shader.name != ShaderName)
            {
                mat.shader = m_shader;
            }

            mat.SetVector("_Area", clipArea);
        }
    }

    private void OnDisable()
    {
#if !UNITY_EDITOR
try
{
#endif

        CancelInvoke("Clip");
    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

    protected void OnEnable()
    {
#if !UNITY_EDITOR
try
{
#endif

        if (!IsInvoking("Clip"))
        {
            InvokeRepeating("Clip", 0, ClipInterval);
        }
    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif

        // find panel
        m_targetPanel = GetComponent<UIPanel>();

        if (m_targetPanel == null)
        {
            throw new ArgumentNullException("Cann't find the right UIPanel");
        }
        if (m_targetPanel.clipping != UIDrawCall.Clipping.SoftClip)
        {
            throw new InvalidOperationException("Don't need to clip");
        }

        m_shader = Shader.Find(ShaderName);

        clipArea = CalcClipArea();
    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}
}