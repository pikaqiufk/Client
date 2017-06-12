#region using

using System.Collections.Generic;
using System.ComponentModel;
using ClientDataModel;
using DataTable;
using Shared;

#endregion

public class ElfInfoController : IControllerBase
{
    public ElfInfoController()
    {
        CleanUp();
    }

    public ElfInfoDataModel DataModel;

    public void CleanUp()
    {
        DataModel = new ElfInfoDataModel();
    }

    public void RefreshData(UIInitArguments data)
    {
        var args = data as ElfInfoArguments;
        if (args.DataModel == null)
        {
            if (args.ItemId == -1)
            {
                return;
            }
            var item = new ElfItemDataModel();
            item.Exdata.InstallData(new List<int> {1});
            item.ItemId = args.ItemId;
            DataModel.ItemData = item;
            DataModel.IsShowSimple = 1;
        }
        else
        {
            DataModel.ItemData = args.DataModel;
            DataModel.IsShowSimple = 0;
        }

        var strDic230025 = GameUtils.GetDictionaryText(230025);
        var strDic230033 = GameUtils.GetDictionaryText(230033);
        DataModel.ShowBtn = args.ShowButton;
        var fightAttr = new Dictionary<int, int>();
        var tbItem = Table.GetItemBase(DataModel.ItemData.ItemId);
        var tbElf = Table.GetElf(tbItem.Exdata[0]);
        var tbAttrRef = Table.GetAttrRef(tbElf.Id);
        var level = DataModel.ItemData.Exdata.Level;
        for (var i = 0; i < 6; i++)
        {
            var id = tbElf.ElfInitProp[i];
            var value = tbElf.ElfProp[i];
            DataModel.BaseAttr[i].Reset();
            if (id != -1)
            {
                var valuelevel = tbElf.GrowAddValue[i];
                value += valuelevel*(level - 1);

                GameUtils.SetAttributeBase(DataModel.BaseAttr, i, id, value);
                //value = GameUtils.EquipAttrValueRef(id, value);
                fightAttr.modifyValue(id, value);
            }
            else
            {
                DataModel.BaseAttr[i].Reset();
            }
        }

        for (var i = 0; i < 6; i++)
        {
            var attr = DataModel.BaseAttr[i];
            var attrType = attr.Type;
            if (attrType != -1)
            {
                var str = "";
                var attrName = GameUtils.AttributeName(attrType);
                var attrValue = GameUtils.AttributeValue(attrType, attr.Value);

                if (attr.ValueEx == 0)
                {
                    str = string.Format(strDic230025, attrName, attrValue);
                }
                else
                {
                    var attrValueEx = GameUtils.AttributeValue(attrType, attr.ValueEx);
                    str = string.Format(strDic230033, attrName, attrValue, attrValueEx);
                }
                DataModel.BaseAttrStr[i] = str;
            }
            else
            {
                DataModel.BaseAttrStr[i] = "";
            }
        }

        if (DataModel.IsShowSimple != 1)
        {
            for (var i = 0; i < 6; i++)
            {
                var id = DataModel.ItemData.Exdata[i + 2];
                var value = DataModel.ItemData.Exdata[i + 8];
                if (id != -1 && value > 0)
                {
                    GameUtils.SetAttributeBase(DataModel.InnateAttr, i, id, value);

                    //value = GameUtils.EquipAttrValueRef(id, value);
                    fightAttr.modifyValue(id, value);
                }
                else
                {
                    DataModel.InnateAttr[i].Reset();
                }
            }
            for (var i = 0; i < 6; i++)
            {
                var attr = DataModel.InnateAttr[i];
                var attrType = attr.Type;
                if (attrType != -1)
                {
                    var str = "";
                    var attrName = GameUtils.AttributeName(attrType);
                    var attrValue = GameUtils.AttributeValue(attrType, attr.Value);

                    if (attr.ValueEx == 0)
                    {
                        str = string.Format(strDic230025, attrName, attrValue);
                    }
                    else
                    {
                        var attrValueEx = GameUtils.AttributeValue(attrType, attr.ValueEx);
                        str = string.Format(strDic230033, attrName, attrValue, attrValueEx);
                    }
                    DataModel.InnateAttrStr[i] = str;
                }
                else
                {
                    DataModel.InnateAttrStr[i] = "";
                }
            }
            DataModel.FightPoint = PlayerDataManager.Instance.GetElfAttrFightPoint(fightAttr, args.CharLevel, -2);
        }

        var tbLevel = Table.GetLevelData(DataModel.ItemData.Exdata.Level);

        DataModel.SellCount = tbElf.ResolveCoef[0]*tbLevel.ElfResolveValue/100 + tbElf.ResolveCoef[1];

        var elfController = UIManager.Instance.GetController(UIConfig.ElfUI);
        for (var i = 0; i < DataModel.SingleGroups.Count; i++)
        {
            var groupId = tbElf.BelongGroup[i];
            var info = DataModel.SingleGroups[i];
            if (groupId != -1)
            {
                var tbElfGroup = Table.GetElfGroup(groupId);
                var param = new object[]
                {
                    info,
                    tbElfGroup,
                    DataModel.ItemData.Index,
                    true
                };
                elfController.CallFromOtherClass("SetGroupAttr", param);
            }
            else
            {
                info.Reset();
            }
        }
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
}