#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using ClientDataModel;
using ClientService;
using DataTable;
using EventSystem;
using GameUI;
using ScorpionNetLib;
using UnityEngine;

#endregion

public class AttributeController : IControllerBase
{
    public AttributeController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(AttributePointOperate.EVENT_TYPE, OnBtnClickOperate);
        EventDispatcher.Instance.AddEventListener(FlagUpdateEvent.EVENT_TYPE, OnUpdateFlagData);
        EventDispatcher.Instance.AddEventListener(FlagInitEvent.EVENT_TYPE, OnFlagInit);
        EventDispatcher.Instance.AddEventListener(AttributeDistributionChange.EVENT_TYPE, OnAttributeDistributionChange);
        EventDispatcher.Instance.AddEventListener(ExDataInitEvent.EVENT_TYPE, OnExDataInit);
        EventDispatcher.Instance.AddEventListener(AttributeDistributionChange.EVENT_TYPE, OnAttrExDataUpData);

        EventDispatcher.Instance.AddEventListener(FruitExdataUpdate.EVENT_TYPE, ExdataUpdate);
        EventDispatcher.Instance.AddEventListener(Event_LevelUp.EVENT_TYPE, LevelUpUpdate);

        EventDispatcher.Instance.AddEventListener(LevelUpInitEvent.EVENT_TYPE, LevelUpUpdate);
        EventDispatcher.Instance.AddEventListener(AttrUIReflesh_Event.EVENT_TYPE, AttrChangeUpdate);

        EventDispatcher.Instance.AddEventListener(OpenAdvancedProperty_Event.EVENT_TYPE,OpenAdvancedProperty);
        EventDispatcher.Instance.AddEventListener(CloseAdvancedProperty_Event.EVENT_TYPE, CloseAdvancedProperty);

        mAutoPiont = new AutoPiont();
    }

    private readonly Dictionary<int, int> AttrIdToIndex = new Dictionary<int, int>
    {
        {1, 0},
        {2, 1},
        {3, 2},
        {4, 3},
        {5, 4},
        {6, 5},
        {7, 6},
        {8, 7},
        {10, 8},
        {11, 9},
        {13, 10},
        {14, 11},
        {19, 12},
        {20, 13}
    };

    private readonly string DoubleStr = "{0}[4BE127]+{1}[-]-{2}[4BE127]+{3}[-]";
    private readonly string DoubleStr2 = "{0}-{1}";
    public AutoPiont mAutoPiont;
    public bool mIsResetPoint;
    public FrameState mState;
    private readonly string OneStr = "{0}[4BE127]+{1}[-]";
    private readonly string OneStr2 = "{0}";
    public Coroutine ButtonPress { get; set; }
    public AttributeUIDataModel DataModel { get; set; }

    private void AttrChangeUpdate(IEvent ievent)
    {
        var e = ievent as AttrUIReflesh_Event;
        if (mState == FrameState.Open)
        {
            if (AttrIdToIndex.ContainsKey(e.Type))
            {
                SetAttributeStr(AttrIdToIndex[e.Type], 0);
            }
        }
    }

    public void OpenAdvancedProperty(IEvent ievent)
    {
        DataModel.OpenAdvancedProperty = true;
    }
    public void CloseAdvancedProperty(IEvent ievent)
    {
        DataModel.OpenAdvancedProperty = false;
    }

    public IEnumerator AttributeAutoAddCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var ret = 0;
            ret = DataModel.IsAutoAdd ? 0 : 1;
            var msg = NetManager.Instance.SetAttributeAutoAdd(ret);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    DataModel.IsAutoAdd = !DataModel.IsAutoAdd;
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error("AttributeAutoAdd..................." + msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("AttributeAutoAdd..................." + msg.State);
            }
        }
    }

    public IEnumerator ButtonAddOnPress(int index)
    {
        var pressCd = 0.25f;
        while (true)
        {
            yield return new WaitForSeconds(pressCd);
            if (OnClickPointAdd(index) == 0)
            {
                NetManager.Instance.StopCoroutine(ButtonPress);
                ButtonPress = null;
                yield break;
            }
            if (pressCd > 0.01)
            {
                pressCd = pressCd*0.8f;
            }
        }
        yield break;
    }

    public IEnumerator ButtonResOnPress(int index)
    {
        var pressCd = 0.25f;
        while (true)
        {
            yield return new WaitForSeconds(pressCd);
            if (OnClickPointDel(index) == 0)
            {
                NetManager.Instance.StopCoroutine(ButtonPress);
                ButtonPress = null;
                yield break;
            }
            if (pressCd > 0.01)
            {
                pressCd = pressCd*0.8f;
            }
        }
        yield break;
    }

    public void ConfirmAttrPoint()
    {
        NetManager.Instance.StartCoroutine(ConfirmCoroutine());
    }

    public IEnumerator ConfirmCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.DistributionAttrPoint(DataModel.StrengthAdd, DataModel.AgilityAdd,
                DataModel.IntelligenceAdd, DataModel.EnduranceAdd);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    for (var i = 0; i < 4; ++i)
                    {
                        PlayerDataManager.Instance.SetExData(5 + i,
                            PlayerDataManager.Instance.GetExData(5 + i) + GetAttributeAdd(i));
                    }
                    PlayerDataManager.Instance.SetExData(52,
                        PlayerDataManager.Instance.GetExData(52) -
                        (DataModel.StrengthAdd + DataModel.AgilityAdd + DataModel.IntelligenceAdd +
                         DataModel.EnduranceAdd));
                    PlayerAttr.Instance.SetAttrChange(PlayerAttr.PlayerAttrChange.AddPoint);
                    for (var i = 0; i < 14; i++)
                    {
                        SetAttributeAdd(i, 0);
                    }
                    DataModel.Original = DataModel.Distribution;
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_AttrPointNotEnough)
                {
                    if (State == FrameState.Open)
                    {
                        GameUtils.ShowNetErrorHint(msg.ErrorCode);
                    }
                    for (var i = 0; i < 14; i++)
                    {
                        SetAttributeAdd(i, 0);
                    }
                    DataModel.Original = 0;

                    //重新请求52号扩展数据，取新的可分配点数
                    PlayerDataManager.Instance.ApplyExtData(52);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error("DistributionAttrPoint..................." + msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("DistributionAttrPoint..................." + msg.State);
            }
        }
    }

    public void ExdataUpdate()
    {
        DataModel.StrengthFruit = PlayerDataManager.Instance.GetExData(eExdataDefine.e9);
        DataModel.AgilityFruit = PlayerDataManager.Instance.GetExData(eExdataDefine.e10);
        DataModel.IntelligenceFruit = PlayerDataManager.Instance.GetExData(eExdataDefine.e11);
        DataModel.EnduranceFruit = PlayerDataManager.Instance.GetExData(eExdataDefine.e12);
    }

    public void ExdataUpdate(IEvent ievent)
    {
        ExdataUpdate();
        ProgressUpdate();
    }

    public void OnAttrExDataUpData(IEvent ievent)
    {
        var playerData = PlayerDataManager.Instance;
        if (playerData != null)
        {
            RefresNoticeData(playerData.GetExData(eExdataDefine.e52) > 0);
        }
    }

    public int GetAttributeAdd(int index)
    {
        var ret = 0;
        switch (index)
        {
            case 0:
            {
                ret = DataModel.StrengthAdd;
            }
                break;
            case 1:
            {
                ret = DataModel.AgilityAdd;
            }
                break;
            case 2:
            {
                ret = DataModel.IntelligenceAdd;
            }
                break;
            case 3:
            {
                ret = DataModel.EnduranceAdd;
            }
                break;
            case 4:
            {
                ret = DataModel.PhyPowerMinAdd;
            }
                break;
            case 5:
            {
                ret = DataModel.PhyPowerMaxAdd;
            }
                break;
            case 6:
            {
                ret = DataModel.MagPowerMinAdd;
            }
                break;
            case 7:
            {
                ret = DataModel.MagPowerMaxAdd;
            }
                break;
            case 8:
            {
                ret = DataModel.PhyArmorAdd;
            }
                break;
            case 9:
            {
                ret = DataModel.MagArmorAdd;
            }
                break;
            case 10:
            {
                ret = DataModel.HpMaxAdd;
            }
                break;
            case 11:
            {
                ret = DataModel.MpMaxAdd;
            }
                break;
            case 12:
            {
                ret = DataModel.HitAdd;
            }
                break;
            case 13:
            {
                ret = DataModel.DodgeAdd;
            }
                break;
        }
        return ret;
    }

    public int GetPlayerAttribute(int i)
    {
        var playerData = PlayerDataManager.Instance.PlayerDataModel.Attributes;
        switch (i)
        {
            case 0:
            {
                return playerData.Strength;
            }
                break;
            case 1:
            {
                return playerData.Agility;
            }
                break;
            case 2:
            {
                return playerData.Intelligence;
            }
                break;
            case 3:
            {
                return playerData.Endurance;
            }
                break;
        }
        return 0;
    }

    public int GetWillAttrPoint(eAttributeType type)
    {
        return PlayerAttr.Instance.GetTryDataValue(type) - PlayerAttr.Instance.GetDataValue(type);
    }

    public void LevelUpUpdate(IEvent ievent)
    {
        if (PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Resources.Count <= 0)
        {
            Logger.Error("PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Resources.Count<=0");
            return;
        }
        var playerLevel = PlayerDataManager.Instance.GetLevel();
        var table = Table.GetLevelData(playerLevel);
        if (table == null)
        {
            return;
        }
        DataModel.StrengthFruitMax = table.FruitLimit[0];
        DataModel.AgilityFruitMax = table.FruitLimit[1];
        DataModel.IntelligenceFruitmax = table.FruitLimit[2];
        DataModel.EnduranceFruitMax = table.FruitLimit[3];
        ProgressUpdate();
    }

    public void OnAttributeDistributionChange(IEvent ievent)
    {
        var e = ievent as AttributeDistributionChange;
        if (mIsResetPoint)
        {
            return;
        }
        if (DataModel.Original < e.Count)
        {
            DataModel.Distribution += (e.Count - DataModel.Original);
            DataModel.Original += e.Count;

            if (DataModel.IsAutoAdd)
            {
                OnClickBtnRecommond();
                ConfirmAttrPoint();
            }
        }
    }

    public void OnBtnClickOperate(IEvent ievent)
    {
        var e = ievent as AttributePointOperate;
        var type = e.Type;
        var index = e.Index;

        switch (type)
        {
            case 0:
            {
                OnClickPointDel(index);
            }
                break;
            case 1:
            {
                OnClickPointAdd(index);
            }
                break;
            case 2:
            {
                OnClickBtnRecommond();
            }
                break;
            case 3:
            {
                RefreshAttrPoint();
            }
                break;
            case 4:
            {
                ConfirmAttrPoint();
            }
                break;
            case 5:
            {
                ResetAddPoint();
            }
                break;
            case 6:
            {
                OnClickBtnAutoAdd();
            }
                break;
            case 10: //press 减
            {
                ButtonPress = NetManager.Instance.StartCoroutine(ButtonResOnPress(index));
            }
                break;
            case 11: //press + 
            {
                ButtonPress = NetManager.Instance.StartCoroutine(ButtonAddOnPress(index));
            }
                break;
            case 20: //release 减
            {
                if (ButtonPress != null)
                {
                    NetManager.Instance.StopCoroutine(ButtonPress);
                    ButtonPress = null;
                }
            }
                break;
            case 21: //release +
            {
                if (ButtonPress != null)
                {
                    NetManager.Instance.StopCoroutine(ButtonPress);
                    ButtonPress = null;
                }
            }
                break;
            case 500: //
            {
                DataModel.ShowFruit = true;
            }
                break;
            case 600: //
            {
                DataModel.ShowFruit = false;
            }
                break;
        }
    }

    public void OnClickBtnAutoAdd()
    {
        NetManager.Instance.StartCoroutine(AttributeAutoAddCoroutine());
    }

    public void OnClickBtnRecommond()
    {
        ResetAddPoint();

        if (DataModel.Distribution <= 0)
        {
            return;
        }
        if (DataModel.ShowType == -1)
        {
            DataModel.ShowType = PlayerDataManager.Instance.GetRoleId();
        }
        var tbActor = Table.GetActor(DataModel.ShowType);

        for (var i = 0; i < 4; i++)
        {
            var node = mAutoPiont.AutoNodes[i];
            node.Index = i;
            node.Weight = tbActor.AutoAddAttr[i];
            node.Point = GetPlayerAttribute(i);
        }
        var totalCount = DataModel.Distribution;
        mAutoPiont.TotalPoint = totalCount;
        mAutoPiont.DistributionPoint();
        DataModel.Distribution = 0;
        for (var i = 0; i < 4; i++)
        {
            var node = mAutoPiont.AutoNodes[i];
            SetAttributeAdd(node.Index, node.Point - GetPlayerAttribute(node.Index));
        }
        RefreshRelateAttribute();
    }

    public int OnClickPointAdd(int index)
    {
        if (DataModel.Distribution == 0)
        {
            return 0;
        }

        var point = GetAttributeAdd(index);
        point++;
        SetAttributeAdd(index, point);
        SetAttributeStr(index, point);
        RefreshRelateAttribute();
        return DataModel.Distribution--;
    }

    public int OnClickPointDel(int index)
    {
        var point = GetAttributeAdd(index);
        if (point == 0)
        {
            return 0;
        }
        point--;
        SetAttributeAdd(index, point);
        SetAttributeStr(index, point);
        RefreshRelateAttribute();
        DataModel.Distribution++;
        return point;
    }

    public void OnExDataInit(IEvent ievent)
    {
        ExdataUpdate();
        ProgressUpdate();

        var playerData = PlayerDataManager.Instance;
        if (playerData != null)
        {
            RefresNoticeData(playerData.GetExData(52) > 0);
        }
    }

    public void OnFlagInit(IEvent ievent)
    {
        var e = ievent as FlagInitEvent;
        DataModel.IsAutoAdd = PlayerDataManager.Instance.GetFlag(1001);
    }

    public void OnUpdateFlagData(IEvent ievent)
    {
        var e = ievent as FlagUpdateEvent;
        if (e.Index == 1001)
        {
            DataModel.IsAutoAdd = e.Value;
        }
    }

    public void ProgressUpdate()
    {
        DataModel.StrengthFruitProgress = DataModel.StrengthFruitMax == 0
            ? 0
            : (float) DataModel.StrengthFruit/DataModel.StrengthFruitMax;
        DataModel.AgilityFruitProgress = DataModel.AgilityFruitMax == 0
            ? 0
            : (float) DataModel.AgilityFruit/DataModel.AgilityFruitMax;
        DataModel.IntelligenceFruitProgress = DataModel.IntelligenceFruitmax == 0
            ? 0
            : (float) DataModel.IntelligenceFruit/DataModel.IntelligenceFruitmax;
        DataModel.EnduranceFruitProgress = DataModel.EnduranceFruitMax == 0
            ? 0
            : (float) DataModel.EnduranceFruit/DataModel.EnduranceFruitMax;
    }

    public void PushPointAdd()
    {
        PlayerAttr.Instance.mRryAddPoint[0] = DataModel.StrengthAdd;
        PlayerAttr.Instance.mRryAddPoint[1] = DataModel.AgilityAdd;
        PlayerAttr.Instance.mRryAddPoint[2] = DataModel.IntelligenceAdd;
        PlayerAttr.Instance.mRryAddPoint[3] = DataModel.EnduranceAdd;
    }

    public void RefreshAttrPoint()
    {
        var count = 0;
        for (var i = 5; i <= 8; i++)
        {
            count += PlayerDataManager.Instance.GetExData(i);
        }
        if (count == 0)
        {
            var e = new ShowUIHintBoard("");
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }
        //是否确定洗点？
        UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 270217, "",
            () => { NetManager.Instance.StartCoroutine(ResetCoroutine()); });
    }

    public void RefreshRelateAttribute()
    {
        PushPointAdd();

        for (var i = 4; i <= 13; ++i)
        {
            var aId = GameUtils.GetBaseAttrIdElf(i);
            SetAttributeAdd(i, GetWillAttrPoint((eAttributeType) aId));
            SetAttributeStr(i, GetWillAttrPoint((eAttributeType) aId));
        }
        //for (var i = eAttributeType.PhyPowerMin; i <= eAttributeType.MpMax; i++)
        //{
        //    SetAttributeAdd((int)i-1, GetWillAttrPoint(i));
        //}
//         var type = ObjManager.Instance.MyPlayer.GetDataId();
//         int[] attriFix = new int[14];
//         Table.ForeachAttrRef((recoard) =>
//         {
//             if (recoard.CharacterId != type)
//             {
//                 return true;
//             }
//             var point = GetAttributeAdd(recoard.AttrId - 1);
// 
//             var recoardAttrLength0 = recoard.Attr.Length;
//             for (int i = 4; i < recoardAttrLength0; i++)
//             {
//                 var attr = recoard.Attr[i];
//                 if (attr > 0)
//                 {
//                     attriFix[i] += attr * point / 100;
//                 }
//             }
//             return true;
//         });
// 
//         var attriFixLength1 = attriFix.Length;
//         for (int i = 4; i < attriFixLength1; i++)
//         {
//             SetAttributeAdd(i, attriFix[i]);
//         }
    }

    public void ResetAddInfos()
    {
        for (var i = 0; i < 14; i++)
        {
            SetAttributeAdd(i, 0);
            SetAttributeStr(i, 0);
        }
    }

    public void ResetAddPoint()
    {
        var add = 0;
        for (var i = 0; i < 4; i++)
        {
            add += GetAttributeAdd(i);
            SetAttributeAdd(i, 0);
        }
        DataModel.Distribution += add;
        RefreshRelateAttribute();
    }

    public IEnumerator ResetCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            mIsResetPoint = true;
            var msg = NetManager.Instance.RefreshAttrPoint(-1);
            yield return msg.SendAndWaitUntilDone();

            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    ResetAddInfos();
                    var playerData = PlayerDataManager.Instance;
                    for (var i = 5; i <= 8; i++)
                    {
                        playerData.SetExData(i, 0);
                    }
                    PlayerAttr.Instance.SetAttrChange(PlayerAttr.PlayerAttrChange.AddPoint);
                    UpdateDistribution(msg.Response);
                    playerData.SetExData(52, msg.Response);

                    PlatformHelper.UMEvent("TalentRest");
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error("RefreshAttrPoint..................." + msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("RefreshAttrPoint..................." + msg.State);
            }
        }
        mIsResetPoint = false;
    }

    public void SetAttributeAdd(int index, int value)
    {
        switch (index)
        {
            case 0:
            {
                DataModel.StrengthAdd = value;
            }
                break;
            case 1:
            {
                DataModel.AgilityAdd = value;
            }
                break;
            case 2:
            {
                DataModel.IntelligenceAdd = value;
            }
                break;
            case 3:
            {
                DataModel.EnduranceAdd = value;
            }
                break;
            case 4:
            {
                DataModel.PhyPowerMinAdd = value;
            }
                break;
            case 5:
            {
                DataModel.PhyPowerMaxAdd = value;
            }
                break;
            case 6:
            {
                DataModel.MagPowerMinAdd = value;
            }
                break;
            case 7:
            {
                DataModel.MagPowerMaxAdd = value;
            }
                break;
            case 8:
            {
                DataModel.PhyArmorAdd = value;
            }
                break;
            case 9:
            {
                DataModel.MagArmorAdd = value;
            }
                break;
            case 10:
            {
                DataModel.HpMaxAdd = value;
            }
                break;
            case 11:
            {
                DataModel.MpMaxAdd = value;
            }
                break;
            case 12:
            {
                DataModel.HitAdd = value;
            }
                break;
            case 13:
            {
                DataModel.DodgeAdd = value;
            }
                break;
        }
    }

    public void SetAttributeStr(int index, int value)
    {
        var playerData = PlayerDataManager.Instance.PlayerDataModel.Attributes;
        switch (index)
        {
            case 0:
            {
                //DataModel.StrengthAdd = value;
                if (value == 0)
                {
                    DataModel.StrengthStr = String.Format(OneStr2, playerData.Strength);
                }
                else
                {
                    DataModel.StrengthStr = String.Format(OneStr, playerData.Strength, DataModel.StrengthAdd);
                }
            }
                break;
            case 1:
            {
                //DataModel.AgilityAdd = value;
                if (value == 0)
                {
                    DataModel.AgilityStr = String.Format(OneStr2, playerData.Agility);
                }
                else
                {
                    DataModel.AgilityStr = String.Format(OneStr, playerData.Agility, DataModel.AgilityAdd);
                }
            }
                break;
            case 2:
            {
                //DataModel.IntelligenceAdd = value;
                if (value == 0)
                {
                    DataModel.IntelligenceStr = String.Format(OneStr2, playerData.Intelligence);
                }
                else
                {
                    DataModel.IntelligenceStr = String.Format(OneStr, playerData.Intelligence, DataModel.IntelligenceAdd);
                }
            }
                break;
            case 3:
            {
                //DataModel.EnduranceAdd = value;
                if (value == 0)
                {
                    DataModel.EnduranceStr = String.Format(OneStr2, playerData.Endurance);
                }
                else
                {
                    DataModel.EnduranceStr = String.Format(OneStr, playerData.Endurance, DataModel.EnduranceAdd);
                }
            }
                break;
            case 4:
            {
                //DataModel.PhyPowerMinAdd = value;
                if (value == 0)
                {
                    DataModel.PhyPowerStr = String.Format(DoubleStr2, playerData.PhyPowerMin, playerData.PhyPowerMax);
                }
                else
                {
                    DataModel.PhyPowerStr = String.Format(DoubleStr, playerData.PhyPowerMin, DataModel.PhyPowerMinAdd,
                        playerData.PhyPowerMax, DataModel.PhyPowerMaxAdd);
                }
            }
                break;
            case 5:
            {
                //DataModel.PhyPowerMaxAdd = value;
                if (value == 0)
                {
                    DataModel.PhyPowerStr = String.Format(DoubleStr2, playerData.PhyPowerMin, playerData.PhyPowerMax);
                }
                else
                {
                    DataModel.PhyPowerStr = String.Format(DoubleStr, playerData.PhyPowerMin, DataModel.PhyPowerMinAdd,
                        playerData.PhyPowerMax, DataModel.PhyPowerMaxAdd);
                }
            }
                break;
            case 6:
            {
                //DataModel.MagPowerMinAdd = value;
                if (value == 0)
                {
                    DataModel.MagPowerStr = String.Format(DoubleStr2, playerData.MagPowerMin, playerData.MagPowerMax);
                }
                else
                {
                    DataModel.MagPowerStr = String.Format(DoubleStr, playerData.MagPowerMin, DataModel.MagPowerMinAdd,
                        playerData.MagPowerMax, DataModel.MagPowerMaxAdd);
                }
            }
                break;
            case 7:
            {
                // DataModel.MagPowerMaxAdd = value;
                if (value == 0)
                {
                    DataModel.MagPowerStr = String.Format(DoubleStr2, playerData.MagPowerMin, playerData.MagPowerMax);
                }
                else
                {
                    DataModel.MagPowerStr = String.Format(DoubleStr, playerData.MagPowerMin, DataModel.MagPowerMinAdd,
                        playerData.MagPowerMax, DataModel.MagPowerMaxAdd);
                }
            }
                break;
            case 8:
            {
                // DataModel.PhyArmorAdd = value;
                if (value == 0)
                {
                    DataModel.PhyArmorStr = String.Format(OneStr2, playerData.PhyArmor);
                }
                else
                {
                    DataModel.PhyArmorStr = String.Format(OneStr, playerData.PhyArmor, DataModel.PhyArmorAdd);
                }
            }
                break;
            case 9:
            {
                // DataModel.MagArmorAdd = value;
                if (value == 0)
                {
                    DataModel.MagArmorStr = String.Format(OneStr2, playerData.MagArmor);
                }
                else
                {
                    DataModel.MagArmorStr = String.Format(OneStr, playerData.MagArmor, DataModel.MagArmorAdd);
                }
            }
                break;
            case 10:
            {
                //DataModel.HpMaxAdd = value;
                if (value == 0)
                {
                    DataModel.HpMaxStr = String.Format(OneStr2, playerData.HpMax, DataModel.HpMaxAdd);
                }
                else
                {
                    DataModel.HpMaxStr = String.Format(OneStr, playerData.HpMax, DataModel.HpMaxAdd);
                }
            }
                break;
            case 11:
            {
                // DataModel.MpMaxAdd = value;
                if (value == 0)
                {
                    DataModel.MpMaxStr = String.Format(OneStr2, playerData.MpMax);
                }
                else
                {
                    DataModel.MpMaxStr = String.Format(OneStr, playerData.MpMax, DataModel.MpMaxAdd);
                }
            }
                break;
            case 12:
            {
                // DataModel.HitAdd = value;
                if (value == 0)
                {
                    DataModel.HitStr = String.Format(OneStr2, playerData.Hit);
                }
                else
                {
                    DataModel.HitStr = String.Format(OneStr, playerData.Hit, DataModel.HitAdd);
                }
            }
                break;
            case 13:
            {
                // DataModel.DodgeAdd = value;
                if (value == 0)
                {
                    DataModel.DodgeStr = String.Format(OneStr2, playerData.Dodge);
                }
                else
                {
                    DataModel.DodgeStr = String.Format(OneStr, playerData.Dodge, DataModel.DodgeAdd);
                }
            }
                break;
        }
    }

    public void UpdateDistribution(int point)
    {
        DataModel.Distribution = point;
        DataModel.Original = DataModel.Distribution;
    }

    public void RefresNoticeData(bool isNeedAdd)
    {
        if (PlayerDataManager.Instance.NoticeData != null)
        {
            PlayerDataManager.Instance.NoticeData.AddAttr = isNeedAdd;
        }
    }

    public void CleanUp()
    {
        DataModel = new AttributeUIDataModel();
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
        if (ButtonPress != null)
        {
            NetManager.Instance.StopCoroutine(ButtonPress);
            ButtonPress = null;
        }
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        var playerData = PlayerDataManager.Instance;
        var distribute = playerData.GetExData(52);
        UpdateDistribution(distribute);
        ResetAddInfos();
        DataModel.IsAutoAdd = PlayerDataManager.Instance.GetFlag(1001);
        if (DataModel.ShowType == -1)
        {
            DataModel.ShowType = PlayerDataManager.Instance.GetRoleId();
        }
        //第一次登陆有可能扩展数据同步过来
        //ExdataUpdate();
        //ProgressUpdate();
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public FrameState State
    {
        get { return mState; }
        set
        {
            mState = value;
            if (value == FrameState.Close)
            {
                Close();
            }
        }
    }
}