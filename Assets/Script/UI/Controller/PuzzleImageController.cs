#region using

using System.ComponentModel;
using ClientDataModel;

#endregion

public class PuzzleImageController : IControllerBase
{
    public PuzzleImageController()
    {
        CleanUp();
        // EventDispatcher.Instance.AddEventListener(UIEvent_SkillFrame_SkillSelect.EVENT_TYPE, OnClicSkillItem);
    }

    public PuzzleImageDataModel DataModel;

    public void CleanUp()
    {
        DataModel = new PuzzleImageDataModel();
    }

    public void RefreshData(UIInitArguments data)
    {
        var arg = data as PuzzleImageArguments;
        DataModel.StatueIndex = arg.StatueIndex;
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
}