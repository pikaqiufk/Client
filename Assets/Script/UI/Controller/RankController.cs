#region using

using System.Collections;
using System.ComponentModel;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ScorpionNetLib;

#endregion

public class RankController : IControllerBase
{
    public RankController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(ExDataInitEvent.EVENT_TYPE, OnInitExData);
        EventDispatcher.Instance.AddEventListener(RankCellClick.EVENT_TYPE, OnClicRankCell);
        EventDispatcher.Instance.AddEventListener(RankOperationEvent.EVENT_TYPE, OnRankOperation);
    }

    public RankDataModel DataModel;

    public void ApplyRankList(int rankType)
    {
        NetManager.Instance.StartCoroutine(ApplyRankListCoroutine(rankType));
    }

    public IEnumerator ApplyRankListCoroutine(int rankType)
    {
        using (new BlockingLayerHelper(0))
        {
            var serverId = PlayerDataManager.Instance.ServerId;
            var msg = NetManager.Instance.GetRankList(serverId, rankType);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    InitRankData(msg.Response);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error("GetRankList Error!............ErrorCode..." + msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("GetRankList Error!............State..." + msg.State);
            }
        }
    }

    public void InitRankData(RankList list)
    {
        var flag = 0;
        DataModel.SelfRank = -1;
        var type = list.RankType;
        DataModel.RandLists[type].Clear();
        var selfGuid = ObjManager.Instance.MyPlayer.GetObjId();
        {
            var __list1 = list.RankData;
            var __listCount1 = __list1.Count;
            for (var __i1 = 0; __i1 < __listCount1; ++__i1)
            {
                var one = __list1[__i1];
                {
                    one.Name = GameUtils.ServerStringAnalysis(one.Name);
                    flag++;
                    var cell = new RankCellDataModel
                    {
                        CharacterId = one.Id,
                        Name = one.Name,
                        Value = one.Value,
                        Id = flag
                    };
                    if (flag == 1)
                    {
                        cell.IsSel = true;
                        DataModel.SelectCellData = cell;
                        PlayerDataManager.Instance.ApplyPlayerInfo(cell.CharacterId, RefresCharacter);
                    }
                    else
                    {
                        cell.IsSel = false;
                    }
                    //ranklist.Add(cell);
                    DataModel.RandLists[type].Add(cell);
                    if (cell.CharacterId == selfGuid)
                    {
                        DataModel.SelfRank = flag;
                    }
                }
            }
        }
        DataModel.SelectList = DataModel.RandLists[type];
    }

    public void OnChangeShowPage(object sender, PropertyChangedEventArgs e)
    {
        for (var i = 0; i < DataModel.ShowPages.Count; i++)
        {
            if (DataModel.ShowPages[i])
            {
                ApplyRankList(i);
            }
        }
    }

    public void OnClicRankCell(IEvent ievent)
    {
        var e = ievent as RankCellClick;
        var index = e.Index;
        RefresRankCell(index);
    }

    public void OnInitExData(IEvent ievent)
    {
        DataModel.WorshipCountMax = Table.GetExdata(312).InitValue;
        RefreshWorshipCount();
        if (DataModel.SelfWorshipCount < DataModel.WorshipCountMax)
        {
            PlayerDataManager.Instance.NoticeData.RankingCanLike = true;
        }
    }

    public void OnRankOperation(IEvent ievent)
    {
        var e = ievent as RankOperationEvent;
        switch (e.Type)
        {
            case 1:
            {
                var charId = DataModel.SelectCellData.CharacterId;
                if (charId == ObjManager.Instance.MyPlayer.GetObjId())
                {
                    var e1 = new Show_UI_Event(UIConfig.CharacterUI);
                    EventDispatcher.Instance.DispatchEvent(e1);
                    return;
                }
                PlayerDataManager.Instance.ShowPlayerInfo(DataModel.SelectCellData.CharacterId);
            }
                break;
            case 2:
            {
                WorshipCharacter();
            }
                break;
            case 3:
            {
                RefreshWorshipCount();
            }
                break;
        }
    }

    public void RefresCharacter(PlayerInfoMsg info)
    {
        if (DataModel.ShowPages[6])
        {
            var e = new RankRefreshModelView(info, true);
            DataModel.TargetWorshipCount = info.WorshipCount;
            EventDispatcher.Instance.DispatchEvent(e);
        }
        else
        {
            var e = new RankRefreshModelView(info, false);
            DataModel.TargetWorshipCount = info.WorshipCount;
            EventDispatcher.Instance.DispatchEvent(e);
        }
    }

    public void RefreshWorshipCount()
    {
        var exdata = PlayerDataManager.Instance.GetExData(312);
        DataModel.SelfWorshipCount = DataModel.WorshipCountMax - exdata;
    }

    public void RefresRankCell(int index)
    {
        var DataModelShowPagesCount0 = DataModel.ShowPages.Count;
        for (var i = 0; i < DataModelShowPagesCount0; i++)
        {
            if (DataModel.ShowPages[i])
            {
                var list = DataModel.RandLists[i];
                var listCount1 = list.Count;
                for (var j = 0; j < listCount1; j++)
                {
                    list[j].IsSel = j == index;
                    if (list[j].IsSel)
                    {
                        DataModel.SelectCellData = list[j];
                    }
                }
                break;
            }
        }

        var characterId = DataModel.SelectCellData.CharacterId;
        PlayerDataManager.Instance.ApplyPlayerInfo(characterId, RefresCharacter);
    }

    public void WorshipCharacter()
    {
        var charId = DataModel.SelectCellData.CharacterId;
        if (charId == ObjManager.Instance.MyPlayer.GetObjId())
        {
            var e = new ShowUIHintBoard(220501);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }
        if (DataModel.SelfWorshipCount >= DataModel.WorshipCountMax)
        {
            //已经没有崇拜次数了
            var e = new ShowUIHintBoard(3000002);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }


        NetManager.Instance.StartCoroutine(WorshipCharacterCoroutine());
    }

    public IEnumerator WorshipCharacterCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var charId = DataModel.SelectCellData.CharacterId;
            var msg = NetManager.Instance.WorshipCharacter(charId);
            var count = DataModel.SelfWorshipCount;
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    DataModel.TargetWorshipCount++;
                    var playerInfo = PlayerDataManager.Instance.GetCharacterSimpleInfo(charId);
                    if (playerInfo != null)
                    {
                        playerInfo.WorshipCount++;

                        var animationId = GameUtils.RankWorshipAction[playerInfo.TypeId];
                        var e = new RankNotifyLogic(1, animationId);
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                    if (DataModel.SelfWorshipCount != count + 1)
                    {
                        DataModel.SelfWorshipCount = count + 1;
                    }
                    if (DataModel.SelfWorshipCount >= DataModel.WorshipCountMax)
                    {
                        PlayerDataManager.Instance.NoticeData.RankingCanLike = false;
                    }
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_CharacterSame)
                {
                    var e = new ShowUIHintBoard(220501);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_WorshipAlready)
                {
                    var e = new ShowUIHintBoard(3000001);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_WorshipCount)
                {
                    var e = new ShowUIHintBoard(3000002);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error("WorshipCharacter Error!............ErrorCode..." + msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("WorshipCharacter Error!............State..." + msg.State);
            }
        }
    }

    public void Close()
    {
        //         for (int i = 0; i < 4; i++)
        //         {
        //             DataModel.ShowPages[i] = false;
        //         }
    }

    public void Tick()
    {
    }

    public void CleanUp()
    {
        if (DataModel != null)
        {
            DataModel.ShowPages.PropertyChanged -= OnChangeShowPage;
        }
        DataModel = new RankDataModel();

        DataModel.ShowPages.PropertyChanged += OnChangeShowPage;
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
        var count = DataModel.ShowPages.Count;
        if (count == 0)
        {
            return;
        }
        for (var i = 0; i < count; i++)
        {
            DataModel.ShowPages[i] = false;
        }
        DataModel.ShowPages[0] = true;
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public FrameState State { get; set; }
}