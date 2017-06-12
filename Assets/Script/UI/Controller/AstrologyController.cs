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

#endregion

public class AstrologyController : IControllerBase
{
    public AstrologyController()
    {
        #region 星座名称初始化

        AstrName = new Dictionary<int, string>();
        //         AstrName.Add(0, GameUtils.GetDictionaryText(230201));
        //         AstrName.Add(1, GameUtils.GetDictionaryText(230202));
        //         AstrName.Add(2, GameUtils.GetDictionaryText(230203));
        //         AstrName.Add(3, GameUtils.GetDictionaryText(230204));
        //         AstrName.Add(4, GameUtils.GetDictionaryText(230205));
        //         AstrName.Add(5, GameUtils.GetDictionaryText(230206));
        //         AstrName.Add(6, GameUtils.GetDictionaryText(230207));
        //         AstrName.Add(7, GameUtils.GetDictionaryText(230208));
        //         AstrName.Add(8, GameUtils.GetDictionaryText(230209));
        //         AstrName.Add(9, GameUtils.GetDictionaryText(230210));
        //         AstrName.Add(10, GameUtils.GetDictionaryText(230211));
        //         AstrName.Add(11, GameUtils.GetDictionaryText(230212));
        AstrName.Add(0, GameUtils.GetDictionaryText(270029)); //"白羊座");
        AstrName.Add(1, GameUtils.GetDictionaryText(270030)); //"金牛座");
        AstrName.Add(2, GameUtils.GetDictionaryText(270031)); //"双子座");
        AstrName.Add(3, GameUtils.GetDictionaryText(270032)); //"巨蟹座");
        AstrName.Add(4, GameUtils.GetDictionaryText(270033)); //"狮子座");
        AstrName.Add(5, GameUtils.GetDictionaryText(270034)); //"处女座");
        AstrName.Add(6, GameUtils.GetDictionaryText(270035)); //"天秤座");
        AstrName.Add(7, GameUtils.GetDictionaryText(270036)); //"天蝎座");
        AstrName.Add(8, GameUtils.GetDictionaryText(270037)); //"人马座");
        AstrName.Add(9, GameUtils.GetDictionaryText(270038)); //"山羊座");
        AstrName.Add(10, GameUtils.GetDictionaryText(270039)); // "水瓶座");
        AstrName.Add(11, GameUtils.GetDictionaryText(270040)); // "双鱼座");

        #endregion

        #region 属性加成判断条件

        ConditionDict = new Dictionary<int, string>();
        ConditionDict.Add(0, GameUtils.GetDictionaryText(270041)); // "（随从个数>{0}/{1}）");  
        ConditionDict.Add(1, GameUtils.GetDictionaryText(270042)); // "（随从最高等级>{0}/{1}） ");
        ConditionDict.Add(2, GameUtils.GetDictionaryText(270043)); // "（随从最高战力>{0}/{1})");
        ConditionDict.Add(3, GameUtils.GetDictionaryText(270044)); // "（随从总等级>{0}/{1})");
        ConditionDict.Add(4, GameUtils.GetDictionaryText(270045)); // "（随从总战力>{0}/{1})");
        ConditionDict.Add(5, GameUtils.GetDictionaryText(270046)); // "（幸运石:{0})");
        ConditionDict.Add(6, GameUtils.GetDictionaryText(270047)); // "（诞生石:{0})");

        #endregion

        EventDispatcher.Instance.AddEventListener(UIEvent_AstrologyBtnBuyList.EVENT_TYPE, BtnBuyList); //购买宝石
        EventDispatcher.Instance.AddEventListener(UIEvent_AstrologyBtnDiamonds.EVENT_TYPE, BtnDiamonds); //点击3个宝石中的一个
        EventDispatcher.Instance.AddEventListener(UIEvent_AstrologyOperation.EVENT_TYPE, AstrologyOperation); //占星屋操作
        EventDispatcher.Instance.AddEventListener(UIEvent_AstrologyBtnPutOn.EVENT_TYPE, BtnPutOn); //穿宝石
        EventDispatcher.Instance.AddEventListener(UIEvent_AstrologyBtnPutOff.EVENT_TYPE, BtnPutOff); //脱宝石
        EventDispatcher.Instance.AddEventListener(UIEvent_AstrologyMainListClick.EVENT_TYPE, MainListClick);
            //第一界面星座list点击事件
        EventDispatcher.Instance.AddEventListener(UIEvent_AstrologyBack.EVENT_TYPE, BtnBack); //返回第一界面   
        EventDispatcher.Instance.AddEventListener(UIEvent_AstrologyBagItemClick.EVENT_TYPE, BagItemClick); //背包物品点击事件
        EventDispatcher.Instance.AddEventListener(UIEvent_AstrologySimpleListClick.EVENT_TYPE, SimpleListClick);
            //星座缩略图点击事件
        EventDispatcher.Instance.AddEventListener(UIEvent_AstrologyBagTabClick.EVENT_TYPE, BagTabClick); //背包Tab 点击事件
        EventDispatcher.Instance.AddEventListener(UIEvent_AstrologyMainIconClick.EVENT_TYPE, MainIconClick); //星座主界面图标事件
        EventDispatcher.Instance.AddEventListener(UIEvent_AstrologyArrangeClick.EVENT_TYPE, ArrangeClick); //背包整理
        EventDispatcher.Instance.AddEventListener(UIEvent_AstrologyDrawResult.EVENT_TYPE, DrawResult); //抽奖结果

        CleanUp();
    }

    public BuildingData AstrBuilding;
    public Dictionary<int, string> AstrName; //星座名称
    public int AstrologyIndex = -1; //星座index
    public string attrGreenColor = "[0cff00]"; //亮绿
    public string attrGreyColor = "[C0C0C0]"; //灰色
    public Dictionary<int, string> ConditionDict; //属性加成判断条件
    public int diaIndex; //3个宝石中的index
    public bool firstRun = true; //第一次运行界面
    public int itemIndex = -1; //背包的index
    public int[] MaxExp = new int[5]; //存放五种品质最大等级经验
    public int selectedType = -1; //0：宝石，1为背包
    public int tab_Page = (int) GEM_TAB_PAGE.PageGemAll;
    public int TotalExp; //升级总的经验
    public int willLevel = 0; //升级将要达到的等级
    //属性判断条件
    public enum ATTR_CONDITION
    {
        NoCondition = 0, //无条件
        PetCount = 1, //随从个数要求
        PetMaxLevel = 2, //随从最大等级要求
        PetMaxPower = 3, //随从最大战力要求
        PetTotalLevel = 4, //随从总等级
        PetTotalPower = 5, //随从中战力和
        AstryLuck = 6, //幸运石
        AstryBirth = 7, //诞生石
        ConditionCount
    }

    //宝石类型
    public enum DIAMOND_INDEX
    {
        Diamond = 0, //宝石
        Crystal = 1, //水晶
        Agate = 2 //玛瑙
    }

    //背包page页
    public enum GEM_TAB_PAGE
    {
        PageGemAll = 0,
        PageDiamond = 1, //宝石
        PageCrystal = 2, //水晶
        PageAgate = 3 //玛瑙
    }

    public AstrologyDetailDataModel DetailData { get; set; }
    public AstrologyDataModel MainData { get; set; }

    #region 详细界面

    //占星屋操作
    public void AstrologyOperation(IEvent ievent)
    {
        var e = ievent as UIEvent_AstrologyOperation;
        switch (e.Type)
        {
            case 0:
            {
                BtnUpShow();
            }
                break;
            case 1:
            {
                BtnUpOk();
            }
                break;
            case 2:
            {
                BtnUpBack();
            }
                break;
        }
    }

    //星座缩略图点击事件
    public void SimpleListClick(IEvent ievent)
    {
        var e = ievent as UIEvent_AstrologySimpleListClick;
        AstrologyIndex = e.Index;
        InitDetailUI(e.Index);

        if (DetailData.ShowLevelUI == 1)
        {
            if (selectedType == 0)
            {
                SetSelectedDiamond(DetailData.DiaGroup[diaIndex]);
                AddAllExp();
                WillLevel(DetailData.SelectedDia);
            }
            else
            {
                SetSelectedDiamond(MainData.BagBase[itemIndex]);
            }
        }
        else
        {
            if (selectedType == 0)
            {
                SetSelectedDiamond(DetailData.DiaGroup[diaIndex]);
            }
            else
            {
                SetSelectedDiamond(MainData.BagBase[itemIndex]);
            }
        }
        SetBagInit(false);
    }

    //设置宝石选择框
    public void SetGroupDiamondSeleted()
    {
        for (var i = 0; i < DetailData.DiaGroup.Count; i++)
        {
            DetailData.DiaGroup[i].Selected = 0;
        }
        DetailData.DiaGroup[diaIndex].Selected = 1;
    }

    //点击3个宝石中的一个
    public void BtnDiamonds(IEvent ievent)
    {
        var e = ievent as UIEvent_AstrologyBtnDiamonds;
        selectedType = 0;
        diaIndex = e.Index;
        SetSelectedDiamond(DetailData.DiaGroup[e.Index]);
        diaIndex = e.Index;
        //取消背包选中状态
        if (itemIndex != -1)
        {
            for (var i = 0; i < MainData.BagBaseTemp.Count; i++)
            {
                if (itemIndex == MainData.BagBaseTemp[i].Index)
                {
                    MainData.BagBaseTemp[i].Selected = 0;
                    break;
                }
            }
        }
        itemIndex = -1;
        if (DetailData.ShowLevelUI == 0)
        {
            RefleshBag(e.Index + 1);
            DetailData.CanPutOn = 0;
            if (DetailData.DiaGroup[diaIndex].ID != -1)
            {
                DetailData.CanPutOff = 1;
            }
            else
            {
                DetailData.CanPutOff = 0;
            }
        }
        else
        {
            AddAllExp();
            WillLevel(DetailData.SelectedDia);
        }
    }

    //穿宝石
    public void BtnPutOn(IEvent ievent)
    {
        var e = ievent as UIEvent_AstrologyBtnPutOn;
        if (itemIndex < 0 || AstrologyIndex < 0)
        {
            return;
        }
        NetManager.Instance.StartCoroutine(AstrologyEquipOn(itemIndex, AstrologyIndex, diaIndex));
    }

    public IEnumerator AstrologyEquipOn(int bagIndex, int astrologyId, int Index)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.AstrologyEquipOn(bagIndex, astrologyId, Index);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    Logger.Info("-----------equipOn ok ");
                }
                else
                {
                    if (msg.ErrorCode == (int) ErrorCodes.Error_ItemNoInBag_All)
                    {
                        var e = new ShowUIHintBoard(705); //test
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                    else
                    {
                        UIManager.Instance.ShowNetError(msg.ErrorCode);
                    }
                }
            }
            else
            {
                var e = new ShowUIHintBoard(705); //test
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    //脱宝石
    public void BtnPutOff(IEvent ievent)
    {
        var e = new UIEvent_AstrologyBtnPutOff();
        NetManager.Instance.StartCoroutine(AstrologyEquipOff(AstrologyIndex, diaIndex));
    }

    public IEnumerator AstrologyEquipOff(int astrologyId, int Index)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.AstrologyEquipOff(astrologyId, Index);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    Logger.Info("-----------equipOn btnputoff ");
                }
                else
                {
                    if (msg.ErrorCode == (int) ErrorCodes.Error_ItemNoInBag_All)
                    {
                        var e = new ShowUIHintBoard(705); //test
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                    else
                    {
                        UIManager.Instance.ShowNetError(msg.ErrorCode);
                    }
                }
            }
            else
            {
                var e = new ShowUIHintBoard(705); //test
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    //无宝石所有清空
    public void SetDiamondNull()
    {
        DetailData.SelectedDia.ID = -1;
        DetailData.WillGroup[0].ID = -1;
        DetailData.WillGroup[1].ID = -1;
        DetailData.WillGroup[2].ID = -1;
        DetailData.DiamondType = "";
        DetailData.DiamondColor = "";
        DetailData.DiamondLevel = "";
        DetailData.WillAttr[0] = "";
        DetailData.WillAttr[1] = "";
        DetailData.SelectColor[0] = false;
        DetailData.SelectColor[1] = false;
        DetailData.ShowGroupProp = 0;
        TotalExp = 0;
        for (var i = 0; i < DetailData.Attributes.Count; i++)
        {
            DetailData.Attributes[i].AttrShow = 0;
        }
        ResetSelectedExp();
    }

    //设置选择的宝石   type =0为背包物品，1为星座物品
    public void SetSelectedDiamond(AstrologyDiamondDataModel item)
    {
        SetGroupDiamondSeleted();
        DetailData.SelectedDia = item;
        DetailData.GemID = item.ID;
        if (item.ID == -1)
        {
            SetDiamondNull();
            return;
        }
        var tbGem = Table.GetGem(item.ID);
        var tbItemType = Table.GetItemType(tbGem.Type);
        var tbColor = Table.GetColorBase(Table.GetGem(item.ID).Quality);
        DetailData.DiamondType = tbItemType.Name;
        DetailData.DiamondLevel = item.Extra.Level.ToString();
        DetailData.DiamondColor = tbColor.Desc;

        SetWillDiaList(item);
        RefleshItemAttr(item); //刷新属性
    }

    //初始化3个装备的宝石
    public void InitDetailUI(int index)
    {
        MainData.SwitchUI = 1;
        for (var i = 0; i < DetailData.DiaGroup.Count; i++)
        {
            DetailData.DiaGroup[i] = MainData.MainList[index].DiamondList[i];
        }
        for (var i = 0; i < DetailData.SimapleList.Count; i++)
        {
            DetailData.SimapleList[i].Selected = 0;
        }
        DetailData.SimapleList[AstrologyIndex].Selected = 1;
    }

    #endregion

    #region 刷新属性

    //刷新属性列表
    public void RefleshItemAttr(AstrologyDiamondDataModel item)
    {
        //  DetailData.Attributes

        var tbGem = Table.GetGem(item.ID);
        for (var i = 0; i < DetailData.Attributes.Count; i++)
        {
            //每个条件可以激活两条属性
            var varStr = "";
            // varStr =  GameUtils.GetDictionaryText(230006);
            var conditionValue = new AsyncResult<int>();
            var isOk = AttrActiveCondition(conditionValue, item, tbGem.ActiveCondition[i], tbGem.Param[i]);
            DetailData.Attributes[i].AttrShow = 1;
            if (isOk)
            {
                varStr += attrGreenColor; //亮绿           
            }
            else
            {
                varStr += attrGreyColor;
                // DetailData.Attributes[i].AttrShow = 0;
            }
            //属性名称和值
            {
                if (tbGem.Prop1[i] != -1)
                {
                    var value = PropValue(item.Extra.Level, tbGem.PropValue1[i]);
                    varStr += AttrFormat(tbGem.Prop1[i], value);
                    varStr += ",";
                }

                if (tbGem.Prop2[i] != -1)
                {
                    var value = PropValue(item.Extra.Level, tbGem.PropValue2[i]);
                    varStr += AttrFormat(tbGem.Prop2[i], value);
                }
                varStr = AttrAttachString(varStr, tbGem.ActiveCondition[i], conditionValue.Result, tbGem.Param[i]);
                DetailData.Attributes[i].AttrName = varStr;
            }
        }
    }

    //属性后面附件的字段
    public string AttrAttachString(string str, int type, int value, int param)
    {
        if (type <= 0)
        {
            return str;
        }
        switch (type)
        {
            case (int) ATTR_CONDITION.NoCondition:
            {
            }
                break;
            case (int) ATTR_CONDITION.PetCount:
            {
                str += string.Format(ConditionDict[0], value, param);
            }
                break;
            case (int) ATTR_CONDITION.PetMaxLevel:
            {
                str += string.Format(ConditionDict[1], value, param);
            }
                break;
            case (int) ATTR_CONDITION.PetMaxPower:
            {
                str += string.Format(ConditionDict[2], value, param);
            }
                break;
            case (int) ATTR_CONDITION.PetTotalLevel:
            {
                str += string.Format(ConditionDict[3], value, param);
            }
                break;
            case (int) ATTR_CONDITION.PetTotalPower:
            {
                str += string.Format(ConditionDict[4], value, param);
            }
                break;
            case (int) ATTR_CONDITION.AstryBirth:
            {
                if (param >= 0 && param <= 11)
                {
                    str += string.Format(ConditionDict[5], AstrName[param]);
                }
            }
                break;
            case (int) ATTR_CONDITION.AstryLuck:
            {
                if (param >= 0 && param <= 11)
                {
                    str += string.Format(ConditionDict[6], AstrName[param]);
                }
            }
                break;
        }
        return str;
    }

    //返回单个属性字符串
    public string AttrFormat(int mprop, int value)
    {
        var str = "";
        var attrValue = GameUtils.AttributeValue(mprop, value);
        var attrName = GameUtils.AttributeName(mprop);
        var strFormat = "{0}+{1}";
        str = string.Format(strFormat, attrName, attrValue);
        return str;
    }

    //属性的dictionary列表
    public void AttrDictionary(Dictionary<int, int> mpropvalue, AstrologyDiamondDataModel item)
    {
        if (item.ID == -1)
        {
            return;
        }
        var tbGem = Table.GetGem(item.ID);
        var AttribuiteCount = DetailData.Attributes.Count; //宝石属性个数
        for (var i = 0; i < AttribuiteCount; i++)
        {
            var conditionValue = new AsyncResult<int>();
            var isOk = AttrActiveCondition(conditionValue, item, tbGem.ActiveCondition[i], tbGem.Param[i], true);
            if (isOk)
            {
                var key = tbGem.Prop1[i];
                if (key != -1)
                {
                    if (mpropvalue.ContainsKey(key))
                    {
                        mpropvalue[key] += PropValue(item.Extra.Level, tbGem.PropValue1[i]);
                    }
                    else
                    {
                        mpropvalue.Add(key, PropValue(item.Extra.Level, tbGem.PropValue1[i]));
                    }
                }
                key = tbGem.Prop2[i];
                if (key != -1)
                {
                    if (mpropvalue.ContainsKey(key))
                    {
                        mpropvalue[key] += PropValue(item.Extra.Level, tbGem.PropValue1[i]);
                    }
                    else
                    {
                        mpropvalue.Add(key, PropValue(item.Extra.Level, tbGem.PropValue1[i]));
                    }
                }
            }
        }
    }

    //单个属性值
    public int PropValue(int level, int mLogic)
    {
        var tbSkillUpdate = Table.GetSkillUpgrading(mLogic);
        if (tbSkillUpdate != null)
        {
            return tbSkillUpdate.GetSkillUpgradingValue(level);
        }
        return 0;
    }

    //属性条件判断  type 类型，value 判定值
    public bool AttrActiveCondition(AsyncResult<int> Count,
                                    AstrologyDiamondDataModel item,
                                    int type,
                                    int value,
                                    bool isDiamondPutOn = false)
    {
        if (item.ID == -1)
        {
            return false;
        }
        // var tbGem = Table.GetGem(item.ID);
        if (type < 0 || type >= (int) ATTR_CONDITION.ConditionCount)
        {
            return false;
        }
        switch (type)
        {
            //无条件
            case (int) ATTR_CONDITION.NoCondition:
            {
                return true;
            }
                break;
            //幸运石或者诞生石
            case (int) ATTR_CONDITION.AstryLuck:
            case (int) ATTR_CONDITION.AstryBirth:
            {
                if (isDiamondPutOn && value == AstrologyIndex)
                {
                    return true;
                }
                if (selectedType == 0 && value == AstrologyIndex)
                {
                    return true;
                }
            }
                break;
            //随从数量
            case (int) ATTR_CONDITION.PetCount:
            {
                var petList = CityManager.Instance.GetAllPetByFilter(PetListFileterType.Employ); //取得所有隋丛林列表
                Count.Result = petList.Count;
                if (petList.Count > value)
                {
                    return true;
                }
            }
                break;
            //随从最大等级
            case (int) ATTR_CONDITION.PetMaxLevel:
            {
                var maxLevel = 0;
                var petList = CityManager.Instance.GetAllPetByFilter(PetListFileterType.Employ); //取得所有隋丛林列表
                for (var i = 0; i < petList.Count; i++)
                {
                    var level = petList[i].Exdata[PetItemExtDataIdx.Level];
                    if (level > maxLevel)
                    {
                        maxLevel = level;
                    }
                }
                Count.Result = maxLevel;
                if (maxLevel > value)
                {
                    return true;
                }
            }
                break;
            //随从最大战力
            case (int) ATTR_CONDITION.PetMaxPower:
            {
                var maxPower = 0;
                var petList = CityManager.Instance.GetAllPetByFilter(PetListFileterType.Employ); //取得所有隋丛林列表
                for (var i = 0; i < petList.Count; i++)
                {
                    if (petList[i].ItemId <= 0)
                    {
                        continue;
                    }
                    var tbPet = Table.GetPet(petList[i].ItemId);
                    if (tbPet == null)
                    {
                        continue;
                    }

                    var power = FightAttribute.CalculatePetFightPower(tbPet, petList[i].Exdata[PetItemExtDataIdx.Level]);
                    if (power > maxPower)
                    {
                        maxPower = power;
                    }
                    Count.Result = maxPower;
                    if (maxPower > value)
                    {
                        return true;
                    }
                }
            }
                break;
            //随从总等级
            case (int) ATTR_CONDITION.PetTotalLevel:
            {
                var totalLevel = 0;
                var petList = CityManager.Instance.GetAllPetByFilter(PetListFileterType.Employ); //取得所有隋丛林列表
                for (var i = 0; i < petList.Count; i++)
                {
                    var level = petList[i].Exdata[PetItemExtDataIdx.Level];
                    totalLevel += level;
                }
                Count.Result = totalLevel;
                if (totalLevel > value)
                {
                    return true;
                }
            }
                break;
            //随从总战力
            case (int) ATTR_CONDITION.PetTotalPower:
            {
                var totalPower = 0;
                var petList = CityManager.Instance.GetAllPetByFilter(PetListFileterType.Employ); //取得所有隋丛林列表
                for (var i = 0; i < petList.Count; i++)
                {
                    if (petList[i].ItemId <= 0)
                    {
                        continue;
                    }
                    var tbPet = Table.GetPet(petList[i].ItemId);
                    if (tbPet == null)
                    {
                        continue;
                    }

                    var power = FightAttribute.CalculatePetFightPower(tbPet, petList[i].Exdata[PetItemExtDataIdx.Level]);
                    totalPower += power;
                }
                Count.Result = totalPower;
                if (totalPower > value)
                {
                    return true;
                }
            }
                break;
        }
        return false;
    }

    //三颗宝石在总属性    总属性+激活属性
    public void AttrTotalProp()
    {
        MainData.Attributes.Clear();
        var mprop = new Dictionary<int, int>();
        var diaItem = MainData.MainList[AstrologyIndex].DiamondList;
        for (var i = 0; i < diaItem.Count; i++)
        {
            AttrDictionary(mprop, diaItem[i]);
        }
        var mList = new List<AstrologyDiamondDataModel>();
        for (var i = 0; i < diaItem.Count; i++)
        {
            mList.Add(diaItem[i]);
        }
        GetGroupAttr(mprop, mList);
        {
            // foreach(var i in mprop)
            var __enumerator1 = (mprop).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var i = __enumerator1.Current;
                {
                    var ii = new AstrologyAttributeDataModel();
                    ii.AttrName = AttrFormat(i.Key, i.Value);
                    MainData.Attributes.Add(ii);
                }
            }
        }
    }

    //设置组合表
    public void SetWillDiaList(AstrologyDiamondDataModel item)
    {
        for (var i = 0; i < DetailData.WillGroup.Count; i++)
        {
            DetailData.WillGroup[i].State = 0;
        }
        for (var i = 0; i < DetailData.WillAttr.Count; i++)
        {
            DetailData.WillAttr[i] = attrGreyColor;
        }
        DetailData.ShowGroupProp = 0;
        if (item.ID == -1)
        {
            return;
        }
        var tbGem = Table.GetGem(item.ID);
        if (tbGem == null)
        {
            return;
        }
        if (tbGem.Combination == -1)
        {
            DetailData.GroupName = "";
            DetailData.WillGroup[0].ID = -1;
            DetailData.WillGroup[1].ID = -1;
            DetailData.WillGroup[2].ID = -1;
            return;
        }
        var tbgroup = Table.GetGemGroup(tbGem.Combination);
        if (tbgroup == null)
        {
            return;
        }
        DetailData.ShowGroupProp = 1;
        DetailData.WillGroup[0].ID = tbgroup.DiaID;
        DetailData.WillGroup[1].ID = tbgroup.CrystalID;
        DetailData.WillGroup[2].ID = tbgroup.AgateID;
        DetailData.GroupName = tbgroup.Name;

        var count = 0;
        for (var j = 0; j < DetailData.WillGroup.Count; j++)
        {
            for (var i = 0; i < DetailData.DiaGroup.Count; i++)
            {
                if (DetailData.WillGroup[j].ID == DetailData.DiaGroup[i].ID)
                {
                    DetailData.WillGroup[j].State = 1;
                    break;
                }
            }
            if (DetailData.WillGroup[j].State == 1)
            {
                count++;
            }
        }


        if (count >= 2)
        {
            DetailData.WillAttr[0] = attrGreenColor;
            DetailData.WillAttr[1] = attrGreenColor;
        }
        if (count == 3)
        {
            DetailData.WillAttr[2] = attrGreenColor;
            DetailData.WillAttr[3] = attrGreenColor;
        }
        for (var i = 0; i < tbgroup.Towprop.Length; i++)
        {
            if (tbgroup.Towprop[i] != -1)
            {
                // int value = PropValue(item.Extra.Level, tbgroup.TowValue[i]);
                var value = tbgroup.TowValue[i];
                DetailData.WillAttr[i] += AttrFormat(tbgroup.Towprop[i], value);
            }
        }

        for (var i = 0; i < tbgroup.Threeprop.Length; i++)
        {
            if (tbgroup.Threeprop[i] != -1)
            {
                //int value = PropValue(item.Extra.Level, tbgroup.ThreeValue[i]);
                var value = tbgroup.ThreeValue[i];
                DetailData.WillAttr[i + 2] += AttrFormat(tbgroup.Threeprop[i], value);
            }
        }
    }

    //计算组合属性
    public void GetGroupAttr(Dictionary<int, int> mdict, List<AstrologyDiamondDataModel> items)
    {
        var maxCount = 0;
        var maxIndex = -1;
        for (var i = 0; i < items.Count; i++)
        {
            if (items[i].ID == -1)
            {
                continue;
            }
            var tbGem = Table.GetGem(items[i].ID);
            if (tbGem.Combination == -1)
            {
                continue;
            }
            var tbGemgroup = Table.GetGemGroup(tbGem.Combination);
            int[] diaId = {tbGemgroup.DiaID, tbGemgroup.CrystalID, tbGemgroup.AgateID};
            var varCount = 0;
            for (var j = 0; j < items.Count; j++)
            {
                for (var k = 0; k < diaId.Length; k++)
                {
                    if (diaId[k] == items[j].ID)
                    {
                        varCount++;
                    }
                }
            }
            if (varCount > maxCount)
            {
                maxCount = varCount;
                maxIndex = i;
            }
        }

        if (maxIndex > -1)
        {
            var tbGem = Table.GetGem(items[maxIndex].ID);
            var tbGemGroup = Table.GetGemGroup(tbGem.Combination);
            if (maxCount >= 2)
            {
                for (var i = 0; i < tbGemGroup.Towprop.Length; i++)
                {
                    if (tbGemGroup.Towprop[i] != -1)
                    {
                        if (mdict.ContainsKey(tbGemGroup.Towprop[i]))
                        {
                            mdict[tbGemGroup.Towprop[i]] += tbGemGroup.TowValue[i];
                        }
                        else
                        {
                            mdict.Add(tbGemGroup.Towprop[i], tbGemGroup.TowValue[i]);
                        }
                    }
                }
            }
            if (maxCount >= 3)
            {
                for (var i = 0; i < tbGemGroup.Threeprop.Length; i++)
                {
                    if (tbGemGroup.Threeprop[i] != -1)
                    {
                        if (mdict.ContainsKey(tbGemGroup.Threeprop[i]))
                        {
                            mdict[tbGemGroup.Threeprop[i]] += tbGemGroup.ThreeValue[i];
                        }
                        else
                        {
                            mdict.Add(tbGemGroup.Threeprop[i], tbGemGroup.ThreeValue[i]);
                        }
                    }
                }
            }
        }
    }

    #endregion

    #region 主界面

    public void InitUI()
    {
        if (firstRun)
        {
            DetailData.DiaGroup.Clear();
            for (var i = 0; i < 3; i++)
            {
                var item = new AstrologyDiamondDataModel();
                DetailData.DiaGroup.Add(item);
            }
            DetailData.SimapleList.Clear();
            for (var i = 0; i < 12; i++)
            {
                var ii = new AstrologySimpleIconDataModel();
                ii.AttrName = AstrName[i];
                ii.Selected = 0;
                DetailData.SimapleList.Add(ii);
            }
        }
        if (AstrologyIndex == -1)
        {
            AstrologyIndex = 0;
            MainListSet(AstrologyIndex);
            AttrTotalProp();
        }
        firstRun = false;
    }

    //星座主界面图标事件
    public void MainIconClick(IEvent ievent)
    {
        var e = ievent as UIEvent_AstrologyMainIconClick;
        MainListSet(e.Index);
        AttrTotalProp();
    }

    //主界面星座list设置
    public void MainListSet(int index)
    {
        for (var i = 0; i < MainData.MainList.Count; i++)
        {
            MainData.MainList[i].IsChoose = 0;
        }
        MainData.MainList[index].IsChoose = 1;
        AstrologyIndex = index;
        MainData.AstrName = MainData.MainList[index].ConstellationName;
    }

    //返回第一界面
    public void BtnBack(IEvent ievent)
    {
        var e = ievent as UIEvent_AstrologyBack;
        if (e.Index == 0) //从主界面到详细界面
        {
            if (MainData.SwitchUI == 0)
            {
                EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.AstrologyUI));
                //EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.CityUI));
            }
            else
            {
                AttrTotalProp();
                MainData.SwitchUI = 0;
            }
        }
    }

    //第一界面星座list点击事件
    public void MainListClick(IEvent ievent)
    {
        var e = ievent as UIEvent_AstrologyMainListClick;
        MainListSet(e.Index);
        InitDetailUI(e.Index);
        for (var i = 0; i < MainData.MainList.Count; i++)
        {
            MainData.MainList[i].IsChoose = 0;
        }
        MainData.MainList[e.Index].IsChoose = 1;
        diaIndex = e.ItemIndex;
        DetailData.ShowLevelUI = 0;
        if (DetailData.DiaGroup[diaIndex].ID != -1)
        {
            DetailData.CanPutOff = 1;
            DetailData.CanPutOn = 0;
        }
        // 顺序为点击的宝石->背包第一个位置的宝石->选择空位置
        var item = MainData.MainList[e.Index];
        selectedType = 0;
        diaIndex = e.ItemIndex;
        SetSelectedDiamond(DetailData.DiaGroup[e.ItemIndex]);
        if (item.DiamondList[e.ItemIndex].ID == -1)
        {
            itemIndex = -1;
            for (var i = 0; i < MainData.BagBase.Count; i++)
            {
                if (MainData.BagBase[i].ID == -1)
                {
                    continue;
                }
                var tbGem = Table.GetGem(MainData.BagBase[i].ID);
                if (tbGem == null)
                {
                    continue;
                }
                if (diaIndex == GetDiamondType(tbGem.Type))
                {
                    itemIndex = i;
                    selectedType = 1;
                    SetSelectedDiamond(MainData.BagBase[0]);
                    DetailData.CanPutOff = 0;
                    DetailData.CanPutOn = 1;
                    break;
                }
            }
        }
        RefleshBag(e.ItemIndex + 1);
        SetBagInit(false);
        //滚动效果
        var ee = new UIEvent_AstrologySetGridLookIndex();
        ee.Type = 1;
        ee.Index = AstrologyIndex;
        EventDispatcher.Instance.DispatchEvent(ee);
    }

    //购买宝石
    public void BtnBuyList(IEvent ievent)
    {
        var e = ievent as UIEvent_AstrologyBtnBuyList;
        if (e.Index == 0)
        {
            //+判断条件
            NetManager.Instance.StartCoroutine(DrawDiamond(e.Index));
        }
        else if (e.Index == 1)
        {
            //+判断条件
            NetManager.Instance.StartCoroutine(DrawDiamond(e.Index));
        }
    }

    public IEnumerator DrawDiamond(int type)
    {
        using (new BlockingLayerHelper(0))
        {
            var tbBuild = Table.GetBuilding(AstrBuilding.TypeId);
            var array = new Int32Array();
            if (type == 0)
            {
                array.Items.Add(100);
            }
            else if (type == 1)
            {
                array.Items.Add(101);
            }
            var msg = NetManager.Instance.UseBuildService(AstrBuilding.AreaId, tbBuild.ServiceId, array);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    Logger.Info("-----------DrawDiamond ok ");
                }
                else
                {
                    if (msg.ErrorCode == (int) ErrorCodes.Error_ItemNoInBag_All)
                    {
                        var e = new ShowUIHintBoard(705); //test
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                }
            }
            else
            {
                var e = new ShowUIHintBoard(705); //test
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    //抽奖结果
    public void DrawResult(IEvent ievent)
    {
        var e = ievent as UIEvent_AstrologyDrawResult;
    }

    #endregion

    #region 背包界面

    //背包整理
    public void ArrangeClick(IEvent ievent)
    {
        NetManager.Instance.StartCoroutine(OnArrangeCoroutine((int) eBagType.GemBag));
    }

    public IEnumerator OnArrangeCoroutine(int nBagId)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.SortBag(nBagId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var bag = msg.Response;
                    InitAstrologyData(bag);
                    SetBagInit();
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Info("  -------------OnArrangeCoroutine -----error--------- {0}", msg.ErrorCode);
                }
            }
            else
            {
                var e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    //初始化背包
    public void SetBagInit(bool setDiamond = true)
    {
        for (var i = 0; i < MainData.BagBaseTemp.Count; i++)
        {
            MainData.BagBaseTemp[i].Selected = 0;
        }
        selectedType = 0;
        itemIndex = -1;
        DetailData.ShowLevelUI = 0;
        if (DetailData.DiaGroup[diaIndex].ID == -1)
        {
            DetailData.CanPutOff = 0;
        }
        else
        {
            DetailData.CanPutOff = 1;
        }
        DetailData.CanPutOn = 0;
        for (var i = 0; i < DetailData.TabPageShow.Count; i++)
        {
            DetailData.TabPageShow[i] = 0;
        }
        if (setDiamond)
        {
            SetSelectedDiamond(DetailData.DiaGroup[diaIndex]);
        }
    }

    //刷新背包
    public void RefleshBag(int tabIndex)
    {
        tab_Page = tabIndex;
        DetailData.TabPage = tabIndex;
        if (tabIndex < 0 || tabIndex > 3)
        {
            return;
        }
        var count = 0;
        var list = new List<AstrologyDiamondDataModel>();
        for (var i = 0; i < MainData.BagBase.Count; i++)
        {
            var temp = new AstrologyDiamondDataModel(MainData.BagBase[i]);

            if (MainData.BagBase[i].ID == -1)
            {
                continue;
            }
            temp.Extra.InstallData(MainData.BagBase[i].Extra);
            if (tabIndex == 0)
            {
                list.Add(temp);
            }
            else
            {
                if (MainData.BagBase[i].DiaType == tabIndex - 1)
                {
                    list.Add(temp);
                }
                else
                {
                    continue;
                }
            }
            if (diaIndex != (MainData.BagBase[i].DiaType))
            {
                temp.State = 1;
            }
            else
            {
                temp.State = 0;
            }
            if (itemIndex == temp.Index)
            {
                temp.Selected = 1;
            }
            count++;
        }

        var varcount = list.Count;
        var tbBagbase = Table.GetBagBase((int) eBagType.GemBag);
        for (var i = 0; i < tbBagbase.MaxCapacity - varcount; i++)
        {
            var temp = new AstrologyDiamondDataModel();
            list.Add(temp);
        }
        MainData.BagBaseTemp = new ObservableCollection<AstrologyDiamondDataModel>(list);

        if (selectedType == 1 && itemIndex != -1)
        {
            SetSelectedDiamond(MainData.BagBase[itemIndex]);
        }
        ResetSelectedExp();
    }

    //背包Tab 点击事件
    public void BagTabClick(IEvent ievent)
    {
        var e = ievent as UIEvent_AstrologyBagTabClick;
        RefleshBag(e.Index);
    }

    //背包物品点击事件
    public void BagItemClick(IEvent ievent)
    {
        var e = ievent as UIEvent_AstrologyBagItemClick;
        if (MainData.BagBaseTemp[e.Index].ID == -1)
        {
            return;
        }
        if (DetailData.ShowLevelUI == 1)
        {
            if (DetailData.SelectedDia.ID == -1)
            {
                return;
            }
            if (itemIndex == -1 || itemIndex != MainData.BagBaseTemp[e.Index].Index)
            {
                if (MainData.BagBaseTemp[e.Index].IsChoose)
                {
                    DeleteOneExp(MainData.BagBaseTemp[e.Index]);
                }
                else
                {
                    AddOneExp(MainData.BagBaseTemp[e.Index]);
                }
                MainData.BagBaseTemp[e.Index].IsChoose = MainData.BagBaseTemp[e.Index].IsChoose ? false : true;
                WillLevel(DetailData.SelectedDia);
            }
        }
        else
        {
            selectedType = 1;
            for (var i = 0; i < MainData.BagBaseTemp.Count; i++)
            {
                MainData.BagBaseTemp[i].Selected = 0;
            }
            itemIndex = MainData.BagBaseTemp[e.Index].Index;
            MainData.BagBaseTemp[e.Index].Selected = 1;
            SetSelectedDiamond(MainData.BagBaseTemp[e.Index]);
            if (MainData.BagBaseTemp[e.Index].State == 1)
            {
                DetailData.CanPutOff = 0;
                DetailData.CanPutOn = 0;
            }
            else
            {
                DetailData.CanPutOff = 0;
                DetailData.CanPutOn = 1;
            }
            SetWillDiaList(MainData.BagBaseTemp[e.Index]);
        }
    }

    #endregion

    #region 升级界面

    //蓝绿选择
    public void OnToggleChange(object sender, PropertyChangedEventArgs e)
    {
        SetSelectColor();
    }

    //设置勾选图标
    public void SetSelectColor()
    {
        if (MainData.BagBaseTemp.Count == 0)
        {
            return;
        }
        for (var i = 0; i < MainData.BagBaseTemp.Count; i++)
        {
            var item = MainData.BagBaseTemp[i];
            if (item.ID == -1)
            {
                continue;
            }
            var tbGem = Table.GetGem(item.ID);
            if (tbGem == null)
            {
                continue;
            }
            if (itemIndex == item.Index)
            {
                continue;
            }
            if (tbGem.Quality == 1)
            {
                item.IsChoose = DetailData.SelectColor[0];
            }
            if (tbGem.Quality == 2)
            {
                item.IsChoose = DetailData.SelectColor[1];
            }
        }

        AddAllExp();
        WillLevel(DetailData.SelectedDia);
    }

    //升级界面显示
    public void BtnUpShow()
    {
        DetailData.ShowLevelUI = 1;
        RefleshBag((int) GEM_TAB_PAGE.PageGemAll);
        for (var i = 0; i < DetailData.TabPageShow.Count; i++)
        {
            DetailData.TabPageShow[i] = 1;
        }
        DetailData.TabPageShow[0] = 0;
        SetSelectColor();
    }

    //确认升级
    public void BtnUpOk()
    {
        var index = -1;
        var bagid = -1;
        if (selectedType == 0)
        {
            index = diaIndex*12 + AstrologyIndex;
            bagid = (int) eBagType.GemEquip;
        }
        else if (selectedType == 1)
        {
            index = itemIndex;
            bagid = (int) eBagType.GemBag;
        }
        else
        {
            return;
        }
        if (index < 0)
        {
            return;
        }
        var itemList = new Int32Array();
        for (var i = 0; i < MainData.BagBaseTemp.Count; i++)
        {
            var item = MainData.BagBaseTemp[i];
            if (item.IsChoose)
            {
                itemList.Items.Add(item.Index);
            }
        }

        if (itemList.Items.Count > 0)
        {
            NetManager.Instance.StartCoroutine(AstrologyLevelUp(bagid, index, itemList));
        }
    }

    public IEnumerator AstrologyLevelUp(int bagId, int bagIndex, Int32Array needList)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.AstrologyLevelUp(bagId, bagIndex, needList);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    Logger.Info("-----------AstrologyLevelUp ok ");
                }
                else
                {
                    if (msg.ErrorCode == (int) ErrorCodes.Error_ItemNoInBag_All)
                    {
                        var e = new ShowUIHintBoard(705); //test
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                    else
                    {
                        UIManager.Instance.ShowNetError(msg.ErrorCode);
                    }
                }
            }
            else
            {
                var e = new ShowUIHintBoard(705); //test
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    //升级界面返回
    public void BtnUpBack()
    {
        DetailData.ShowLevelUI = 0;

        TotalExp = 0;
        DetailData.DiamondLevel = DetailData.SelectedDia.Extra.Level.ToString();
        for (var i = 0; i < MainData.BagBaseTemp.Count; i++)
        {
            MainData.BagBaseTemp[i].IsChoose = false;
        }

        for (var i = 0; i < DetailData.TabPageShow.Count; i++)
        {
            DetailData.TabPageShow[i] = 0;
        }
        //RefleshBag(tab_Page);
    }

    //增加一个宝石的经验
    public bool AddOneExp(AstrologyDiamondDataModel item)
    {
        if (item.ID == -1)
        {
            return true;
        }
        var vartotal = 0;
        var tbGem = Table.GetGem(item.ID);
        if (MaxExp[tbGem.Quality] == -1)
        {
            MaxExp[tbGem.Quality] = GetTolalExp(tbGem.Quality, tbGem.MaxLevel + 1);
        }
        vartotal = GetTolalExp(tbGem.Quality, item.Extra.Level) + item.Extra.Exp + tbGem.InitExp;
        if (vartotal <= MaxExp[tbGem.Quality])
        {
            TotalExp += vartotal;
            return true;
        }
        return false;
    }

    //delete 一个宝石的经验
    public void DeleteOneExp(AstrologyDiamondDataModel item)
    {
        if (item.ID == -1)
        {
            return;
        }
        var tbGem = Table.GetGem(item.ID);
        TotalExp -= GetTolalExp(tbGem.Quality, item.Extra.Level) + item.Extra.Exp + tbGem.InitExp;
    }

    //增加所有经验
    public bool AddAllExp()
    {
        TotalExp = 0;
        for (var i = 0; i < MainData.BagBaseTemp.Count; i++)
        {
            var item = MainData.BagBaseTemp[i];
            if (item.IsChoose)
            {
                if (!AddOneExp(item))
                {
                    return false;
                }
            }
        }
        return true;
    }

    //计算将要到达的等级
    public void WillLevel(AstrologyDiamondDataModel item)
    {
        if (item.ID == -1)
        {
            //SetDiamondNull();
            return;
        }
        var nowExp = 0;
        var level = 0;
        var tbGem = Table.GetGem(item.ID);
        var tbGem2 = Table.GetGem(DetailData.SelectedDia.ID);
        var LeftExp = TotalExp - Table.GetLevelData(item.Extra.Level).Exp[tbGem.Quality] + item.Extra.Exp;
        if (LeftExp < 0)
        {
            nowExp = item.Extra.Exp + TotalExp;
            level = item.Extra.Level;
            DetailData.LessCount = nowExp;
            DetailData.MoreCount = Table.GetLevelData(level).Exp[tbGem2.Quality];
            DetailData.Percent = DetailData.LessCount/(float) DetailData.MoreCount;
            DetailData.DiamondLevel = DetailData.SelectedDia.Extra.Level + " -> " + level;
            return;
        }
        for (var i = item.Extra.Level + 1; i <= tbGem.MaxLevel; i++)
        {
            if (LeftExp - Table.GetLevelData(i).Exp[tbGem.Quality] < 0)
            {
                nowExp = LeftExp;
                level = i;
                DetailData.LessCount = nowExp;
                DetailData.MoreCount = Table.GetLevelData(level).Exp[tbGem2.Quality];
                DetailData.Percent = DetailData.LessCount/(float) DetailData.MoreCount;
                DetailData.DiamondLevel = DetailData.SelectedDia.Extra.Level + " -> " + level;
                return;
            }
            LeftExp -= Table.GetLevelData(i).Exp[tbGem.Quality];
            nowExp = LeftExp;
            level = i + 1;
        }
        if (level > tbGem.MaxLevel)
        {
            level = tbGem.MaxLevel;
            nowExp = Table.GetLevelData(tbGem.MaxLevel).Exp[tbGem.Quality];
        }
        DetailData.LessCount = nowExp;
        DetailData.MoreCount = Table.GetLevelData(level).Exp[tbGem2.Quality];
        DetailData.Percent = DetailData.LessCount/(float) DetailData.MoreCount;
        DetailData.DiamondLevel = DetailData.SelectedDia.Extra.Level + " -> " + level;
    }

    //获得所有经验
    public int GetTolalExp(int type, int level)
    {
        var exp = 0;

        for (var i = 1; i < level; ++i)
        {
            var tbExp = Table.GetLevelData(i);
            exp += tbExp.Exp[type];
        }
        return exp;
    }

    //重置选择的经验
    public void ResetSelectedExp()
    {
        TotalExp = 0;
        for (var i = 0; i < MainData.BagBaseTemp.Count; i++)
        {
            MainData.BagBaseTemp[i].IsChoose = false;
        }

        if (DetailData.SelectedDia.ID == -1)
        {
            DetailData.LessCount = 0;
            DetailData.MoreCount = 0;
            DetailData.Percent = 0;
            return;
        }
        var tbGem = Table.GetGem(DetailData.SelectedDia.ID);
        DetailData.LessCount = DetailData.SelectedDia.Extra.Exp;
        DetailData.MoreCount = Table.GetLevelData(DetailData.SelectedDia.Extra.Level).Exp[tbGem.Quality];
        DetailData.Percent = DetailData.LessCount/(float) DetailData.MoreCount;
        DetailData.DiamondLevel = DetailData.SelectedDia.Extra.Level.ToString();
    }

    //获取宝石类型
    public int GetDiamondType(int type)
    {
        switch (type)
        {
            case 50000:
            {
                return (int) DIAMOND_INDEX.Diamond;
            }
                break;
            case 51000:
            {
                return (int) DIAMOND_INDEX.Crystal;
            }
                break;
            case 52000:
            {
                return (int) DIAMOND_INDEX.Agate;
            }
                break;
            default:
                return -1; //(int) DIAMOND_INDEX.Diamond;
        }
    }

    #endregion

    #region 接口

    public FrameState State { get; set; }

    public void CleanUp()
    {
        if (DetailData != null)
        {
            DetailData.SelectColor.PropertyChanged -= OnToggleChange;
        }
        MainData = new AstrologyDataModel();
        DetailData = new AstrologyDetailDataModel();
        DetailData.SelectColor.PropertyChanged += OnToggleChange;
        MaxExp = new int[5] {-1, -1, -1, -1, -1};
    }

    public void RefreshData(UIInitArguments data)
    {
        var args = data as AstrologyArguments;
        if (args == null)
        {
            return;
        }
        if (args.BuildingData != null)
        {
            AstrBuilding = args.BuildingData;
        }
        InitUI();
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        switch (name)
        {
            case "MainData":
            {
                return MainData;
            }
                break;
            case "DetailData":
            {
                return DetailData;
            }
                break;
        }
        return null;
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
        if (name == "InitAstrologyData")
        {
            InitAstrologyData(param[0] as BagBaseData);
        }
        else if (name == "UpdateAstrologyData")
        {
            UpdateAstrologyData((int) param[0], param[1] as ItemsChangeData);
        }

        return null;
    }

    public void OnShow()
    {
        var e = new UIEvent_AstrologySetGridLookIndex();
        e.Type = 0;
        e.Index = AstrologyIndex;
        EventDispatcher.Instance.DispatchEvent(e);
    }

    //初始化宝石背包数据
    public void InitAstrologyData(BagBaseData bagBase)
    {
        var bagID = bagBase.BagId;
        if (bagID == (int) eBagType.GemBag)
        {
            MainData.BagBase.Clear();
            var tbBaseBase = Table.GetBagBase(bagID);
            for (var i = 0; i < tbBaseBase.MaxCapacity; i++)
            {
                var item = new AstrologyDiamondDataModel();
                item.Index = i;
                item.ID = -1;
                MainData.BagBase.Add(item);
            }

            for (var i = 0; i < tbBaseBase.MaxCapacity; i++)
            {
                var item = new AstrologyDiamondDataModel();
                item.ID = -1;
                MainData.BagBaseTemp.Add(item);
            }

            for (var i = 0; i < bagBase.Items.Count; i++)
            {
                var ii = bagBase.Items[i];
                MainData.BagBase[ii.Index].ID = ii.ItemId;
                if (ii.ItemId == -1)
                {
                    continue;
                }
                var tbGem = Table.GetGem(ii.ItemId);
                MainData.BagBase[ii.Index].DiaType = GetDiamondType(tbGem.Type);
                MainData.BagBase[ii.Index].Extra.InstallData(ii.Exdata);
            }
            if (!firstRun)
            {
                RefleshBag(tab_Page);
            }
        }
        else if (bagID == (int) eBagType.GemEquip)
        {
            MainData.MainList.Clear();
            var tbBaseBase = Table.GetBagBase(bagID);

            for (var i = 0; i < tbBaseBase.MaxCapacity/3; i++)
            {
                var ac = new AstrologyConstellationDataModel();
                ac.ConstellationName = AstrName[i];
                ac.Icon = 3; //test
                MainData.MainList.Add(ac);
            }
            for (var i = 0; i < bagBase.Items.Count; i++)
            {
                var item = bagBase.Items[i];
                var ii = MainData.MainList[item.Index%12].DiamondList[item.Index/12];
                ii.ID = item.ItemId;
                if (ii.ID == -1)
                {
                    continue;
                }
                var tbGem = Table.GetGem(ii.ID);
                ii.DiaType = GetDiamondType(tbGem.Type);
                ii.Extra.InstallData(item.Exdata);
            }
        }
    }

    //更新宝石背包
    public void UpdateAstrologyData(int bagid, ItemsChangeData bag)
    {
        if (bagid == (int) eBagType.GemBag)
        {
            {
                // foreach(var item in bag.ItemsChange)
                var __enumerator2 = (bag.ItemsChange).GetEnumerator();
                while (__enumerator2.MoveNext())
                {
                    var item = __enumerator2.Current;
                    {
                        var index = item.Value.Index;
                        MainData.BagBase[index].ID = item.Value.ItemId;
                        MainData.BagBase[index].Extra.InstallData(item.Value.Exdata);
                        if (MainData.BagBase[index].ID == -1)
                        {
                            continue;
                        }
                        var tbGem = Table.GetGem(MainData.BagBase[index].ID);
                        MainData.BagBase[index].DiaType = GetDiamondType(tbGem.Type);
                    }
                }
            }
            RefleshBag(tab_Page);
        }
        else if (bagid == (int) eBagType.GemEquip)
        {
            {
                // foreach(var item in bag.ItemsChange)
                var __enumerator3 = (bag.ItemsChange).GetEnumerator();
                while (__enumerator3.MoveNext())
                {
                    var item = __enumerator3.Current;
                    {
                        var index = item.Value.Index;
                        var ii = MainData.MainList[index%12].DiamondList[index/12];
                        ii.ID = item.Value.ItemId;
                        var tbGem = Table.GetGem(ii.ID);
                        if (ii.ID == -1)
                        {
                            continue;
                        }
                        ii.DiaType = GetDiamondType(tbGem.Type);
                        ii.Extra.InstallData(item.Value.Exdata);
                    }
                }
            }
            //SetBagInit();
        }
    }

    #endregion
}