#region using

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ClientDataModel;
using ClientService;
using DataContract;
using EventSystem;
using ScorpionNetLib;

#endregion

public class MailController : IControllerBase
{
    private static readonly MailCellData EmptyMailCellData = new MailCellData
    {
        InfoData = new MailInfoData()
    };

    public MailController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(MailSyncEvent.EVENT_TYPE, OnMailSync); //SC更新邮件
        EventDispatcher.Instance.AddEventListener(MailCellClickEvent.EVENT_TYPE, OnMailCellClick); //邮件点击
        EventDispatcher.Instance.AddEventListener(MailOperactionEvent.EVENT_TYPE, OnMailOperaction); //邮件操作
        EventDispatcher.Instance.AddEventListener(Enter_Scene_Event.EVENT_TYPE, OnEnterScene); //进入场景
    }

    private bool mInit;
    private readonly MailCellDataComplarer mMailComparer = new MailCellDataComplarer();
    public MailDataModel DataModel { get; set; }
    //更新邮件信息
    public void AddMailData(List<MailCell> mails, bool clean)
    {
        var list = new List<MailCellData>();
        if (clean)
        {
        }
        else
        {
            list.AddRange(DataModel.MailCells);
        }
        var mailCells = list;
        {
            var __list3 = mails;
            var __listCount3 = __list3.Count;
            for (var __i3 = 0; __i3 < __listCount3; ++__i3)
            {
                var mail = __list3[__i3];
                {
                    var cellData = new MailCellData
                    {
                        Id = mail.Guid,
                        IsApply = false,
                        Name = mail.Name,
                        DateTime = Extension.FromServerBinary(mail.StartTime),
                        State = mail.State
                    };
                    if (mailCells.Contains(cellData, mMailComparer))
                    {
                        mailCells.Remove(cellData);
                        clean = true;
                    }
                    mailCells.Add(cellData);
                }
            }
        }
        list.Sort();

        DataModel.MailCells = new ObservableCollection<MailCellData>(list);

        DataModel.CellCount = mailCells.Count;
        if (DataModel.CellCount == 1)
        {
            OnApplyMail(0);
        }
        AnalyseNotice();

        DataModel.IsSelectAll = false;
    }

    //更新是否有邮件提示
    public void AnalyseNotice()
    {
        if (DataModel.MailCells.Count == 0)
        {
            DataModel.IsEmpty = true;
        }
        else
        {
            DataModel.IsEmpty = false;
        }

        var hasNew = false;
        var itemCount = 0;
        {
            // foreach(var mail in DataModel.MailCells)
            var __enumerator12 = (DataModel.MailCells).GetEnumerator();
            while (__enumerator12.MoveNext())
            {
                var mail = __enumerator12.Current;
                {
                    if (mail.State == (int) MailStateType.NewMail || mail.State == (int) MailStateType.NewMailHave ||
                        mail.State == (int) MailStateType.NewMailNothing ||
                        mail.State == (int) MailStateType.OldMailHave)
                    {
                        hasNew = true;
                    }

                    if (mail.State == (int) MailStateType.NewMailHave || mail.State == (int) MailStateType.OldMailHave ||
                        mail.State == (int) MailStateType.NewMail)
                    {
                        itemCount++;
                    }
                }
            }
        }

        PlayerDataManager.Instance.NoticeData.MailCount = itemCount;
        PlayerDataManager.Instance.NoticeData.MailNew = hasNew;
    }

    //请求邮件
    public IEnumerator ApplyMailsCoroutine(MailCellData data)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ApplyMailInfo(data.Id);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var mailInfo = msg.Response;
                    data.State = mailInfo.State;
                    var content = GameUtils.ConvertChatContent(mailInfo.Text);
                    data.InfoData.Content = content.Replace("\\n", "\n");
                    data.InfoData.Sender = mailInfo.Send;
                    var index = 0;
                    if (data.IsReceive != 1)
                    {
                        {
                            var __list9 = mailInfo.Items;
                            var __listCount9 = __list9.Count;
                            for (var __i9 = 0; __i9 < __listCount9; ++__i9)
                            {
                                var i = __list9[__i9];
                                {
                                    data.InfoData.Items[index].ItemId = i.ItemId;
                                    data.InfoData.Items[index].Count = i.Count;
                                    data.InfoData.Items[index].Exdata.InstallData(i.Exdata);
                                    index++;
                                }
                            }
                        }
                    }
                    for (var i = index; i < 5; i++)
                    {
                        data.InfoData.Items[i].Reset();
                    }
                    DataModel.SelectData.IsApply = true;
                    AnalyseNotice();
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_MailNotFind)
                {
                    //邮件没有找到
                    var e = new ShowUIHintBoard(msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error("ApplyMailInfo Error!............ErrorCode..." + msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("ApplyMailInfo Error!............State..." + msg.State);
            }
        }
    }

    //查看邮件
    public void DelectMails()
    {
        var isAttach = false;
        var mials = new Uint64Array();
        {
            // foreach(var cell in DataModel.MailCells)
            var __enumerator7 = (DataModel.MailCells).GetEnumerator();
            while (__enumerator7.MoveNext())
            {
                var cell = __enumerator7.Current;
                {
                    if (cell.IsSelect)
                    {
                        mials.Items.Add(cell.Id);
                        if (cell.IsAttach)
                        {
                            isAttach = true;
                        }
                    }
                }
            }
        }
        if (mials.Items.Count == 0)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(422));
            return;
        }

        if (isAttach)
        {
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 417, "", DelectMailsConfirm);
            return;
        }
        DelectMailsConfirm();
    }

    //删除邮件
    public void DelectMailsConfirm()
    {
        var mials = new Uint64Array();
        {
            // foreach(var cell in DataModel.MailCells)
            var __enumerator8 = (DataModel.MailCells).GetEnumerator();
            while (__enumerator8.MoveNext())
            {
                var cell = __enumerator8.Current;
                {
                    if (cell.IsSelect)
                    {
                        mials.Items.Add(cell.Id);
                    }
                }
            }
        }
        if (mials.Items.Count == 0)
        {
            return;
        }
        NetManager.Instance.StartCoroutine(DelectMailsCoroutine(mials));
    }

    //查看邮件
    public IEnumerator DelectMailsCoroutine(Uint64Array mails)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.DeleteMail(mails);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(418));
                    {
                        var mailList = new List<MailCellData>(DataModel.MailCells.ToArray());
                        var list11 = mails.Items;
                        var listCount11 = list11.Count;
                        var isReset = false;

                        for (var i11 = 0; i11 < listCount11; ++i11)
                        {
                            var mail = list11[i11];
                            {
                                var data = GetMailCellData(mail);

                                PlatformHelper.UMEvent("Mail", "Delete", data.Name);

                                if (data == DataModel.SelectData)
                                {
                                    isReset = true;
                                }
                                data.Id = 0;
                                mailList.Remove(data);

                                DataModel.IsSelectAll = false;
                            }
                        }
                        DataModel.MailCells = new ObservableCollection<MailCellData>(mailList);
                        DataModel.CellCount = DataModel.MailCells.Count;
                        if (isReset && DataModel.CellCount > 0)
                        {
                            OnApplyMail(0);
                        }
                        else
                        {
                            ResetSelectMailInfo();
                        }
                    }

                    AnalyseNotice();
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error("ReceiveMail Error!............ErrorCode..." + msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("ReceiveMail Error!............State..." + msg.State);
            }
        }
    }

    public MailCellData GetMailCellData(ulong id)
    {
        return DataModel.MailCells.FirstOrDefault(cell => cell.Id == id);
    }

    private void Init()
    {
        mInit = true;
        DataModel.MaxCount = GameUtils.MaxMailCount;
    }

    //请求邮件信息
    public void OnApplyMail(int index)
    {
        if (index >= DataModel.MailCells.Count)
        {
            return;
        }
        var cellData = DataModel.MailCells[index];
        {
            // foreach(var cell in DataModel.MailCells)
            var __enumerator2 = (DataModel.MailCells).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var cell = __enumerator2.Current;
                {
                    cell.IsClicked = cell == cellData ? 1 : 0;
                }
            }
        }
        DataModel.SelectData = cellData;
        if (cellData.IsApply)
        {
            return;
        }
        NetManager.Instance.StartCoroutine(ApplyMailsCoroutine(cellData));
    }

    //checkbox变化调用
    public void OnChangeSelectAll(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "IsSelectAll")
        {
            var isSelect = DataModel.IsSelectAll;
            {
                // foreach(var cell in DataModel.MailCells)
                var __enumerator1 = (DataModel.MailCells).GetEnumerator();
                while (__enumerator1.MoveNext())
                {
                    var cell = __enumerator1.Current;
                    {
                        cell.IsSelect = isSelect;
                    }
                }
            }
        }
    }

    private void OnCheckAllSelect(bool isSelect)
    {
        var __enumerator1 = (DataModel.MailCells).GetEnumerator();
        while (__enumerator1.MoveNext())
        {
            var cell = __enumerator1.Current;
            {
                cell.IsSelect = isSelect;
            }
        }

        DataModel.IsSelectAll = isSelect;
    }

    //切换场景判断邮件是否快满
    public void OnEnterScene(IEvent iEvent)
    {
        WarnMailCount();
    }

    public void OnMailCellCheck(int index, int value)
    {
        if (index >= DataModel.MailCells.Count)
        {
            return;
        }
        var cellData = DataModel.MailCells[index];

        cellData.IsSelect = value == 1;

        if (cellData.IsSelect == false)
        {
            if (DataModel.IsSelectAll)
            {
                DataModel.IsSelectAll = false;
            }
        }
        else
        {
            if (DataModel.IsSelectAll == false)
            {
                var isAll = true;
                foreach (var cell in DataModel.MailCells)
                {
                    if (cell.IsSelect == false)
                    {
                        isAll = false;
                        break;
                    }
                }
                if (isAll)
                {
                    DataModel.IsSelectAll = true;
                }
            }
        }
    }

    //单邮件点击事件
    public void OnMailCellClick(IEvent ievent)
    {
        var e = ievent as MailCellClickEvent;
        var index = e.Index;
        if (e.Type == 1)
        {
            OnApplyMail(index);
        }
        else if (e.Type == 2)
        {
            OnMailCellCheck(index, e.Value);
        }
    }

    //邮件操作事件
    public void OnMailOperaction(IEvent ievent)
    {
        var e = ievent as MailOperactionEvent;
        switch (e.Type)
        {
            case 1:
            {
                ReceiveMail();
            }
                break;
            case 2:
            {
                ReceiveMails();
            }
                break;
            case 3:
            {
                DelectMails();
            }
                break;
            case 4:
            {
                OnCheckAllSelect(true);
            }
                break;
            case 5:
            {
                OnCheckAllSelect(false);
            }
                break;
            default:
                break;
        }
    }

    //SC邮件更新
    public void OnMailSync(IEvent ievent)
    {
        var e = ievent as MailSyncEvent;
        var mails = e.List;
        AddMailData(mails.Mails, false);

        for (int i = 0; i < mails.Mails.Count; i++)
        {
            PlatformHelper.UMEvent("Mail", "GetMail", mails.Mails[i].Name);
        } 
    }

    //领取邮件
    public void ReceiveMail()
    {
        var data = DataModel.SelectData;
        if (data.Id == 0)
        {
            return;
        }
        if (data.IsReceive == 1)
        {
            //邮件已经领取
            var e = new ShowUIHintBoard(3200001);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }
        if (data.IsAttach == false)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(419));
            return;
        }
        var mials = new Uint64Array();
        mials.Items.Add(data.Id);
        NetManager.Instance.StartCoroutine(ReceiveMailsCoroutine(mials));
    }

    //批量领取邮件
    public void ReceiveMails()
    {
        var mials = new Uint64Array();
        var isCheck = false;
        {
            // foreach(var cell in DataModel.MailCells)
            var __enumerator6 = (DataModel.MailCells).GetEnumerator();
            while (__enumerator6.MoveNext())
            {
                var cell = __enumerator6.Current;
                {
                    if (cell.IsSelect)
                    {
                        isCheck = true;
                        if (cell.IsAttach)
                        {
                            mials.Items.Add(cell.Id);
                        }
                    }
                }
            }
        }

        if (isCheck == false)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(421));
            return;
        }
        if (mials.Items.Count == 0)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(419));
            return;
        }
        NetManager.Instance.StartCoroutine(ReceiveMailsCoroutine(mials));
    }

    //领取邮件
    public IEnumerator ReceiveMailsCoroutine(Uint64Array mails)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ReceiveMail(mails);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    //DataModel.IsSelectAll = false;
                    var receieve = msg.Response;
                    if (receieve == 0)
                    {
                        //您包裹已满！
                        var e = new ShowUIHintBoard(302);
                        EventDispatcher.Instance.DispatchEvent(e);
                        yield break;
                    }
                    if (mails.Items.Count == receieve)
                    {
                        EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(420));
                    }
                    else
                    {
                        //背包已满，不能领取全部邮件
                        GameUtils.ShowHintTip(3200006);
                    }

                    for (var i = 0; i < receieve; i++)
                    {
                        var id = mails.Items[i];
                        var data = GetMailCellData(id);
                        data.State = 2;
                        if (data.IsApply)
                        {
                            {
                                // foreach(var item in data.InfoData.Items)
                                var __enumerator10 = (data.InfoData.Items).GetEnumerator();
                                while (__enumerator10.MoveNext())
                                {
                                    var item = __enumerator10.Current;
                                    {
                                        item.ItemId = -1;
                                        item.Count = 0;
                                    }
                                }
                            }
                        }
                        PlatformHelper.UMEvent("Mail", "GetItem", data.Name);
                    }
                    AnalyseNotice();
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_MailNotFind
                         || msg.ErrorCode == (int) ErrorCodes.Error_MailReceiveOver)
                {
                    //邮件没有找到
                    var e = new ShowUIHintBoard(msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else
                {
                    var e = new ShowUIHintBoard(msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(e);
                    Logger.Error("ReceiveMail Error!............ErrorCode..." + msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("ReceiveMail Error!............State..." + msg.State);
            }
        }
    }

    //情况邮件箱
    public void ResetSelectMailInfo()
    {
        DataModel.SelectData = EmptyMailCellData;
    }

    //邮件警告数量45，50分别提示
    public void WarnMailCount()
    {
        if (mInit == false)
        {
            Init();
        }
        var count = DataModel.MailCells.Count;
        if (count >= DataModel.MaxCount)
        {
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 416, "",
                () =>
                {
                    var e = new Show_UI_Event(UIConfig.MailUI);
                    EventDispatcher.Instance.DispatchEvent(e);
                });
        }
        else if (count > DataModel.MaxCount - 5)
        {
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 415, "",
                () =>
                {
                    var e = new Show_UI_Event(UIConfig.MailUI);
                    EventDispatcher.Instance.DispatchEvent(e);
                });
        }
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public void CleanUp()
    {
        DataModel = new MailDataModel();
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        if (name == "AddMailData")
        {
            AddMailData(param[0] as List<MailCell>, (bool) param[1]);
        }

        return null;
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
        if (DataModel.SelectData.Id == ulong.MaxValue)
        {
            if (DataModel.MailCells.Count > 0)
            {
                DataModel.SelectData = DataModel.MailCells[0];
                DataModel.MailCells[0].IsClicked = 1;
                OnApplyMail(0);
            }
        }
    }

    public FrameState State { get; set; }
}