#region using

using System.Collections;
using System.Collections.Generic;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using UnityEngine;

#endregion

public enum PetListFileterType
{
	None = 0,
	Piece = 1 << PetStateType.Piece, //碎片
	NoEmploy = 1 << PetStateType.NoEmploy, //未雇佣
	Idle = 1 << PetStateType.Idle, //已雇佣，目前空闲的
	Mission = 1 << PetStateType.Mission, //正在做任务
	Building = 1 << PetStateType.Building, //正在建筑中
	Hatch = 1 << PetStateType.Hatch, //进阶孵化

	Employ = Idle | Mission | Building | Hatch, //已雇佣的

	All = Piece | NoEmploy | Idle | Mission | Building | Hatch
}

public class CityManager : Singleton<CityManager>
{
    public const int CitySceneType = 1000; //家园场景类型Id
    public const int MissionExpExtIdx = 62; //随从任务经验扩展计数Id
    public const int MissionLevelExtIdx = 63; //随从任务等级扩展计数Id
    public List<BuildingData> BuildingDataList = new List<BuildingData>(); //家园建筑数据
    public List<BuildMissionOne> BuildingMissionList = new List<BuildMissionOne>(); //家园任务数据
    private readonly Dictionary<int, GameObject> BuildingModel = new Dictionary<int, GameObject>(); //家园建筑GameObject
    public bool Inited;
    public List<PetMissionData> PetMissionDataList = new List<PetMissionData>(); //随从任务列表
    //获得任务经验
    public int GetMissionExp()
    {
        var extData = PlayerDataManager.Instance.GetExData();
        if (MissionExpExtIdx >= 0 && MissionExpExtIdx < extData.Count)
        {
            return extData[MissionExpExtIdx];
        }
        return 0;
    }

    //获得任务等级
    public int GetMissionLevel()
    {
        var extData = PlayerDataManager.Instance.GetExData();
        if (MissionLevelExtIdx >= 0 && MissionLevelExtIdx < extData.Count)
        {
            return extData[MissionLevelExtIdx];
        }
        return 0;
    }

    public void Reset()
    {
        BuildingDataList.Clear();
        PetMissionDataList.Clear();
        BuildingModel.Clear();
        Inited = false;
    }

    #region 建筑数据

    //要塞等级
    public int GetMajorLevel()
    {
//         {
//             var __list1 = BuildingDataList;
//             var __listCount1 = __list1.Count;
//             for (int __i1 = 0; __i1 < __listCount1; ++__i1)
//             {
//                 var data = __list1[__i1];
//                 {
//                     var table = Table.GetBuilding(data.TypeId);
//                     if (null != table && BuildingType.BaseCamp == (BuildingType)table.Type)
//                     {
//                         return table.Level;
//                     }
//                 }
//             }
//         }
        if (null != PlayerDataManager.Instance.PlayerDataModel)
        {
            return PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.HomeMaterial1;
        }

        return 0;
    }

    //根据区域id获得建筑
    public BuildingData GetBuildingByAreaId(int idx)
    {
        {
            var __list2 = BuildingDataList;
            var __listCount2 = __list2.Count;
            for (var __i2 = 0; __i2 < __listCount2; ++__i2)
            {
                var data = __list2[__i2];
                {
                    if (data.AreaId == idx)
                    {
                        return data;
                    }
                }
            }
        }
        return null;
    }

    //获得该类型建筑个数
    public int GetBuildingCountByType(BuildingType type)
    {
        var count = 0;
        {
            var __list15 = BuildingDataList;
            var __listCount15 = __list15.Count;
            for (var __i15 = 0; __i15 < __listCount15; ++__i15)
            {
                var building = __list15[__i15];
                {
                    var table = Table.GetBuilding(building.TypeId);
                    if (null == table)
                    {
                        continue;
                    }
                    if ((BuildingType) table.Type == type)
                    {
                        count++;
                    }
                }
            }
        }
        return count;
    }

    public BuildingData GetBuildingDataByType(BuildingType type)
    {
        {
            var __list15 = BuildingDataList;
            var __listCount15 = __list15.Count;
            for (var __i15 = 0; __i15 < __listCount15; ++__i15)
            {
                var building = __list15[__i15];
                {
                    var table = Table.GetBuilding(building.TypeId);
                    if (null == table)
                    {
                        continue;
                    }
                    if ((BuildingType) table.Type == type)
                    {
                        return building;
                    }
                }
            }
        }
        return null;
    }

    //清除建筑模型
    public void ClearBuildingModel()
    {
        {
            // foreach(var pair in BuildingModel)
            var __enumerator3 = (BuildingModel).GetEnumerator();
            while (__enumerator3.MoveNext())
            {
                var pair = __enumerator3.Current;
                {
                    if (null != pair.Value)
                    {
                        Object.Destroy(pair.Value);
                    }
                }
            }
        }
        BuildingModel.Clear();
    }

    //刷新建筑模型
    public void RefreshBuilding(int id, int typeId)
    {
        GameObject buildingModel = null;
        if (BuildingModel.TryGetValue(id, out buildingModel))
        {
            Object.Destroy(buildingModel);
            BuildingModel.Remove(id);
        }

        var tableArea = Table.GetHomeSence(id);
        var tableBuilding = Table.GetBuilding(typeId);
        var tableRes = Table.GetBuildingRes(tableBuilding.ResId);

        ResourceManager.PrepareResource<GameObject>(tableRes.Path, res =>
        {
            buildingModel = Object.Instantiate(res) as GameObject;
            var buildingTransform = buildingModel.transform;
            if (tableArea.PosY >= 0)
            {
                buildingTransform.position = new Vector3(tableArea.PosX, tableArea.PosY, tableArea.PosZ);
            }
            else
            {
                buildingTransform.position = GameLogic.GetTerrainPosition(tableArea.PosX, tableArea.PosZ);
            }

            BuildingModel.Add(id, buildingModel);
        });
    }

    //是否在家园场景里
    public bool IsInCityScene()
    {
        return CitySceneType == GameLogic.Instance.Scene.SceneTypeId;
    }

    //获得当前正在升级或者建造的建筑个数
    public int GetCurrentUpgradingOrBuildingNumber()
    {
        var count = 0;
        {
            var __list16 = BuildingDataList;
            var __listCount16 = __list16.Count;
            for (var __i16 = 0; __i16 < __listCount16; ++__i16)
            {
                var building = __list16[__i16];
                {
                    if (BuildStateType.Building == (BuildStateType) building.State ||
                        BuildStateType.Upgrading == (BuildStateType) building.State)
                    {
                        count++;
                    }
                }
            }
        }
        return count;
    }

    #endregion

    #region 随从

    //枚举所有宠物
    public IEnumerable<BagItemDataModel> EnumAllPet()
    {
        {
            // foreach(var pet in PlayerDataManager.Instance.EnumBagItem((int)eBagType.Pet))
            var __enumerator4 = (PlayerDataManager.Instance.EnumBagItem((int) eBagType.Pet)).GetEnumerator();
            while (__enumerator4.MoveNext())
            {
                var pet = __enumerator4.Current;
                {
                    yield return pet;
                }
            }
        }
    }

    //获得随从
    public BagItemDataModel GetPetById(int petId)
    {
        {
            // foreach(var pet in EnumAllPet())
            var __enumerator5 = (EnumAllPet()).GetEnumerator();
            while (__enumerator5.MoveNext())
            {
                var pet = __enumerator5.Current;
                {
                    if (pet.ItemId == petId)
                    {
                        return pet;
                    }
                }
            }
        }
        return null;
    }

    //获得随从
    public BagItemDataModel GetPetByBagIdx(int idx)
    {
        {
            // foreach(var pet in EnumAllPet())
            var __enumerator5 = (EnumAllPet()).GetEnumerator();
            while (__enumerator5.MoveNext())
            {
                var pet = __enumerator5.Current;
                {
                    if (pet.Index == idx)
                    {
                        return pet;
                    }
                }
            }
        }
        return null;
    }

    #endregion

    #region 随从任务

    //比较
    public static int MissionCompare(PetMissionData a, PetMissionData b)
    {
        var state1 = (PetMissionStateType) a.State;
        var state2 = (PetMissionStateType) b.State;

        if (state1 == PetMissionStateType.Finish)
        {
            return -1;
        }
        if (state2 == PetMissionStateType.Finish)
        {
            return 1;
        }

        var t1 = Extension.FromServerBinary(a.OverTime);
        var t2 = Extension.FromServerBinary(b.OverTime);

        if (t1 < t2)
        {
            return -1;
        }
        return 1;

        return 0;
    }


    //获得某个任务
    public PetMissionData GetPetMission(int missionId)
    {
        {
            var __list6 = PetMissionDataList;
            var __listCount6 = __list6.Count;
            for (var __i6 = 0; __i6 < __listCount6; ++__i6)
            {
                var mission = __list6[__i6];
                {
                    if (mission.Id == missionId)
                    {
                        return mission;
                    }
                }
            }
        }
        return null;
    }

    //设置任务数据
    public bool SetPetMission(int missionId, PetMissionData data)
    {
        var PetMissionDataListCount0 = PetMissionDataList.Count;
        for (var i = 0; i < PetMissionDataListCount0; i++)
        {
            if (PetMissionDataList[i].Id == missionId)
            {
                PetMissionDataList[i] = data;
                return true;
            }
        }


        return false;
    }

    public void UpdatePetMissionState(bool notify = true)
    {
        var changed = false;

        var PetMissionDataListCount0 = PetMissionDataList.Count;
        for (var i = 0; i < PetMissionDataListCount0; i++)
        {
            if (PetMissionStateType.Do == (PetMissionStateType) PetMissionDataList[i].State)
            {
                var time = Extension.FromServerBinary(PetMissionDataList[i].OverTime);
                if (Game.Instance.ServerTime > time)
                {
                    PetMissionDataList[i].State = (int) PetMissionStateType.Finish;
                    changed = true;
                }
            }
        }

        if (changed && notify)
        {
            EventDispatcher.Instance.DispatchEvent(new UIEvent_UpdatePetMissionList());
        }
    }

    #endregion

    #region 网络

    //发送建筑请求
    public void SendCityOptRequest(CityOperationType opt, int buildingIdx, List<int> param = null)
    {
        NetManager.Instance.StartCoroutine(SendCityOptRequestCoroutine(opt, buildingIdx, param));
    }

    private IEnumerator SendCityOptRequestCoroutine(CityOperationType opt,
                                                    int buildingIdx,
                                                    List<int> param)
    {
        using (new BlockingLayerHelper(0))
        {
            var array = new Int32Array();
            if (null != param)
            {
                {
                    var __list7 = param;
                    var __listCount7 = __list7.Count;
                    for (var __i7 = 0; __i7 < __listCount7; ++__i7)
                    {
                        var value = __list7[__i7];
                        {
                            array.Items.Add(value);
                        }
                    }
                }
            }

            var msg = NetManager.Instance.CityOperationRequest((int) opt, buildingIdx, array);
            yield return msg.SendAndWaitUntilDone();

            if (msg.State != MessageState.Reply)
            {
                Logger.Debug("SendBuildRequestCoroutine:MessageState.Timeout");
                yield break;
            }

            if (msg.ErrorCode != (int) ErrorCodes.OK)
            {
                var errorCode = (ErrorCodes) msg.ErrorCode;
                switch (errorCode)
                {
                    case ErrorCodes.Error_LevelNoEnough:
                    {
                        EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(200000108));
                    }
                        break;
                    case ErrorCodes.Error_ResNoEnough:
                    {
                        EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(200002002));
                    }
                        break;
                    case ErrorCodes.Error_BuildCountMax:
                    {
                        EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(200002801));
                    }
                        break;
                    case ErrorCodes.Error_NeedCityLevelMore:
                    {
                        EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(200002803));
                    }
                        break;
                    case ErrorCodes.Error_CityCanotBuildMore:
                    {
                        EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300113));
                    }
                        break;
                    default:
                        UIManager.Instance.ShowNetError(msg.ErrorCode);
                        break;
                }
                Logger.Debug("SendBuildRequestCoroutine error=[{0}]", msg.ErrorCode);

                EventDispatcher.Instance.DispatchEvent(new UIEvent_CityUpdateBuilding(buildingIdx));

                yield break;
            }
            switch (opt)
            {
                case CityOperationType.BUILD:
                {
                    PlatformHelper.Event("city", "Build");
                    //EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300108));
                }
                    break;
                case CityOperationType.UPGRADE:
                {
                    PlatformHelper.Event("city", "Upgrade");
                    EventDispatcher.Instance.DispatchEvent(new UIEvent_OnCityBuildingOptEvent(buildingIdx, opt));
                    //EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300107));
                }
                    break;
                case CityOperationType.DESTROY:
                {
                    //EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard("Destroy Ok"));

                    var e = new CityBuildingLevelupEvent(buildingIdx);
                    var tbBuilding = Table.GetBuilding(GetBuildingByAreaId(buildingIdx).TypeId);
                    e.HomeExp = Table.GetBuilding(tbBuilding.RemovedBuildID).GetMainHouseExp;
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                    break;
                case CityOperationType.ASSIGNPET:
                case CityOperationType.ASSIGNPETINDEX:
                {
                    EventDispatcher.Instance.DispatchEvent(new UIEvent_CityAssignPetEvent(buildingIdx));
                }
                    break;
            }
        }
    }

    //进入家园
    public void EnterCity()
    {
        NetManager.Instance.StartCoroutine(EnterCityCoroutine(-1));
    }

    private IEnumerator EnterCityCoroutine(int typeId)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.EnterCity(typeId);
            yield return msg.SendAndWaitUntilDone();
        }
    }

    //离开家园
    public void LeaveCity()
    {
        NetManager.Instance.StartCoroutine(LeaveCityCoroutine());
    }

    private IEnumerator LeaveCityCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ExitDungeon(-1);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error(".....LeaveCityCoroutine.......{0}.", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error(".....LeaveCityCoroutine.......{0}.", msg.State);
            }
        }
    }

    //操作宠物
    public void OperatePet(int petId, PetOperationType type, int param = 0)
    {
        NetManager.Instance.StartCoroutine(OperatePetCoroutine(petId, type, param));
    }

    private IEnumerator OperatePetCoroutine(int petId, PetOperationType type, int param)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.OperatePet(petId, (int) type, param);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    if (type == PetOperationType.EMPLOY)
                    {
                        EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300206));
                    }
                    else if (type == PetOperationType.FIRE)
                    {
                        EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300205));
                    }
                    else if (type == PetOperationType.RECYCLESOUL)
                    {
                        EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(270110));
                    }
                }
                else
                {
                    Logger.Error(".....OperatePetCoroutine.......{0}.", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error(".....OperatePetCoroutine.......{0}.", msg.State);
            }
        }
    }

    //操作随从任务
    public void OperatePetMission(int missionId, PetMissionOpt type, List<int> param)
    {
        NetManager.Instance.StartCoroutine(OperatePetMissionCoroutine(missionId, type, param));
    }

    private IEnumerator OperatePetMissionCoroutine(int missionId, PetMissionOpt type, List<int> param)
    {
        using (new BlockingLayerHelper(0))
        {
            var array = new Int32Array();
            array.Items.AddRange(param);
            var msg = NetManager.Instance.OperatePetMission(missionId, (int) type, array);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    if (PetMissionOpt.COMPLETE == type)
                    {
                        PlatformHelper.Event("city", "petMissionComplete");
                        var mission = Instance.GetPetMission(missionId);
                        var tbMissionInfo = Table.GetGetMissionInfo(mission.Level);
                        var count = mission.PetCount;
                        if (count > 0 && count <= 3)
                        {
                            EventDispatcher.Instance.DispatchEvent(
                                new UIEvent_PetFlyAnim(tbMissionInfo.HomeExp[count - 1]));
                        }
                        EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300207));
                    }
                    //EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300114));
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error(".....OperatePetMissionCoroutine.......{0}.", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error(".....OperatePetMissionCoroutine.......{0}.", msg.State);
            }
        }
    }

    //使用建筑服务
    public void UseBuildingService(int areaId, int serviceId, List<int> param = null)
    {
        NetManager.Instance.StartCoroutine(UseBuildServiceCoroutine(areaId, serviceId, param));
    }

    private IEnumerator RefreshPush(int type)
    {
        yield return new WaitForSeconds(3);
        EventDispatcher.Instance.DispatchEvent(new UIEvent_RefreshPush(type, 0));
    }

    private IEnumerator UseBuildServiceCoroutine(int areaId, int serviceId, List<int> param)
    {
        using (new BlockingLayerHelper(0))
        {
            var array = new Int32Array();
            if (null != param)
            {
                {
                    var __list8 = param;
                    var __listCount8 = __list8.Count;
                    for (var __i8 = 0; __i8 < __listCount8; ++__i8)
                    {
                        var item = __list8[__i8];
                        {
                            array.Items.Add(item);
                        }
                    }
                }
            }
            var msg = NetManager.Instance.UseBuildService(areaId, serviceId, array);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var building = GetBuildingByAreaId(areaId);
                    var table = Table.GetBuilding(building.TypeId);
                    var type = (BuildingType) table.Type;
                    var count = msg.Response.Data32[0];

                    if (BuildingType.LogPlace == type)
                    {
                        PlatformHelper.Event("city", "LogPlace");
                        if (count <= 0)
                        {
                            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300203));
                        }
                        else
                        {
                            var tbserver = Table.GetBuildingService(table.ServiceId);
                            var exp = tbserver.Param[4]*count/10000;
                            var str = string.Format(GameUtils.GetDictionaryText(300200), count);
                            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(str));
                            EventDispatcher.Instance.DispatchEvent(new CityGetResAnim(areaId, 0, 8, count));
                            if (exp > 0)
                            {
                                EventDispatcher.Instance.DispatchEvent(new CityGetResAnim(areaId, 1, 8, exp));
                            }
                        }
                        EventDispatcher.Instance.DispatchEvent(new CityBulidingNoticeAdd(areaId));
                        PlayerDataManager.Instance.NoticeData.LogPlaceComplete4 = false;

                        //刷新推送
                        NetManager.Instance.StartCoroutine(RefreshPush(11));
                    }
                    else if (BuildingType.Mine == type)
                    {
                        PlatformHelper.Event("city", "Mine");
                        if (count <= 0)
                        {
                            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300203));
                        }
                        else
                        {
                            var tbserver = Table.GetBuildingService(table.ServiceId);
                            var exp = tbserver.Param[4]*count/10000;
                            var str = string.Format(GameUtils.GetDictionaryText(300201), count);
                            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(str));
                            EventDispatcher.Instance.DispatchEvent(new CityGetResAnim(areaId, 0, 9, count));
                            if (exp > 0)
                            {
                                EventDispatcher.Instance.DispatchEvent(new CityGetResAnim(areaId, 1, 9, exp));
                            }
                        }
                        EventDispatcher.Instance.DispatchEvent(new CityBulidingNoticeAdd(areaId));
                        PlayerDataManager.Instance.NoticeData.MineComplete4 = false;

                        //刷新推送
                        NetManager.Instance.StartCoroutine(RefreshPush(10));
                    }
                    else if (BuildingType.MercenaryCamp == type)
                    {
                        if (0 == array.Items[0])
                        {
                            EventDispatcher.Instance.DispatchEvent(new UIEvent_SelectHatchingCell(-1));
                        }
                        else if (1 == array.Items[0])
                        {
                            //完成孵化
                            PlatformHelper.Event("city", "HatchingComplete");
                            EventDispatcher.Instance.DispatchEvent(new UIEvent_HatchingRoomEvent("NewPet", count));
                        }
                    }
                }
                else
                {
                    EventDispatcher.Instance.DispatchEvent(new UIEvent_ErrorTip((ErrorCodes) msg.ErrorCode));
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error(".....UseBuildServiceCoroutine.......{0}.", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error(".....UseBuildServiceCoroutine.......{0}.", msg.State);
            }
        }
    }

    //孵化室抽奖
    public void HatchingRoomDrawLottery(int type)
    {
        NetManager.Instance.StartCoroutine(HatchingRoomDrawLotteryCoroutine(type));
    }

    private IEnumerator HatchingRoomDrawLotteryCoroutine(int type)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.DrawLotteryPetEgg(type);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    {
                        var __list9 = msg.Response.Items;
                        var __listCount9 = __list9.Count;
                        for (var __i9 = 0; __i9 < __listCount9; ++__i9)
                        {
                            var item = __list9[__i9];
                            {
                                //temp
                            }
                        }
                    }
                }
                else
                {
                    EventDispatcher.Instance.DispatchEvent(new UIEvent_ErrorTip((ErrorCodes) msg.ErrorCode));
                    Logger.Error(".....HatchingRoomDrawLotteryCoroutine.......{0}.", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error(".....HatchingRoomDrawLotteryCoroutine.......{0}.", msg.State);
            }
        }
    }

    #endregion

    #region 功能

    //根据过滤筛选出被显示的随从
    public List<BagItemDataModel> GetAllPetByFilter(PetListFileterType filter = PetListFileterType.All)
    {
        var list = new List<BagItemDataModel>();
        {
            // foreach(var pet in EnumAllPet())
            var __enumerator10 = (EnumAllPet()).GetEnumerator();
            while (__enumerator10.MoveNext())
            {
                var pet = __enumerator10.Current;
                {
                    var stateIdx = pet.Exdata[PetItemExtDataIdx.State];
                    //var stateIdx = UnityEngine.Random.RandomRange(0,5); //debug
                    var state = (PetStateType) stateIdx;

                    //判断过滤

                    if (0 == ((int) filter & 1 << stateIdx))
                    {
                        continue;
                    }

                    list.Add(pet);
                }
            }
        }

        return list;
    }

    public BagItemDataModel GetAllPetByFilterItemId(PetListFileterType filter, int itemId)
    {
        var __list5 = Instance.GetAllPetByFilter(PetListFileterType.Employ);
        var __listCount5 = __list5.Count;
        for (var __i5 = 0; __i5 < __listCount5; ++__i5)
        {
            var pet = __list5[__i5];
            if (pet.ItemId == itemId)
            {
                return pet;
            }
        }
        return null;
    }

    private static string StrSoul = string.Empty;
    //数据转换
    public static PetItemDataModel PetItem2DataModel(BagItemDataModel pet, PetItemDataModel petData = null)
    {
        if (string.IsNullOrEmpty(StrSoul))
        {
            StrSoul = GameUtils.GetDictionaryText(626);
        }
        int[] StateString = {611, 603, 600, 602, 601};
        int[] AttrIndex = {5, 6, 7, 8, 10, 11, 13};

        if (null == petData)
        {
            petData = new PetItemDataModel();
        }

        var tablePet = Table.GetPet(pet.ItemId);
        if (null == tablePet)
        {
            return null;
        }
        petData.Name = tablePet.Name;
        petData.PetId = pet.ItemId;
        petData.ItemId = pet.ItemId;
        petData.Type = tablePet.Type;
        petData.BagItemIndex = pet.Index;
        petData.Level = pet.Exdata[PetItemExtDataIdx.Level];
        petData.StarLevel = tablePet.Ladder; //pet.Exdata[PetItemExtDataIdx.StarLevel];
        petData.State = pet.Exdata[PetItemExtDataIdx.State];
        petData.PieceNum = pet.Exdata[PetItemExtDataIdx.FragmentNum];
        petData.MaxNum = tablePet.NeedItemCount;
        var stateIdx = petData.State;
        var state = (PetStateType) stateIdx;

        var str = "";
        if (PetStateType.Piece == state)
        {
            var tableItem = Table.GetItemBase(tablePet.NeedItemId);
            //petData.StarLevel = 0;
            petData.MaxNum = tableItem.Exdata[1];
// 			if (petData.PieceNum >= petData.MaxNum)
// 			{
// 				str = GameUtils.GetDictionaryText(StateString[stateIdx]);
// 			}
        }
        else
        {
            if (PetStateType.Idle == state)
            {
            }
            else if (PetStateType.Mission == state)
            {
                /*
				string missionName = "";
				{
					var __list11 = CityManager.Instance.PetMissionDataList;
					var __listCount11 = __list11.Count;
					for (int __i11 = 0; __i11 < __listCount11; ++__i11)
					{
						var mission = __list11[__i11];
						{
							{
								// foreach(var missionPet in mission.PetList)
								var __enumerator13 = (mission.PetList).GetEnumerator();
								while (__enumerator13.MoveNext())
								{
									var missionPet = __enumerator13.Current;
									{
										if (missionPet == petData.PetId)
										{
											missionName = mission.Name;
											break;
										}
									}
								}
							}
            
							if (!string.IsNullOrEmpty(missionName))
								break;
						}
					}
				}
				str = string.Format("{0}{1}", GameUtils.GetDictionaryText(300127),missionName);
				 * */
                str = GameUtils.GetDictionaryText(300127);
            }
            else if (PetStateType.Building == state)
            {
                var find = false;
                {
                    var __list12 = Instance.BuildingDataList;
                    var __listCount12 = __list12.Count;
                    for (var __i12 = 0; __i12 < __listCount12; ++__i12)
                    {
                        var tempBuilding = __list12[__i12];
                        {
                            {
                                // foreach(var tempPet in tempBuilding.PetList)
                                var __enumerator14 = (tempBuilding.PetList).GetEnumerator();
                                while (__enumerator14.MoveNext())
                                {
                                    var tempPet = __enumerator14.Current;
                                    {
                                        if (tempPet == pet.ItemId)
                                        {
                                            var tableBuilding = Table.GetBuilding(tempBuilding.TypeId);
                                            str = string.Format(GameUtils.GetDictionaryText(StateString[stateIdx]),
                                                tableBuilding.Name);
                                            find = true;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (find)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            //可进阶或者合成的
            if (tablePet.NeedItemCount > 0 && petData.PieceNum >= petData.MaxNum)
            {
                if (PetStateType.Piece == state)
                {
//可进阶
                }
                //str = GameUtils.GetDictionaryText(612);
            }
        }


        petData.Desc = str;
        var myRecord = new PetSkillRecord();
        //技能 
        var templist = new List<int>();
        var PetItemDataModelMaxSkill1 = PetItemDataModel.MaxSkill;
        for (var i = 0; i < PetItemDataModelMaxSkill1; i++)
        {
            petData.Skill.Skills[i].SkillId = tablePet.Skill[i];
            petData.Skill.Skills[i].UnLockLevel = tablePet.ActiveLadder[i];
            var active = petData.StarLevel >= petData.Skill.Skills[i].UnLockLevel;
            petData.Skill.Skills[i].Active = active;
            if (active)
            {
                petData.Skill.Skills[i].Col = MColor.white;
            }
            else
            {
                petData.Skill.Skills[i].Col = MColor.grey;
            }
        }
        //特殊技能
        int[] SkillLevelLimit = {tablePet.Speciality[0], tablePet.Speciality[1], tablePet.Speciality[2]};
        const int PetItemDataModelMaxSkill2 = PetItemDataModel.MaxSkill;
        for (var i = 0; i < PetItemDataModelMaxSkill2; i++)
        {
            petData.Skill.SpecialSkills[i].UnLockLevel = SkillLevelLimit[i];
            if (petData.Level < SkillLevelLimit[i])
            {
                petData.Skill.SpecialSkills[i].Name = "[ff0000]" +
                                                      string.Format(GameUtils.GetDictionaryText(300204),
                                                          SkillLevelLimit[i]) + "[-]";
                ;
                petData.Skill.SpecialSkills[i].Desc = "";
                petData.Skill.SpecialSkills[i].Col = MColor.white;
                petData.Skill.SpecialSkills[i].Active = false;
            }
            else
            {
                petData.Skill.SpecialSkills[i].Col = new Color(0, 1, 1);
                petData.Skill.SpecialSkills[i].SkillId = pet.Exdata[PetItemExtDataIdx.SpecialSkill_Begin + i];
                var tableSkill = Table.GetPetSkill(petData.Skill.SpecialSkills[i].SkillId);
                if (null == tableSkill)
                {
                    petData.Skill.SpecialSkills[i].Name = "";
                    petData.Skill.SpecialSkills[i].Desc = "";
                    petData.Skill.SpecialSkills[i].Active = false;
                }
                else
                {
                    petData.Skill.SpecialSkills[i].Name = tableSkill.Name;
                    petData.Skill.SpecialSkills[i].Desc = tableSkill.Desc;
                    petData.Skill.SpecialSkills[i].Active = true;
                }
            }
        }

        /*
        if (PetStateType.Idle == state)
        {
            petData.ShowCheck = true;
            petData.ShowMask = false;
        }
        else
        {
            petData.ShowCheck = false;
            petData.ShowMask = true;
        }
		*/

        petData.Attributes[0].Type = 1001;
        petData.Attributes[0].Value = FightAttribute.GetPetAttribut(petData.PetId, eAttributeType.PhyPowerMin,
            petData.Level);
        petData.Attributes[0].ValueEx = FightAttribute.GetPetAttribut(petData.PetId, eAttributeType.PhyPowerMax,
            petData.Level);
        petData.Attributes[1].Type = 1002;
        petData.Attributes[1].Value = FightAttribute.GetPetAttribut(petData.PetId, eAttributeType.MagPowerMin,
            petData.Level);
        petData.Attributes[1].ValueEx = FightAttribute.GetPetAttribut(petData.PetId, eAttributeType.MagPowerMax,
            petData.Level);
        petData.Attributes[2].Type = 10;
        petData.Attributes[2].Value = FightAttribute.GetPetAttribut(petData.PetId, eAttributeType.PhyArmor,
            petData.Level);
        petData.Attributes[3].Type = 11;
        petData.Attributes[3].Value = FightAttribute.GetPetAttribut(petData.PetId, eAttributeType.MagArmor,
            petData.Level);
        petData.Attributes[4].Type = 13;
        petData.Attributes[4].Value = FightAttribute.GetPetAttribut(petData.PetId, eAttributeType.HpMax, petData.Level);

        var Ref = new Dictionary<eAttributeType, int>();
        petData.FightPoint = FightAttribute.CalculatePetFightPower(tablePet, petData.Level, Ref);

        return petData;
    }

    public static IEnumerable<int> EnumPetSkill(PetRecord table)
    {
        var tableSkillLength3 = table.Skill.Length;
        for (var i = 0; i < tableSkillLength3; i++)
        {
            if (table.Ladder > table.ActiveLadder[i])
            {
                yield return table.Skill[i];
            }
        }
    }

    #endregion
}