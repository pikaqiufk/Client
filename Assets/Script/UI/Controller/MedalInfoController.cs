#region using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using ClientDataModel;
using DataTable;

#endregion

public class MedalInfoController : IControllerBase
{
    public MedalInfoController()
    {
        #region 勋章初始化

        mMedalType = new Dictionary<int, string>();
        mMedalType.Add(0, GameUtils.GetDictionaryText(230201));
        mMedalType.Add(1, GameUtils.GetDictionaryText(230202));
        mMedalType.Add(2, GameUtils.GetDictionaryText(230203));
        mMedalType.Add(3, GameUtils.GetDictionaryText(230204));
        mMedalType.Add(4, GameUtils.GetDictionaryText(230205));
        mMedalType.Add(5, GameUtils.GetDictionaryText(230206));
        mMedalType.Add(6, GameUtils.GetDictionaryText(230207));
        mMedalType.Add(7, GameUtils.GetDictionaryText(230208));
        mMedalType.Add(8, GameUtils.GetDictionaryText(230209));
        mMedalType.Add(9, GameUtils.GetDictionaryText(230210));
        mMedalType.Add(10, GameUtils.GetDictionaryText(230211));
        mMedalType.Add(11, GameUtils.GetDictionaryText(230212));
        mMedalType.Add(12, GameUtils.GetDictionaryText(230213));
        mMedalType.Add(13, GameUtils.GetDictionaryText(230214));

        #endregion

        // EventDispatcher.Instance.AddEventListener(UIEvent_SkillFrame_SkillSelect.EVENT_TYPE, OnClicSkillItem);
        CleanUp();
    }

    public Dictionary<int, string> mMedalType;
    public MedalInfoDataModel MedalInfoData { get; set; }

    public bool InitItemInfo()
    {
        var canFind = false;
        var medalId = MedalInfoData.ItemData.BaseItemId;

        MedalInfoData.ItemPropUI.Clear();

        var itemBaseTable = Table.GetItemBase(medalId);
        if (null == itemBaseTable)
        {
            Logger.Error("Cant find ItemBaseTable !!");
        }

        if (itemBaseTable.Sell != -1)
        {
            MedalInfoData.SaleMoney = itemBaseTable.Sell.ToString();
        }
        else
        {
            //不可出售
            MedalInfoData.SaleMoney = GameUtils.GetDictionaryText(270115);
        }

        var varType = -1;
        var varValue = -1;
        var medalTable = Table.GetMedal(itemBaseTable.Exdata[0]);
        MedalInfoData.MedalId = itemBaseTable.Exdata[0];
        var medalTablePropValueLength0 = medalTable.PropValue.Length;
        for (var i = 0; i < medalTablePropValueLength0; i++)
        {
            varType = medalTable.AddPropID[i];
            varValue = medalTable.PropValue[i];
            if (varValue != -1)
            {
                var MAttrUI = new MedalItemAttrDataModal();
                var tbProp = Table.GetSkillUpgrading(medalTable.PropValue[i]);
                MAttrUI.AttrName = ExpressionHelper.AttrName[varType];
                var value = tbProp.GetSkillUpgradingValue(MedalInfoData.ItemData.Exdata.Level);
                MAttrUI.AttrValue = GameUtils.AttributeValue(varType, value);
                MedalInfoData.ItemPropUI.Add(MAttrUI);
            }
        }

        var ss = "";
        MedalInfoData.ItemData.BaseItemId = itemBaseTable.Exdata[0];
        mMedalType.TryGetValue(medalTable.MedalType, out ss);
        MedalInfoData.MedalType = ss;
        var UpgradingTable = Table.GetSkillUpgrading(medalTable.LevelUpExp);
        MedalInfoData.MaxExp = UpgradingTable.GetSkillUpgradingValue(MedalInfoData.ItemData.Exdata.Level);
        MedalInfoData.TotalExp = MedalInfoData.ItemData.Exdata.Exp + "/" + MedalInfoData.MaxExp;
        canFind = true;
        if (!canFind)
        {
            Logger.Error("Cant find ItemBaseTable Param !!");
            return false;
        }
        return true;
    }

    public void CleanUp()
    {
        MedalInfoData = new MedalInfoDataModel();
    }

    public void RefreshData(UIInitArguments data)
    {
        var args = data as MedalInfoArguments;
        if (args != null)
        {
            MedalInfoData = args.MedalInfoData;
        }
        InitItemInfo();
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return MedalInfoData;
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

    public FrameState State { get; set; }
}