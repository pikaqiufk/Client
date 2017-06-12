#region using

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

public class AnimationsPlayer : MonoBehaviour
{
    public List<KeyFrame> KeyFrames = new List<KeyFrame>();
    public bool Loop = true;
#if UNITY_EDITOR
    private void Awake()
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

#endif

    private IEnumerator Finished(float time)
    {
        yield return new WaitForSeconds(time + 0.5f);

        if (!Loop)
        {
            yield break;
        }

        var KeyFramesCount3 = KeyFrames.Count;
        for (var i = 0; i < KeyFramesCount3; i++)
        {
            var key = KeyFrames[i];
            key.Target.SetActive(key.InitActive);
        }

        OnTrigger();
    }

    private void OnDestroy()
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

    public void OnDisable()
    {
#if !UNITY_EDITOR
try
{
#endif

        var KeyFramesCount3 = KeyFrames.Count;
        for (var i = 0; i < KeyFramesCount3; i++)
        {
            var key = KeyFrames[i];
            key.Target.SetActive(key.InitActive);
        }
    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

    public void OnEnable()
    {
#if !UNITY_EDITOR
try
{
#endif
        OnTrigger();

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
        float maxTime = 0;
        var KeyFramesCount3 = KeyFrames.Count;
        for (var i = 0; i < KeyFramesCount3; i++)
        {
            var key = KeyFrames[i];
            var time = key.StartTime + (key.Animation == null ? 0 : key.Animation.length);
            if (time > maxTime)
            {
                maxTime = time;
            }

            key.InitActive = key.Target.activeSelf;
            StartCoroutine(RunOneKeyFrame(key.StartTime, key.Target, key.Animation, key.Enable));
        }

        StartCoroutine(Finished(maxTime));
    }

    private IEnumerator RunOneKeyFrame(float delay, GameObject o, AnimationClip animation, bool enable)
    {
        yield return new WaitForSeconds(delay);

        o.SetActive(enable);

        if (animation != null)
        {
            var anim = o.GetComponent<Animation>();
            if (anim == null)
            {
                anim = o.AddComponent<Animation>();
            }

            anim.AddClip(animation, animation.name);
            anim.Play(animation.name);
        }
    }

    [Serializable]
    public class KeyFrame
    {
        public AnimationClip Animation;
        public bool Enable;
        [NonSerialized] public bool InitActive;
        public float StartTime;
        public GameObject Target;
    }
}