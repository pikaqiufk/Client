using System;
#region using

using Thinksquirrel.Utilities;
using UnityEngine;

#endregion

public class CameraController : MonoBehaviour
{
    private readonly float Angle = -45;
    public bool Dirty = true;
    public float FAngle = 35;
    public float Height = 1;
    public float Length = 10;
    private Vector3 LookAtOffset;
    public float MaxLength = 15;
    public GameObject mFollowObj;
    private Transform mFollowTransform;
    public float MinLength = 5;
    private CameraShake mShake;
    private Transform mTransform;
    private Vector3 Offset;
    public bool SmoothMove;
    public float ZoomSpeed = -0.03f;

    public GameObject FollowObj
    {
        get { return mFollowObj; }
        set
        {
            mFollowObj = value;
            mFollowTransform = mFollowObj.transform;
            var vecLookAt = mFollowTransform.position + LookAtOffset;
            var vecPosition = vecLookAt + Offset;
            if (mTransform == null)
            {
                Logger.Error("CameraController Transform is null");
                return;
            }
            mTransform.position = vecPosition;
            mTransform.LookAt(vecLookAt);
            mTransform.localRotation = Quaternion.Euler(FAngle, Angle, 0);
            gameObject.camera.fieldOfView = 40;
            var child = gameObject.transform.FindChild("DistortionCamera");
            if (child && child.camera)
            {
                var distortionCamera = child.camera;
                var mainCamera = gameObject.camera;
                distortionCamera.cullingMask = LayerMask.GetMask("Distortion");
                distortionCamera.clearFlags = CameraClearFlags.Nothing;
                distortionCamera.backgroundColor = Color.black;
                distortionCamera.fieldOfView = mainCamera.fieldOfView;
                distortionCamera.nearClipPlane = mainCamera.nearClipPlane;
                distortionCamera.farClipPlane = mainCamera.farClipPlane;
            }
        }
    }

    private void Awake()
    {
#if !UNITY_EDITOR
try
{
#endif

        mTransform = gameObject.transform;

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    private void LateUpdate()
    {
#if !UNITY_EDITOR
try
{
#endif

        if (FollowObj)
        {
            if (Dirty)
            {
                // 动态调整用，需要调摄像机的时候，可以使用
                Offset = new Vector3(Mathf.Cos(Mathf.Deg2Rad*Angle)*Mathf.Cos(Mathf.Deg2Rad*FAngle),
                    Mathf.Sin(Mathf.Deg2Rad*FAngle),
                    -Mathf.Cos(Mathf.Deg2Rad*Angle)*Mathf.Cos(Mathf.Deg2Rad*FAngle))*Length;
                LookAtOffset = new Vector3(0, Height, 0);
                Dirty = false;
            }

            var vecLookAt = mFollowTransform.position + LookAtOffset;
            var vecPosition = vecLookAt + Offset;

            if (SmoothMove)
            {
                mTransform.position = Vector3.Lerp(mTransform.position, vecPosition, Time.deltaTime*10);
                if ((mTransform.position - vecPosition).sqrMagnitude < 0.1)
                {
                    SmoothMove = false;
                }
            }
            else
            {
                var rot = mShake.shakeRotationOffset.eulerAngles;
                mTransform.position = vecPosition + mShake.shakePositionOffset;
                mTransform.localRotation = Quaternion.Euler(FAngle + rot.x, Angle + rot.y, rot.z);
            }

#if UNITY_EDITOR
            //gameObject.transform.LookAt(vecLookAt);
#endif
        }


#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    // Use this for initialization
    public void Start()
    {
#if !UNITY_EDITOR
try
{
#endif

        Height = GameSetting.Instance.Height;
        Length = GameSetting.Instance.Length;

        Offset = new Vector3(Mathf.Cos(Mathf.Deg2Rad*Angle)*Mathf.Cos(Mathf.Deg2Rad*FAngle),
            Mathf.Sin(Mathf.Deg2Rad*FAngle),
            -Mathf.Cos(Mathf.Deg2Rad*Angle)*Mathf.Cos(Mathf.Deg2Rad*FAngle))*Length;
        LookAtOffset = new Vector3(0, Height, 0);

        mShake = gameObject.GetComponent<CameraShake>();


#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }
}