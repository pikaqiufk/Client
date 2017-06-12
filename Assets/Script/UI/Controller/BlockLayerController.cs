/********************************************************************************* 

                         Scorpion



  *FileName:BottleneckLayerFrameCtrler

  *Version:1.0

  *Date:2017-06-08

  *Description:

**********************************************************************************/  


#region using

using System.Collections;
using System.ComponentModel;
using ClientDataModel;
using UnityEngine;

#endregion

public class BottleneckLayerFrameCtrler : IControllerBase
{
    #region 静态变量

    #endregion

    #region 成员变量

    private BlockLayerBoxDataModel DataModel;
    private FrameState m_State;
    private Coroutine m_WaitCoroutine;

    #endregion

    #region 构造函数

    public BottleneckLayerFrameCtrler()
    {
        CleanUp();
    }

    #endregion

    #region 固有函数

    public void Close()
    {
        if (m_WaitCoroutine != null)
        {
            NetManager.Instance.StopCoroutine(m_WaitCoroutine);
            m_WaitCoroutine = null;
        }
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        if (m_WaitCoroutine != null)
        {
            NetManager.Instance.StopCoroutine(m_WaitCoroutine);
        }

        DataModel.IsShowAnime = false;
        m_WaitCoroutine = NetManager.Instance.StartCoroutine(AwaitOneSecondAndRevealAnimationCoroutine(data));
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public void CleanUp()
    {
        if (m_WaitCoroutine != null)
        {
            NetManager.Instance.StopCoroutine(m_WaitCoroutine);
        }
        DataModel = new BlockLayerBoxDataModel();
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        return null;
    }

    public void OnShow()
    {
    }

    public FrameState State
    {
        get { return m_State; }
        set { m_State = value; }
    }

    #endregion

    #region 逻辑函数

    public IEnumerator AwaitOneSecondAndRevealAnimationCoroutine(UIInitArguments msg)
    {
        yield return new WaitForSeconds(1);
        if (msg != null)
        {
            var args = (BlockLayerArguments)msg;
            DataModel.IsShowAnime = args.Type != 1;
        }
        else
        {
            DataModel.IsShowAnime = true;
        }
    }

    #endregion

    #region 事件

    #endregion   
}