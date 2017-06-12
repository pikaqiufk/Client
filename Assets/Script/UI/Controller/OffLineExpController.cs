#region using

using System;
using System.Collections;
using System.ComponentModel;
using ClientDataModel;
using ClientService;
using DataTable;
using EventSystem;
using ScorpionNetLib;

#endregion

public class OffLineExpController : IControllerBase
{
    //public DateTime _updateExp = DateTime.Now;
    public OffLineExpController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(UI_Event_OffLineExp.EVENT_TYPE, BeginExercisesBtnEvent);
        EventDispatcher.Instance.AddEventListener(UiEventChangeOutLineTime.EVENT_TYPE, ChangeOnLineTime);
        EventDispatcher.Instance.AddEventListener(Enter_Scene_Event.EVENT_TYPE, EnterGameLastTime);
        EventDispatcher.Instance.AddEventListener(UI_Event_IsExercising.EVENT_TYPE, IsExercising);
        //EventDispatcher.Instance.AddEventListener(Ui_OffLineFrame_SetVisible.EVENT_TYPE, SetFrameVisibleEvent);
    }

    public OffLineExpDataModel DataModel = new OffLineExpDataModel();
    public DateTime deltaTime = DateTime.Now;
    public bool IsEnterScene;
    public DateTime LastOpenOffLineUITime = DateTime.MinValue;
    public int OffLineExp;
    //public DateTime _updatetime = DateTime.Now;
    public DateTime OpenOffLineUITime = DateTime.MaxValue;
    public long OutLineExp;
    //   public static readonly int OutLineLevelMin = Table.GetClientConfig(581).ToInt(); //离线经验最大值
    public int OutLineExpRef; //离线经验修正比例
    public int OutLineGemRef; //离线经验钻石比例
    public int OutLineGoldRef; //离线经验金钱比例
    public int OutLineOpenLevel; //离线经验开启等级
    private bool RunTick;

    public DateTime OutLineTime
    {
        get { return DateTime.FromBinary(DataModel.LastTime); }
        set { DataModel.LastTime = value.ToBinary(); }
    }

    public void ApplyOffLineExp()
    {
        if (IsEnterScene)
        {
            NetManager.Instance.StartCoroutine(ApplyOffLineExpCoroutine());
        }
    }

    public IEnumerator ApplyOffLineExpCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ApplyLeaveExp(0);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    OffLineExp = msg.Response.Exp;
                    OutLineTime = Extension.FromServerBinary(msg.Response.Time);
                    RefreshData(new UIInitArguments());
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
        }
    }

    public void BeginExercisesBtnEvent(IEvent ievent)
    {
        var e = ievent as UI_Event_OffLineExp;
        var needcount = 0;
        var tbLevel = Table.GetLevelData(GetLevel());
        if (tbLevel == null)
        {
            return;
        }
        var tbVip = PlayerDataManager.Instance.TbVip;
        RefreshData(new UIInitArguments());
        switch (e.Type)
        {
            case 3: //刷新经验用。
                break;
            case 0:
            {
                OffLineExp = OffLineExp + GetNowLeaveExp();
                OutLineTime = Game.Instance.ServerTime.AddMinutes(1);
                DataModel.IsExerciseing = false;

                NetManager.Instance.StartCoroutine(GetLeaveExpCoroutine(e.Type, needcount));
                return;
            }
            //钻石数量
            case 2:
            {
                if (tbVip.Muse4Reward == 0)
                {
//引导购买，提升vip
                    do
                    {
                        tbVip = Table.GetVIP(tbVip.Id + 1);
                    } while (tbVip.Muse4Reward == 0);
                    GameUtils.GuideToBuyVip(tbVip.Id);
                    return;
                }
                var myDiamond = PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Diamond;
                var need = DataModel.Gem;
                if (myDiamond < need)
                {
                    EventDispatcher.Instance.DispatchEvent(
                        new ShowUIHintBoard(string.Format(GameUtils.GetDictionaryText(210102))));
                    return;
                }
                NetManager.Instance.StartCoroutine(GetLeaveExpCoroutine(e.Type, need));
                break;
            }
            //金币数量
            case 1:
            {
                if (tbVip.Muse2Reward == 0)
                {
//引导购买，提升vip
                    do
                    {
                        tbVip = Table.GetVIP(tbVip.Id + 1);
                    } while (tbVip.Muse2Reward == 0);
                    GameUtils.GuideToBuyVip(tbVip.Id);
                    return;
                }
                var mymoney = PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Gold;
                if (mymoney < DataModel.Gold)
                {
                    EventDispatcher.Instance.DispatchEvent(
                        new ShowUIHintBoard(string.Format(GameUtils.GetDictionaryText(210100))));
                    return;
                }
                NetManager.Instance.StartCoroutine(GetLeaveExpCoroutine(e.Type, DataModel.Gold));
                break;
            }
        }
    }

    public void ChangeOnLineTime(IEvent ievent)
    {
        DataModel.EnterGameLastTime = Game.Instance.ServerTime.ToBinary();
        OpenOffLineUITime = Game.Instance.ServerTime.AddMinutes(1f);
        SetFrameViaible(false);
    }

    public void ClearLasttime()
    {
        DataModel.EnterGameLastTime = Game.Instance.ServerTime.ToBinary();
        OpenOffLineUITime = Game.Instance.ServerTime.AddMinutes(1f);
        DataModel.IsExerciseing = false;
    }

    public void EnterGameLastTime(IEvent ievent)
    {
        DataModel.EnterGameLastTime = Game.Instance.ServerTime.ToBinary();
        IsEnterScene = true;
        OpenOffLineUITime = Game.Instance.ServerTime.AddMinutes(1f);
        GameLogic.Instance.StartCoroutine(ApplyOffLineExpCoroutine());
    }

    public IEnumerator GetLeaveExpCoroutine(int Type, int needcount)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.GetLeaveExp(Type, needcount);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    OutLineTime = Game.Instance.ServerTime.AddMinutes(1);
                    var tbLevel = Table.GetLevelData(GetLevel());
                    if (tbLevel == null)
                    {
                        yield break;
                    }
                    OffLineExp = 0;
                    DataModel.Exp = GetNowLeaveExp();
                    DataModel.ExercisesTime = 0;
                    DataModel.ExercisesiString = (DataModel.ExercisesTime) + "%";
                    PlayerDataManager.Instance.NoticeData.OffLine = false;
                    RunTick = true;
                    DataModel.Gold = (int) (OutLineGoldRef*(float) DataModel.Exp/tbLevel.LeaveExpBase);
                    var gemneed = OutLineGemRef*(float) DataModel.Exp/tbLevel.LeaveExpBase;
                    if (gemneed > 0 && gemneed < 1)
                    {
                        DataModel.Gem = 1;
                    }
                    else
                    {
                        DataModel.Gem = (OutLineGemRef*DataModel.Exp/tbLevel.LeaveExpBase);
                    }
                    SetFrameViaible(false);
                }
                else
                {
                    if (msg.ErrorCode == (int) ErrorCodes.Error_TimeNotOver)
                    {
                        var info = GameUtils.GetDictionaryText(200002051);
                        UIManager.Instance.ShowMessage(MessageBoxType.Ok, info, "");
                        yield break;
                    }
                    if (msg.ErrorCode == (int) ErrorCodes.CharacterLevelMax)
                    {
                        var info = GameUtils.GetDictionaryText(300117);
                        UIManager.Instance.ShowMessage(MessageBoxType.Ok, info, "");
                        yield break;
                    }
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
        }
    }

    public int GetLevel()
    {
        return PlayerDataManager.Instance.GetLevel();
    }

    public int GetNowLeaveExp()
    {
        if (GetLevel() < OutLineOpenLevel)
        {
            return 0;
        }
        var seconds = (Game.Instance.ServerTime - OutLineTime).TotalSeconds;
        var minutes = (int) seconds/60;
        if (seconds >= 60)
        {
            var tbLevel = Table.GetLevelData(GetLevel());
            if (tbLevel == null)
            {
                return 0;
            }
            if (OutLineExp >= tbLevel.LeaveExpBase)
            {
                return tbLevel.LeaveExpBase;
            }
            var addExp = OutLineExp + 1.0f*tbLevel.DynamicExp*OutLineExpRef*minutes/10000;
            if (addExp > tbLevel.LeaveExpBase)
            {
                return tbLevel.LeaveExpBase;
            }
            return (int) addExp;
        }
        return (int) OutLineExp;
    }

    public void Init()
    {
        OutLineExpRef = Table.GetClientConfig(580).ToInt(); //离线经验修正比例
        OutLineGoldRef = Table.GetClientConfig(582).ToInt(); //离线经验金钱比例
        OutLineGemRef = Table.GetClientConfig(583).ToInt(); //离线经验钻石比例
        OutLineOpenLevel = Table.GetClientConfig(104).ToInt(); //离线经验开启等级
    }

    public void IsExercising(IEvent ievent)
    {
        DataModel.IsExerciseing = true;
    }

    public void OverOutLineTrigger(int minutes)
    {
        var tbLevel = Table.GetLevelData(GetLevel());
        if (tbLevel == null)
        {
            return;
        }
        if (OutLineExp >= tbLevel.LeaveExpBase)
        {
            return;
        }
        var addExp = OutLineExp + 1.0f*tbLevel.DynamicExp*OutLineExpRef*minutes/10000;
        if (addExp > tbLevel.LeaveExpBase)
        {
            OutLineExp = tbLevel.LeaveExpBase;
        }
        else
        {
            OutLineExp = (int) addExp;
        }
    }

    public void SetFrameViaible(bool b)
    {
        if (b)
        {
            LastOpenOffLineUITime = Game.Instance.ServerTime.AddMinutes(10);
            ApplyOffLineExp();
        }
        else
        {
            ClearLasttime();
        }
    }

    public void OnShow()
    {
    }

    public void CleanUp()
    {
        DataModel = new OffLineExpDataModel();
        Init();
    }

    public void RefreshData(UIInitArguments data)
    {
        var tbLevel = Table.GetLevelData(GetLevel());
        if (tbLevel == null)
        {
            return;
        }
        DataModel.Exp = OffLineExp + GetNowLeaveExp() > 0 ? OffLineExp + GetNowLeaveExp() : 0;
        DataModel.Exp = DataModel.Exp > tbLevel.LeaveExpBase ? tbLevel.LeaveExpBase : DataModel.Exp;
        DataModel.ExercisesTime = (float) DataModel.Exp/tbLevel.LeaveExpBase;
        DataModel.ExercisesiString = (DataModel.ExercisesTime*100).ToString("0.00") + "%";
        DataModel.Gold = (int) (OutLineGoldRef*(float) DataModel.Exp/tbLevel.LeaveExpBase);
        if (DataModel.ExercisesTime >= GameUtils.OfflineExpRatelimit)
        {
            if (PlayerDataManager.Instance.GetLevel() < GameUtils.MaxLevel)
            {
                PlayerDataManager.Instance.NoticeData.OffLine = true;
                RunTick = false;
            }
        }
        else
        {
            RunTick = true;
        }
        var gemNeed = OutLineGemRef*(float) DataModel.Exp/tbLevel.LeaveExpBase;
        if (gemNeed > 0 && gemNeed < 1)
        {
            DataModel.Gem = 1;
        }
        else
        {
            DataModel.Gem = OutLineGemRef*DataModel.Exp/tbLevel.LeaveExpBase;
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
        if (RunTick)
        {
            if (GetLevel() < OutLineOpenLevel)
            {
                return;
            }
            if (IsEnterScene && Game.Instance.ServerTime >= OpenOffLineUITime)
            {
                if (deltaTime > DateTime.Now)
                {
                    return;
                }
                deltaTime = DateTime.Now.AddSeconds(60);
                RefreshData(new UIInitArguments());

                if (Game.Instance.ServerTime >= LastOpenOffLineUITime)
                {
                    SetFrameViaible(true); //请求数据
                }
            }
        }
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