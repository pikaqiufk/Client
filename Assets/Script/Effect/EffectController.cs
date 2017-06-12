#region using

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

public enum EffectState
{
    WaitToFire,
    Fired,
    WaitToFinish,
    Finished
}

public class EffectController : MonoBehaviour
{
    public Vector3 Dir;
    private bool mWillDestroy;
    public Action<EffectController, ulong> PlayOverCallback = null;
    private readonly List<ParticleSystem> pList = new List<ParticleSystem>();
    public Vector3? Pos = null;
    public EffectState State;
    public Transform trans;
    public float Delay { get; set; }
    public float Duration { get; set; }
    public bool Follow { get; set; }
    public ulong Id { get; set; }
    public float LoopTime { get; set; }

    public void Fire()
    {
        State = EffectState.Fired;

        if (gameObject == null)
        {
            return;
        }

        // 如果在游戏场景的时候，场景没了或者effect root没了，就可以直接删除了
        if (GameLogic.Instance)
        {
            if (!GameLogic.Instance.Scene || !GameLogic.Instance.Scene.EffectRoot)
            {
                mWillDestroy = true;
                return;
            }
        }

        if (trans.parent.parent == ComplexObjectPool.Holder)
        {
            // 这是个bug
            Logger.Error("特效没有清理干净，这是个bug，提bug吧赶紧的。");
            return;
        }

        ComplexObjectPool.SetActive(gameObject, true);

        if (!Follow)
        {
            trans.parent = GameLogic.Instance.Scene.EffectRoot.transform;
            trans.forward = Dir;
            if (Pos != null)
            {
                trans.position = Pos.Value;
            }
        }

        GameUtils.ResetEffect(gameObject);
    }

    public void InitLoop()
    {
        OptList<ParticleSystem>.List.Clear();
        gameObject.GetComponentsInChildren(true, OptList<ParticleSystem>.List);
        var particlesLength0 = OptList<ParticleSystem>.List.Count;
        for (var i = 0; i < particlesLength0; i++)
        {
            var p = OptList<ParticleSystem>.List[i];
            if (p.loop)
            {
                pList.Add(p);
            }
        }
    }

    private void OnDestroy()
    {
#if !UNITY_EDITOR
        try
        {
#endif

        State = EffectState.Finished;
        EffectManager.Instance.RemoveEffect(Id);
        ComplexObjectPool.NotifyDestroied(gameObject);

#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    public void ResetLoop()
    {
        {
            var __list1 = pList;
            var __listCount1 = __list1.Count;
            for (var __i1 = 0; __i1 < __listCount1; ++__i1)
            {
                var p = __list1[__i1];
                {
                    p.loop = true;
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


#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    public void StopLoop()
    {
        State = EffectState.WaitToFinish;
        OptList<ParticleSystem>.List.Clear();
        gameObject.GetComponentsInChildren(OptList<ParticleSystem>.List);
        var particlesLength1 = OptList<ParticleSystem>.List.Count;
	    //bool loop = false;
        for (var i = 0; i < particlesLength1; i++)
        {
            var p = OptList<ParticleSystem>.List[i];
	        //loop |= p.loop;
            p.loop = false;
        }
	    //if (loop)
	    {
			if (LoopTime >= 0)
			{
				Duration = LoopTime;
			}    
	    }
        
    }

    public void Tick()
    {
        if (gameObject == null)
        {
            mWillDestroy = true;
            return;
            //Destroy(this);
        }

        if (State == EffectState.WaitToFire)
        {
            Delay -= Time.deltaTime;
            if (Delay <= 0)
            {
                Fire();
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
#if !UNITY_EDITOR
        try
        {
#endif
        if (State == EffectState.Finished || State == EffectState.WaitToFire)
        {
            return;
        }

        if (mWillDestroy)
        {
            State = EffectState.Finished;
            EffectManager.Instance.StopEffect(Id);
            return;
        }
        Duration -= Time.deltaTime;
        if (Duration <= 0)
        {
            if (null != PlayOverCallback)
            {
                PlayOverCallback(this, Id);
            }
            State = EffectState.Finished;
            EffectManager.Instance.StopEffect(Id);
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