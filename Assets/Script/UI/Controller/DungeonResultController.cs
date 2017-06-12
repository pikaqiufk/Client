#region using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ClientDataModel;
using DataTable;
using EventSystem;
using Shared;

#endregion

public class DungeonResultController : IControllerBase
{
    public DungeonResultController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(DungeonResultChoose.EVENT_TYPE, OnDungeonResultChoose);
        EventDispatcher.Instance.AddEventListener(UIEvent_CityDungeonResult.EVENT_TYPE, OnCityDungeonResult);
    }

    public DungeonResultDataModel DataModel;
    public int mDrawId;
    public int mDrawIndex;
    //家园副本结果
    public void OnCityDungeonResult(IEvent ievent)
    {
        var e = ievent as UIEvent_CityDungeonResult;
        if (null == e)
        {
            return;
        }

        var paramList = e.Param;

        //副本类型
        var paramIdx = 0;
        var tbFuben = Table.GetFuben(DataModel.FubenId);
        if (tbFuben == null)
        {
            Logger.Error("In OnCityDungeonResult() tbFuben = null!");
            return;
        }

        var myLevel = PlayerDataManager.Instance.GetLevel();
        var assistType = (eDungeonAssistType) tbFuben.AssistType;
        switch (assistType)
        {
            case eDungeonAssistType.CityGoldSingle:
            {
                DataModel.Type = 1;

                var Data = DataModel.GoldDungeon;

                //击杀描述
                var kill = 0;
                var total = 0;
                for (var i = 0; i < Data.Desc.Count; i++)
                {
                    if (paramIdx < paramList.Count)
                    {
                        kill = paramList[paramIdx++];
                        total = paramList[paramIdx++];
                    }
                    kill = Math.Min(kill, total);
                    var temp = string.Format("{0}/{1}", kill, total);
                    if (kill >= total)
                    {
                        Data.Desc[i] = "[00FF00]" + temp + "[-]";
                    }
                    else if (0 == kill)
                    {
                        Data.Desc[i] = "[FF0000]" + temp + "[-]";
                    }
                    else
                    {
                        Data.Desc[i] = "[FFFFFF]" + temp + "[-]";
                    }
                }

                Data.ExtraRewardPercent = 100 + kill*20;
                var rewardScale = 0.01f*Data.ExtraRewardPercent;

                //副本奖励
                for (int i = 0, imax = tbFuben.RewardId.Count; i < imax; ++i)
                {
                    var reward = Data.Rewards[i];
                    var itemId = tbFuben.RewardId[i];
                    var itemCount = tbFuben.RewardCount[i];
                    reward.ItemId = itemId;
                    if (itemId == -1)
                    {
                        continue;
                    }
                    reward.Count = (int) (itemCount*rewardScale + 0.5f);
                }
            }
                break;
            case eDungeonAssistType.CityExpMulty:
            case eDungeonAssistType.CityExpSingle:
            {
                DataModel.Type = 2;

                var rank = paramList[paramIdx++];
                var Data = DataModel.ExpDungeon;
                Data.Rank = rank;
                Data.ShowMulty = assistType == eDungeonAssistType.CityExpMulty;
                if (Data.ShowMulty)
                {
                    Data.LeaderName = e.LeaderName;
                    var leaderGainedReward = paramList[paramIdx++];
                    Data.ShowMulty = leaderGainedReward == 0;
                }

                //副本奖励
                for (int i = 0, imax = tbFuben.RewardId.Count; i < imax; ++i)
                {
                    var itemData = Data.Rewards[i];
                    itemData.ItemId = tbFuben.RewardId[i];
                    itemData.Count = GameUtils.GetRewardCount(tbFuben, tbFuben.RewardCount[i], rank, myLevel);
                }

                //队长奖励
                if (Data.ShowMulty)
                {
                    var itemData = Data.LeaderRewards[0];
                    var count = tbFuben.ScanReward[0];
                    count = GameUtils.GetRewardCount(tbFuben, count, rank, myLevel);
                    itemData.ItemId = -1;
                    itemData.Count = count;

                    count = tbFuben.ScanReward[1];
                    count = GameUtils.GetRewardCount(tbFuben, count, rank, myLevel);
                    itemData = Data.LeaderRewards[1];
                    itemData.ItemId = count > 0 ? (int) eResourcesType.CityWood : -1;
                    itemData.Count = count;
                }
            }
                break;
            case eDungeonAssistType.OrganRoom:
            {
                DataModel.Type = 3;

                var Data = DataModel.GoldDungeon2;

                Data.Gold = paramList[paramIdx++];
            }
                break;
            case eDungeonAssistType.FrozenThrone:
            {
                DataModel.Type = 4;

                var rank = paramList[paramIdx++];
                var Data = DataModel.NormalDungeon;
                Data.Rank = rank;

                var items = new List<ItemIdDataModel>();
                for (int i = 0, imax = tbFuben.RewardId.Count; i < imax; ++i)
                {
                    var itemId = tbFuben.RewardId[i];
                    if (itemId == -1)
                    {
                        break;
                    }
                    var itemCount = tbFuben.RewardCount[i];
                    itemCount = GameUtils.GetRewardCount(tbFuben, itemCount, rank, myLevel);
                    var item = new ItemIdDataModel();
                    item.ItemId = itemId;
                    item.Count = itemCount;
                    items.Add(item);
                }
                Data.Rewards = new ObservableCollection<ItemIdDataModel>(items);
            }
                break;
            case eDungeonAssistType.CastleCraft1:
            case eDungeonAssistType.CastleCraft2:
            case eDungeonAssistType.CastleCraft3:
            case eDungeonAssistType.CastleCraft4:
            case eDungeonAssistType.CastleCraft5:
            case eDungeonAssistType.CastleCraft6:
            {
                DataModel.Type = 5;

                var rank = paramList[paramIdx++];
                var score = paramList[paramIdx++];
                var Data = DataModel.CastleCraft;
                DataModel.Rank = rank;
                Data.Score = score;

                //计算奖励
                var isDynamicReward = tbFuben.IsDynamicExp == 1;
                var items = new Dictionary<int, int>();
                //基础奖励
                for (int i = 0, imax = tbFuben.RewardId.Count; i < imax; ++i)
                {
                    var itemId = tbFuben.RewardId[i];
                    if (itemId == -1)
                    {
                        break;
                    }
                    var itemCount = tbFuben.RewardCount[i];
                    items.modifyValue(itemId, itemCount);
                }

                //额外经验
                var exp = 0;
                if (isDynamicReward)
                {
                    exp = (int) (1.0*tbFuben.DynamicExpRatio*Table.GetLevelData(myLevel).DynamicExp/10000*score);
                }
                if (exp > 0)
                {
                    items.modifyValue((int) eResourcesType.ExpRes, exp);
                }

                //额外荣誉
                var honor = tbFuben.ScanReward[0];
                if (isDynamicReward)
                {
                    honor = Table.GetSkillUpgrading(honor).GetSkillUpgradingValue(rank);
                }
                if (honor > 0)
                {
                    items.modifyValue((int) eResourcesType.Honor, honor);
                }

                if (items.ContainsKey((int) eResourcesType.ExpRes))
                {
                    Data.Exp = items[(int) eResourcesType.ExpRes];
                }
                if (items.ContainsKey((int) eResourcesType.GoldRes))
                {
                    Data.Gold = items[(int) eResourcesType.GoldRes];
                }
                if (items.ContainsKey((int) eResourcesType.Honor))
                {
                    Data.Honor = items[(int) eResourcesType.Honor];
                }
            }
                break;
            case eDungeonAssistType.AllianceWar:
            {
                DataModel.Type = 6;
                var Data = DataModel.AttackCity;
                if (paramList.Count > 0)
                {
                    var sucess = false;
                    var param = paramList[0];
                    var instance = PlayerDataManager.Instance;
                    if (instance._battleCityDic.ContainsKey(param))
                    {
                        DataModel.AttackCity.BattleName = instance._battleCityDic[param].Name +
                                                          GameUtils.GetDictionaryText(270272);
                    }
                    else
                    {
                        DataModel.AttackCity.BattleName = GameUtils.GetDictionaryText(270291);
                    }
                    if (instance.BattleUnionDataModel.MyUnion.UnionID == param)
                    {
                        Data.ResultType = 1;
                    }
                    else
                    {
                        Data.ResultType = 0;
                    }
                }
                var items = new List<ItemIdDataModel>();
                for (int i = 0, imax = tbFuben.RewardId.Count; i < imax; ++i)
                {
                    var itemId = tbFuben.RewardId[i];
                    if (itemId == -1)
                    {
                        break;
                    }
                    var item = new ItemIdDataModel();
                    item.ItemId = itemId;
                    item.Count = tbFuben.RewardCount[i];
                    items.Add(item);
                }
                Data.Rewards = new ObservableCollection<ItemIdDataModel>(items);
            }
                break;
            case eDungeonAssistType.MieShiWar:
                {
                    DataModel.FightResult = paramList[2];
                }
                break;
        }
    }

    public void OnDungeonResultChoose(IEvent ievent)
    {
        var e = ievent as DungeonResultChoose;

        var choose = e.Index;
        var tbDraw = Table.GetDraw(mDrawId);

        if (choose < 0 || choose >= DataModel.AwardItems.Count)
        {
            return;
        }
        if (mDrawIndex < 0 || mDrawIndex >= tbDraw.DropItem.Length)
        {
            return;
        }
        DataModel.AwardItems[choose].ItemId = tbDraw.DropItem[mDrawIndex];
        DataModel.AwardItems[choose].Count = tbDraw.Count[mDrawIndex];


        var flag1 = 0;
        var flag2 = 0;
        for (var i = 0; i < 3; i++)
        {
            if (flag1 == choose)
            {
                flag1++;
            }
            if (flag2 == mDrawIndex)
            {
                flag2++;
            }
            var itemId = tbDraw.DropItem[flag2];
            DataModel.AwardItems[flag1].ItemId = itemId;
            DataModel.AwardItems[flag1].Count = tbDraw.Count[flag2];
            var tbItem = Table.GetItemBase(itemId);
            if (tbItem.Type >= 10000 && tbItem.Type <= 10099)
            {
                GameUtils.EquipRandomAttribute(DataModel.AwardItems[flag1]);
            }
            flag1++;
            flag2++;
        }
    }

    public void CleanUp()
    {
        DataModel = new DungeonResultDataModel();
    }

    public void RefreshData(UIInitArguments data)
    {
        var args = data as DungeonResultArguments;
        if (args == null)
        {
            return;
        }

        var seconds = args.Second;
        if (seconds <= GameUtils.FubenStar3Time*60)
        {
            DataModel.Start = 3;
        }
        else if (seconds <= GameUtils.FubenStar2Time*60)
        {
            DataModel.Start = 2;
        }
        else
        {
            DataModel.Start = 1;
        }
        DataModel.Type = 0;
        DataModel.FubenId = args.FubenId;
        DataModel.FinishTime = string.Format(GameUtils.GetDictionaryText(210404), seconds/60, seconds%60);
        mDrawId = args.DrawId;
        mDrawIndex = args.DrawIndex;

        var itemBase = args.ItemBase;
        if (itemBase == null || mDrawIndex < 0 || mDrawIndex >= DataModel.AwardItems.Count)
        {
            return;
        }
        DataModel.AwardItems[mDrawIndex].ItemId = itemBase.ItemId;
        DataModel.AwardItems[mDrawIndex].Count = itemBase.Count;
        DataModel.AwardItems[mDrawIndex].Exdata.InstallData(itemBase.Exdata);
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

    public FrameState State { get; set; }
}