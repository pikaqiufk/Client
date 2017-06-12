using System;
#region using

using System.Collections;
using UnityEngine;

#endregion

public class BGMFixed : MonoBehaviour
{
    //private float LastVolume = 1.0f;
    private Coroutine coIn;
    private Coroutine coOut;
    private float oldVolume;
    // Use this for initialization
    private AudioSource selfMusic;
    private Coroutine thisIn;
    private Coroutine thisOut;

    private void BGIn()
    {
        if (coOut != null)
        {
            return;
        }
        ThisOut();
        if (coIn != null)
        {
            StopCoroutine(coIn);
            coIn = null;
        }
        coOut = StartCoroutine(SoundManager.Instance.SetBgmPause(false));
    }

    private void BGOut()
    {
        if (coIn != null)
        {
            return;
        }
        ThisIn();
        if (coOut != null)
        {
            StopCoroutine(coOut);
            coOut = null;
        }
        coIn = StartCoroutine(SoundManager.Instance.SetBgmPause(true));
    }

    private void OnDestroy()
    {
#if !UNITY_EDITOR
try
{
#endif

        coIn = null;
        coOut = null;
        thisOut = null;
        thisIn = null;

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    public void OnTriggerEnter(Collider other)
    {
        if (null == ObjManager.Instance.MyPlayer || null == other || null == other.gameObject)
        {
            return;
        }

        if (other.gameObject != ObjManager.Instance.MyPlayer.gameObject)
        {
            return;
        }
        //this.StartCoroutine(SetEnable(true));
        //if (!selfMusic.isPlaying)
        //{
        //    selfMusic.Play();
        //}
        BGOut();
    }

    public void OnTriggerExit(Collider other)
    {
        if (null == ObjManager.Instance.MyPlayer || null == other || null == other.gameObject)
        {
            return;
        }

        if (other.gameObject != ObjManager.Instance.MyPlayer.gameObject)
        {
            return;
        }
        //thisOut = this.StartCoroutine(SetEnable(false));
        //selfMusic.Pause();
        BGIn();
    }

    private void Reset()
    {
        if (selfMusic == null)
        {
            return;
        }
        var nowVolume = 1.0f;
        if (selfMusic.mute)
        {
            return;
        }
        {
            var minDistance = selfMusic.minDistance*selfMusic.minDistance;

            if (ObjManager.Instance == null)
            {
                return;
            }

            if (ObjManager.Instance.MyPlayer == null)
            {
                return;
            }

            var SelfPos = ObjManager.Instance.MyPlayer.Position;
            var dis = (transform.position - SelfPos).sqrMagnitude;
            if (dis <= minDistance)
            {
                nowVolume = 1.0f;
            }
            else
            {
                var maxDistance = selfMusic.maxDistance*selfMusic.maxDistance;
                if (maxDistance < dis)
                {
                    nowVolume = 0;
                }
                else
                {
                    nowVolume = 1 - (dis - minDistance)/(maxDistance - minDistance);
                }
            }
        }


        if (nowVolume > 0.3f)
        {
            if (!selfMusic.isPlaying)
            {
                selfMusic.Play();
            }
            if (coOut != null)
            {
                return;
            }
            if (coIn != null)
            {
                StopCoroutine(coIn);
                coIn = null;
            }
            coOut = StartCoroutine(SoundManager.Instance.SetBgmPause(true));
        }
        else
        {
            if (coIn != null)
            {
                return;
            }
            if (coOut != null)
            {
                StopCoroutine(coOut);
                coOut = null;
            }
            selfMusic.Pause();
            coIn = StartCoroutine(SoundManager.Instance.SetBgmPause(false));
        }
    }

    public IEnumerator SetEnable(bool b = true)
    {
        if (b)
        {
            if (selfMusic.isPlaying)
            {
                var time = 0.2f;
                while (time > 0)
                {
                    selfMusic.volume = (time/0.2f)*oldVolume;
                    time -= Time.deltaTime;
                    //Logger.Info("volume = {0}", selfMusic.volume);
                    yield return null;
                }
                selfMusic.Pause();
            }
        }
        else
        {
            if (selfMusic.isPlaying)
            {
                yield break;
            }
            selfMusic.volume = 0;
            selfMusic.Play();
            float time = 0;
            while (time < 1.4f)
            {
                selfMusic.volume = (time/1.4f)*oldVolume;
                time += Time.deltaTime;
                //Logger.Info("volume = {0}", selfMusic.volume);
                yield return null;
            }
            selfMusic.volume = oldVolume;
        }
    }

    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif

        selfMusic = gameObject.GetComponent<AudioSource>();
        oldVolume = selfMusic.volume;

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    private void ThisIn()
    {
        if (thisIn != null)
        {
            return;
        }
        if (thisOut != null)
        {
            StopCoroutine(thisOut);
            thisOut = null;
        }
        thisIn = StartCoroutine(SetEnable(false));
    }

    private void ThisOut()
    {
        if (thisOut != null)
        {
            return;
        }
        if (thisIn != null)
        {
            StopCoroutine(thisIn);
            thisIn = null;
        }
        thisOut = StartCoroutine(SetEnable(true));
    }
}