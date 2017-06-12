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
using Shared;
using UnityEngine;

#endregion

public class SailingController : IControllerBase
{
    public static List<Vector2> staticPos = new List<Vector2>(); //港口坐标

    public SailingController()
    {
        #region 勋章初始化

        MedalType = new Dictionary<int, string>();
        MedalType.Add(0, GameUtils.GetDictionaryText(230201));
        MedalType.Add(1, GameUtils.GetDictionaryText(230202));
        MedalType.Add(2, GameUtils.GetDictionaryText(230203));
        MedalType.Add(3, GameUtils.GetDictionaryText(230204));
        MedalType.Add(4, GameUtils.GetDictionaryText(230205));
        MedalType.Add(5, GameUtils.GetDictionaryText(230206));
        MedalType.Add(6, GameUtils.GetDictionaryText(230207));
        MedalType.Add(7, GameUtils.GetDictionaryText(230208));
        MedalType.Add(8, GameUtils.GetDictionaryText(230209));
        MedalType.Add(9, GameUtils.GetDictionaryText(230210));
        MedalType.Add(10, GameUtils.GetDictionaryText(230211));
        MedalType.Add(11, GameUtils.GetDictionaryText(230212));
        MedalType.Add(12, GameUtils.GetDictionaryText(230213));
        MedalType.Add(13, GameUtils.GetDictionaryText(230214));

        #endregion

        for (var i = 0; i < 12; i += 2)
        {
            var tbClientx = Table.GetClientConfig(550 + i);
            var tbClienty = Table.GetClientConfig(551 + i);
            var x = int.Parse(tbClientx.Value);
            var y = int.Parse(tbClienty.Value);
            staticPos.Add(new Vector2(x, y));
        }

        CleanUp();
        EventDispatcher.Instance.AddEventListener(UIEvent_SailingReturnBtn.EVENT_TYPE, OnClickReturnBtn); //返回按钮
        EventDispatcher.Instance.AddEventListener(UIEvent_Sail_ExdataUpdate.EVENT_TYPE, OnExdataUpdate); //返回按钮
        EventDispatcher.Instance.AddEventListener(UIEvent_SailingPackItemUI.EVENT_TYPE, OnClickPackItem); //点击背包物品
        EventDispatcher.Instance.AddEventListener(UIEvent_SailingPickAll.EVENT_TYPE, PickAll); //捡起所有的掉落船饰        
        EventDispatcher.Instance.AddEventListener(UIEvent_SailingPickOne.EVENT_TYPE, PickOne); //捡起单个掉落船饰
        EventDispatcher.Instance.AddEventListener(UIEvent_SailingPutOnClick.EVENT_TYPE, PutOnClick); //穿上船饰
        EventDispatcher.Instance.AddEventListener(UIEvent_SailingLightPoint.EVENT_TYPE, LightPointClick); //开始出航
        EventDispatcher.Instance.AddEventListener(UIEvent_SailingLineButton.EVENT_TYPE, LineButtonClick); //领取结果
        EventDispatcher.Instance.AddEventListener(UIEvent_CityUpdateBuilding.EVENT_TYPE, UpdateBuilding); //更新建筑数据
        EventDispatcher.Instance.AddEventListener(UIEvent_SailingOperation.EVENT_TYPE, SailingOperation); //勇士港操作事件
        EventDispatcher.Instance.AddEventListener(CityDataInitEvent.EVENT_TYPE, OnCityDataInit); // 初始化农场数据
        EventDispatcher.Instance.AddEventListener(UIEvent_SailingLightPointAccess.EVENT_TYPE, LightPointAccessClick); // 直达

        EventDispatcher.Instance.AddEventListener(CityWeakNoticeRefreshEvent.EVENT_TYPE, CityWeakNoticeRefresh);
    }

    public List<int> BaglevelUpList = new List<int>();
    public BuildingData BraveHarborBuild;
    public bool CanSailing;
    public int LeftScanCount = 0;
    private Coroutine mAutoShipCoroutine;
    public Dictionary<int, string> MedalType; //勋章类型
    public int mSelectIndex;
    public int mSelectType = (int) SelectType.BagSelect; //背包选择或者装备选择，为升级显示用
    private object NoticeTriggerr;
    public int ScanCount;
    public double ScanSpeed = 0.06; //设置船速系数
    public string ScanTips270249;
    public string ScanTips270250;
    public MedalItemDataModel SelectedItem; //设置被操作的勋章
    public List<int> TempBagLevelUplist = new List<int>();
    //选择类型，升级用
    public enum SelectType
    {
        BagSelect, //背包选择
        EquipSelect //装备选择
    }

    public int ActionCount { get; set; } //船所走的步数
    public int ActionIndex { get; set; } //船所在的航线index
    public int DataState { get; set; } //船航行的状态：无-1，0正常行驶，购买1，扫荡10
    public bool IsScan { get; set; } //是否在扫荡
    public SailingDataModel SailingData { get; set; }
    //升级物品list创建
    public void AddBagSelectList()
    {
        SailingData.TempData.TotalExp = 0;
        var exp = 0;
        BaglevelUpList.Clear();
        TempBagLevelUplist.Clear();

        var tempitem = SailingData.TempData.TempMedalItem;
        if (tempitem.BaseItemId == -1)
        {
            return;
        }
        var MedalTable = Table.GetMedal(tempitem.BaseItemId);

        exp = tempitem.Exdata.Exp + GetTolalExp(MedalTable.LevelUpExp, tempitem.Exdata.Level);
        var totalExp = GetTolalExp(MedalTable.LevelUpExp, MedalTable.MaxLevel);
        // int dropCount =  SailingData.ShipEquip.DropItem.Count;
        // for (int i = 0; i < dropCount; i++)
        //{
        //    var item = SailingData.ShipEquip.DropItem[i];
        //    if (item.IsShowCheck == 1)
        //    {
        //        if (item.BaseItemId == -1)
        //        {
        //            continue;
        //        }
        //        var ItemBaseTable2 = Table.GetItemBase(item.BaseItemId);
        //        var MedalTable2 = Table.GetMedal(ItemBaseTable2.Exdata[0]);

        //        exp += MedalTable2.InitExp;
        //        exp += item.Exdata.Exp;
        //        exp += GetTolalExp(MedalTable2.LevelUpExp, item.Exdata.Level);
        //       TempBagLevelUplist.Add(i);
        //        if (exp > totalExp)
        //        {
        //            //满经验
        //        }
        //    }
        //}

        var bagCount = SailingData.ShipEquip.BagItem.Count;
        for (var i = 0; i < bagCount; i++)
        {
            var item = SailingData.ShipEquip.BagItem[i];
            if (item.IsShowCheck == 1)
            {
                if (item.BaseItemId == -1)
                {
                    continue;
                }
                var ItemBaseTable2 = Table.GetItemBase(item.BaseItemId);
                var MedalTable2 = Table.GetMedal(ItemBaseTable2.Exdata[0]);

                exp += MedalTable2.InitExp;
                exp += item.Exdata.Exp;
                exp += GetTolalExp(MedalTable2.LevelUpExp, item.Exdata.Level);
                BaglevelUpList.Add(i);
                if (exp > totalExp)
                {
                    //满经验
                }
            }
        }
        SailingData.TempData.TotalExp = exp;
    }

    //吞噬临时包裹的list创建
    public void AddTempBagSelectList(int BestIndex)
    {
        SailingData.TempData.TotalExp = 0;
        var Exp = 0;
        TempBagLevelUplist.Clear();

        var tempitem = SailingData.ShipEquip.DropItem[BestIndex];
        var MedalTable = Table.GetMedal(tempitem.BaseItemId);

        Exp += tempitem.Exdata.Exp;
        Exp += GetTolalExp(MedalTable.LevelUpExp, tempitem.Exdata.Level);

        var totalExp = GetTolalExp(MedalTable.LevelUpExp, MedalTable.MaxLevel + 1);

        var dropCount = SailingData.ShipEquip.DropItem.Count;
        for (var i = 0; i < dropCount; i++)
        {
            var item = SailingData.ShipEquip.DropItem[i];
            if (item.BaseItemId == -1 || i == BestIndex)
            {
                continue;
            }
            var MedalTable2 = Table.GetMedal(item.BaseItemId);
            Exp += MedalTable2.InitExp;
            Exp += item.Exdata.Exp;
            Exp += GetTolalExp(MedalTable2.LevelUpExp, item.Exdata.Level);
            TempBagLevelUplist.Add(i);
            if (SailingData.TempData.TotalExp > totalExp)
            {
                //满经验
                SailingData.TempData.TotalExp = Exp;
                return;
            }
        }
        SailingData.TempData.TotalExp = Exp;
    }

    public void AnalyseNotice()
    {
        var diff = -1;
        CanSailing = false;
        var isNotice = false;
        {
            var __list6 = BraveHarborBuild.Exdata;
            var __listCount6 = __list6.Count;
            var index = 0;
            for (var __i6 = 0; __i6 < __listCount6; ++__i6)
            {
                var i = __list6[__i6];
                {
                    if (index > 4)
                    {
                        break;
                    }
                    if (i == 2 || i == 3 || i == 12)
                    {
                        var overTime = BraveHarborBuild.Exdata64[index];
                        if (overTime == 0)
                        {
                            index++;
                            continue;
                        }
                        var okTime =
                            (int) (Extension.FromServerBinary(overTime) - Game.Instance.ServerTime).TotalSeconds;
                        if (okTime <= 0)
                        {
                            isNotice = true;
                            break;
                        }
                        if (okTime < diff || diff == -1)
                        {
                            diff = okTime;
                        }
                    }
                    if (i == 10)
                    {
                        CanSailing = true;
                    }
                    index++;
                }
            }
        }
        var tbBuilding = Table.GetBuilding(BraveHarborBuild.TypeId);
        if (tbBuilding != null)
        {
            var tbBuildingService = Table.GetBuildingService(tbBuilding.ServiceId);
            PlayerDataManager.Instance.NoticeData.SailingIco = tbBuildingService.TipsIndex;
        }
        PlayerDataManager.Instance.NoticeData.SailingNotice = isNotice;
        EventDispatcher.Instance.DispatchEvent(new CityBulidingNoticeRefresh(BraveHarborBuild));
        if (isNotice)
        {
            return;
        }
        if (NoticeTriggerr != null)
        {
            TimeManager.Instance.DeleteTrigger(NoticeTriggerr);
            NoticeTriggerr = null;
        }

        NoticeTriggerr = TimeManager.Instance.CreateTrigger(Game.Instance.ServerTime.AddSeconds(1 + diff), () =>
        {
            if (NoticeTriggerr != null)
            {
                AnalyseNotice();
                EventDispatcher.Instance.DispatchEvent(new CityBulidingNoticeRefresh(BraveHarborBuild));
                TimeManager.Instance.DeleteTrigger(NoticeTriggerr);
                NoticeTriggerr = null;
            }
        });
    }

    //扫荡七海
    public void AutoShipClick()
    {
        if (IsScan)
        {
            IsScan = !IsScan;
            SetScanName(IsScan);
            return;
        }
        var count = PlayerDataManager.Instance.GetExData((int) eExdataDefine.e71);
        var scanCount = PlayerDataManager.Instance.TbVip.SailScanCount;
        //vip等级不足
        if (scanCount <= 0)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(200000014));
            return;
        }
        //今日扫荡次数已用完
        if (scanCount - count <= 0)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300830));
            return;
        }
        ScanCount = 0;
        {
            // foreach(var medalItemDataModel in SailingData.ShipEquip.DropItem)
            var __enumerator8 = (SailingData.ShipEquip.DropItem).GetEnumerator();
            while (__enumerator8.MoveNext())
            {
                var medalItemDataModel = __enumerator8.Current;
                {
                    if (medalItemDataModel.BaseItemId == -1)
                    {
                        ScanCount++;
                    }
                }
            }
        }
        //判断临时掉落包是否满
        if (ScanCount < 1)
        {
            var ee = new ShowUIHintBoard(302);
            EventDispatcher.Instance.DispatchEvent(ee);
            return;
        }
        SailingData.IsShowWoodTip = true;
    }

    //自动扫荡Coroutine
    public IEnumerator AutoShipClickCoroutine(int count)
    {
        var instance = PlayerDataManager.Instance;
        ;
        var scanDistance = PlayerDataManager.Instance.GetExData(eExdataDefine.e71);
        //int mScanCount = 0;
        for (var i = 0; i < count; i++)
        {
            for (var index = 4; index >= 0; --index)
            {
                var oldValue = BraveHarborBuild.Exdata[index];
                //不可点
                if (oldValue == 0)
                {
                    continue;
                }
                //正在航行
                var needmoney = 0;
                var tbSailing = Table.GetSailing(index);
                if (scanDistance >= instance.TbVip.SailScanCount)
                {
                    IsScan = false;
                    SetScanName(IsScan);
                    mAutoShipCoroutine = null;
                }
                if (oldValue%10 == 2)
                {
                    //判断钱是否够
                    needmoney = GetBuyMoney(index, tbSailing.distanceParam);
                    var resCount = PlayerDataManager.Instance.GetRes(tbSailing.ConsumeType);
                    if (needmoney < 0 || resCount < needmoney)
                    {
                        var ee = new ShowUIHintBoard(270226);
                        EventDispatcher.Instance.DispatchEvent(ee);
                        PlayerDataManager.Instance.ShowItemInfoGet(tbSailing.ConsumeType);
                        IsScan = false;
                        SetScanName(IsScan);
                        mAutoShipCoroutine = null;
                        yield break;
                    }

                    SailingData.Ship[index].Times = "";
                    var prob = GetShipPercent(index);
                    if (prob > 0 && prob < 1)
                    {
                        DataState = 1;
                        ActionIndex = index;
                        ActionCount = 0;
                        while (DataState == 1)
                        {
                            if (!IsScan)
                            {
                                StopShipScan(index);
                                mAutoShipCoroutine = null;
                                yield break;
                            }
                            yield return new WaitForSeconds(0.05f);
                        }
                    }
                }
                else
                //可航行
                {
                    //判断钱是否够
                    if (tbSailing == null)
                    {
                        mAutoShipCoroutine = null;
                        yield break;
                    }
                    needmoney = tbSailing.Distance/tbSailing.distanceParam;
                    var resCount = PlayerDataManager.Instance.GetRes(tbSailing.ConsumeType);
                    if (needmoney < 0 || resCount < needmoney)
                    {
                        var ee = new ShowUIHintBoard(270226);
                        EventDispatcher.Instance.DispatchEvent(ee);
                        PlayerDataManager.Instance.ShowItemInfoGet(tbSailing.ConsumeType);
                        IsScan = false;
                        SetScanName(IsScan);
                        mAutoShipCoroutine = null;
                        yield break;
                    }

                    SailingData.States[index] = 2;
                    SailingData.Ship[index].IsShowShip = true;
                    SailingData.Ship[index].Times = " ";
                    DataState = 10;
                    ActionIndex = index;
                    ActionCount = 0;
                    BraveHarborBuild.Exdata[index] = 0;
                    SetScanName(IsScan);
                    while (DataState == 10)
                    {
                        if (!IsScan)
                        {
                            StopShipScan(index);
                            mAutoShipCoroutine = null;
                            yield break;
                        }
                        yield return new WaitForSeconds(0.05f);
                    }
                }

                //网络包
                var tbBuild = Table.GetBuilding(BraveHarborBuild.TypeId);
                var array = new Int32Array();
                array.Items.Add(3);
                //扫荡第一次，发送1修改服务器exdata + 1
                //if (mScanCount == 0)
                //{
                //    array.Items.Add(1);
                //}
                //else
                //{   
                //    array.Items.Add(0);
                //}
                using (new BlockingLayerHelper(0))
                {
                    var msg = NetManager.Instance.UseBuildService(BraveHarborBuild.AreaId, tbBuild.ServiceId, array);
                    yield return msg.SendAndWaitUntilDone();
                    if (msg.State == MessageState.Reply)
                    {
                        if (msg.ErrorCode == (int) ErrorCodes.OK)
                        {
                            // mScanCount++;
                            var result = msg.Response.Data32[0];
                            var resCount = PlayerDataManager.Instance.GetRes(tbSailing.ConsumeType);
                            PlayerDataManager.Instance.PlayerDataModel.Bags.Resources[tbSailing.ConsumeType] =
                                resCount - needmoney;
                            scanDistance += tbSailing.Distance*10;
                            if (result > 0)
                            {
                                EventDispatcher.Instance.DispatchEvent(new UIEvent_SailingFlyAnim(index,
                                    tbSailing.SuccessGetExp));
                                BraveHarborBuild.Exdata[result] = BraveHarborBuild.Exdata[result]%10 + 10;
                            }
                            else
                            {
                                EventDispatcher.Instance.DispatchEvent(new UIEvent_SailingFlyAnim(index,
                                    tbSailing.FailedGetExp));
                            }
                            if (index == 0)
                            {
                                BraveHarborBuild.Exdata[index] = 10;
                            }
                            else
                            {
                                BraveHarborBuild.Exdata[index] = BraveHarborBuild.Exdata[index]/10*10;
                            }
                            if (index != 4)
                            {
                                SailingData.Ship[index + 1].Posion = new Vector3(staticPos[index + 1].x,
                                    staticPos[index + 1].y, 0);
                            }

                            RefeshBuildData(BraveHarborBuild);
                            if (!IsScan)
                            {
                                StopShipScan(index);
                                mAutoShipCoroutine = null;
                                yield break;
                            }
                        }
                        else if (msg.ErrorCode == (int) ErrorCodes.MoneyNotEnough)
                        {
							var ee = new ShowUIHintBoard(210102);
                            EventDispatcher.Instance.DispatchEvent(ee);
                            StopShipScan(index);
                            mAutoShipCoroutine = null;
                            yield break;
                        }
                        else
                        {
                            UIManager.Instance.ShowNetError(msg.ErrorCode);
                            StopShipScan(index);
                            mAutoShipCoroutine = null;
                            yield break;
                        }
                    }
                    else
                    {
                        var e = new ShowUIHintBoard(220821);
                        EventDispatcher.Instance.DispatchEvent(e);
                        StopShipScan(index);
                        mAutoShipCoroutine = null;
                        yield break;
                    }
                    break;
                }
            }
        }
        IsScan = false;
        SetScanName(IsScan);
        mAutoShipCoroutine = null;
    }

    //购买到港
    public IEnumerator BuyShipCoroutine(int nIndex, int money)
    {
        //动画
        var prob = GetShipPercent(nIndex);
        SailingData.Ship[nIndex].Times = "";
        SailingData.Ship[nIndex].SpeedOrGet = 0;
        // SailingData.AddSpeed[nIndex] = "";
        if (prob > 0 && prob < 1)
        {
            DataState = 1;
            ActionIndex = nIndex;
            ActionCount = 0;
            while (DataState == 1)
            {
                yield return new WaitForSeconds(0.05f);
            }
        }
        //double prob = GetShipPercent(nIndex);
        //if (prob > 0 && prob < 1)
        //{
        //    double diff = 1 - prob;
        //    int count = (int)(diff / 0.1) + 1;
        //    for (int i = 0; i < count; ++i)
        //    {
        //        yield return new WaitForSeconds(0.1f);
        //        ResetShipPercent(nIndex, prob + diff * i / count);
        //    }
        //}
        //网络包
        var tbBuild = Table.GetBuilding(BraveHarborBuild.TypeId);
        var array = new Int32Array();
        array.Items.Add(2);
        array.Items.Add(nIndex);
        array.Items.Add(money);
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.UseBuildService(BraveHarborBuild.AreaId, tbBuild.ServiceId, array);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var result = msg.Response.Data32[0];
                    var tbSailing = Table.GetSailing(nIndex);
                    if (result > 0)
                    {
                        BraveHarborBuild.Exdata[result] = BraveHarborBuild.Exdata[result]%10 + 10;
                        EventDispatcher.Instance.DispatchEvent(new UIEvent_SailingFlyAnim(nIndex,
                            tbSailing.SuccessGetExp));
                        //成功
                        var ee = new ShowUIHintBoard(230217);
                        EventDispatcher.Instance.DispatchEvent(ee);
                    }
                    else
                    {
                        //失败
                        var ee = new ShowUIHintBoard(230218);
                        EventDispatcher.Instance.DispatchEvent(ee);
                        EventDispatcher.Instance.DispatchEvent(new UIEvent_SailingFlyAnim(nIndex, tbSailing.FailedGetExp));
                    }
                    if (nIndex == 0)
                    {
                        BraveHarborBuild.Exdata[nIndex] = 10;
                    }
                    else
                    {
                        BraveHarborBuild.Exdata[nIndex] = BraveHarborBuild.Exdata[nIndex]/10*10;
                    }
                    RefeshBuildData(BraveHarborBuild);
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
        DataState = 0;
    }

    //计算总船坞属性加成
    public void CalculateItemProp()
    {
        var PropDict = new Dictionary<int, int>();
        var myValue = 0;
        {
            // foreach(var item in SailingData.ShipEquip.EquipItem)
            var __enumerator3 = (SailingData.ShipEquip.EquipItem).GetEnumerator();
            while (__enumerator3.MoveNext())
            {
                var item = __enumerator3.Current;
                {
                    if (item.BaseItemId != -1)
                    {
                        var tbMedal = Table.GetMedal(item.BaseItemId);
                        var tbMedalAddPropIDLength12 = tbMedal.AddPropID.Length;
                        for (var i = 0; i < tbMedalAddPropIDLength12; i++)
                        {
                            if (tbMedal.PropValue[i] != -1)
                            {
                                var tbProp = Table.GetSkillUpgrading(tbMedal.PropValue[i]);
                                myValue = tbProp.GetSkillUpgradingValue(item.Exdata.Level);
                                if (PropDict.ContainsKey(tbMedal.AddPropID[i]))
                                {
                                    PropDict[tbMedal.AddPropID[i]] += myValue;
                                }
                                else
                                {
                                    PropDict.Add(tbMedal.AddPropID[i], myValue);
                                }
                            }
                        }
                    }
                }
            }
        }

        //int attrCount =  SailingData.TempData.AttributesAll.Count;
        //for (int i = 0; i < attrCount; i++)
        //{
        //    SailingData.TempData.AttributesAll[i].Type = -1;
        //}
        SailingData.TempData.AttributesAll.Clear();
        var attrList = new List<AttributeChangeDataModel>();
        if (PropDict.Count != 0)
        {
            var i = 0;
            {
                // foreach(var kvp in PropDict)
                var __enumerator4 = (PropDict).GetEnumerator();
                while (__enumerator4.MoveNext())
                {
                    var kvp = __enumerator4.Current;
                    {
                        var item = new AttributeChangeDataModel();
                        item.Type = kvp.Key;
                        item.Value = kvp.Value;
                        attrList.Add(item);
                    }
                }
            }
        }
        SailingData.TempData.AttributesAll = new ObservableCollection<AttributeChangeDataModel>(attrList);
    }

    private void CityWeakNoticeRefresh(IEvent ievent)
    {
        if (BraveHarborBuild == null)
        {
            return;
        }
        PlayerDataManager.Instance.WeakNoticeData.Sailing = CanSailing;
        EventDispatcher.Instance.DispatchEvent(new CityBulidingWeakNoticeRefresh(BraveHarborBuild));
    }

    //关闭tick
    public void CloseTick()
    {
        if (IsScan)
        {
            IsScan = false;
            SetScanName(IsScan);
        }
        if (mAutoShipCoroutine != null)
        {
            NetManager.Instance.StopCoroutine(mAutoShipCoroutine);
            mAutoShipCoroutine = null;
        }
        // BraveHarborBuild = null;
    }

    public void CloseWoodTips()
    {
        SailingData.IsShowWoodTip = false;
    }

    //吞噬所有掉落船饰
    public void EatAll()
    {
        var tempBagList = new Int32Array();
        var bagList = new Int32Array();

        int[] args = {0, 0};
        var BestIndex = FindBestMedal(args);
        var TempItemCount = args[0];
        var QualityCount = args[1]; ///品质>=紫色的个数
        if (TempItemCount < 2)
        {
            return;
        }
        AddTempBagSelectList(BestIndex);
        if (TempBagLevelUplist.Count > 0)
        {
            var tempCount = TempBagLevelUplist.Count;
            for (var i = 0; i < tempCount; i++)
            {
                tempBagList.Items.Add(TempBagLevelUplist[i]);
            }
            if (QualityCount > 1)
            {
                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 230216, "",
                    () =>
                    {
                        NetManager.Instance.StartCoroutine(LevelUpCoroutine((int) eBagType.MedalTemp, BestIndex,
                            tempBagList, bagList));
                    });
            }
            else
            {
                NetManager.Instance.StartCoroutine(LevelUpCoroutine((int) eBagType.MedalTemp, BestIndex, tempBagList,
                    bagList));
            }
        }
    }

    //寻找最好品质的临时背包物品
    public int FindBestMedal(int[] TempItemCount_QualityCount) //ref int TempItemCount,ref int QualityCount
    {
        var tempQuality = -1;
        var index = -1;
        if (SailingData.ShipEquip.DropItem.Count != 0)
        {
            {
                // foreach(var item in SailingData.ShipEquip.DropItem)
                var __enumerator1 = (SailingData.ShipEquip.DropItem).GetEnumerator();
                while (__enumerator1.MoveNext())
                {
                    var item = __enumerator1.Current;
                    {
                        if (item.BaseItemId != -1)
                        {
                            TempItemCount_QualityCount[0]++;
                            var tbMedal = Table.GetMedal(item.BaseItemId);
                            if (tbMedal.Quality > tempQuality)
                            {
                                tempQuality = tbMedal.Quality;
                                index = item.Index;
                            }
                            if (tbMedal.Quality > 2)
                            {
                                TempItemCount_QualityCount[1]++;
                            }
                        }
                    }
                }
            }
        }

        return index;
    }

    //计算购买到港等的消耗
    public int GetBuyMoney(int index, int disParam)
    {
        var distance = GetShipDistance(index);
        if (distance == 0)
        {
            return 0;
        }
        return distance/disParam + 1;
    }

    //更新船只位置
    public int GetShipDistance(int index)
    {
        var overTime = BraveHarborBuild.Exdata64[index];
        var over = Extension.FromServerBinary(overTime);
        var tbSailing = Table.GetSailing(index);
        double rnd;
        if (Game.Instance.ServerTime >= over)
        {
            rnd = 0;
        }
        else
        {
            var seconds = (over - Game.Instance.ServerTime).TotalSeconds;
            var tbBuild = Table.GetBuilding(BraveHarborBuild.TypeId);
            var tbBS = Table.GetBuildingService(tbBuild.ServiceId);
            var speed = tbBS.Param[0];
            var NeedSeconds = (int) (tbSailing.Distance/(double) speed*60);
            rnd = seconds/NeedSeconds; //MyRandom.Random();
        }
        return (int) (100*rnd*tbSailing.Distance);
    }

    public double GetShipPercent(int index)
    {
        var overTime = BraveHarborBuild.Exdata64[index];
        var over = Extension.FromServerBinary(overTime);
        double rnd;
        if (Game.Instance.ServerTime >= over)
        {
            rnd = 1;
        }
        else
        {
            var seconds = (over - Game.Instance.ServerTime).TotalSeconds;
            var tbBuild = Table.GetBuilding(BraveHarborBuild.TypeId);
            var tbBS = Table.GetBuildingService(tbBuild.ServiceId);
            var tbSailing = Table.GetSailing(index);
            var speed = tbBS.Param[0] + tbBS.Param[1]*BraveHarborBuild.PetList.Count;
            var NeedSeconds = (int) (tbSailing.Distance/(double) speed*60);
            rnd = 1 - seconds/NeedSeconds;
        }
        return rnd;
    }

    //计算经验总和
    public int GetTolalExp(int index, int level)
    {
        var exp = 0;
        var tbProp = Table.GetSkillUpgrading(index);
        if (tbProp == null)
        {
            return 0;
        }
        for (var i = 1; i < level; ++i)
        {
            exp += tbProp.GetSkillUpgradingValue(i);
        }
        return exp;
    }

    //-----------------------------------主界面显示功能-----------------------

    //初始化界面数据
    public void InitData()
    {
        if (SelectedItem != null)
        {
            SelectedItem.Selected = 0;
        }
        var tbBuilding = Table.GetBuilding(BraveHarborBuild.TypeId);
        var tbBuildingServer = Table.GetBuildingService(tbBuilding.ServiceId);

        var SailingDataShipEquipEquipItemCount22 = SailingData.ShipEquip.EquipItem.Count;
	    int lvel = PlayerDataManager.Instance.GetLevel();
		for (var i = 0; i < SailingDataShipEquipEquipItemCount22; i++)
		{
            SailingData.ShipEquip.EquipItem[i].PlayEffect = false;
			var limit = Table.GetClientConfig(1106 + i).ToInt();
			if (lvel>=limit)
			{
				SailingData.ShipEquip.EquipItem[i].IsLock = 0;
				continue;
			}
			SailingData.ShipEquip.EquipItem[i].IsLock = 1;
		}
		/*
        for (var i = 0; i < SailingDataShipEquipEquipItemCount22; i++)
        {
            if (i < tbBuildingServer.Param[2])
            {
                SailingData.ShipEquip.EquipItem[i].IsLock = 0;
                continue;
            }
            SailingData.ShipEquip.EquipItem[i].IsLock = 1;
        }
		 * */
        {
            // foreach(var item in SailingData.ShipEquip.BagItem)
            var __enumerator21 = (SailingData.ShipEquip.BagItem).GetEnumerator();
            while (__enumerator21.MoveNext())
            {
                var item = __enumerator21.Current;
                {
                    item.IsShowCheck = 0;
                }
            }
        }
        {
            // foreach(var item in SailingData.ShipEquip.DropItem)
            var __enumerator22 = (SailingData.ShipEquip.DropItem).GetEnumerator();
            while (__enumerator22.MoveNext())
            {
                var item = __enumerator22.Current;
                {
                    item.IsShowCheck = 0;
                }
            }
        }

        var SailingDataUIColorSelectCount23 = SailingData.UI.ColorSelect.Count;
        for (var i = 0; i < SailingDataUIColorSelectCount23; i++)
        {
            SailingData.UI.ColorSelect[i] = false;
        }

        SetScanTips();
    }

    //初始化背包
    public void InitMedalBag(BagBaseData bagBase)
    {
        var bagID = bagBase.BagId;
        switch (bagID)
        {
            case 6:
            {
                var list = new List<MedalItemDataModel>();
                SailingData.ShipEquip.BagItem.Clear();
                var tbBaseBase = Table.GetBagBase(bagID);
                var tbBaseBaseMaxCapacity20 = tbBaseBase.MaxCapacity;
                for (var i = 0; i < tbBaseBaseMaxCapacity20; i++)
                {
                    var Medalitem = new MedalItemDataModel();
                    Medalitem.Index = i;
                    list.Add(Medalitem);
                    //SailingData.ShipEquip.BagItem.Add(Medalitem);
                }
                {
                    var __list10 = bagBase.Items;
                    var __listCount10 = __list10.Count;
                    for (var __i10 = 0; __i10 < __listCount10; ++__i10)
                    {
                        var item = __list10[__i10];
                        {
                            var Medalitem = list[item.Index];
                            //MedalItemDataModel Medalitem = SailingData.ShipEquip.BagItem[item.Index];
                            Medalitem.BagId = (int) eBagType.MedalBag;
                            Medalitem.BaseItemId = item.ItemId;
                            Medalitem.Exdata.InstallData(item.Exdata);
                        }
                    }
                }
                SailingData.ShipEquip.BagItem = new ObservableCollection<MedalItemDataModel>(list);
            }
                break;
            case 19:
            {
                var __list11 = bagBase.Items;
                var __listCount11 = __list11.Count;
                for (var __i11 = 0; __i11 < __listCount11; ++__i11)
                {
                    var item = __list11[__i11];
                    {
                        var Medalitem = SailingData.ShipEquip.EquipItem[item.Index];
                        Medalitem.BaseItemId = item.ItemId;
                        Medalitem.BagId = (int) eBagType.MedalUsed;
                        Medalitem.Exdata.InstallData(item.Exdata);
                    }
                }
            }
                break;
            case 20:
            {
                var tbBaseBase = Table.GetBagBase(bagID);
                var tbBaseBaseMaxCapacity21 = tbBaseBase.MaxCapacity;
                var list = new List<MedalItemDataModel>();
                for (var i = 0; i < tbBaseBaseMaxCapacity21; i++)
                {
                    var Medalitem = new MedalItemDataModel();
                    Medalitem.Index = i;
                    list.Add(Medalitem);
                    //SailingData.ShipEquip.DropItem.Add(Medalitem);
                }
                {
                    var __list12 = bagBase.Items;
                    var __listCount12 = __list12.Count;
                    for (var __i12 = 0; __i12 < __listCount12; ++__i12)
                    {
                        var item = __list12[__i12];
                        {
                            var Medalitem = list[item.Index];
                            //MedalItemDataModel Medalitem = SailingData.ShipEquip.DropItem[item.Index];
                            Medalitem.BagId = (int) eBagType.MedalTemp;
                            Medalitem.BaseItemId = item.ItemId;
                            Medalitem.Exdata.InstallData(item.Exdata);
                        }
                    }
                }
                SailingData.ShipEquip.DropItem = new ObservableCollection<MedalItemDataModel>(list);
            }
                break;
            //default:
            //    {
            //       break;
            //    }
        }
    }

    //升级返回按钮
    public void LevelBackClick()
    {
        SailingData.UI.IsShowWitchUI = 1;

        var SailingDataShipEquipBagItemCount6 = SailingData.ShipEquip.BagItem.Count;
        for (var i = 0; i < SailingDataShipEquipBagItemCount6; i++)
        {
            SailingData.ShipEquip.BagItem[i].IsShowCheck = 0;
        }
        var SailingDataShipEquipDropItemCount7 = SailingData.ShipEquip.DropItem.Count;
        for (var i = 0; i < SailingDataShipEquipDropItemCount7; i++)
        {
            SailingData.ShipEquip.DropItem[i].IsShowCheck = 0;
        }

        var enumerator = (SailingData.ShipEquip.EquipItem).GetEnumerator();
        while (enumerator.MoveNext())
        {
            var item = enumerator.Current;
            item.PlayEffect = false;
        }
    }

    //升级界面显示
    public void LevelUIShow()
    {
        SailingData.UI.IsShowWitchUI = 2;
        var SailingDataUIColorSelectCount5 = SailingData.UI.ColorSelect.Count;
        for (var i = 0; i < SailingDataUIColorSelectCount5; i++)
        {
            if (SailingData.UI.ColorSelect[i])
            {
                SelectColor(i);
            }
        }
        RefleshUI();
    }

    //升级确定按钮
    public void LevelUpClick()
    {
        var tempBagList = new Int32Array();
        var bagList = new Int32Array();
        //强化材料为0返回
        if (BaglevelUpList.Count == 0 && TempBagLevelUplist.Count == 0)
        {
            return;
        }

        var listCount = BaglevelUpList.Count;
        for (var i = 0; i < listCount; i++)
        {
            bagList.Items.Add(BaglevelUpList[i]);
        }

        var tempListCount = TempBagLevelUplist.Count;
        for (var i = 0; i < tempListCount; i++)
        {
            tempBagList.Items.Add(TempBagLevelUplist[i]);
        }

        NetManager.Instance.StartCoroutine(LevelUpCoroutine(SailingData.TempData.TempMedalItem.BagId,
            SailingData.TempData.TempMedalItem.Index, tempBagList, bagList));
    }

    public IEnumerator LevelUpCoroutine(int bagID,
                                        int index,
                                        Int32Array TempBagList,
                                        Int32Array BagList)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.EnchanceMedal(bagID, index, TempBagList, BagList);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    if (SailingData.UI.IsShowWitchUI == 2)
                    {
                        {
                            // foreach(var item in SailingData.ShipEquip.DropItem)
                            var __enumerator2 = (SailingData.ShipEquip.DropItem).GetEnumerator();
                            while (__enumerator2.MoveNext())
                            {
                                var item = __enumerator2.Current;
                                {
                                    item.IsShowCheck = 0;
                                }
                            }
                        }
                        EventDispatcher.Instance.DispatchEvent(new UIEvent_SailingPlayAnimation());
                    }
                    else
                    {
                        var itemsList = new List<int>();
                        for (var i = 0; i < TempBagList.Items.Count; i++)
                        {
                            var itemid = TempBagList.Items[i];
                            itemsList.Add(SailingData.ShipEquip.DropItem[itemid].BaseItemId);
                        }
                        var ee = new UIEvent_SailingPlayEatAnim();
                        ee.Index = index;
                        ee.List = TempBagLevelUplist;
                        ee.ItemIds = itemsList;
                        EventDispatcher.Instance.DispatchEvent(ee);
                    }
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
    public void LightPointAccessClick(IEvent ievent)
    {
        var e = ievent as UIEvent_SailingLightPointAccess;
        var index = e.Index;

        var count = 0;
        var dropitem = SailingData.ShipEquip.DropItem;
        for (var i = 0; i < dropitem.Count; i++)
        {
            if (dropitem[i].BaseItemId == -1)
            {
                count++;
                break;
            }
        }
        if (count == 0)
        {
            var ee = new ShowUIHintBoard(302);
            EventDispatcher.Instance.DispatchEvent(ee);
            return;
        }

        if (SailingData.States[index] != 0)
        {
            var ee = new ShowUIHintBoard(220817);
            EventDispatcher.Instance.DispatchEvent(ee);
            return;
        }

        if (SailingData.States[index] == 12)
        {
            var ee = new ShowUIHintBoard(220827);
            EventDispatcher.Instance.DispatchEvent(ee);
            return;
        }

        SailingData.Ship[index].SpeedOrGet = 1;

        NetManager.Instance.StartCoroutine(LightPointClickAccessCoroutine(index));
    }
    //开始出航
    public void LightPointClick(IEvent ievent)
    {
        var e = ievent as UIEvent_SailingLightPoint;
        var index = e.Index;

        var count = 0;
        var dropitem = SailingData.ShipEquip.DropItem;
        for (var i = 0; i < dropitem.Count; i++)
        {
            if (dropitem[i].BaseItemId == -1)
            {
                count++;
                break;
            }
        }
        if (count == 0)
        {
            var ee = new ShowUIHintBoard(302);
            EventDispatcher.Instance.DispatchEvent(ee);
            return;
        }

        if (SailingData.States[index] == 0)
        {
            var ee = new ShowUIHintBoard(220817);
            EventDispatcher.Instance.DispatchEvent(ee);
            return;
        }

        if (SailingData.States[index] == 12)
        {
            var ee = new ShowUIHintBoard(220827);
            EventDispatcher.Instance.DispatchEvent(ee);
            return;
        }

        SailingData.Ship[index].SpeedOrGet = 1;

        NetManager.Instance.StartCoroutine(LightPointClickCoroutine(index));
    }

    // 出海
    public IEnumerator LightPointClickCoroutine(int nIndex)
    {
        using (new BlockingLayerHelper(0))
        {
            var tbBuild = Table.GetBuilding(BraveHarborBuild.TypeId);
            var array = new Int32Array();
            array.Items.Add(0);
            array.Items.Add(nIndex);
            var msg = NetManager.Instance.UseBuildService(BraveHarborBuild.AreaId, tbBuild.ServiceId, array);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    SailingData.Ship[nIndex].IsStart = true;
                    SailingData.Ship[nIndex].IsStart = false;
                    //EventDispatcher.Instance.DispatchEvent(new UIEvent_SailingAnim(nIndex,0));
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

    //直达
    public IEnumerator LightPointClickAccessCoroutine(int nIndex)
    {
        using (new BlockingLayerHelper(0))
        {
            var tbBuild = Table.GetBuilding(BraveHarborBuild.TypeId);
            var array = new Int32Array();
            array.Items.Add(4);
            array.Items.Add(nIndex);
            var msg = NetManager.Instance.UseBuildService(BraveHarborBuild.AreaId, tbBuild.ServiceId, array);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int)ErrorCodes.OK)
                {
                    SailingData.Ship[nIndex].IsStart = true;
                    SailingData.Ship[nIndex].IsStart = false;
                    //EventDispatcher.Instance.DispatchEvent(new UIEvent_SailingAnim(nIndex,0));
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

    //领取结果
    public void LineButtonClick(IEvent ievent)
    {
        if (BraveHarborBuild == null)
        {
            return;
        }
        //判断包裹是否满
        var count = 0;
        var dropitem = SailingData.ShipEquip.DropItem;
        for (var i = 0; i < dropitem.Count; i++)
        {
            if (dropitem[i].BaseItemId == -1)
            {
                count++;
                break;
            }
        }
        if (count == 0)
        {
            var ee = new ShowUIHintBoard(302);
            EventDispatcher.Instance.DispatchEvent(ee);
            return;
        }

        var e = ievent as UIEvent_SailingLineButton;
        var index = e.Index;
        if (BraveHarborBuild.Exdata[index]%10 == 3)
        {
            NetManager.Instance.StartCoroutine(LineButtonClickCoroutine(index));
        }
        else if (BraveHarborBuild.Exdata[index]%10 == 2)
        {
            var sailtb = Table.GetSailing(index);
            var needmoney = GetBuyMoney(index, sailtb.distanceParam);
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel,
                string.Format(GameUtils.GetDictionaryText(1501), needmoney), "",
                () =>
                {
                    //木材不足
                    var resCount = PlayerDataManager.Instance.GetRes(sailtb.ConsumeType);
                    if (needmoney < 0 || resCount < needmoney)
                    {
                        var ee = new ShowUIHintBoard(270226);
                        EventDispatcher.Instance.DispatchEvent(ee);
                        PlayerDataManager.Instance.ShowItemInfoGet(sailtb.ConsumeType);
                        return;
                    }
                    NetManager.Instance.StartCoroutine(BuyShipCoroutine(index, needmoney));
                }
                );
            //   UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, string.Format("购买：{0}", Table.GetFuben(fubenId).Name), "",
            //() =>
            //{
            //    DungeonBtnClick e2 = new DungeonBtnClick(4, eDungeonType.Team, fubenId);
            //    EventDispatcher.Instance.DispatchEvent(e);
            //},
            //            () =>
            //            {
            //                DungeonBtnClick e2 = new DungeonBtnClick(5, eDungeonType.Team, fubenId);
            //                EventDispatcher.Instance.DispatchEvent(e);
            //            });
        }
    }

    public IEnumerator LineButtonClickCoroutine(int nIndex)
    {
        using (new BlockingLayerHelper(0))
        {
            var tbBuild = Table.GetBuilding(BraveHarborBuild.TypeId);
            var array = new Int32Array();
            array.Items.Add(1);
            array.Items.Add(nIndex);
            var msg = NetManager.Instance.UseBuildService(BraveHarborBuild.AreaId, tbBuild.ServiceId, array);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var result = msg.Response.Data32[0];
                    SailingData.Ship[nIndex].IsGet = true;
                    SailingData.Ship[nIndex].IsGet = false;

                    var tbSailng = Table.GetSailing(nIndex);
                    if (result > 0)
                    {
                        //成功
                        var ee = new ShowUIHintBoard(230217);
                        EventDispatcher.Instance.DispatchEvent(ee);
                        EventDispatcher.Instance.DispatchEvent(new UIEvent_SailingFlyAnim(nIndex, tbSailng.SuccessGetExp));
                    }
                    else
                    {
                        //失败
                        var ee = new ShowUIHintBoard(230218);
                        EventDispatcher.Instance.DispatchEvent(ee);
                        EventDispatcher.Instance.DispatchEvent(new UIEvent_SailingFlyAnim(nIndex, tbSailng.SuccessGetExp));
                    }
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

    //整理背包
    public void OnArrange()
    {
        if (SailingData.ShipEquip.BagItem.Count > 1)
        {
            NetManager.Instance.StartCoroutine(OnArrangeCoroutine((int) eBagType.MedalBag));
        }
    }

    public IEnumerator OnArrangeCoroutine(int nBagId)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.SortBag(nBagId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var bag = msg.Response;
                    InitMedalBag(bag);
                    if (SailingData.UI.IsShowWitchUI == 2)
                    {
                        if (mSelectType == (int) SelectType.EquipSelect)
                        {
                            LevelUIShow();
                        }
                        else
                        {
                            SailingData.UI.IsShowWitchUI = 1;
                        }
                    }
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

    public void OnCityDataInit(IEvent ievent)
    {
        BraveHarborBuild = null;
        {
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
                    if (tbBuild.Type == 12)
                    {
                        BraveHarborBuild = buildingData;
                        break;
                    }
                }
            }
        }
        if (BraveHarborBuild != null)
        {
            AnalyseNotice();
        }
    }

    //点击背包物品事件
    public void OnClickPackItem(IEvent ievent)
    {
        var e = ievent as UIEvent_SailingPackItemUI;
        if (e.Index >= 0)
        {
            if (SailingData.UI.IsShowWitchUI != 2)
            {
                if (SelectedItem != null)
                {
                    SelectedItem.Selected = 0;
                }
                if (e.BagId == (int) eBagType.MedalBag)
                {
                    mSelectType = (int) SelectType.BagSelect;
                    SetTempMedalItem(SailingData.ShipEquip.BagItem[e.Index]);
                    SailingData.ShipEquip.BagItem[e.Index].Selected = 1;
                    SelectedItem = SailingData.ShipEquip.BagItem[e.Index];
                }
                else if (e.BagId == (int) eBagType.MedalUsed)
                {
                    mSelectType = (int) SelectType.EquipSelect;
                    SetTempMedalItem(SailingData.ShipEquip.EquipItem[e.Index]);
                    SailingData.ShipEquip.EquipItem[e.Index].Selected = 1;
                    SelectedItem = SailingData.ShipEquip.EquipItem[e.Index];
                }
                else
                {
                    SailingData.TempData.TempMedalItem = null;
                    return;
                }
                var tempdata = new MedalInfoDataModel();
                tempdata.ItemData = SailingData.TempData.TempMedalItem;
                if (tempdata.ItemData.BaseItemId == -1)
                {
                    return;
                }
                tempdata.ItemData.BagId = e.BagId;
                tempdata.ItemData.Index = e.Index;
                tempdata.PutOnOrOff = e.PutOnOrOff;
                var tbMedal = Table.GetMedal(tempdata.ItemData.BaseItemId);
                if (tbMedal.CanEquipment == 1)
                {
                    tempdata.IsShowButton = 1;
                }
                else
                {
                    tempdata.IsShowButton = 0;
                }
                EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.MedalInfoUI,
                    new MedalInfoArguments {MedalInfoData = tempdata}));
                mSelectIndex = e.Index;
            }
            else
            {
                var tempdata = SailingData.TempData.TempMedalItem;
                if (tempdata.Index == e.Index && tempdata.BagId == (int) eBagType.MedalBag)
                {
                    return;
                }
                var MedalTable2 = Table.GetMedal(tempdata.BaseItemId);
                if (tempdata.Exdata.Level >= MedalTable2.MaxLevel)
                {
                    var ee = new ShowUIHintBoard(220809);
                    EventDispatcher.Instance.DispatchEvent(ee);
                    return;
                }

                var judge = false;
                var varItem = SailingData.ShipEquip.BagItem[e.Index];
                if (varItem.BaseItemId == -1)
                {
                    return;
                }

                var MedalTable = Table.GetMedal(varItem.BaseItemId);
                if (MedalTable.MedalType == 13)
                {
                    SetShowCheck((int) eBagType.MedalBag, e.Index);
                    return;
                }

                if (MedalTable.Quality > 2 && varItem.IsShowCheck == 0)
                {
                    //你选择了高阶零件消耗升级，是否确认
                    UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, GameUtils.GetDictionaryText(220810), "",
                        () => { SetShowCheck((int) eBagType.MedalBag, e.Index); }
                        );
                }
                else
                {
                    SetShowCheck((int) eBagType.MedalBag, e.Index);
                }
            }
        }
    }

    //------------------------------------背包界面----------------------------------------------
    //返回按钮//船坞---海港切换
    public void OnClickReturnBtn(IEvent ievent)
    {
        var e = ievent as UIEvent_SailingReturnBtn;
        if (IsScan)
        {
            IsScan = false;
            SetScanName(IsScan);
        }

        if (e.Showtype == 1)
        {
            SailingData.UI.IsShowWitchUI = 0;
        }
        else
        {
            SailingData.UI.IsShowWitchUI = 1;
        }

        if (SailingData.UI.IsShowWitchUI != 2)
        {
            var SailingDataShipEquipBagItemCount15 = SailingData.ShipEquip.BagItem.Count;
            for (var i = 0; i < SailingDataShipEquipBagItemCount15; i++)
            {
                SailingData.ShipEquip.BagItem[i].IsShowCheck = 0;
            }
            var SailingDataShipEquipDropItemCount16 = SailingData.ShipEquip.DropItem.Count;
            for (var i = 0; i < SailingDataShipEquipDropItemCount16; i++)
            {
                SailingData.ShipEquip.DropItem[i].IsShowCheck = 0;
            }
        }
        if (SailingData.UI.IsShowWitchUI == 1)
        {
            CalculateItemProp();
            var enumerator = (SailingData.ShipEquip.EquipItem).GetEnumerator();
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                item.PlayEffect = false;
            }
        }
    }

    private void OnExdataUpdate(IEvent ievent)
    {
        SetScanTips();
    }

    //升级界面checkbox响应
    public void OnToggleChange(object sender, PropertyChangedEventArgs e)
    {
        var index = 0;
        if (!int.TryParse(e.PropertyName, out index))
        {
            return;
        }
        SelectColor(index);
        RefleshUI();
    }

    //-----------------------------------出航界面-------------------------------
    //捡起所有的掉落船饰
    public void PickAll(IEvent ievent)
    {
        var e = ievent as UIEvent_SailingPickAll;
        var count = 0;
        {
            // foreach(var item in SailingData.ShipEquip.DropItem)
            var __enumerator7 = (SailingData.ShipEquip.DropItem).GetEnumerator();
            while (__enumerator7.MoveNext())
            {
                var item = __enumerator7.Current;
                {
                    if (item.BaseItemId != -1)
                    {
                        count++;
                        break;
                    }
                }
            }
        }
        //临时掉落背包装备个数是否为0
        if (count != 0)
        {
            NetManager.Instance.StartCoroutine(PickUpMemedalCoroutine(-1));
        }
    }

    //捡起单个掉落船饰
    public void PickOne(IEvent ievent)
    {
        var e = ievent as UIEvent_SailingPickOne;
        if (SailingData.UI.IsShowWitchUI == 2)
        {
            var varItem = SailingData.ShipEquip.DropItem[e.Index];
            if (varItem.BaseItemId == -1)
            {
                return;
            }
            var ItemBaseTable = Table.GetItemBase(varItem.BaseItemId);
            var MedalTable = Table.GetMedal(ItemBaseTable.Exdata[0]);

            if (MedalTable.MedalType == 13)
            {
                SetShowCheck((int) eBagType.MedalTemp, e.Index);
                return;
            }
            if (MedalTable.Quality > 2 && varItem.IsShowCheck == 0)
            {
                //你选择了高阶零件消耗升级，是否确认
                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, GameUtils.GetDictionaryText(220810), "",
                    () => { SetShowCheck((int) eBagType.MedalTemp, e.Index); }
                    );
            }
            else
            {
                SetShowCheck((int) eBagType.MedalTemp, e.Index);
            }
        }
        else
        {
            var count = 0;
            var tempData = SailingData.ShipEquip.DropItem[e.Index];
            if (tempData.BaseItemId == -1)
            {
                return;
            }
            var ItemBaseTable = Table.GetItemBase(tempData.BaseItemId);
            if (ItemBaseTable == null)
            {
                return;
            }
            var MedalTable = Table.GetMedal(ItemBaseTable.Exdata[0]);
            if (MedalTable == null)
            {
                return;
            }
            //废弃零件无法收进背包
            if (MedalTable.Quality == 0)
            {
                var ee = new ShowUIHintBoard(220811);
                EventDispatcher.Instance.DispatchEvent(ee);
                return;
            }

            var SailingDataShipEquipBagItemCount18 = SailingData.ShipEquip.BagItem.Count;
            for (var i = 0; i < SailingDataShipEquipBagItemCount18; i++)
            {
                if (SailingData.ShipEquip.BagItem[i].BaseItemId == -1)
                {
                    count++;
                    break;
                }
            }
            //背包已满
            if (count == 0)
            {
                var ee = new ShowUIHintBoard(220812);
                EventDispatcher.Instance.DispatchEvent(ee);
            }
            NetManager.Instance.StartCoroutine(PickUpMemedalCoroutine(e.Index));
        }
    }

    public IEnumerator PickUpMemedalCoroutine(int index)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.PickUpMedal(index);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
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

    //穿上船饰
    public void PutOnClick(IEvent ievent)
    {
        var count = 0;
        var e = ievent as UIEvent_SailingPutOnClick;
        SailingData.UI.IsShowWitchUI = 1;

        if (e.PutOnOrOff == 1)
        {
            var IsSameType = false;
            var varItem = SailingData.TempData.TempMedalItem;
            var MedalTable = Table.GetMedal(varItem.BaseItemId);
            var medalType = MedalTable.MedalType;
            {
                // foreach(var item in SailingData.ShipEquip.EquipItem)
                var __enumerator5 = (SailingData.ShipEquip.EquipItem).GetEnumerator();
                while (__enumerator5.MoveNext())
                {
                    var item = __enumerator5.Current;
                    {
                        if (item.BaseItemId == -1)
                        {
                            if (item.IsLock != 1)
                            {
                                count++;
                            }
                            continue;
                        }
                        var MedalTable2 = Table.GetMedal(item.BaseItemId);
                        if (MedalTable2.MedalType == medalType)
                        {
                            IsSameType = true;
                        }
                    }
                }
            }
            ////你的装备零件栏已满
            if (!IsSameType && count == 0)
            {
                var ee = new ShowUIHintBoard(220802);
                EventDispatcher.Instance.DispatchEvent(ee);
                return;
            }
            //if (count == 0)
            //{
            //    var ee = new ShowUIHintBoard(220802);
            //    EventDispatcher.Instance.DispatchEvent(ee);
            //    return;
            //}
            //同类型零件最多只能装备一个
            //if (IsSameType)
            //{
            //    var ee = new ShowUIHintBoard(220803);
            //    EventDispatcher.Instance.DispatchEvent(ee);
            //    return;
            //}

            NetManager.Instance.StartCoroutine(PutOnMemedalCoroutine((int) eBagType.MedalBag,
                SailingData.TempData.TempMedalItem.Index));
        }
        else
        {
            var SailingDataShipEquipBagItemCount14 = SailingData.ShipEquip.BagItem.Count;
            for (var i = 0; i < SailingDataShipEquipBagItemCount14; i++)
            {
                if (SailingData.ShipEquip.BagItem[i].BaseItemId == -1)
                {
                    count++;
                    break;
                }
            }
            //背包已满，请先吞噬掉一些零件
            if (count == 0)
            {
                var ee = new ShowUIHintBoard(220804);
                EventDispatcher.Instance.DispatchEvent(ee);
            }

            NetManager.Instance.StartCoroutine(PutOnMemedalCoroutine((int) eBagType.MedalUsed,
                SailingData.TempData.TempMedalItem.Index));
        }
    }

    public IEnumerator PutOnMemedalCoroutine(int bagID, int index)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.EquipMedal(bagID, index);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    if (msg.Response >= 0 && msg.Response <= SailingData.ShipEquip.EquipItem.Count)
                    {
                        var item = SailingData.ShipEquip.EquipItem[msg.Response];
                        item.PlayEffect = false;
                        if (bagID == (int)eBagType.MedalBag)
                        {
                            item.PlayEffect = true;
                        }
                    }
                        
                    if (SelectedItem != null)
                    {
                        SelectedItem.Selected = 0;
                    }
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

    // 刷新勇士港building
    public void RefeshBuildData(BuildingData buildingData)
    {
        var tbBuild = Table.GetBuilding(buildingData.TypeId);
        if (tbBuild == null)
        {
            return;
        }
        if (tbBuild.Type != (int) BuildingType.BraveHarbor)
        {
            return;
        }
        var index = 0;
        {
            var __list9 = buildingData.Exdata;
            var __listCount9 = __list9.Count;
            for (var __i9 = 0; __i9 < __listCount9; ++__i9)
            {
                var i = __list9[__i9];
                {
                    if (index > 4)
                    {
                        break;
                    }
                    SailingData.States[index] = i;
                    SailingData.Ship[index].DistancePerCent = 0f;
                    //SailingData.AddSpeed[index] = "";
                    if (i == 2 || i == 12 || i == 3)
                    {
                        // SailingData.AddSpeed[index] = GameUtils.GetDictionaryText(220819);
                        if (i == 3)
                        {
                            SailingData.Ship[index].SpeedOrGet = 2;
                            SailingData.Ship[index].IsShowShip = false;
                        }
                        else
                        {
                            SailingData.Ship[index].SpeedOrGet = 1;
                            SailingData.Ship[index].IsShowShip = true;
                        }
                        UpdataShip(index);
                    }
                    else
                    {
                        //if (i == 0)
                        //{
                        SailingData.Ship[index].SpeedOrGet = 0;
                        // }
                        SailingData.Ship[index].IsShowShip = false;
                        SailingData.Ship[index].Times = " ";
                        if (index == 4)
                        {
                            SailingData.States[5] = 0;
                        }
                    }
                    index++;
                }
            }
        }
        AnalyseNotice();
        RefleshAccessBtn();
    }
    public void RefleshAccessBtn()
    {
        for (int i = 0; i < SailingData.States.Count && i < SailingData.lCanAccess.Count; i++)
       {
           var tb = Table.GetSailing(i);
           if(tb != null)
           {
               SailingData.lCanAccess[i] = SailingData.States[i] == 0 && tb.CanCall>0 ; 
           }
       }
    }
    //刷新属性增加显示
    public void RefleshLevelUpAttr()
    {
        var tempData = SailingData.TempData.TempMedalItem;
        if (tempData.BaseItemId != -1)
        {
            var ItemBaseTable = Table.GetItemBase(tempData.BaseItemId);
            var MedalTable = Table.GetMedal(ItemBaseTable.Exdata[0]);

            var MedalTableAddPropIDLength11 = MedalTable.AddPropID.Length;
            for (var i = 0; i < MedalTableAddPropIDLength11; i++)
            {
                if (MedalTable.AddPropID[i] == -1)
                {
                    SailingData.TempData.Attributes[i].Type = -1;
                }
                else
                {
                    var tbProp = Table.GetSkillUpgrading(MedalTable.PropValue[i]);
                    SailingData.TempData.Attributes[i].Type = MedalTable.AddPropID[i];
                    SailingData.TempData.Attributes[i].Value = tbProp.GetSkillUpgradingValue(tempData.Exdata.Level);
                    SailingData.TempData.Attributes[i].Change = tbProp.GetSkillUpgradingValue(SailingData.UI.NextLevel);
                }
            }
        }
    }

    //levelup页面刷新
    public void RefleshUI()
    {
        var tempData = SailingData.TempData.TempMedalItem;
        //刷新经验界面
        AddBagSelectList();
        var UICurrentExp = 0;
        var UICurrentExpMax = 0;
        if (tempData.BaseItemId != -1)
        {
            var MedalTable = Table.GetMedal(tempData.BaseItemId);
            var tbProp = Table.GetSkillUpgrading(MedalTable.LevelUpExp);
            var templevel = tempData.Exdata.Level;
            var MedalTableMaxLevel10 = MedalTable.MaxLevel;
            for (var i = templevel; i <= MedalTableMaxLevel10; i++)
            {
                var levelexp = GetTolalExp(MedalTable.LevelUpExp, templevel);
                if (levelexp < SailingData.TempData.TotalExp)
                {
                    templevel++;
                }
                else
                {
                    break;
                }
            }
            if (templevel > MedalTable.MaxLevel + 1)
            {
                SailingData.UI.NextLevel = MedalTable.MaxLevel;
                UICurrentExpMax = tbProp.GetSkillUpgradingValue(MedalTable.MaxLevel);
                UICurrentExp = UICurrentExpMax;
            }
            else
            {
                var temp = templevel - 1;
                temp = (temp < tempData.Exdata.Level)
                    ? tempData.Exdata.Level
                    : temp;

                //SailingData.UI.NextLevel = templevel - 1;
                SailingData.UI.NextLevel = tempData.Exdata.Level + 1;
                SailingData.UI.NextLevel = (SailingData.UI.NextLevel < tempData.Exdata.Level)
                    ? tempData.Exdata.Level
                    : SailingData.UI.NextLevel;

                UICurrentExp = SailingData.TempData.TotalExp -
                               GetTolalExp(MedalTable.LevelUpExp, templevel - 1);
                UICurrentExpMax = tbProp.GetSkillUpgradingValue(temp);
                UICurrentExp = UICurrentExp > UICurrentExpMax ? UICurrentExpMax : UICurrentExp;
            }


            float tempfloat = 0;
            if (UICurrentExpMax != 0)
            {
                tempfloat = UICurrentExp/(float) UICurrentExpMax;
            }
            SailingData.UI.UICurrentExp = UICurrentExp;
            SailingData.UI.UICurrentExpMax = UICurrentExpMax;
            SailingData.UI.currentExp = tempfloat;
        }
        RefleshLevelUpAttr();
    }

    //计算船只位置
    public void ResetShipPercent(int index, double prob)
    {
        SailingData.Ship[index].DistancePerCent = (float) prob;
        SailingData.Ship[index].Posion = new Vector3(
            (int) (staticPos[index].x + (staticPos[index + 1].x - staticPos[index].x)*prob),
            (int) (staticPos[index].y + (staticPos[index + 1].y - staticPos[index].y)*prob), 0);
    }

    //重置船只位置
    public void ResetShipPosition()
    {
        for (var i = 0; i < SailingData.Ship.Count; i++)
        {
            SailingData.Ship[i].Posion = new Vector3(staticPos[i].x, staticPos[i].y, 0);
            SailingData.Ship[i].DistancePerCent = 0f;
        }
    }

    //勇士港操作事件
    public void SailingOperation(IEvent ievent)
    {
        var e = ievent as UIEvent_SailingOperation;
        switch (e.Type)
        {
            case 0:
            {
                OnArrange();
            }
                break;
            case 1:
            {
                EatAll();
            }
                break;
            case 2:
            {
                LevelUIShow();
            }
                break;
            case 3:
            {
                LevelUpClick();
            }
                break;
            case 4:
            {
                AutoShipClick();
            }
                break;
            case 5:
            {
                LevelBackClick();
            }
                break;
            case 6:
            {
                CloseTick();
            }
                break;
            case 7:
            {
                WoodTipsOk();
            }
                break;
            case 8:
            {
                CloseWoodTips();
            }
                break;
        }
    }

    //---------------------------------升级界面----------------------------------------

    //设置勾选物品
    public void SelectColor(int index)
    {
        var SailingDataShipEquipBagItemCount0 = SailingData.ShipEquip.BagItem.Count;
        for (var i = 0; i < SailingDataShipEquipBagItemCount0; i++)
        {
            var item = SailingData.ShipEquip.BagItem[i];
            if (item.BaseItemId == -1)
            {
                continue;
            }
            if (item.Index == SailingData.TempData.TempMedalItem.Index &&
                SailingData.TempData.TempMedalItem.BagId == (int) eBagType.MedalBag)
            {
                continue;
            }
            var MedelItem = Table.GetMedal(item.BaseItemId);
            if (MedelItem.Quality == index)
            {
                if (SailingData.UI.ColorSelect[index])
                {
                    item.IsShowCheck = 1;
                }
                else
                {
                    item.IsShowCheck = 0;
                }
            }
        }

        //int dropCount =  SailingData.ShipEquip.DropItem.Count;
        //for (int i = 0; i < dropCount; i++)
        //{
        //    var item = SailingData.ShipEquip.DropItem[i];
        //    if (item.BaseItemId == -1)
        //    {
        //        continue;
        //    }
        //    var MedelItem = Table.GetMedal(item.BaseItemId);
        //    if (MedelItem.Quality == index)
        //    {
        //        if (SailingData.UI.ColorSelect[index])
        //        {
        //            item.IsShowCheck = 1;
        //        }
        //        else
        //        {
        //            item.IsShowCheck = 0;
        //        }
        //    }
        //}
    }

    //设置扫荡和取消扫荡标签
    public void SetScanName(bool IsScan)
    {
        if (IsScan)
        {
            SailingData.ScanName = GameUtils.GetDictionaryText(220823);
        }
        else
        {
            SailingData.ScanName = GameUtils.GetDictionaryText(220822);
        }
    }

    private void SetScanTips()
    {
        //var vipLevel= PlayerDataManager.Instance.GetRes((int) eResourcesType.VipLevel);
        var count = PlayerDataManager.Instance.GetExData((int) eExdataDefine.e71);
        var scanCount = PlayerDataManager.Instance.TbVip.SailScanCount;
        if (scanCount <= 0)
        {
            SailingData.ScanTips = ScanTips270250;
        }
        else
        {
            var countShow = scanCount - count;
            countShow = countShow <= 0 ? 0 : countShow;
            SailingData.ScanTips = string.Format(ScanTips270249, countShow);
        }
    }

    //点击设置勾选框   
    public void SetShowCheck(int bagId, int index)
    {
        if (bagId == (int) eBagType.MedalBag)
        {
            SailingData.ShipEquip.BagItem[index].IsShowCheck =
                (SailingData.ShipEquip.BagItem[index].IsShowCheck == 1) ? 0 : 1;
        }
        else if (bagId == (int) eBagType.MedalTemp)
        {
            SailingData.ShipEquip.DropItem[index].IsShowCheck =
                (SailingData.ShipEquip.DropItem[index].IsShowCheck == 1) ? 0 : 1;
        }
        RefleshUI();
    }

    //设置要升级的物品
    public void SetTempMedalItem(MedalItemDataModel item)
    {
        SailingData.TempData.TempMedalItem = new MedalItemDataModel(item);
    }

    //终止扫荡后status重置
    public void StopShipScan(int index)
    {
        DataState = -1;
        IsScan = false;
        BraveHarborBuild.Exdata[index] = 10;
        ResetShipPosition();
        RefeshBuildData(BraveHarborBuild);
        SetScanName(IsScan);
    }

    //更新船只位置
    public void UpdataShip(int index)
    {
        var overTime = BraveHarborBuild.Exdata64[index];
        var over = Extension.FromServerBinary(overTime);
        float rnd;
        if (Game.Instance.ServerTime >= over)
        {
            rnd = 1;
            BraveHarborBuild.Exdata[index] = BraveHarborBuild.Exdata[index]/10*10 + 3;
            SailingData.Ship[index].IsShowShip = false;
            //   SailingData.Ship[index].Times = GameUtils.GetDictionaryText(1500);
            //SailingData.AddSpeed[index] = GameUtils.GetDictionaryText(220820);
            SailingData.Ship[index].SpeedOrGet = 2;
            SailingData.Ship[index].Times = "";
            if (index == 4)
            {
                SailingData.States[5] = 10;
            }
        }
        else
        {
            if (index == 4)
            {
                SailingData.States[5] = 0;
            }
            var seconds = (float) (over - Game.Instance.ServerTime).TotalSeconds;
            var tbBuild = Table.GetBuilding(BraveHarborBuild.TypeId);
            var tbBS = Table.GetBuildingService(tbBuild.ServiceId);
            var tbSailing = Table.GetSailing(index);
            var speed = tbBS.Param[0] + tbBS.Param[1]*BraveHarborBuild.PetList.Count;
            var NeedSeconds = (int) (tbSailing.Distance/(double) speed*60);
            rnd = 1 - seconds/NeedSeconds; //MyRandom.Random();
            //修改勇士港升级后speed值变大，倒是rnd 为负的情况
            rnd = rnd < 0 ? 0 : rnd;
            rnd = rnd > 1 ? 1 : rnd;
            SailingData.Ship[index].Times = GameUtils.GetTimeDiffString(over);
        }
        SailingData.Ship[index].DistancePerCent = rnd;
        SailingData.Ship[index].Posion = new Vector3(
            (int) (staticPos[index].x + (staticPos[index + 1].x - staticPos[index].x)*rnd),
            (int) (staticPos[index].y + (staticPos[index + 1].y - staticPos[index].y)*rnd), 0);
    }

    //更新建筑数据
    public void UpdateBuilding(IEvent ievent)
    {
        if (BraveHarborBuild == null)
        {
            return;
        }
        var e = ievent as UIEvent_CityUpdateBuilding;
        if (BraveHarborBuild.AreaId == e.Idx)
        {
            var buildingData = CityManager.Instance.GetBuildingByAreaId(e.Idx);
            RefeshBuildData(buildingData);
        }
    }

    //更新背包
    public void UpdateMedalBag(int bagid, ItemsChangeData bag)
    {
        if (bagid == (int) eBagType.MedalBag)
        {
            {
                // foreach(var baseData in bag.ItemsChange)
                var __enumerator13 = (bag.ItemsChange).GetEnumerator();
                while (__enumerator13.MoveNext())
                {
                    var baseData = __enumerator13.Current;
                    {
                        var index = baseData.Key;
                        var bagitem = SailingData.ShipEquip.BagItem[index];
                        bagitem.BaseItemId = baseData.Value.ItemId;
                        bagitem.Index = baseData.Value.Index;
                        bagitem.BagId = (int) eBagType.MedalBag;
                        bagitem.Exdata.InstallData(baseData.Value.Exdata);
                        if (bagitem.BaseItemId == -1)
                        {
                            bagitem.Exdata.Level = 0;
                            bagitem.Exdata.Exp = 0;
                        }
                    }
                }
            }
            if (SailingData.UI.IsShowWitchUI == 2)
            {
                //SailingData.TempData.TempMedalItem =
                //    SailingData.ShipEquip.BagItem[SailingData.TempData.TempMedalItem.Index];
                {
                    // foreach(var item in SailingData.ShipEquip.DropItem)
                    var __enumerator14 = (SailingData.ShipEquip.DropItem).GetEnumerator();
                    while (__enumerator14.MoveNext())
                    {
                        var item = __enumerator14.Current;
                        {
                            item.IsShowCheck = 0;
                        }
                    }
                }
                {
                    // foreach(var item in SailingData.ShipEquip.BagItem)
                    var __enumerator15 = (SailingData.ShipEquip.BagItem).GetEnumerator();
                    while (__enumerator15.MoveNext())
                    {
                        var item = __enumerator15.Current;
                        {
                            item.IsShowCheck = 0;
                        }
                    }
                }
                if (mSelectType == (int) SelectType.BagSelect)
                {
                    SetTempMedalItem(SailingData.ShipEquip.BagItem[mSelectIndex]);
                }
                else if (mSelectType == (int) SelectType.EquipSelect)
                {
                    SetTempMedalItem(SailingData.ShipEquip.EquipItem[mSelectIndex]);
                }
                RefleshUI();
            }
        }
        else if (bagid == (int) eBagType.MedalUsed)
        {
            {
                // foreach(var baseData in bag.ItemsChange)
                var __enumerator16 = (bag.ItemsChange).GetEnumerator();
                while (__enumerator16.MoveNext())
                {
                    var baseData = __enumerator16.Current;
                    {
                        var index = baseData.Key;
                        var equipitem = SailingData.ShipEquip.EquipItem[index];
                        equipitem.BaseItemId = baseData.Value.ItemId;
                        equipitem.BagId = (int) eBagType.MedalUsed;
                        equipitem.BaseItemId = baseData.Value.ItemId;
                        equipitem.Index = baseData.Value.Index;
                        equipitem.Exdata.InstallData(baseData.Value.Exdata);
                        if (equipitem.BaseItemId == -1)
                        {
                            equipitem.Exdata.Level = 0;
                            equipitem.Exdata.Exp = 0;
                        }
                    }
                }
            }
            if (SailingData.UI.IsShowWitchUI == 2)
            {
                //SailingData.TempData.TempMedalItem =
                //    SailingData.ShipEquip.EquipItem[SailingData.TempData.TempMedalItem.Index];
                {
                    // foreach(var item in SailingData.ShipEquip.DropItem)
                    var __enumerator17 = (SailingData.ShipEquip.DropItem).GetEnumerator();
                    while (__enumerator17.MoveNext())
                    {
                        var item = __enumerator17.Current;
                        {
                            item.IsShowCheck = 0;
                        }
                    }
                }
                {
                    // foreach(var item in SailingData.ShipEquip.BagItem)
                    var __enumerator18 = (SailingData.ShipEquip.BagItem).GetEnumerator();
                    while (__enumerator18.MoveNext())
                    {
                        var item = __enumerator18.Current;
                        {
                            item.IsShowCheck = 0;
                        }
                    }
                }
                if (mSelectType == (int) SelectType.BagSelect)
                {
                    SetTempMedalItem(SailingData.ShipEquip.BagItem[mSelectIndex]);
                }
                else if (mSelectType == (int) SelectType.EquipSelect)
                {
                    SetTempMedalItem(SailingData.ShipEquip.EquipItem[mSelectIndex]);
                }
                RefleshUI();
            }
            CalculateItemProp();
        }
        else if (bagid == (int) eBagType.MedalTemp)
        {
            {
                // foreach(var baseData in bag.ItemsChange)
                var __enumerator19 = (bag.ItemsChange).GetEnumerator();
                while (__enumerator19.MoveNext())
                {
                    var baseData = __enumerator19.Current;
                    {
                        var index = baseData.Key;
                        var dropitem = SailingData.ShipEquip.DropItem[index];
                        dropitem.BaseItemId = baseData.Value.ItemId;
                        dropitem.BagId = (int) eBagType.MedalTemp;
                        dropitem.BaseItemId = baseData.Value.ItemId;
                        dropitem.Index = baseData.Value.Index;
                        dropitem.Exdata.InstallData(baseData.Value.Exdata);
                        if (baseData.Value.ItemId == -1)
                        {
                            dropitem.Exdata.Level = 0;
                            dropitem.Exdata.Exp = 0;
                        }
                    }
                }
            }
        }
    }

    public void WoodTipsOk()
    {
        IsScan = !IsScan;
        SetScanName(IsScan);
        SailingData.IsShowWoodTip = false;
        if (mAutoShipCoroutine != null)
        {
            NetManager.Instance.StopCoroutine(mAutoShipCoroutine);
            mAutoShipCoroutine = null;
        }
        mAutoShipCoroutine = NetManager.Instance.StartCoroutine(AutoShipClickCoroutine(ScanCount));
    }

    //-----------------------------------接口---------------------------------------
    public INotifyPropertyChanged GetDataModel(string name)
    {
        return SailingData;
    }

    public void CleanUp()
    {
        if (SailingData != null)
        {
            SailingData.UI.ColorSelect.PropertyChanged -= OnToggleChange;
        }
        SailingData = new SailingDataModel();
        SailingData.UI.ColorSelect.PropertyChanged += OnToggleChange;
        for (var i = 0; i < SailingData.Ship.Count; i++)
        {
            SailingData.Ship[i] = new SailingShipDataModel();
        }
        ScanTips270250 = GameUtils.GetDictionaryText(270250);
        ScanTips270249 = GameUtils.GetDictionaryText(270249);

        var disStr = GameUtils.GetDictionaryText(300848);
        for (var i = 0; i < SailingData.DistanceStr.Count; i++)
        {
            SailingData.DistanceStr[i] = Table.GetSailing(i).Distance + disStr;
        }
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        if (name == "InitMedalBag")
        {
            InitMedalBag(param[0] as BagBaseData);
        }
        else if (name == "UpdateMedalBag")
        {
            UpdateMedalBag((int) param[0], param[1] as ItemsChangeData);
        }

        return null;
    }

    public void OnShow()
    {
    }

    public void Close()
    {
        PlayerDataManager.Instance.WeakNoticeData.Sailing = false;
        EventDispatcher.Instance.DispatchEvent(new CityBulidingWeakNoticeRefresh(BraveHarborBuild));
    }

    public void Tick()
    {
        if (State == FrameState.Close)
        {
            return;
        }
        if (DataState == -1)
        {
            RefeshBuildData(BraveHarborBuild);
            return;
        }

        if (DataState != 1 && DataState != 10)
        {
            if (0 != Time.frameCount%30)
            {
                return;
            }
        }

        if (DataState == 1)
        {
            var prob = GetShipPercent(ActionIndex);
            if (prob > 0 && prob < 1)
            {
                prob += ActionCount*ScanSpeed;
                if (prob > 1)
                {
                    BraveHarborBuild.Exdata[ActionIndex] = BraveHarborBuild.Exdata[ActionIndex]/10*10 + 3;
                    // SailingData.Ship[ActionIndex].Times = GameUtils.GetDictionaryText(1500);
                    //BraveHarborBuild.Exdata64[ActionIndex] = Game.Instance.ServerTime.ToBinary();
                    prob = 1;
                    DataState = 2;
                }
                ActionCount++;
                ResetShipPercent(ActionIndex, prob);
            }
            return;
        }

        if (DataState == 10)
        {
            var prob = ActionCount*ScanSpeed;
            if (prob > 1)
            {
                BraveHarborBuild.Exdata[ActionIndex] = BraveHarborBuild.Exdata[ActionIndex]/10*10 + 3;
                //SailingData.Ship[ActionIndex].Times = GameUtils.GetDictionaryText(1500);
                prob = 1;
                DataState = 2;
            }
            ActionCount++;
            ResetShipPercent(ActionIndex, prob);
            return;
        }
        var index = 0;


        {
            var __list20 = BraveHarborBuild.Exdata;
            var __listCount20 = __list20.Count;
            for (var __i20 = 0; __i20 < __listCount20; ++__i20)
            {
                var i = __list20[__i20];
                {
                    if (index > 4)
                    {
                        break;
                    }
                    SailingData.States[index] = i;
                    if (i == 2 || i == 12)
                    {
                        UpdataShip(index);
                    }
                    index++;
                }
            }
        }
    }

    public void RefreshData(UIInitArguments data)
    {
        var args = data as SailingArguments;
        DataState = 0;
        if (args == null)
        {
            return;
        }
        if (args.Tab > 1)
        {
            return;
        }
        SailingData.UI.IsShowWitchUI = args.Tab;
        BraveHarborBuild = args.BuildingData;
        RefeshBuildData(BraveHarborBuild);
        SetScanName(IsScan);
        InitData();
        CalculateItemProp();
    }

    public FrameState State { get; set; }
}