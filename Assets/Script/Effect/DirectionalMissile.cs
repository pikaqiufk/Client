using System;
#region using

using UnityEngine;

#endregion

//带有跟踪功能，并且可以设置发射角度的
public class DirectionalMissile : Missile
{
    public Vector3 CurrentTarget;
    private float Distance;
    public Vector3 StartDirection;
    private float StartDistance;

    private void OnEnable()
    {
#if !UNITY_EDITOR
try
{
#endif
        if (!sBulletRoot)
        {
            var go = new GameObject("BulletRoot");
            sBulletRoot = go.transform;
        }

        mMyTransform.parent = sBulletRoot;

        if (Target == null)
        {
            return;
        }

        StartDistance = (mMyTransform.position - Target.position).magnitude;
        Distance = StartDistance;
        mMyTransform.LookAt(Target, Vector3.up);
        CurrentTarget = mMyTransform.localToWorldMatrix.MultiplyPoint3x4(StartDirection);

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

    protected override void UpdateTargetPos()
    {
        Distance -= Time.deltaTime*Speed;
        SetTargetPos(Vector3.Lerp(CurrentTarget, Target.position, 1.0f - Distance/StartDistance));
    }
}