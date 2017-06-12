using System;
#region using

using UnityEngine;

#endregion

[RequireComponent(typeof (Renderer))]
public class ModelTextureAnimation : MonoBehaviour
{
    //动画曲线
    [HideInInspector] public AnimationCurve Curve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f),
        new Keyframe(1f, 1f, 1f, 0f));

    //每帧间隔
    public float Duration = 1.0f;
    //当前帧索引
    public int Index;
    private float mDelta;
    private Material mMaterial;
    private Renderer mRenderer;
    //材质主纹理名字
    public string TextureName = "_MainTex";
    private int TotalTitleNum;
    //行数
    public int XTile = 1;
    //列数
    public int YTile = 1;

    private void OnBecameInvisible()
    {
        enabled = false;
    }

    private void OnBecameVisible()
    {
        enabled = true;
    }

    public void OnResetTile()
    {
        TotalTitleNum = XTile*YTile;
        if (null != mMaterial)
        {
            mMaterial.SetTextureScale(TextureName, new Vector2(1.0f/XTile, 1.0f/YTile));
        }
    }

    //根据帧数算出Offset
    private void SetTextureOffset()
    {
        var x = (1.0f/XTile)*(Index%XTile);
        var y = 1 - (1.0f/YTile)*(Index/XTile);
        if (null != mMaterial)
        {
            mMaterial.SetTextureOffset(TextureName, new Vector2(x, y));
        }
    }

    // Use this for initialization
    [ExecuteInEditMode]
    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif

        mRenderer = renderer;
        mMaterial = mRenderer.material;
        if (null == mMaterial)
        {
            Logger.Error("null==mMaterial");
            return;
        }
        OnResetTile();
        SetTextureOffset();
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    // Update is called once per frame
    private void Update()
    {
#if !UNITY_EDITOR
try
{
#endif
        mDelta += Time.deltaTime;
        var rate = Duration/TotalTitleNum;
        if (mDelta >= rate)
        {
            mDelta = 0;
            Index ++;
            Index %= TotalTitleNum;
            SetTextureOffset();
        }

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }
}