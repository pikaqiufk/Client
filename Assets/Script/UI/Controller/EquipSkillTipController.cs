#region using

using System;
using System.ComponentModel;
using ClientDataModel;

#endregion

public class EquipSkillTipController : IControllerBase
{
    private EquipSkillTipDataModel DataModel;

    public EquipSkillTipController()
    {
        CleanUp();
    }


    public void CleanUp()
    {
        DataModel = new EquipSkillTipDataModel();
    }

    public void OnShow()
    {
    }

    public void Close()
    {
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        if (data.Args.Count <= 1)
        {
            return;
        }

        DataModel.BuffId = data.Args[0];
        DataModel.Current.BuffLevel = data.Args[1];
        DataModel.Current.BuffId = DataModel.BuffId;
        DataModel.Next.BuffLevel = DataModel.Current.BuffLevel + 1;
        DataModel.Next.BuffId = DataModel.BuffId;
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }
    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        return null;
    }

    public FrameState State { get; set; }
}
