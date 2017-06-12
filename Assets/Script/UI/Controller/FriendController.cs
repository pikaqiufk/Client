#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using LZ4s;
using ScorpionNetLib;
using ProtoBuf;
using Shared;

#if !UNITY_EDITOR
using UnityEngine;
#endif

#endregion

public class FriendController : IControllerBase
{
    private static readonly ContactInfoDataModel EmptyContactInfo = new ContactInfoDataModel();

    public FriendController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(ChatVoiceContent.EVENT_TYPE, OnChatVoiceContent);
        EventDispatcher.Instance.AddEventListener(FriendSeekBtnClick.EVENT_TYPE, OnClicCharSeek);
        EventDispatcher.Instance.AddEventListener(FriendClickShowInfo.EVENT_TYPE, OnClicCharInfo);
        EventDispatcher.Instance.AddEventListener(FriendOperationEvent.EVENT_TYPE, OnFriendOperation);
        EventDispatcher.Instance.AddEventListener(FriendClickTabEvent.EVENT_TYPE, OnFriendClickTabEvent);
        EventDispatcher.Instance.AddEventListener(FriendAddSyncEvent.EVENT_TYPE, OnFriendAyncAdd);
        EventDispatcher.Instance.AddEventListener(FriendDelSyncEvent.EVENT_TYPE, OnFriendAyncDel);
        EventDispatcher.Instance.AddEventListener(FriendUpdateSyncEvent.EVENT_TYPE, OnFriendAyncUpdate);
        EventDispatcher.Instance.AddEventListener(ChatMainPrivateChar.EVENT_TYPE, OnChatPrivate);
        EventDispatcher.Instance.AddEventListener(FriendClickType.EVENT_TYPE, OnFriendClickType);
        EventDispatcher.Instance.AddEventListener(WhisperChatMessage.EVENT_TYPE, OnWhisperChatMessage);
        EventDispatcher.Instance.AddEventListener(FriendContactCell.EVENT_TYPE, OnFriendContactCell);
        EventDispatcher.Instance.AddEventListener(FriendContactClickAddFriend.EVENT_TYPE, OnFriendContactAddFriend);
        EventDispatcher.Instance.AddEventListener(AddFaceNode.EVENT_TYPE, OnAddFaceNode);
        EventDispatcher.Instance.AddEventListener(ChatShareItemEvent.EVENT_TYPE, OnChatShareItem);
        EventDispatcher.Instance.AddEventListener(ChatMainSendVoiceData.EVENT_TYPE, OnSendVoiceChat);

        EventDispatcher.Instance.AddEventListener(FriendReceive.EVENT_TYPE, OnFriendReceive);
    }

    public FriendDataModel DataModel;
    private string inputStr = string.Empty;
    private string inputStr2 = string.Empty;
    private int mAsyncState = -1;
    private readonly List<DateTime> mCdTimeList = new List<DateTime>(4);
    private string mCharacterChatDirectory = "";
    public Dictionary<string, string> mDicItemLink = new Dictionary<string, string>();
    private int mLoadSeekPostion;
    private Dictionary<ulong, List<ChatMessageData>> mSaveListCaches;
    private object mSaveTImerTrigger;
    private Dictionary<ulong, List<ChatMessageData>> mUnWriteListCaches;
    private readonly int PageChatCount = 10;

    private void AddChatRecord(ulong charId, FriendMessageDataModel message)
    {
        var data = message.MessageData as ChatMessageData;
        if (data == null)
        {
            return;
        }

        List<ChatMessageData> list;
        if (!mUnWriteListCaches.TryGetValue(charId, out list))
        {
            list = new List<ChatMessageData>();
            mUnWriteListCaches[charId] = list;
        }

        if (list.Count == 0)
        {
            data.ShowTime = 1;
        }
        else
        {
            var last = list[list.Count - 1];
            var dif = DateTime.FromBinary(data.Times) - DateTime.FromBinary(last.Times);
            if (dif.TotalMinutes > 5)
            {
                data.ShowTime = 1;
            }
            else
            {
                data.ShowTime = 0;
            }
        }
        list.Add(data);
        if (data.SoundData != null)
        {
            var e = new ChatSoundTranslateAddEvent(data.SoundData);
            EventDispatcher.Instance.DispatchEvent(e);
        }
        //SaveChatRecord(charId, name, message, type,lv,ladder);
    }

    private void AddContactCellInfo(ulong charId, string name, FriendMessageDataModel message)
    {
        if (DataModel.SelectContact.CharacterId == charId)
        {
            return;
        }
        //请玩家数据
        ApplyPlayerheadInfo(charId, info =>
        {
            //加入联系人
            var infoData = GetContactInfoData(charId);
            if (infoData == null)
            {
                infoData = new ContactInfoDataModel();
                infoData.HasUpdate = true;
                infoData.CharacterId = info.CharacterId;
                infoData.Name = info.Name;
                infoData.Type = info.RoleId;
                infoData.UnreadCount = 0;
            }
            else
            {
                DataModel.ContactInfos.Remove(infoData);
            }
            infoData.Ladder = info.Ladder;
            infoData.Level = info.Level;
            infoData.UnreadCount++;
            InsetContactInfo(infoData);
            UpdateNoticeUnreadCount();
            var msgInfo = message.MessageData as ChatMessageData;
            if (msgInfo == null)
            {
                return;
            }
            msgInfo.RoleId = infoData.Type;
            infoData.NextIndex = 0;
            AddContactCellNextIndex(infoData.Index);
            UpdateContactCellNextIndex();
            //写入文件
            AddChatRecord(charId, message);
        });
    }

    private void AddContactCellNextIndex(int index)
    {
        var c = DataModel.ContactInfos.Count;
        for (var i = 0; i < c; i++)
        {
            var d = DataModel.ContactInfos[i];
            if (d.NextIndex != index)
            {
                d.NextIndex++;
            }
        }
    }

    private void AddContactInfo(PlayerHeadInfoMsg info)
    {
        var addInfo = GetContactInfoData(info.CharacterId);
        if (addInfo == null)
        {
            addInfo = new ContactInfoDataModel();
            addInfo.Name = info.Name;
            addInfo.CharacterId = info.CharacterId;
        }
        else
        {
//删除加到开始位置
            DataModel.ContactInfos.Remove(addInfo);
        }
        addInfo.HasUpdate = true;
        addInfo.Ladder = info.Ladder;
        addInfo.Level = info.Level;
        addInfo.Type = info.RoleId;
        InsetContactInfo(addInfo);
        SelectFriendContactCell(0);
        UpdateContactCellNextIndex();
    }

    private void AddFace(int faceId)
    {
        var dataChatInfoNode = new ChatInfoNodeData();
        dataChatInfoNode.Type = (int) eChatLinkType.Face;
        dataChatInfoNode.Id = faceId;
        var str = "";
        using (var ms = new MemoryStream())
        {
            Serializer.Serialize(ms, dataChatInfoNode);
            var wrap = LZ4Codec.Encode32(ms.GetBuffer(), 0, (int)ms.Length);
            str = Convert.ToBase64String(wrap);
        }
        str = SpecialCode.ChatBegin + str + SpecialCode.ChatEnd;
        var value = "{" + faceId + "}";
        mDicItemLink[value] = str;

        var inputStr = GameUtils.GetDictionaryText(100001058);
        if (DataModel.InputChat == inputStr)
        {
            DataModel.InputChat = value;
        }
        else
        {
            DataModel.InputChat += value;
        }
    }

    private void AddFriend(ulong uid, int type)
    {
        if (uid < 1000)
        {
//机器人提示玩家不在线
            GameUtils.ShowHintTip(200000003);
            return;
        }
        switch (type)
        {
            case 0:
            {
                var friendMax = Table.GetClientConfig(320).Value.ToInt();
                if (DataModel.FriendInfos.Count >= friendMax)
                {
                    //ShowUIHintBoard e = new ShowUIHintBoard("好友列表已满");
                    var e = new ShowUIHintBoard(220200);
                    EventDispatcher.Instance.DispatchEvent(e);
                    return;
                }
                {
                    // foreach(var info in DataModel.FriendInfos)
                    var __enumerator5 = (DataModel.FriendInfos).GetEnumerator();
                    while (__enumerator5.MoveNext())
                    {
                        var info = __enumerator5.Current;
                        {
                            if (info.Guid == uid)
                            {
                                //已经是好友了
                                var e = new ShowUIHintBoard(270099);
                                EventDispatcher.Instance.DispatchEvent(e);
                                return;
                            }
                        }
                    }
                }
            }
                break;
            case 1:
            {
                var enemyMax = Table.GetClientConfig(321).Value.ToInt();
                if (DataModel.EnemyInfos.Count >= enemyMax)
                {
                    //ShowUIHintBoard e = new ShowUIHintBoard("仇人列表已满");
                    var e = new ShowUIHintBoard(220201);
                    EventDispatcher.Instance.DispatchEvent(e);
                    return;
                }
                {
                    // foreach(var info in DataModel.EnemyInfos)
                    var __enumerator6 = (DataModel.EnemyInfos).GetEnumerator();
                    while (__enumerator6.MoveNext())
                    {
                        var info = __enumerator6.Current;
                        {
                            if (info.Guid == uid)
                            {
                                //已经是仇人了
                                var e = new ShowUIHintBoard(270100);
                                EventDispatcher.Instance.DispatchEvent(e);
                                return;
                            }
                        }
                    }
                }
                //   NetManager.Instance.StartCoroutine(DelFriendByIdCoroutine( uid, type));
            }
                break;
            case 2:
            {
                var blackMax = Table.GetClientConfig(322).Value.ToInt();
                if (DataModel.BlackInfos.Count >= blackMax)
                {
                    //ShowUIHintBoard e = new ShowUIHintBoard("屏蔽列表已满");
                    var e = new ShowUIHintBoard(220202);
                    EventDispatcher.Instance.DispatchEvent(e);
                    return;
                }
                {
                    // foreach(var info in DataModel.BlackInfos)
                    var __enumerator7 = (DataModel.BlackInfos).GetEnumerator();
                    while (__enumerator7.MoveNext())
                    {
                        var info = __enumerator7.Current;
                        {
                            if (info.Guid == uid)
                            {
                                //已经屏蔽了
                                var e = new ShowUIHintBoard(270101);
                                EventDispatcher.Instance.DispatchEvent(e);
                                return;
                            }
                        }
                    }
                }
                //       NetManager.Instance.StartCoroutine(DelFriendByIdCoroutine( uid, type));
            }
                break;
        }
        NetManager.Instance.StartCoroutine(AddFriendByIdCoroutine(uid, type));
    }

    private IEnumerator AddFriendByIdCoroutine(ulong uid, int type)
    {
        using (new BlockingLayerHelper(0))
        {
            Logger.Info(".............AddFriendById..................");
            var msg = NetManager.Instance.AddFriendById(uid, type);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    AddFriendInfo(type, msg.Response);
                    switch (type)
                    {
                        case 0:
                        {
                            DataModel.EmptyTips[0] = false;
                            var e = new ShowUIHintBoard(270222);
                            EventDispatcher.Instance.DispatchEvent(e);

                            PlatformHelper.UMEvent("Friend", "Add", uid.ToString());
                        }
                            break;
                        case 1:
                        {
                            DataModel.EmptyTips[1] = false;
                            var e = new ShowUIHintBoard(270223);
                            EventDispatcher.Instance.DispatchEvent(e);
                        }
                            break;
                        case 2:
                        {
                            DataModel.EmptyTips[2] = false;
                            var e = new ShowUIHintBoard(270224);
                            EventDispatcher.Instance.DispatchEvent(e);

                            PlatformHelper.UMEvent("Friend", "PingBi", uid.ToString());
                        }
                            break;
                    }
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_FriendIsHave)
                {
                    switch (type)
                    {
                        case 0:
                        {
                            var e = new ShowUIHintBoard(270099);
                            EventDispatcher.Instance.DispatchEvent(e);
                        }
                            break;
                        case 1:
                        {
                            var e = new ShowUIHintBoard(270100);
                            EventDispatcher.Instance.DispatchEvent(e);
                        }
                            break;
                        case 2:
                        {
                            var e = new ShowUIHintBoard(270101);
                            EventDispatcher.Instance.DispatchEvent(e);
                        }
                            break;
                    }
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_FriendIsMore)
                {
                    //ShowUIHintBoard e = new ShowUIHintBoard("好友列表已满");
                    var e = new ShowUIHintBoard(220200);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_EnemyIsMore)
                {
                    //ShowUIHintBoard e = new ShowUIHintBoard("仇人列表已满");
                    var e = new ShowUIHintBoard(220201);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_ShieldIsMore)
                {
                    //ShowUIHintBoard e = new ShowUIHintBoard("屏蔽列表已满");
                    var e = new ShowUIHintBoard(220202);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error("AddFriendById errocode = {0}", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("AddFriendById state = {0}", msg.State);
            }
        }
    }

    private void AddFriendInfo(int type, CharacterSimpleData friend)
    {
        var list = new List<FriendInfoDataModel>(GetFriendInfo(type));
        var data = GetFriendInfoDataModel(friend);
        list.Add(data);
        list.Sort();
        var dataList = new ObservableCollection<FriendInfoDataModel>(list);
        SetFriendInfo(type, dataList);
    }

    public void AddShareItem(BagItemDataModel itemData)
    {
        var dataChatInfoNode = new ChatInfoNodeData();
        dataChatInfoNode.Type = (int) eChatLinkType.Equip;
        dataChatInfoNode.Id = itemData.ItemId;

        var nowItemExdataCount0 = itemData.Exdata.Count;
        for (var i = 0; i < nowItemExdataCount0; i++)
        {
            dataChatInfoNode.ExData.Add(itemData.Exdata[i]);
        }
        var str = "";
        using (var ms = new MemoryStream())
        {
            Serializer.Serialize(ms, dataChatInfoNode);
            var wrap = LZ4Codec.Encode32(ms.GetBuffer(), 0, (int)ms.Length);
            str = Convert.ToBase64String(wrap);
        }

        str = SpecialCode.ChatBegin + str + SpecialCode.ChatEnd;
        var tbTable = Table.GetItemBase(itemData.ItemId);
        var value = tbTable.Name;
        var color = GameUtils.GetTableColorString(tbTable.Quality);
        value = String.Format("[{0}][{1}][-]", color, value);

        mDicItemLink[value] = str;
        //         var inputStr = DataModel.InputChat + str;
        //         {
        //             var __enumerator3 = (mDicItemLink).GetEnumerator();
        //             while (__enumerator3.MoveNext())
        //             {
        //                 var i = __enumerator3.Current;
        //                 {
        //                     inputStr = inputStr.Replace(i.Key, i.Value);
        //                 }
        //             }
        //         }

        var inputStr = GameUtils.GetDictionaryText(100001058);
        if (DataModel.InputChat == inputStr)
        {
            DataModel.InputChat = value;
        }
        else
        {
            DataModel.InputChat += value;
        }
    }

    public void ApplyAllInfo()
    {
        for (var i = 0; i < 3; i++)
        {
            ApplyFriends(i);
        }
        ApplyRecentcontacts();

        RegisterSaveCache();
    }

    public void ApplyFriends(int type)
    {
        var e2 = new Close_UI_Event(UIConfig.OperationList);
        EventDispatcher.Instance.DispatchEvent(e2);
        NetManager.Instance.StartCoroutine(ApplyFriendsCoroutine(type));
    }

    private IEnumerator ApplyFriendsCoroutine(int type)
    {
        using (new BlockingLayerHelper(0))
        {
            Logger.Info(".............ApplyFriends..................");
            var msg = NetManager.Instance.ApplyFriends(type);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    UpdateFriendInfo(type, msg.Response.Characters);
                    if (msg.Response.Characters.Count == 0)
                    {
                        DataModel.EmptyTips[type] = true;
                    }
                    else
                    {
                        DataModel.EmptyTips[type] = false;
                    }
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error("ApplyFriends errocode = {0}", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("ApplyFriends state = {0}", msg.State);
            }
        }
    }

    private void ApplyPlayerheadInfo(ulong charId, Action<PlayerHeadInfoMsg> act)
    {
        NetManager.Instance.StartCoroutine(ApplyPlayerheadInfoEnumerator(charId, act));
    }

    public IEnumerator ApplyPlayerheadInfoEnumerator(ulong charId, Action<PlayerHeadInfoMsg> act)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ApplyPlayerHeadInfo(charId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var info = msg.Response;
                    if (act != null)
                    {
                        act(info);
                    }
                }
            }
        }
    }

    private void ApplyRecentcontacts()
    {
        NetManager.Instance.StartCoroutine(ApplyRecentcontactsCoroutine());
    }

    public IEnumerator ApplyRecentcontactsCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.GetRecentcontacts(-1);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    LoadChatHistory();

                    var list = msg.Response.Characters;
                    UpdateRecentcontacts(list);
                }
            }
        }
    }

    private void CheckUpdateInfo(ContactInfoDataModel info, int level, int ladder)
    {
        if (info.Ladder != ladder || info.Level != level)
        {
            info.Ladder = ladder;
            info.Level = level;

            UpdateChatRecord(info.CharacterId, info.Level, info.Ladder);
        }
    }

    private void CleacChatRecord()
    {
        DataModel.SelectContact.UnreadCount = 0;
        UpdateNoticeUnreadCount();
        mLoadSeekPostion = 0;
        DataModel.ChatMessages = new ObservableCollection<FriendMessageDataModel>();

        var charId = DataModel.SelectContact.CharacterId;
        List<ChatMessageData> list;
        if (mUnWriteListCaches.TryGetValue(charId, out list))
        {
            mUnWriteListCaches.Remove(charId);
        }

        DelectChatRecord(charId);
        DelectRecentcontacts(charId);
    }

    private void DelectChatRecord(ulong charId)
    {
        var fileName = Path.Combine(mCharacterChatDirectory, charId.ToString());
        var hasFile = File.Exists(fileName);
        if (hasFile == false)
        {
            return;
        }
        File.Delete(fileName);
    }

    private void DelectRecentcontacts(ulong characterId)
    {
        NetManager.Instance.StartCoroutine(DelectRecentcontactsCoroutine(characterId));
    }

    public IEnumerator DelectRecentcontactsCoroutine(ulong characterId)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.DeleteRecentcontacts(characterId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                }
            }
        }
    }

    private IEnumerator DelFriendByIdCoroutine(ulong uid, int type)
    {
        using (new BlockingLayerHelper(0))
        {
            Logger.Info(".............DelFriendById..................");
            var msg = NetManager.Instance.DelFriendById(uid, type);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    DelFriendInfo(type, uid);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error("DelFriendById errocode = {0}", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("DelFriendById state = {0}", msg.State);
            }
        }
    }

    private void DelFriendInfo(int type, ulong characterId)
    {
        var infos = GetFriendInfo(type);
        FriendInfoDataModel delInfo = null;
        {
            // foreach(var info in infos)
            var __enumerator9 = (infos).GetEnumerator();
            while (__enumerator9.MoveNext())
            {
                var info = __enumerator9.Current;
                {
                    if (info.Guid == characterId)
                    {
                        delInfo = info;
                        break;
                    }
                }
            }
        }
        infos.Remove(delInfo);
        if (infos.Count == 0)
        {
            DataModel.EmptyTips[type] = true;
        }
    }

    private ContactInfoDataModel GetChatFileInfo(string fullName, string fileName)
    {
        var charId = fileName.ToUlong();
        using (var fs = new FileStream(fullName, FileMode.Open, FileAccess.Read))
        {
            var type = 0;
            var lv = 0;
            var ladder = 0;
            var charName = "";
            var buffer = new byte[4];
            {
                try
                {
                    fs.Read(buffer, 0, 4);
                    lv = SerializerUtility.ReadInt(buffer, 0);

                    fs.Read(buffer, 0, 4);
                    ladder = SerializerUtility.ReadInt(buffer, 0);

                    fs.Read(buffer, 0, 4);
                    type = SerializerUtility.ReadInt(buffer, 0);

                    fs.Read(buffer, 0, 4);
                    var length = SerializerUtility.ReadInt(buffer, 0);
                    var data = new byte[length];
                    fs.Read(data, 0, length);
                    charName = Encoding.UTF8.GetString(data);
                }
                catch (Exception)
                {
                    fs.Close();
                    DelectChatRecord(charId);
                    return null;
                }
            }

            fs.Close();
            var info = new ContactInfoDataModel();
            info.Name = charName;
            info.HasUpdate = false;
            info.CharacterId = charId;
            info.Level = lv;
            info.Type = type;
            info.Ladder = ladder;
            return info;
        }
    }

    private ContactInfoDataModel GetContactInfoData(ulong id)
    {
        foreach (var info in DataModel.ContactInfos)
        {
            if (info.CharacterId == id)
            {
                return info;
            }
        }
        return null;
    }

    public FriendInfoDataModel GetFiendInfo(int type, ulong guid)
    {
        switch (type)
        {
            case 0:
            {
                var count = DataModel.FriendInfos.Count;
                for (var i = 0; i < count; i++)
                {
                    var info = DataModel.FriendInfos[i];
                    if (info.Guid == guid)
                    {
                        return info;
                    }
                }
            }
                break;
            case 1:
            {
                var count = DataModel.EnemyInfos.Count;
                for (var i = 0; i < count; i++)
                {
                    var info = DataModel.EnemyInfos[i];
                    if (info.Guid == guid)
                    {
                        return info;
                    }
                }
            }
                break;
            case 2:
            {
                var count = DataModel.BlackInfos.Count;
                for (var i = 0; i < count; i++)
                {
                    var info = DataModel.BlackInfos[i];
                    if (info.Guid == guid)
                    {
                        return info;
                    }
                }
            }
                break;
        }
        return null;
    }

    private ObservableCollection<FriendInfoDataModel> GetFriendInfo(int type)
    {
        switch (type)
        {
            case 0:
            {
                return DataModel.FriendInfos;
            }
                break;
            case 1:
            {
                return DataModel.EnemyInfos;
            }
                break;
            case 2:
            {
                return DataModel.BlackInfos;
            }
                break;
            case 3:
            {
                return DataModel.SeekInfos;
            }
                break;
        }
        return null;
    }

    private FriendInfoDataModel GetFriendInfoDataModel(CharacterSimpleData friend, FriendInfoDataModel data = null)
    {
        if (data == null)
        {
            data = new FriendInfoDataModel();
        }
        data.Guid = friend.Id;
        data.Name = friend.Name;
        data.Level = friend.Level;
        data.TypeId = friend.TypeId;
        data.SceneId = friend.SceneId;
        data.FightPoint = friend.FightPoint;
        data.Ladder = friend.Ladder;
        data.ServerId = friend.ServerId;
        var serverName = data.ServerName;
        PlayerDataManager.Instance.ServerNames.TryGetValue(data.ServerId, out serverName);
        data.ServerName = serverName;
        data.IsOnline = friend.Online == 1;
        var str = GameUtils.GetDictionaryText(240607);
        str = string.Format(str, data.Level, data.Ladder);
        data.LevelInfo = str;
        str = GameUtils.GetDictionaryText(240606);
        str = string.Format(str, data.FightPoint);
        data.FightPointInfo = str;
        return data;
    }

    private void InsetContactInfo(ContactInfoDataModel info)
    {
        DataModel.ContactInfos.Insert(0, info);
        UpdateContactCellIndex();
        info.NextIndex = 0;
        AddContactCellNextIndex(info.Index);
    }

    private bool IsInBalckListId(ulong id)
    {
        var count = DataModel.BlackInfos.Count;
        for (var i = 0; i < count; i++)
        {
            var info = DataModel.BlackInfos[i];
            if (info.Guid == id)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsInBalckListName(string name)
    {
        var count = DataModel.BlackInfos.Count;
        for (var i = 0; i < count; i++)
        {
            var info = DataModel.BlackInfos[i];
            if (info.Name == name)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsInEnemyListId(ulong id)
    {
        var count = DataModel.EnemyInfos.Count;
        for (var i = 0; i < count; i++)
        {
            var info = DataModel.EnemyInfos[i];
            if (info.Guid == id)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsInFriendListId(ulong id)
    {
        var count = DataModel.FriendInfos.Count;
        for (var i = 0; i < count; i++)
        {
            var info = DataModel.FriendInfos[i];
            if (info.Guid == id)
            {
                return true;
            }
        }
        return false;
    }

    private List<FriendMessageDataModel> LoadCharacterChatRecord(ulong charId)
    {
        mLoadSeekPostion = 0;
        var ret = new List<FriendMessageDataModel>();
        List<ChatMessageData> list;
        if (mUnWriteListCaches.TryGetValue(charId, out list) && list.Count > 0)
        {
            for (var i = list.Count - 1; i >= 0; i--)
            {
                var msg = new FriendMessageDataModel();
                msg.MessageData = list[i];
                ret.Insert(0, msg);
            }
            return ret;
        }
        ret = LoadCharRecord(charId);
        return ret;
    }

    private List<FriendMessageDataModel> LoadCharRecord(ulong charId)
    {
        var msgList = new List<FriendMessageDataModel>();
        var fileName = Path.Combine(mCharacterChatDirectory, charId.ToString());
        var hasFile = File.Exists(fileName);
        if (hasFile == false)
        {
            return msgList;
        }

        using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
            var buffer = new byte[4];
            var length = 0;
            for (var i = 0; i < PageChatCount; i++)
            {
                mLoadSeekPostion -= 4;
                fs.Seek(mLoadSeekPostion, SeekOrigin.End);
                fs.Read(buffer, 0, 4);
                length = SerializerUtility.ReadInt(buffer, 0);
                try
                {
                    mLoadSeekPostion -= length;
                    fs.Seek(mLoadSeekPostion, SeekOrigin.End);
                    var bytes = new byte[length];
                    fs.Read(bytes, 0, length);
                    ChatMessageData data;
                    using (var ms = new MemoryStream(bytes, false))
                    {
                        if (ms.Length == 0)
                        {
                            mLoadSeekPostion += 4;
                            mLoadSeekPostion += length;
                            break;
                        }
                        data = Serializer.Deserialize<ChatMessageData>(ms);
                        if (data == null)
                        {
                            mLoadSeekPostion += 4;
                            mLoadSeekPostion += length;
                            break;
                        }
                        if (data.SoundData != null && data.Content == "")
                        {
//没有记录翻译的再请求翻译一遍
                            var e = new ChatSoundTranslateAddEvent(data.SoundData);
                            EventDispatcher.Instance.DispatchEvent(e);
                        }
                        var msg = new FriendMessageDataModel();
                        msg.MessageData = data;
                        msgList.Insert(0, msg);
                    }
                }
                catch (Exception)
                {
                    mLoadSeekPostion += 4;
                    mLoadSeekPostion += length;
                    break;
                }
            }
        }
        var c = msgList.Count;
        var lastTime = DateTime.Now;
        for (var i = 0; i < msgList.Count; i++)
        {
            var msg = msgList[i];
            var msgInfo = msg.MessageData as ChatMessageData;
            if (i == 0)
            {
                msgInfo.ShowTime = 1;
                lastTime = DateTime.FromBinary(msgInfo.Times);
                continue;
            }
            var msgTime = DateTime.FromBinary(msgInfo.Times);
            var dif = msgTime - lastTime;
            lastTime = msgTime;
            if (dif.TotalMinutes > 5)
            {
                msgInfo.ShowTime = 1;
            }
            else
            {
                msgInfo.ShowTime = 0;
            }
        }

        return msgList;
    }

    private void LoadChatHistory()
    {
        var charId = PlayerDataManager.Instance.GetGuid();
        var chatDirectory = "";
#if !UNITY_EDITOR
        chatDirectory = Path.Combine(Application.temporaryCachePath, "ChatHistory");
#else
        chatDirectory = Path.Combine(Directory.GetCurrentDirectory(), "ChatHistory");
#endif


        if (!Directory.Exists(chatDirectory))
        {
            Directory.CreateDirectory(chatDirectory);
        }
        mCharacterChatDirectory = Path.Combine(chatDirectory, charId.ToString());
        if (!Directory.Exists(mCharacterChatDirectory))
        {
            Directory.CreateDirectory(mCharacterChatDirectory);
            return;
        }
        var folder = new DirectoryInfo(mCharacterChatDirectory);
        var files = folder.GetFiles();
        var list = new List<ContactInfoDataModel>();
        if (files.Length > 0)
        {
            var fileList = new List<FileInfo>(files);
            fileList.Sort((l, r) => { return (int) (r.LastWriteTime - l.LastWriteTime).TotalMilliseconds; });

            foreach (var file in fileList)
            {
                var fileName = file.FullName;
                var info = GetChatFileInfo(fileName, file.Name);
                if (info != null)
                {
                    list.Add(info);
                }
            }
        }
        DataModel.ContactInfos = new ObservableCollection<ContactInfoDataModel>(list);
        UpdateContactCellIndex();
        SelectFriendContactCell(0);
    }

    private void LoadMore()
    {
        var info = DataModel.SelectContact;
        var showList = LoadCharRecord(info.CharacterId);
        if (showList == null)
        {
            return;
        }

        var count = showList.Count;

        if (count > 0)
        {
            if (DataModel.ChatMessages.Count > 0)
            {
                var bottom = showList[count - 1].MessageData as ChatMessageData;
                var top = DataModel.ChatMessages[0].MessageData as ChatMessageData;
                if (bottom != null && top != null)
                {
                    if (top.ShowTime == 1)
                    {
                        var dif = DateTime.FromBinary(top.Times) - DateTime.FromBinary(bottom.Times);
                        if (dif.TotalMinutes < 5)
                        {
                            top.ShowTime = 0;
                        }
                    }
                }
            }

            for (var i = count - 1; i >= 0; i--)
            {
                var msg = showList[i];

                DataModel.ChatMessages.Insert(0, msg);
            }
        }
    }

    private void OnAddFaceNode(IEvent ievent)
    {
        var e = ievent as AddFaceNode;
        if (e.Type != 3)
        {
            return;
        }
        AddFace(e.FaceId);
    }

    public void OnChatPrivate(IEvent ievent)
    {
        var e = ievent as ChatMainPrivateChar;
        var arg = new FriendArguments();
        arg.Type = 1;
        arg.Data = e.Data;
        if (State == FrameState.Open)
        {
            RefreshData(arg);
        }
        else
        {
            var e1 = new Show_UI_Event(UIConfig.FriendUI, arg);
            EventDispatcher.Instance.DispatchEvent(e1);
        }
    }

    public void OnChatShareItem(IEvent ievent)
    {
        var e = ievent as ChatShareItemEvent;
        if (e.Type != 3)
        {
            return;
        }
        AddShareItem(e.Data);
    }

    private void OnChatVoiceContent(IEvent ievent)
    {
        var e = ievent as ChatVoiceContent;
        foreach (var listCach in mUnWriteListCaches)
        {
            foreach (var data in listCach.Value)
            {
                if (data.SoundData == e.SoundData)
                {
                    data.Content = e.Content;
                    return;
                }
            }
        }
    }

    private void OnCleanContact()
    {
        DataModel.SelectContact.IsSelect = false;
        DataModel.SelectContact = EmptyContactInfo;
    }

    private void OnClicCharInfo(IEvent ievent)
    {
        var e = ievent as FriendClickShowInfo;

        var tabid = 0;
        var DataModelIsSelectTabCount0 = DataModel.IsSelectTab.Count;
        for (var i = 0; i < DataModelIsSelectTabCount0; i++)
        {
            if (DataModel.IsSelectTab[i])
            {
                if (i == 0)
                {
                    tabid = 7;
                    break;
                }
                if (i == 1)
                {
                    tabid = 8;
                    break;
                }
                if (i == 2)
                {
                    tabid = 9;
                    break;
                }
                if (i == 3)
                {
                    tabid = 10;
                    break;
                }
            }
        }
        var d = e.Data;
        PlayerDataManager.Instance.ShowCharacterPopMenu(d.Guid, d.Name, tabid, d.Level, d.Ladder, d.TypeId);
    }

    private void OnClicCharSeek(IEvent ievent)
    {
        var e = ievent as FriendSeekBtnClick;
        var t = e.Type;

        for (var i = 0; i < DataModel.IsSelectTab.Count; i++)
        {
            DataModel.IsSelectTab[i] = false;
        }
        DataModel.IsSelectTab[3] = true;
        DataModel.SelectToggle = 3;
        if (t == 0)
        {
            if ((Game.Instance.ServerTime - mCdTimeList[1]).TotalSeconds < 3)
            {
//请勿频繁查询
                GameUtils.ShowHintTip(220217);
                return;
            }
            mCdTimeList[1] = Game.Instance.ServerTime;
            SeekFriends();
        }
        else
        {
            if ((Game.Instance.ServerTime - mCdTimeList[0]).TotalSeconds < 3)
            {
//请勿频繁征友
                GameUtils.ShowHintTip(220216);
                return;
            }
            mCdTimeList[0] = Game.Instance.ServerTime;
            QuickSeekFriends();
        }
    }

    private void OnFriendAyncAdd(IEvent ievent)
    {
        var e = ievent as FriendAddSyncEvent;
        AddFriendInfo(e.Type, e.Data);
    }

    private void OnFriendAyncDel(IEvent ievent)
    {
        var e = ievent as FriendDelSyncEvent;
        DelFriendInfo(e.Type, e.CharacterId);
    }

    private void OnFriendAyncUpdate(IEvent ievent)
    {
        var e = ievent as FriendUpdateSyncEvent;
        var infos = e.Data.Characters;
        var fiendChange = false;
        var enemyChange = false;
        var blackChange = false;
        {
            // foreach(var info in infos)
            var __enumerator4 = (infos).GetEnumerator();
            while (__enumerator4.MoveNext())
            {
                var info = __enumerator4.Current;
                {
                    foreach (var friendInfo in DataModel.FriendInfos)
                    {
                        if (friendInfo.Guid == info.Id)
                        {
                            GetFriendInfoDataModel(info, friendInfo);
                            fiendChange = true;
                            break;
                        }
                    }
                    foreach (var enemyInfo in DataModel.EnemyInfos)
                    {
                        if (enemyInfo.Guid == info.Id)
                        {
                            GetFriendInfoDataModel(info, enemyInfo);
                            enemyChange = true;
                            break;
                        }
                    }
                    foreach (var blackInfo in DataModel.BlackInfos)
                    {
                        if (blackInfo.Guid == info.Id)
                        {
                            GetFriendInfoDataModel(info, blackInfo);
                            blackChange = true;
                            break;
                        }
                    }
                }
            }
        }
        if (fiendChange)
        {
            var list = new List<FriendInfoDataModel>(DataModel.FriendInfos);
            list.Sort();
            SetFriendInfo(0, new ObservableCollection<FriendInfoDataModel>(list));
        }
        if (enemyChange)
        {
            var list = new List<FriendInfoDataModel>(DataModel.EnemyInfos);
            list.Sort();
            SetFriendInfo(1, new ObservableCollection<FriendInfoDataModel>(list));
        }
        if (blackChange)
        {
            var list = new List<FriendInfoDataModel>(DataModel.BlackInfos);
            list.Sort();
            SetFriendInfo(2, new ObservableCollection<FriendInfoDataModel>(list));
        }
    }

    private void OnFriendClickTabEvent(IEvent ievent)
    {
        var e = ievent as FriendClickTabEvent;
        for (var i = 0; i < DataModel.IsSelectTab.Count; i++)
        {
            DataModel.IsSelectTab[i] = false;
        }
        DataModel.IsSelectTab[e.Type] = true;
        if (e.Type != 4)
        {
            mAsyncState = -1;
            OnCleanContact();
            WriteChacheRecords();
        }
        else
        {
            SelectFriendContactCell(0);
        }
    }

    private void OnFriendClickType(IEvent ievent)
    {
        var e = ievent as FriendClickType;
        switch (e.Type)
        {
            case 1:
            {
                SendChatMessage();
            }
                break;
            case 2:
            {
                LoadMore();
            }
                break;
            case 3:
            {
                CleacChatRecord();
            }
                break;
            case 4:
            {
                WriteChacheRecords();
                UpdateContactOrder(true);
            }
                break;
            case 5:
            case 6:
            {
                OnInputForcus(e.Type);
            }
                break;
        }
    }

    private void OnFriendContactCell(IEvent ievent)
    {
        var e = ievent as FriendContactCell;
        SelectFriendContactCell(e.Index);
    }

    private void OnFriendContactAddFriend(IEvent ievent)
    {
        var e = ievent as FriendContactClickAddFriend;

        var addFriendEvent = new FriendOperationEvent(0, 1, e.Data.Name, e.Data.Guid);
        EventDispatcher.Instance.DispatchEvent(addFriendEvent);
    }

    private void OnFriendOperation(IEvent ievent)
    {
        var e = ievent as FriendOperationEvent;
        var ft = e.FriendType;
        if (e.OperationType == 1)
        {
            AddFriend(e.Id, e.FriendType);
        }
        else
        {
            NetManager.Instance.StartCoroutine(DelFriendByIdCoroutine(e.Id, e.FriendType));
        }
    }

    private void OnFriendReceive(IEvent ievent)
    {
        var e = ievent as FriendReceive;
        if (mAsyncState == -1)
        {
            return;
        }

        var datas = e.Data.Characters;
        DataModel.SeekInfos.Clear();
        if (datas.Count == 0)
        {
            //没有查找结果
            DataModel.EmptyTips[3] = true;
            var e1 = new ShowUIHintBoard(270103);
            EventDispatcher.Instance.DispatchEvent(e1);
        }
        else
        {
            DataModel.EmptyTips[3] = false;
            UpdateFriendInfo(3, datas);
        }

        var e2 = new FriendNotify(1);
        EventDispatcher.Instance.DispatchEvent(e2);
    }

    private void OnInputForcus(int type)
    {
        switch (type)
        {
            case 5:
            {
                if (DataModel.InputChat == inputStr)
                {
                    DataModel.InputChat = string.Empty;
                }
            }
                break;
            case 6:
            {
                if (DataModel.InputSeek == inputStr2)
                {
                    DataModel.InputSeek = string.Empty;
                }
            }
                break;
        }
    }

    private void OnSendVoiceChat(IEvent ievent)
    {
        var selfGuid = PlayerDataManager.Instance.GetGuid();
        var e = ievent as ChatMainSendVoiceData;

        if (e.IsWhisper == false)
        {
            return;
        }
        if (e.VoiceData.Length < 1)
        {
            Logger.Debug("voiceData.Length < 1");
            return;
        }

        if (e.VoiceLength < 0.5f)
        {
            //时间太短
            Logger.Debug("record time < 0.5s");
            return;
        }

        var speakTime = (int) Math.Ceiling(e.VoiceLength);


        var charData = DataModel.SelectContact;
        var chatContent = "/" + charData.Name + " " + speakTime;
        var content = new ChatMessageContent
        {
            Content = chatContent,
            SoundData = e.VoiceData
        };

        NetManager.Instance.StartCoroutine(SendChatMessageEnumerator((int) eChatChannel.Whisper, content, selfGuid,
            charData.Name));
    }

    private void OnWhisperChatMessage(IEvent ievent)
    {
        var e = ievent as WhisperChatMessage;
        var message = new FriendMessageDataModel();
        message.MessageData = e.DataModel;
        var msgInfo = e.DataModel;
        var type = 0;
        var level = 0;
        var ladder = 0;
        if (DataModel.SelectContact.CharacterId == msgInfo.CharId)
        {
            var info = message.MessageData as ChatMessageData;
            if (info == null)
            {
                return;
            }
            type = DataModel.SelectContact.Type;
            level = DataModel.SelectContact.Level;
            ladder = DataModel.SelectContact.Ladder;
            if (DataModel.ChatMessages.Count > 0)
            {
                var last = DataModel.ChatMessages[DataModel.ChatMessages.Count - 1].MessageData as ChatMessageData;
                if (last == null)
                {
                    return;
                }
                var dif = DateTime.FromBinary(info.Times) - DateTime.FromBinary(last.Times);
                if (dif.TotalMinutes > 5)
                {
                    info.ShowTime = 1;
                }
            }
            else
            {
                info.ShowTime = 1;
            }
            if (info.Type == (int) eChatChannel.MyWhisper)
            {
                info.RoleId = PlayerDataManager.Instance.GetRoleId();
            }
            else if (info.Type == (int) eChatChannel.Whisper)
            {
                info.RoleId = DataModel.SelectContact.Type;
            }
            DataModel.ChatMessages.Add(message);
            if (DataModel.SelectContact.CharacterId != msgInfo.CharId)
            {
                DataModel.SelectContact.CharacterId = msgInfo.CharId;
            }

            DataModel.SelectContact.NextIndex = 0;
            AddContactCellNextIndex(DataModel.SelectContact.NextIndex);

            if (State == FrameState.Close)
            {
                DataModel.SelectContact.UnreadCount++;
                UpdateNoticeUnreadCount();
            }
            //             if (DataModel.SelectContact.Index != 0)
            //             {
            //                 DataModel.ContactInfos.Remove(DataModel.SelectContact);
            //                 InsetContactInfo(DataModel.SelectContact);
            //             }
        }
        else
        {
            var info = GetContactInfoData(msgInfo.CharId);
            if (info != null)
            {
                info.UnreadCount++;
                UpdateNoticeUnreadCount();
                msgInfo.RoleId = info.Type;
                info.NextIndex = 0;
                AddContactCellNextIndex(info.Index);
                UpdateContactOrder(false);
            }
            else
            {
//创建联系人
                AddContactCellInfo(msgInfo.CharId, msgInfo.Name, message);
                return;
            }
        }
        AddChatRecord(msgInfo.CharId, message);
    }

    public void QuickSeekFriends(bool showNoTip = true)
    {
        NetManager.Instance.StartCoroutine(SeekFriendsCoroutine(showNoTip));
    }

    private void RegisterSaveCache()
    {
        mSaveListCaches = new Dictionary<ulong, List<ChatMessageData>>();

        if (mSaveTImerTrigger != null)
        {
            TimeManager.Instance.DeleteTrigger(mSaveTImerTrigger);
        }

        mSaveTImerTrigger = TimeManager.Instance.CreateTrigger(Game.Instance.ServerTime, TickSaveCache, 1000*10);
    }

    private void SaveChatRecordList(ContactInfoDataModel info, List<ChatMessageData> list)
    {
        var charId = info.CharacterId;
        var name = info.Name;
        var type = info.Type;
        var level = info.Level;
        var ladder = info.Ladder;

        var fileName = Path.Combine(mCharacterChatDirectory, charId.ToString());
        var hasFile = File.Exists(fileName);
        var buffer = new byte[4];
        using (var fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
        {
            if (hasFile == false)
            {
                SerializerUtility.WriteInt(buffer, level, 0);
                fs.Write(buffer, 0, 4);
                SerializerUtility.WriteInt(buffer, ladder, 0);
                fs.Write(buffer, 0, 4);

                SerializerUtility.WriteInt(buffer, type, 0);
                fs.Write(buffer, 0, 4);

                var bytes = Encoding.UTF8.GetBytes(name);
                SerializerUtility.WriteInt(buffer, bytes.Length, 0);
                fs.Write(buffer, 0, 4);
                fs.Write(bytes, 0, bytes.Length);
            }
            fs.Seek(0, SeekOrigin.End);
            foreach (var data in list)
            {
                using (var ms = new MemoryStream())
                {
                    Serializer.Serialize(ms, data);
                    var bytes = ms.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                    SerializerUtility.WriteInt(buffer, bytes.Length, 0);
                    fs.Write(buffer, 0, 4);
                }
            }
        }
    }

    private IEnumerator SeekCharactersCoroutine(string name)
    {
        using (new BlockingLayerHelper(0))
        {
            Logger.Info(".............SeekCharacters..................");
            var msg = NetManager.Instance.SeekCharacters(name);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    mAsyncState = 3;
                    var e2 = new FriendNotify(2);
                    EventDispatcher.Instance.DispatchEvent(e2);
//                     var datas = msg.Response.Characters;
//                     DataModel.SeekInfos.Clear();
//                     if (datas.Count == 0)
//                     {
//                         //没有查找结果
//                         DataModel.EmptyTips[3] = true;
//                         ShowUIHintBoard e = new ShowUIHintBoard(270103);
//                         EventDispatcher.Instance.DispatchEvent(e);
//                     }
//                     else
//                     {
//                         DataModel.EmptyTips[3] = false;
//                         UpdateFriendInfo(3, datas);
//                     }
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Unknow)
                {
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_StringIsNone)
                {
                    var e = new ShowUIHintBoard(msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error("SeekCharacters errocode = {0}", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("SeekCharacters state = {0}", msg.State);
            }
        }
    }

    public void SeekFriends()
    {
        if (string.IsNullOrEmpty(DataModel.InputSeek))
        {
            //输入的名字不能为空
            var e1 = new ShowUIHintBoard(270102);
            EventDispatcher.Instance.DispatchEvent(e1);
            return;
        }


        NetManager.Instance.StartCoroutine(SeekCharactersCoroutine(DataModel.InputSeek));
    }

    private IEnumerator SeekFriendsCoroutine(bool showNoTip)
    {
        using (new BlockingLayerHelper(0))
        {
			ShowUIHintBoard e = new ShowUIHintBoard(220974);
			EventDispatcher.Instance.DispatchEvent(e);

            Logger.Info(".............SeekFriends..................");
            var msg = NetManager.Instance.SeekFriends("");
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    mAsyncState = 3;
                    var e2 = new FriendNotify(2);
                    EventDispatcher.Instance.DispatchEvent(e2);


					
//                     var datas = msg.Response.Characters;
//                     DataModel.SeekInfos.Clear();
//                     if (datas.Count == 0)
//                     {
//                         if (showNoTip)
//                         {
//                             ShowUIHintBoard e = new ShowUIHintBoard(270103);
//                             EventDispatcher.Instance.DispatchEvent(e);
//                         }
//                         DataModel.EmptyTips[3] = true;
//                     }
//                     else
//                     {
//                         DataModel.EmptyTips[3] = false;
//                         UpdateFriendInfo(3, datas);
//                     }
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Unknow)
                {
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error("SeekFriends errocode = {0}", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("SeekFriends state = {0}", msg.State);
            }
        }
    }

    private void SelectFriendContactCell(int index)
    {
        if (index < 0 || index >= DataModel.ContactInfos.Count)
        {
            return;
        }
        //         if (DataModel.SelectContact.Index == index)
        //         {
        //             return;
        //         }
        DataModel.SelectContact.IsSelect = false;
        var info = DataModel.ContactInfos[index];
        DataModel.SelectContact = info;
        DataModel.SelectContact.IsSelect = true;
        DataModel.SelectContact.UnreadCount = 0;
        UpdateNoticeUnreadCount();

        SetCharacterChatRecord(DataModel.SelectContact.CharacterId);
        if (DataModel.SelectContact.HasUpdate == false)
        {
            ApplyPlayerheadInfo(DataModel.SelectContact.CharacterId, UpdateSelectInfo);
        }
    }

    //------------------------------------------------------Chat-----------------------------
    private void SendChatMessage()
    {
        if (string.IsNullOrEmpty(DataModel.InputChat))
        {
            GameUtils.ShowHintTip(270054);
            return;
        }

        var charData = DataModel.SelectContact;
        var chatContent = DataModel.InputChat;
        var lenth = chatContent.GetStringLength();
        if (lenth > GameUtils.ChatWorldCount)
        {
            //字数太长了
            var str = GameUtils.GetDictionaryText(2000002);
            str = string.Format(str, GameUtils.ChatWorldCount);
            var e1 = new ShowUIHintBoard(str);
            EventDispatcher.Instance.DispatchEvent(e1);
            return;
        }
        foreach (var i in mDicItemLink)
        {
            chatContent = chatContent.Replace(i.Key, i.Value);
        }

        chatContent = chatContent.RemoveColorFalg();

        chatContent = "/" + charData.Name + " " + chatContent;
        var content = new ChatMessageContent {Content = chatContent};
        var selfGuid = PlayerDataManager.Instance.GetGuid();
        NetManager.Instance.StartCoroutine(SendChatMessageEnumerator((int) eChatChannel.Whisper, content, selfGuid,
            charData.Name));
    }

    public IEnumerator SendChatMessageEnumerator(int chatType,
                                                 ChatMessageContent content,
                                                 ulong characterId,
                                                 string targerName)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ChatChatMessage(chatType, content, characterId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                DataModel.InputChat = "";
                mDicItemLink.Clear();
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    //SetChannelChatCd(chatType);
                    PlatformHelper.UMEvent("Chat", chatType.ToString(), characterId.ToString());
                }
                else if (msg.ErrorCode == (int) ErrorCodes.NameNotFindCharacter)
                {
                    //玩家名字不存在                
                    var e1 =
                        new ChatMainHelpMeesage(string.Format(GameUtils.GetDictionaryText(2000001), targerName));
                    EventDispatcher.Instance.DispatchEvent(e1);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_SetRefuseWhisper)
                {
                    var e1 =
                        new ChatMainHelpMeesage(string.Format(GameUtils.GetDictionaryText(998), targerName));
                    EventDispatcher.Instance.DispatchEvent(e1);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_SetShieldYou)
                {
                    //{0}屏蔽了你
                    var str = string.Format(GameUtils.GetDictionaryText(270056), targerName);
                    var e1 = new ChatMainHelpMeesage(str);
                    EventDispatcher.Instance.DispatchEvent(e1);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_SetYouShield)
                {
                    //{0}屏蔽了你
                    var str = string.Format(GameUtils.GetDictionaryText(270055), targerName);
                    var e1 = new ChatMainHelpMeesage(str);
                    EventDispatcher.Instance.DispatchEvent(e1);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_ChatNone
                         || msg.ErrorCode == (int) ErrorCodes.Error_ChatLengthMax
                         || msg.ErrorCode == (int) ErrorCodes.Error_WhisperNameNone
                         || msg.ErrorCode == (int) ErrorCodes.Error_NotWhisperSelf)
                {
                    var e = new ShowUIHintBoard(200000000 + msg.ErrorCode);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else
                {
                    var e = new ShowUIHintBoard(200000000 + msg.ErrorCode);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
            }
            else
            {
                Logger.Error("SendChatMessage Error!............State..." + msg.State);
            }
        }
    }

    private void SetCharacterChatRecord(ulong charId)
    {
        var showList = LoadCharacterChatRecord(charId);
        DataModel.ChatMessages = new ObservableCollection<FriendMessageDataModel>(showList);
    }

    private void SetContactChatInfo(ulong charId, string name, int ladder, int lv, int roleId)
    {
        ContactInfoDataModel addInfo = null;
        var flag = 0;
        foreach (var info in DataModel.ContactInfos)
        {
            flag++;
            if (info.CharacterId == charId)
            {
                if (flag == 0)
                {
                    SelectFriendContactCell(0);
                    CheckUpdateInfo(DataModel.SelectContact, addInfo.Level, addInfo.Ladder);
                    return;
                }
                addInfo = info;
                break;
            }
        }
        if (addInfo != null)
        {
            DataModel.ContactInfos.Remove(addInfo);
            InsetContactInfo(addInfo);
            SelectFriendContactCell(0);
            CheckUpdateInfo(DataModel.SelectContact, lv, ladder);
            UpdateContactCellNextIndex();
        }
        else
        {
            if (roleId != -1 && ladder != -1 && lv != -1 && !string.IsNullOrEmpty(name))
            {
                addInfo = new ContactInfoDataModel();
                addInfo.Name = name;
                addInfo.HasUpdate = true;
                addInfo.CharacterId = charId;
                addInfo.Ladder = ladder;
                addInfo.Level = lv;
                addInfo.Type = roleId;
                InsetContactInfo(addInfo);
                SelectFriendContactCell(0);
                UpdateContactCellNextIndex();
            }
            else
            {
//信息不全时，请求网络
                ApplyPlayerheadInfo(charId, AddContactInfo);
            }
        }
    }

    private void SetFriendInfo(int type, ObservableCollection<FriendInfoDataModel> info)
    {
        switch (type)
        {
            case 0:
            {
                DataModel.FriendInfos = info;
                if (DataModel.FriendInfos.Count == 0)
                {
                    DataModel.HasFriends = false;
                }
                else
                {
                    DataModel.HasFriends = true;
                }
            }
                break;
            case 1:
            {
                DataModel.EnemyInfos = info;
            }
                break;
            case 2:
            {
                DataModel.BlackInfos = info;
            }
                break;
            case 3:
            {
                DataModel.SeekInfos = info;
                foreach (var i in DataModel.SeekInfos)
                {
                    i.IsShowAddFriend = 1;
                }
            }
                break;
        }
        if (info.Count == 0)
        {
            DataModel.EmptyTips[type] = true;
        }
        else
        {
            DataModel.EmptyTips[type] = false;
        }
    }

    private void TickSaveCache()
    {
        if (State != FrameState.Close)
        {
//只在关闭时调用
            return;
        }
        if (mUnWriteListCaches.Count == 0)
        {
            return;
        }

        mSaveListCaches.Clear();

        //筛选要保存的
        foreach (var listCach in mUnWriteListCaches)
        {
            var charId = listCach.Key;
            List<ChatMessageData> list = null;
            if (!mSaveListCaches.TryGetValue(charId, out list))
            {
                list = new List<ChatMessageData>();
                mSaveListCaches.Add(charId, list);
            }
            foreach (var messageData in listCach.Value)
            {
                if (messageData.SoundData != null && messageData.Content == "")
                {
                    break;
                }
                list.Add(messageData);
            }
        }
        //从cache中删除
        foreach (var listCach in mSaveListCaches)
        {
            var charId = listCach.Key;
            List<ChatMessageData> list = null;
            if (mUnWriteListCaches.TryGetValue(charId, out list))
            {
                foreach (var messageData in listCach.Value)
                {
                    list.Remove(messageData);
                }
            }
        }

        //写入文件
        foreach (var listCach in mSaveListCaches)
        {
            var charId = listCach.Key;
            if (listCach.Value.Count == 0)
            {
                continue;
            }
            var contactInfo = GetContactInfoData(charId);
            if (contactInfo == null)
            {
                continue;
            }
            SaveChatRecordList(contactInfo, listCach.Value);
        }

        mSaveListCaches.Clear();
    }

    private void UpdateChatRecord(ulong charId, int lv, int ladder)
    {
        var fileName = Path.Combine(mCharacterChatDirectory, charId.ToString());
        var hasFile = File.Exists(fileName);
        if (hasFile == false)
        {
            return;
        }
        var buffer = new byte[4];
        using (var fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
        {
            SerializerUtility.WriteInt(buffer, lv, 0);
            fs.Write(buffer, 0, 4);
            SerializerUtility.WriteInt(buffer, ladder, 0);
            fs.Write(buffer, 0, 4);
        }
    }

    private void UpdateContactCellIndex()
    {
        var c = DataModel.ContactInfos.Count;
        for (var i = 0; i < c; i++)
        {
            var d = DataModel.ContactInfos[i];
            d.Index = i;
        }
    }

    private void UpdateContactCellNextIndex()
    {
        var c = DataModel.ContactInfos.Count;
        for (var i = 0; i < c; i++)
        {
            var d = DataModel.ContactInfos[i];
            d.NextIndex = i;
        }
    }

    private void UpdateContactOrder(bool select = false)
    {
        var list = new List<ContactInfoDataModel>(DataModel.ContactInfos);
        list.Sort((l, r) => { return l.NextIndex - r.NextIndex; });

        DataModel.ContactInfos = new ObservableCollection<ContactInfoDataModel>(list);
        UpdateContactCellIndex();
        UpdateContactCellNextIndex();
        if (select)
        {
            SelectFriendContactCell(0);
        }
    }

    private void UpdateFriendInfo(int type, List<CharacterSimpleData> friends)
    {
        var list = new List<FriendInfoDataModel>();
        {
            var __list8 = friends;
            var __listCount8 = __list8.Count;
            for (var __i8 = 0; __i8 < __listCount8; ++__i8)
            {
                var friend = __list8[__i8];
                {
                    var data = GetFriendInfoDataModel(friend);
                    list.Add(data);
                }
            }
        }
        list.Sort();

        var dataList = new ObservableCollection<FriendInfoDataModel>(list);
        SetFriendInfo(type, dataList);
    }

    private void UpdateNoticeUnreadCount()
    {
        var count = 0;
        foreach (var info in DataModel.ContactInfos)
        {
            count += info.UnreadCount;
        }
        PlayerDataManager.Instance.NoticeData.ChatUnRead = count;
    }

    private void UpdateRecentcontacts(List<PlayerHeadInfoMsg> list)
    {
        var localList = new List<ContactInfoDataModel>(DataModel.ContactInfos);
        var orderList = new List<ContactInfoDataModel>();
        var length = list.Count;
        for (var i = length - 1; i >= 0; i--)
        {
            var infoMsg = list[i];
            ContactInfoDataModel find = null;
            foreach (var model in localList)
            {
                if (model.CharacterId == infoMsg.CharacterId)
                {
                    find = model;
                    localList.Remove(model);
                    break;
                }
            }
            if (find == null)
            {
                find = new ContactInfoDataModel();
                find.Name = infoMsg.Name;
                find.HasUpdate = true;
                find.CharacterId = infoMsg.CharacterId;
                find.Level = infoMsg.Level;
                find.Ladder = infoMsg.Ladder;
                find.Type = infoMsg.RoleId;
                find.UnreadCount = 0;
                UpdateNoticeUnreadCount();
                orderList.Add(find);
            }
            else
            {
                find.HasUpdate = true;
                orderList.Add(find);
            }
        }
        foreach (var model in localList)
        {
            orderList.Add(model);
        }

        DataModel.ContactInfos = new ObservableCollection<ContactInfoDataModel>(orderList);
        UpdateContactCellIndex();
        SelectFriendContactCell(0);
    }

    private void UpdateSelectInfo(PlayerHeadInfoMsg info)
    {
        DataModel.SelectContact.HasUpdate = true;
        CheckUpdateInfo(DataModel.SelectContact, info.Level, info.Ladder);
    }

    private void WriteChacheRecords()
    {
        foreach (var listCach in mUnWriteListCaches)
        {
            var charId = listCach.Key;
            var contactInfo = GetContactInfoData(charId);
            if (contactInfo == null)
            {
                continue;
            }
            SaveChatRecordList(contactInfo, listCach.Value);
        }
        mUnWriteListCaches.Clear();
    }

    public void CleanUp()
    {
        EmptyContactInfo.CharacterId = 0;
        EmptyContactInfo.Name = "";
        EmptyContactInfo.IsSelect = false;
        DataModel = new FriendDataModel();
        mCdTimeList.Clear();

        for (var i = 0; i < 2; i++)
        {
//seek fast
            mCdTimeList.Add(Game.Instance.ServerTime);
        }
        mUnWriteListCaches = new Dictionary<ulong, List<ChatMessageData>>();
        inputStr = GameUtils.GetDictionaryText(100001058);
        DataModel.InputChat = inputStr;
        inputStr2 = GameUtils.GetDictionaryText(240612);
        DataModel.InputSeek = inputStr2;
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        if (name == "ApplyAllInfo")
        {
            ApplyAllInfo();
        }
        else if (name == "IsInFriendListId")
        {
            var id = (ulong) param[0];
            return IsInFriendListId(id);
        }
        else if (name == "IsInEnemyListId")
        {
            var id = (ulong) param[0];
            return IsInEnemyListId(id);
        }
        else if (name == "IsInBalckListId")
        {
            var id = (ulong) param[0];
            return IsInBalckListId(id);
        }
        else if (name == "IsInBalckListName")
        {
            var str = (string) param[0];
            return IsInBalckListName(str);
        }
        else if (name == "GetFiendInfo")
        {
            var type = (int) param[0];
            var guid = (ulong) param[1];
            return GetFiendInfo(type, guid);
        }
        return null;
    }

    public void OnShow()
    {
    }

    public void Close()
    {
        var e = new Close_UI_Event(UIConfig.ChatItemList);
        EventDispatcher.Instance.DispatchEvent(e);

        var e1 = new Close_UI_Event(UIConfig.FaceList);
        EventDispatcher.Instance.DispatchEvent(e1);

        PlayerDataManager.Instance.CloseCharacterPopMenu();

        WriteChacheRecords();
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        for (var i = 0; i < DataModel.IsSelectTab.Count; i++)
        {
            DataModel.IsSelectTab[i] = false;
        }
        DataModel.EmptyTips[3] = true;
        DataModel.SeekInfos.Clear();
        var arg = data as FriendArguments;

        if (arg == null || arg.Type == 0)
        {
            if (DataModel.FriendInfos.Count == 0)
            {
                DataModel.IsSelectTab[3] = true;
                DataModel.SelectToggle = 3;
//                 if ((Game.Instance.ServerTime - mCdTimeList[0]).TotalSeconds > 3)
//                 {
//                     QuickSeekFriends(false);
//                 }
            }
            else
            {
                DataModel.SelectToggle = 0;
                DataModel.IsSelectTab[0] = true;
            }
        }
        else if (arg.Type == 1)
        {
            DataModel.SelectToggle = 4;
            DataModel.IsSelectTab[4] = true;
            var d = arg.Data;
            SetContactChatInfo(d.CharacterId, d.CharacterName, d.Ladder, d.Level, d.RoleId);
        }
        else if (arg.Type == 2)
        {
            DataModel.SelectToggle = 4;
            DataModel.IsSelectTab[4] = true;
            UpdateContactOrder();
            UpdateContactCellNextIndex();
            SelectFriendContactCell(0);
        }
        else
        {
            DataModel.SelectToggle = 0;
            DataModel.IsSelectTab[0] = true;
        }
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public FrameState State { get; set; }
}