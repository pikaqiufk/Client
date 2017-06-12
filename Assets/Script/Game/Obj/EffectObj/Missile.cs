using System;
#region using

using UnityEngine;

#endregion

//带有跟踪功能
public class Missile : Bullet
{
    //目标ObjId

    public Transform Target;

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

    // Update is called once per frame
    protected override void Update()
    {
#if !UNITY_EDITOR
try
{
#endif

        if (null != Target)
        {
            UpdateTargetPos();
            base.Update();
        }
        else
        {
            //GameObject.Destroy(this.gameObject);
            Destroy();
        }


#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    protected virtual void UpdateTargetPos()
    {
        SetTargetPos(Target.position);
    }
}