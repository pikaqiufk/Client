#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using LitJson;
using ScorpionNetLib;
using Shared;

#endregion

public class RechargeFrameController : IControllerBase
{
    public RechargeFrameController()
    {
        platfrom = "android";
#if UNITY_ANDROID
        platfrom = "android";
#elif UNITY_IOS
        platfrom = "ios";
#endif
        CleanUp();
        EventDispatcher.Instance.AddEventListener(UIEvent_RechargeFrame_OnClick.EVENT_TYPE, OnFrameClicked);
        EventDispatcher.Instance.AddEventListener(ExDataInitEvent.EVENT_TYPE, OnExDataInit);
        EventDispatcher.Instance.AddEventListener(ExData64InitEvent.EVENT_TYPE, OnExDataInit);
        EventDispatcher.Instance.AddEventListener(ExDataUpDataEvent.EVENT_TYPE, OnExDataUpData);
        EventDispatcher.Instance.AddEventListener(Resource_Change_Event.EVENT_TYPE, OnUpdataVipExp);
        EventDispatcher.Instance.AddEventListener(RechargeSuccessEvent.EVENT_TYPE, OnRechargeSuccess);
        EventDispatcher.Instance.AddEventListener(OnTouZiBtnClick_Event.EVENT_TYPE, OnTouZiBtnClick);
 
    }

    public RechargeDataModel DataModel;
    private int maxVipLevel;
    private readonly HashSet<int> mExdataKey = new HashSet<int>();
    private readonly string platfrom;

    private void BuyBannerItem()
    {
        if (DataModel.BannerItem != null)
        {
            OnRechargeItemClick(DataModel.BannerItem.TableId);
        }
    }

    private IEnumerator BuyGoods(int tableid)
    {

        var outMessage = new ApplyOrderMessage();
        var channel = GameSetting.Channel;
        outMessage.Channel = string.Format("{0}.{1}", platfrom, channel);
        outMessage.GoodId = tableid;
        outMessage.ExtInfo = "XXXX";

        var msg = NetManager.Instance.ApplyOrderSerial(outMessage);
        yield return msg.SendAndWaitUntilDone();

        if (msg.State == MessageState.Reply)
        {
            if (msg.ErrorCode == (int) ErrorCodes.OK)
            {

                var table = Table.GetRecharge(tableid);
                var roleid = PlayerDataManager.Instance.GetGuid().ToString();
                var roleName = PlayerDataManager.Instance.GetName();
                var goodsName = table.Name;
                var oid = msg.Response.OrderId;
                var level = PlayerDataManager.Instance.GetRes((int)eResourcesType.LevelRes).ToString();
                var serverId = PlayerDataManager.Instance.ServerId.ToString();
                var serverName = PlayerDataManager.Instance.ServerName;

                var sb = new System.Text.StringBuilder();
                var writer = new JsonWriter(sb);
                writer.WriteObjectStart();
                writer.WritePropertyName("roleID");
                writer.Write(roleid);
                writer.WritePropertyName("roleName");
                writer.Write(roleName);
                writer.WritePropertyName("goodsName");
                writer.Write(goodsName);
                writer.WritePropertyName("goodsPrice");
                writer.Write(table.Price.ToString());
                writer.WritePropertyName("oid");
                writer.Write(oid);
                writer.WritePropertyName("roleLevel");
                writer.Write(level);
                writer.WritePropertyName("serverId");
                writer.Write(serverId);
                writer.WritePropertyName("serverName");
                writer.Write(serverName);

                writer.WriteObjectEnd();
                PlatformHelper.MakePayWithGoodInfo(sb.ToString());
            }
        }
    }

    private RechargeItemDataModel CreateItemFromTable(RechargeRecord table)
    {
        if (table.Visible != 0 && table.Platfrom.Equals(platfrom) && table.Type != 3)
        {
            var item = new RechargeItemDataModel();
            item.TableId = table.Id;
            item.ItemId = table.ItemId;
            item.GoodName = table.Name;
            item.GoodPrice = table.Price;
            item.GoodType = table.Type;
            item.GoodUnit = GetGoodUnit();
            item.PurchaseTimes = PlayerDataManager.Instance.GetExData(table.ExdataId);
            if (item.GoodType == 0) // 月卡
            {
                var timespan = GetMonthCardLastTime();
                if (timespan.TotalSeconds > 0)
                {
                    var desc = string.Format(table.Desc, (int) timespan.TotalDays);
                    item.GoodDesc = desc;
                    item.Recommendation = false;
                }
                else
                {
                    item.GoodDesc = table.ExDesc;
                    item.Recommendation = true;
                }
            }
            else if (item.GoodType == 1) // 普通充值
            {
                var lastTimes = table.ExTimes - item.PurchaseTimes;
                if (lastTimes > 0)
                {
                    item.GoodDesc = string.Format(table.ExDesc, lastTimes);
                    item.Recommendation = true;
                }
                else
                {
                    item.GoodDesc = table.Desc;
                    item.Recommendation = false;
                }
            }
            return item;
        }
        return null;
    }

    private string GetGoodUnit()
    {
        var unit = "元";

        //example
        if (GameSetting.Channel.Equals("91netdragon"))
        {
            unit = "豆";
        }
        return unit;
    }

    private TimeSpan GetMonthCardLastTime()
    {
        var expirationDate = /*DateTime.Now.AddHours(1).ToBinary();*/
            PlayerDataManager.Instance.GetExData64((int) Exdata64TimeType.MonthCardExpirationDate);
        var expirationTime = Extension.FromServerBinary(expirationDate);
        return expirationTime - Game.Instance.ServerTime;
    }

    private void OnDrugStore()
    {
        var arg = new StoreArguments {Tab = 0};
        var e = new Show_UI_Event(UIConfig.StoreUI, arg);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    private void OnExDataInit(IEvent ievent)
    {
        RefreshFristRecharge();
        RefreshRechargeItem();
        RefreshBanner();
    }

    private void OnExDataUpData(IEvent ievent)
    {
        var e = ievent as ExDataUpDataEvent;

        if (mExdataKey.Contains(e.Key))
        {
            RefreshRechargeItem();
            RefreshBanner();
        }

        if (e.Key == (int) eExdataDefine.e69)
        {
            RefreshBanner();
        }
    }

    private void OnFrameClicked(IEvent ievent)
    {
        var e = ievent as UIEvent_RechargeFrame_OnClick;

        switch (e.index)
        {
            case 0: // 充值物品点击
            {
                if (e.exData != null)
                {
                    OnRechargeItemClick((int) e.exData);
                }
                else
                {
                    Logger.Error("recharge item tableid = null!!");
                }
            }
                break;
            case 1: // banner购买
            {
                BuyBannerItem();
            }
                break;
            case 2: //随身药店
            {
                OnDrugStore();
            }
                break;
            case 3: //随身仓库
            {
                OnWarehouse();
            }
                break;
            case 4: //快捷修理
            {
                OnQuickRepair();
            }

                break;
            case 5: //vip信息翻页 -1前 1 后
            {
                if (e.exData != null)
                {
                    VipInfoPageScroll((int) e.exData);
                }
            }

                break;
        }
    }

    private void OnQuickRepair()
    {
        GameUtils.OnQuickRepair();
    }

    private void OnRechargeItemClick(int tableid)
    {
        NetManager.Instance.StartCoroutine(BuyGoods(tableid));
    }

    private void OnTouZiBtnClick(IEvent ievent)
    {
        var e = ievent as OnTouZiBtnClick_Event;
        if (e == null)
        {
            return;
        }

        var tableid = e.TableId;
        NetManager.Instance.StartCoroutine(BuyGoods(tableid));
    }

    private void OnRechargeSuccess(IEvent ievent)
    {
        var e = ievent as RechargeSuccessEvent;
        var table = Table.GetRecharge(e.RechargeId);
        if (table != null)
        {
            var str = GameUtils.GetDictionaryText(300833);
            UIManager.Instance.ShowMessage(MessageBoxType.Ok, str);
        }
    }

    private void OnUpdataVipExp(IEvent ievent)
    {
        var e = ievent as Resource_Change_Event;
        if (e.Type == eResourcesType.VipExpRes)
        {
            RefreshVipInfo();
        }
        else if (e.Type == eResourcesType.VipLevel)
        {
            RefreshVipInfo();
        }
    }

    private void OnWarehouse()
    {
        EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.DepotUI));
    }

    private void RefreshBanner()
    {
        DataModel.ShowFristChargeBanner = false;
        DataModel.ShowMonthBanner = false;
        DataModel.ShowItemBanner = false;

        var payCountTotal = PlayerDataManager.Instance.GetExData(eExdataDefine.e69);
//         if (payCountTotal < 1)
//         {
//             DataModel.ShowFristChargeBanner = true;
//         }
//         else
        {
            if (GetMonthCardLastTime().TotalSeconds < 0)
            {
                DataModel.ShowMonthBanner = true;
                DataModel.BannerItem = DataModel.RechargeItems.First(item => item.GoodType == 0);
            }
            else
            {
                DataModel.ShowItemBanner = true;
                var c = DataModel.RechargeItems.Count;
                DataModel.BannerItem = DataModel.RechargeItems[c - 1];
                for (var i = 0; i < c; i++)
                {
                    var item = DataModel.RechargeItems[i];
                    if (item.GoodType == 0)
                    {
                        continue;
                    }
                    var record = Table.GetRecharge(item.TableId);
                    var exdata = PlayerDataManager.Instance.GetExData(record.ExdataId);
                    if (record.ExTimes > exdata)
                    {
                        DataModel.BannerItem = item;
                        break;
                    }
                }
            }
        }
    }

    //刷新首充物品
    private void RefreshFristRecharge()
    {
        //首充奖励
        DataModel.FristChargeReward.Clear();
        var tbId = 590 + PlayerDataManager.Instance.GetRoleId();
        var configTable = Table.GetClientConfig(tbId);
        if (configTable != null)
        {
            var skillUpGradeTable = Table.GetSkillUpgrading(configTable.Value.ToInt());
            if (skillUpGradeTable != null)
            {
                var itemIds = skillUpGradeTable.Values;
                var c = itemIds.Count;
                for (var i = 0; i < c; i++)
                {
                    var id = itemIds[i];
                    var item = new ItemIdDataModel();
                    item.ItemId = id;
                    item.Count = 1;
                    DataModel.FristChargeReward.Add(item);
                }
            }
        }
    }

    //刷新充值商品
    public void RefreshRechargeItem()
    {
        DataModel.RechargeItems.Clear();
        //充值物品
        Table.ForeachRecharge(table =>
        {
            var item = CreateItemFromTable(table);
            if (null != item)
            {
                DataModel.RechargeItems.Add(item);
            }
            return true;
        });
    }

    //刷新vip特权功能开放
    private void RefreshVipFunction()
    {
        if (DataModel.VipLevel < 1)
        {
            return;
        }
        var table = Table.GetVIP(DataModel.VipLevel);
        DataModel.RepairShow = table.Repair;
        DataModel.StoreShow = table.Depot;
    }

    //刷新vip信息
    private void RefreshVipInfo()
    {
        //vip相关
        DataModel.VipExp = PlayerDataManager.Instance.GetRes((int) eResourcesType.VipExpRes);
        DataModel.VipLevel = PlayerDataManager.Instance.GetRes(15);

        if (DataModel.VipLevel != maxVipLevel)
        {
            var nextLevel = DataModel.VipLevel < maxVipLevel ? DataModel.VipLevel + 1 : maxVipLevel;
            var tbVip = Table.GetVIP(nextLevel);
            DataModel.VipExpString = string.Format("{0}/{1}", DataModel.VipExp, tbVip.NeedVipExp);
            DataModel.VipProgressValue = DataModel.VipExp/(float) tbVip.NeedVipExp;
            var levelupExp = tbVip.NeedVipExp - DataModel.VipExp;
            DataModel.NeedVipExpToLevelUp = levelupExp.ToString();
            DataModel.IsHideMaxLabel = 0;
        }
        else
        {
            DataModel.VipExpString = "MAX";
            DataModel.VipProgressValue = 1;
            DataModel.NeedVipExpToLevelUp = "0";
            DataModel.IsHideMaxLabel = 1;
        }

        RefreshVipFunction();
    }

    //刷新vip功能信息页面
    private void ToggleVipInfo()
    {
        if (DataModel.VipInfoIndex == 0)
        {
            var nextLevel = DataModel.VipLevel < maxVipLevel ? DataModel.VipLevel + 1 : maxVipLevel;
            DataModel.VipInfoIndex = nextLevel;
        }
        var tbVip = Table.GetVIP(DataModel.VipInfoIndex);
        DataModel.VipInfo = tbVip.Desc.Replace("\\n", "\n");
        var netLabel = DataModel.VipInfo.Replace("\n", "");
        //计算行数
        var count = DataModel.VipInfo.Length - netLabel.Length;
        DataModel.VipInfoLineCount = count + 1;

        DataModel.VipItemId = new ItemIdDataModel
        {
            Count = 1,
            ItemId = tbVip.PackItemParam[0]
        };

        DataModel.VipBuffId = new ItemIdDataModel
        {
            Count = 1,
            ItemId = tbVip.PackItemParam[1]
        };

        DataModel.VipTitleid = new ItemIdDataModel
        {
            Count = 1,
            ItemId = tbVip.PackItemParam[2]
        };

        DataModel.NeedDiamond = tbVip.NeedVipExp;
    }

    private void VipInfoPageScroll(int direction)
    {
        if (direction > 0)
        {
            if (DataModel.VipInfoIndex < maxVipLevel)
            {
                DataModel.VipInfoIndex++;
            }
        }
        else
        {
            if (DataModel.VipInfoIndex > 1)
            {
                DataModel.VipInfoIndex--;
            }
        }

        ToggleVipInfo();
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        return null;
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        if (name.Equals("RechargeDataModel"))
        {
            return DataModel;
        }

        return null;
    }

    public void RefreshData(UIInitArguments data)
    {
        var args = data as RechargeFrameArguments;

        if (args != null && args.Tab != -1)
        {
            DataModel.TableSelect = args.Tab;
        }
        else
        {
            DataModel.TableSelect = 0;
        }

        RefreshVipInfo();
        ToggleVipInfo();
    }

    public void Tick()
    {
    }

    public void Close()
    {
    }

    public void OnShow()
    {
    }

    public void CleanUp()
    {
        DataModel = new RechargeDataModel();
        maxVipLevel = 0;

        mExdataKey.Clear();
        Table.ForeachVIP(table =>
        {
            maxVipLevel++;
            return true;
        });

        Table.ForeachRecharge(table =>
        {
            mExdataKey.Add(table.ExdataId);
            return true;
        });

        maxVipLevel -= 2; // 去掉首0和尾max
    }

    public FrameState State { get; set; }
}