#region using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ClientDataModel;
using DataTable;
using EventSystem;

#endregion

public class EquipPackController : IControllerBase
{
    public EquipPackController()
    {
        CleanUp();
    }

    public EquipPackDataModel DataModel;

    public readonly List<eEquipType> EquipOrder1 = new List<eEquipType>
    {
        eEquipType.WeaponMain,
        eEquipType.WeaponScend,
        eEquipType.Helmet,
        eEquipType.Chest,
        eEquipType.Hand,
        eEquipType.Leg,
        eEquipType.Foot,
        eEquipType.Necklace,
        eEquipType.RingL,
        eEquipType.RingR
    };

    public readonly List<int> EquipOrder2 = new List<int>
    {
        (int) eBagType.Equip11,
        (int) eBagType.Equip12,
        (int) eBagType.Equip01,
        (int) eBagType.Equip05,
        (int) eBagType.Equip08,
        (int) eBagType.Equip09,
        (int) eBagType.Equip10,
        (int) eBagType.Equip02,
        (int) eBagType.Equip07
    };

    public int ComparerItem(EquipItemDataModel a, EquipItemDataModel b)
    {
        //部位
        var tbEquipA = Table.GetEquipBase(a.BagItemData.ItemId);
        var tbEquipB = Table.GetEquipBase(b.BagItemData.ItemId);
        var bagIdA = GameUtils.GetEquipBagId(tbEquipA);
        var bagIdB = GameUtils.GetEquipBagId(tbEquipB);
        var indexA = EquipOrder2.FindIndex(i => i == bagIdA);
        var indexB = EquipOrder2.FindIndex(i => i == bagIdB);
        if (indexA != indexB)
        {
            return indexA - indexB;
        }

        //阶数
        if (tbEquipA.Ladder != tbEquipB.Ladder)
        {
            return tbEquipB.Ladder - tbEquipA.Ladder;
        }

        //品质
        var tbItemA = Table.GetItemBase(a.BagItemData.ItemId);
        var tbItemB = Table.GetItemBase(b.BagItemData.ItemId);
        if (tbItemA.Quality != tbItemB.Quality)
        {
            return tbItemB.Quality - tbItemA.Quality;
        }

        //强化等级
        var enhanceA = a.BagItemData.Exdata.Enchance;
        var enhanceB = b.BagItemData.Exdata.Enchance;
        if (enhanceA != enhanceB)
        {
            return enhanceB - enhanceA;
        }

        //战力
        if (a.BagItemData.FightValue != b.BagItemData.FightValue)
        {
            return b.BagItemData.FightValue - a.BagItemData.FightValue;
        }

        //item id
        if (a.BagItemData.ItemId != b.BagItemData.ItemId)
        {
            return a.BagItemData.ItemId - b.BagItemData.ItemId;
        }

        //bag index
        return a.BagItemData.Index - b.BagItemData.Index;
    }

    public void Refresh()
    {
        var equipItems = new List<EquipItemDataModel>();
        PlayerDataManager.Instance.ForeachEquip(EquipOrder1, bagItem =>
        {
            if (bagItem.ItemId != -1)
            {
                var equipItem = new EquipItemDataModel();
                equipItem.BagItemData = bagItem;
                equipItems.Add(equipItem);
            }
        });

        DataModel.EquipItems = new ObservableCollection<EquipItemDataModel>(equipItems);

        var packItems = new List<EquipItemDataModel>();
        {
            var playerBags = PlayerDataManager.Instance.PlayerDataModel.Bags;
            var __enumerator1 = (playerBags.Bags[(int) eBagType.Equip].Items).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var item = __enumerator1.Current;
                {
                    if (item.ItemId != -1)
                    {
                        var equipItem = new EquipItemDataModel();
                        equipItem.BagItemData = item;
                        packItems.Add(equipItem);
                    }
                }
            }
        }
        packItems.Sort(ComparerItem);
        DataModel.PackItems = new ObservableCollection<EquipItemDataModel>(packItems);
    }

    public void RefreshForEquipInherit(BagItemDataModel inherit, BagItemDataModel inherited)
    {
        var itemId = -1;
        if (inherit.ItemId != -1)
        {
            itemId = inherit.ItemId;
        }
        else
        {
            if (inherited.ItemId != -1)
            {
                itemId = inherited.ItemId;
            }
        }
        if (itemId == -1)
        {
            Refresh();
            return;
        }
        var playerBags = PlayerDataManager.Instance.PlayerDataModel.Bags;
        var tbItemBase = Table.GetItemBase(itemId);

        var equipItems = new List<EquipItemDataModel>();
        {
            PlayerDataManager.Instance.ForeachEquip(bagData =>
            {
                if (bagData.ItemId != -1 && (inherit != bagData && inherited != bagData))
                {
                    var tb = Table.GetItemBase(bagData.ItemId);
                    if (tbItemBase == null || GameUtils.CheckInheritType(tbItemBase, tb))
                    {
                        var equipItem = new EquipItemDataModel();
                        equipItem.BagItemData = bagData;
                        equipItems.Add(equipItem);
                    }
                }
            });
        }
        DataModel.EquipItems = new ObservableCollection<EquipItemDataModel>(equipItems);

        var packItems = new List<EquipItemDataModel>();
        {
            var __enumerator6 = (playerBags.Bags[(int) eBagType.Equip].Items).GetEnumerator();
            while (__enumerator6.MoveNext())
            {
                var item = __enumerator6.Current;
                {
                    if (item.ItemId != -1 && (inherit != item && inherited != item))
                    {
                        var tb = Table.GetItemBase(item.ItemId);
                        if (tbItemBase == null || GameUtils.CheckInheritType(tbItemBase, tb))
                        {
                            var equipItem = new EquipItemDataModel();
                            equipItem.BagItemData = item;
                            packItems.Add(equipItem);
                        }
                    }
                }
            }
        }
        packItems.Sort(ComparerItem);
        DataModel.PackItems = new ObservableCollection<EquipItemDataModel>(packItems);
    }

    public void RefreshForEvoEquip()
    {
        var playerBags = PlayerDataManager.Instance.PlayerDataModel.Bags;

        Action<BagItemDataModel, ICollection<EquipItemDataModel>> func = (bagData, items) =>
        {
            if (bagData.ItemId != -1)
            {
                var tbEquip = Table.GetEquipBase(bagData.ItemId);
                if (tbEquip.UpdateEquipID != -1)
                {
                    var equipItem = new EquipItemDataModel();
                    equipItem.BagItemData = bagData;
                    items.Add(equipItem);
                }
            }
        };

        var equipItems = new ObservableCollection<EquipItemDataModel>();
        {
            PlayerDataManager.Instance.ForeachEquip(EquipOrder1, bagData => { func(bagData, equipItems); });
        }
        DataModel.EquipItems = equipItems;

        var packItems = new List<EquipItemDataModel>();
        {
            var __enumerator6 = (playerBags.Bags[(int) eBagType.Equip].Items).GetEnumerator();
            while (__enumerator6.MoveNext())
            {
                var bagData = __enumerator6.Current;
                {
                    func(bagData, packItems);
                }
            }
        }
        packItems.Sort(ComparerItem);
        DataModel.PackItems = new ObservableCollection<EquipItemDataModel>(packItems);
    }

    public void RefreshForSameEquipUpdateId(int updateId, List<BagItemDataModel> excludeList)
    {
        var playerBags = PlayerDataManager.Instance.PlayerDataModel.Bags;

        Action<BagItemDataModel, ICollection<EquipItemDataModel>> func = (bagData, items) =>
        {
            if (bagData.ItemId != -1)
            {
                var tbEquip = Table.GetEquipBase(bagData.ItemId);
                if (updateId == tbEquip.EquipUpdateLogic && !excludeList.Contains(bagData, new BagItemDataModelComParer()))
                {
                    var equipItem = new EquipItemDataModel();
                    equipItem.BagItemData = bagData;
                    items.Add(equipItem);
                }
            }
        };

        var equipItems = new ObservableCollection<EquipItemDataModel>();
        {
            PlayerDataManager.Instance.ForeachEquip(bagData => { func(bagData, equipItems); });
        }
        DataModel.EquipItems = equipItems;

        var packItems = new List<EquipItemDataModel>();
        {
            var __enumerator6 = (playerBags.Bags[(int)eBagType.Equip].Items).GetEnumerator();
            while (__enumerator6.MoveNext())
            {
                var bagData = __enumerator6.Current;
                {
                    func(bagData, packItems);
                }
            }
        }
        packItems.Sort(ComparerItem);
        DataModel.PackItems = new ObservableCollection<EquipItemDataModel>(packItems);
    }

    public void RefreshForItemId(int itemId, List<BagItemDataModel> excludeList)
    {
        var playerBags = PlayerDataManager.Instance.PlayerDataModel.Bags;

        Action<BagItemDataModel, ICollection<EquipItemDataModel>> func = (bagData, items) =>
        {
            if (itemId == bagData.ItemId && !excludeList.Contains(bagData, new BagItemDataModelComParer()))
            {
                var equipItem = new EquipItemDataModel();
                equipItem.BagItemData = bagData;
                items.Add(equipItem);
            }
        };

        var equipItems = new ObservableCollection<EquipItemDataModel>();
        {
            PlayerDataManager.Instance.ForeachEquip(bagData => { func(bagData, equipItems); });
        }
        DataModel.EquipItems = equipItems;

        var packItems = new List<EquipItemDataModel>();
        {
            var __enumerator6 = (playerBags.Bags[(int) eBagType.Equip].Items).GetEnumerator();
            while (__enumerator6.MoveNext())
            {
                var bagData = __enumerator6.Current;
                {
                    func(bagData, packItems);
                }
            }
        }
        packItems.Sort(ComparerItem);
        DataModel.PackItems = new ObservableCollection<EquipItemDataModel>(packItems);
    }

    public void RefreshForItemType(BagItemDataModel bagItem)
    {
        var playerBags = PlayerDataManager.Instance.PlayerDataModel.Bags;
        var tbItemBase = Table.GetItemBase(bagItem.ItemId);

        var equipItems = new List<EquipItemDataModel>();
        {
            PlayerDataManager.Instance.ForeachEquip(bagData =>
            {
                if (bagData.ItemId != -1 && bagItem != bagData)
                {
                    var tb = Table.GetItemBase(bagData.ItemId);
                    if (tbItemBase == null || tb.Type == tbItemBase.Type)
                    {
                        var equipItem = new EquipItemDataModel();
                        equipItem.BagItemData = bagData;
                        equipItems.Add(equipItem);
                    }
                }
            });
        }
        DataModel.EquipItems = new ObservableCollection<EquipItemDataModel>(equipItems);

        var packItems = new List<EquipItemDataModel>();
        {
            var __enumerator6 = (playerBags.Bags[(int) eBagType.Equip].Items).GetEnumerator();
            while (__enumerator6.MoveNext())
            {
                var item = __enumerator6.Current;
                {
                    if (item.ItemId != -1 && bagItem != item)
                    {
                        var tb = Table.GetItemBase(item.ItemId);
                        if (tbItemBase == null || tb.Type == tbItemBase.Type)
                        {
                            var equipItem = new EquipItemDataModel();
                            equipItem.BagItemData = item;
                            packItems.Add(equipItem);
                        }
                    }
                }
            }
        }
        packItems.Sort(ComparerItem);
        DataModel.PackItems = new ObservableCollection<EquipItemDataModel>(packItems);
    }

    public void RefreshSelectFlag(BagItemDataModel bagItemData)
    {
        {
            // foreach(var item in DataModel.EquipItems)
            var __enumerator4 = (DataModel.EquipItems).GetEnumerator();
            while (__enumerator4.MoveNext())
            {
                var item = __enumerator4.Current;
                {
                    item.SelectFlag = (item.BagItemData.BagId == bagItemData.BagId
                                       && item.BagItemData.Index == bagItemData.Index);
                    if (item.SelectFlag)
                    {
                        DataModel.TogglePack = 0;
                    }
                }
            }
        }
        {
            // foreach(var item in DataModel.PackItems)
            var __enumerator5 = (DataModel.PackItems).GetEnumerator();
            while (__enumerator5.MoveNext())
            {
                var item = __enumerator5.Current;
                {
                    item.SelectFlag = (item.BagItemData.BagId == bagItemData.BagId
                                       && item.BagItemData.Index == bagItemData.Index);
                    if (item.SelectFlag)
                    {
                        DataModel.TogglePack = 1;
                    }
                }
            }
        }
    }

    public void CleanUp()
    {
        DataModel = new EquipPackDataModel();
    }

    public void RefreshData(UIInitArguments data)
    {
        var args = data as EquipPackArguments;
        if (args == null)
        {
            return;
        }
        if (args.RefreshForEvoEquip)
        {
            RefreshForEvoEquip();
        }
        else
        {
            Refresh();
        }
        if (args.DataModel == null)
        {
            return;
        }
        //
        var start = 0;
        // foreach(var packItem in DataModel.EquipItems)
        var __enumerator2 = (DataModel.EquipItems).GetEnumerator();
        while (__enumerator2.MoveNext())
        {
            var packItem = __enumerator2.Current;
            {
                if (packItem.BagItemData.BagId == args.DataModel.BagId
                    && packItem.BagItemData.Index == args.DataModel.Index)
                {
                    DataModel.TogglePack = 0;
//                     EquipUIPackStart e = new EquipUIPackStart(0, start);
//                     EventDispatcher.Instance.DispatchEvent(e);
                    return;
                }
                start++;
            }
        }

        start = 0;
        var __enumerator3 = (DataModel.PackItems).GetEnumerator();
        while (__enumerator3.MoveNext())
        {
            var packItem = __enumerator3.Current;
            {
                if (packItem.BagItemData.BagId == args.DataModel.BagId
                    && packItem.BagItemData.Index == args.DataModel.Index)
                {
                    DataModel.TogglePack = 1;
//                     EquipUIPackStart e = new EquipUIPackStart(1, start);
//                     EventDispatcher.Instance.DispatchEvent(e);
                    return;
                }
                start++;
            }
        }
        DataModel.TogglePack = 0;
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public void Close()
    {
    }

    public void OnShow()
    {
        var start = 0;
        // foreach(var packItem in DataModel.EquipItems)
        var __enumerator2 = (DataModel.EquipItems).GetEnumerator();
        while (__enumerator2.MoveNext())
        {
            var packItem = __enumerator2.Current;
            {
                if (packItem.SelectFlag)
                {
                    var e = new EquipUIPackStart(0, start);
                    EventDispatcher.Instance.DispatchEvent(e);
                    return;
                }
                start++;
            }
        }

        start = 0;
        var __enumerator3 = (DataModel.PackItems).GetEnumerator();
        while (__enumerator3.MoveNext())
        {
            var packItem = __enumerator3.Current;
            {
                if (packItem.SelectFlag)
                {
                    var e = new EquipUIPackStart(1, start);
                    EventDispatcher.Instance.DispatchEvent(e);
                    return;
                }
                start++;
            }
        }
    }

    public void Tick()
    {
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        if (name == "RefreshSelectFlag")
        {
            RefreshSelectFlag((BagItemDataModel) param[0]);
        }
        else if (name == "RefreshForItemType")
        {
            RefreshForItemType((BagItemDataModel) param[0]);
        }
        else if (name == "RefreshForEquipInherit")
        {
            RefreshForEquipInherit((BagItemDataModel) param[0], (BagItemDataModel) param[1]);
        }
        else if (name == "RefreshForItemId")
        {
            RefreshForItemId((int) param[0], (List<BagItemDataModel>) param[1]);
        }
        else if (name == "RefreshForSameEquipUpdateId")
        {
            RefreshForSameEquipUpdateId((int)param[0], (List<BagItemDataModel>)param[1]);
        }
        else if (name == "RefreshForEvoEquip")
        {
            RefreshForEvoEquip();
        }
        else if (name == "Refresh")
        {
            Refresh();
        }
        return null;
    }

    public FrameState State { get; set; }
}