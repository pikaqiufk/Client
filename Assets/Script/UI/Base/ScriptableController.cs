
using System;
using System.Collections.Generic;
using System.ComponentModel;
using CLRSharp;
using TB.ComponentModel;
using UnityEngine;


public class CrossBind_IControllerBase : ICrossBind
{
    public Type Type
    {
        get { return typeof(IControllerBase); }

    }

    public object CreateBind(CLRSharp_Instance inst)
    {
        var context = ThreadContext.activeContext;
        return new ScriptableController(context.environment, inst);
    }
}


public class ScriptableController : IControllerBase
{
    public object mRuntimeObject;
    public ICLRSharp_Environment mEnvironment;
    public string mName;

    public IMethod mGetDataModel;
    public IMethod mClose;
    public IMethod mCleanUp;
    public IMethod mRefreshDate;
    public IMethod mGetState;
    public IMethod mSetState;
    public IMethod mTick;
    public IMethod mOnChangeScene;
    public IMethod mCallFromMethod;
    public IMethod mOnShow;

    public string GetName()
    {
        return mName;
    }

    public ScriptableController(ICLRSharp_Environment env, ICLRType type, string name)
    {
        var method = type.GetMethod(".ctor", MethodParamList.constEmpty());
        mRuntimeObject = method.Invoke(ThreadContext.activeContext, null, null);

        mName = name;

        Init(env, (CLRSharp_Instance) mRuntimeObject);
    }

    public ScriptableController(ICLRSharp_Environment env, CLRSharp_Instance instance)
    {
        Init(env, instance);
    }

    void Init(ICLRSharp_Environment env, CLRSharp_Instance instance)
    {
        mRuntimeObject = instance;

        var type = instance.type;

        mGetDataModel = type.GetMethod("GetDataModel",
            new MethodParamList(new[] {env.GetType(typeof (string))}));

        mClose = type.GetMethod("Close",
            MethodParamList.constEmpty());

        mCleanUp = type.GetMethod("CleanUp",
            MethodParamList.constEmpty());

        mRefreshDate = type.GetMethod("RefreshData",
            new MethodParamList(new[] {env.GetType(typeof (UIInitArguments))}));

        mGetState = type.GetMethod("get_State", MethodParamList.constEmpty());
        mSetState = type.GetMethod("set_State", new MethodParamList(new[] {env.GetType(typeof (FrameState))}));

        mTick = type.GetMethod("Tick", new MethodParamList(MethodParamList.constEmpty()));
        mOnChangeScene = type.GetMethod("OnChangeScene", new MethodParamList(new[] {env.GetType(typeof (int))}));

        mCallFromMethod = type.GetMethod("CallFromOtherClass",
            new MethodParamList(new[] {env.GetType(typeof (string)), env.GetType(typeof (object[]))}));

        mOnShow = type.GetMethod("OnShow",
            MethodParamList.constEmpty());

    }


    public INotifyPropertyChanged GetDataModel(string name)
    {
        try
        {
            if (mGetDataModel != null)
            {
                var arr = MemoryPool.GetArray(1);
                arr[0] = name;
                var ret =
                    (INotifyPropertyChanged) mGetDataModel.Invoke(ThreadContext.activeContext, mRuntimeObject, arr);
                MemoryPool.ReleaseArray(arr);
                return ret;
            }
        }
        catch (Exception ex)
        {
            Logger.Error("call GetDataModel from {0} error, exception: {1}", mName, ex.ToString());
        }

        return null;
    }

    public FrameState State
    {
        get
        {
            try
            {
                var ret = mGetState.Invoke(ThreadContext.activeContext, mRuntimeObject, null);
                return (FrameState) ret.Convert(typeof (FrameState));
            }
            catch (Exception ex)
            {
                Logger.Error("call get State from {0} error, exception: {1}", mName, ex.ToString());
            }

            return FrameState.Bad;
        }
        set
        {
            try
            {
                var arr = MemoryPool.GetArray(1);
                arr[0] = value;
                mSetState.Invoke(ThreadContext.activeContext, mRuntimeObject, arr);
                MemoryPool.ReleaseArray(arr);
            }
            catch (Exception ex)
            {
                Logger.Error("call set State from {0} error, exception: {1}", mName, ex.ToString());
            }
        }
    }

    public void Close()
    {
        try
        {
            if (mClose != null)
            {
                mClose.Invoke(ThreadContext.activeContext, mRuntimeObject, null);
            }
        }
        catch (Exception ex)
        {
            Logger.Error("call Close from {0} error, exception: {1}", mName, ex.ToString());
        }
    }

    public void Tick()
    {
        try
        {
            if (mTick != null)
            {
                mTick.Invoke(ThreadContext.activeContext, mRuntimeObject, null);
            }
        }
        catch (Exception ex)
        {
            Logger.Error("call Tick from {0} error, exception: {1}", mName, ex.ToString());
        }
    }

    public void CleanUp()
    {
        try
        {
            if (mCleanUp != null)
            {
                mCleanUp.Invoke(ThreadContext.activeContext, mRuntimeObject, null);
            }
        }
        catch (Exception ex)
        {
            Logger.Error("call CleanUp from {0} error, exception: {1}", mName, ex.ToString());
        }
    }

    public void OnChangeScene(int sceneId)
    {
        try
        {
            if (mOnChangeScene != null)
            {
                var arr = MemoryPool.GetArray(1);
                arr[0] = sceneId;
                mOnChangeScene.Invoke(ThreadContext.activeContext, mRuntimeObject, arr);
                MemoryPool.ReleaseArray(arr);
            }
        }
        catch (Exception ex)
        {
            Logger.Error("call OnChangeScene from {0} error, exception: {1}", mName, ex.ToString());
        }
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        try
        {
            if (mCallFromMethod != null)
            {
                var arr = MemoryPool.GetArray(2);
                arr[0] = name;
                arr[1] = param;
                var ret = mCallFromMethod.Invoke(ThreadContext.activeContext, mRuntimeObject, arr);
                MemoryPool.ReleaseArray(arr);
                if (ret is VBox)
                {
                    return (ret as VBox).BoxDefine();
                }

                return ret;
            }

        }
        catch (Exception ex)
        {
            Logger.Error("call CallFromOtherClass from {0} error, exception: {1}", mName, ex.ToString());
        }
        return null;
    }

    public void OnShow()
    {
        try
        {
            if (mOnShow != null)
            {
                mOnShow.Invoke(ThreadContext.activeContext, mRuntimeObject, null);
            }
        }
        catch (Exception ex)
        {
            Logger.Error("call OnShow from {0} error, exception: {1}", mName, ex.ToString());
        }
    }

    public void RefreshData(UIInitArguments data)
    {
        try
        {
            if (mRefreshDate != null)
            {
                var arr = MemoryPool.GetArray(2);
                arr[0] = data;
                mRefreshDate.Invoke(ThreadContext.activeContext, mRuntimeObject, arr);
                MemoryPool.ReleaseArray(arr);
            }
        }
        catch (Exception ex)
        {
            Logger.Error("call RefreshData from {0} error, exception: {1}", mName, ex.ToString());
        }
    }
}