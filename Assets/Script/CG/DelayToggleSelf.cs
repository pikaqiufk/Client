using System;
#region using

using System.Collections;
using UnityEngine;

#endregion

public class DelayToggleSelf : MonoBehaviour
{
    public float DelayTime;

    private IEnumerator DelayToggle(float time)
    {
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(!gameObject.active);
    }

    // Use this for initialization
    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif

        StartCoroutine(DelayToggle(DelayTime));

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }
}