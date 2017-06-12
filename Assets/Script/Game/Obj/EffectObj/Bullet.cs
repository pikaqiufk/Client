#region using

using System;
using System.Collections;
using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;

#endregion

//不带跟踪功能的子弹 一直飞向目标点 
public class Bullet : MonoBehaviour
{
    //允许的误差
    public const float EPSILON = 0.1f;
    //子弹存在的最长时间，超过这个时间就删除
    public const float MAX_LIFETIME = 10.0f;
    public static Transform sBulletRoot;
    //方向
    private Vector3 mDir;
    //子弹击中回掉
    private Action<Bullet> mHitCallback;
    //Transform引用
    protected Transform mMyTransform;
    //移动速度
    private float mSpeed = 5.0f;
    //目标点
    private Vector3 mTargetPos;

    public float Speed
    {
        get { return mSpeed; }
        set { mSpeed = value; }
    }

    private void Awake()
    {
#if !UNITY_EDITOR
try
{
#endif

        mMyTransform = gameObject.transform;

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    //计算方向
    protected virtual void CalculateDir()
    {
        mDir = (mTargetPos - mMyTransform.position).normalized;
        mMyTransform.LookAt(mTargetPos);
    }

    public virtual void Destroy()
    {
        OptList<TrailRenderer_Base>.List.Clear();
        gameObject.GetComponentsInChildren(true, OptList<TrailRenderer_Base>.List);
        {
            var __array7 = OptList<TrailRenderer_Base>.List;
            var __arrayLength7 = __array7.Count;
            for (var __i7 = 0; __i7 < __arrayLength7; ++__i7)
            {
                var t = __array7[__i7];
                {
                    t.ClearSystem(false);
                }
            }
        }

        ComplexObjectPool.Release(gameObject);
        StopAllCoroutines();
    }

    //初始化
    public void Init(float speed, Vector3 targetPos, Action<Bullet> hitCallback = null)
    {
        mSpeed = speed;
        SetTargetPos(targetPos);
        SetHitCallback(hitCallback);

        //安全删除，防止这个子弹因为意外一直存在
        StopAllCoroutines();
        StartCoroutine(SafeRemove());
    }

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

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    protected IEnumerator SafeRemove()
    {
        yield return new WaitForSeconds(MAX_LIFETIME);
        ComplexObjectPool.Release(gameObject);
    }

    //到达目标点回掉
    public void SetHitCallback(Action<Bullet> hitCallback)
    {
        mHitCallback = hitCallback;
    }

    //设置目标点
    public virtual void SetTargetPos(Vector3 pos)
    {
        mTargetPos = pos;
        CalculateDir();
    }

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
    protected virtual void Update()
    {
#if !UNITY_EDITOR
try
{
#endif


        var offset = mSpeed*Time.deltaTime;

        var distance = Vector3.Distance(mMyTransform.position, mTargetPos);
        if (distance <= offset + EPSILON)
        {
            try
            {
//防止因为回掉出异常而没有正常移除子弹造成泄露
                if (null != mHitCallback)
                {
                    mHitCallback(this);
                    mHitCallback = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
            finally
            {
                //GameObject.Destroy(gameObject);
                Destroy();
            }
            return;
        }

        mMyTransform.position += (offset*mDir);

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }
}