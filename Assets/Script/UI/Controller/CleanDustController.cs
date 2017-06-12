/********************************************************************************* 

                         Scorpion



  *FileName:PurgeDirtFramectrler

  *Version:1.0

  *Date:2017-06-09

  *Description:

**********************************************************************************/  
#region using

using System.ComponentModel;
using ClientDataModel;

#endregion

public class PurgeDirtFramectrler : IControllerBase
{

    #region 成员变量

    private CleanDustDataModel DataModel;

    #endregion

    #region 构造函数

    public PurgeDirtFramectrler()
    {
        CleanUp();
        // EventDispatcher.Instance.AddEventListener(UIEvent_SkillFrame_SkillSelect.EVENT_TYPE, OnClicSkillItem);
    }

    #endregion

    #region 固有函数

    public void CleanUp()
    {
        DataModel = new CleanDustDataModel();
    }

    public void RefreshData(UIInitArguments data)
    {
        var _arg = data as CleanDustArguments;
        DataModel.StatueIndex = _arg.StatueIndex;
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public void Close()
    {
    }

    public void Tick()
    {
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

    public FrameState State { get; set; }

    #endregion 
}