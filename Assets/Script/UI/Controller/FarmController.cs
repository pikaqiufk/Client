#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using Shared;
using UnityEngine;

#endregion

public class FarmController : IControllerBase
{
    public static FarmOrderData EmptyData = new FarmOrderData();

    public FarmController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(CityDataInitEvent.EVENT_TYPE, OnCityDataInit); // 初始化农场数据
        EventDispatcher.Instance.AddEventListener(FarmOperateEvent.EVENT_TYPE, OnFarmOperate); //农场操作函数
        EventDispatcher.Instance.AddEventListener(FarmLandCellClick.EVENT_TYPE, OnFarmLandCellClick); //农场土地消息处理
        EventDispatcher.Instance.AddEventListener(FarmMenuDragEvent.EVENT_TYPE, OnFarmMenuDrag); //设置被拖拽的物体的index
        EventDispatcher.Instance.AddEventListener(FarmMenuClickEvent.EVENT_TYPE, OnFarmMenuClick); //菜单物体被点击
        EventDispatcher.Instance.AddEventListener(FarmOrderListClick.EVENT_TYPE, OnFarmOrderListClick); // 订单list点击事件

        EventDispatcher.Instance.AddEventListener(ExDataInitEvent.EVENT_TYPE, OnExDataInit); //初始化ExData
        EventDispatcher.Instance.AddEventListener(ExDataUpDataEvent.EVENT_TYPE, OnUpdateExData); //同步ExData
        EventDispatcher.Instance.AddEventListener(VipLevelChangedEvent.EVENT_TYPE, OnVipLevelChanged); //vip level改变了

        EventDispatcher.Instance.AddEventListener(CityWeakNoticeRefreshEvent.EVENT_TYPE, OnWeakNoticeRefresh);
    }

    private FarmArguments _farmArguments;
    public BuildingData BuildingData;
    public FarmDataModel DataModel;
    public Dictionary<int, int> dicNum_Level;
    public Dictionary<int, int> DicOrdIdToCount = new Dictionary<int, int>(); //农场订单物品
    //public int SeedPage = 1;  //种子菜单页面pageindex
    private int HarvestCount;
    private readonly List<PetSkillRecord> mPetSkill = new List<PetSkillRecord>();
    public int mSelectIndex = -1;
    public BuildingRecord mTbBuilding;
    public BuildingServiceRecord mTbBuildingService;
    //----------------------------------------------------------------------------------Notice-----------------------------
    public object NoticeFarmTrigger;
    public int OrderMaxCount;
    public Coroutine RefreshCoroutine;
    //查找最近时间设置，刷新订单
    public void AnalyseMissionTime()
    {
        var minTime = Game.Instance.ServerTime;
        var bFind = false;
        var count = DataModel.Orders.Count;
        for (var i = 0; i < count; i++)
        {
            var orderData = DataModel.Orders[i];
            if (orderData.State != (int) CityMissionState.Wait)
            {
                continue;
            }
            var freshTime = orderData.RefresTime;
            if (bFind == false)
            {
                bFind = true;
                minTime = freshTime;
            }
            else
            {
                if (minTime > freshTime)
                {
                    minTime = freshTime;
                }
            }
        }

        if (RefreshCoroutine != null)
        {
            NetManager.Instance.StopCoroutine(RefreshCoroutine);
            RefreshCoroutine = null;
        }

        if (bFind)
        {
            DataModel.MinRefreshTime = minTime;
            RefreshCoroutine = NetManager.Instance.StartCoroutine(RefreshOrder());
        }
    }

    //刷新农场标志
    public void AnalyseNoticeFarm()
    {
        if (NoticeFarmTrigger != null)
        {
            TimeManager.Instance.DeleteTrigger(NoticeFarmTrigger);
            NoticeFarmTrigger = null;
        }
        var isMature = false;
        var minSec = -1;
        var count = DataModel.CropLand.Count;
        for (var i = 0; i < count; i++)
        {
            var land = DataModel.CropLand[i];
            if (land == null)
            {
                continue;
            }
            if (land.Type == -1)
            {
                continue;
            }

            if (land.State == (int) LandState.Mature)
            {
                isMature = true;
                var tbItem = Table.GetItemBase(land.Type);
                if (tbItem != null)
                {
                    PlayerDataManager.Instance.NoticeData.FarmTotalIcon = tbItem.Icon;
                }
                break;
            }
            if (land.State == (int) LandState.Growing)
            {
                var dif = (int) (land.MatureTime - Game.Instance.ServerTime).TotalSeconds;
                if (dif <= 0)
                {
                    var tbItem = Table.GetItemBase(land.Type);
                    if (tbItem != null)
                    {
                        PlayerDataManager.Instance.NoticeData.FarmTotalIcon = tbItem.Icon;
                    }
                    isMature = true;
                    break;
                }
                if (minSec == -1 || minSec > dif)
                {
                    minSec = dif;
                }
            }
        }

        PlayerDataManager.Instance.NoticeData.FarmTotal = isMature;
        if (isMature == false)
        {
            if (minSec != -1)
            {
                //等待scends刷新标志
                NoticeFarmTrigger = TimeManager.Instance.CreateTrigger(Game.Instance.ServerTime.AddSeconds(minSec + 1),
                    () => { AnalyseNoticeFarm(); });
                //NetManager.Instance.StartCoroutine(AnalyseNoticeFarmCoroutine(minSec));
            }
        }
    }

    //申请建筑数据
    public void ApplyCityBuildingData()
    {
        NetManager.Instance.StartCoroutine(ApplyCityBuildingDataCoroutine());
    }

    //申请建筑数据
    public IEnumerator ApplyCityBuildingDataCoroutine()
    {
        if (BuildingData == null)
        {
            yield break;
        }
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ApplyCityBuildingData(BuildingData.AreaId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var index = 0;
                    foreach (var data in CityManager.Instance.BuildingDataList)
                    {
                        if (data.AreaId == BuildingData.AreaId)
                        {
                            break;
                        }
                        index++;
                    }
                    CityManager.Instance.BuildingDataList[index] = msg.Response;
                    RefreshBuildingData(msg.Response);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error("ApplyCityBuildingData............ErrorCode..." + msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("ApplyCityBuildingData............State..." + msg.State);
            }
        }
    }

    //0type 0=提交 1=放弃 2 花费去刷新
    public IEnumerator CityMissionCoroutine(int index, int type, int cost)
    {
        using (new BlockingLayerHelper(0))
        {
            var orderData = DataModel.SelectOrder;
            var msg = NetManager.Instance.CityMissionOperation(type, index, cost);

            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    switch (type)
                    {
                        case 0:
                        {
                            PlatformHelper.Event("city", "farmOrderComplete");
                            EventDispatcher.Instance.DispatchEvent(new FarmOrderFlyAnim());
                            PlayerDataManager.Instance.BagItemCountChange(msg.Response);
                            var keys = DicOrdIdToCount.Keys.ToList();
                            foreach (var i in keys)
                            {
                                DicOrdIdToCount[i] = PlayerDataManager.Instance.GetItemCount(i);
                            }
                            //   var responseCount= msg.Response.BagsChange.
                            //foreach (var ii in msg.Response.BagsChange)
                            //{
                            //    var value = ii.Value;
                            //    foreach (var ss in value.ItemsChange)
                            //    {
                            //        var value2 = ss.Value;
                            //        if (DicOrdIdToCount.ContainsKey(value2.ItemId))
                            //        {
                            //            DicOrdIdToCount[value2.ItemId] = value2.Count;
                            //        }
                            //    }
                            //}
                            SetOrderWaiteTime(index);
                            RefreshSelectOrder();
                            SetOrderCanDeliver();
                        }
                            break;
                        case 1:
                        {
                            //不在用这个放弃
                        }
                            break;
                        case 2:
                        {
//购买成功刷新一次
                            NetManager.Instance.StartCoroutine(CityRefreshMissionCoroutine(-1));
                        }
                            break;
                    }
                }
                else if (msg.ErrorCode == (int) ErrorCodes.ItemNotEnough)
                {
                    GameUtils.ShowNetErrorHint(msg.ErrorCode);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_DataOverflow
                         || msg.ErrorCode == (int) ErrorCodes.Unknow
                         || msg.ErrorCode == (int) ErrorCodes.Error_CityMissionNotFind
                         || msg.ErrorCode == (int) ErrorCodes.Error_CityMissionState
                         || msg.ErrorCode == (int) ErrorCodes.Error_CityMissionTime)
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    NetManager.Instance.StartCoroutine(CityRefreshMissionCoroutine(-1));
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error("CityMissionOperation............ErrorCode..." + msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("CityMissionOperation............State..." + msg.State);
            }
        }
    }

    //请求、刷新订单
    public IEnumerator CityRefreshMissionCoroutine(int type)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.CityRefreshMission(type);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var missions = msg.Response.CityMissions;
                    mSelectIndex = -1;
                    RefreshMissionList(missions);
                    AnalyseMissionTime();
                    RefreshSelectOrder();
                    SetOrderCanDeliver();
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error("CityRefreshMissionCoroutine............ErrorCode..." + msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("CityRefreshMissionCoroutine............State..." + msg.State);
            }
        }
    }

    public void CloseLaneMenu()
    {
        DataModel.LandMenuData.Index = -1;
    }

    //删除订单
    public IEnumerator DropMissionCoroutine(int index)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.DropCityMission(index);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    ResetMissionone(msg.Response, index);
                    RefreshSelectOrder();
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_CityMissionFreeCd)
                {
                    SetOrderWaiteTime(index);
                    RefreshSelectOrder();
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_DataOverflow
                         || msg.ErrorCode == (int) ErrorCodes.Error_CityMissionState
                         || msg.ErrorCode == (int) ErrorCodes.Error_BuildNotFind)
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    NetManager.Instance.StartCoroutine(CityRefreshMissionCoroutine(-1));
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error("DropCityMission............ErrorCode..." + msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("DropCityMission............State..." + msg.State);
            }
        }
    }

    private int FixPlantNeedTime(PlantRecord tbPlant)
    {
        var TimeRef = 0;

        foreach (var skill in mPetSkill)
        {
            if (tbPlant.PlantType == skill.Param[0] && skill.EffectId == 2)
            {
                if (0 != skill.Param[1])
                {
                    TimeRef += skill.Param[1];
                }
            }
        }
        return TimeRef;
    }

    private bool GetWeakNoticeState()
    {
        //空地
        var c = DataModel.CropLand.Count;
        var ret = false;
        for (var i = 0; i < c; i++)
        {
            var land = DataModel.CropLand[i];
            if (land.State == (int) LandState.Blank)
            {
                ret = true;
                break;
            }
        }

        if (ret == false)
        {
            return false;
        }

        //种子
        var bag = PlayerDataManager.Instance.GetBag((int) eBagType.FarmDepot);
        if (bag == null)
        {
            return false;
        }

        var c2 = bag.Items.Count;
        for (var i = 0; i < c2; i++)
        {
            var item = bag.Items[i];
            if (item.ItemId < 1)
            {
                continue;
            }
            var table = Table.GetItemBase(item.ItemId);
            if (table == null)
            {
                continue;
            }
            if (table.Type == 90000)
            {
                return true;
            }
        }

        return false;
    }

    //若干秒后设置作物为成熟期
    public IEnumerator LandMatureTimer(int scends, FarmCropDataModel data)
    {
        yield return new WaitForSeconds(scends);
        data.State = (int) LandState.Mature;
    }

    //菜单翻页
    public void MenuPageUp()
    {
        if (DataModel.LandMenuData.TotalPage >= DataModel.LandMenuData.SeedPage + 1)
        {
            DataModel.LandMenuData.SeedPage++;
        }
        else if (DataModel.LandMenuData.TotalPage < DataModel.LandMenuData.SeedPage + 1)
        {
            DataModel.LandMenuData.SeedPage = 1;
        }
        SetMenuItemList(DataModel.LandMenuData.MenuList);
    }

    //服务器返回数据赋值到订单数据
    public void MissionDataToOrder(BuildMissionOne missionOne, FarmOrderData orderData)
    {
        var missionId = missionOne.MissionId;
        orderData.OrderId = missionId;
        //orderData.Index = index;
        var itemCount = missionOne.ItemIdList.Count;
        if (itemCount > 6)
        {
            Logger.Error("物品太多了 配置有问题~~");
            itemCount = 6;
        }
        for (var i = 0; i < itemCount; i++)
        {
            var id = missionOne.ItemIdList[i];
            var count = missionOne.ItemCountList[i];
            orderData.OrderItems[i].ItemId = id;
            orderData.OrderItems[i].Count = count;
            DicOrdIdToCount[id] = PlayerDataManager.Instance.GetItemCount(id);
        }
        for (var i = itemCount; i < 6; i++)
        {
            orderData.OrderItems[i].ItemId = -1;
            orderData.OrderItems[i].Count = 0;
        }
        orderData.Gold = missionOne.GiveMoney;
        orderData.Exp = missionOne.GiveExp;
        orderData.ItemId = missionOne.GiveItem;
        orderData.State = missionOne.State;
        orderData.RefresTime = Extension.FromServerBinary(missionOne.RefreshTime);
    }

    // 初始化农场数据
    public void OnCityDataInit(IEvent ievent)
    {
        //var miss = CityManager.Instance.BuildingMissionList;
        // RefreshMission(miss);


        BuildingData = null;
        {
            // foreach(var buildingData in CityManager.Instance.BuildingDataList)
            var __enumerator6 = (CityManager.Instance.BuildingDataList).GetEnumerator();
            while (__enumerator6.MoveNext())
            {
                var buildingData = __enumerator6.Current;
                {
                    var typeId = buildingData.TypeId;
                    var tbBuild = Table.GetBuilding(typeId);
                    if (tbBuild == null)
                    {
                        continue;
                    }
                    if (tbBuild.Type == 2)
                    {
                        BuildingData = buildingData;
                        break;
                    }
                }
            }
        }
        if (BuildingData != null)
        {
            RefreshBuildingData(BuildingData);
        }
    }

    //删除订单
    public void OnClickOrderDelect()
    {
        if (DataModel.RemainCount > 0)
        {
            //今日订单免费刷新次数还有N次，是否刷新这个订单？
            var str = string.Format(GameUtils.GetDictionaryText(270241), DataModel.RemainCount);
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, str, "",
                () => { NetManager.Instance.StartCoroutine(DropMissionCoroutine(mSelectIndex)); });
        }
        else
        {
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 270242, "", () =>
            {
                //今日订单免费刷新次数已经用完，是否删除这个订单？
                NetManager.Instance.StartCoroutine(DropMissionCoroutine(mSelectIndex));
            });
        }
    }

    //刷新订单
    public void OnClickOrderRefresh(int index)
    {
        var orderData = DataModel.Orders[index];
        if (orderData.State != (int) CityMissionState.Wait)
        {
            return;
        }
        var dif = orderData.RefresTime - Game.Instance.ServerTime;
        var totalCost = (int) Math.Ceiling((float) dif.TotalSeconds/(60.0f*5))*GameUtils.OrderRefreshCost;

        if (totalCost <= 0)
        {
            NetManager.Instance.StartCoroutine(CityMissionCoroutine(index, 2, 0));
        }
        else
        {
            //是否花费{0}钻石立刻获得订单？
            var dicStr = GameUtils.GetDictionaryText(270098);
            var str = string.Format(dicStr, totalCost);
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, str, "", () =>
            {
                if (totalCost > PlayerDataManager.Instance.GetRes((int) eResourcesType.DiamondRes))
                {
                    var e = new ShowUIHintBoard(210102);
                    EventDispatcher.Instance.DispatchEvent(e);
                    return;
                }
                NetManager.Instance.StartCoroutine(CityMissionCoroutine(index, 2, totalCost));
            });
        }
    }

    public void OnClickOrderRefreshMinTime()
    {
        var count = DataModel.Orders.Count;
        var index = -1;
        for (var i = 0; i < count; i++)
        {
            var orderData = DataModel.Orders[i];
            if (orderData.State != (int) CityMissionState.Wait)
            {
//有不在等待中的
                SetSelectOrder(i);
                break;
            }
            if (orderData.RefresTime == DataModel.MinRefreshTime)
            {
                index = i;
            }
        }
        if (index == -1)
        {
            index = 0;
        }

        OnClickOrderRefresh(index);
    }

    //订单提交
    public void OnClickOrderSubmit()
    {
        if (mSelectIndex < 0 || mSelectIndex >= DataModel.Orders.Count)
        {
            return;
        }
        var order = DataModel.Orders[mSelectIndex];
        if (order.State == (int) CityMissionState.Wait)
        {
            return;
        }
        for (var i = 0; i < order.OrderItems.Count; i++)
        {
            var itemId = order.OrderItems[i].ItemId;
            if (itemId == -1)
            {
                continue;
            }
            var itemCount = order.OrderItems[i].Count;
            var count = PlayerDataManager.Instance.GetItemCount(itemId);
            if (count < itemCount)
            {
                //材料不足
                var e = new ShowUIHintBoard(210101);
                EventDispatcher.Instance.DispatchEvent(e);
                return;
            }
        }
        NetManager.Instance.StartCoroutine(CityMissionCoroutine(mSelectIndex, 0, 0));
    }

    //同步ExData
    public void OnExDataInit(IEvent ievent)
    {
        var tbVip = PlayerDataManager.Instance.TbVip;
        DataModel.RemainCount = PlayerDataManager.Instance.GetExData(325) + tbVip.FarmAddRefleshCount;
    }

    //农场土地处理 点击、 被拖拽到它上面
    public void OnFarmLandCellClick(IEvent ievent)
    {
        var e = ievent as FarmLandCellClick;
        if (e.IsDraging)
        {
            OnFarmLandCellDrag(e.Index);
        }
        else
        {
            OnFarmLandCellClick(e.Index);
        }
    }

    //土地点击事件
    public void OnFarmLandCellClick(int index)
    {
        var menu = DataModel.LandMenuData;
        menu.SeedPage = 1;
        if (menu.Index == index)
        {
            return;
        }
        var landData = DataModel.CropLand[index];

        var state = (LandState) landData.State;
        switch (state)
        {
            case LandState.Lock:
                break;
            case LandState.Blank:
            {
                ShowSeedMenu(index);
            }
                break;
            case LandState.Growing:
            {
                ShowGrowMenu(index);
            }
                break;
            case LandState.Mature:
            {
                ShowMatureMenu(index);
            }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    //菜单物体拖拽处理
    public void OnFarmLandCellDrag(int index)
    {
        var landData = DataModel.CropLand[index];
        var state = (MenuState) DataModel.LandMenuData.State;
        var menuData = DataModel.LandMenuData.MenuList[DataModel.LandMenuData.DragIndex];
        switch (state)
        {
            case MenuState.Invalid:
                break;
            case MenuState.Seed:
            {
                if (landData.State != (int) LandState.Blank)
                {
                    return;
                }
                if (menuData.Data == 0)
                {
                    return;
                }

                UseBuildService(index, OperateType.Seed, menuData.Id);
            }
                break;
            case MenuState.Growing:
            {
                if (landData.State != (int) LandState.Growing)
                {
                    return;
                }
                if (menuData.Data == 0)
                {
                    return;
                }
                UseBuildService(index, OperateType.Speedup, menuData.Id);
            }
                break;
            case MenuState.Mature:
            {
                if (landData.State != (int) LandState.Mature)
                {
                    return;
                }
                UseBuildService(index, OperateType.Mature);
            }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    //菜单物体被点击
    public void OnFarmMenuClick(IEvent ievent)
    {
        var e = ievent as FarmMenuClickEvent;
        var menu = DataModel.LandMenuData;
        var index = e.Index + menu.ItemList.Count*(menu.SeedPage - 1);
        if (index >= menu.MenuList.Count)
        {
            Logger.Error("OnFarmMenuClick  index >= menu.MenuList.Count");
            return;
        }
        var menuIndex = index;
        var landIndex = menu.Index;
        var menuData = menu.MenuList[index];
        var land = DataModel.CropLand[DataModel.LandMenuData.Index];
        menu.PlantId = land.Type;
        if (menu.State == (int) MenuState.Growing
            && land.State == (int) LandState.Growing
            && menuData.Id == -1
            && menuIndex == 0)
        {
            //是否确定要铲除该作物？拆除后将不会收获任何作物
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 270093, "",
                () => { UseBuildService(landIndex, OperateType.Wipeout); });
        }
        menu.Index = -1;
    }

    //设置被拖拽的物体的index
    public void OnFarmMenuDrag(IEvent ievent)
    {
        var e = ievent as FarmMenuDragEvent;
        var menu = DataModel.LandMenuData;
        var index = e.Index + menu.ItemList.Count*(menu.SeedPage - 1);
        if (index >= menu.MenuList.Count)
        {
            Logger.Error("OnFarmMenuClick  index >= menu.MenuList.Count");
            return;
        }
        DataModel.LandMenuData.DragIndex = index;
    }

    //农场操作函数
    public void OnFarmOperate(IEvent ievent)
    {
        var e = ievent as FarmOperateEvent;
        switch (e.Type)
        {
//             case 0:
//             case 1:
//             case 2:
//             case 3:
//             case 4:
//             case 5:
//                 {
//                     BtnBuyAddOrReduce(e.Type);
//                 }
//                 break;
//             case 13:
//                 {
//                     StoreConfrimOk();
//                 }
//                 break;
//             case 14:
//                 {
//                     StoreConfrimCancel();
//                 }
//                 break;
            case 15:
            {
                CloseLaneMenu();
            }
                break;
            case 16:
            {
                var e2 = new CityBulidingNoticeRefresh(BuildingData);
                EventDispatcher.Instance.DispatchEvent(e2);
            }
                break;
            case 17:
            {
                OnClickOrderSubmit();
            }
                break;
            case 18:
            {
                OnClickOrderDelect();
            }
                break;
            case 19:
            {
                OnClickOrderRefreshMinTime();
            }
                break;
            case 20:
            {
                MenuPageUp();
            }
                break;
            //case 21:
            //    {
            //        MenuPageDown();
            //    }
            //    break;
            case 22:
            {
                mSelectIndex = -1;
                RefreshSelectOrder();
                //SetOrderCanDeliver();
            }
                break;
        }
    }

    //----------------------------------------------------------------------------------Store--------------------------
    // 订单list点击事件
    public void OnFarmOrderListClick(IEvent ievent)
    {
        var e = ievent as FarmOrderListClick;
        var index = e.Index;

        var orderData = DataModel.Orders[index];
        if (orderData.State == (int) CityMissionState.Wait)
        {
            OnClickOrderRefresh(index);
            return;
        }
        SetSelectOrder(index);
    }

    //初始化ExData
    public void OnUpdateExData(IEvent ievent)
    {
        var e = ievent as ExDataUpDataEvent;
        if (e.Key == 325)
        {
            var tbVip = PlayerDataManager.Instance.TbVip;
            DataModel.RemainCount = e.Value + tbVip.FarmAddRefleshCount;
        }
    }

    //vip level改变了
    public void OnVipLevelChanged(IEvent ievent)
    {
        var tbVip = PlayerDataManager.Instance.TbVip;
        DataModel.RemainCount = PlayerDataManager.Instance.GetExData(325) + tbVip.FarmAddRefleshCount;
    }

    private void OnWeakNoticeRefresh(IEvent ievent)
    {
        if (BuildingData == null)
        {
            return;
        }
        PlayerDataManager.Instance.WeakNoticeData.Farm = GetWeakNoticeState();
        EventDispatcher.Instance.DispatchEvent(new CityBulidingWeakNoticeRefresh(BuildingData));
    }

    public void RefreshBuildingData(BuildingData buildingData)
    {
        if (buildingData == null)
        {
            return;
        }
        BuildingData = buildingData;
        var buildingDataExdataCount0 = buildingData.Exdata.Count;
        for (var i = 0; i < buildingDataExdataCount0; i++)
        {
            DataModel.CropLand[i].Type = buildingData.Exdata[i];
            if (DataModel.CropLand[i].Type != -1)
            {
                DataModel.CropLand[i].MatureTime = Extension.FromServerBinary(buildingData.Exdata64[i]);
                if (DataModel.CropLand[i].MatureTime < Game.Instance.ServerTime)
                {
                    DataModel.CropLand[i].Index = i;
                    DataModel.CropLand[i].State = (int) LandState.Mature;
                }
                else
                {
                    DataModel.CropLand[i].State = (int) LandState.Growing;
                    DataModel.CropLand[i].Index = i;
                    if (DataModel.CropLand[i].MatureTimer != null)
                    {
                        NetManager.Instance.StopCoroutine(DataModel.CropLand[i].MatureTimer);
                    }
                    var scends = (int) (DataModel.CropLand[i].MatureTime - Game.Instance.ServerTime).TotalSeconds;
                    DataModel.CropLand[i].MatureTimer =
                        NetManager.Instance.StartCoroutine(LandMatureTimer(scends, DataModel.CropLand[i]));
                }
            }
            else
            {
                DataModel.CropLand[i].Index = i;
                DataModel.CropLand[i].Type = -1;
                DataModel.CropLand[i].State = (int) LandState.Blank;
            }
        }

        mTbBuilding = Table.GetBuilding(buildingData.TypeId);
        if (mTbBuilding == null)
        {
            return;
        }
        mTbBuildingService = Table.GetBuildingService(mTbBuilding.ServiceId);
        if (mTbBuildingService == null)
        {
            return;
        }
        var depot = PlayerDataManager.Instance.GetBag((int) eBagType.FarmDepot);

        if (depot == null)
        {
            return;
        }

        if (buildingData.Exdata.Count < mTbBuildingService.Param[0])
        {
            for (var i = buildingData.Exdata.Count; i < mTbBuildingService.Param[0]; i++)
            {
                DataModel.CropLand[i].Type = -1;
                DataModel.CropLand[i].Index = i;
                DataModel.CropLand[i].State = (int) LandState.Blank;
            }
        }

        for (var i = mTbBuildingService.Param[0]; i < 15; i++)
        {
            DataModel.CropLand[i].Type = -1;
            DataModel.CropLand[i].Index = i;
            DataModel.CropLand[i].State = (int) LandState.Lock;
        }


        var index = 0;
        depot.Capacity = mTbBuildingService.Param[2];
        {
            // foreach(var item in depot.Items)
            var __enumerator2 = (depot.Items).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var item = __enumerator2.Current;
                {
                    if (index < mTbBuildingService.Param[2])
                    {
                        item.Status = (int) eBagItemType.UnLock;
                    }
                    else
                    {
                        item.Status = (int) eBagItemType.Lock;
                    }
                    index++;
                }
            }
        }

        AnalyseNoticeFarm();
    }

    //刷新订单列表
    public void RefreshMissionList(List<BuildMissionOne> missionList)
    {
        DicOrdIdToCount.Clear();
        if (RefreshCoroutine != null)
        {
            NetManager.Instance.StopCoroutine(RefreshCoroutine);
            RefreshCoroutine = null;
        }
        var list = new List<FarmOrderData>();
        var index = 0;
        {
            var __list7 = missionList;
            var __listCount7 = __list7.Count;
            for (var __i7 = 0; __i7 < __listCount7; ++__i7)
            {
                var missionOne = __list7[__i7];
                {
                    var orderData = new FarmOrderData();
                    MissionDataToOrder(missionOne, orderData);
                    //orderData.IsSelect = false;
                    index++;
                    list.Add(orderData);
                }
            }
        }
        for (var i = list.Count; i < OrderMaxCount; i++)
        {
            if (i >= 0)
            {
                var orderData = new FarmOrderData();
                orderData.State = (int) CityMissionState.Lock;
                list.Add(orderData);
            }
        }
        DataModel.Orders = new ObservableCollection<FarmOrderData>(list);
        var name = GameUtils.GetDictionaryText(270237);
        for (var i = 0; i < DataModel.Orders.Count; i++)
        {
            var level = -1;
            {
                // foreach(var VARIABLE in dicNum_Level)
                var __enumerator2 = (dicNum_Level).GetEnumerator();
                while (__enumerator2.MoveNext())
                {
                    var VARIABLE = __enumerator2.Current;
                    {
                        if (i < VARIABLE.Key)
                        {
                            if (-1 == level)
                            {
                                level = VARIABLE.Value;
                            }
                            else
                            {
                                level = Math.Min(level, VARIABLE.Value);
                            }
                        }
                    }
                }
            }
            DataModel.Orders[i].OpenName = string.Format(name, level);
        }
    }

    public IEnumerator RefreshOrder()
    {
        yield return new WaitForSeconds((int) (DataModel.MinRefreshTime - Game.Instance.ServerTime).TotalSeconds);
        NetManager.Instance.StartCoroutine(CityRefreshMissionCoroutine(0));
    }

    private void RefreshPetSkillData()
    {
        mPetSkill.Clear();
        if (BuildingData == null)
        {
            return;
        }
        foreach (var pet in BuildingData.PetList)
        {
            if (pet == -1)
            {
                continue;
            }
            var petData = CityManager.Instance.GetPetById(pet);
            if (petData == null)
            {
                continue;
            }

            var level = petData.Exdata[PetItemExtDataIdx.Level];

            var tbPet = Table.GetPet(pet);
            for (var idx = 0; idx < tbPet.Speciality.Length; idx++)
            {
                var skillId = petData.Exdata[PetItemExtDataIdx.SpecialSkill_Begin + idx];
                if (-1 == skillId)
                {
                    continue;
                }

                if (level < tbPet.Speciality[idx])
                {
                    continue;
                }
                var tablePetSkill = Table.GetPetSkill(skillId);
                if (null == tablePetSkill)
                {
                    continue;
                }
                mPetSkill.Add(tablePetSkill);
            }
        }
    }

    //刷新选择的订单
    public void RefreshSelectOrder()
    {
        if (mSelectIndex != -1)
        {
            var orderData = DataModel.Orders[mSelectIndex];
            if (orderData.State == (int) CityMissionState.Wait)
            {
                ResetSelectOrder();
            }
        }
        var count = DataModel.Orders.Count;

        for (var i = 0; i < count; i++)
        {
            var orderData = DataModel.Orders[i];
            if (orderData.State == (int) CityMissionState.Normal)
            {
                SetSelectOrder(i);
                break;
            }
        }
        if (mSelectIndex == -1)
        {
            DataModel.IsAllWaite = true;
        }
        else
        {
            DataModel.IsAllWaite = false;
        }
    }

    //重置单个订单
    public void ResetMissionone(BuildMissionOne mission, int index)
    {
        if (index < 0 || index >= DataModel.Orders.Count)
        {
            return;
        }
        var orderData = DataModel.Orders[index];
        MissionDataToOrder(mission, orderData);
        SetOrderCanDeliver();
    }

    //重置被选中的订单数据
    public void ResetSelectOrder()
    {
        mSelectIndex = -1;
        DataModel.SelectOrder.IsSelect = false;
        DataModel.SelectOrder = EmptyData;
    }

    //设置农场菜单显示的物体
    public void SetMenuItemList(ObservableCollection<FarmLandMenuCell> list)
    {
        var t = 0;
        var menu = DataModel.LandMenuData;
        var itemCount = menu.ItemList.Count;
        for (var j = 0; j < itemCount; j++)
        {
            var item = menu.ItemList[j];
            item.IsEnable = false;
        }

        if (list.Count%itemCount != 0)
        {
            menu.TotalPage = list.Count/itemCount + 1;
        }
        else
        {
            menu.TotalPage = list.Count/itemCount;
        }

        //if ((menu.SeedPage - 1) * itemCount > list.Count)
        //{
        //    menu.SeedPage = 1;
        //}
        for (var j = (menu.SeedPage - 1)*itemCount; j < list.Count; j++)
        {
            if (t < menu.ItemList.Count)
            {
                var item = new FarmLandMenuCell(list[j]);
                menu.ItemList[t] = item;
                t++;
            }
            else
            {
                break;
            }
        }
        if (list.Count > menu.ItemList.Count)
        {
            menu.IsShowUpPage = 1;
        }
        else
        {
            menu.IsShowUpPage = 0;
        }
    }

    //是否显示可提交
    private void SetOrderCanDeliver()
    {
        var deliverCount = 0;
        for (var i = 0; i < DataModel.Orders.Count; i++)
        {
            var orderItems = DataModel.Orders[i].OrderItems;
            var orderCount = orderItems.Count;
            if (DataModel.Orders[i].State == (int) CityMissionState.Wait ||
                DataModel.Orders[i].State == (int) CityMissionState.Lock) //未开启
            {
                continue;
            }
            var canDeliver = true;
            for (var j = 0; j < orderCount; j++)
            {
                var item = orderItems[j];
                if (item.ItemId != -1)
                {
                    if (DicOrdIdToCount[item.ItemId] >= item.Count)
                    {
                    }
                    else
                    {
                        canDeliver = false;
                        break;
                    }
                }
            }
            DataModel.Orders[i].IsCanDeliver = canDeliver;
            if (canDeliver)
            {
                deliverCount++;
            }
        }
        DataModel.DeliverCount = deliverCount;
    }

    //设置订单等待时间
    public void SetOrderWaiteTime(int index)
    {
        var orderData = DataModel.Orders[index];
        orderData.State = (int) CityMissionState.Wait;
        if (mTbBuildingService != null)
        {
            orderData.RefresTime = Game.Instance.ServerTime.AddMinutes(mTbBuildingService.Param[3]);
        }
        ResetSelectOrder();
        AnalyseMissionTime();
    }

    //设置被选的订单数据
    public void SetSelectOrder(int index)
    {
        if (index < 0 || index >= DataModel.Orders.Count)
        {
            return;
        }
        if (mSelectIndex == index)
        {
            return;
        }
        if (DataModel.Orders[index].State == (int) CityMissionState.Lock)
        {
            return;
        }
        var orderData = DataModel.Orders[index];
        mSelectIndex = index;
        DataModel.SelectOrder.IsSelect = false;
        DataModel.SelectOrder = orderData;
        DataModel.SelectOrder.IsSelect = true;
    }

    //显示成长菜单
    public void ShowGrowMenu(int index)
    {
        var menu = DataModel.LandMenuData;
        var landData = DataModel.CropLand[index];
        menu.State = (int) MenuState.Growing;
        var list = new List<FarmLandMenuCell>();
        var cell = new FarmLandMenuCell();
        cell.Index = 0;
        cell.Id = -1;
        var tbPlant = Table.GetPlant(landData.Type);
        if (tbPlant == null)
        {
            return;
        }
        if (tbPlant.CanRemove == 1)
        {
            //铲除
            cell.Name = GameUtils.GetDictionaryText(270095);
            cell.IconId = 1002019;
            list.Add(cell);
        }

        var ii = 1;
        for (var i = 0; i < 4; i++)
        {
            var menuCell = new FarmLandMenuCell();
            var itemId = 91200 + i;
            var count = PlayerDataManager.Instance.GetItemCount(itemId);
            if (count == 0)
            {
                continue;
            }
            var tbItem = Table.GetItemBase(itemId);
            menuCell.Name = tbItem.Name;
            menuCell.Index = ii;
            menuCell.Id = itemId;
            menuCell.IconId = tbItem.Icon;
            menuCell.Data = count;
            menuCell.ItemId = itemId;
            ii++;
            list.Add(menuCell);
        }
        if (list.Count == 0)
        {
            menu.IsShowMenuScroll = false;
        }
        else
        {
            menu.IsShowMenuScroll = true;
        }
        menu.MenuList = new ObservableCollection<FarmLandMenuCell>(list);
        menu.Index = index;

        //FarmMenuCountRefresh ee = new FarmMenuCountRefresh(list.Count);
        //EventDispatcher.Instance.DispatchEvent(ee);

        var land = DataModel.CropLand[menu.Index];
        menu.PlantId = land.Type;
        var sec = (int) (land.MatureTime - Game.Instance.ServerTime).TotalSeconds;
        var e = new FarmMatureRefresh(sec, tbPlant.MatureCycle);
        EventDispatcher.Instance.DispatchEvent(e);
        SetMenuItemList(menu.MenuList);
    }

    //显示收获菜单
    public void ShowMatureMenu(int index)
    {
        var menu = DataModel.LandMenuData;
        menu.State = (int) MenuState.Mature;
        var list = new List<FarmLandMenuCell>();
        var cell = new FarmLandMenuCell();
        cell.Index = 0;
        cell.Id = 0;
        //收获
        cell.Name = GameUtils.GetDictionaryText(270094);
        cell.IconId = 1002020;

        list.Add(cell);

        menu.MenuList = new ObservableCollection<FarmLandMenuCell>(list);
        menu.Index = index;
        var land = DataModel.CropLand[index];
        menu.PlantId = land.Type;
        menu.IsShowMenuScroll = true;
        SetMenuItemList(menu.MenuList);
        //FarmMenuCountRefresh ee = new FarmMenuCountRefresh(list.Count);
        //EventDispatcher.Instance.DispatchEvent(ee);
    }

    //显示种子菜单
    public void ShowSeedMenu(int index)
    {
        var menu = DataModel.LandMenuData;
        menu.State = (int) MenuState.Seed;
        var i = 0;
        var list = new List<FarmLandMenuCell>();
        var ss = mTbBuildingService.Param[1];
        Table.ForeachPlant(recoard =>
        {
            var itemId = recoard.PlantItemID;
            var count = PlayerDataManager.Instance.GetItemCount(itemId);
            if (count == 0)
            {
                return true;
            }
            var tbItem = Table.GetItemBase(itemId);
            if (tbItem == null)
            {
                return true;
            }
            var cell = new FarmLandMenuCell();
            cell.Name = recoard.PlantName;
            cell.Index = i;
            cell.Id = recoard.Id;
            cell.ItemId = itemId;
            cell.IconId = tbItem.Icon;
            cell.Data = count;
            if (recoard.PlantLevel > mTbBuildingService.Param[1])
            {
                return true;
                //cell.IsEnable = false;
            }

            i++;
            list.Add(cell);
            return true;
        });
        if (list.Count == 0)
        {
            //请前往作物商店购买种子
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(270096));
            return;
        }
        menu.IsShowMenuScroll = true;
        menu.MenuList = new ObservableCollection<FarmLandMenuCell>(list);
        menu.Index = index;
        SetMenuItemList(menu.MenuList);
        //FarmMenuCountRefresh ee = new FarmMenuCountRefresh(list.Count);
        //EventDispatcher.Instance.DispatchEvent(ee);
    }

    //农场操作发包
    public void UseBuildService(int index, OperateType type, int dataEx = -1)
    {
        switch (type)
        {
            case OperateType.Seed:
                break;
            case OperateType.Mature:
                break;
            case OperateType.Speedup:
                break;
            case OperateType.Wipeout:
                break;
            default:
                throw new ArgumentOutOfRangeException("type");
        }
        NetManager.Instance.StartCoroutine(UseBuildServiceCoroutine(index, type, dataEx));
    }

    //农场操作发包
    public IEnumerator UseBuildServiceCoroutine(int index, OperateType type, int dataEx)
    {
        using (new BlockingLayerHelper(0))
        {
            var param = new Int32Array();
            param.Items.Add((int) type);
            param.Items.Add(index);

            switch (type)
            {
                case OperateType.Seed:
                {
                    param.Items.Add(dataEx);
                }
                    break;
                case OperateType.Mature:
                    break;
                case OperateType.Speedup:
                {
                    param.Items.Add(dataEx);
                }
                    break;
                case OperateType.Wipeout:
                    break;
                default:
                    break;
            }

            var msg = NetManager.Instance.UseBuildService(BuildingData.AreaId, mTbBuilding.ServiceId, param);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var landData = DataModel.CropLand[index];
                    var data = msg.Response.Data32;
                    switch (type)
                    {
                        case OperateType.Seed:
                        {
                            EventDispatcher.Instance.DispatchEvent(new FarmCellTipEvent(OperateType.Seed, index, dataEx,
                                -1));

                            {
                                // foreach(var menu in DataModel.LandMenuData.MenuList)
                                var __enumerator3 = (DataModel.LandMenuData.MenuList).GetEnumerator();
                                while (__enumerator3.MoveNext())
                                {
                                    var menu = __enumerator3.Current;
                                    {
                                        if (menu.Id == dataEx)
                                        {
                                            menu.Data--;
                                            EventDispatcher.Instance.DispatchEvent(new FramDragRefreshCount(menu.Data,
                                                menu.Index));
                                            break;
                                        }
                                    }
                                }
                            }

                            landData.Type = dataEx;
                            var tbPlant = Table.GetPlant(dataEx);
                            landData.State = (int) LandState.Growing;
                            if (landData.MatureTimer != null)
                            {
                                NetManager.Instance.StopCoroutine(landData.MatureTimer);
                            }
                            var fix = FixPlantNeedTime(tbPlant);
                            var scends = tbPlant.MatureCycle*60;
                            if (fix != 0)
                            {
                                scends = scends*(fix + 10000)/10000;
                            }
                            landData.MatureTime = Game.Instance.ServerTime.AddSeconds(scends);
                            landData.MatureTimer = NetManager.Instance.StartCoroutine(LandMatureTimer(scends, landData));
                            BuildingData.Exdata[index] = landData.Type;
                            BuildingData.Exdata64[index] = landData.MatureTime.ToServerBinary();
                            DataModel.LandMenuData.Index = -1;
                            if (DicOrdIdToCount.ContainsKey(dataEx))
                            {
                                DicOrdIdToCount[dataEx] -= 1;
                            }
                            SetOrderCanDeliver();
                        }
                            break;
                        case OperateType.Mature:
                        {
                            for (int i = 0, imax = data.Count; i < imax; i += 2)
                            {
                                var itemId = DataModel.CropLand[index].Type;
                                PlatformHelper.Event("City", "FarmMature", itemId);
                                var tbPlant = Table.GetPlant(itemId);
                                var ee = new FarmCellTipEvent(OperateType.Mature, index, data[i], data[i + 1]);
                                ee.Exp = tbPlant.GetHomeExp;
                                EventDispatcher.Instance.DispatchEvent(ee);
                                if (DicOrdIdToCount.ContainsKey(data[i]))
                                {
                                    DicOrdIdToCount[data[i]] += data[i + 1];
                                }
                            }
                            landData.State = (int) LandState.Blank;
                            landData.Type = -1;
                            if (landData.MatureTimer != null)
                            {
                                NetManager.Instance.StopCoroutine(landData.MatureTimer);
                                landData.MatureTimer = null;
                            }
                            DataModel.LandMenuData.Index = -1;
                            BuildingData.Exdata[index] = -1;
                            BuildingData.Exdata64[index] = Game.Instance.ServerTime.ToServerBinary();
                            HarvestCount++;
                            if (6 == HarvestCount)
                            {
                                if (PlayerDataManager.Instance.GetFlag(523) &&
                                    !PlayerDataManager.Instance.GetFlag(524))
                                {
                                    EventDispatcher.Instance.DispatchEvent(new UIEvent_ShowPlantDemo());
                                }
                            }
                            SetOrderCanDeliver();
                        }
                            break;
                        case OperateType.Speedup:
                        {
                            EventDispatcher.Instance.DispatchEvent(new FarmCellTipEvent(OperateType.Speedup, index,
                                dataEx,
                                -1));
                            {
                                // foreach(var menu in DataModel.LandMenuData.MenuList)
                                var __enumerator4 = (DataModel.LandMenuData.MenuList).GetEnumerator();
                                while (__enumerator4.MoveNext())
                                {
                                    var menu = __enumerator4.Current;
                                    {
                                        if (menu.Id == dataEx)
                                        {
                                            menu.Data--;
                                            var e = new FramDragRefreshCount(menu.Data, menu.Index);
                                            EventDispatcher.Instance.DispatchEvent(e);
                                            break;
                                        }
                                    }
                                }
                            }

                            PlatformHelper.Event("City", "FarmSpeedUp", dataEx);
                            var tbItem = Table.GetItemBase(dataEx);
                            if (tbItem.Exdata[0] <= 0)
                            {
                                landData.MatureTime = Game.Instance.ServerTime;
                            }
                            else
                            {
                                landData.MatureTime = landData.MatureTime.AddMinutes(-tbItem.Exdata[0]);
                            }
                            if (landData.MatureTimer != null)
                            {
                                NetManager.Instance.StopCoroutine(landData.MatureTimer);
                            }

                            var scends = (int) (landData.MatureTime - Game.Instance.ServerTime).TotalSeconds;

                            if (scends <= 0)
                            {
                                landData.State = (int) LandState.Mature;
                            }
                            else
                            {
                                landData.MatureTimer =
                                    NetManager.Instance.StartCoroutine(LandMatureTimer(scends, landData));
                            }
                            if (index == DataModel.LandMenuData.Index)
                            {
                                var tbPlant = Table.GetPlant(landData.Type);
                                var e = new FarmMatureRefresh(scends, tbPlant.MatureCycle);
                                EventDispatcher.Instance.DispatchEvent(e);
                            }
                            BuildingData.Exdata64[index] = landData.MatureTime.ToServerBinary();
                            DataModel.LandMenuData.Index = -1;
                        }
                            break;
                        case OperateType.Wipeout:
                        {
                            landData.Type = -1;
                            if (landData.MatureTimer != null)
                            {
                                NetManager.Instance.StopCoroutine(landData.MatureTimer);
                                landData.MatureTimer = null;
                            }
                            landData.State = (int) LandState.Blank;
                            var e = new FarmMatureRefresh(0, 0);
                            EventDispatcher.Instance.DispatchEvent(e);
                            BuildingData.Exdata[index] = -1;
                        }
                            break;
                        default:
                            break;
                    }

                    AnalyseNoticeFarm();
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_AlreadyHaveSeed
                         || msg.ErrorCode == (int) ErrorCodes.Error_NeedFarmLevelMore
                         || msg.ErrorCode == (int) ErrorCodes.Error_NotFindSeed
                         || msg.ErrorCode == (int) ErrorCodes.Error_SeedTimeNotOver
                         || msg.ErrorCode == (int) ErrorCodes.Error_NotFindSeed)
                {
                    ApplyCityBuildingData();
                }
                else if (msg.ErrorCode == (int) ErrorCodes.ParamError
                         || msg.ErrorCode == (int) ErrorCodes.Error_DataOverflow
                         || msg.ErrorCode == (int) ErrorCodes.Error_ItemID
                         || msg.ErrorCode == (int) ErrorCodes.ItemNotEnough
                         || msg.ErrorCode == (int) ErrorCodes.Error_ItemNot91000)
                {
                    Logger.Error("UseBuildService............ErrorCode..." + msg.ErrorCode);
                }
                else
                {
                    Logger.Error("UseBuildService............ErrorCode..." + msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("UseBuildService............State..." + msg.State);
            }
        }
    }

    public void CleanUp()
    {
        HarvestCount = 0;
        dicNum_Level = new Dictionary<int, int>();

        Table.ForeachBuilding(tbBuilding =>
        {
            if (tbBuilding.Type == (int) BuildingType.Farm)
            {
                var tableService = Table.GetBuildingService(tbBuilding.ServiceId);
                if (null == tableService)
                {
                    return true;
                }
                var max = tableService.Param[4];
                if (max > OrderMaxCount)
                {
                    OrderMaxCount = max;
                }
                if (dicNum_Level.ContainsKey(max))
                {
                    dicNum_Level[max] = Math.Min(dicNum_Level[max], tbBuilding.Level);
                }
                else
                {
                    dicNum_Level.Add(max, tbBuilding.Level);
                }
            }

            return true;
        });


        if (RefreshCoroutine != null)
        {
            NetManager.Instance.StopCoroutine(RefreshCoroutine);
            RefreshCoroutine = null;
        }
        DataModel = new FarmDataModel();
        for (var i = 0; i < DataModel.LandMenuData.ItemList.Count; i++)
        {
            DataModel.LandMenuData.ItemList[i] = new FarmLandMenuCell();
        }
        DataModel.LandMenuData.SeedPage = 1;
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        if (name == "GetBuildLevel")
        {
            if (mTbBuilding != null)
            {
                return mTbBuilding.Level;
            }
            return 0;
        }
        return null;
    }

    public void OnShow()
    {
        if (_farmArguments == null)
        {
            return;
        }
        RefreshBuildingData(_farmArguments.BuildingData);
        RefreshPetSkillData();
        NetManager.Instance.StartCoroutine(CityRefreshMissionCoroutine(-1));
    }

    public void Close()
    {
        HarvestCount = 0;
        PlayerDataManager.Instance.WeakNoticeData.Farm = false;
        EventDispatcher.Instance.DispatchEvent(new CityBulidingWeakNoticeRefresh(BuildingData));
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        var args = data as FarmArguments;
        if (args == null)
        {
            return;
        }
        _farmArguments = args;
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public FrameState State { get; set; }
}