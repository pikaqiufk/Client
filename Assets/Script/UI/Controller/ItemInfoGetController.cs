#region using

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ClientDataModel;
using DataTable;
using EventSystem;
using Shared;

#endregion

public class ItemInfoGetController : IControllerBase
{
    public static int GET_PATH_COUNT; //获取途径总个数 

    public ItemInfoGetController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(Event_ItemInfoGetClick.EVENT_TYPE, ItemGetClick);
        //EventDispatcher.Instance.AddEventListener(Event_ItemInfoGetOperation.EVENT_TYPE, Operation);
    }

    public ItemInfoGetDataModel DataModel;

    public void ItemGetClick(IEvent ievent)
    {
        var e = ievent as Event_ItemInfoGetClick;
        var item = DataModel.GetPathList[e.Index];
        var tbItemGet = Table.GetItemGetInfo(item.ItemGetId);
        if (tbItemGet.IsShow == -1) //开启条件
        {
            EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.ItemInfoGetUI));
            GameUtils.GotoUiTab(tbItemGet.UIName, tbItemGet.Param[0], tbItemGet.Param[1], tbItemGet.Param[2]);
            //EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.GetConfig(tbItemGet.UIName)));
        }
        else
        {
            var dic = PlayerDataManager.Instance.CheckCondition(tbItemGet.IsShow);
            if (dic != 0)
            {
                //不符合副本扫荡条件
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(dic));
                return;
            }
        }
        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.ItemInfoGetUI));
        //EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.GetConfig(tbItemGet.UIName)));
        GameUtils.GotoUiTab(tbItemGet.UIName, tbItemGet.Param[0], tbItemGet.Param[1], tbItemGet.Param[2]);
    }

    public void CleanUp()
    {
        GET_PATH_COUNT = 0;
        DataModel = new ItemInfoGetDataModel();
        Table.ForeachItemGetInfo(
            record =>
            {
                GET_PATH_COUNT++;
                return true;
            }
            );
    }

    public void RefreshData(UIInitArguments data)
    {
        var arg = data as ItemInfoGetArguments;
        if (arg == null)
        {
            return;
        }
        DataModel.GetPathList.Clear();
        if (arg.ItemId != -1)
        {
            DataModel.ItemData.ItemId = arg.ItemId;
        }
        if (arg.ItemData != null)
        {
            DataModel.ItemData = arg.ItemData;
        }
        var tbItem = Table.GetItemBase(DataModel.ItemData.ItemId);
        if (tbItem == null)
        {
            return;
        }
        //显示获取途径
        if (tbItem.GetWay != -1)
        {
            var count = 0;
            var list = new List<ItemGetPathDataModel>();
            for (var i = 0; i < GET_PATH_COUNT; i++)
            {
                var isShow = BitFlag.GetLow(tbItem.GetWay, i);
                if (isShow)
                {
                    var tbItemGetInfo = Table.GetItemGetInfo(i);
                    if (tbItemGetInfo != null)
                    {
                        var item = new ItemGetPathDataModel();
                        item.ItemGetId = i;
                        list.Add(item);
                    }
                }
            }
            DataModel.GetPathList = new ObservableCollection<ItemGetPathDataModel>(list);
        }
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

    public void OnShow()
    {
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        return null;
    }

    //public void Operation(IEvent ievent)
    //{
    //    Event_ItemInfoGetOperation e = ievent as Event_ItemInfoGetOperation;
    //    switch (e.Type)
    //    {
    //        case 0:
    //        {
    //        }
    //            break;
    //        default:
    //            break;
    //    }
    //}

    public FrameState State { get; set; }
}