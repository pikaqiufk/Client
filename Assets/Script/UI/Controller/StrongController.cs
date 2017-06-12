#region using

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ClientDataModel;
using DataTable;
using EventSystem;

#endregion

public class StrongController : IControllerBase
{
    public StrongController()
    {
        EventDispatcher.Instance.AddEventListener(StrongCellClickedEvent.EVENT_TYPE, OnItemClick);
        CleanUp();
    }

    public StrongDataModel DataModel;
    private bool FirstRun = true;
    private readonly Dictionary<int, int> IdToSort = new Dictionary<int, int>();
    public int SelectIndex = -1;

    public enum eStrongType
    {
        Vip = 1, //VIP等级
        EquipEnhance = 2, //装备强化
        EquipRefine = 3, //装备精炼
        EquipLevel = 4, //装备等阶
        ElfLevel = 5, //灵兽总等级
        WingAdvance = 6, //翅膀进阶阶数
        WingTrain = 7, //翅膀培养重数
        SkillLevel = 8, //技能等级
        TalentUse = 9, //天赋使用点
        MonsterBounty = 10, //怪物悬赏数量
        HandbookCount = 11, //图鉴收集数量
        RankLevel = 12, //军衔等级
        StatueCount = 13, //守护神像数量
        SailingQuality = 14, //航海船饰品质
        Count = 15
    }

    private void FirstShowSort(List<StrongItemDataModel> lists)
    {
        lists.Sort((a, b) =>
        {
            if (a.IsOpen < b.IsOpen)
            {
                return 1;
            }
            if (a.IsOpen == b.IsOpen)
            {
                if (a.State < b.State)
                {
                    return -1;
                }
                if (a.State == b.State)
                {
                    if (a.Sort < b.Sort)
                    {
                        return -1;
                    }
                    return 1;
                }
                return 1;
            }
            return -1;
        });
    }

    private List<int> GetCount(eStrongType type, List<int> param2)
    {
        var result = new List<int> {0, 0, 0};
        var resultCount = result.Count;
        switch (type)
        {
            case eStrongType.Vip:
            {
                var level = PlayerDataManager.Instance.GetRes((int) eResourcesType.VipLevel);
                result[0] = level;
                result[1] = level;
                result[2] = level;
            }
                break;
            case eStrongType.EquipEnhance:
            {
                var totalCount = 0;
                PlayerDataManager.Instance.ForeachEquip(item =>
                {
                    var exchance = item.Exdata.Enchance;
                    if (exchance >= 0)
                    {
                        totalCount += exchance;
                    }
                });
                result[0] = totalCount;
                result[1] = totalCount;
                result[2] = totalCount;
            }
                break;
            case eStrongType.EquipRefine:
            {
                float totalCount = 0;
                var count = 0;
                PlayerDataManager.Instance.ForeachEquip(item =>
                {
                    if (item.ItemId == -1)
                    {
                        return;
                    }
                    var tbEquipBase = Table.GetEquipBase(item.ItemId);
                    if (tbEquipBase == null)
                    {
                        return;
                    }
                    var append = (float) item.Exdata.Append/tbEquipBase.AddAttrMaxValue*100;
                    append = append > 100 ? 100 : append;
                    count++;
                    totalCount += append;
                });
                var avg = 0;
                if (count != 0)
                {
                    avg = (int) totalCount/count;
                }
                result[0] = avg;
                result[1] = avg;
                result[2] = avg;
            }
                break;
            case eStrongType.EquipLevel:
            {
                var levelCount = 0;
                PlayerDataManager.Instance.ForeachEquip(item =>
                {
                    if (item.ItemId == -1)
                    {
                        return;
                    }
                    var tbEquipBase = Table.GetEquipBase(item.ItemId);
                    if (tbEquipBase == null)
                    {
                        return;
                    }
                    levelCount += tbEquipBase.Ladder;
                });
                result[0] = levelCount;
                result[1] = levelCount;
                result[2] = levelCount;
            }
                break;
            case eStrongType.ElfLevel:
            {
                var elfController = UIManager.Instance.GetController(UIConfig.ElfUI);
                var elfData = elfController.GetDataModel("") as ElfDataModel;
                var totalLevel = 0;
                foreach (var item in elfData.ElfList)
                {
                    if (item.ItemId > 0)
                    {
                        totalLevel += item.Exdata.Level;
                    }
                }
                result[0] = totalLevel;
                result[1] = totalLevel;
                result[2] = totalLevel;
            }
                break;
            case eStrongType.WingAdvance:
            {
                var level = PlayerDataManager.Instance.GetExData(eExdataDefine.e308);
                result[0] = level;
                result[1] = level;
                result[2] = level;
            }
                break;
            case eStrongType.WingTrain:
            {
                var trainCount = 0;
                var wingController = UIManager.Instance.GetController(UIConfig.WingUI);
                var wingData = wingController.GetDataModel("") as WingDataModel;
                var extraData = wingData.ItemData.ExtraData;
                var count = System.Math.Min(extraData.Count, (int)eWingExDefine.eGrowMax + 1);
                for (var i = 0; i < count; i++)
                {
                    if (i%2 == 1)
                    {
                        var tbTrain = Table.GetWingTrain(extraData[i]);
                        trainCount += tbTrain.TrainCount;
                    }
                }
                result[0] = trainCount;
                result[1] = trainCount;
                result[2] = trainCount;
            }
                break;
            case eStrongType.SkillLevel:
            {
                var skillCount = 0;
                var skillData = PlayerDataManager.Instance.PlayerDataModel.SkillData.OtherSkills;
                foreach (var item in skillData)
                {
                    skillCount += item.SkillLv;
                }
                result[0] = skillCount;
                result[1] = skillCount;
                result[2] = skillCount;
            }
                break;
            case eStrongType.TalentUse:
            {
                var count = GameUtils.GetAllSkillTalentCount();
                result[0] = count;
                result[1] = count;
                result[2] = count;
            }
                break;
            case eStrongType.MonsterBounty:
            {
                var count = PlayerDataManager.Instance.TotalBountyCount;
                result[0] = count;
                result[1] = count;
                result[2] = count;
            }
                break;
            case eStrongType.HandbookCount:
            {
                var count = PlayerDataManager.Instance.TotalGroupCount;
                result[0] = count;
                result[1] = count;
                result[2] = count;
            }
                break;
            case eStrongType.RankLevel:
            {
                var count = PlayerDataManager.Instance.GetExData(eExdataDefine.e250);
                result[0] = count;
                result[1] = count;
                result[2] = count;
            }
                break;
            case eStrongType.StatueCount:
            {
                var statueController = UIManager.Instance.GetController(UIConfig.AreanaUI);
                var statueData = statueController.GetDataModel("Statue") as StatueDataModel;
                var totalLevel = 0;
                foreach (var item in statueData.StatueInfos)
                {
                    if (item.IsOpen)
                    {
                        var tbStatue = Table.GetStatue(item.DataIndex);
                        totalLevel += tbStatue.Level;
                    }
                }
                result[0] = totalLevel;
                result[1] = totalLevel;
                result[2] = totalLevel;
            }
                break;
            case eStrongType.SailingQuality:
            {
                var sailController = UIManager.Instance.GetController(UIConfig.SailingUI);
                var sailData = sailController.GetDataModel("") as SailingDataModel;
                var totalLevel = 0;
                foreach (var item in sailData.ShipEquip.EquipItem)
                {
                    if (item.BaseItemId > 0)
                    {
                        totalLevel += item.Exdata.Level;
                    }
                }
                result[0] = totalLevel;
                result[1] = totalLevel;
                result[2] = totalLevel;
            }
                break;
        }
        return result;
    }

    private void OnItemClick(IEvent ievent)
    {
        var e = ievent as StrongCellClickedEvent;
        var id = DataModel.Lists[e.Index].Id;
        var tbStrongType = Table.GetStrongType(id);
        SelectIndex = e.Index;
        EventDispatcher.Instance.DispatchEvent(new UIEvent_StrongSetGridLookIndex(1, SelectIndex));
        GameUtils.GotoUiTab(tbStrongType.UiId, tbStrongType.Tab);
    }

    private void ShowNotFirstSort(List<StrongItemDataModel> lists)
    {
        lists.Sort((a, b) =>
        {
            if (a.FirstSort < b.FirstSort)
            {
                return -1;
            }
            return 1;
        });
    }

    public void RefreshData(UIInitArguments data)
    {
    }

    public void CleanUp()
    {
        DataModel = new StrongDataModel();
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

    public void OnShow()
    {
        var level = PlayerDataManager.Instance.GetLevel();
        var tbStrongData = Table.GetStrongData(level);
        if (tbStrongData == null)
        {
            return;
        }

        DataModel.SuggestForce = tbStrongData.SujectForce;
        var nowForce = PlayerDataManager.Instance.PlayerDataModel.Attributes.FightValue;
        var barValue = (float) nowForce/DataModel.SuggestForce;
        barValue = ((barValue < 0.3f ? 0.3f : barValue) > 1 ? 1 : barValue);
        DataModel.BarValue = barValue;
        var count = tbStrongData.TypeId.Length;
        var param1List = new List<int> {-1, -1, -1};
        var param2List = new List<int> {-1, -1, -1};
        var lists = new List<StrongItemDataModel>();
        var sortCount = 0;
        for (var i = 0; i < count; i++)
        {
            if (i == (int) eStrongType.Count - 1)
            {
                break;
            }
            var strongItem = new StrongItemDataModel();
            var tbStrongType = Table.GetStrongType(tbStrongData.TypeId[i]);
            if (tbStrongType.Sort == -1)
            {
                continue;
            }
            param1List[0] = tbStrongData.Param[i, 0];
            param1List[1] = tbStrongData.Param[i, 2];
            param1List[2] = tbStrongData.Param[i, 4];
            param2List[0] = tbStrongData.Param[i, 1];
            param2List[1] = tbStrongData.Param[i, 3];
            param2List[2] = tbStrongData.Param[i, 5];
            var varList = GetCount((eStrongType) tbStrongData.TypeId[i], param2List);
            var state = 3;
            for (var j = 0; j < 3; j++)
            {
                if (param1List[j] > varList[j])
                {
                    state = j;
                    break;
                }
            }
            strongItem.State = state;
            strongItem.Id = tbStrongType.Id;
            if (IdToSort.ContainsKey(tbStrongType.Id))
            {
                strongItem.FirstSort = IdToSort[tbStrongType.Id];
            }
            strongItem.Sort = tbStrongType.Sort;
            if (!GameSetting.Instance.IgnoreButtonCondition)
            {
                if (tbStrongType.ConditionId == -1)
                {
                    strongItem.IsOpen = 1;
                }
                else
                {
                    strongItem.IsOpen = PlayerDataManager.Instance.CheckCondition(tbStrongType.ConditionId) == 0 ? 1 : 0;
                }
            }
            else
            {
                strongItem.IsOpen = 1;
            }

            if (strongItem.IsOpen == 0)
            {
                var tbCondition = Table.GetConditionTable(tbStrongType.ConditionId);
                strongItem.OpenStr = GameUtils.GetDictionaryText(tbCondition.FlagTrueDict);
            }

            //if (tbStrongType.Param[1] == -1)
            //{
            if (state == 0)
            {
                strongItem.NowStateStr = string.Format(tbStrongType.ShowStr, varList[state]);
                strongItem.WillStateStr = string.Format(tbStrongType.ShowStr, param1List[state]);
            }
            else if (state >= 2)
            {
                strongItem.NowStateStr = string.Format(tbStrongType.ShowStr, varList[2]);
                strongItem.WillStateStr = string.Format(tbStrongType.ShowStr, param1List[2]);
            }
            else
            {
                strongItem.NowStateStr = string.Format(tbStrongType.ShowStr, varList[state]);
                strongItem.WillStateStr = string.Format(tbStrongType.ShowStr, param1List[state + 1]);
            }
            //}
            //else
            //{
            //    if (state > 2)
            //    {
            //        strongItem.NowStateStr = string.Format(tbStrongType.ShowStr, varList[2], param2List[2]);
            //        strongItem.WillStateStr = string.Format(tbStrongType.ShowStr, param1List[1], param2List[1]);
            //    }
            //    else
            //    {
            //        strongItem.NowStateStr = string.Format(tbStrongType.ShowStr, varList[state], param2List[state]);
            //        strongItem.WillStateStr = string.Format(tbStrongType.ShowStr, param1List[1], param2List[1]);
            //    }

            //}
            lists.Add(strongItem);
        }

        if (FirstRun)
        {
            FirstShowSort(lists);
            var mCount = lists.Count;
            for (var i = 0; i < mCount; i++)
            {
                IdToSort.Add(lists[i].Id, i);
            }
            FirstRun = false;
        }
        else
        {
            ShowNotFirstSort(lists);
        }
        DataModel.Lists = new ObservableCollection<StrongItemDataModel>(lists);

        EventDispatcher.Instance.DispatchEvent(new UIEvent_StrongSetGridLookIndex(0, SelectIndex));
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        return null;
    }

    public FrameState State { get; set; }
}