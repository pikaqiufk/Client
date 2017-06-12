#region using

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ScorpionNetLib;

#endregion

public class RecycleController : IControllerBase
{
    public RecycleController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(UIEvent_RecycleBtn.EVENT_TYPE, BtnRecycle); //回收、出售按钮
        EventDispatcher.Instance.AddEventListener(UIEvent_RecycleGetOK.EVENT_TYPE, BtnGetOK); //回收
        EventDispatcher.Instance.AddEventListener(UIEvent_RecycleGetCancel.EVENT_TYPE, BtnGetCancel); //关闭回收获得页面
        EventDispatcher.Instance.AddEventListener(UIEvent_RecycleItemSelect.EVENT_TYPE, ItemSelect); //背包物品点击事件
        EventDispatcher.Instance.AddEventListener(UIEvent_RecycleArrange.EVENT_TYPE, BtnArrange); //整理
        EventDispatcher.Instance.AddEventListener(UIEvent_BagChange.EVENT_TYPE, OnRefrehEquipBagItemStatus); //刷新背包状态
    }

    public IControllerBase BackPack;
    public int RecycleType; // 0 出售，1回收
    public RecycleDataModal RecycleData { get; set; }
    //添加物品到回收列表中
    public void AddRecycleItem()
    {
        RecycleData.RecycleItem.Clear();
        var list = new List<BagItemDataModel>();

        var baseData = PlayerDataManager.Instance.GetBag((int) eBagType.Equip);
        var count = 0;
        {
            // foreach(var item in baseData.Items)
            var __enumerator4 = (baseData.Items).GetEnumerator();
            while (__enumerator4.MoveNext())
            {
                var item = __enumerator4.Current;
                {
                    if (item.IsChoose)
                    {
                        list.Add(item);
                        count++;
                    }
                }
            }
        }
        //默认回收背包有格子
        var tbBaseType = Table.GetBagBase((int) eBagType.Equip);
        for (var i = count; i < tbBaseType.MaxCapacity; i++)
        {
            var bgItem = new BagItemDataModel();
            bgItem.ItemId = -1;
            list.Add(bgItem);
        }

        RecycleData.RecycleItem = new ObservableCollection<BagItemDataModel>(list);
    }

    //整理背包
    public void BtnArrange(IEvent itevent)
    {
        InitRecycleBags();
        RefleshBags();
        var e = itevent as UIEvent_RecycleArrange;
        var ee = new PackArrangeEventUi(e.TabPage);
        EventDispatcher.Instance.DispatchEvent(ee);
    }

    //关闭回收获得页面
    public void BtnGetCancel(IEvent ivent)
    {
        RecycleData.UIGetShow = 0;
    }

    //回收或出售确认并发包
    public void BtnGetOK(IEvent ivent)
    {
        var TempEquipList = new Int32Array();
        if (RecycleData.RecycleItem.Count <= 0)
        {
            return;
        }
        var RecycleDataRecycleItemCount2 = RecycleData.RecycleItem.Count;
        for (var i = 0; i < RecycleDataRecycleItemCount2; i++)
        {
            if (RecycleData.RecycleItem[i].ItemId != -1)
            {
                TempEquipList.Items.Add(RecycleData.RecycleItem[i].Index);
            }
        }
        //回收物品列表是否大于0
        NetManager.Instance.StartCoroutine(RecycleCoroutine(TempEquipList));
    }

    //回收
    public void BtnRecycle(IEvent ivent)
    {
        var e = ivent as UIEvent_RecycleBtn;
        RecycleType = e.Type;
        if (RecycleType == 0)
        {
            RecycleData.SellOrRecycleStr = GameUtils.GetDictionaryText(270238);
        }
        else
        {
            RecycleData.SellOrRecycleStr = GameUtils.GetDictionaryText(270239);
        }
        var sellAllMoney = 0;
        var MoneyCount = 0;
        var MagicCount = 0;
        RecycleData.RecycleGetItem.Clear();
        var getList = new List<BagItemDataModel>();
        if (RecycleData.RecycleItem[0].ItemId == -1)
        {
            return;
        }
        RecycleData.UIGetShow = 1;
        {
            // foreach(var item in RecycleData.RecycleItem)
            var __enumerator2 = (RecycleData.RecycleItem).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var item = __enumerator2.Current;
                {
                    if (item.ItemId == -1)
                    {
                        continue;
                    }
                    var tbItemBase = Table.GetItemBase(item.ItemId);
                    if (tbItemBase.CallBackType == -1)
                    {
                        continue;
                    }
                    if (tbItemBase.CallBackType == 2) //金币
                    {
                        if (tbItemBase.Sell > 0)
                        {
                            MoneyCount += tbItemBase.Sell;
                        }
                    }
                    else if (tbItemBase.CallBackType == 10) //魔尘
                    {
                        if (tbItemBase.CallBackPrice > 0)
                        {
                            MagicCount += tbItemBase.CallBackPrice;
                        }
                    }

                    if (tbItemBase.Sell > 0)
                    {
                        sellAllMoney += tbItemBase.Sell;
                    }
                    //装备强化道具返回
                    if (item.Exdata.Enchance > 0 && item.Exdata.Enchance < 15)
                    {
                        var tbBlessing = Table.GetEquipBlessing(item.Exdata.Enchance);
                        var tbBlessingCallBackItemLength0 = tbBlessing.CallBackItem.Length;
                        for (var i = 0; i < tbBlessingCallBackItemLength0; i++)
                        {
                            if (tbBlessing.CallBackItem[i] == -1)
                            {
                                continue;
                            }
                            var isExist = false;
                            {
                                // foreach(var getItem in RecycleData.RecycleGetItem)
                                var __enumerator7 = (getList).GetEnumerator();
                                while (__enumerator7.MoveNext())
                                {
                                    var getItem = __enumerator7.Current;
                                    {
                                        if (getItem.ItemId == tbBlessing.CallBackItem[i])
                                        {
                                            getItem.Count += tbBlessing.CallBackCount[i];
                                            isExist = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (!isExist)
                            {
                                var bagitem = new BagItemDataModel();
                                bagitem.ItemId = tbBlessing.CallBackItem[i];
                                bagitem.Count = tbBlessing.CallBackCount[i];
                                getList.Add(bagitem);
                                //RecycleData.RecycleGetItem.Add(bagitem);
                            }
                        }
                    }


                    var tbEquipBase = Table.GetEquipBase(tbItemBase.Exdata[0]);
                    //装备追加道具返回
                    if (item.Exdata.Append > tbEquipBase.AddAttrUpMaxValue)
                    {
                        var tbEquipAdd = Table.GetEquipAdditional1(tbEquipBase.AddIndexID);
                        var tbSkillUpdate = Table.GetSkillUpgrading(tbEquipAdd.AddPropArea);

                        var countIndex = 0;
                        var tbSkillUpdateValuesCount1 = tbSkillUpdate.Values.Count;
                        for (var i = 0; i < tbSkillUpdateValuesCount1; i++)
                        {
                            if (tbSkillUpdate.Values[i] > item.Exdata.Append)
                            {
                                countIndex = i;
                                break;
                            }
                        }
                        var tbSkillUpdate2 = Table.GetSkillUpgrading(tbEquipAdd.CallBackCount);

                        var isExist = false;
                        {
                            // foreach(var getItem in RecycleData.RecycleGetItem)
                            var __enumerator8 = (getList).GetEnumerator();
                            while (__enumerator8.MoveNext())
                            {
                                var getItem = __enumerator8.Current;
                                {
                                    if (getItem.ItemId == tbEquipAdd.CallBackItem)
                                    {
                                        getItem.Count += tbSkillUpdate2.Values[countIndex];
                                        isExist = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (!isExist)
                        {
                            var bagitem = new BagItemDataModel();
                            bagitem.ItemId = tbEquipAdd.CallBackItem;
                            bagitem.Count = tbSkillUpdate2.Values[countIndex];
                            getList.Add(bagitem);
                            //RecycleData.RecycleGetItem.Add(bagitem);
                        }
                    }
                }
            }
        }

        if (RecycleType == 0) //全部出售
        {
            if (sellAllMoney > 0)
            {
                var varitem = new BagItemDataModel();
                varitem.ItemId = 2;
                varitem.Count = sellAllMoney;
                getList.Insert(0, varitem);
                //RecycleData.RecycleGetItem.Insert(0, varitem);
            }
        }
        else //全部回收
        {
            if (MoneyCount > 0)
            {
                var varitem = new BagItemDataModel();
                varitem.ItemId = 2;
                varitem.Count = MoneyCount;
                getList.Insert(0, varitem);
                //RecycleData.RecycleGetItem.Insert(0, varitem);
                //RecycleData.UIMoneyShow = 1;
            }
            //else
            //{
            //    RecycleData.UIMoneyShow = 0;
            //}
            if (MagicCount > 0)
            {
                var varitem = new BagItemDataModel();
                varitem.ItemId = 10;
                varitem.Count = MagicCount;
                getList.Insert(0, varitem);
                //RecycleData.RecycleGetItem.Insert(0, varitem);
            }
        }
        RecycleData.RecycleGetItem = new ObservableCollection<BagItemDataModel>(getList);
        //else
        //{
        //    RecycleData.UIMagicShow = 0;
        //}
        ////回收物品居中显示
        //int gridcount = 0;
        //for (int i = 0; i < RecycleData.RecycleGetItem.Count; i++)
        //{
        //    if (RecycleData.RecycleGetItem[i].ItemId != -1)
        //    {
        //        gridcount++;
        //    }
        //}
        //gridcount = (gridcount+1)/2 -1;
        //gridcount = gridcount < 0 ? 0 : gridcount;
        var ee = new UIEvent_RecycleSetGridCenter();
        //ee.index = gridcount;
        EventDispatcher.Instance.DispatchEvent(ee);
    }

    //物品品质判断，>=紫色提示或追加值大于最大成长值
    public bool EquipQualityJudge(BagItemDataModel BaseItem)
    {
        var tbBaseItem = Table.GetItemBase(BaseItem.ItemId);
        if (tbBaseItem.Quality > 3)
        {
            return true;
        }
        if (BaseItem.Exdata.Enchance > 6)
        {
            return true;
        }
//        var tbEquipItem = Table.GetEquipBase(BaseItem.ItemId);
//         if (BaseItem.Exdata.Append > tbEquipItem.AddAttrUpMaxValue)
//         {
//             return true;
//         }
        return false;
    }

    //初始化背包
    public void InitRecycleBags()
    {
        //默认回收背包有格子
        var tbBaseType = Table.GetBagBase((int) eBagType.Equip);
        if (RecycleData.RecycleItem.Count != tbBaseType.MaxCapacity)
        {
            RecycleData.RecycleItem.Clear();
            var list = new List<BagItemDataModel>();

            for (var i = 0; i < tbBaseType.MaxCapacity; i++)
            {
                var bagItem = new BagItemDataModel();
                bagItem.ItemId = -1;
                list.Add(bagItem);
                //RecycleData.RecycleItem.Add(bagItem);
            }
            RecycleData.RecycleItem = new ObservableCollection<BagItemDataModel>(list);
        }
    }

    //背包物品点击事件
    public void ItemSelect(IEvent ivent)
    {
        var e = ivent as UIEvent_RecycleItemSelect;
        var varItem = e.Item;
        //varItem = PlayerDataManager.Instance.GetItem(e.BagID, e.Index);

        if (e.type == 0)
        {
            if (varItem.IsChoose)
            {
                varItem.IsChoose = false;
                AddRecycleItem();
                return;
            }
            if (!EquipQualityJudge(varItem))
            {
                varItem.IsChoose = true;
                AddRecycleItem();
                return;
            }
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, GameUtils.GetDictionaryText(230200), "", () =>
            {
                varItem.IsChoose = true;
                AddRecycleItem();
            },
                () => { varItem.IsChoose = false; });
        }
        else
        {
            varItem.IsChoose = false;
            AddRecycleItem();
        }
    }

    //刷新背包物品状态
    public void OnRefrehEquipBagItemStatus(IEvent ievent)
    {
        var e = ievent as UIEvent_BagChange;
        if (e.HasType(eBagType.Equip))
        {
            if (State == FrameState.Open)
            {
                PlayerDataManager.Instance.RefreshEquipBagStatus();
            }
        }
    }

    //checkbox变化调用
    public void OnToggleChange(object sender, PropertyChangedEventArgs e)
    {
        var index = 0;
        if (!int.TryParse(e.PropertyName, out index))
        {
            return;
        }
        SetColorSelect(index);
    }

    //回收包
    public IEnumerator RecycleCoroutine(Int32Array TempEquipList)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.RecoveryEquip(RecycleType, TempEquipList);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
					var list = new Int32Array();
					list.Items.Add(2800);
					PlayerDataManager.Instance.SetFlagNet(list);

                    if (RecycleType == 0)
                    {
                        RecycleData.IsSellEffect = true;
                        RecycleData.IsSellEffect = false;

                        for (int i = 0; i < TempEquipList.Items.Count; i++)
                        {
                            var item = PlayerDataManager.Instance.GetItem((int)eBagType.Equip, TempEquipList.Items[i]);
                            if (item != null)
                            {
                                PlatformHelper.UMEvent("EquipSell", item.ItemId.ToString());
                            }
                        }
                    }
                    else
                    {
                        RecycleData.IsRecycleEffect = true;
                        RecycleData.IsRecycleEffect = false;


                        for (int i = 0; i < TempEquipList.Items.Count; i++)
                        { 
                            var item = PlayerDataManager.Instance.GetItem((int)eBagType.Equip, TempEquipList.Items[i]);

                            if (item != null)
                            {
                                PlatformHelper.UMEvent("EquipRecycle", item.ItemId.ToString());
                            }
                        }
                    }


                    var baseData = PlayerDataManager.Instance.GetBag((int) eBagType.Equip);
                    var isFull = baseData.Size == baseData.Capacity;
                    for (var i = 0; i < TempEquipList.Items.Count; i++)
                    {
                        var index = TempEquipList.Items[i];
                        if (index < baseData.Items.Count)
                        {
                            baseData.Items[index].ItemId = -1;
                            baseData.Items[index].IsChoose = false;
                        }
                    }
                    InitRecycleBags();
                    RefleshBags();
                    baseData.Size -= TempEquipList.Items.Count;
                    if (isFull && TempEquipList.Items.Count > 0)
                    {
                        var e = new EquipBagNotFullChange();
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                    RecycleData.UIGetShow = 0;
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                var e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    public void RefleshBags()
    {
        var baseData = PlayerDataManager.Instance.GetBag((int) eBagType.Equip);
        RecycleData.BagItems.Clear();
        var list = new List<BagItemDataModel>();
        for (var i = 0; i < baseData.Items.Count; i++)
        {
            var item = baseData.Items[i];
            if (item.ItemId != -1)
            {
                var tbItem = Table.GetItemBase(item.ItemId);
                if (tbItem != null)
                {
                    if (tbItem.CallBackType <= 0 || tbItem.CallBackPrice <= 0)
                    { // 不可回收
                        continue;
                    }
                }
                list.Add(item);
            }
        }
        var tbBaseType = Table.GetBagBase((int) eBagType.Equip);
        for (var i = list.Count; i < tbBaseType.MaxCapacity; i++)
        {
            var bagItem = new BagItemDataModel();
            bagItem.ItemId = -1;
            list.Add(bagItem);
            //RecycleData.RecycleItem.Add(bagItem);
        }
        RecycleData.BagItems = new ObservableCollection<BagItemDataModel>(list);
    }

    public void SetColorSelect(int index)
    {
        var baseData = PlayerDataManager.Instance.GetBag((int) eBagType.Equip);

        var count = 0;
        {
            // foreach(var item in baseData.Items)
            var __enumerator1 = (baseData.Items).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var item = __enumerator1.Current;
                {
                    if (item.ItemId == -1)
                    {
                        count++;
                        continue;
                    }
                    var tbItemBase = Table.GetItemBase(item.ItemId);
                    if (index == tbItemBase.Quality)
                    {
                        if (RecycleData.ColorSelect[index])
                        {
                            item.IsChoose = true;
                        }
                        else
                        {
                            item.IsChoose = false;
                        }
                    }
                    count++;
                }
            }
        }
        AddRecycleItem();
    }

    public void CleanUp()
    {
        if (RecycleData != null)
        {
            RecycleData.ColorSelect.PropertyChanged -= OnToggleChange;
        }


        BackPack = UIManager.Instance.GetController(UIConfig.BackPackUI);
        RecycleData = new RecycleDataModal();


        RecycleData.ColorSelect.PropertyChanged += OnToggleChange;
    }

    public void RefreshData(UIInitArguments data)
    {
        var arg2 = new BackPackArguments();
        arg2.Tab = 0;
        //BackPack.RefreshData(arg2);
        BackPack.CallFromOtherClass("SetPackType", new object[] {BackPackController.BackPackType.Recycle});
        var RecycleDataColorSelectCount3 = RecycleData.ColorSelect.Count;
        for (var i = 0; i < RecycleDataColorSelectCount3; i++)
        {
            RecycleData.ColorSelect[i] = false;
        }
        InitRecycleBags();
        RecycleData.ColorSelect[0] = true;
        SetColorSelect(0);
        RecycleData.ColorSelect[1] = true;
        SetColorSelect(1);
        RefleshBags();
        var args = data as RecycleArguments;
        if (args == null)
        {
            return;
        }

        if (args.ItemDataModel != null)
        {
            var item = args.ItemDataModel;
            var varItem = PlayerDataManager.Instance.GetItem(item.BagId, item.Index);
            varItem.IsChoose = true;
            AddRecycleItem();
        }
        RecycleData.UIGetShow = 0;
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return RecycleData;
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
        BackPack.CallFromOtherClass("SetPackType", new object[] {BackPackController.BackPackType.Recycle});
    }

    public FrameState State { get; set; }
}