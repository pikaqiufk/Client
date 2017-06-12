#region using

using System.Collections.Generic;
using System.ComponentModel;
using ClientDataModel;
using DataTable;
using EventSystem;
using Shared;

#endregion

public class GainItemHintController : IControllerBase
{
    #region 数据

    public FrameState State { get; set; }

    public GainItemHintDataModel DataModel;

    //提示框的存在时间，到时间后会自动关闭
    public float LastTime = 5.0f;

    //定时器handler，用来自动关闭提示界面
    public object mTimer;

    //缓存，显示不了的提示暂存在这里
    public List<CacheEntry> Caches = new List<CacheEntry>();

    #endregion

    #region 初始化以及虚函数

    public GainItemHintController()
    {
        CleanUp();

        LastTime = int.Parse(Table.GetClientConfig(109).Value)/1000.0f;

        EventDispatcher.Instance.AddEventListener(UIEvent_HintEquipEvent.EVENT_TYPE, OnButtonEquip);
        EventDispatcher.Instance.AddEventListener(UIEvent_HintUseItemEvent.EVENT_TYPE, OnButtonUse);
        EventDispatcher.Instance.AddEventListener(EquipChangeEvent.EVENT_TYPE, OnEquipChange);
        EventDispatcher.Instance.AddEventListener(UseItemEvent.EVENT_TYPE, OnUseItem);
        EventDispatcher.Instance.AddEventListener(UIEvent_HintCloseEvent.EVENT_TYPE, OnClose);
        EventDispatcher.Instance.AddEventListener(UIEvent_BagChange.EVENT_TYPE, OnBagItemChange);
    }

    public void CleanUp()
    {
        DataModel = new GainItemHintDataModel();
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
        var args = data as GainItemHintArguments;
        if (args == null)
        {
            return;
        }

        ResetTimer();

        var itemId = args.ItemId;
        var bagIdx = args.BagIndex;

        if (CheckEntry(itemId))
        {
            return;
        }

        // 选择上面那个，还是下面那个
        var entryId = 0;
        for (; entryId < 2; entryId++)
        {
            if (!BitFlag.GetLow(DataModel.UseMask, entryId))
            {
                break;
            }
        }

        if (entryId < 2)
        {
//如果还有空闲的tip pannel，则显示
            SetupEntry(entryId, itemId, bagIdx);
        }
        else
        {
//如果没有空闲的tip pannel，则缓存下来
            var entry = Caches.Find(e => e.ItemId == itemId);
            if (entry == null)
            {
                entry = new CacheEntry {ItemId = itemId, BagIdx = bagIdx};
                Caches.Add(entry);
            }
        }
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

    #endregion

    #region 事件响应

    //装备
    private void OnButtonEquip(IEvent ievent)
    {
        var e = ievent as UIEvent_HintEquipEvent;
        var index = e.Index;
        var data = DataModel.Entrys[index];
        var bagItem = data.BagItemData;

        EquipCompareController.ReplaceEquip(bagItem);
    }

    //响应装备更换事件
    private void OnEquipChange(IEvent ievent)
    {
        if (State != FrameState.Open)
        {
            return;
        }

        var e = ievent as EquipChangeEvent;
        var equip = e.Item;

        var playerData = PlayerDataManager.Instance;
        var worstEquip = playerData.FindWorstEquip(equip);

        //把缓存里比这件装备差的，都移除掉
        var toRemove = new List<CacheEntry>();
        {
            var __list1 = Caches;
            var __listCount1 = __list1.Count;
            for (var __i1 = 0; __i1 < __listCount1; ++__i1)
            {
                var entry = __list1[__i1];
                {
                    var tbItem2 = Table.GetItemBase(entry.ItemId);
                    var item2 = playerData.GetItem(tbItem2.InitInBag, entry.BagIdx);

                    int fightValueAdd;
                    if (playerData.CompareEquips(worstEquip, item2, out fightValueAdd))
                    {
                        if (fightValueAdd <= 0)
                        {
                            toRemove.Add(entry);
                        }
                    }
                }
            }
        }

        for (var i = Caches.Count - 1; i >= 0; --i)
        {
            if (toRemove.Contains(Caches[i]))
            {
                Caches.RemoveAt(i);
            }
        }

        for (var i = 0; i < 2; ++i)
        {
            // 检查是否需要关闭hint panel
            if (BitFlag.GetLow(DataModel.UseMask, i))
            {
                var data2 = DataModel.Entrys[i];
                var bagItem = data2.BagItemData;
                if (bagItem.BagId == equip.BagId && bagItem.Index == equip.Index)
                {
                    ClosePanel(i);
                }
                else
                {
                    int fightValueAdd;
                    if (playerData.CompareEquips(worstEquip, bagItem, out fightValueAdd))
                    {
                        if (fightValueAdd <= 0)
                        {
                            ClosePanel(i);
                        }
                        else
                        {
                            data2.FightValueOld = worstEquip.FightValue;
                            data2.FightValueAdd = fightValueAdd;
                        }
                    }
                }
            }
        }
    }

    //使用
    private void OnButtonUse(IEvent ievent)
    {
        var e = ievent as UIEvent_HintUseItemEvent;
        var data = DataModel.Entrys[e.Index];
        GameUtils.UseItem(data.BagItemData);
    }

    //响应物品被使用的事件
    private void OnUseItem(IEvent ievent)
    {
        OnUseItemNew(ievent);
        //if (State != FrameState.Open)
        //{
        //    return;
        //}

        //var e = ievent as UseItemEvent;
        //var item = e.Item;

        //var itemTotalCount = PlayerDataManager.Instance.GetItemTotalCount(item.ItemId);
        //if (itemTotalCount.Count > 1)
        //{
        //    item.Count = itemTotalCount.Count - 1;
        //    ResetTimer();
        //    return;
        //}

        //for (var i = 0; i < 2; ++i)
        //{
        //    // 检查是否需要关闭hint panel
        //    if (BitFlag.GetLow(DataModel.UseMask, i))
        //    {
        //        var itemData = DataModel.Entrys[i].BagItemData;
        //        if (itemData == item)
        //        {
        //            ClosePanel(i);
        //        }
        //    }
        //}
    }


    private void OnUseItemNew(IEvent ievent)
    {
        if (State != FrameState.Open)
        {
            return;
        }

        var e = ievent as UseItemEvent;
        if (e == null)
            return;

        var item = e.Item;
        if (item == null)
            return;

        var entryIndex = -1;
        for (var i = 0; i < 2; ++i)
        {
            if (BitFlag.GetLow(DataModel.UseMask, i))
            {
                var itemData = DataModel.Entrys[i].BagItemData;
                if (itemData == item)
                {
                    entryIndex = i;
                    break;
                }
            }
        }

        if (entryIndex == -1)
            return;

        var itemTotalCount = PlayerDataManager.Instance.GetItemTotalCount(item.ItemId);
        if (itemTotalCount.Count > 1)
        {
            if (item.Count <= 1)
            {
                var bagItem = PlayerDataManager.Instance.GetBagItemByItemId(item.BagId, item.ItemId);
                if (bagItem != null)
                {
                    DataModel.Entrys[entryIndex].BagItemData = bagItem;
                }

                //ClosePanel(entryIndex);
                //return;
            }

            DataModel.Entrys[entryIndex].Count = itemTotalCount.Count - 1;

            ResetTimer();
        }
        else
        {
            ClosePanel(entryIndex);
        }
    }

    //关闭界面
    private void OnClose(IEvent ievent)
    {
        CloseUI();
    }

    #endregion

    #region 其它

    //重置Timer
    public void ResetTimer()
    {
        var time = Game.Instance.ServerTime.AddSeconds(LastTime);
        if (mTimer != null)
        {
            TimeManager.Instance.ChangeTime(mTimer, time);
        }
        else
        {
            mTimer = TimeManager.Instance.CreateTrigger(time, CloseUI);
        }
    }

    //检查并显示一个提示条
    public bool SetupEntry(int entryId, int itemId, int bagIdx)
    {
        var args = PlayerDataManager.Instance.GetGainItemHintEntryArgs(itemId, bagIdx);
        if (args == null)
        {
            return false;
        }

        DataModel.UseMask = BitFlag.IntSetFlag(DataModel.UseMask, entryId);

        var entry = DataModel.Entrys[entryId];
        entry.BagItemData = args.ItemData;
        entry.Count = args.Count;
        entry.FightValueOld = args.FightValueOld;
        entry.Index = args.OldEquipIdx;
        entry.FightValueAdd = entry.BagItemData.FightValue - entry.FightValueOld;
        return true;
    }

    //检查下，当前显示的entry里有没有 itemId 的物品，如果有，则刷新个数
    private bool CheckEntry(int itemId)
    {
        var tbItem = Table.GetItemBase(itemId);
        //如果不是物品，返回false
        if (tbItem.Type >= 10000 && tbItem.Type <= 10099)
        {
            return false;
        }

        var entrys = DataModel.Entrys;
        for (int i = 0, imax = entrys.Count; i < imax; i++)
        {
            if (BitFlag.GetLow(DataModel.UseMask, i))
            {
                var entry = entrys[i];
                if (entry.BagItemData.ItemId == itemId)
                {
                    //刷新count，由于获得了同样的物品，数量肯定发生变化了
                    entry.Count = PlayerDataManager.Instance.GetItemTotalCount(itemId).Count;
                    return true;
                }
            }
        }
        return false;
    }

    public void OnBagItemChange(IEvent ievent)
    {
        if (State != FrameState.Open)
        {
            return;
        }

        var e = ievent as UIEvent_BagChange;
        if (e.HasType(eBagType.BaseItem))
        {
            for (var i = 0; i < 2; ++i)
            {
                if (BitFlag.GetLow(DataModel.UseMask, i))
                {
                    var itemData = DataModel.Entrys[i].BagItemData;
                    if (itemData.ItemId == -1 || itemData.Count == 0)
                    {
                        ClosePanel(i);
                    }
                }
            }                    
        }
    }

    //关闭一个提示条
    public void ClosePanel(int index)
    {
        ResetTimer();
        if (Caches.Count > 0)
        {
            for (int i = 0, imax = Caches.Count; i < imax; i++)
            {
                var cache = Caches[i];
                if (CheckEntry(cache.ItemId) || SetupEntry(index, cache.ItemId, cache.BagIdx))
                {
                    Caches.RemoveRange(0, i + 1);
                    return;
                }
            }
            Caches.Clear();
        }
        //缓存中没有符合条件的，则关闭该提示条
        DataModel.UseMask = BitFlag.IntSetFlag(DataModel.UseMask, index, false);
        if (DataModel.UseMask == 0)
        {
            CloseUI();
        }
    }

    //关闭整个UI
    public void CloseUI()
    {
        if (mTimer != null)
        {
            TimeManager.Instance.DeleteTrigger(mTimer);
            mTimer = null;
        }
        DataModel.UseMask = 0;
        Caches.Clear();
        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.GainItemHintUI));
    }

    #endregion
}