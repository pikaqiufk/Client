/********************************************************************************* 
                         Scorpion

  *FileName:ChattingRecordsListCtrler
  *Version:1.0
  *Date:2017-06-06
  *Description:
**********************************************************************************/  
#region using

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ClientDataModel;
using EventSystem;

#endregion

public class ChattingRecordsListCtrler : IControllerBase
{

    #region 静态变量
    #endregion

    #region 成员变量

    private ChatItemListDataModel m_DataModel;
    private int mChatType;

    #endregion

    #region 构造函数

    public ChattingRecordsListCtrler()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(ChatItemListClick.EVENT_TYPE, OnChattingRecordsListClick);
    }

    #endregion

    #region 固有函数

    public void CleanUp()
    {
        m_DataModel = new ChatItemListDataModel();
    }

    public void RefreshData(UIInitArguments data)
    {
        var _arg = data as ChatItemListArguments;
        if (_arg == null)
        {
            return;
        }
        mChatType = _arg.Type;
        var _list = new List<BagItemDataModel>();
        var equipBag = PlayerDataManager.Instance.GetBag((int) eBagType.Equip);
        for (var i = 0; i < equipBag.Capacity; i++)
        {
            var _bagItem = equipBag.Items[i];
            if (_bagItem.ItemId != -1)
            {
                _list.Add(_bagItem);
            }
        }
        var _equipList = PlayerDataManager.Instance.PlayerDataModel.EquipList;
        for (var i = 0; i < _equipList.Count; i++)
        {
            var _bagItem = _equipList[i];
            if (_bagItem.ItemId != -1)
            {
                _list.Add(_bagItem);
            }
        }
        m_DataModel.ItemList = new ObservableCollection<BagItemDataModel>(_list);

        if (m_DataModel.ItemList.Count == 0)
        {
            m_DataModel.HasItem = false;
        }
        else
        {
            m_DataModel.HasItem = true;
        }
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return m_DataModel;
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

    #region 事件

    private void OnChattingRecordsListClick(IEvent ievent)
    {
        var _e = ievent as ChatItemListClick;

        var _e1 = new ChatShareItemEvent(mChatType, _e.DataModel);
        EventDispatcher.Instance.DispatchEvent(_e1);
    }

    #endregion
}