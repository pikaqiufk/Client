using System;
#region using

using System.Collections.Generic;
using UnityEngine;

#endregion

public class EffectDisableBehaviour : MonoBehaviour
{
    public float ActionTime;
    private float mTime;
    public MonoBehaviour Target;
    public List<MonoBehaviour> Targets;

    private void OnEnable()
    {
#if !UNITY_EDITOR
try
{
#endif

        mTime = ActionTime;

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

        mTime -= Time.deltaTime;
        if (mTime < 0)
        {
            var TargetsCount0 = Targets.Count;
            for (var i = 0; i < TargetsCount0; i++)
            {
                if (Targets[i])
                {
                    Targets[i].enabled = false;
                }
            }

            if (Target != null)
            {
                Target.enabled = false;
            }

            enabled = false;
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