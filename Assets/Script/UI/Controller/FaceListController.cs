
/********************************************************************************* 

                         Scorpion




  *FileName:FaceListController

  *Version:1.0

  *Date:2017-06-06

  *Description:

**********************************************************************************/
#region using

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ClientDataModel;
using DataTable;
using EventSystem;

#endregion

public class FaceChainFrameCtrler : IControllerBase
{

    #region 静态变量

    #endregion

    #region 成员变量
    public ChatFaceListDataModel DataModel;
    private int m_chatType;
    private bool m_init;
    #endregion

    #region 构造函数
    public FaceChainFrameCtrler()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(FaceListClickIndex.EVENT_TYPE, OnFaceChainPressIndexEvent);
    }
    #endregion

    #region 固有函数
    public void CleanUp()
    {
        m_init = false;


        DataModel = new ChatFaceListDataModel();
    }

    public void RefreshData(UIInitArguments data)
    {
        if (m_init == false)
        {
            OnInitialization();
        }
        var _arg = data as FaceListArguments;
        if (_arg == null)
        {
            return;
        }
        m_chatType = _arg.Type;
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
    #region 普通函数

    private void PressFace(int index)
    {
        if (index < 0 || index >= DataModel.FaceList.Count)
        {
            return;
        }
        var _face = DataModel.FaceList[index];
        var _e = new AddFaceNode(m_chatType, _face.FaceId);
        EventDispatcher.Instance.DispatchEvent(_e);
    }



    private void OnInitialization()
    {
        var _list = new List<ChatFaceDataModel>();
        Table.ForeachFace(record =>
        {
            var _face = new ChatFaceDataModel();
            _face.FaceId = record.Id;
            _list.Add(_face);
            return true;
        });
        DataModel.FaceList = new ObservableCollection<ChatFaceDataModel>(_list);
        m_init = true;
    }
    #endregion


    #region 事件
    private void OnFaceChainPressIndexEvent(IEvent ievent)
    {
        var _e = ievent as FaceListClickIndex;
        var _index = _e.Index;
        PressFace(_index);
    }
    #endregion
 

 



}