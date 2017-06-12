/********************************************************************************* 

                         Scorpion



  *FileName:BossAwardFrameCtrler

  *Version:1.0

  *Date:2017-06-08

  *Description:

**********************************************************************************/  
#region using
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ClientDataModel;

#endregion

public class BossAwardFrameCtrler : IControllerBase
{

    #region 静态变量

    #endregion

    #region 成员变量

    private ActivityRewardFrameDataModel DataModel;

    #endregion

    #region 构造函数

    public BossAwardFrameCtrler()
    {
        CleanUp();
    }

    #endregion

    #region 固有函数

    public void CleanUp()
    {
        DataModel = new ActivityRewardFrameDataModel();
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
        var _args = (BossRewardArguments)data;

        var _seconds = _args.Seconds;
        var _formatStr = GameUtils.GetDictionaryText(1052);
        DataModel.UseTime = string.Format(_formatStr, _seconds / 60, _seconds % 60);

        var _completeType = _args.CompleteType;
        DataModel.IsSuccess = _completeType == eDungeonCompleteType.Success;

        var _items = new List<ItemIdDataModel>();
        var _itemDatas = _args.Items;
        foreach (var itemData in _itemDatas)
        {
            var _item = new ItemIdDataModel();
            _item.ItemId = itemData.ItemId;
            _item.Count = itemData.Count;
            _items.Add(_item);
        }
        DataModel.Rewards = new ObservableCollection<ItemIdDataModel>(_items);
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

    #endregion

    #region 逻辑函数

    #endregion

    #region 事件

    #endregion       
}