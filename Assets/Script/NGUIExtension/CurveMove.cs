using System;
#region using

using System.Collections.Generic;
using UnityEngine;


#endregion

public class CurveMove : MonoBehaviour
{
    public float Duration = 1f;
    public Vector3 From; //起始点
    public Vector3 HighPostion = Vector3.zero; //贝塞尔曲线高点
    public bool IsMove;
    private float mTime;
    private Bezier myBezier;
    public List<EventDelegate> OnFinish = new List<EventDelegate>();
    public Vector3 To; //终止点

    private void Destroy()
    {
        IsMove = false;
        if (OnFinish.Count != 0)
        {
            EventDelegate.Execute(OnFinish); // OnFinish.Execute();
        }
        else
        {
            Destroy(gameObject);
        }

        //ComplexObjectPool.Release(gameObject);
    }

    private void FixedUpdate()
    {
#if !UNITY_EDITOR
try
{
#endif

        if (!IsMove)
        {
            return;
        }

        mTime += Time.deltaTime;

        var t = mTime/Duration;
        t = Mathf.Min(1f, t);
        transform.localPosition = myBezier.GetPointAtTime(t);

        if (mTime > Duration)
        {
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

    public void Play()
    {
        Play(From, To, HighPostion);
    }

    public void Play(Vector3 f, Vector3 t, Vector3 h)
    {
        myBezier = new Bezier(From, HighPostion, To);
        mTime = 0;
        IsMove = true;

        //var tweens = GetComponentsInChildren<UITweener>();
        //foreach (var tween in tweens)
        //{
        //    tween.ResetForPlay();
        //    tween.PlayForward();
        //}
    }
}