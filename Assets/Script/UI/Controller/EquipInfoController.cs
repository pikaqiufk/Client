#region using

using System;
using System.ComponentModel;
using System.Linq;
using ClientDataModel;
using DataTable;

#endregion

public class EquipInfoController : IControllerBase
{
    public EquipInfoController()
    {
        CleanUp();
    }

    public EquipInfoDataModel DataModel;
    public bool mIsInit = true;
    public string StrDic230004;
    public string StrDic230006;
    public string StrDic230025;
    public string StrDic230032;
    public string StrDic230033;
    public string StrDic230034;

    public void InitStr()
    {
        StrDic230004 = GameUtils.GetDictionaryText(230004);
        StrDic230006 = GameUtils.GetDictionaryText(230006);
        StrDic230034 = GameUtils.GetDictionaryText(230034);
        StrDic230033 = GameUtils.GetDictionaryText(230033);
        StrDic230032 = GameUtils.GetDictionaryText(230032);
        StrDic230025 = GameUtils.GetDictionaryText(230025);
        mIsInit = false;
    }

    public void CleanUp()
    {
        DataModel = new EquipInfoDataModel();
        mIsInit = true;
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        throw new NotImplementedException(name);
    }

    public void OnShow()
    {
    }

    public void Close()
    {
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        var args = data as EquipInfoArguments;
        if (args == null)
        {
            return;
        }

        var type = args.Type;

        if (mIsInit)
        {
            InitStr();
        }
        var itemId = args.ItemId;
        //int itemId = 304001;
        var tbItem = Table.GetItemBase(itemId);
        if (tbItem == null)
        {
            return;
        }
        DataModel.BuffId = -1;
        var equipId = tbItem.Exdata[0];
        var tbEquip = Table.GetEquipBase(equipId);
        if (tbEquip != null)
        {
            DataModel.BuffLevel = tbEquip.AddBuffSkillLevel;

            if (tbEquip.BuffGroupId >= 0)
            {
                var tbBuffGroup = Table.GetBuffGroup(tbEquip.BuffGroupId);
                if (tbBuffGroup != null && tbBuffGroup.BuffID.Count == 1)
                {
                    DataModel.BuffId = tbBuffGroup.BuffID[0];
                }
                else
                {
                    DataModel.BuffId = 8999;
                }
            }
        }
        
        if (tbEquip.FIghtNumDesc == -1)
        {
            DataModel.FightNum = "?????";
        }
        else
        {
            DataModel.FightNum = tbEquip.FIghtNumDesc.ToString();
        }
       
        if (tbEquip == null)
        {
            return;
        }
        DataModel.ItemId = itemId;
        DataModel.EquipId = equipId;
        DataModel.EnchanceLevel = 0;
        DataModel.CanUseLevel = PlayerDataManager.Instance.GetLevel() < tbItem.UseLevel ? 1 : 0;
        //职业符合不？
        if (tbEquip.Occupation != -1)
        {
            DataModel.CanRole = PlayerDataManager.Instance.GetRoleId() == tbEquip.Occupation ? 0 : 1;
        }
        var strDic = GameUtils.GetDictionaryText(230004);

        DataModel.PhaseDesc = string.Format(strDic, GameUtils.NumEntoCh(tbEquip.Ladder));

        strDic = GameUtils.GetDictionaryText(230006);

        for (var i = 0; i != 2; ++i)
        {
            var attrId = tbEquip.NeedAttrId[i];
            if (attrId != -1)
            {
                var attrValue = tbEquip.NeedAttrValue[i];
                var selfAttrValue = PlayerDataManager.Instance.GetAttribute(attrId);
                var needStr = string.Format(strDic, GameUtils.AttributeName(attrId), selfAttrValue, attrValue);

                if (selfAttrValue < attrValue)
                {
                    DataModel.NeedAttr[i] = string.Format("[FF0000]{0}[-]", needStr);
                }
                else
                {
                    DataModel.NeedAttr[i] = string.Format("[00FF00]{0}[-]", needStr);
                }
            }
            else
            {
                DataModel.NeedAttr[i] = "";
            }
        }

        var enchanceLevel = DataModel.EnchanceLevel;

        for (var i = 0; i < 4; i++)
        {
            var nAttrId = tbEquip.BaseAttr[i];
            if (nAttrId != -1)
            {
                var baseValue = tbEquip.BaseValue[i];
                var changeValue = 0;
                if (enchanceLevel > 0)
                {
                    changeValue = GameUtils.GetBaseAttr(tbEquip, enchanceLevel, i, nAttrId) - baseValue;
                }
                GameUtils.SetAttribute(DataModel.BaseAttr, i, nAttrId, baseValue, changeValue);
            }
            else
            {
                DataModel.BaseAttr[i].Reset();
            }
        }
        for (var i = 0; i < 4; i++)
        {
            var attrData = DataModel.BaseAttr[i];
            var nAttrId = attrData.Type;
            if (nAttrId != -1)
            {
                var attrName = GameUtils.AttributeName(nAttrId);
                var attrValue = GameUtils.AttributeValue(nAttrId, attrData.Value);

                if (attrData.ValueEx != 0)
                {
                    if (attrData.Change != 0 || attrData.ChangeEx != 0)
                    {
                        var attrValueEx = GameUtils.AttributeValue(nAttrId, attrData.ValueEx);
                        var attrChange = GameUtils.AttributeValue(nAttrId, attrData.Change);
                        var attrChangeEx = GameUtils.AttributeValue(nAttrId, attrData.ChangeEx);
                        //rDic = "{0}+:{1}[00ff00](+{2})[-]-{3}[00ff00](+{4})[-]";
                        strDic = StrDic230034;
                        DataModel.BaseAttrStr[i] = string.Format(strDic, attrName, attrValue, attrChange, attrValueEx,
                            attrChangeEx);
                    }
                    else
                    {
                        var attrValueEx = GameUtils.AttributeValue(nAttrId, attrData.ValueEx);
                        //strDic = "{0}+:{1}-{2}";
                        strDic = StrDic230033;
                        DataModel.BaseAttrStr[i] = string.Format(strDic, attrName, attrValue, attrValueEx);
                    }
                }
                else
                {
                    if (attrData.Change != 0 || attrData.ChangeEx != 0)
                    {
                        var attrChange = GameUtils.AttributeValue(nAttrId, attrData.Change);
                        //strDic = "{0}+:{1}[00ff00](+{2})[-]";
                        strDic = StrDic230032;
                        DataModel.BaseAttrStr[i] = string.Format(strDic, attrName, attrValue, attrChange);
                    }
                    else
                    {
                        //strDic = "{0}+:{1}";
                        strDic = StrDic230025;
                        DataModel.BaseAttrStr[i] = string.Format(strDic, attrName, attrValue);
                    }
                }
            }
            else
            {
                DataModel.BaseAttrStr[i] = "";
            }
        }

        strDic = StrDic230025;
        //strDic = "{0}+:{1}";
        for (var i = 0; i != 2; ++i)
        {
            var nAttrId = tbEquip.BaseFixedAttrId[i];
            if (nAttrId != -1)
            {
                var attrName = GameUtils.AttributeName(nAttrId);
                var attrValue = GameUtils.AttributeValue(nAttrId, tbEquip.BaseFixedAttrValue[i]);
                DataModel.AddAttrStr[i] = string.Format(strDic, attrName, attrValue);
            }
            else
            {
                DataModel.AddAttrStr[i] = "";
            }
        }

        //灵魂、卓越、字符串显示
        DataModel.StrExcellent = "";
        DataModel.StrSoul = "";
        var min = 0;
        var minbool = false;
        var max = 0;

        if (type == 1)
        {
            //取决于材料
            DataModel.StrAppend = GameUtils.GetDictionaryText(300836);
        }
        else
        {
            //随机数值
            if (tbEquip.JingLianDescId == -1)
            {
                DataModel.StrAppend = GameUtils.GetDictionaryText(300837);
            }
            else
            {
                DataModel.StrAppend = tbEquip.JingLianDescId.ToString();
            }
            
        }


        if (type == 1)
        {
            //取决于材料装备
            DataModel.StrExcellent = GameUtils.GetDictionaryText(300838);
        }
        else
        {
            if (tbEquip.ExcellentAttrCount != -1)
            {
                var tbEquipRalate = Table.GetEquipRelate(tbEquip.ExcellentAttrCount);
                if (tbEquipRalate == null)
                {
                    return;
                }
                for (var i = 0; i < tbEquipRalate.AttrCount.Length; i++)
                {
                    if (tbEquipRalate.AttrCount[i] > 0)
                    {
                        max = i;
                        if (!minbool)
                        {
                            minbool = true;
                            min = i;
                        }
                    }
                }
                if (min != 0)
                {
                    if (tbEquip.ZhuoYueDescId == -1)
                    {
                        if (min == max)
                        {
                            DataModel.StrExcellent = min + GameUtils.GetDictionaryText(300839); //"条随机属性";
                        }
                        else
                        {
                            DataModel.StrExcellent = string.Format("{0}-{1}" + GameUtils.GetDictionaryText(300839), min, max);
                        }
                        DataModel.ExcellentHeight = 16;
                    }
                    else
                    {
                        DataModel.StrExcellent = GameUtils.GetDictionaryText(tbEquip.ZhuoYueDescId); //"条随机属性";
                        string[] subts = DataModel.StrExcellent.Split('\n');
                        DataModel.ExcellentHeight = (subts.Length) * 16;
                    }
                }
            }
        }


        if (type == 1)
        {
            DataModel.StrSoul = GameUtils.GetDictionaryText(300840);
        }
        else
        {
            if (tbEquip.RandomAttrCount != -1)
            {
                var tbEquipRalate = Table.GetEquipRelate(tbEquip.RandomAttrCount);
                if (tbEquipRalate == null)
                {
                    return;
                }
                min = 0;
                minbool = false;
                max = 0;
                for (var i = 0; i < tbEquipRalate.AttrCount.Length; i++)
                {
                    if (tbEquipRalate.AttrCount[i] > 0)
                    {
                        max = i;
                        if (!minbool)
                        {
                            minbool = true;
                            min = i;
                        }
                    }
                }
                if (min != 0)
                {
                    if (tbEquip.LingHunDescId == -1)
                    {
                        if (min == max)
                        {
                            DataModel.StrSoul = min + GameUtils.GetDictionaryText(300839); //"条随机属性";
                        }
                        else
                        {
                            DataModel.StrSoul = string.Format("{0}-{1}" + GameUtils.GetDictionaryText(300839), min, max);
                        }
                        DataModel.SouleHeight = 16;
                    }
                    else
                    {
                        DataModel.StrSoul = GameUtils.GetDictionaryText(tbEquip.LingHunDescId); //"条随机属性";
                        string[] subts = DataModel.StrExcellent.Split('\n');
                        DataModel.SouleHeight = (subts.Length) * 16;
                    }
                }
            }
        }


        //套装相关
        for (var i = 0; i < 10; i++)
        {
            DataModel.TieCount[i] = 0;
        }
        DataModel.TieId = tbEquip.TieId;
        var nNowTieCount = 0;
        for (var i = 0; i != 4; ++i)
        {
            DataModel.TieAttrCount[i] = 0;
        }

        if (tbEquip.TieId == -1)
        {
            return;
        }
        var tbTie = Table.GetEquipTie(tbEquip.TieId);
        if (tbTie == null)
        {
            return;
        }

        PlayerDataManager.Instance.ForeachEquip(item =>
        {
            var ItemId = item.ItemId;
            if (ItemId == -1)
            {
                return;
            }
            var tbTieItem = Table.GetItemBase(ItemId);
            if (tbTieItem == null)
            {
                return;
            }
            var tbTieEquip = Table.GetEquipBase(tbTieItem.Exdata[0]);
            if (tbTieEquip == null)
            {
                return;
            }
            if (tbEquip.TieId == tbTieEquip.TieId)
            {
                DataModel.TieCount[tbTieEquip.TieIndex] = 1;
                nNowTieCount++;
            }
        });

        DataModel.TieNowCount = nNowTieCount;
        for (var i = 0; i != 4; ++i)
        {
            if (nNowTieCount >= tbTie.NeedCount[i])
            {
                DataModel.TieAttrCount[i] = 1;
            }
        }
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public FrameState State { get; set; }
}