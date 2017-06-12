using System;
#region using

using UnityEngine;

#endregion

[RequireComponent(typeof (Camera))]
public class UIModelCamera : MonoBehaviour
{
    public int Depth = 16;
    public int Height = 256;
    //观看模型所在位置的偏移
    public Vector3 LookAtOffset;
    private Camera mCamera;
    //所要观看的模型
    public GameObject Model;
    private RenderTexture mRenderTexture;
    //距离模型所在位置的偏移
    public Vector3 Offset;
    //哪个UITexture要使用这个相机所渲染的Render Texture
    public UITexture Texture;
    public bool UseLookAtOffset = true;
    public bool UseRenderTextrue = true;
    //Render Texture属性
    public int Width = 256;

    private void Awake()
    {
#if !UNITY_EDITOR
try
{
#endif

        mCamera = gameObject.GetComponent<Camera>();
        if (UseRenderTextrue)
        {
            mRenderTexture = new RenderTexture(Width, Height, Depth);
            mCamera.targetTexture = mRenderTexture;
        }

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    public RenderTexture GetRenderTexture()
    {
        return mRenderTexture;
    }

    private void OnDestroy()
    {
#if !UNITY_EDITOR
try
{
#endif

        Destroy(mRenderTexture);

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    //设置所有观看的模型
    public void SetModel(GameObject obj)
    {
        Model = obj;
        if (null != Model)
        {
            var objTransform = gameObject.transform;
            var modelTransform = Model.transform;
            objTransform.position = modelTransform.position + Offset;
            if (UseLookAtOffset)
            {
                objTransform.forward = modelTransform.position + LookAtOffset - objTransform.position;
            }
            else
            {
                objTransform.forward = -modelTransform.forward;
            }
            //mCamera.cullingMask = 1<<Model.layer;
            if (null != Texture)
            {
                Texture.gameObject.SetActive(true);
            }
        }
        else
        {
            if (null != Texture)
            {
                Texture.gameObject.SetActive(false);
            }
        }
    }

    //
    public void SetTexture(UITexture texture)
    {
        Texture = texture;
        if (null != Texture)
        {
            Texture.mainTexture = GetRenderTexture();
        }
    }

    // Use this for initialization
    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif


        SetModel(Model);
        SetTexture(Texture);

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }
}