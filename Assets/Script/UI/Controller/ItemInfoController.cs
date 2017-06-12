#region using

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ClientDataModel;
using ClientService;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using Shared;
using UnityEngine;

#endregion

public class ItemInfoController : IControllerBase
{
    public const int GET_PATH_COUNT = 5; //获取途径总个数 

    public ItemInfoController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(UIEvent_ItemInfoFrame_BtnAffirmClick.EVENT_TYPE, OnClickAffrim);
        EventDispatcher.Instance.AddEventListener(ItemInfoOperate.EVENT_TYPE, OnItemInfoOperate);
        EventDispatcher.Instance.AddEventListener(ItemInfoCountChange.EVENT_TYPE, OnItemInfoCountChange);
        EventDispatcher.Instance.AddEventListener(Event_ItemInfoClick.EVENT_TYPE, ItemGetClick);
        EventDispatcher.Instance.AddEventListener(UIEvent_BagItemCountChange.EVENT_TYPE, OnBagItemCountChange);
    }

    public ItemInfoDataModel DataModel { get; set; }
    private readonly Dictionary<int, string> plantType = new Dictionary<int, string>();
    public Coroutine ButtonPress { get; set; }

    public IEnumerator ButtonAddOnPress(int type)
    {
        var pressCd = 0.25f;
        while (true)
        {
            yield return new WaitForSeconds(pressCd);
            if (OnAdd(type) == false)
            {
                NetManager.Instance.StopCoroutine(ButtonPress);
                ButtonPress = null;
                yield break;
            }
            if (pressCd > 0.01)
            {
                pressCd = pressCd*0.8f;
            }
        }
        yield break;
    }

    public IEnumerator ButtonDelOnPress(int type)
    {
        var pressCd = 0.25f;
        while (true)
        {
            yield return new WaitForSeconds(pressCd);
            if (OnDel(type) == false)
            {
                NetManager.Instance.StopCoroutine(ButtonPress);
                ButtonPress = null;
                yield break;
            }
            if (pressCd > 0.01)
            {
                pressCd = pressCd*0.8f;
            }
        }
        yield break;
    }

    //随从蛋提示
    public void InitPetEgg(ItemInfoDataModel dataModel)
    {
        var tbItemBase = Table.GetItemBase(dataModel.ItemData.ItemId);
        if (tbItemBase != null)
        {
            var petid = tbItemBase.Exdata[0];
            var tbPet = Table.GetPet(petid);
            var petTime = GameUtils.GetTimeDiffString(tbItemBase.Exdata[1]*60, true);
            var str = "";
            for (var i = 0; i < tbPet.Ladder; i++)
            {
                str += GameUtils.StarIcon;
            }
            dataModel.Tips = string.Format(tbItemBase.Desc, tbPet.Name, str, petTime);
        }
    }

    //随从魂魄提示
    public void InitPetSoul(ItemInfoDataModel dataModel)
    {
        var tbItemBase = Table.GetItemBase(dataModel.ItemData.ItemId);
        if (tbItemBase != null)
        {
            var count = 0;
            var petid = tbItemBase.Exdata[2];
            var tbPet = Table.GetPet(petid);
            var items = PlayerDataManager.Instance.EnumBagItem((int) eBagType.Pet);
            foreach (var item in items)
            {
                if (null != item && petid == item.ItemId)
                {
                    count = item.Exdata[PetItemExtDataIdx.FragmentNum];
                    break;
                }
            }

            if (tbPet != null)
            {
                dataModel.Tips = string.Format(tbItemBase.Desc, tbPet.Name, tbItemBase.Exdata[1], count);
            }
        }
    }

    public void InitSeedInfo(ItemInfoDataModel dataModel)
    {
        var tbPlant = Table.GetPlant(dataModel.ItemData.ItemId);
        if (tbPlant == null)
        {
            return;
        }
        DataModel.SeedLimit = tbPlant.PlantLevel;
        var level = (int) UIManager.Instance.GetController(UIConfig.FarmUI).CallFromOtherClass("GetBuildLevel", null);
        if (tbPlant.PlantLevel > level)
        {
            DataModel.SeedColor = MColor.white;
        }
        else
        {
            DataModel.SeedColor = GameUtils.GetTableColor(0);
        }
        var type = tbPlant.PlantType;
        if (plantType.ContainsKey(type))
        {
            dataModel.SeedType = plantType[type];
        }
        var str1 = "";
        var str2 = "";
        var num1 = tbPlant.MatureCycle/60;
        var num2 = tbPlant.MatureCycle%60;
        if (num1 > 0)
        {
            str1 = num1 + GameUtils.GetDictionaryText(1040);
        }
        if (num2 > 0)
        {
            str2 = num2 + GameUtils.GetDictionaryText(1041);
        }
        dataModel.SeedCircle = str1 + str2;
        if (tbPlant.HarvestCount[0] == tbPlant.HarvestCount[1])
        {
            dataModel.SeedCount = tbPlant.HarvestCount[0].ToString();
        }
        else
        {
            dataModel.SeedCount = tbPlant.HarvestCount[0] + "-" + tbPlant.HarvestCount[1];
        }
    }

    public static void InitTreasureMap(ItemInfoDataModel dataModel)
    {
        var exData = dataModel.ItemData.Exdata;
        var sceneId = exData[0];
        if (sceneId == 0)
        {
//未初始化的藏宝图
            dataModel.Tips = GameUtils.GetDictionaryText(210500);
        }
        else
        {
            var tbScene = Table.GetScene(sceneId);
            if (tbScene == null)
            {
                Logger.Error("InitTreasureMap(), tbScene == null!!!!");
                return;
            }
            var tip = Table.GetItemBase(dataModel.ItemData.ItemId).Desc;
            dataModel.Tips = string.Format(tip, tbScene.Name, exData[1], exData[2]);
        }
    }

    public void ItemGetClick(IEvent ievent)
    {
        var e = ievent as Event_ItemInfoClick;
        var item = DataModel.GetPathList[e.Index];
        var tbItemGet = Table.GetItemGetInfo(item.ItemGetId);
        if (tbItemGet.IsShow == -1) //开启条件
        {
            EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.ItemInfoUI));
            GameUtils.GotoUiTab(tbItemGet.UIName, tbItemGet.Param[0], tbItemGet.Param[1], tbItemGet.Param[2]);
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
        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.ItemInfoUI));
        GameUtils.GotoUiTab(tbItemGet.UIName, tbItemGet.Param[0], tbItemGet.Param[1], tbItemGet.Param[2]);
    }

    public bool OnAdd(int type)
    {
        if (type == 1)
        {
            if (DataModel.SellCount < DataModel.ItemData.Count)
            {
                DataModel.SellCount++;
                DataModel.SellRate = (float) (DataModel.SellCount - 1)/(DataModel.ItemData.Count - 1);
                return true;
            }
        }
        else if (type == 2)
        {
            if (DataModel.UseCount < DataModel.ItemData.Count)
            {
                DataModel.UseCount++;
                DataModel.UseRate = (float) (DataModel.UseCount - 1)/(DataModel.ItemData.Count - 1);
                return true;
            }
        }
        else if (type == 3)
        {
            if (DataModel.RecycleCount < DataModel.ItemData.Count)
            {
                DataModel.RecycleCount++;
                DataModel.RecycleRate = (float) (DataModel.RecycleCount - 1)/(DataModel.ItemData.Count - 1);
                return true;
            }
        }
        return false;
    }

    private void OnBagItemCountChange(IEvent ievent)
    {
        var e = ievent as UIEvent_BagItemCountChange;
        var myItem = DataModel.ItemData;
        if (e.ItemId != myItem.ItemId)
        {
            return;
        }
        DataModel.SellCount = myItem.Count;
    }

    public void OnClickAffrim(IEvent e)
    {
        var ee = e as UIEvent_ItemInfoFrame_BtnAffirmClick;
        if (ee.Type == 0)
        {
            NetManager.Instance.StartCoroutine(SellItemCorotion());
        }
        else if (ee.Type == 1)
        {
            GameUtils.UseItem(DataModel);
        }
        else if (ee.Type == 2)
        {
            NetManager.Instance.StartCoroutine(RecycleItemCorotion());
        }
        else if (ee.Type == 3)
        {
            DataModel.IsShowRecycleMessage = false;
        }
    }

    public void OnClickGet()
    {
        var tbItem = Table.GetItemBase(DataModel.ItemData.ItemId);
        if (tbItem == null)
        {
            return;
        }
        if (tbItem.GetWay != -1)
        {
            DataModel.IsShowGetPath = true;
            DataModel.GetPathList.Clear();
            var list = new List<ItemGetPathDataModel>();
            for (var i = 0; i < ItemInfoGetController.GET_PATH_COUNT; i++)
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

    public void OnClickGetClose()
    {
        DataModel.IsShowGetPath = false;
    }

    public void OnClickRecycle()
    {
        DataModel.RecycleCount = DataModel.ItemData.Count;
        DataModel.RecycleRate = 1.0f;

        if (DataModel.ItemData.Count == 1)
        {
            var str = GameUtils.GetDictionaryText(710);
            var tbItem = Table.GetItemBase(DataModel.ItemData.ItemId);
            if (tbItem.CallBackType == -1)
            {
                str = GameUtils.GetDictionaryText(240167);
                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, str, "", () => { });
                return;
            }
            str = string.Format(str, tbItem.Name, tbItem.CallBackPrice);
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, str, "",
                () => { NetManager.Instance.StartCoroutine(RecycleItemCorotion()); });
        }
        else
        {
            DataModel.IsShowRecycleMessage = true;
        }
    }

    public void OnCountChange(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "SellRate")
        {
            DataModel.SellCount = (int) (Mathf.Round(DataModel.SellRate*(DataModel.ItemData.Count - 1)) + 1);
        }
        else if (e.PropertyName == "UseRate")
        {
            DataModel.UseCount = (int) (Mathf.Round(DataModel.UseRate*(DataModel.ItemData.Count - 1)) + 1);
        }
        else if (e.PropertyName == "RecycleRate")
        {
            DataModel.RecycleCount = (int) (Mathf.Round(DataModel.RecycleRate*(DataModel.ItemData.Count - 1)) + 1);
        }
    }

    public bool OnDel(int type)
    {
        if (type == 1)
        {
            if (DataModel.SellCount > 1)
            {
                DataModel.SellCount--;
                DataModel.SellRate = (float) (DataModel.SellCount - 1)/(DataModel.ItemData.Count - 1);
                return true;
            }
        }
        else if (type == 2)
        {
            if (DataModel.UseCount > 1)
            {
                DataModel.UseCount--;
                DataModel.UseRate = (float) (DataModel.UseCount - 1)/(DataModel.ItemData.Count - 1);
                return true;
            }
        }
        else
        {
            if (DataModel.RecycleCount > 1)
            {
                DataModel.RecycleCount--;
                DataModel.RecycleRate = (float) (DataModel.RecycleCount - 1)/(DataModel.ItemData.Count - 1);
                return true;
            }
        }
        return false;
    }

    public void OnItemInfoCountChange(IEvent ievent)
    {
        var e = ievent as ItemInfoCountChange;
        //Type 2 使用 1 出售  3 回收
        //Index 0 Add Click 1 Del Click 2 Add Press 3 Del Press 4 Add Release 5 Del Release
        var type = e.Type;
        if (type == 0)
        {
            if (e.Index == 0)
            {
                if (DataModel.UseCount < DataModel.ItemData.Count)
                {
                    DataModel.UseCount++;
                }
            }
            else if (e.Index == 1)
            {
                if (DataModel.UseCount > 0)
                {
                    DataModel.UseCount--;
                }
            }
        }
        else
        {
            if (e.Index == 0)
            {
                OnAdd(type);
            }
            else if (e.Index == 1)
            {
                OnDel(type);
            }
            else if (e.Index == 2) //2 Add Press
            {
                ButtonPress = NetManager.Instance.StartCoroutine(ButtonAddOnPress(type));
            }
            else if (e.Index == 3) //3 Del Press
            {
                ButtonPress = NetManager.Instance.StartCoroutine(ButtonDelOnPress(type));
            }
            else if (e.Index == 4) //Add Release
            {
                if (ButtonPress != null)
                {
                    NetManager.Instance.StopCoroutine(ButtonPress);
                    ButtonPress = null;
                }
            }
            else if (e.Index == 5) //Del Release
            {
                if (ButtonPress != null)
                {
                    NetManager.Instance.StopCoroutine(ButtonPress);
                    ButtonPress = null;
                }
            }
        }
    }

    public void OnItemInfoOperate(IEvent ievent)
    {
        var e = ievent as ItemInfoOperate;
        switch (e.Type)
        {
            case 1:
            {
                OnUseItem();
            }
                break;
            case 2:
            {
                OnClickRecycle();
            }
                break;
            case 3:
            {
                if ((DataModel.ItemData.ItemId >= 30000) && (DataModel.ItemData.ItemId < 40000))
                {
                    EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.ItemInfoUI));
                    EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.HandBook));
                }
            }
                break;
            case 4:
            {
                OnClickGet();
            }
                break;
            case 5:
            {
                OnClickGetClose();
            }
                break;
        }
    }

    public void OnUseItem()
    {
        var tbItem = Table.GetItemBase(DataModel.ItemData.ItemId);
        if (tbItem.CanUse == 1 || DataModel.ItemData.Count == 1)
        {
//单个使用，或者物品数量就是1直接使用
            GameUtils.UseItem(DataModel);
        }
        else if (tbItem.CanUse == 2)
        {
            var e = new ItemInfoNotifyEvent(0);
            EventDispatcher.Instance.DispatchEvent(e);
        }
    }

    public IEnumerator RecycleItemCorotion()
    {
        using (var blockingLayer = new BlockingLayerHelper(0))
        {
            var item = DataModel.ItemData;
            var msg = NetManager.Instance.RecycleBagItem(item.BagId, item.ItemId, item.Index, DataModel.RecycleCount);
            yield return msg.SendAndWaitUntilDone();

            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    //回收成功
                    var e1 = new ShowUIHintBoard(270110);
                    EventDispatcher.Instance.DispatchEvent(e1);
                    var e = new Close_UI_Event(UIConfig.ItemInfoUI);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                Logger.Info(string.Format("SellItemCorotion....State = {0} ErroeCode = {1}", msg.State, msg.ErrorCode));
            }
        }
    }

    public IEnumerator SellItemCorotion()
    {
        using (var blockingLayer = new BlockingLayerHelper(0))
        {
            var item = DataModel.ItemData;
            var count = DataModel.SellCount;
            var msg = NetManager.Instance.SellBagItem(item.BagId, item.ItemId, item.Index, DataModel.SellCount);
            yield return msg.SendAndWaitUntilDone();

            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var e1 = new ShowUIHintBoard(223003);
                    EventDispatcher.Instance.DispatchEvent(e1);

                    if (item.Count == count || item.Count == 0)
                    {
//完全卖完数量才关闭窗口
                        item.Count = 0;
                        var e = new Close_UI_Event(UIConfig.ItemInfoUI);
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                    else
                    {
                        item.Count -= count;
                    }
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                Logger.Info(string.Format("SellItemCorotion....State = {0} ErroeCode = {1}", msg.State, msg.ErrorCode));
            }
        }
    }

    public void CleanUp()
    {
        if (DataModel != null)
        {
            DataModel.PropertyChanged -= OnCountChange;
        }
        DataModel = new ItemInfoDataModel();

        DataModel.PropertyChanged += OnCountChange;
        plantType.Clear();
        plantType.Add(0, GameUtils.GetDictionaryText(240210));
        plantType.Add(1, GameUtils.GetDictionaryText(240211));
        plantType.Add(2, GameUtils.GetDictionaryText(240212));
        plantType.Add(3, GameUtils.GetDictionaryText(240214));
        plantType.Add(4, GameUtils.GetDictionaryText(240215));
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

    public void Close()
    {
        if (UIManager.GetInstance().GetController(UIConfig.ChestInfoUI).State == FrameState.Open)
        {
            DataModel.IsTips = 0;
        }
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        var args = data as ItemInfoArguments;
        if (args == null)
        {
            return;
        }

        var showType = args.ShowType;
        DataModel.IsShowRecycleMessage = false;

        if (args.DataModel != null)
        {
            var dataModel = args.DataModel;
            DataModel.ItemData.Clone(dataModel);
            DataModel.Tips = Table.GetItemBase(DataModel.ItemData.ItemId).Desc;
            if (DataModel.ItemData.Count != 0)
            {
                DataModel.SellCount = dataModel.Count;
                DataModel.SellRate = (float) (DataModel.SellCount)/DataModel.ItemData.Count;
                DataModel.UseCount = dataModel.Count;
                DataModel.UseRate = (float) (DataModel.UseCount)/DataModel.ItemData.Count;
                DataModel.RecycleCount = dataModel.Count;
                DataModel.RecycleRate = (float) (DataModel.RecycleCount)/DataModel.ItemData.Count;
            }
            else
            {
//没有数量就单纯显示吧
                DataModel.IsTips = 1;
            }

            if (showType == (int) eEquipBtnShow.Share || showType == (int) eEquipBtnShow.None)
            {
                DataModel.IsTips = 1;
            }
            else
            {
                DataModel.IsTips = 0;
            }
        }
        else
        {
            DataModel.ItemData.Reset();
            DataModel.ItemData.ItemId = args.ItemId;
            DataModel.Tips = Table.GetItemBase(DataModel.ItemData.ItemId).Desc;

            DataModel.SellCount = 1;
            DataModel.UseCount = 1;
            DataModel.SellRate = 0.0f;
            DataModel.UseRate = 0.0f;
            DataModel.RecycleCount = 1;
            DataModel.RecycleRate = 0.0f;
            if (showType == (int) eEquipBtnShow.Share || showType == (int) eEquipBtnShow.None)
            {
                DataModel.IsTips = 1;
            }
            else
            {
                DataModel.IsTips = 0;
            }
        }

        var tbItem = Table.GetItemBase(DataModel.ItemData.ItemId);
        if (tbItem == null)
        {
            return;
        }

        var type = Table.GetItemType(tbItem.Type);
        if (type == null)
        {
            return;
        }
        if (tbItem.Sell > 0 || tbItem.CallBackPrice > 0)
        {
            DataModel.ShowSellInfo = true;
        }
        else
        {
            DataModel.ShowSellInfo = false;
        }

        if (tbItem.CanUse == 1)
        {
            DataModel.UseCount = 1;
        }

        DataModel.CallBackType = tbItem.CallBackType;
        switch (tbItem.Type)
        {
            case 21000: //技能书
            {
                GameUtils.InitSkillBook(DataModel);
            }
                break;
            case 26300: //藏宝图
            {
                InitTreasureMap(DataModel);
            }
                break;
            case 90000: //
            {
                InitSeedInfo(DataModel);
            }
                break;
            case 70000: //随从魂魄
            {
                InitPetSoul(DataModel);
            }
                break;
            case 26000: //随从蛋
            case 26100: //随从蛋
            {
                InitPetEgg(DataModel);
            }
                break;
        }

        var tbIype = Table.GetItemType(tbItem.Type);

        //等级
        if (tbItem.UseLevel > PlayerDataManager.Instance.GetLevel())
        {
            DataModel.LevelColor = MColor.red;
        }
        else
        {
            DataModel.LevelColor = MColor.green;
        }
        //职业
        var role = tbItem.OccupationLimit;

        if (role != -1)
        {
            var tbCharacter = Table.GetCharacterBase(role);
            var roleType = PlayerDataManager.Instance.GetRoleId();
            if (tbCharacter != null)
            {
                if (roleType != role)
                {
                    DataModel.ProfessionColor = MColor.red;
                }
                else
                {
                    DataModel.ProfessionColor = MColor.green;
                }
                DataModel.ProfessionLimit = tbCharacter.Name;
            }
        }
        else
        {
            DataModel.ProfessionLimit = GameUtils.GetDictionaryText(220700);
            DataModel.ProfessionColor = MColor.green;
        }

        for (var i = 0; i < DataModel.ShowList.Count; i++)
        {
            DataModel.ShowList[i] = BitFlag.GetLow(tbIype.Info, i);
        }
        //显示获取途径
        DataModel.IsShowGetPath = false;
        if (tbItem.GetWay != -1)
        {
            DataModel.ShowList[13] = true;
            OnClickGet();
        }
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public FrameState State { get; set; }
}