#region using

using System.Collections.Generic;
using System.ComponentModel;
using ClientDataModel;
using DataTable;
using Shared;

#endregion

public class WingInfoController : IControllerBase
{
    //--------------------------------------------------------base
    public WingInfoController()
    {
        CleanUp();
    }

    public WingInfoDataModel DataModel;

    public int FixAttributeValue(int attrId, int attrValue)
    {
        if (attrId == 21 || attrId == 22)
        {
            return attrValue*100;
        }
        return attrValue;
    }

    public void CleanUp()
    {
        DataModel = new WingInfoDataModel();
    }

    public void RefreshData(UIInitArguments data)
    {
        var arg = data as WingInfogArguments;
        if (arg == null)
        {
            return;
        }
        var itemData = arg.ItemData;
        DataModel.ItemId = itemData.ItemId;
        var tbItem = Table.GetItemBase(itemData.ItemId);
        if (tbItem == null)
        {
            return;
        }
        var dicAttr = new Dictionary<int, int>();
        var attrs = new List<AttributeBaseDataModel>();
        var tbWing = Table.GetWingQuality(itemData.ItemId);
        //基础属性
        for (var i = 0; i != tbWing.AddPropID.Length; ++i)
        {
            var nAttrId = tbWing.AddPropID[i];
            if (nAttrId < 0)
            {
                break;
            }
            var nValue = tbWing.AddPropValue[i];
            if (nValue > 0 && nAttrId != -1)
            {
                dicAttr.modifyValue(nAttrId, nValue);
            }
        }
        //培养属性
        for (var i = 0; i != 5; ++i)
        {
            var tbWingTrain = Table.GetWingTrain(itemData.Exdata[1 + i*2]);
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
        DataModel.FightPoint = PlayerDataManager.Instance.GetAttrFightPoint(dicAttr, arg.CharLevel);
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
        //{0}：+{1}
        var strDic230025 = GameUtils.GetDictionaryText(230025);
        //{0}：{1}-{2}
        var strDic230033 = GameUtils.GetDictionaryText(230033);

        for (var i = 0; i < attrs.Count; i++)
        {
            var attr = attrs[i];
            var attrType = attr.Type;
            var attrName = GameUtils.AttributeName(attrType);
            var attrValue = GameUtils.AttributeValue(attrType, attr.Value);
            var str = "";
            if (attr.ValueEx == 0)
            {
                str = string.Format(strDic230025, attrName, attrValue);
            }
            else
            {
                var attrValueEx = GameUtils.AttributeValue(attrType, attr.ValueEx);
                str = string.Format(strDic230033, attrName, attrValue, attrValueEx);
            }
            DataModel.AttrStr[i] = str;
        }
        for (var i = attrs.Count; i < 15; i++)
        {
            DataModel.AttrStr[i] = "";
        }


        var role = tbItem.OccupationLimit;

        if (role != -1)
        {
            var tbCharacter = Table.GetCharacterBase(role);
            var roleType = PlayerDataManager.Instance.GetRoleId();
            if (tbCharacter != null)
            {
                if (roleType != role)
                {
                    DataModel.ProfessionColor = MColor.red;
                }
                else
                {
                    DataModel.ProfessionColor = MColor.green;
                }
                DataModel.ProfessionLimit = tbCharacter.Name;
            }
        }
        else
        {
            DataModel.ProfessionLimit = GameUtils.GetDictionaryText(220700);
            DataModel.ProfessionColor = MColor.green;
        }

        var tbEquip = Table.GetEquipBase(tbItem.Exdata[0]);
        if (tbEquip == null)
        {
            return;
        }

        var strDic = GameUtils.GetDictionaryText(230004);
        DataModel.PhaseDesc = string.Format(strDic, GameUtils.NumEntoCh(tbEquip.Ladder));
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public void Close()
    {
    }

    public void OnShow()
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

    public FrameState State { get; set; }
}