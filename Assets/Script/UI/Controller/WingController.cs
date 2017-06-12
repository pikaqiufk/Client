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

public class WingController : IControllerBase
{
    public WingController()
    {
        CleanUp();

        EventDispatcher.Instance.AddEventListener(WingOperateEvent.EVENT_TYPE, OnWingOperate);
        EventDispatcher.Instance.AddEventListener(WingQuailtyCellClick.EVENT_TYPE, OnWingQuailtyCellClick);
        EventDispatcher.Instance.AddEventListener(Event_LevelUp.EVENT_TYPE, OnLevelup);
        EventDispatcher.Instance.AddEventListener(UIEvent_BagItemCountChange.EVENT_TYPE, OnBagItemCountChange);
    }

    public WingDataModel DataModel;
    public Coroutine mAdvanceCoroutine;
    public bool mIsAutoAdvance;
    public Coroutine mTrainCoroutine;

    public IControllerBase mWingChargeController;

    public int[] mWingPartIco =
    {
        Table.GetClientConfig(290).Value.ToInt(), // "翅翼",
        Table.GetClientConfig(291).Value.ToInt(), // "翅鞘",
        Table.GetClientConfig(292).Value.ToInt(), // "翅羽",
        Table.GetClientConfig(293).Value.ToInt(), // "翅骨",
        Table.GetClientConfig(294).Value.ToInt() // "翅翎"
    };

    public string[] mWingPartName =
    {
        GameUtils.GetDictionaryText(270124), // "翅翼",
        GameUtils.GetDictionaryText(270125), // "翅鞘",
        GameUtils.GetDictionaryText(270126), // "翅羽",
        GameUtils.GetDictionaryText(270127), // "翅骨",
        GameUtils.GetDictionaryText(270128) // "翅翎"
    };

    public List<bool> ShowBegin = new List<bool> {true, true, true, true, true};
    //----------------------------------------------------------Train
    //检查是否可进行培养
    public bool CheckTrarWingPart()
    {
        var tbWingTrain = Table.GetWingTrain(DataModel.PartData.TrainId);
        if (tbWingTrain.UsedMoney > PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Gold)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210100));
            PlayerDataManager.Instance.ShowItemInfoGet((int) eResourcesType.GoldRes);
            return false;
        }
        var tbWingQuality = Table.GetWingQuality(DataModel.ItemData.WingQuailty);
        if (tbWingTrain.Condition > tbWingQuality.Segment)
        {
            var str = GameUtils.GetDictionaryText(220304);
            str = string.Format(str, tbWingTrain.Condition);
            var e = new ShowUIHintBoard(str);
            EventDispatcher.Instance.DispatchEvent(e);
            return false;
        }
        if (tbWingTrain.UpStarID == -1)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(220305));
            return false;
        }
        //if (tbWingTrain.MaterialCount > PlayerDataManager.Instance.GetItemCount(tbWingTrain.MaterialID))
        //{
        //    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210101));
        //    PlayerDataManager.Instance.ShowItemInfoGet(tbWingTrain.MaterialID);
        //    return false;
        //}
        if (!GameUtils.CheckEnoughItems(tbWingTrain.MaterialID, tbWingTrain.MaterialCount))
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210101));
            return false;
        }
        return true;
    }

    public WingChargeDataModel GetWingChargeDataModle()
    {
        if (mWingChargeController != null)
        {
            var tempModel = mWingChargeController.GetDataModel("");
            if (tempModel != null)
            {
                var wingCahrgeModel = tempModel as WingChargeDataModel;
                if (wingCahrgeModel != null)
                {
                    return wingCahrgeModel;
                }
            }
        }

        return null;
    }

    //----------------------------------------------------------Advanced
    //检查是否可进行进阶(分为成长跟突破)
    public bool CheckWingAdvanced(bool isShowTip = true)
    {
        if (DataModel.ItemData == null || DataModel.ItemData.WingQuailty == -1)
        {
            return false;
        }
        var tbWingQuality = Table.GetWingQuality(DataModel.ItemData.WingQuailty);
        if (tbWingQuality == null)
            return false;

        var needGold = 0;
        var needItemId = -1;
        var needItemCount = 0;
        if (DataModel.IsAdvanceFull) // 突破
        {
            needItemId = tbWingQuality.BreakNeedItem;
            needItemCount = tbWingQuality.BreakNeedCount;
            needGold = tbWingQuality.BreakNeedMoney;
        }
        else
        {
            needItemId = tbWingQuality.MaterialNeed;
            needItemCount = tbWingQuality.MaterialCount;
            needGold = tbWingQuality.UsedMoney;
        }

        if (needGold > PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Gold)
        {
            if (isShowTip)
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210100));
                PlayerDataManager.Instance.ShowItemInfoGet((int) eResourcesType.GoldRes);
            }
            return false;
        }
        if (tbWingQuality.LevelLimit > PlayerDataManager.Instance.GetLevel())
        {
            if (isShowTip)
            {
                var str = GameUtils.GetDictionaryText(220307);
                str = string.Format(str, tbWingQuality.LevelLimit);
                var e = new ShowUIHintBoard(str);
                EventDispatcher.Instance.DispatchEvent(e);
            }
            return false;
        }
        if (tbWingQuality.Segment >= GameUtils.WingQualityMax)
        {
            if (isShowTip)
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(220308));
            }
            return false;
        }

        if (needItemCount > PlayerDataManager.Instance.GetItemCount(needItemId))
        {
            if (isShowTip)
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210101));
                var items = new Dictionary<int, int>();
                items[needItemId] = needItemCount;

                var tbGift = Table.GetGift(4000);
                if (tbGift != null)
                {
                    if (mWingChargeController == null)
                    {
                        mWingChargeController = UIManager.Instance.GetController(UIConfig.WingChargeFrame);
                    }
                    
                    if (GetWingChargeDataModle() != null && GetWingChargeDataModle().IsShowWingCharge == 1) // 没领取过翅膀商城
                    {
                        EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.WingChargeFrame));
                    }
                    else
                    {
                        GameUtils.CheckEnoughItems(items, true);
                    }
                }
                else
                {
                    GameUtils.CheckEnoughItems(items, true);
                }
            }
            return false;
        }

        return true;
    }

    //检查所有阶翅膀图标是否为灰色
    public void CheckWingPartGrey()
    {
        DataModel.TrainNotice = false;
        for (var i = 0; i < 5; i++)
        {
            CheckWingPartGreyIndex(i);
        }
    }

    //检查某一阶阶翅膀图标是否为灰色
    public void CheckWingPartGreyIndex(int index)
    {
        if (index < 0 || index >= 5)
        {
            return;
        }
        var trainId = DataModel.ItemData.ExtraData[index*2 + 1];
        var tbTrain = Table.GetWingTrain(trainId);

        var tbWingQuality = Table.GetWingQuality(DataModel.ItemData.WingQuailty);

        if (tbTrain.TrainCount == 1
            && tbTrain.TrainStar == 1
            && tbTrain.Condition > tbWingQuality.Segment)
        {
            DataModel.IsIcoGrey[index] = true;
            DataModel.TrainNotice = true;
        }
        else
        {
            DataModel.IsIcoGrey[index] = false;
        }
    }

    //人物属性修正
    public int FixAttributeValue(int attrId, int attrValue)
    {
        if (attrId == 21 || attrId == 22)
        {
            return attrValue*100;
        }
        return attrValue;
    }

    //初始化升级所需物品数据
    public void InitWingItem(BagBaseData bagBase)
    {
        if (bagBase.Items.Count == 0)
        {
            DataModel.ItemData.ItemId = -1;
            return;
        }
        var itemInfo = bagBase.Items[0];
        DataModel.ItemData.Count = itemInfo.Count;
        DataModel.ItemData.Index = itemInfo.Index;
        DataModel.ItemData.ExtraData.InstallData(itemInfo.Exdata);
        SetWingItemId(itemInfo.ItemId);
        RefresWholeAttribute();
        CheckWingPartGrey();
    }

    //初始化翅膀升阶信息
    public void InitWingQuailtys()
    {
        var roleType = PlayerDataManager.Instance.GetRoleId();
        if (DataModel.QualityDatas.Count == 0)
        {
            Table.ForeachWingQuality(recoard =>
            {
                if (recoard.Career != roleType)
                {
                    return true;
                }
                if (recoard.Segment > GameUtils.WingQualityMax)
                {
                    return true;
                }
                var data = new WingQualityData();
                data.WingId = recoard.Id;
                data.IsGrey = recoard.Id > DataModel.ItemData.WingQuailty - 1;
                DataModel.QualityDatas.Add(data);

                return true;
            });
        }
        RefresWingAdvanced();
        RefresWingQualityAttribute(DataModel.ItemData.ItemId);
        var tbWingQuality = Table.GetWingQuality(DataModel.ItemData.WingQuailty);
        if (tbWingQuality == null)
        {
            Logger.Error("InitWingQuailtys: tbWingQuality ==null");
        }

        var curGrowValue = DataModel.ItemData.ExtraData.Benison;
        var maxGrowValue = 0;
        if (tbWingQuality != null)
        {
            maxGrowValue = tbWingQuality.GrowProgress;
        }
        DataModel.IsAdvanceFull = (curGrowValue >= maxGrowValue);

        DataModel.AdvanceSlider.MaxValues = new List<int> { maxGrowValue };
        if (maxGrowValue != 0)
        {
            DataModel.AdvanceSlider.BeginValue = curGrowValue / (float)maxGrowValue;
            DataModel.AdvanceSlider.TargetValue = DataModel.AdvanceSlider.BeginValue;
        }
        else
        {
            DataModel.AdvanceSlider.BeginValue = 0.0f;
            DataModel.AdvanceSlider.TargetValue = DataModel.AdvanceSlider.BeginValue;
        }
    }

    private void OnBagItemCountChange(IEvent ievent)
    {
        var e = ievent as UIEvent_BagItemCountChange;
        var tb = Table.GetWingTrain(1);
        if (e.ItemId == tb.MaterialID)
        {
            RefreshWingTranNotice(e.ChangeCount);
        }

        if (DataModel.ItemData == null || DataModel.ItemData.WingQuailty == -1)
        {
            return;
        }

        RefreshWingAdvancedNotice(e.ItemId, e.ChangeCount);
    }

    //点击自动培养
    public void OnClickPartTrainIfAuto()
    {
        if (DataModel.IsAutoTrain)
        {
            OnTrarWingPartAuto();
        }
    }

    //点击翅膀部位培养按钮
    public void OnClickTrarWingPartAuto()
    {
        var tbVip = PlayerDataManager.Instance.TbVip;
        if ((tbVip.WingAdvanced == 0))
        {
            do
            {
                tbVip = Table.GetVIP(tbVip.Id + 1);
            } while (tbVip.WingAdvanced == 0);

            GameUtils.GuideToBuyVip(tbVip.Id);
            return;
        }

        if (DataModel.IsAutoTrain)
        {
            DataModel.IsAutoTrain = false;
            return;
        }
        DataModel.IsAutoTrain = true;
        OnTrarWingPartAuto();
    }

    //点击翅膀部位培养进阶
    public void OnClickWingAdvancedAuto()
    {
        var tbVip = PlayerDataManager.Instance.TbVip;
        if ((tbVip.WingAdvanced == 0))
        {
            do
            {
                tbVip = Table.GetVIP(tbVip.Id + 1);
            } while (tbVip.WingAdvanced == 0);

            GameUtils.GuideToBuyVip(tbVip.Id);
            return;
        }

        if (DataModel.IsAutoAdvance)
        {
            DataModel.IsAutoAdvance = false;
        }
        else
        {
            DataModel.IsAutoAdvance = true;
        }
        WingAdvancedAuto();
    }

    //点击进入部位进行培养
    public void OnClickWingPart(int index)
    {
        DataModel.IsAutoTrain = false;
        PlayerDataManager.Instance.WeakNoticeData.WingTraining = false;
        DataModel.CanUpGrade[index] = false;
        var trainId = DataModel.ItemData.ExtraData[index*2 + 1];
        var tbTrain = Table.GetWingTrain(trainId);

        var tbWingQuality = Table.GetWingQuality(DataModel.ItemData.WingQuailty);

        if (tbTrain.TrainCount == 1
            && tbTrain.TrainStar == 1
            && tbTrain.Condition > tbWingQuality.Segment)
        {
            //需要{0}阶翅膀
            var str = string.Format(GameUtils.GetDictionaryText(270132), tbTrain.Condition);
            var e1 = new ShowUIHintBoard(str);
            EventDispatcher.Instance.DispatchEvent(e1);
            return;
        }
        var partData = new WingPartData();

        partData.IcoId = mWingPartIco[index];
        partData.Name = mWingPartName[index];
        partData.Layer = tbTrain.TrainCount;
        partData.Star = tbTrain.TrainStar;
        partData.ItemData = DataModel.ItemData;
        partData.PartIndex = index;
        partData.TrainId = trainId;
        partData.Exp = DataModel.ItemData.ExtraData[index*2 + 2];

        partData.TrainSlider.MaxValues = new List<int> {tbTrain.ExpLimit};
        partData.TrainSlider.BeginValue = partData.Exp/(float) tbTrain.ExpLimit;
        partData.TrainSlider.TargetValue = partData.TrainSlider.BeginValue;

        DataModel.PartData = partData;
        DataModel.PartSelectedIndex = index;
        OnRefreshWingPart();
    }

    private void OnLevelup(IEvent ievent)
    {
        var level = PlayerDataManager.Instance.GetLevel();
        if (level%10 != 0)
        {
            return;
        }
        if (CheckWingAdvanced(false))
        {
            PlayerDataManager.Instance.WeakNoticeData.WingAdvance = true;
            PlayerDataManager.Instance.WeakNoticeData.BagTotal = true;
            PlayerDataManager.Instance.WeakNoticeData.WingTotal = true;
            PlayerDataManager.Instance.WeakNoticeData.BagEquipWing = true;
        }
    }

    //刷新翅膀部位显示信息
    public void OnRefreshWingPart()
    {
        var partData = DataModel.PartData;
        var tbTrain = Table.GetWingTrain(partData.TrainId);
        var index = partData.PartIndex;

        if (tbTrain.UpStarID == -1)
        {
            partData.IsMaxTrain = true;
        }
        else
        {
            partData.IsMaxTrain = false;
        }

        if (ShowBegin[index])
        {
            if (partData.Exp != 0 || tbTrain.TrainStar != 1)
            {
                ShowBegin[index] = false;
            }
        }
        var star = tbTrain.TrainStar - 1;
        if (tbTrain.UpStarID == -1)
        {
            star = tbTrain.TrainStar;
        }
        var e = new WingRefreshStarPage(tbTrain.PosX, star, partData.PartIndex, ShowBegin[index]);

        if (ShowBegin[index])
        {
            ShowBegin[index] = false;
        }

        EventDispatcher.Instance.DispatchEvent(e);
        RefresPartAttribute();
    }

    //显示部位信息
    public void OnShowStarTip(int type)
    {
        if (type == 0)
        {
            //已到达最前端
            var e = new ShowUIHintBoard(270129);
            EventDispatcher.Instance.DispatchEvent(e);
        }
        else
        {
            var partData = DataModel.PartData;
            var tbTrain = Table.GetWingTrain(partData.TrainId);
            if (tbTrain.TrainCount == 10)
            {
                //已到达最尾端
                var e = new ShowUIHintBoard(270130);
                EventDispatcher.Instance.DispatchEvent(e);
            }
            else
            {
                //当前培养到最大才能开启下一阶段
                var e = new ShowUIHintBoard(270131);
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    //培养部位
    public void OnTrarWingPart()
    {
        if (DataModel.IsAutoTrain)
        {
            DataModel.IsAutoTrain = false;
        }
        if (mTrainCoroutine != null)
        {
            //NetManager.Instance.StopCoroutine(mTrainCoroutine);
        }
        if (!CheckTrarWingPart())
        {
            return;
        }
        mTrainCoroutine = NetManager.Instance.StartCoroutine(WingTrainCoroutine(0.0f));
    }

    //部位自动培养
    public void OnTrarWingPartAuto(float delay = 0.0f)
    {
        if (!CheckTrarWingPart())
        {
            DataModel.IsAutoTrain = false;
            return;
        }
        if (mTrainCoroutine != null)
        {
            NetManager.Instance.StopCoroutine(mTrainCoroutine);
        }
        mTrainCoroutine = NetManager.Instance.StartCoroutine(WingTrainCoroutine(delay));
    }

//监听所有翅膀操作事件包括
// 1.	显示部位信息
// 2.	翅膀部位培养
// 3.	关闭自动培养
// 4.	开启自动培养
// 5.	翅膀升阶
// 6.	开启自动升阶
// 7.	关闭关闭自动升级
    public void OnWingOperate(IEvent ievent)
    {
        var e = ievent as WingOperateEvent;
        switch (e.Type)
        {
            case -3:
            {
                OnShowStarTip(e.Index);
            }
                break;
            case -2:
            {
                if (e.Index >= 0 && e.Index <= 1)
                {
                    DataModel.ShowTab = e.Index;
                }
                DataModel.IsAutoTrain = false;
                DataModel.IsAutoAdvance = false;
            }
                break;
            case -1:
            {
                switch (e.Index)
                {
                    case 0:
                    {
                        OnTrarWingPart();
                    }
                        break;
                    case 1:
                    {
                        OnClickTrarWingPartAuto();
                    }
                        break;
                    case 2:
                    {
                        OnClickPartTrainIfAuto();
                    }
                        break;
                }
            }
                break;
            case 0:
            {
                OnClickWingPart(e.Index);
            }
                break;
            case 1:
            {
                switch (e.Index)
                {
                    case 0:
                    {
                        WingAdvanced();
                    }
                        break;
                    case 1:
                    {
                        OnClickWingAdvancedAuto();
                    }
                        break;
                }
            }
                break;
        }
    }

    //监听翅膀cell点击事件

    public void OnWingQuailtyCellClick(IEvent ievent)
    {
        var e = ievent as WingQuailtyCellClick;
        if (e == null)
            return;
        var wingId = e.Data.WingId;
        if (DataModel.QualityId != wingId)
        {
            RefresWingQualityAttribute(wingId);
            RefreshWingModel(wingId);
        }
    }

    //刷新翅膀阶数数据
    public void RefreshWingAdvance(int ret)
    {
        var oldSliderValue = DataModel.AdvanceSlider.TargetValue;
        var tbWingQuality = Table.GetWingQuality(DataModel.ItemData.WingQuailty);
        if (tbWingQuality == null)
            return;

        if (ret == 1)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(220306));
            DataModel.ItemData.ExtraData.Benison = 0;
            //SetWingItemId(tbWingQuality.Id+1);
            RefresWingAdvanced();
            CheckWingPartGrey();
            RefresWholeAttribute();
            PlayerAttr.Instance.SetAttrChange(PlayerAttr.PlayerAttrChange.Wing);

            tbWingQuality = Table.GetWingQuality(DataModel.ItemData.ItemId);
            var maxList = new List<int>();
            maxList.Add(tbWingQuality.GrowProgress);
            DataModel.AdvanceSlider.BeginValue = 0.0f;
            DataModel.AdvanceSlider.MaxValues = maxList;
            DataModel.AdvanceSlider.TargetValue = DataModel.AdvanceSlider.BeginValue;

            RefreshWingModel(DataModel.ItemData.ItemId);

            var arg = new UIInitArguments();
            arg.Args = new List<int>{DataModel.ItemData.ItemId, 1};;
            var e = new Show_UI_Event(UIConfig.ModelDisplayFrame, arg);
            EventDispatcher.Instance.DispatchEvent(e);
        }
        else
        {
            var newGrowth = DataModel.ItemData.ExtraData.Benison;// +tbWingQuality.FailedAddValue;
            if (newGrowth >= tbWingQuality.GrowProgress)
            {
                newGrowth = tbWingQuality.GrowProgress;
                DataModel.IsAdvanceFull = true;
                DataModel.ItemData.ExtraData.Benison = newGrowth;
                SetWingItemId(tbWingQuality.Id);
            }
            else
            {
                DataModel.ItemData.ExtraData.Benison = newGrowth;
            }

            DataModel.AdvanceSlider.MaxValues = new List<int> {tbWingQuality.GrowProgress};
            DataModel.AdvanceSlider.BeginValue = newGrowth / (float)tbWingQuality.GrowProgress;
            DataModel.AdvanceSlider.TargetValue = DataModel.AdvanceSlider.BeginValue;
        }

        RefresWingQualityAttribute(DataModel.ItemData.ItemId);

        if (DataModel.IsAutoAdvance)
        {
            if (DataModel.IsAdvanceFull)    // 进阶满了，取消自动
            {
                DataModel.IsAutoAdvance = false;
            }
            else
            {
                var dif = DataModel.AdvanceSlider.TargetValue - oldSliderValue;
                var costTime = dif + 0.25f;
                WingAdvancedAuto(costTime);
            }
        }
    }

    private void RefreshWingAdvancedNotice(int itemId, int changeCount)
    {
        if (DataModel.ItemData == null || DataModel.ItemData.WingQuailty == -1)
        {
            return;
        }

        var tbWingQuality = Table.GetWingQuality(DataModel.ItemData.WingQuailty);
        var needItem = -1;
        var needItemCount = 0;
        var needGold = 0;
        if (DataModel.IsAdvanceFull)
        {
            needItem = tbWingQuality.BreakNeedItem;
            needGold = tbWingQuality.BreakNeedMoney;
            needItemCount = tbWingQuality.BreakNeedCount;
        }
        else
        {
            needItem = tbWingQuality.MaterialNeed;
            needGold = tbWingQuality.UsedMoney;
            needItemCount = tbWingQuality.MaterialCount;
        }

        if (needItem != itemId)
        {
            return;
        }

        if (needGold > PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Gold)
        {
            return;
        }

        if (tbWingQuality.LevelLimit > PlayerDataManager.Instance.GetLevel())
        {
            return;
        }
        if (tbWingQuality.Segment >= GameUtils.WingQualityMax)
        {
            return;
        }

        var newvalue = PlayerDataManager.Instance.GetItemCount(needItem);
        var oldvalue = newvalue - changeCount;
        if (needItemCount > oldvalue && needItemCount <= newvalue)
        {
            PlayerDataManager.Instance.WeakNoticeData.WingAdvance = true;
            PlayerDataManager.Instance.WeakNoticeData.BagTotal = true;
            PlayerDataManager.Instance.WeakNoticeData.WingTotal = true;
            PlayerDataManager.Instance.WeakNoticeData.BagEquipWing = true;
        }
    }

    private void RefreshWingTranNotice(int changeCount)
    {
        //调整界面后翅膀培养界面弱红点不在适用
        return;
        var wingItem = DataModel.ItemData;
        for (var i = 0; i < 5; i++)
        {
            if (DataModel.IsIcoGrey[i])
            {
                continue;
            }
            var index = i*2 + 1;
            var id = wingItem.ExtraData[index];
            var tbTrain = Table.GetWingTrain(id);
            if (tbTrain == null)
            {
                continue;
            }

            var count = tbTrain.MaterialCount;
            var gold = PlayerDataManager.Instance.GetRes((int) eResourcesType.GoldRes);
            if (tbTrain.UsedMoney > gold)
            {
                continue;
            }
            var tbWingQuality = Table.GetWingQuality(DataModel.ItemData.WingQuailty);

            if (tbTrain.Condition > tbWingQuality.Segment)
            {
                continue;
            }

            if (tbTrain.UpStarID == -1)
            {
                continue;
            }
            
            var newvalue = PlayerDataManager.Instance.GetItemTotalCount(tbTrain.MaterialID).Count;
            var oldvalue = newvalue - changeCount;

            if (count > oldvalue && count <= newvalue)
            {
                PlayerDataManager.Instance.WeakNoticeData.WingTraining = true;
                PlayerDataManager.Instance.WeakNoticeData.BagTotal = true;
                PlayerDataManager.Instance.WeakNoticeData.WingTotal = true;
                PlayerDataManager.Instance.WeakNoticeData.BagEquipWing = true;
                DataModel.CanUpGrade[i] = true;
            }
        }
    }

    //刷新部位属性
    public void RefresPartAttribute()
    {
        var index = DataModel.PartData.PartIndex;
        DataModel.PartData.PartAttributes.Clear();
        var attrs = new ObservableCollection<AttributeChangeDataModel>();
        var dicAttr = new Dictionary<int, int>();
        var tbWingTrain = Table.GetWingTrain(DataModel.PartData.TrainId);
        var tbWingTrainAddPropIDLength1 = tbWingTrain.AddPropID.Length;
        for (var i = 0; i < tbWingTrainAddPropIDLength1; i++)
        {
            var nAttrId = tbWingTrain.AddPropID[i];
            var nValue = tbWingTrain.AddPropValue[i];
            if (nAttrId < 0 || nValue <= 0)
            {
                break;
            }
            if (nValue > 0 && nAttrId != -1)
            {
                dicAttr.modifyValue(nAttrId, nValue);
            }
        }

        var dicAttrNext = new Dictionary<int, int>();
        if (tbWingTrain.UpStarID != -1)
        {
            var tbWingTrainNext = Table.GetWingTrain(tbWingTrain.UpStarID);
            var tbWingTrainNextAddPropIDLength2 = tbWingTrainNext.AddPropID.Length;
            for (var i = 0; i < tbWingTrainNextAddPropIDLength2; i++)
            {
                var nAttrId = tbWingTrainNext.AddPropID[i];
                var nValue = tbWingTrainNext.AddPropValue[i];
                if (nAttrId < 0 || nValue <= 0)
                {
                    break;
                }
                if (nValue > 0 && nAttrId != -1)
                {
                    dicAttrNext.modifyValue(nAttrId, nValue);
                }
            }
        }
        {
            // foreach(var i in dicAttr)
            var __enumerator2 = (dicAttr).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var i = __enumerator2.Current;
                {
                    var attr = new AttributeChangeDataModel();
                    attr.Type = i.Key;
                    attr.Value = i.Value;
                    var nextValue = 0;
                    if (dicAttrNext.TryGetValue(attr.Type, out nextValue))
                    {
                        attr.Change = nextValue - i.Value;
                        attr.Change = FixAttributeValue(i.Key, attr.Change);
                    }
                    attr.Value = FixAttributeValue(i.Key, attr.Value);
                    attrs.Add(attr);
                }
            }
        }
        {
            // foreach(var i in dicAttrNext)
            var __enumerator3 = (dicAttrNext).GetEnumerator();
            while (__enumerator3.MoveNext())
            {
                var i = __enumerator3.Current;
                {
                    var type = i.Key;
                    var value = 0;
                    if (!dicAttr.TryGetValue(type, out value))
                    {
                        var attr = new AttributeChangeDataModel();
                        attr.Type = type;
                        attr.Value = value;
                        attr.Change = i.Value - value;
                        attr.Change = FixAttributeValue(i.Key, attr.Change);
                        attr.Value = FixAttributeValue(i.Key, attr.Value);
                        attrs.Add(attr);
                    }
                }
            }
        }
        DataModel.PartData.PartAttributes = attrs;
    }

    //刷新翅膀总属性
    public void RefresWholeAttribute()
    {
        var dicAttr = new Dictionary<int, int>();
        var attrs = new ObservableCollection<AttributeBaseDataModel>();
        var tbWing = Table.GetWingQuality(DataModel.ItemData.ItemId);
        if (tbWing == null)
            return;

        PlayerAttr.FillWingAdvanceAttr(dicAttr, DataModel.ItemData);

        //培养属性
        for (var i = 0; i != 5; ++i)
        {
            var tbWingTrain = Table.GetWingTrain(DataModel.ItemData.ExtraData[1 + i*2]);
            if (tbWingTrain == null)
            {
                continue;
            }
            for (var j = 0; j != tbWingTrain.AddPropID.Length; ++j)
            {
                var nAttrId = tbWingTrain.AddPropID[j];
                var nValue = tbWingTrain.AddPropValue[j];
                if (nAttrId < 0 || nValue <= 0)
                {
                    break;
                }
                if (nValue > 0 && nAttrId != -1)
                {
                    if (nAttrId == 105)
                    {
                        if (dicAttr.ContainsKey(5))
                        {
                            dicAttr.modifyValue(5, nValue);
                        }
                        if (dicAttr.ContainsKey(6))
                        {
                            dicAttr.modifyValue(6, nValue);
                        }
                        if (dicAttr.ContainsKey(7))
                        {
                            dicAttr.modifyValue(7, nValue);
                        }
                        if (dicAttr.ContainsKey(8))
                        {
                            dicAttr.modifyValue(8, nValue);
                        }
                    }
                    else
                    {
                        dicAttr.modifyValue(nAttrId, nValue);
                    }
                }
            }
        }
        //翅膀战力
        DataModel.Fightforce = PlayerDataManager.Instance.GetAttrFightPoint(dicAttr);
        {
            // foreach(var i in dicAttr)
            var __enumerator1 = (dicAttr).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var i = __enumerator1.Current;
                {
                    var attr = new AttributeBaseDataModel();
                    attr.Type = i.Key;
                    attr.Value = i.Value;
                    attr.Value = FixAttributeValue(i.Key, attr.Value);
                    attrs.Add(attr);
                }
            }
        }
        DataModel.WholeAttributes = attrs;
    }

    //刷新翅膀阶数显示数据
    public void RefresWingAdvanced()
    {
        {
            // foreach(var data in DataModel.QualityDatas)
            var __enumerator4 = (DataModel.QualityDatas).GetEnumerator();
            while (__enumerator4.MoveNext())
            {
                var data = __enumerator4.Current;
                {
                    if (data.ItemId <= DataModel.ItemData.ItemId)
                    {
                        data.IsGrey = false;
                    }
                    else
                    {
                        data.IsGrey = true;
                    }
                }
            }
        }
    }

    //刷新翅膀升阶人物属性
    public void RefresWingQualityAttribute(int wingId)
    {
        var tbWing = Table.GetWingQuality(wingId);
        if (tbWing == null)
        {
            return;
        }
        if (tbWing.Segment > GameUtils.WingQualityMax)
        {
            GameUtils.ShowHintTip(200012);
            return;
        }

        {
            // foreach(var data in DataModel.QualityDatas)
            var __enumerator5 = (DataModel.QualityDatas).GetEnumerator();
            while (__enumerator5.MoveNext())
            {
                var data = __enumerator5.Current;
                {
                    data.IsSelect = data.ItemId == wingId ? 1 : 0;
                    //var tbQuality = Table.GetWingQuality(data.WingId);
                    if (data.IsSelect == 1)
                    {
                        var tbWingQuality = Table.GetWingQuality(data.WingId);
                        DataModel.SelectQuality = tbWingQuality.Segment;
                        var tbItem = Table.GetItemBase(data.ItemId);
                        if (tbItem != null)
                        {
                            DataModel.SelectName = tbItem.Name;
                        }
                        else
                        {
                            DataModel.SelectName = "";
                        }
                    }
                }
            }
        }

        DataModel.QualityId = wingId;
        DataModel.QualityAttributes.Clear();
        var attrs = new ObservableCollection<AttributeChangeDataModel>();
        var dicAttr = new Dictionary<int, int>();
        var dicAttrNext = new Dictionary<int, int>();
        var dicAttrNextMax = new Dictionary<int, int>();

        if (wingId == DataModel.ItemData.WingQuailty)
        { // 当前阶段
            PlayerAttr.FillWingAdvanceAttr(dicAttr, DataModel.ItemData);
            if (DataModel.IsAdvanceFull)
            {
                PlayerAttr.FillWingBreakAttr(dicAttrNext, wingId + 1);
                // dicAttrNext = dicAttrNext - dicAttr;
                var tempAttrDict = new Dictionary<int, int>();
                var enumorator1 = dicAttrNext.GetEnumerator();
                while (enumorator1.MoveNext())
                {
                    int attr;
                    if (dicAttr.TryGetValue(enumorator1.Current.Key, out attr))
                    {
                        tempAttrDict[enumorator1.Current.Key] = enumorator1.Current.Value - attr;
                    }
                }
                dicAttrNext = tempAttrDict;
            }
            else
            { // 成长属性
                for (var i = 0; i < tbWing.GrowPropID.Length; ++i)
                {
                    var nAttrId = tbWing.GrowPropID[i];
                    if (nAttrId < 0)
                    {
                        break;
                    }
                    var valueMin = tbWing.GrowMinProp[i];
                    var valueMax = tbWing.GrowMaxProp[i];
                    if (valueMin > 0 && valueMax >= valueMin)
                    {
                        dicAttrNext.modifyValue(nAttrId, valueMin);
                        if (valueMax != valueMin)
                        {
                            dicAttrNextMax.modifyValue(nAttrId, valueMax);
                        }
                    }
                }      
            }
        }
        else if (wingId > DataModel.ItemData.WingQuailty)
        { // 其它阶段
            PlayerAttr.FillWingAdvanceAttr(dicAttr, DataModel.ItemData);
            PlayerAttr.FillWingBreakAttr(dicAttrNext, wingId);

            // dicAttrNext = dicAttrNext - dicAttr;
            var tempAttrDict = new Dictionary<int, int>();
            var enumorator1 = dicAttrNext.GetEnumerator();
            while (enumorator1.MoveNext())
            {
                int attr;
                if (dicAttr.TryGetValue(enumorator1.Current.Key, out attr))
                {
                    tempAttrDict[enumorator1.Current.Key] = enumorator1.Current.Value - attr;
                }
            }
            dicAttrNext = tempAttrDict;

            dicAttr.Clear();
            PlayerAttr.FillWingBreakAttr(dicAttr, wingId);
        }
        else
        {
            PlayerAttr.FillWingBreakAttr(dicAttr, wingId);            
        }

        var __enumerator7 = (dicAttr).GetEnumerator();
        while (__enumerator7.MoveNext())
        {
            var i = __enumerator7.Current;
            {
                var attr = new AttributeChangeDataModel();
                attr.Type = i.Key;
                attr.Value = i.Value;
                int nextValue;
                if (dicAttrNext.TryGetValue(attr.Type, out nextValue))
                {
                    attr.Change = nextValue;
                    attr.Change = FixAttributeValue(i.Key, attr.Change);
                    int nextValueMax;
                    if (dicAttrNextMax.TryGetValue(attr.Type, out nextValueMax))
                        attr.ChangeEx = nextValueMax;
                }
                attr.Value = FixAttributeValue(i.Key, attr.Value);
                attrs.Add(attr);
            }
        }

        DataModel.QualityAttributes = attrs;
    }

    public void RefreshWingModel(int modelId)
    {
        var e = new WingModelRefreh(modelId);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    //刷新部位培养经验
    public void RefresWingTrain(int ret)
    {
        for (var i = 0; i < 5; i++)
        {
            DataModel.CanUpGrade[i] = false;
        }

        var NowExp = DataModel.PartData.Exp;
        var tbWingTrain = Table.GetWingTrain(DataModel.PartData.TrainId);
        var oldTrainCount = tbWingTrain.TrainCount;
        if (ret == 1)
        {
//暴击！增加经验 {0}
            NowExp += tbWingTrain.CritAddExp;
            var str = string.Format(GameUtils.GetDictionaryText(220301), tbWingTrain.CritAddExp);
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(str));

            var e5 = new WingNotifyLogicEvent(1);
            EventDispatcher.Instance.DispatchEvent(e5);
        }
        else
        {
//增加经验 {0}
            var str = string.Format(GameUtils.GetDictionaryText(220300), tbWingTrain.AddExp);
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(str));
            NowExp += tbWingTrain.AddExp;
        }
        var levelup = 0;
        var maxList = new List<int>();
        maxList.Add(tbWingTrain.ExpLimit);
        while (NowExp >= tbWingTrain.ExpLimit)
        {
            levelup++;
            if (tbWingTrain.UpStarID == -1)
            {
                NowExp = 0;
                break;
            }
            NowExp -= tbWingTrain.ExpLimit;
            tbWingTrain = Table.GetWingTrain(tbWingTrain.UpStarID);
            maxList.Add(tbWingTrain.ExpLimit);
            PlayerAttr.Instance.SetAttrChange(PlayerAttr.PlayerAttrChange.Wing);
        }

        var partIndex = DataModel.PartData.PartIndex;

        DataModel.PartData.Exp = NowExp;
        DataModel.ItemData.ExtraData[partIndex*2 + 2] = DataModel.PartData.Exp;

        var oldSliderValue = DataModel.PartData.TrainSlider.TargetValue;
        if (levelup > 0)
        {
            DataModel.PartData.Layer = tbWingTrain.TrainCount;
            DataModel.PartData.Star = tbWingTrain.TrainStar;

            DataModel.ItemData.ExtraData[partIndex*2 + 1] = tbWingTrain.Id;
            DataModel.PartData.TrainId = tbWingTrain.Id;

            if (tbWingTrain.UpStarID == -1)
            {
                DataModel.PartData.IsMaxTrain = true;
            }
            CheckWingPartGreyIndex(partIndex);
            RefresWholeAttribute();
            RefresPartAttribute();
            var newTrainCount = tbWingTrain.TrainCount;
            if (newTrainCount != oldTrainCount)
            {
//成功升级，当前重数已培养完成
                var e2 = new ShowUIHintBoard(220303);
                EventDispatcher.Instance.DispatchEvent(e2);
                var e = new WingRefreshTrainCount(tbWingTrain.PosX);
                EventDispatcher.Instance.DispatchEvent(e);
                OnRefreshWingPart();

                // PartIndex 0->翅翼  1-》翅翘 2-》翅羽 3-》翅骨 4->翅翎
                PlatformHelper.UMEvent("WingTrain", DataModel.PartData.PartIndex.ToString(), newTrainCount);
            }
            else
            {
//成功升级
                var e1 = new ShowUIHintBoard(220302);
                EventDispatcher.Instance.DispatchEvent(e1);

                var e = new WingRefreshStarCount(tbWingTrain.TrainStar - 1);
                EventDispatcher.Instance.DispatchEvent(e);
            }
            DataModel.PartData.TrainSlider.MaxValues = maxList;
            DataModel.PartData.TrainSlider.TargetValue = DataModel.PartData.Exp/(float) tbWingTrain.ExpLimit +
                                                         (maxList.Count - 1);
        }
        else
        {
            DataModel.PartData.TrainSlider.MaxValues = new List<int> {tbWingTrain.ExpLimit};
            DataModel.PartData.TrainSlider.TargetValue = DataModel.PartData.Exp/(float) tbWingTrain.ExpLimit;

            var dif = DataModel.PartData.TrainSlider.TargetValue - oldSliderValue;
            var costTime = dif + 0.15f;
            if (DataModel.IsAutoTrain)
            {
                OnTrarWingPartAuto(costTime);
            }
        }
    }

    //设置翅膀物品ID
    public void SetWingItemId(int id)
    {
        DataModel.ItemData.ItemId = id;
        var tbWingQuality = Table.GetWingQuality(DataModel.ItemData.ItemId);
        if (tbWingQuality.Segment == GameUtils.WingQualityMax)
        {
            DataModel.IsMaxAdvance = true;
        }
        else
        {
            DataModel.IsMaxAdvance = false;
        }

        var curGrowValue = DataModel.ItemData.ExtraData.Benison;
        var maxGrowValue = tbWingQuality.GrowProgress;
        DataModel.IsAdvanceFull = (curGrowValue >= maxGrowValue);
        if (DataModel.IsAdvanceFull)
        {
            DataModel.ItemData.NeedItemId = tbWingQuality.BreakNeedItem;
            DataModel.ItemData.NeedItemCount = tbWingQuality.BreakNeedCount;
            DataModel.AdvanceCostMoney = tbWingQuality.BreakNeedMoney;
        }
        else
        {
            DataModel.ItemData.NeedItemId = tbWingQuality.MaterialNeed;
            DataModel.ItemData.NeedItemCount = tbWingQuality.MaterialCount;
            DataModel.AdvanceCostMoney = tbWingQuality.UsedMoney;
        }
    }

    //刷新翅膀升级所需物品
    public void UpdateWingItem(ItemsChangeData changeData)
    {
        var itemInfo = changeData.ItemsChange[0];
        SetWingItemId(itemInfo.ItemId);
        DataModel.ItemData.Count = itemInfo.Count;
        DataModel.ItemData.Index = itemInfo.Index;
        DataModel.ItemData.ExtraData.InstallData(itemInfo.Exdata);
        //OnClickWingPart(0);
        RefresWholeAttribute();
        CheckWingPartGrey();
        PlayerAttr.Instance.SetAttrChange(PlayerAttr.PlayerAttrChange.Equip);
    }

    //翅膀进阶
    public void WingAdvanced()
    {
        if (DataModel.IsAutoAdvance)
        {
            DataModel.IsAutoAdvance = false;
        }
        if (!CheckWingAdvanced())
        {
            return;
        }
        if (mAdvanceCoroutine != null)
        {
            NetManager.Instance.StopCoroutine(mAdvanceCoroutine);
        }
        mAdvanceCoroutine = NetManager.Instance.StartCoroutine(WingAdvancedCoroutine(0.0f));
    }

    //翅膀自动进阶
    public void WingAdvancedAuto(float delay = 0.0f)
    {
        if (!CheckWingAdvanced())
        {
            DataModel.IsAutoAdvance = false;
            return;
        }
        if (mAdvanceCoroutine != null)
        {
            NetManager.Instance.StopCoroutine(mAdvanceCoroutine);
        }
        mAdvanceCoroutine = NetManager.Instance.StartCoroutine(WingAdvancedCoroutine(delay));
    }

    //发送翅膀进阶网络请求
    public IEnumerator WingAdvancedCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        using (new BlockingLayerHelper(1))
        {
            var msg = NetManager.Instance.WingFormation(-1);
            yield return msg.SendAndWaitUntilDone();
            mAdvanceCoroutine = null;
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    PlayerDataManager.Instance.SyncResources(msg.Response.Resources);
                    NetManager.Instance.SyncItems(msg.Response.Items);
                    RefreshWingAdvance(msg.Response.AdvanceRet);

                    if (msg.Response.AdvanceRet == 1)
                    {
                        var tbWingQuality = Table.GetWingQuality(DataModel.ItemData.ItemId);
                        if (tbWingQuality != null)
                        {
                            PlatformHelper.UMEvent("WingAdvance", tbWingQuality.Segment.ToString());
                        } 
                    }
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    DataModel.IsAutoAdvance = false;
                    Logger.Error("WingFormation Error!............ErrorCode..." + msg.ErrorCode);
                }
            }
            else
            {
                DataModel.IsAutoAdvance = false;
                Logger.Error("WingFormation Error!............State..." + msg.State);
            }
        }
    }

    //向服务器发送翅膀部位培养请求
    public IEnumerator WingTrainCoroutine(float delay)
    {
        //using (new BlockingLayerHelper(1))
        {
            yield return new WaitForSeconds(delay);

            if (!DataModel.IsAutoTrain && delay > 0.0001f)
            {
                yield break;
            }

            var msg = NetManager.Instance.WingTrain(DataModel.PartData.PartIndex);
            yield return msg.SendAndWaitUntilDone();
            mTrainCoroutine = null;
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    PlayerDataManager.Instance.SyncResources(msg.Response.Resources);
                    NetManager.Instance.SyncItems(msg.Response.Items);
                    RefresWingTrain(msg.Response.TrainRet);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    DataModel.IsAutoTrain = false;
                    Logger.Error("WingTrain Error!............ErrorCode..." + msg.ErrorCode);
                }
            }
            else
            {
                DataModel.IsAutoTrain = false;
                Logger.Error("WingTrain Error!............State..." + msg.State);
            }
        }
    }

    public void CleanUp()
    {
        DataModel = new WingDataModel();
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        if (name == "UpdateWingItem")
        {
            UpdateWingItem(param[0] as ItemsChangeData);
        }
        else if (name == "InitWingItem")
        {
            InitWingItem(param[0] as BagBaseData);
        }

        return null;
    }

    public void OnShow()
    {
        RefreshWingModel(DataModel.QualityId);
    }

    public void Close()
    {
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        var arg = data as WingArguments;
        if (arg != null && arg.Tab != -1)
        {
            DataModel.ShowTab = arg.Tab;
        }
        else
        {
            DataModel.ShowTab = 1;
        }

        InitWingQuailtys();
        CheckWingPartGrey();
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public FrameState State { get; set; }
}