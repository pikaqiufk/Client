using System;
using UnityEngine;
using System.Collections;
using Thinksquirrel.Utilities;

[ExecuteInEditMode]
public class FightCamera : MonoBehaviour
{
    public float Height = 1;
    public float Lenght = 10;

    public float Speed = 0.02f;

    private float Angle = -45;
    private float FAngle = 45;

    private Vector3 Offset;
    private Vector3 LookAtOffset;

    public GameObject mFollowObj;

    private CameraShake mShake;

    public static bool TestMode = false;

    public GameObject FollowObj
    {
        get { return mFollowObj; }
        set
        {
            mFollowObj = value;
            var vecLookAt = mFollowObj.transform.position + LookAtOffset;
            var vecPosition = vecLookAt + Offset;
            gameObject.transform.position = vecPosition;
            gameObject.transform.LookAt(vecLookAt);
            gameObject.transform.localRotation = Quaternion.Euler(FAngle, Angle, 0);
            gameObject.camera.fieldOfView = 40;
        }
    }

    void Awake()
    {
        TestMode = true;
    }

    // Use this for initialization
    void Start()
    {

		var uiroot = GameObject.Find("UIRoot");
		if (uiroot != null)
		{
			uiroot.SetActive(false);
		}

		uiroot = GameObject.Find("UI Root");
		if (uiroot != null)
		{
			uiroot.SetActive(false);
		}
		
		var sceenRoot = GameObject.Find("SceneRoot");
		if (sceenRoot != null)
		{
			sceenRoot.SetActive(false);
		}
		
		var teleport = GameObject.Find("Teleport");
		if (teleport != null)
		{
			teleport.SetActive(false);
		}

		var mainCamera = GameObject.Find("Main Camera");
		if (mainCamera != null)
		{
			mainCamera.SetActive(false);
		}

        Offset = new Vector3(Mathf.Cos(Mathf.Deg2Rad * Angle) * Mathf.Cos(Mathf.Deg2Rad * FAngle), Mathf.Sin(Mathf.Deg2Rad * FAngle),
            -Mathf.Cos(Mathf.Deg2Rad * Angle) * Mathf.Cos(Mathf.Deg2Rad * FAngle)) * Lenght;
        LookAtOffset = new Vector3(0, Height, 0);

        mShake = gameObject.GetComponent<CameraShake>();

        FollowObj = mFollowObj;
    }

    // Update is called once per frame
    void Update()
    {
        float step = 50 * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            mFollowObj.transform.Translate(transform.right * -Speed, Space.World);
            Vector3 newDir = Vector3.RotateTowards(mFollowObj.transform.forward, -transform.right, step, 0.0F);
            mFollowObj.transform.rotation = Quaternion.LookRotation(newDir);
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            mFollowObj.transform.Translate(transform.right * Speed, Space.World);
            Vector3 newDir = Vector3.RotateTowards(mFollowObj.transform.forward, transform.right, step, 0.0F);
            mFollowObj.transform.rotation = Quaternion.LookRotation(newDir);
        }
        else if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            mFollowObj.transform.Translate(transform.forward * Speed, Space.World);
            var camDir = transform.forward;
            camDir.y = 0;
            Vector3 newDir = Vector3.RotateTowards(mFollowObj.transform.forward, camDir, step, 0.0F);
            mFollowObj.transform.rotation = Quaternion.LookRotation(newDir);
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            mFollowObj.transform.Translate(transform.forward * -Speed, Space.World);
            var camDir = transform.forward;
            camDir.y = 0;
            Vector3 newDir = Vector3.RotateTowards(mFollowObj.transform.forward, -camDir, step, 0.0F);
            mFollowObj.transform.rotation = Quaternion.LookRotation(newDir);
        }
    }

    void LateUpdate()
    {
        if (FollowObj)
        {

#if UNITY_EDITOR
            // 动态调整用，需要调摄像机的时候，可以使用
            Offset = new Vector3(Mathf.Cos(Mathf.Deg2Rad * Angle) * Mathf.Cos(Mathf.Deg2Rad * FAngle), Mathf.Sin(Mathf.Deg2Rad * FAngle),
            -Mathf.Cos(Mathf.Deg2Rad * Angle) * Mathf.Cos(Mathf.Deg2Rad * FAngle)) * Lenght;
            LookAtOffset = new Vector3(0, Height, 0);
#endif

            var vecLookAt = mFollowObj.transform.position + LookAtOffset;
            var vecPosition = vecLookAt + Offset;

            var rot = GetComponent<CameraShake>().shakeRotationOffset.eulerAngles;
            gameObject.transform.position = vecPosition + mShake.shakePositionOffset;
            gameObject.transform.localRotation = Quaternion.Euler(FAngle + rot.x, Angle + rot.y, rot.z);

            
#if UNITY_EDITOR
            //gameObject.transform.LookAt(vecLookAt);
#endif
        }

    }
}
