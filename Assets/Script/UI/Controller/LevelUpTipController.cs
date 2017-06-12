#region using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using ClientDataModel;
using DataTable;
using EventSystem;

#endregion

public class LevelUpTipController : IControllerBase
{
    public LevelUpTipController()
    {
        EventDispatcher.Instance.AddEventListener(UIEvent_SyncLevelUpAttrChange.EVENT_TYPE, LevelupChange);

        CleanUp();
    }

    public LevelUpTipDataModel DataModel;

    private void LevelupChange(IEvent ievent)
    {
        var e = ievent as UIEvent_SyncLevelUpAttrChange;
        var newAttrData = e.AttrData.NewAttr;
        var oldAttrData = e.AttrData.OldAttr;
        var oldLevel = 0;
        var newLevel = 0;
        if (!oldAttrData.TryGetValue((int) eAttributeType.Level, out oldLevel))
        {
            return;
        }
        if (!newAttrData.TryGetValue((int) eAttributeType.Level, out newLevel))
        {
            return;
        }

        var tableId = -1;
        for (var i = LevelUpRecordIdList.Count - 1; i >= 0; i--)
        {
            if (oldLevel < LevelUpRecordIdList[i] && newLevel >= LevelUpRecordIdList[i])
            {
                tableId = LevelUpRecordIdList[i];
                break;
            }
        }
        if (tableId == -1)
        {
            return;
        }

        var tbGetlevel = Table.GetLevelupTips(tableId);
        if (tbGetlevel.IsShow != 1)
        {
            return;
        }

        var length = tbGetlevel.DictTip.Length;
        for (var i = 0; i < length; i++)
        {
            var item = tbGetlevel.DictTip[i];
            if (item != -1)
            {
                if (i == 0)
                {
                    DataModel.Tips[i] = String.Format(GameUtils.GetDictionaryText(item), newLevel);
                }
                else
                {
                    DataModel.Tips[i] = GameUtils.GetDictionaryText(item);
                }
            }
            else
            {
                DataModel.Tips[i] = String.Empty;
            }
        }
        DataModel.Level = newLevel;
        foreach (var item in newAttrData)
        {
            switch (item.Key)
            {
                case (int) eAttributeType.PhyPowerMax:
                {
                    DataModel.NewPhyPowerMax = item.Value;
                }
                    break;
                case (int) eAttributeType.MagPowerMax:
                {
                    DataModel.NewMagPowerMax = item.Value;
                }
                    break;
                case (int) eAttributeType.PhyArmor:
                {
                    DataModel.NewPhyArmor = item.Value;
                }
                    break;
                case (int) eAttributeType.MagArmor:
                {
                    DataModel.NewMagArmor = item.Value;
                }
                    break;
                case (int) eAttributeType.HpMax:
                {
                    DataModel.NewHpMax = item.Value;
                }
                    break;
            }
        }
        foreach (var item in oldAttrData)
        {
            switch (item.Key)
            {
                case (int) eAttributeType.PhyPowerMax:
                {
                    DataModel.OldPhyPowerMax = item.Value;
                }
                    break;
                case (int) eAttributeType.MagPowerMax:
                {
                    DataModel.OldMagPowerMax = item.Value;
                }
                    break;
                case (int) eAttributeType.PhyArmor:
                {
                    DataModel.OldPhyArmor = item.Value;
                }
                    break;
                case (int) eAttributeType.MagArmor:
                {
                    DataModel.OldMagArmor = item.Value;
                }
                    break;
                case (int) eAttributeType.HpMax:
                {
                    DataModel.OldHpMax = item.Value;
                }
                    break;
            }
        }
        EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.LevelUpTip));
    }

    public void CleanUp()
    {
        DataModel = new LevelUpTipDataModel();
        InitLevelUpTable();
    }

    public void RefreshData(UIInitArguments data)
    {
        DataModel.RoleId = PlayerDataManager.Instance.GetRoleId();
        DataModel.ShowBtn = PlayerDataManager.Instance.GetFlag(1001) ? 1 : 0;
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
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        return null;
    }

    public FrameState State { get; set; }

    #region 升级界面提示逻辑

    private readonly List<int> LevelUpRecordIdList = new List<int>(); //升级提示表id
    //public Dictionary<int,int>  LevelUpOldAttr = new Dictionary<int, int>();   //升级前属性值
    //public Dictionary<int, int> LevelUpNewAttr = new Dictionary<int, int>();  //升级后属性值
    //private Coroutine AttrDelayCoroutine = null;   //升级前后的延迟
    //private int CharacterOldLevel = -1;      //记载旧等级，主要考虑第一次同步等级不提示。

    private void InitLevelUpTable()
    {
        Table.ForeachLevelupTips(table =>
        {
            LevelUpRecordIdList.Add(table.Id);
            return true;
        });
    }

    #endregion
}