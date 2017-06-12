#region using

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ClientDataModel;
using ClientService;
using DataTable;
using EventSystem;
using ObjCommand;
using ScorpionNetLib;

#endregion

public class HandBookController : IControllerBase
{
    public HandBookController()
    {
        CleanUp();

        EventDispatcher.Instance.AddEventListener(UIEvent_HandBookFrame_ComposeBookPiece.EVENT_TYPE,
            ComposeBookPieceClick);
        EventDispatcher.Instance.AddEventListener(UIEvent_HandBookFrame_OnBookClick.EVENT_TYPE, ShowBookInfo);
        EventDispatcher.Instance.AddEventListener(UIEvent_HandBookFrame_OnGetBookClick.EVENT_TYPE, OnGetBookClick);
        EventDispatcher.Instance.AddEventListener(UIEvent_HandBookFrame_ComposeBookPieceFromBookInfo.EVENT_TYPE,
            ComposeBookPieceFromBookInfo);
        EventDispatcher.Instance.AddEventListener(UIEvent_HandBookFrame_OnBookGroupToggled.EVENT_TYPE,
            OnBookGroupToggled);
        EventDispatcher.Instance.AddEventListener(UIEvent_HandBookFrame_OnBountyGroupToggled.EVENT_TYPE,
            OnBountyGroupToggled);
        EventDispatcher.Instance.AddEventListener(UIEvent_HandBookFrame_OnGroupBookActive.EVENT_TYPE,
            OnBookActiveInGroup);
        EventDispatcher.Instance.AddEventListener(UIEvent_HandBookFrame_OnBountyBookActive.EVENT_TYPE,
            OnBountyBookActive);
        EventDispatcher.Instance.AddEventListener(UIEvent_HandBookFrame_ComposeBookCardFromBookInfo.EVENT_TYPE,
            OnComposeBookCardFromBookInfo);
    }

    //悬赏cache
    public Dictionary<int, List<int>> bountyDictionary = new Dictionary<int, List<int>>();
    //bookbuffer
    public List<HandBookItemDataModel> mBookBufferList = new List<HandBookItemDataModel>();
    public Dictionary<int, HandBookItemDataModel> mBooksCacheDic = new Dictionary<int, HandBookItemDataModel>();
    //兑换数据
    public Dictionary<int, ItemComposeRecord> mComposeTableDic = new Dictionary<int, ItemComposeRecord>();
    public HandBookDataModel mHandBookDataModel;
    public bool UsingItem;

    public IEnumerator ActivateBook(HandBookItemDataModel dataModel, int id, int groupId, int bitIndex)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ActivateBook(id, groupId, bitIndex);
            yield return msg.SendAndWaitUntilDone();


            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    //先修改playerdata里存储的dictionary
                    PlayerDataManager.Instance.SetBookGroupEnable(groupId, bitIndex);

                    var groupDataModel = mHandBookDataModel.SelectedGropBooks;
                    dataModel.BookCount--;
                    // groupDataModel.BookEnable[bitIndex] = 1;
                    groupDataModel.GropBook[bitIndex].BookEnable = 1;
                    groupDataModel.GropCount++;
                    if (groupDataModel.GropCount >= groupDataModel.GropMaxCount)
                    {
                        groupDataModel.GropEnable = 1;
                    }
                    RefreshNotice();
                    var ee = new ShowUIHintBoard(200011);
                    EventDispatcher.Instance.DispatchEvent(ee);
                    PlayerAttr.Instance.SetAttrChange(PlayerAttr.PlayerAttrChange.HandBook);

                    PlatformHelper.UMEvent("HandBook", "ActiveInGroup", id);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
        }
    }

    public IEnumerator BountyBookActive(HandBookItemDataModel dataModel)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ActivateBook(dataModel.BookId, -1, -1);
            yield return msg.SendAndWaitUntilDone();

            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    PlayerDataManager.Instance.SetBountyBookEnable(dataModel.BookId);
                    dataModel.BookCount--;
                    dataModel.BountyActive = 1;
                    RefreshNotice();
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(200011));
                    PlayerAttr.Instance.SetAttrChange(PlayerAttr.PlayerAttrChange.HandBook);

                    PlatformHelper.UMEvent("HandBook", "Active", dataModel.BookId);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
        }
    }

    public IEnumerator ComposeBookCard(int id)
    {
        using (new BlockingLayerHelper(0))
        {
            var composeCount = 1;
            var msg = NetManager.Instance.ComposeItem(id, composeCount);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var selectBook = mHandBookDataModel.SelectedBook[0];
                    var castBook = mHandBookDataModel.SelectedBook[1];
                    castBook.BookCount -= selectBook.UpGradeRequestCount;
                    selectBook.BookCount++;
                    HandBookItemDataModel book;
                    if (mBooksCacheDic.TryGetValue(castBook.BookId, out book))
                    {
                        book.BookCount = castBook.BookCount;
                    }
                    else
                    {
                        Logger.Error("ComposeBookCard error, bookinfo: " + castBook.BookId + "do not exist!");
                    }

                    if (mBooksCacheDic.TryGetValue(selectBook.BookId, out book))
                    {
                        book.BookCount = selectBook.BookCount;
                    }
                    else
                    {
                        Logger.Error("ComposeBookCard error, bookinfo: " + selectBook.BookId + "do not exist!");
                    }

                    RefreshNotice();
                    var ee = new ShowUIHintBoard(200010);
                    EventDispatcher.Instance.DispatchEvent(ee);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
        }
    }

    public void ComposeBookPieceClick(IEvent ievent)
    {
        var e = ievent as UIEvent_HandBookFrame_ComposeBookPiece;
        NetManager.Instance.StartCoroutine(ComposeBookPieceMsg(e.Index));
    }

    public void ComposeBookPieceFromBookInfo(IEvent ievent)
    {
        var index = 0;
        {
            // foreach(var dataModel in mHandBookDataModel.Books)
            var __enumerator5 = (mHandBookDataModel.Books).GetEnumerator();
            while (__enumerator5.MoveNext())
            {
                var dataModel = __enumerator5.Current;
                {
                    if (dataModel.Equals(mHandBookDataModel.SelectedBook[0]))
                    {
                        break;
                    }
                    index++;
                }
            }
        }
        if (index >= mHandBookDataModel.Books.Count)
        {
            Logger.Error("error index at handBooks !!");
        }
        NetManager.Instance.StartCoroutine(ComposeBookPieceMsg(index));
    }

    public IEnumerator ComposeBookPieceMsg(int index)
    {
        using (new BlockingLayerHelper(0))
        {
            var book = mHandBookDataModel.Books[index];

            var pieceId = Table.GetHandBook(book.BookId).PieceId;
            var pieceBag = PlayerDataManager.Instance.GetBag(2);
            var bagIndex = -1;
            {
                // foreach(var item in pieceBag.Items)
                var __enumerator6 = (pieceBag.Items).GetEnumerator();
                while (__enumerator6.MoveNext())
                {
                    var item = __enumerator6.Current;
                    {
                        if (pieceId == item.ItemId)
                        {
                            bagIndex = item.Index;
                            break;
                        }
                    }
                }
            }
            //检查碎片
            if (bagIndex == -1)
            {
                var ee = new ShowUIHintBoard(200009);
                EventDispatcher.Instance.DispatchEvent(ee);
                yield break;
            }

            UsingItem = true;

            var msg = NetManager.Instance.UseItem(2, bagIndex, 1);
            yield return msg.SendAndWaitUntilDone();

            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    if (UsingItem)
                    {
                        var bookTable = Table.GetHandBook(book.BookId);
                        book.BookCount++;
                        RefreshNotice();

                        book.BookPieceCount -= bookTable.Count;
                        book.Composeable = (float) book.BookPieceCount/bookTable.Count;

                        mHandBookDataModel.SelectedBook[0].Copy(book);
                        if (book.Composeable < 1)
                        {
                            PlayerDataManager.Instance.NoticeData.HandBookCompose--;
                            ResetBooks();
                        }
                    }
                    else
                    {
                        UIManager.Instance.ShowNetError(msg.ErrorCode);
                    }
                    var ee = new ShowUIHintBoard(200010);
                    EventDispatcher.Instance.DispatchEvent(ee);

                    PlatformHelper.UMEvent("HandBook", "Piece", book.BookId);
                }
                else
                {
                    if (msg.ErrorCode == (int) ErrorCodes.Error_SkillNoCD)
                    {
                    }
                    else
                    {
                        UIManager.Instance.ShowNetError(msg.ErrorCode);
                    }
                }
            }
        }
    }

    public HandBookItemDataModel CreateOneBookFromTable(HandBookRecord table)
    {
        var handbookitem = new HandBookItemDataModel();
        handbookitem.BookId = table.Id;
        handbookitem.BookSortId = table.ListSort;
        handbookitem.ItemId = table.Id;
        handbookitem.BookPieceCount = 0;
        handbookitem.BookMaxCapacity = Table.GetItemBase(table.Id).MaxCount;
        if (table.NpcId != -1)
        {
            handbookitem.MonsterName = Table.GetNpcBase(table.NpcId).Name;
            handbookitem.MonsterLevel = Table.GetNpcBase(table.NpcId).Level;
        }
        handbookitem.TrackType = table.TrackType;
        handbookitem.locationName = table.TrackString;
        handbookitem.TrackParam[0] = table.TrackParam[0];
        handbookitem.TrackParam[1] = table.TrackParam[1];
        handbookitem.TrackParam[2] = table.TrackParam[2];
        handbookitem.BountyBookAttr = string.Format("{0}: +{1}", ExpressionHelper.AttrName[table.AttrId],
            table.AttrValue);
        handbookitem.BountyMoney = table.Money;
        //-----bountybook data start------------------------------
        var key = table.RewardGroupId;
        var value = table.Id;
        List<int> output;
        if (bountyDictionary.TryGetValue(key, out output))
        {
            output.Add(value);
        }
        else
        {
            output = new List<int>();
            output.Add(value);
            bountyDictionary.Add(key, output);
        }
        //-----bountybook data end------------------------------

        //初始化合成属性
        ItemComposeRecord ComposeTable;
        var bExist = mComposeTableDic.TryGetValue(handbookitem.BookId, out ComposeTable);
        if (bExist)
        {
            handbookitem.BookUpgradeRequestCast = ComposeTable.NeedValue;
            handbookitem.UpGradeRequestBookId = ComposeTable.NeedId[0];
            handbookitem.UpGradeRequestCount = ComposeTable.NeedCount[0];
            handbookitem.BookComposeTableId = ComposeTable.Id;
        }
        return handbookitem;
    }

    public HandBookGropDataModel CreateOneBookGrop(BookGroupRecord table, int GropId)
    {
        var bookGrop = new HandBookGropDataModel();
        bookGrop.GropId = table.Id;
        bookGrop.GropName = table.Desc;
        var maxCount = 0;
        var count = 0;

        for (var i = 0; i < 6; i++)
        {
            var id = table.ItemId[i];
            if (id != -1)
            {
                var oneGroup = new OneGroupDataModel();
                maxCount++;
                var attrName = ExpressionHelper.AttrName[table.AttrId[i]];
                var attrValue = table.AttrValue[i];
                oneGroup.BookAttrInfo = string.Format("{0}: +{1}", attrName, attrValue);
                if (PlayerDataManager.Instance.GetBookGropEnable(GropId, i))
                {
                    count++;
                    oneGroup.BookEnable = 1;
                }
                HandBookItemDataModel item;
                var bExist = mBooksCacheDic.TryGetValue(id, out item);
                if (bExist)
                {
                    oneGroup.book = item;
                }
                bookGrop.GropBook.Add(oneGroup);
            }
        }

        bookGrop.GropCount = count;
        bookGrop.GropMaxCount = maxCount;
        bookGrop.GropEnable = count == maxCount ? 1 : 0;
        for (var i = 0; i < 4; i++)
        {
            if (table.GroupAttrId[i] != -1)
            {
                var attrName = ExpressionHelper.AttrName[table.GroupAttrId[i]];
                var attrValue = table.GroupAttrValue[i];
                bookGrop.BookGropAttrInfo[i] = string.Format("{0}: +{1}", attrName, attrValue);
            }
            else
            {
                bookGrop.BookGropAttrInfo[i] = string.Empty;
            }
        }
        {
            // foreach(var oneGroupDataModel in bookGrop.GropBook)
            var __enumerator4 = (bookGrop.GropBook).GetEnumerator();
            while (__enumerator4.MoveNext())
            {
                var oneGroupDataModel = __enumerator4.Current;
                {
                    if (oneGroupDataModel.BookEnable != 1 && oneGroupDataModel.book.BookCount >= 1)
                    {
                        bookGrop.NoticeCount++;
                    }
                }
            }
        }

        return bookGrop;
    }

    public void OnBookActiveInGroup(IEvent ievent)
    {
        var e = ievent as UIEvent_HandBookFrame_OnGroupBookActive;
        var BookItemDataModel = e.DataModel;
        var index = e.index;
        var id = BookItemDataModel.BookId;
        var groupId = mHandBookDataModel.SelectedGropBooks.GropId;
        NetManager.Instance.StartCoroutine(ActivateBook(BookItemDataModel, id, groupId, index));
    }

    public void OnBookGroupToggled(IEvent ievent)
    {
        var e = ievent as UIEvent_HandBookFrame_OnBookGroupToggled;
        mHandBookDataModel.SelectedGropBooks = mHandBookDataModel.GropBooks[e.Index];
        for (var i = 0; i < mHandBookDataModel.GropBooks.Count; i++)
        {
            mHandBookDataModel.GropBooks[i].ShowToggle = false;
        }
        mHandBookDataModel.SelectedGropBooks.ShowToggle = true;
    }

    public void OnBountyBookActive(IEvent ievent)
    {
        var e = ievent as UIEvent_HandBookFrame_OnBountyBookActive;
        NetManager.Instance.StartCoroutine(BountyBookActive(e.DataModel));
    }

    public void OnBountyGroupToggled(IEvent ievent)
    {
        var e = ievent as UIEvent_HandBookFrame_OnBountyGroupToggled;
        mHandBookDataModel.selectedBountyGroup = mHandBookDataModel.MonsterBounty[e.Index];
    }

    public void OnComposeBookCardFromBookInfo(IEvent ievent)
    {
        var selectBook = mHandBookDataModel.SelectedBook[0];
        var castBook = mHandBookDataModel.SelectedBook[1];
        var gold = PlayerDataManager.Instance.GetRes((int) eResourcesType.GoldRes);
        if (selectBook.BookUpgradeRequestCast > gold)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210100));
            return;
        }
        if (selectBook.UpGradeRequestCount > castBook.BookCount)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210101));
            return;
        }

        NetManager.Instance.StartCoroutine(ComposeBookCard(selectBook.BookComposeTableId));
    }

    public void OnGetBookClick(IEvent ievent)
    {
        if (mHandBookDataModel.BookInfoShow == 1)
        {
            var itemData = mHandBookDataModel.SelectedBook[0];
            //21去打怪获取
            if (21 == itemData.TrackType)
            {
                ObjManager.Instance.MyPlayer.LeaveAutoCombat();
                GameControl.Executer.Stop();
                var command = GameControl.GoToCommand(itemData.TrackParam[0], itemData.TrackParam[1],
                    itemData.TrackParam[2]);
                var command1 = new FuncCommand(() =>
                {
                    GameControl.Instance.TargetObj = null;
                    ObjManager.Instance.MyPlayer.EnterAutoCombat();
                });
                GameControl.Executer.PushCommand(command);
                GameControl.Executer.PushCommand(command1);
                EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.HandBook));
            }
            else if (22 == itemData.TrackType)
            {
                var e = new Show_UI_Event(UIConfig.DungeonUI);
                EventDispatcher.Instance.DispatchEvent(e);

                GameUtils.GotoUiTab(25, itemData.TrackParam[0]);
            }
            else if (23 == itemData.TrackType)
            {
                var dicid = itemData.TrackParam[0];
                if (dicid > 0)
                {
                    var ee = new ShowUIHintBoard(dicid);
                    EventDispatcher.Instance.DispatchEvent(ee);
                }
            }
            else if (23 == itemData.TrackType)
            {
                var ee = new ShowUIHintBoard(itemData.TrackParam[1]);
                EventDispatcher.Instance.DispatchEvent(ee);
            }
        }
    }

    //刷新技能红点
    public void RefreshNotice()
    {
        var totalBountyCount = 0;
        var totalGroupCount = 0;
        var count = 0;
        {
            // foreach(var gropDataModel in mHandBookDataModel.MonsterBounty)
            var __enumerator9 = (mHandBookDataModel.MonsterBounty).GetEnumerator();
            while (__enumerator9.MoveNext())
            {
                var gropDataModel = __enumerator9.Current;
                {
                    var groupCount = 0;
                    {
                        // foreach(var book in gropDataModel.BountyGroupBooks)
                        var __enumerator12 = (gropDataModel.BountyGroupBooks).GetEnumerator();
                        while (__enumerator12.MoveNext())
                        {
                            var book = __enumerator12.Current;
                            {
                                if (book.BountyActive == 0)
                                {
                                    if (book.BookCount >= 1)
                                    {
                                        groupCount++;
                                    }
                                }
                                else
                                {
                                    totalBountyCount++;
                                }
                            }
                        }
                    }
                    gropDataModel.NoticeCount = groupCount;
                    if (groupCount >= 1)
                    {
                        count++;
                    }
                }
            }
        }
        PlayerDataManager.Instance.NoticeData.HandBookWanted = count;

        //图鉴组合红点
        count = 0;
        {
            // foreach(var oneGroup in mHandBookDataModel.GropBooks)
            var __enumerator10 = (mHandBookDataModel.GropBooks).GetEnumerator();
            while (__enumerator10.MoveNext())
            {
                var oneGroup = __enumerator10.Current;
                {
                    var groupCount = 0;
                    {
                        // foreach(var oneGroupDataModel in oneGroup.GropBook)
                        var __enumerator13 = (oneGroup.GropBook).GetEnumerator();
                        while (__enumerator13.MoveNext())
                        {
                            var oneGroupDataModel = __enumerator13.Current;
                            {
                                if (oneGroupDataModel.BookEnable != 1)
                                {
                                    if (oneGroupDataModel.book.BookCount >= 1)
                                    {
                                        groupCount++;
                                    }
                                }
                                else
                                {
                                    totalGroupCount++;
                                }
                            }
                        }
                    }

                    oneGroup.NoticeCount = groupCount;
                    if (groupCount >= 1)
                    {
                        count++;
                    }
                }
            }
        }
        PlayerDataManager.Instance.NoticeData.HandBookGroup = count;
        PlayerDataManager.Instance.TotalBountyCount = totalBountyCount;
        PlayerDataManager.Instance.TotalGroupCount = totalGroupCount;
    }

    public void RefreshNoticeData()
    {
        var totalBountyCount = 0;
        var totalGroupCount = 0;
        var c0 = mHandBookDataModel.Books.Count;
        var noticeBookCount = 0;
        for (var i = 0; i < c0; i++)
        {
            var book = mHandBookDataModel.Books[i];
            if (book.Composeable >= 1)
            {
                noticeBookCount++;
            }
        }
        PlayerDataManager.Instance.NoticeData.HandBookCompose = noticeBookCount;

        //图鉴组合table页上的红点
        UpdateBookGroup();
        var groupCount = 0;
        var c3 = mHandBookDataModel.GropBooks.Count;
        for (var i = 0; i < c3; i++)
        {
            var oneGroup = mHandBookDataModel.GropBooks[i];
            if (oneGroup.NoticeCount >= 1)
            {
                groupCount++;
            }
            var __enumerator13 = (oneGroup.GropBook).GetEnumerator();
            while (__enumerator13.MoveNext())
            {
                var oneGroupDataModel = __enumerator13.Current;
                {
                    if (oneGroupDataModel.BookEnable == 1)
                    {
                        totalGroupCount++;
                    }
                }
            }
        }
        PlayerDataManager.Instance.NoticeData.HandBookGroup = groupCount;

        //悬赏红点
        var bountys = mHandBookDataModel.MonsterBounty;
        var c = bountys.Count;
        var bountyTableCount = 0;
        for (var i = 0; i < c; i++)
        {
            var oneGroupCount = 0;
            var bountyBookGroup = bountys[i].BountyGroupBooks;
            var c2 = bountyBookGroup.Count;
            for (var j = 0; j < c2; j++)
            {
                var book = bountyBookGroup[j];
                if (book.BountyActive == 0)
                {
                    if (book.BookCount >= 1)
                    {
                        oneGroupCount++;
                    }
                }
                else
                {
                    totalBountyCount++;
                }
            }
            bountys[i].NoticeCount = oneGroupCount;
            if (oneGroupCount > 0)
            {
                bountyTableCount++;
            }
        }
        PlayerDataManager.Instance.NoticeData.HandBookWanted = bountyTableCount;
        PlayerDataManager.Instance.TotalBountyCount = totalBountyCount;
        PlayerDataManager.Instance.TotalGroupCount = totalGroupCount;
    }

    public void ResetBooks()
    {
        //         var list = new List<HandBookItemDataModel>(mHandBookDataModel.Books);
        //         list.Sort();
        //         mHandBookDataModel.Books = new ObservableCollection<HandBookItemDataModel>(list);
        var c0 = mBookBufferList.Count;
        mBookBufferList.Sort();
        mHandBookDataModel.Books = new ObservableCollection<HandBookItemDataModel>();
        for (var i = 0; i < c0; i++)
        {
            mHandBookDataModel.Books.Add(mBookBufferList[i]);
        }
    }

    public void ShowBookInfo(IEvent ievent)
    {
        var e = ievent as UIEvent_HandBookFrame_OnBookClick;
        if (e.DataModel == null)
        {
            mHandBookDataModel.BookInfoShow = 0;
        }
        else
        {
            mHandBookDataModel.SelectedBook[0].Copy(e.DataModel);
            mHandBookDataModel.SelectedBook[1].Copy(new HandBookItemDataModel());
            mHandBookDataModel.BookInfoDesc = GameUtils.GetDictionaryText(e.DataModel.BookId);
            {
                // foreach(var dataModel in mHandBookDataModel.Books)
                var __enumerator7 = (mHandBookDataModel.Books).GetEnumerator();
                while (__enumerator7.MoveNext())
                {
                    var dataModel = __enumerator7.Current;
                    {
                        if (e.DataModel.UpGradeRequestBookId == dataModel.BookId)
                        {
                            mHandBookDataModel.SelectedBook[1].Copy(dataModel);
                            break;
                        }
                    }
                }
            }
            mHandBookDataModel.BookInfoShow = 1;
        }
    }

    public void UpdateBookGroup()
    {
        var c0 = mHandBookDataModel.GropBooks.Count;
        for (var i = 0; i < c0; i++)
        {
            var oneGroup = mHandBookDataModel.GropBooks[i];
            oneGroup.NoticeCount = 0;
            var c1 = oneGroup.GropBook.Count;
            var count = 0;
            for (var j = 0; j < c1; j++)
            {
                if (PlayerDataManager.Instance.GetBookGropEnable(oneGroup.GropId, j))
                {
                    count++;
                    oneGroup.GropBook[j].BookEnable = 1;
                }
                else if (oneGroup.GropBook[j].book.BookCount >= 1)
                {
                    oneGroup.NoticeCount++;
                }
            }
            oneGroup.GropCount = count;
            oneGroup.GropEnable = (count == oneGroup.GropMaxCount ? 1 : 0);
        }
    }

    public void UpdateNotice()
    {
        RefreshData(null);
    }

    public void CleanUp()
    {
        mHandBookDataModel = new HandBookDataModel();

        Table.ForeachItemCompose(table =>
        {
            if (table.Type != 0)
            {
                return true;
            }
            if (!mComposeTableDic.ContainsKey(table.ComposeView))
            {
                mComposeTableDic.Add(table.ComposeView, table);
            }
            return true;
        });

        bountyDictionary.Clear();
        mBookBufferList.Clear();
        Table.ForeachHandBook(table =>
        {
            var book = CreateOneBookFromTable(table);
            mHandBookDataModel.Books.Add(book);
            mBookBufferList.Add(book);
            return true;
        });

        mBooksCacheDic.Clear();
        var books = mHandBookDataModel.Books;
        for (var i = 0; i < books.Count; i++)
        {
            mBooksCacheDic.Add(books[i].BookId, books[i]);
        }

        //分组界面数据
        Table.ForeachBookGroup(group =>
        {
            var bookGroup = CreateOneBookGrop(group, group.Id);
            mHandBookDataModel.GropBooks.Add(bookGroup);
            return true;
        });

        //悬赏界面数据
        mHandBookDataModel.MonsterBounty.Clear();
        var bountyTableCount = 0;
        {
            var enumerator = bountyDictionary.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var pair = enumerator.Current;
                var oneGroupCount = 0;
                var bountyBookGroup = new BountyBookGropDataModel();
                var dicId = Table.GetHandBook(pair.Value[0]).RewardGroupId;
                bountyBookGroup.BountyName = GameUtils.GetDictionaryText(dicId);
                {
                    var count2 = pair.Value.Count;
                    for (var j = 0; j < count2; j++)
                    {
                        var key = pair.Value[j];
                        HandBookItemDataModel item;
                        var bExist = mBooksCacheDic.TryGetValue(key, out item);
                        if (bExist)
                        {
                            bountyBookGroup.BountyGroupBooks.Add(item);
                            if (item.BountyActive == 0 && item.BookCount >= 1)
                            {
                                oneGroupCount++;
                            }
                        }
                    }
                }
                bountyBookGroup.NoticeCount = oneGroupCount;
                mHandBookDataModel.MonsterBounty.Add(bountyBookGroup);
                if (oneGroupCount >= 1)
                {
                    bountyTableCount++;
                }
            }
        }
        PlayerDataManager.Instance.NoticeData.HandBookWanted = bountyTableCount;

        mHandBookDataModel.SelectedGropBooks = mHandBookDataModel.GropBooks[0];
        mHandBookDataModel.SelectedGropBooks.ShowToggle = true;
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        if (name == "UpdateNotice")
        {
            UpdateNotice();
        }
        else if (name == "RefreshCount")
        {
            RefreshData(null);
        }

        return null;
    }

    public void OnShow()
    {
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return mHandBookDataModel;
    }

    public void Close()
    {
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        var args = data as HandBookArguments;
        if (args != null && args.Tab != -1)
        {
            mHandBookDataModel.TabSelect1 = args.Tab;
        }
        else
        {
            mHandBookDataModel.TabSelect1 = 0;
        }
        var c0 = mHandBookDataModel.Books.Count;
        for (var i = 0; i < c0; i++)
        {
            var book = mHandBookDataModel.Books[i];
            book.BountyActive = PlayerDataManager.Instance.GetBountyBookEnable(book.BookId) ? 1 : 0;
            book.BookCount = PlayerDataManager.Instance.GetItemTotalCount(book.BookId).Count;
            var table = Table.GetHandBook(book.BookId);
            book.BookPieceCount = PlayerDataManager.Instance.GetItemTotalCount(table.PieceId).Count;
            if (book.BookPieceCount == 0)
            {
                book.Composeable = 0;
            }
            else
            {
                book.Composeable = (float) book.BookPieceCount/table.Count;
            }
        }
        mBookBufferList.Sort();
        mHandBookDataModel.Books = new ObservableCollection<HandBookItemDataModel>();
        for (var i = 0; i < c0; i++)
        {
            mHandBookDataModel.Books.Add(mBookBufferList[i]);
        }

        if (State == FrameState.Open)
        {
            EventDispatcher.Instance.DispatchEvent(new UIEvent_HandBookFrame_RestScrollViewPostion());
        }

        RefreshNoticeData();
        UsingItem = false;
    }

    public FrameState State { get; set; }
}