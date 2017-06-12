#region using

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ClientDataModel;
using ClientService;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using Shared;

#endregion

public class DungeonRewardController : IControllerBase
{
    public DungeonRewardController()
    {
        EventDispatcher.Instance.AddEventListener(UIEvent_DungeonReward.EVENT_TYPE, RecvReward);
        CleanUp();
    }

    public DungeonRewardFrameDataModel DataModel;

    public void CleanUp()
    {
        DataModel = new DungeonRewardFrameDataModel();
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

    public void RecvReward(IEvent ievent)
    {
        var evt = ievent as UIEvent_DungeonReward;
        if (evt == null)
            return;
        var items = new List<ItemIdDataModel>();
        for (int i = 0; i < evt.reward.Items.Count; i++)
        {
            var item = new ItemIdDataModel();
            item.ItemId = evt.reward.Items[i].ItemId;
            item.Count = evt.reward.Items[i].Count;
            items.Add(item);
        }
        DataModel.Rewards = new ObservableCollection<ItemIdDataModel>(items);
    }

    public void RefreshData(UIInitArguments data)
    {
        var args = data as DungeonRewardArguments;
        if (args == null)
        {
            return;
        }

        var seconds = args.Seconds;
        var formatStr = GameUtils.GetDictionaryText(1052);
        DataModel.UseTime = string.Format(formatStr, seconds/60, seconds%60);

        var tbFuben = Table.GetFuben(args.FubenId);
        if (tbFuben == null)
        {
            Logger.Error("tbFuben == null in DungeonRewardController.RefreshData()");
            return;
        }

        var enterCount = PlayerDataManager.Instance.GetExData(tbFuben.TodayCountExdata);
        //var items = new List<ItemIdDataModel>();
        //for (int i = 0, imax = tbFuben.DisplayCount.Count; i < imax; ++i)
        //{
        //    var itemId = tbFuben.DisplayReward[i];
        //    if (itemId == -1)
        //    {
        //        break;
        //    }
        //    var itemCount = tbFuben.DisplayCount[i];
        //    itemCount = GameUtils.GetRewardCount(tbFuben, itemCount, 0, enterCount);
        //    var item = new ItemIdDataModel();
        //    item.ItemId = itemId;
        //    item.Count = itemCount;
        //    items.Add(item);
        //}
        //DataModel.Rewards = new ObservableCollection<ItemIdDataModel>(items);
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