#region using

using System;
using System.Collections;
using System.Collections.Generic;
using DataTable;
using EventSystem;
using Thinksquirrel.Utilities;
using UnityEngine;

#endregion

#if UNITY_EDITOR
[RequireComponent(typeof (MeshRenderer))]
#endif
public class SceneAnimationTrigger : MonoBehaviour
{
    public Vector3 CameraPos;
    public List<CameraShakeKeyFrame> CameraShakeKeyFrames = new List<CameraShakeKeyFrame>();
    public List<KeyFrame> KeyFrames = new List<KeyFrame>();
    private bool mIsRunning;
    public bool MoveCamera;
    private CameraShake mShake;
    public int TriggerId;

    private IEnumerator Finished(float time)
    {
        yield return new WaitForSeconds(time);
        mIsRunning = false;
        MoveMainCameraBack();
    }

    private IEnumerator MoveMainCamera()
    {
        if (MoveCamera)
        {
            var cameraController = GameLogic.Instance.MainCamera.GetComponent<CameraController>();
            cameraController.enabled = false;

            TweenPosition.Begin(cameraController.gameObject, 1, CameraPos, true);
            yield return new WaitForSeconds(1);
        }

        OnTrigger();
    }

    private void MoveMainCameraBack()
    {
        var cameraController = GameLogic.Instance.MainCamera.GetComponent<CameraController>();
        cameraController.SmoothMove = true;
        cameraController.enabled = true;
    }

    private void OnDestroy()
    {
#if !UNITY_EDITOR
try
{
#endif

        EventDispatcher.Instance.RemoveEventListener(SceneEvent_Trigger.EVENT_TYPE, SceneEventListener);

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    private void OnTrigger()
    {
        // do not trigger twice.
        if (mIsRunning)
        {
            return;
        }

        float maxTime = 0;
        var KeyFramesCount3 = KeyFrames.Count;
        for (var i = 0; i < KeyFramesCount3; i++)
        {
            var key = KeyFrames[i];
            var time = key.StartTime + key.Animation.length;
            if (time > maxTime)
            {
                maxTime = time;
            }
            StartCoroutine(RunOneKeyFrame(key.StartTime, key.Target, key.Animation));
        }

        var CameraShakeKeyFramesCount4 = CameraShakeKeyFrames.Count;
        for (var i = 0; i < CameraShakeKeyFramesCount4; i++)
        {
            var key = CameraShakeKeyFrames[i];
            var time = key.StartTime + (2*Mathf.PI/key.Speed)*key.Count;
            if (time > maxTime)
            {
                maxTime = time;
            }
            StartCoroutine(RunCameraShake(key.Camera, key.Rot, key.StartTime, key.Count, key.Speed, key.Magnitude));
        }

        StartCoroutine(Finished(maxTime));
        mIsRunning = true;
    }

    private IEnumerator RunCameraShake(Camera cam, Vector3 rot, float delay, int count, float speed, float magnitude)
    {
        yield return new WaitForSeconds(delay);

        var shake = cam.GetComponent<CameraShake>();
        shake.Shake(CameraShake.ShakeType.LocalPosition, count, Vector3.one, rot, magnitude, speed, 0.0f, 1.0f, true,
            null);
    }

    private IEnumerator RunOneKeyFrame(float delay, GameObject o, AnimationClip animation)
    {
        yield return new WaitForSeconds(delay);
        var anim = o.GetComponent<Animation>();
        if (anim == null)
        {
            anim = o.AddComponent<Animation>();
        }

        anim.AddClip(animation, animation.name);
        anim.Play(animation.name);
    }

    public void RunToEnd()
    {
        mIsRunning = true;

        var KeyFramesCount0 = KeyFrames.Count;
        for (var i = 0; i < KeyFramesCount0; i++)
        {
            var key = KeyFrames[i];
            var anim = key.Target.GetComponent<Animation>();
            if (anim == null)
            {
                anim = key.Target.AddComponent<Animation>();
            }

            anim.AddClip(key.Animation, key.Animation.name);
            anim.Play(key.Animation.name);
            anim[key.Animation.name].time = key.Animation.length;
            anim.Sample();
        }
    }

    private void SceneEventListener(IEvent evt)
    {
        var e = evt as SceneEvent_Trigger;
        if (e.TriggerId == TriggerId)
        {
            StartCoroutine(MoveMainCamera());
            var table = Table.GetTriggerArea(TriggerId);
            if (null != table)
            {
                if (-1 != table.SoundID)
                {
                    SoundManager.Instance.PlaySoundEffect(table.SoundID);
                }
            }
        }
    }

    // Use this for initialization
    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif

        EventDispatcher.Instance.AddEventListener(SceneEvent_Trigger.EVENT_TYPE, SceneEventListener);

        var KeyFramesCount1 = KeyFrames.Count;
        for (var i = 0; i < KeyFramesCount1; i++)
        {
            var key = KeyFrames[i];
            key.Target.isStatic = false;
        }

#if !UNITY_EDITOR
//         if (FightCamera.TestMode)
//         {
//             if (gameObject.collider == null)
//             {
//                 var collider = gameObject.AddComponent<MeshCollider>();
//                 collider.isTrigger = true;
//             }
//             else
//             {
//                 gameObject.collider.isTrigger = true;
//             }
//         }
//         else
#endif
        {
            if (gameObject.collider != null)
            {
                gameObject.collider.enabled = false;
            }
        }

#if !UNITY_EDITOR
//        if (!FightCamera.TestMode)
        {
            var CameraShakeKeyFramesCount2 =  CameraShakeKeyFrames.Count;
            for (int i = 0; i <CameraShakeKeyFramesCount2; i++)
            {
                var key = CameraShakeKeyFrames[i];
                key.Camera = GameLogic.Instance.MainCamera;
            }
        }   
#endif

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    [Serializable]
    public class KeyFrame
    {
        public AnimationClip Animation;
        public float StartTime;
        public GameObject Target;
    }

    [Serializable]
    public class CameraShakeKeyFrame
    {
        public Camera Camera;
        public int Count;
        public float Magnitude;
        public Vector3 Rot;
        public float Speed;
        public float StartTime;
    }

#if UNITY_EDITOR
    private void Awake()
    {
#if !UNITY_EDITOR
try
{
#endif
        var CameraShakeKeyFramesCount4 = CameraShakeKeyFrames.Count;
        for (var i = 0; i < CameraShakeKeyFramesCount4; i++)
        {
            var key = CameraShakeKeyFrames[i];
            if (key.Camera == null)
            {
                key.Camera = GameLogic.Instance.MainCamera;
            }
        }

        // 改成IgnoreRaycast层，只有这个层才接受物理
        var layer = LayerMask.NameToLayer("Ignore Raycast");
        if (collider)
        {
            gameObject.layer = layer;
        }
        OptList<Collider>.List.Clear();
        GetComponentsInChildren(true, OptList<Collider>.List);
        for (var i = 0; i < OptList<Collider>.List.Count; i++)
        {
            OptList<Collider>.List[i].gameObject.layer = layer;
        }

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    private void OnTriggerEnter()
    {
        OnTrigger();
    }

#endif
}