using System;
#region using

using UnityEngine;

#endregion

public class Rotate : MonoBehaviour
{
    private Transform mTransform;
    public Vector3 Speed;

    private void Awake()
    {
#if !UNITY_EDITOR
try
{
#endif

        mTransform = transform;

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

        mTransform.Rotate(Speed*Time.fixedDeltaTime);

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }
}