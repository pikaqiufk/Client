using System;
#region using

using BehaviourMachine;
using UnityEngine;

#endregion

public class BaseState : StateBehaviour
{
    protected ObjCharacter mCharacter;
    protected float mTime;

    private void Awake()
    {
#if !UNITY_EDITOR
try
{
#endif

        mCharacter = gameObject.GetComponent<ObjCharacter>();

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    public float GetElapseTime()
    {
        return mTime;
    }

    protected virtual void OnDisable()
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

    protected virtual void OnEnable()
    {
#if !UNITY_EDITOR
try
{
#endif

        ResetState();

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    protected virtual void ResetState()
    {
        mTime = 0;
    }

    protected virtual void Update()
    {
#if !UNITY_EDITOR
try
{
#endif

        mTime += Time.deltaTime;


#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }
}