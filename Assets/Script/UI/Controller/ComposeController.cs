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

#endregion

public class ComposeController : IControllerBase
{
    public ComposeController()
    {
        CleanUp();

        EventDispatcher.Instance.AddEventListener(ComposeMenuCellClick.EVENT_TYPE, OnClickMenuCellItem);
        EventDispatcher.Instance.AddEventListener(ComposeItemOnClick.EVENT_TYPE, OnClickComposeItem);
        EventDispatcher.Instance.AddEventListener(ShowComposFlag_Event.EVENT_TYPE, SetComposeFlag);

    }

    public ComposeUIDataModel DataModel;
    public Dictionary<int, List<ItemComposeRecord>> mComposeTable = new Dictionary<int, List<ItemComposeRecord>>();
    private bool mIsInit;
    public FrameState mState;

    public IEnumerator ComposeItemCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var composeCount = PlayerDataManager.Instance.GetExData((int)eExdataDefine.e415);
            var msg = NetManager.Instance.ComposeItem(DataModel.SelectIndex, 1);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int)ErrorCodes.OK)
                {
                    if (msg.Response == -1)
                    {
                        EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(454));
                    }
                    else
                    {
                        PlatformHelper.Event("city", "compose", DataModel.SelectIndex);
                        //合成前几次获得经验
                        //                         if (BuildingData != null)
                        //                         {
                        //                             var tbBuilding = Table.GetBuilding(BuildingData.TypeId);
                        //                             var tbServer = Table.GetBuildingService(tbBuilding.ServiceId);
                        //                             if (composeCount < tbServer.Param[3])
                        //                             {
                        //                                 EventDispatcher.Instance.DispatchEvent(new UIEvent_ComposeFlyAnim(tbServer.Param[2]));
                        //                             }
                        //                         }
                        EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(453));
                    }
                    EventDispatcher.Instance.DispatchEvent(new ComposeItemEffectEvent(msg.Response != -1));
                }
                else if (msg.ErrorCode == (int)ErrorCodes.Error_ItemNoInBag_All)
                {
                    var e = new ShowUIHintBoard(300116);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else if (msg.ErrorCode == (int)ErrorCodes.Error_ItemComposeID
                         || msg.ErrorCode == (int)ErrorCodes.ItemNotEnough
                         || msg.ErrorCode == (int)ErrorCodes.MoneyNotEnough)
                {
                    var e = new ShowUIHintBoard(msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else
                {
                    var e = new ShowUIHintBoard(msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(e);
                    Logger.Debug("ComposeItem..................." + msg.ErrorCode);
                }
            }
            else
            {
                Logger.Debug("ComposeItem..................." + msg.State);
            }
        }
    }

    public void Init()
    {
        if (mComposeTable.Count == 0)
        {
            Table.ForeachItemCompose(recoard =>
            {
                var type = recoard.Type;
                if (type == 0)
                {
                    return true;
                }
                List<ItemComposeRecord> list = null;
                if (!mComposeTable.TryGetValue(type, out list))
                {
                    list = new List<ItemComposeRecord>();
                    mComposeTable[type] = list;
                }
                list.Add(recoard);

                return true;
            });
        }
        if (DataModel.MenuState == null)
        {
            DataModel.MenuState = new Dictionary<int, bool>();
            var maxType = (int)eComposeType.Count;
            for (var i = (int)eComposeType.Ticket; i < maxType; i++)
            {
                DataModel.MenuState.Add(i, i == 0);
            }
        }


    }

    public void SetComposeFlag(IEvent e)
    {

        var tbCompose = Table.GetItemCompose(DataModel.SelectIndex);
        for (var i = 0; i < 4; i++)
        {
            if (tbCompose.NeedId[i] != -1)
            {
                if (PlayerDataManager.Instance.GetItemCount(tbCompose.NeedId[i]) > tbCompose.NeedCount[i])
                {
                    PlayerDataManager.Instance.NoticeData.ComposeNotice = true;
                    //return;
                }
                else
                {
                    PlayerDataManager.Instance.NoticeData.ComposeNotice = false;
                }
            }
        }
    }

    public void OnClickComposeItem(IEvent ievent)
    {
        var playerData = PlayerDataManager.Instance.PlayerDataModel;
        var tbCompose = Table.GetItemCompose(DataModel.SelectIndex);
        if (DataModel.PermitLevel != -1)
        {
            var str = string.Format(GameUtils.GetDictionaryText(300908), DataModel.PermitLevel);
            GameUtils.ShowHintTip(str);
            return;
        }
        if (PlayerDataManager.Instance.GetRemaindCapacity(eBagType.BaseItem) == 0)
        {
            //
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300116));
            return;
        }

        for (var i = 0; i < 4; i++)
        {
            if (tbCompose.NeedId[i] != -1)
            {
                if (PlayerDataManager.Instance.GetItemCount(tbCompose.NeedId[i]) < tbCompose.NeedCount[i])
                {
                    //"材料不足"
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210101));
                    return;
                }
            }
        }
        switch (tbCompose.NeedRes)
        {
            case 2:
                {
                    if (tbCompose.NeedValue > playerData.Bags.Resources.Gold)
                    {
                        //"金钱不足"
                        EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210100));
                        return;
                    }
                }
                break;
            case 3:
                {
                    if (tbCompose.NeedValue > playerData.Bags.Resources.Diamond)
                    {
                        //"钻石不足"
                        EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210102));
                        return;
                    }
                }
                break;
        }

        NetManager.Instance.StartCoroutine(ComposeItemCoroutine());
    }

    public void OnClickMenuCellItem(IEvent ievent)
    {
        var e = ievent as ComposeMenuCellClick;
        var clickMenu = e.MenuData;

        var selectSubMenu = -1;
        if (clickMenu.Type == 0)
        {
            var dic = new Dictionary<int, bool>();
            if (DataModel.MenuState[clickMenu.TableId] == false)
            {
                {
                    // foreach(var b in DataModel.MenuState)
                    var __enumerator1 = (DataModel.MenuState).GetEnumerator();
                    while (__enumerator1.MoveNext())
                    {
                        var b = __enumerator1.Current;
                        {
                            dic.Add(b.Key, b.Key == clickMenu.TableId);
                        }
                    }
                }
            }
            else
            {
                {
                    // foreach(var b in DataModel.MenuState)
                    var __enumerator2 = (DataModel.MenuState).GetEnumerator();
                    while (__enumerator2.MoveNext())
                    {
                        var b = __enumerator2.Current;
                        {
                            dic.Add(b.Key, false);
                        }
                    }
                }
            }
            DataModel.MenuState = dic;
            RefreshSelectReset();
        }
        else
        {
            selectSubMenu = clickMenu.TableId;
            RefreshSelect(selectSubMenu, clickMenu);
        }
    }

    public void RefreshSelect(int selectIndex, ComposeMenuDataModel clickMenu)
    {
        SetSelectIndex(selectIndex, clickMenu);
        {
            // foreach(var menuData in DataModel.MenuDatas)
            var enumerator4 = (DataModel.MenuDatas).GetEnumerator();
            while (enumerator4.MoveNext())
            {
                var menuData = enumerator4.Current;
                {
                    if (menuData.Type == 1)
                    {
                        if (menuData.TableId == selectIndex)
                        {
                            menuData.IsOpen = 1;
                        }
                        else
                        {
                            menuData.IsOpen = 0;
                        }
                    }
                }
            }
        }
    }

    //private Dictionary<int, int> mLimitCounts = new Dictionary<int, int>();
    //private Dictionary<int,List<int>> mAllLimit = new Dictionary<int, List<int>>(); 
    public void RefreshSelectReset()
    {
        DataModel.MenuDatas.Clear();
       

        var list = new List<ComposeMenuDataModel>();
        var roleId = PlayerDataManager.Instance.GetRoleId();
        {
            // foreach(var b in DataModel.MenuState)
            var __enumerator3 = (DataModel.MenuState).GetEnumerator();
            while (__enumerator3.MoveNext())
            {
                var b = __enumerator3.Current;
                {
                    var menu = new ComposeMenuDataModel();
                    menu.Type = 0;
                    menu.TableId = b.Key;
                    var type = (eComposeType)b.Key;

                    //                     if (type != eComposeType.Ticket && DataModel.NeedBack == false)
                    //                     {
                    // //NPC的合成服务 去掉技能书的合成页
                    //                         continue;
                    //                     }

                    switch (type)
                    {
                        case eComposeType.Ticket:
                            {
                                //门票
                                menu.TypeName = GameUtils.GetDictionaryText(540);
                            }
                            break;

                        case eComposeType.Rune:
                            {
                                //属性符文
                                menu.TypeName = GameUtils.GetDictionaryText(541);
                            }
                            break;
                        case eComposeType.SkillBook:
                            {
                                //技能书
                                menu.TypeName = GameUtils.GetDictionaryText(542);
                            }
                            break;
                        case eComposeType.SkillPiece:
                            {
                                //技能残章
                                menu.TypeName = GameUtils.GetDictionaryText(543);
                                //menu.TypeName = "技能残章";
                            }
                            break;
                        case eComposeType.MayaShenQi:
                        {
                            menu.TypeName = GameUtils.GetDictionaryText(100002114);
                        }
                            break;
                    }
                    list.Add(menu);

                    //                     int count = -1;
                    //                     if (!mLimitCounts.TryGetValue(b.Key, out count))
                    //                     {
                    //                         count = -1;
                    //                     }
                    var level = PlayerDataManager.Instance.GetRes((int)eResourcesType.LevelRes);
                    if (b.Value)
                    {
                        menu.IsOpen = 1;
                        var index = 0;
                        List<ItemComposeRecord> itemList = null;
                        if (mComposeTable.TryGetValue(menu.TableId, out itemList))
                        {
                            {
                                var __list6 = itemList;
                                var __listCount6 = __list6.Count;
                                var select = -1;
                                ComposeMenuDataModel subMenuRef = null;
                                for (var __i6 = 0; __i6 < __listCount6; ++__i6)
                                {
                                    var record = __list6[__i6];
                                    {
                                        var lv = -1;
                                        if (level < record.ComposeOpenLevel)
                                        {
                                            lv = record.ComposeOpenLevel;
                                        }
                                        else
                                        {
                                            lv = -1;
                                        }
                                        if (!BitFlag.GetLow(record.SortByCareer, roleId))
                                        {
                                            continue;
                                        }
                                        var subMenu = new ComposeMenuDataModel();
                                        subMenu.PermitLevel = lv;
                                        if ((DataModel.ShowId == -1 && index == 0) || DataModel.ShowId == record.Id)
                                        {
                                            select = record.Id;
                                            SetSelectIndex(record.Id, subMenu);
                                            subMenu.IsOpen = 1;
                                        }
                                        else
                                        {
                                            subMenu.IsOpen = 0;
                                            if (subMenuRef == null)
                                            {
                                                subMenuRef = subMenu;
                                            }
                                        }
                                        index++;
                                        subMenu.Type = 1;
                                        subMenu.TableId = record.Id;
                                        subMenu.ItemId = Table.GetItemCompose(record.Id).ComposeView;
                                        list.Add(subMenu);
                                    }
                                }

                                if (select == -1 && subMenuRef != null)
                                {
                                    SetSelectIndex(subMenuRef.TableId, subMenuRef);
                                    subMenuRef.IsOpen = 1;
                                }
                            }
                        }
                    }
                }
            }
        }
        DataModel.MenuDatas = new ObservableCollection<ComposeMenuDataModel>(list);
    }

    public void SetSelectIndex(int index, ComposeMenuDataModel clickMenu)
    {
        DataModel.PermitLevel = clickMenu.PermitLevel;
        //if (DataModel.PermitLevel != -1)
        //{
        //    DataModel.PermitLevel ++;
        //}
        DataModel.SelectIndex = index;
        if (index == -1)
        {
            return;
        }
        var tbCompose = Table.GetItemCompose(DataModel.SelectIndex);
        var count = 0;
        for (var i = 0; i < tbCompose.NeedId.Count; i++)
        {
            if (tbCompose.NeedId[i] != -1)
            {
                count++;
            }
        }
        DataModel.ConsumeCount = count;
    }

    public void CleanUp()
    {
        DataModel = new ComposeUIDataModel();
        mComposeTable.Clear();
        mIsInit = false;
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

    public void RefreshData(UIInitArguments data)
    {
        if (mIsInit == false)
        {
            mIsInit = true;
            Init();
        }

        var args = data as ComposeArguments;

        if (args != null)
        {
            if (args.BuildingData != null)
            {
                //BuildingData = args.BuildingData;
            }
            else
            {
            }
            DataModel.ShowId = args.Tab;
        }
        else
        {
            DataModel.ShowId = -1;

        }
        /*
        if (BuildingData != null)
        {
            var tbBuliding = Table.GetBuilding(BuildingData.TypeId);
            var tbBulidingServer = Table.GetBuildingService(tbBuliding.ServiceId);
//             mLimitCounts.Clear();
// 
//             for (int i = 1; i < 4; i++)
//             {
//                 if (tbBulidingServer.Param[i * 2] != -1)
//                 {
//                     mLimitCounts.Add(tbBulidingServer.Param[i * 2], tbBulidingServer.Param[i * 2 + 1]);
//                 }
//             }
            var add = tbBulidingServer.Param[0]/100;

            var ret = CityPetSkill.GetBSParamByIndex((BuildingType) tbBuliding.Type, tbBulidingServer, 0,
                BuildingData.PetList);
            DataModel.Add = add + ret;
        }
        else
        {
//             mLimitCounts.Clear();
//             var tbBulidingServer = Table.GetBuildingService(1000);
// 
//             for (int i = 1; i < 4; i++)
//             {
//                 if (tbBulidingServer.Param[i * 2] != -1)
//                 {
//                     mLimitCounts.Add(tbBulidingServer.Param[i * 2], tbBulidingServer.Param[i * 2 + 1]);
//                 }
//             }
            DataModel.Add = 0;
        }
        */
        var showtype = eComposeType.Ticket;
        if (DataModel.ShowId != -1)
        {
            var tbCompose = Table.GetItemCompose(DataModel.ShowId);
            if (tbCompose != null)
            {
                showtype = (eComposeType)tbCompose.Type;
            }
        }

        var maxType = (int)eComposeType.Count;
        for (var i = (int)eComposeType.Ticket; i < maxType; i++)
        {
            DataModel.MenuState[i] = i == (int)showtype;
        }
        RefreshSelectReset();
    }

    public void Close()
    {
    }

    public void Tick()
    {
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public FrameState State
    {
        get { return mState; }
        set { mState = value; }
    }
}