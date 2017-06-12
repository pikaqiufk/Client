/********************************************************************************* 

                         Scorpion



  *FileName:ChattingMajorFrameCtrler

  *Version:1.0

  *Date:2017-06-08

  *Description:

**********************************************************************************/  
#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using LZ4s;
using ScorpionNetLib;
using ProtoBuf;
using Shared;

#endregion

public class ChattingMajorFrameCtrler : IControllerBase
{

    #region 静态变量

    private readonly Dictionary<int, List<int>> s_dicCitySettings = new Dictionary<int, List<int>>();

    #endregion

    #region 成员变量

    //每个内容频道的聊天内容
    private Dictionary<int, List<ChatMessageDataModel>> m_dicChannelMessages;
    private ChatMainDataModel DataModel;
    private string m_strInput = string.Empty;
    //发送频道的cd时间点
    private Dictionary<int, DateTime> m_dicChannelCDs;

    //连接内容和显示内容的对应关系，例如物品位置表情等内容通过64位编码压缩进行发送，
    //但在输入框内显示是显示有意义的字符，
    //接受连接字符时，转换成相应的内容，
    //真正发送的时候再替换一次
    private Dictionary<string, string> m_dicDicItemLink = new Dictionary<string, string>();
    private bool m_bIsInit = false;
    //私聊的对象
    private string m_strPrivateName;
    //每个显示频道的数量
    private List<int> m_listViewRecord = new List<int>();
    //全部的聊天内容，上线为1000
    private List<ChatMessageDataModel> m_listTotalCharMessages;
    private byte[] m_btTranslatingVoice;
    //缓存音频聊天
    private LinkedList<byte[]> m_listVoiceList;
    //当前的显示频道
    private int m_iShowChannel { get; set; }

    #endregion

    #region 构造函数

    public ChattingMajorFrameCtrler()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(ChatSoundTranslateAddEvent.EVENT_TYPE, OnChitchatVoiceInterpretAddEvent);
        EventDispatcher.Instance.AddEventListener(ChatMainSendBtnClick.EVENT_TYPE, OnSendButtonTapEvent);
        EventDispatcher.Instance.AddEventListener(ChatMainChangeChannel.EVENT_TYPE, OnAlterTongueChannelEvent);
        EventDispatcher.Instance.AddEventListener(ChatMainFaceAdd.EVENT_TYPE, OnAdditionPhizEvent);
        EventDispatcher.Instance.AddEventListener(Event_PushMessage.EVENT_TYPE, OnPushMsgEvent);
        EventDispatcher.Instance.AddEventListener(ChatShareItemEvent.EVENT_TYPE, OnChitchatSharingPropEvent);
        EventDispatcher.Instance.AddEventListener(ChatWordCountChage.EVENT_TYPE, OnChitchatWordNumChageEvent);
        EventDispatcher.Instance.AddEventListener(ChatTrumpetVisibleChange.EVENT_TYPE, OnChitchatTrumpetHormChangeEvent);
        EventDispatcher.Instance.AddEventListener(ChatMainSendVoiceData.EVENT_TYPE, OnSendSoundChitchatEvent);
        EventDispatcher.Instance.AddEventListener(ChatMainPlayVoice.EVENT_TYPE, OnGameVoiceEvent);
        EventDispatcher.Instance.AddEventListener(ChatMainSpeechRecognized.EVENT_TYPE, onSpeakAcceptedEvent);
        EventDispatcher.Instance.AddEventListener(ChatMainHelpMeesage.EVENT_TYPE, OnChitchatCoreHelpMsgEvent);
        EventDispatcher.Instance.AddEventListener(AddFaceNode.EVENT_TYPE, OnAdditionPhizNodeEvent);
        EventDispatcher.Instance.AddEventListener(ChatMainOperate.EVENT_TYPE, OnChitchatCoreOperationEvent);
        EventDispatcher.Instance.AddEventListener(ChatCityCellClick.EVENT_TYPE, OnChitchatCityCellTapEvent);

        EventDispatcher.Instance.AddEventListener(IpAddressSet.EVENT_TYPE, OnIPSpeechSetEvent);
    }

    #endregion

    #region 固有函数

    public void OnShow()
    {
    }

    public void Close()
    {
        var _e = new Close_UI_Event(UIConfig.ChatItemList);
        EventDispatcher.Instance.DispatchEvent(_e);

        var _e1 = new Close_UI_Event(UIConfig.FaceList);
        EventDispatcher.Instance.DispatchEvent(_e1);

        PlayerDataManager.Instance.CloseCharacterPopMenu();
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        if (name == "EnterGame")
        {
            OnGoGame();
            return null;
        }
        return null;
    }

    public void CleanUp()
    {
        m_listTotalCharMessages = new List<ChatMessageDataModel>();
        m_dicChannelMessages = new Dictionary<int, List<ChatMessageDataModel>>();
        m_dicChannelCDs = new Dictionary<int, DateTime>();
        m_listVoiceList = new LinkedList<byte[]>();

        if (DataModel != null)
        {
            DataModel.IsShowChannel.PropertyChanged -= OnDisplayChannelCallOff;
            DataModel.IsSelectChannel.PropertyChanged -= OnDisplayChannelCallOff;
        }
        DataModel = new ChatMainDataModel();
        DataModel.IsShowChannel[0] = true;
        DataModel.SpeekChannel = 1;
        DataModel.IsShowChannel.PropertyChanged += OnChooseChannelCallOff;
        DataModel.IsSelectChannel.PropertyChanged += OnChooseChannelCallOff;
        m_strInput = GameUtils.GetDictionaryText(100001058);
        DataModel.InputChat = m_strInput;
    }

    public void RefreshData(UIInitArguments data)
    {
        DataModel.InputTrunoet = "";
        if (DataModel.InputChat == string.Empty)
        {
            DataModel.InputChat = m_strInput;
        }
        if (DataModel.InputTrunoet == string.Empty)
        {
            DataModel.InputTrunoet = m_strInput;
        }
        var _args = data as ChatMainArguments;
        if (_args != null)
        {
            if (_args.Type == 0)
            {
                var _itemData = _args.ItemDataModel;
                if (_itemData != null)
                {
                    SendItemLink(_itemData);
                }
            }
            else if (_args.Type == 1)
            {
                SendPostionLink();
            }
        }
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public void Tick()
    {
        if (m_btTranslatingVoice != null || m_listVoiceList.Count < 1)
        {
            return;
        }
        m_btTranslatingVoice = m_listVoiceList.First.Value;
        m_listVoiceList.RemoveFirst();
        var _pcm = WSpeak.GetDecodePcm(m_btTranslatingVoice);
        PlatformHelper.SpeechRecognize(m_btTranslatingVoice.Length, _pcm, _pcm.Length * 2);
    }

    public FrameState State { get; set; }

    #endregion

    #region 逻辑函数

    //向聊天中发送表情
    private void SendEmoji(int emojiId)
    {
        var _dataChatInfoNode = new ChatInfoNodeData();
        _dataChatInfoNode.Type = (int)eChatLinkType.Face;
        _dataChatInfoNode.Id = emojiId;
        var _str = "";
        using (var _ms = new MemoryStream())
        {
            Serializer.Serialize(_ms, _dataChatInfoNode);
            var _wrap = LZ4Codec.Encode32(_ms.GetBuffer(), 0, (int)_ms.Length);
            _str = Convert.ToBase64String(_wrap);
        }
        _str = SpecialCode.ChatBegin + _str + SpecialCode.ChatEnd;
        var _value = "{" + emojiId + "}";
        m_dicDicItemLink[_value] = _str;
        SendEnterText(_value);
    }

    private void SendEnterText(string str)
    {
        if (DataModel.ShowTrumpet)
        {
            DataModel.InputTrunoet += str;
        }
        else
        {
            if (DataModel.InputChat == m_strInput)
            {
                DataModel.InputChat = str;
            }
            else
            {
                DataModel.InputChat += str;
            }
        }
    }

    //向聊天中发送物品链接
    private void SendItemLink(BagItemDataModel itemMsg)
    {
        var _dataChatInfoNode = new ChatInfoNodeData();
        _dataChatInfoNode.Type = (int)eChatLinkType.Equip;
        _dataChatInfoNode.Id = itemMsg.ItemId;

        var _nowItemExdataCount0 = itemMsg.Exdata.Count;
        for (var i = 0; i < _nowItemExdataCount0; i++)
        {
            _dataChatInfoNode.ExData.Add(itemMsg.Exdata[i]);
        }
        var _str = "";
        using (var _ms = new MemoryStream())
        {
            Serializer.Serialize(_ms, _dataChatInfoNode);
            var _wrap = LZ4Codec.Encode32(_ms.GetBuffer(), 0, (int)_ms.Length);
            _str = Convert.ToBase64String(_wrap);
        }

        _str = SpecialCode.ChatBegin + _str + SpecialCode.ChatEnd;
        var _tbTable = Table.GetItemBase(itemMsg.ItemId);
        var _value = _tbTable.Name;
        var _color = GameUtils.GetTableColorString(_tbTable.Quality);
        _value = String.Format("[{0}][{1}][-]", _color, _value);

        m_dicDicItemLink[_value] = _str;

        //         var inputStr = DataModel.InputChat + str;
        //         {
        //             var __enumerator3 = (m_dicDicItemLink).GetEnumerator();
        //             while (__enumerator3.MoveNext())
        //             {
        //                 var i = __enumerator3.Current;
        //                 {
        //                     inputStr = inputStr.Replace(i.Key, i.Value);
        //                 }
        //             }
        //         }
        SendEnterText(_value);
    }

    //向聊天中发送位置
    private void SendPostionLink()
    {
        var _dataChatInfoNode = new ChatInfoNodeData();
        _dataChatInfoNode.Type = (int)eChatLinkType.Postion;
        var _sceneId = GameLogic.Instance.Scene.SceneTypeId;
        _dataChatInfoNode.ExData.Add(_sceneId);
        var _postion = ObjManager.Instance.MyPlayer.Position;
        var _x = GameUtils.MultiplyPrecision(_postion.x);
        _dataChatInfoNode.ExData.Add(_x);
        var _y = GameUtils.MultiplyPrecision(_postion.z);
        _dataChatInfoNode.ExData.Add(_y);

        var _str = "";
        using (var _ms = new MemoryStream())
        {
            Serializer.Serialize(_ms, _dataChatInfoNode);
            var _wrap = LZ4Codec.Encode32(_ms.GetBuffer(), 0, (int)_ms.Length);
            _str = Convert.ToBase64String(_wrap);
        }
        _str = SpecialCode.ChatBegin + _str + SpecialCode.ChatEnd;
        var _tbScene = Table.GetScene(_sceneId);
        var _value = String.Format("[{0}:{1},{2}]", _tbScene.Name, (int)_postion.x, (int)_postion.z);
        m_dicDicItemLink[_value] = _str;
        var _inputStr = DataModel.InputChat + _str;
        {
            // foreach(var i in m_dicDicItemLink)
            var _enumerator4 = (m_dicDicItemLink).GetEnumerator();
            while (_enumerator4.MoveNext())
            {
                var _i = _enumerator4.Current;
                {
                    _inputStr = _inputStr.Replace(_i.Key, _i.Value);
                }
            }
        }
        SendEnterText(_value);
    }

    //检查频道是否可见某种信息
    private int InspectCanView(int opinionChannel, eChatChannel chatMold)
    {
        if (chatMold == eChatChannel.Help && opinionChannel != 7)
        {
            return 1;
        }
        if (chatMold == eChatChannel.MyWhisper)
        {
            chatMold = eChatChannel.Whisper;
        }

        if (opinionChannel == 4)
        {
            if (chatMold == eChatChannel.Horn)
            {
                return 0;
            }
            if (chatMold == eChatChannel.SystemScroll)
            {
                chatMold = eChatChannel.System;
            }
            if ((int)chatMold < 0 || (int)chatMold >= DataModel.IsSelectChannel.Count)
            {
                return 0;
            }
            return DataModel.IsSelectChannel[(int)chatMold] ? 1 : 0;
        }
        var _tbChat = Table.GetChatInfo((int)chatMold);
        if (_tbChat == null)
        {
            return 0;
        }
        if (opinionChannel < 0 || opinionChannel >= _tbChat.Channal.Length)
        {
            return 0;
        }
        return _tbChat.Channal[opinionChannel];
        return 0;
    }

    private string InspectInputStr(string tex)
    {
        var _nend1 = tex.IndexOf("/", 0, StringComparison.Ordinal);
        if (_nend1 != 0)
        {
            return tex;
        }
        var _nend2 = tex.IndexOf(" ", 0, StringComparison.Ordinal);
        if (_nend2 == -1)
        {
            return tex;
        }
        var _a = tex.Substring(_nend1 + 1, _nend2 - _nend1 - 1);
        if (_a.Length != 1)
        {
            m_strPrivateName = _a;

            return tex;
        }
        var _channel = _a[0];
        switch (_channel)
        {
            case 'w':
                {
                    DataModel.SpeekChannel = (int)eChatChannel.World;
                    return tex.Substring(_nend2 + 1, tex.Length - _nend2 - 1);
                }
                break;
            case 's':
                {
                    DataModel.SpeekChannel = (int)eChatChannel.Scene;
                    return tex.Substring(_nend2 + 1, tex.Length - _nend2 - 1);
                }
                break;
            case 'g':
                {
                    DataModel.SpeekChannel = (int)eChatChannel.Guild;
                    return tex.Substring(_nend2 + 1, tex.Length - _nend2 - 1);
                }
                break;
            case 't':
                {
                    DataModel.SpeekChannel = (int)eChatChannel.Team;
                    return tex.Substring(_nend2 + 1, tex.Length - _nend2 - 1);
                }
                break;
            case 'c':
                {
                    DataModel.SpeekChannel = (int)eChatChannel.City;
                    return tex.Substring(_nend2 + 1, tex.Length - _nend2 - 1);
                }
                break;
            default:
                {
                    return tex;
                }
                break;
        }

        return tex;
    }

    private bool InspectSpeakCD(int speakChannel)
    {
        DateTime _cd;
        if (!m_dicChannelCDs.TryGetValue(speakChannel, out _cd))
        {
            return true;
        }
        if (Game.Instance.ServerTime > _cd)
        {
            return true;
        }
        return false;
    }

    private int InspectSpeekLv(int speekChannel)
    {
        var _tbChat = Table.GetChatInfo(speekChannel);
        if (_tbChat == null)
        {
            return 0;
        }
        var _level = PlayerDataManager.Instance.GetLevel();
        if (_level < _tbChat.NeedLevel)
        {
            return _tbChat.NeedLevel;
        }
        return 0;
    }

    private void GoIntoChannel()
    {
        NetManager.Instance.StartCoroutine(GoIntoChannelCoroutine());
    }

    private IEnumerator GoIntoChannelCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var _id = (ulong)DataModel.SubCityIdChoose;
            var _msg = NetManager.Instance.EnterChannel(_id, "");
            yield return _msg.SendAndWaitUntilDone();
            if (_msg.State == MessageState.Reply)
            {
                if (_msg.ErrorCode == (int)ErrorCodes.OK)
                {
                    DataModel.MainCityId = DataModel.MainCityIdChoose;
                    DataModel.SubCityId = DataModel.SubCityIdChoose;
                }
                else
                {
                    var _e = new ShowUIHintBoard(_msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(_e);
                }
            }
            else
            {
                Logger.Error("EnterChannel Error!............State..." + _msg.State);
            }
        }
    }

    private int AcquireViewMaxNum(int showChannel)
    {
        if (m_listViewRecord.Count > m_iShowChannel)
        {
            var _maxClount = m_listViewRecord[m_iShowChannel];
            return _maxClount;
        }
        return -1;
    }

    //     private void ApplyIpAddress()
    //     {
    //         //var url = @"http://int.dpool.sina.com.cn/iplookup/iplookup.php?format=js";
    //         var url = @"http://ip.ws.126.net/ipquery";
    // 
    //         HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
    //         request.Method = "GET";
    //         //request.Method = "POST";
    //         request.UserAgent = string.Empty;
    //         request.Timeout = 10;
    //         request.BeginGetResponse(result =>
    //         {
    //             var webRequest = ((HttpWebRequest)result.AsyncState);
    //             WebResponse webResponse = null;
    //             try
    //             {
    //                 webResponse = webRequest.EndGetResponse(result);
    //             }
    //             catch (Exception ex)
    //             {
    //                 Logger.Error(ex.ToString());
    //             }
    // 
    //             lock (NetManager.Instance.EventQueue)
    //             {
    //                 NetManager.Instance.EventQueue.Enqueue(new DoSomethingEvent(() =>
    //                 {
    //                     try
    //                     {
    //                         Stream stream = webResponse.GetResponseStream();
    // 
    //                         StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("gb2312"));
    //                         string read = reader.ReadToEnd();
    // 
    //                         var m = Regex.Match(read, "city:\"(.*?)\"");
    //                         var city = m.Groups[1].Captures[0].Value;
    // 
    // 
    //                         m = Regex.Match(read, "province:\"(.*?)\"");
    //                         var province = m.Groups[1].Captures[0].Value;
    // 
    //                          InitIpAddress(province, city);
    //                          InitNetIpAddress();
    //                     }
    //                     catch (Exception ex)
    //                     {
    //                         InitIpAddress("", "");
    //                         InitNetIpAddress();
    //                     }
    //                 }));
    //             }
    //         }, request);
    //     }
    private void InitalizeIPAddress(string main, string sub)
    {
        if (main == "" && sub == "")
        {
            DataModel.MainCityId = 0;
            DataModel.SubCityId = 0;
            return;
        }
        var _bFind = false;
        foreach (var setting in s_dicCitySettings)
        {
            var _id = setting.Key;
            var _btCity = Table.GetCityTalk(_id);
            if (_btCity != null)
            {
                if (main.Contains(_btCity.Name))
                {
                    DataModel.MainCityId = _id;
                    _bFind = true;
                    break;
                }
            }
        }

        if (_bFind == false)
        {
            DataModel.MainCityId = 0;
            DataModel.SubCityId = 0;
            return;
        }

        if (DataModel.MainCityId < 1000)
        {
            DataModel.SubCityId = DataModel.MainCityId;
            return;
        }

        _bFind = false;
        List<int> list;
        if (!s_dicCitySettings.TryGetValue(DataModel.MainCityId, out list))
        {
            DataModel.MainCityId = 0;
            DataModel.SubCityId = 0;
            return;
        }

        foreach (var i in list)
        {
            var _btCity = Table.GetCityTalk(i);
            if (_btCity != null)
            {
                if (sub.Contains(_btCity.Name))
                {
                    DataModel.SubCityId = i;
                    _bFind = true;
                    break;
                }
            }
        }

        if (_bFind == false)
        {
            DataModel.SubCityId = list[0];
        }
    }

    private void InitalizeNetworkIPAddress()
    {
        DataModel.MainCityIdChoose = DataModel.MainCityId;
        DataModel.SubCityIdChoose = DataModel.SubCityId;

        lock (NetManager.Instance.EventQueue)
        {
            NetManager.Instance.EventQueue.Enqueue(new DoSomethingEvent(() => { GoIntoChannel(); }));
        }
    }
    private void OnChatwagMainWordNumChage()
    {
        var _str = DataModel.InputChat;
        {
            // foreach(var i in m_dicDicItemLink)
            var _enumerator2 = (m_dicDicItemLink).GetEnumerator();
            while (_enumerator2.MoveNext())
            {
                var _i = _enumerator2.Current;
                {
                    _str = _str.Replace(_i.Key, _i.Value);
                }
            }
        }
        var _count = _str.GetStringLength();
        if (_count > GameUtils.ChatWorldCount)
        {
            _str = _str.SubStringLength(GameUtils.ChatWorldCount);
            var _enumerator2 = (m_dicDicItemLink).GetEnumerator();
            while (_enumerator2.MoveNext())
            {
                var i = _enumerator2.Current;
                {
                    _str = _str.Replace(i.Value, i.Key);
                }
            }
            DataModel.InputChat = _str;
        }
    }



    private void OnChatHormWordNumChage()
    {
        var _str = DataModel.InputTrunoet;
        {
            // foreach(var i in m_dicDicItemLink)
            var _enumerator2 = (m_dicDicItemLink).GetEnumerator();
            while (_enumerator2.MoveNext())
            {
                var _i = _enumerator2.Current;
                {
                    _str = _str.Replace(_i.Key, _i.Value);
                }
            }
        }
        var _count = _str.GetStringLength();
        var _e1 = new ChatTrumpetWordCountCheck(_count);
        EventDispatcher.Instance.DispatchEvent(_e1);
    }

    //把一个聊天结构记录到本地，并且更新到显示的地方
    private void PushedMsg(ChatMessageDataModel chat)
    {
        var _type = chat.Type;
        if (_type != (int)eChatChannel.Horn)
        {
            var _fiendCon = UIManager.Instance.GetController(UIConfig.FriendUI);
            var _ret = (bool)_fiendCon.CallFromOtherClass("IsInBalckListId", new[] { (object)chat.CharId });
            if (_ret)
            {
                return;
            }
        }
        var _tbChat = Table.GetChatInfo(_type);
        if (_tbChat == null)
        {
            return;
        }
        if (_type == (int)eChatChannel.SystemScroll)
        {
            chat.Name = "";
        }
        if (_type == (int)eChatChannel.Whisper
            || _type == (int)eChatChannel.MyWhisper)
        {
            var _chatMessage = new ChatMessageData();
            _chatMessage.Type = chat.Type;
            _chatMessage.Content = chat.Content;
            _chatMessage.CharId = chat.CharId;
            _chatMessage.Name = chat.Name;
            _chatMessage.SoundData = chat.SoundData;
            _chatMessage.Seconds = chat.Seconds;
            _chatMessage.Times = Game.Instance.ServerTime.ToBinary();
            var _e = new WhisperChatMessage(_chatMessage);
            EventDispatcher.Instance.DispatchEvent(_e);
            return;
        }
        if (_tbChat.Channal[(int)eChatShowChannel.SystemScroll] == 1)
        {
            var _ret = GameUtils.ConvertChatContent(chat.Content);
            var _strColor = GameUtils.GetTableColorString(_tbChat.ColorId[0]);
            if (_strColor != "FFFFFF")
            {
                _ret = string.Format("[{0}]{1}[-]", _strColor, _ret);
            }
            var _arg = new SystemNoticeArguments { NoticeInfo = _ret };
            EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.SystemNoticeFrame, _arg));
        }

        List<ChatMessageDataModel> list = null;
        if (!m_dicChannelMessages.TryGetValue(_type, out list))
        {
            list = new List<ChatMessageDataModel>();
            m_dicChannelMessages[_type] = list;
        }
        //如果达到内容频道的上限，则删除前10个，防止频繁删除
        if (list.Count >= _tbChat.MaxWord)
        {
            var _limit = 10;
            if (_tbChat.MaxWord < 10)
            {
                _limit = list.Count - _tbChat.MaxWord;
            }
            for (var i = 0; i < _limit; i++)
            {
                var _l = list[i];
                m_listTotalCharMessages.Remove(_l);
            }
            list.RemoveRange(0, _limit);
        }
        list.Add(chat);

        m_listTotalCharMessages.Add(chat);

        if (chat.SoundData != null)
        {
            m_listVoiceList.AddLast(chat.SoundData);
        }
        if ((eChatChannel)chat.Type == eChatChannel.Horn)
        {
            DataModel.TrumpetData = chat;
            EventDispatcher.Instance.DispatchEvent(new ChatMainNewTrumpet());
        }
        //加到当前选择频道
        if (InspectCanView(m_iShowChannel, (eChatChannel)chat.Type) == 1)
        {
            var _maxClount = AcquireViewMaxNum(m_iShowChannel);
            if (_maxClount > 0 && DataModel.ChatMessages.Count >= _maxClount)
            {
                var _listDatas = new List<ChatMessageDataModel>();
                _listDatas.AddRange(DataModel.ChatMessages);
                _listDatas.RemoveRange(0, 10);
                _listDatas.Add(chat);
                DataModel.ChatMessages = new ObservableCollection<ChatMessageDataModel>(_listDatas);
            }
            else
            {
                DataModel.ChatMessages.Add(chat);
            }

            if (DataModel.MainUIMessages.Count == 3)
            {
                DataModel.MainUIMessages.RemoveAt(0);
            }
            DataModel.MainUIMessages.Add(chat);
        }

        //加到许愿池聊天频道
        if (InspectCanView(7, (eChatChannel)chat.Type) == 1)
        {
            var _maxClount = AcquireViewMaxNum(7);
            if (_maxClount > 0 && DataModel.WishMessages.Count >= _maxClount)
            {
                var _listDatas = new List<ChatMessageDataModel>();
                _listDatas.AddRange(DataModel.WishMessages);
                _listDatas.RemoveRange(0, 10);
                _listDatas.Add(chat);
                DataModel.WishMessages = new ObservableCollection<ChatMessageDataModel>(_listDatas);
            }
            else
            {
                DataModel.WishMessages.Add(chat);
            }
            chat.ChatCount = DataModel.WishMessages.Count;
            DataModel.WishMessage = chat;
        }
    }
    private void OnTapTownCallOff()
    {
        DataModel.MainCityIdChoose = DataModel.MainCityId;
        DataModel.SubCityIdChoose = DataModel.SubCityId;
    }

    private void OnTapTownConfirmation()
    {
        GoIntoChannel();
    }

    private void OnGoGame()
    {
        OnInitalize();
    }

    private void OnInitalize()
    {
        m_listViewRecord.Clear();
        m_listViewRecord.Add(Table.GetClientConfig(330).Value.ToInt());
        m_listViewRecord.Add(Table.GetClientConfig(331).Value.ToInt());
        m_listViewRecord.Add(Table.GetClientConfig(332).Value.ToInt());
        m_listViewRecord.Add(Table.GetClientConfig(333).Value.ToInt());
        m_listViewRecord.Add(Table.GetClientConfig(334).Value.ToInt());
        m_listViewRecord.Add(Table.GetClientConfig(335).Value.ToInt());
        m_listViewRecord.Add(Table.GetClientConfig(336).Value.ToInt());
        m_listViewRecord.Add(Table.GetClientConfig(337).Value.ToInt());

        DataModel.TrumpetCount = PlayerDataManager.Instance.GetItemTotalCount(21900);

        s_dicCitySettings.Clear();

        Table.ForeachCityTalk(record =>
        {
            if (record.Id < 1000)
            {
                s_dicCitySettings.Add(record.Id, new List<int> { record.Id });
                return true;
            }
            if (record.IsParent == 0)
            {
                return true;
            }
            var _list = new List<int>();
            var _tbSu = Table.GetSkillUpgrading(record.Param);
            if (_tbSu != null)
            {
                if (_tbSu.Type == 0)
                {
                    for (var i = 0; i < _tbSu.Values.Count; i++)
                    {
                        var _id = _tbSu.Values[i];
                        var _tbCt = Table.GetCityTalk(_id);
                        if (_tbCt != null)
                        {
                            _list.Add(_id);
                        }
                    }
                }
            }
            s_dicCitySettings.Add(record.Id, _list);
            return true;
        });
    }

    private void OnImportFocus(int type)
    {
        switch (type)
        {
            case 6:
                {
                    if (DataModel.InputChat == m_strInput)
                    {
                        DataModel.InputChat = string.Empty;
                    }
                }
                break;
            case 7:
                {
                    if (DataModel.InputTrunoet == m_strInput)
                    {
                        DataModel.InputTrunoet = string.Empty;
                    }
                }
                break;
        }
    }

  

    public void OnChooseChannelCallOff(object sender, PropertyChangedEventArgs e)
    {
        if (4 == m_iShowChannel)
        {
            ChooseOpinionChannel();
        }
    }

    private void OnDisplayChannelCallOff(object sender, PropertyChangedEventArgs e)
    {
        var _index = int.Parse(e.PropertyName);
        if (DataModel.IsShowChannel[_index])
        {
            m_iShowChannel = _index;
            ChooseOpinionChannel();
        }
    }
    //选择显示频道
    private void ChooseSpeakChannel(int speekChannel)
    {
        DataModel.SpeekChannel = speekChannel;
        SetimportTex("");
    }

    //选择显示频道
    private void ChooseOpinionChannel()
    {
        DataModel.ChatMessages.Clear();
        {
            var _list = new List<ChatMessageDataModel>();
            var _list1 = m_listTotalCharMessages;
            var _listCount1 = _list1.Count;
            var _maxClount = AcquireViewMaxNum(m_iShowChannel);
            for (var _i1 = _listCount1 - 1; _i1 >= 0; _i1--)
            {
                var _charMessage = _list1[_i1];
                {
                    if (InspectCanView(m_iShowChannel, (eChatChannel)_charMessage.Type) == 1)
                    {
                        _list.Insert(0, _charMessage);
                        if (_maxClount > 0 && DataModel.ChatMessages.Count >= _maxClount)
                        {
                            break;
                        }
                    }
                }
            }
            DataModel.ChatMessages = new ObservableCollection<ChatMessageDataModel>(_list);
        }
        DataModel.MainUIMessages.Clear();
        var _start = 0;
        if (DataModel.ChatMessages.Count >= 3)
        {
            _start = DataModel.ChatMessages.Count - 3;
        }

        var _DataModelChatMessagesCount0 = DataModel.ChatMessages.Count;
        for (var i = _start; i < _DataModelChatMessagesCount0; i++)
        {
            DataModel.MainUIMessages.Add(DataModel.ChatMessages[i]);
        }
    }

    private void SendChitchatMsg(eChatChannel chatType,
                                ChatMessageContent Content,
                                ulong characterId = 0,
                                string TargerName = "")
    {
        if (string.IsNullOrEmpty(Content.Content))
        {
            //聊天内容不能为空
            var _e = new ShowUIHintBoard(270054);
            EventDispatcher.Instance.DispatchEvent(_e);
            return;
        }
        switch (chatType)
        {
            case eChatChannel.System:
                break;
            case eChatChannel.World:
                NetManager.Instance.StartCoroutine(SendChitchatMsgCoroutine((int)chatType, Content, characterId,
                    TargerName));
                break;
            case eChatChannel.City:
                NetManager.Instance.StartCoroutine(SendChitchatMsgCoroutine((int)chatType, Content, characterId,
                    TargerName));
                break;
            case eChatChannel.Scene:
                NetManager.Instance.StartCoroutine(SendSceneMsgCoroution((int)chatType, Content, characterId));
                break;
            case eChatChannel.Guild:
                NetManager.Instance.StartCoroutine(SendGroupMsgCoroutine((int)chatType, Content, characterId));
                break;
            case eChatChannel.Team:
                NetManager.Instance.StartCoroutine(SendGroupMsgCoroutine((int)chatType, Content, characterId));
                break;
            case eChatChannel.Whisper:
                {
                    var _fiendCon = UIManager.Instance.GetController(UIConfig.FriendUI);
                    var _ret = (bool)_fiendCon.CallFromOtherClass("IsInBalckListName", new[] { (object)TargerName });
                    if (_ret)
                    {
                        //你屏蔽了{0}
                        var _str = string.Format(GameUtils.GetDictionaryText(270055), TargerName);
                        var _e1 = new ChatMainHelpMeesage(_str);
                        EventDispatcher.Instance.DispatchEvent(_e1);
                        return;
                    }
                    NetManager.Instance.StartCoroutine(SendChitchatMsgCoroutine((int)chatType, Content, characterId,
                        TargerName));
                }
                break;
            case eChatChannel.Horn:
                NetManager.Instance.StartCoroutine(SendTrumpetMsgCoroutine(Content));
                break;
            case eChatChannel.Count:
                break;
            default:
                throw new ArgumentOutOfRangeException("chatType");
        }
    }

    private IEnumerator SendChitchatMsgCoroutine(int chatType,
                                                 ChatMessageContent Content,
                                                 ulong characterId,
                                                 string TargerName)
    {
        using (new BlockingLayerHelper(0))
        {
            var _msg = NetManager.Instance.ChatChatMessage(chatType, Content, characterId);
            yield return _msg.SendAndWaitUntilDone();
            if (_msg.State == MessageState.Reply)
            {
                if (_msg.ErrorCode == (int)ErrorCodes.OK)
                {
                    RecordChannelChitchatCD(chatType);

                    PlatformHelper.UMEvent("Chat", chatType.ToString(), characterId.ToString());
                }
                else if (_msg.ErrorCode == (int)ErrorCodes.NameNotFindCharacter)
                {
                    //玩家名字不存在                
                    var _e1 =
                        new ChatMainHelpMeesage(string.Format(GameUtils.GetDictionaryText(2000001), TargerName));
                    EventDispatcher.Instance.DispatchEvent(_e1);
                }
                else if (_msg.ErrorCode == (int)ErrorCodes.Error_SetRefuseWhisper)
                {
                    var _e1 =
                        new ChatMainHelpMeesage(string.Format(GameUtils.GetDictionaryText(998), TargerName));
                    EventDispatcher.Instance.DispatchEvent(_e1);
                }
                else if (_msg.ErrorCode == (int)ErrorCodes.Error_SetShieldYou)
                {
                    //{0}屏蔽了你
                    var _str = string.Format(GameUtils.GetDictionaryText(270056), TargerName);
                    var _e1 = new ChatMainHelpMeesage(_str);
                    EventDispatcher.Instance.DispatchEvent(_e1);
                }
                else if (_msg.ErrorCode == (int)ErrorCodes.Error_SetYouShield)
                {
                    //{0}屏蔽了你
                    var _str = string.Format(GameUtils.GetDictionaryText(270055), TargerName);
                    var _e1 = new ChatMainHelpMeesage(_str);
                    EventDispatcher.Instance.DispatchEvent(_e1);
                }
                else if (_msg.ErrorCode == (int)ErrorCodes.Error_ChatNone
                         || _msg.ErrorCode == (int)ErrorCodes.Error_ChatLengthMax
                         || _msg.ErrorCode == (int)ErrorCodes.Error_WhisperNameNone
                         || _msg.ErrorCode == (int)ErrorCodes.Error_NotWhisperSelf)
                {
                    var _e = new ShowUIHintBoard(200000000 + _msg.ErrorCode);
                    EventDispatcher.Instance.DispatchEvent(_e);
                }
                else
                {
                    var _e = new ShowUIHintBoard(200000000 + _msg.ErrorCode);
                    EventDispatcher.Instance.DispatchEvent(_e);
                }
            }
            else
            {
                Logger.Error("SendChatMessage Error!............State..." + _msg.State);
            }
        }
    }

    private IEnumerator SendTrumpetMsgCoroutine(ChatMessageContent Content)
    {
        using (new BlockingLayerHelper(0))
        {
            var _playerManager = PlayerDataManager.Instance;
            var _serverId = _playerManager.ServerId;
            var _msg = NetManager.Instance.SendHornMessage((uint)_serverId, (int)eChatChannel.Horn,
                _playerManager.GetGuid(), _playerManager.GetName(), Content);
            yield return _msg.SendAndWaitUntilDone();
            if (_msg.State == MessageState.Reply)
            {
                if (_msg.ErrorCode == (int)ErrorCodes.OK)
                {
                    RecordChannelChitchatCD((int)eChatChannel.Horn);
                    PlatformHelper.UMEvent("Chat", ((int)eChatChannel.Horn).ToString(), _playerManager.GetGuid().ToString());
                }
                else if (_msg.ErrorCode == (int)ErrorCodes.Error_ChatLengthMax
                         || _msg.ErrorCode == (int)ErrorCodes.Error_ResNoEnough)
                {
                    var _e = new ShowUIHintBoard(_msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(_e);
                }
                else
                {
                    var _e = new ShowUIHintBoard(_msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(_e);
                }
            }
            else
            {
                Logger.Error("SendLogicMessage Error!............State..." + _msg.State);
            }
        }
    }

    private IEnumerator SendSceneMsgCoroution(int chatType, ChatMessageContent Content, ulong characterId)
    {
        using (new BlockingLayerHelper(0))
        {
            var _msg = NetManager.Instance.SceneChatMessage(chatType, Content, characterId);
            yield return _msg.SendAndWaitUntilDone();
            if (_msg.State == MessageState.Reply)
            {
                if (_msg.ErrorCode == (int)ErrorCodes.OK)
                {
                    RecordChannelChitchatCD(chatType);

                    PlatformHelper.UMEvent("Chat", chatType.ToString(), characterId.ToString());
                }
                else if (_msg.ErrorCode == (int)ErrorCodes.Error_ChatLengthMax)
                {
                    var _e = new ShowUIHintBoard(_msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(_e);
                }
                else
                {
                    var _e = new ShowUIHintBoard(_msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(_e);
                }
            }
            else
            {
                Logger.Error("SendSceneMessage Error!............State..." + _msg.State);
            }
        }
    }

    private IEnumerator SendGroupMsgCoroutine(int chatType, ChatMessageContent Content, ulong characterId)
    {
        using (new BlockingLayerHelper(0))
        {
            var _msg = NetManager.Instance.TeamChatMessage(chatType, Content, characterId);
            yield return _msg.SendAndWaitUntilDone();
            if (_msg.State == MessageState.Reply)
            {
                if (_msg.ErrorCode == (int)ErrorCodes.OK)
                {
                    RecordChannelChitchatCD(chatType);
                    PlatformHelper.UMEvent("Chat", chatType.ToString(), characterId.ToString());
                }
                else if (_msg.ErrorCode == (int)ErrorCodes.NameNotFindCharacter)
                {
                    //玩家名字不存在
                    var _chat = new ChatMessageDataModel
                    {
                        Type = (int)eChatChannel.Help,
                        Content = GameUtils.GetDictionaryText(2000001),
                        Name = "",
                        CharId = characterId
                    };

                    PushedMsg(_chat);
                }
                else if (_msg.ErrorCode == (int)ErrorCodes.Error_CharacterNoTeam
                         || _msg.ErrorCode == (int)ErrorCodes.Error_ChatLengthMax
                         || _msg.ErrorCode == (int)ErrorCodes.ServerID
                         || _msg.ErrorCode == (int)ErrorCodes.Error_CharacterNoAlliance
                         || _msg.ErrorCode == (int)ErrorCodes.Error_AllianceNotFind)
                {
                    var _e = new ShowUIHintBoard(200000000 + _msg.ErrorCode);
                    EventDispatcher.Instance.DispatchEvent(_e);
                }
                else
                {
                    var _e = new ShowUIHintBoard(200000000 + _msg.ErrorCode);
                    EventDispatcher.Instance.DispatchEvent(_e);
                }
            }
            else
            {
                Logger.Error("SendChatMessage Error!............State..." + _msg.State);
            }
        }
    }

    //根据发送的频道的记录cd时间，记录下一个可发送的时间点
    private void RecordChannelChitchatCD(int chatType)
    {
        var _tbChat = Table.GetChatInfo(chatType);
        if (_tbChat == null)
        {
            return;
        }
        m_dicChannelCDs[chatType] = Game.Instance.ServerTime.AddMilliseconds(_tbChat.CD);
    }

    private void SetimportTex(string str)
    {
        if (DataModel.ShowTrumpet)
        {
            DataModel.InputTrunoet = str;
        }
        else
        {
            DataModel.InputChat = str;
        }
    }

    private void SetCoreChitchatCity(int id)
    {
        DataModel.MainCityIdChoose = id;
        foreach (var model in DataModel.ChatCityList)
        {
            if (model.ChatCityId == id)
            {
                model.IsSelect = true;
            }
            else
            {
                model.IsSelect = false;
            }
        }

        List<int> _list;
        if (s_dicCitySettings.TryGetValue(DataModel.MainCityIdChoose, out _list))
        {
            DataModel.SubCityIdChoose = _list[0];
        }
    }

    private void SetReplaceChitchatCity(int id)
    {
        DataModel.SubCityIdChoose = id;
        foreach (var model in DataModel.ChatCityList)
        {
            if (model.ChatCityId == id)
            {
                model.IsSelect = true;
            }
            else
            {
                model.IsSelect = false;
            }
        }
    }

    //去除换行和字符串尾端的空格
    public string CutDownStr(string str)
    {
        str = str.Replace("\n", "");
        str = str.TrimEnd();
        return str;
    }

    private void RenewalCoreCityList()
    {
        var _list = new List<ChatCityCellDataModel>();
        foreach (var setting in s_dicCitySettings)
        {
            var _cell = new ChatCityCellDataModel();
            if (setting.Key == DataModel.MainCityIdChoose)
            {
                _cell.IsSelect = true;
            }
            else
            {
                _cell.IsSelect = false;
            }
            _cell.ChatCityId = setting.Key;
            _list.Add(_cell);
        }
        DataModel.ChatCityList = new ObservableCollection<ChatCityCellDataModel>(_list);
    }

    private void RenewalReplaceCityList()
    {
        List<int> _list;
        if (!s_dicCitySettings.TryGetValue(DataModel.MainCityIdChoose, out _list))
        {
            DataModel.ChatCityList.Clear();
            return;
        }
        var _cityList = new List<ChatCityCellDataModel>();
        foreach (var i in _list)
        {
            var _cell = new ChatCityCellDataModel();
            if (i == DataModel.SubCityIdChoose)
            {
                _cell.IsSelect = true;
            }
            else
            {
                _cell.IsSelect = false;
            }
            _cell.ChatCityId = i;
            _cityList.Add(_cell);
        }
        DataModel.ChatCityList = new ObservableCollection<ChatCityCellDataModel>(_cityList);
    }

    #endregion

    #region 事件

    private void OnIPSpeechSetEvent(IEvent ievent)
    {
        var _e = ievent as IpAddressSet;
        InitalizeIPAddress(_e.Province, _e.City);
        InitalizeNetworkIPAddress();
    }

    private void OnGameVoiceEvent(IEvent ievent)
    {
        var _e = ievent as ChatMainPlayVoice;

        WSpeak.PlayVoice(_e.SoundData);
    }
   private void OnAdditionPhizEvent(IEvent ievent)
    {
        var e = ievent as ChatMainFaceAdd;
        SendEmoji(e.FaceId);
    }

   private void OnAdditionPhizNodeEvent(IEvent ievent)
    {
        var _e = ievent as AddFaceNode;
        if (_e.Type == 3)
        {
            return;
        }
        SendEmoji(_e.FaceId);
    }

    private void OnAlterTongueChannelEvent(IEvent ievent)
    {
        var _e = ievent as ChatMainChangeChannel;
        ChooseSpeakChannel(_e.Channel);
    }

    //-------------------------------------------------City------------------------
    private void OnChitchatCityCellTapEvent(IEvent ievent)
    {
        var _e = ievent as ChatCityCellClick;
        var _index = _e.Index;
        if (_index < 0 || _index >= DataModel.ChatCityList.Count)
        {
            return;
        }

        var _cell = DataModel.ChatCityList[_index];
        if (_cell.IsSelect)
        {
            return;
        }

        var _tbCity = Table.GetCityTalk(_cell.ChatCityId);
        if (_tbCity == null)
        {
            return;
        }
        if (_tbCity.IsParent == 1 || _cell.ChatCityId < 1000)
        {
            SetCoreChitchatCity(_cell.ChatCityId);
        }
        else
        {
            SetReplaceChitchatCity(_cell.ChatCityId);
        }
    }

    private void OnChitchatCoreHelpMsgEvent(IEvent ievent)
    {
        var _e = ievent as ChatMainHelpMeesage;
        var _content = _e.Content;

        var _chat = new ChatMessageDataModel
        {
            Type = (int)eChatChannel.Help,
            Content = _content,
            Name = "",
            CharId = 0
        };
        PushedMsg(_chat);
    }

    private void OnChitchatCoreOperationEvent(IEvent ievent)
    {
        var _e = ievent as ChatMainOperate;
        switch (_e.Type)
        {
            case 1:
                {
                    OnTapTownCallOff();
                }
                break;
            case 2:
                {
                    RenewalCoreCityList();
                }
                break;
            case 3:
                {
                    RenewalReplaceCityList();
                }
                break;
            case 4:
                {
                    OnTapTownConfirmation();
                }
                break;
            case 5:
                {
                    OnTapTownCallOff();
                }
                break;
            case 6:
            case 7:
                {
                    OnImportFocus(_e.Type);
                }
                break;
        }
    }
    private void OnChitchatSharingPropEvent(IEvent ievent)
    {
        var _e = ievent as ChatShareItemEvent;
        if (_e.Type == 3)
        {
            return;
        }
        SendItemLink(_e.Data);
    }

    private void OnChitchatVoiceInterpretAddEvent(IEvent ievent)
    {
        var _e = ievent as ChatSoundTranslateAddEvent;
        m_listVoiceList.AddLast(_e.SoundData);
    }

    private void OnChitchatTrumpetHormChangeEvent(IEvent ievent)
    {
        var _e = ievent as ChatTrumpetVisibleChange;
        if (_e.IsVisible)
        {
            var _count = PlayerDataManager.Instance.GetItemCount((int)eBagType.BaseItem, 21900);
            if (_count <= 0)
            {
                //跳转到商城
                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 270256, "",
                    () => { GameUtils.GotoUiTab(79, 2); });
                return;
            }
        }
        DataModel.ShowTrumpet = _e.IsVisible;
    }
    public void OnSendSoundChitchatEvent(IEvent ievent)
    {
        var _charId = PlayerDataManager.Instance.GetGuid();
        var _e = ievent as ChatMainSendVoiceData;

        if (_e.IsWhisper)
        {
            return;
        }
        if (!InspectSpeakCD(DataModel.SpeekChannel))
        {
            //发言太过频繁了
            var _e1 = new ShowUIHintBoard(2000003);
            EventDispatcher.Instance.DispatchEvent(_e1);
            return;
        }
        var _lvChat = InspectSpeekLv(DataModel.SpeekChannel);
        if (_lvChat > 0)
        {
            //等级不够
            var _str = GameUtils.GetDictionaryText(210113);
            _str = string.Format(_str, _lvChat);
            var _e1 = new ShowUIHintBoard(_str);
            EventDispatcher.Instance.DispatchEvent(_e1);
            return;
        }

        if (_e.VoiceData.Length < 1)
        {
            Logger.Debug("voiceData.Length < 1");
            return;
        }

        if (_e.VoiceLength < 0.5f)
        {
            //时间太短
            Logger.Debug("record time < 0.5s");
            return;
        }

        var _speakTime = (int)Math.Ceiling(_e.VoiceLength);

        var _content = new ChatMessageContent
        {
            Content = _speakTime.ToString(),
            SoundData = _e.VoiceData
        };

        SendChitchatMsg((eChatChannel)DataModel.SpeekChannel, _content, _charId, m_strPrivateName);
        if (DataModel.SpeekChannel == (int)(eChatChannel.Whisper))
        {
            SetimportTex("/" + m_strPrivateName + " ");
        }
        else
        {
            SetimportTex("");
        }
        //        m_dicDicItemLink.Clear();
    }
    private void onSpeakAcceptedEvent(IEvent iEvent)
    {
        var e = iEvent as ChatMainSpeechRecognized;
        if (string.IsNullOrEmpty(e.Content) || null == m_btTranslatingVoice)
        {
            m_btTranslatingVoice = null;
            return;
        }

        foreach (var message in m_listTotalCharMessages)
        {
            if (message.SoundData != null
                && message.SoundData == m_btTranslatingVoice)
            {
                message.Content = e.Content;
                EventDispatcher.Instance.DispatchEvent(new ChatVoiceContent(m_btTranslatingVoice, e.Content));
                m_btTranslatingVoice = null;
                break;
            }
        }
        if (m_btTranslatingVoice != null)
        {
            EventDispatcher.Instance.DispatchEvent(new ChatVoiceContent(m_btTranslatingVoice, e.Content));
            m_btTranslatingVoice = null;
        }
    }

    //接受聊天内容，生成一个ChatMessageDataModel结构
    public void OnPushMsgEvent(IEvent evt)
    {
        var _e = evt as Event_PushMessage;
        PushedMsg(_e.DataModel);
    }
    private void OnChitchatWordNumChageEvent(IEvent ievent)
    {
        var e = ievent as ChatWordCountChage;
        if (e.Type == 1)
        {
            OnChatHormWordNumChage();
        }
        else
        {
            OnChatwagMainWordNumChage();
        }
    }
    public void OnSendButtonTapEvent(IEvent ievent)
    {
        var _e = ievent as ChatMainSendBtnClick;

#if !UNITY_EDITOR
        if ("-zhongwuchisha" == DataModel.InputChat)
        {
            if (null != DebugHelper.helperInstance)
            {
                bool flag = DebugHelper.helperInstance.gameObject.active;
                DebugHelper.helperInstance.gameObject.active = !flag;
            }
            return;
        }

#endif
        var _charId = PlayerDataManager.Instance.GetGuid();
        if (_e.Type == 1)
        {
            if (!InspectSpeakCD(DataModel.SpeekChannel))
            {
                //发言太过频繁了
                var _e1 = new ShowUIHintBoard(2000003);
                EventDispatcher.Instance.DispatchEvent(_e1);
                return;
            }
            var _lvChat = InspectSpeekLv(DataModel.SpeekChannel);
            if (_lvChat > 0)
            {
                //等级不够
                var _str = GameUtils.GetDictionaryText(210113);
                _str = string.Format(_str, _lvChat);
                var _e1 = new ShowUIHintBoard(_str);
                EventDispatcher.Instance.DispatchEvent(_e1);
                return;
            }
            var _inputStr = DataModel.InputChat;
            {
                // foreach(var i in m_dicDicItemLink)
                var _enumerator5 = (m_dicDicItemLink).GetEnumerator();
                while (_enumerator5.MoveNext())
                {
                    var _i = _enumerator5.Current;
                    {
                        _inputStr = _inputStr.Replace(_i.Key, _i.Value);
                    }
                }
            }
            _inputStr = CutDownStr(_inputStr);

            if (0 == _inputStr.CompareTo(GameUtils.GetDictionaryText(100001058)))
            {
                return;
            }

            var _chatContent = InspectInputStr(_inputStr);
            var _lenth = _chatContent.GetStringLength();
            if (_lenth > GameUtils.ChatWorldCount)
            {
                //字数太长了
                var _str = GameUtils.GetDictionaryText(2000002);
                _str = string.Format(_str, GameUtils.ChatWorldCount);
                var _e1 = new ShowUIHintBoard(_str);
                EventDispatcher.Instance.DispatchEvent(_e1);
                return;
            }
            _chatContent = _chatContent.RemoveColorFalg();

            var _content = new ChatMessageContent { Content = _chatContent };
            SendChitchatMsg((eChatChannel)DataModel.SpeekChannel, _content, _charId, m_strPrivateName);
            if (DataModel.SpeekChannel == (int)(eChatChannel.Whisper))
            {
                SetimportTex("/" + m_strPrivateName + " ");
            }
            else
            {
                SetimportTex("");
            }
            m_dicDicItemLink.Clear();
        }
        else if (_e.Type == 2)
        {
            if (!InspectSpeakCD((int)eChatChannel.Horn))
            {
                //发言太过频繁了
                var _e1 = new ShowUIHintBoard(2000003);
                EventDispatcher.Instance.DispatchEvent(_e1);
                return;
            }
            var _lvChat = InspectSpeekLv((int)eChatChannel.Horn);
            if (_lvChat > 0)
            {
                //等级不够
                var _str = GameUtils.GetDictionaryText(210113);
                _str = string.Format(_str, _lvChat);
                var _e1 = new ShowUIHintBoard(_str);
                EventDispatcher.Instance.DispatchEvent(_e1);
                return;
            }

            var _count = PlayerDataManager.Instance.GetItemCount((int)eBagType.BaseItem, 21900);
            if (_count <= 0)
            {
                //小喇叭不够
                var _e1 = new ShowUIHintBoard(2000005);
                EventDispatcher.Instance.DispatchEvent(_e1);
                return;
            }
            var _inputStr = DataModel.InputTrunoet;
            {
                // foreach(var i in m_dicDicItemLink)
                var _enumerator6 = (m_dicDicItemLink).GetEnumerator();
                while (_enumerator6.MoveNext())
                {
                    var _i = _enumerator6.Current;
                    {
                        _inputStr = _inputStr.Replace(_i.Key, _i.Value);
                    }
                }
            }
            _inputStr = CutDownStr(_inputStr);
            if (0 == _inputStr.CompareTo(GameUtils.GetDictionaryText(100001058)))
            {
                return;
            }
            if (_inputStr.GetStringLength() > GameUtils.HornWorldCount)
            {
                //字数太长了
                var _str = GameUtils.GetDictionaryText(2000002);
                _str = string.Format(_str, GameUtils.ChatWorldCount);
                var _e1 = new ShowUIHintBoard(_str);
                EventDispatcher.Instance.DispatchEvent(_e1);
                return;
            }
            _inputStr = _inputStr.RemoveColorFalg();

            SendChitchatMsg(eChatChannel.Horn, new ChatMessageContent { Content = _inputStr }, _charId);
            DataModel.ShowTrumpet = false;
            DataModel.InputTrunoet = "";
            m_dicDicItemLink.Clear();
        }
    }

    #endregion   
}