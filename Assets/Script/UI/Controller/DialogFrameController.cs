#region using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using ClientDataModel;
using EventSystem;

#endregion

public class DialogFrameController : IControllerBase
{
    public DialogFrameController()
    {
        CleanUp();

        EventDispatcher.Instance.AddEventListener(Event_ShowDialogue.EVENT_TYPE, UpdateDialogueData);
        EventDispatcher.Instance.AddEventListener(Event_ShowNextDialogue.EVENT_TYPE, evn => { ShowNextDialogue(); });
    }

    public DialogueDataModel DataModel = new DialogueDataModel();
    public Action mCallback;
    public List<DialogueData> mDialogue;
    public FrameState mState;

    public void ShowNextDialogue()
    {
        if (mDialogue.Count > 0)
        {
            DataModel.DialogContent = "    " + mDialogue[0].DialogContent;
            DataModel.ModelId = mDialogue[0].NpcDataId;
	        DataModel.CharacterName = mDialogue[0].Name;
            mDialogue.RemoveAt(0);
            return;
        }

        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.DialogFrame));
        if (null != mCallback)
        {
            mCallback();
        }
    }

    public void UpdateDialogueData(IEvent ievent)
    {
        var evn = ievent as Event_ShowDialogue;
        mDialogue = evn.Dialogue;
        mCallback = evn.Callback;

        ShowNextDialogue();
    }

    public void Close()
    {
        DataModel.ModelId = -2;
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public void CleanUp()
    {
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        throw new NotImplementedException(name);
    }

    public void OnShow()
    {
    }

    public FrameState State
    {
        get { return mState; }
        set { mState = value; }
    }
}