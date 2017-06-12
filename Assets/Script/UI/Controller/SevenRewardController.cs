#region using

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ClientDataModel;
using ClientService;
using DataTable;
using EventSystem;
using ScorpionNetLib;

#endregion

public class SevenRewardController : IControllerBase
{
    //public SevenRewardController DataModel;
    public SevenRewardController()
    {
        EventDispatcher.Instance.AddEventListener(UIEvent_SevenRewardInit.EVENT_TYPE, InitData);
        EventDispatcher.Instance.AddEventListener(UIEvent_SevenRewardItemClick.EVENT_TYPE, ItemCellClick);
        CleanUp();
    }

    public SevenRewardDataModel DataModel;

    public enum SevenRewardState
    {
        NotCanGet = 0, //不可领取
        CanGet = 1, //可领取未领取
        HasGot = 2 //已经领取
    }

    private void AnalysisNotice()
    {
        var count = DataModel.Lists.Count;
        var isOk = false;
        for (var i = 0; i < count; i++)
        {
            var item = DataModel.Lists[i];
            if (item.State == (int) SevenRewardState.CanGet)
            {
                isOk = true;
            }
        }
        PlayerDataManager.Instance.NoticeData.SevenDay = isOk;
    }

    private IEnumerator ClaimRewardCoroutine(int type, int id, int index)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ActivationReward(type, id);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    DataModel.Lists[index].State = (int) SevenRewardState.HasGot;
                    AnalysisNotice();
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error(".....MatchingCancel.......{0}.", msg.ErrorCode);
                }
            }
        }
    }

    private void InitData(IEvent ievent)
    {
        var list = new List<SevenRewardItemDataModel>();
        var loginDay = PlayerDataManager.Instance.GetExData(eExdataDefine.e94);
        var flagData = PlayerDataManager.Instance.FlagData;
        Table.ForeachGift(table =>
        {
            if (table.Type == (int) eRewardType.SevenDayReward)
            {
                var canGet = false;
                var item = new SevenRewardItemDataModel();

                item.Day = table.Param[0];
                canGet = (item.Day <= loginDay);
                for (var i = 0; i < 3; i++)
                {
                    item.Rewards[i].ItemId = table.Param[i*2 + 1];
                    item.Rewards[i].Count = table.Param[i*2 + 2];
                }
                item.Rewards[3].ItemId = table.Param[7];
                item.Rewards[3].Count = 1;

                if (canGet)
                {
                    if (flagData.GetFlag(table.Flag) == 1)
                    {
                        item.State = (int) SevenRewardState.HasGot;
                    }
                    else
                    {
                        item.State = (int) SevenRewardState.CanGet;
                    }
                }
                else
                {
                    item.State = (int) SevenRewardState.NotCanGet;
                }
                item.TableId = table.Id;
                list.Add(item);
            }
            return true;
        });
        DataModel.Lists = new ObservableCollection<SevenRewardItemDataModel>(list);
        AnalysisNotice();
    }

    private void ItemCellClick(IEvent ievent)
    {
        var e = ievent as UIEvent_SevenRewardItemClick;
        var selectItem = DataModel.Lists[e.Index];
        NetManager.Instance.StartCoroutine(ClaimRewardCoroutine((int) eActivationRewardType.TableGift,
            selectItem.TableId, e.Index));
    }

    public void CleanUp()
    {
        DataModel = new SevenRewardDataModel();
    }

    public void RefreshData(UIInitArguments data)
    {
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