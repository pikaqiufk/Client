using System;
#region using

using UnityEngine;

#endregion

public class SelectReminderLogic : MonoBehaviour
{
    private MeshRenderer mMeshRenderer;
    private float mScaleChange;
    private float mScalseFlag = 1.0f;
    private float mScalseSize;
    private float mScalseSizeMax;
    private float mScalseSizeMin;
    private float mTimeFlag;
    private Transform mTrans;

    public void SetColorSize(float size, Color col)
    {
        if (mMeshRenderer == null)
        {
            mMeshRenderer = GetComponent<MeshRenderer>();
            mMeshRenderer.material = new Material(mMeshRenderer.material);
        }
        if (mMeshRenderer)
        {
            mMeshRenderer.material.SetColor("_TintColor", col);
        }
        if (mTrans == null)
        {
            mTrans = transform;
        }
        if (mTrans)
        {
            mTrans.localScale = new Vector3(size, size, 1.0f);
        }
        mScalseSize = size;
        mScalseFlag = size;
        mScalseSizeMax = mScalseSize*1.15f;
        mScalseSizeMin = mScalseSize*0.95f;
        mScaleChange = 0.02f;
    }

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

    private void Update()
    {
#if !UNITY_EDITOR
try
{
#endif

        mTimeFlag += Time.deltaTime;
        if (mTimeFlag < 0.05f)
        {
            return;
        }

        mTimeFlag = 0.0f;
        if (mScalseFlag >= mScalseSizeMax)
        {
            mScaleChange = -0.02f;
        }
        if (mScalseFlag <= mScalseSizeMin)
        {
            mScaleChange = 0.02f;
        }
        mScalseFlag += mScaleChange;
        if (mTrans)
        {
            mTrans.localScale = new Vector3(mScalseFlag, mScalseFlag, 1.0f);
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