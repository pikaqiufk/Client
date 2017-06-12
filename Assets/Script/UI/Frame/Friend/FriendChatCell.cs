#region using

using System;
using System.Collections.Generic;
using System.IO;
using DataContract;
using DataTable;
using EventSystem;
using LZ4s;
using ProtoBuf;
using UnityEngine;

#endregion

namespace GameUI
{
    public class FriendChatCell : MonoBehaviour
    {
        public UIWidget BackGround;
        public UIWidget Content;
        public float FaceScale = 1.0f;
        public int FontSize;
        public UISprite HeadIco;
        public UISprite HeadInfo;
        private readonly int MaxWidth = 400;
        private readonly List<CharInfoNode> ChatNodeList = new List<CharInfoNode>();
        private ChatMessageData chatDataMsg;
        private readonly List<GameObject> ColliderList = new List<GameObject>();
        private bool isMine;
        private bool isFace;
        private readonly int LineOffset = 2;
        private readonly List<UIWidget> WidgetList = new List<UIWidget>();
        private int xPos = 20;
        private int yPos = -5;
        private bool updateCollider;
        private int totalLength;
        private ChatVoiceCellLogic voiceCell;
        public UILabel TimeInfo;
        public UIWidget TimeWidget;

        public object ChatMessageData
        {
            get { return chatDataMsg; }
            set
            {
                var chatInfo = value as ChatMessageData;
                chatDataMsg = chatInfo;
                InitMessage();
            }
        }

        private static byte[] decodeBuffer = new byte[1024 * 32];
        public void AddChatNode(bool checkSensitive, string str)
        {
            ChatInfoNodeData data = null;
            try
            {
                var bytes = Convert.FromBase64String(str);
                var unwrapped = LZ4Codec.Decode32(bytes, 0, bytes.Length, decodeBuffer, 0, decodeBuffer.Length, false);
                using (var ms = new MemoryStream(decodeBuffer, 0, unwrapped, false))
                {
                    data = Serializer.Deserialize<ChatInfoNodeData>(ms);
                }
            }
            catch (Exception)
            {
                // ignored
            }

            if (data == null)
            {
                ChatNodeList.Add(new CharInfoNode(checkSensitive, str));
                return;
            }
            if (data.Type == (int) eChatLinkType.Dictionary)
            {
                //AnalyseDictionaryNode(data);
                return;
            }
            var infoNode = new CharInfoNode(false, "", data);
            ChatNodeList.Add(infoNode);
        }

        private void AddXPos(int offset)
        {
            xPos += offset;
            if (xPos > totalLength)
            {
                totalLength = xPos;
                Content.width = totalLength + 10;

                if (isMine)
                {
                    var loc = Content.transform.localPosition;
                    loc.x = BackGround.width - 60 - totalLength;
                    Content.transform.localPosition = loc;
                }
            }
        }

        private void AddVoiceLabel()
        {
            var strContent = chatDataMsg.Content;
            if (string.IsNullOrEmpty(strContent))
            {
                return;
            }
            var height = FontSize;
            height += LineOffset;
            yPos -= height;
            BackGround.height += height;
            Content.height += height;
            CreateTextLabel(strContent);
        }

        private void ParseText(bool checkSensitive, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            var startIndex = 0;
            var endIndex = 0;
            var startString = "";
            var endString = text;
            string str;
            while (!string.IsNullOrEmpty(endString))
            {
                startIndex = endString.IndexOf(SpecialCode.ChatBegin);
                endIndex = endString.IndexOf(SpecialCode.ChatEnd);
                if (startIndex >= 0)
                {
                    if (startIndex > 0)
                    {
                        str = endString.Substring(0, startIndex);
                        ChatNodeList.Add(new CharInfoNode(checkSensitive, str));
                    }
                    str = endString.Substring(startIndex + SpecialCode.ChatBegin.Length,
                        endIndex - startIndex - SpecialCode.ChatEnd.Length);
                    AddChatNode(checkSensitive, str);
                    endString = endString.Substring(endIndex + SpecialCode.ChatEnd.Length);
                }
                else
                {
                    ChatNodeList.Add(new CharInfoNode(checkSensitive, endString));
                    endString = "";
                }
            }
        }

        private void WrapLine()
        {
            WidgetList.Clear();

            var height = 0;
            if (isFace)
            {
                xPos = (int) (FontSize*FaceScale*0.5f);
                height = (int) (FontSize*FaceScale);
                isFace = false;
            }
            else
            {
                xPos = (int) (FontSize*0.5f);
                height = FontSize;
            }
            height += LineOffset;
            yPos -= height;
            BackGround.height += height;
            Content.height += height;
        }

        private void CreateFaceLabel(CharInfoNode node)
        {
            var obj = ComplexObjectPool.NewObjectSync("UI/ChatMain/ChatFaceNode.prefab");
            if (obj == null)
            {
                return;
            }
            obj.SetActive(true);
            var faceSprite = obj.GetComponent<UISprite>();
            var objTransform = obj.transform;
            //objTransform.parent = Content.transform;
            objTransform.SetParentEX(Content.transform);
            objTransform.localScale = Vector3.one;

            obj.SetActive(true);
            var faceWidth = (int) (FontSize*FaceScale);
            var faceHeight = (int) (FontSize*FaceScale);
            faceSprite.width = faceWidth;
            faceSprite.height = faceHeight;
            if (MaxWidth < xPos + faceWidth)
            {
                //换行

                xPos = (int) (FontSize*0.5f);

                if (isFace)
                {
                    var y = FontSize*FaceScale;
                    yPos -= (int) y;
                    BackGround.height += (int) y;
                    Content.height += FontSize;
                    isFace = false;
                }
                else
                {
                    yPos -= FontSize;
                    BackGround.height += FontSize;
                    Content.height += FontSize;
                }
                yPos -= LineOffset;


                WidgetList.Clear();
            }
            var face = node.NodeData.Id;
            var tbFace = Table.GetFace(face);
            if (tbFace == null)
            {
                return;
            }
            if (isFace == false)
            {
                isFace = true;
                var c = WidgetList.Count;
                for (var i = 0; i < c; i++)
                {
                    var w = WidgetList[i];
                    var l = w.transform.localPosition;
                    l.y -= FontSize*(FaceScale - 1.0f)*0.5f;
                    w.transform.localPosition = l;
                }
            }

            faceSprite.spriteName = tbFace.Name + "_1";
            var ani = obj.GetComponent<UISpriteAnimation>();
            ani.namePrefix = tbFace.Name + "_";
            var faceY = yPos;

            objTransform.localPosition = new Vector3(xPos, faceY, 0);
            AddXPos(faceWidth);
        }

        private void CreateItemLabel(CharInfoNode node)
        {
            var obj = ComplexObjectPool.NewObjectSync("UI/ChatMain/ChatLableNode.prefab");

            if (obj == null)
            {
                return;
            }
            var objTransform = obj.transform;
            objTransform.SetParentEX(Content.transform);
            obj.gameObject.collider.enabled = true;

            obj.SetActive(true);
            var label = obj.GetComponent<UILabel>();
            ColliderList.Add(obj.gameObject);
            var sbstr = "";
            var text = "";
            label.fontSize = FontSize;

            NGUIText.fontSize = label.fontSize;
            NGUIText.finalSize = label.fontSize;
            NGUIText.dynamicFont = label.trueTypeFont;
            NGUIText.regionWidth = MaxWidth - xPos;
            NGUIText.pixelDensity = 1.0f;
            NGUIText.maxLines = 10000;
            NGUIText.regionHeight = 10000;
            NGUIText.finalLineHeight = label.fontSize;
            NGUIText.fontScale = 1.0f;
            var chatLogic = obj.GetComponent<ChatLableLogic>();
            chatLogic.InfoNode = node;
            var tbTable = Table.GetItemBase(node.NodeData.Id);
            var value = tbTable.Name;
            var itemClor = GameUtils.GetTableColor(tbTable.Quality);
            //label.color = itemClor;
            NGUIText.WrapText(value, out sbstr, false, true);
            var index = sbstr.IndexOf("\n");
            label.text = "[" + GameUtils.ColorToString(itemClor) + "]" + "[" + value + "]" + "[-]";
            label.text = value;
            var y = yPos;

            if (index > -1)
            {
                xPos = (int) (FontSize*0.5f);

                var height = 0;
                if (isFace)
                {
                    height = (int) (FontSize*FaceScale);
                    isFace = false;
                }
                else
                {
                    height = FontSize;
                }
                height += LineOffset;
                y -= height;
                BackGround.height += height;
                Content.height += height;
            }
            else
            {
                if (isFace)
                {
                    y -= FontSize;
                }
            }

            label.gameObject.transform.localPosition = new Vector3(xPos, y, 0);
            objTransform.localScale = Vector3.one;
            AddXPos(label.width);
        }

        private void CreateTextLabel(string value)
        {
            var objres = ResourceManager.PrepareResourceSync<GameObject>("UI/ChatMain/ChatLableNode.prefab");
            var obj = Instantiate(objres) as GameObject;
            if (obj == null)
            {
                return;
            }
            var objTransform = obj.transform;
            //objTransform.parent = Content.transform;
            objTransform.SetParentEX(Content.transform);
            obj.gameObject.collider.enabled = false;
            var label = obj.GetComponent<UILabel>();

            var sbstr = "";
            var text = "";


            label.fontSize = FontSize;
            NGUIText.fontSize = label.fontSize;
            NGUIText.finalSize = label.fontSize;
            NGUIText.dynamicFont = label.trueTypeFont;
            if (xPos > MaxWidth)
            {
                WrapLine();
            }
            NGUIText.regionWidth = MaxWidth - xPos;
            NGUIText.maxLines = 10000;
            NGUIText.pixelDensity = 1.0f;
            NGUIText.regionHeight = 10000;
            NGUIText.finalLineHeight = label.fontSize;
            NGUIText.fontScale = 1.0f;
            NGUIText.WrapText(value, out sbstr, false, true);
            var index = sbstr.IndexOf("\n");

            if (index > -1)
            {
                text = sbstr.Substring(0, index);
            }
            else
            {
                text = sbstr;
            }
            label.text = text;

            var y = yPos;
            if (isFace)
            {
                y -= (int) (FontSize*(FaceScale - 1.0f)*0.5f);
            }
            objTransform.localPosition = new Vector3(xPos, y, 0);
            objTransform.localScale = Vector3.one;
            AddXPos(label.width);
            sbstr = sbstr.Remove(0, text.Length);

            WidgetList.Add(label);

            if (sbstr.Length > 0)
            {
                //换行
                WrapLine();
                sbstr = sbstr.Replace("\n", "");
                CreateTextLabel(sbstr);
            }
        }

        private void CreateVoiceLabel(CharInfoNode node)
        {
            var obj = ComplexObjectPool.NewObjectSync("UI/ChatMain/ChatVoiceCell.prefab");
            if (obj == null)
            {
                return;
            }
            obj.SetActive(true);
            voiceCell = obj.GetComponent<ChatVoiceCellLogic>();
            if (voiceCell == null)
            {
                return;
            }

            voiceCell.SetChatInfo(FontSize, chatDataMsg.Seconds, MaxWidth);
            voiceCell.Seconds = chatDataMsg.Seconds;
            voiceCell.SoundData = chatDataMsg.SoundData;
            var objTransform = obj.transform;
            //objTransform.parent = Content.transform;
            objTransform.SetParentEX(Content.transform);
            objTransform.localScale = Vector3.one;
            objTransform.localPosition = new Vector3(xPos, yPos, 0);

            obj.SetActive(true);
            AddXPos(200);
            xPos = 10;
            AddVoiceLabel();
        }

        public void Display()
        {
            var __list1 = ChatNodeList;
            var __listCount1 = __list1.Count;
            for (var __i1 = 0; __i1 < __listCount1; ++__i1)
            {
                var infoNode = __list1[__i1];
                switch (infoNode.LinkType)
                {
                case eChatLinkType.Text:
                    CreateTextLabel(infoNode.InfoContent);
                    break;
                case eChatLinkType.Face:
                    CreateFaceLabel(infoNode);
                    break;
                case eChatLinkType.Equip:
                    CreateItemLabel(infoNode);
                    break;
                    //                         case eChatLinkType.Postion:
                    //                             CreatePostionLabel(infoNode);
                    //                             break;
                    //                         case eChatLinkType.Character:
                    //                             CreateCharacterLabel(infoNode);
                    //                             break;
                case eChatLinkType.Voice:
                    CreateVoiceLabel(infoNode);
                    break;
                default:
                    break;
                }
            }
            if (isFace)
            {
                isFace = false;
                BackGround.height += (int) (FontSize*(FaceScale - 1.0f));
                Content.height += (int) (FontSize*(FaceScale - 1.0f));
            }
            BackGround.height -= LineOffset;
        }

        public int GetMaxLength()
        {
            return totalLength;
        }

        private void InitMessage()
        {
            if (chatDataMsg == null)
            {
                return;
            }
            isMine = chatDataMsg.Type == (int) eChatChannel.MyWhisper;

            var roleId = chatDataMsg.RoleId;
            var tbChar = Table.GetCharacterBase(roleId);
            if (tbChar == null)
            {
                return;
            }
            GameUtils.SetSpriteIcon(HeadInfo, tbChar.HeadIcon);
            ResetMessage();
            SetTime();

            if (chatDataMsg.SoundData != null)
            {
                var data = new ChatInfoNodeData();
                data.Type = (int) eChatLinkType.Voice;
                data.SoundData = chatDataMsg.SoundData;
                var node = new CharInfoNode(false, "", data);
                ChatNodeList.Add(node);
            }
            else
            {
                ParseText(ShouldCheckSensitiveWord(chatDataMsg), chatDataMsg.Content);
            }
            Display();
        }

        private bool ShouldCheckSensitiveWord(ChatMessageData chatDataMsg)
        {
            return chatDataMsg != null 
                && chatDataMsg.Type != (int)eChatChannel.System
                && chatDataMsg.Type != (int)eChatChannel.SystemScroll
                && chatDataMsg.Type != (int)eChatChannel.WishingDraw
                && chatDataMsg.Type != (int)eChatChannel.WishingGroup
                && chatDataMsg.Type != (int)eChatChannel.Scene
                && chatDataMsg.Type != (int)eChatChannel.Help;
        }

        private void LateUpdate()
        {
#if !UNITY_EDITOR
            try
            {
#endif

                if (updateCollider)
                {
                    updateCollider = false;
                    foreach (var node in ColliderList)
                    {
                        var o = node.GetComponent<BoxCollider>();
                        if (o)
                        {
                            NGUITools.UpdateWidgetCollider(o, true);
                        }
                    }
                }

#if !UNITY_EDITOR
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
#endif
        }

        private void OnGetVoiceDetails(IEvent ievent)
        {
            var e = ievent as ChatVoiceContent;
            if (e.SoundData != chatDataMsg.SoundData)
            {
                return;
            }
            chatDataMsg.Content = e.Content;

            AddVoiceLabel();
            var e1 = new FrindChatNotifyProvider(BackGround);
            EventDispatcher.Instance.DispatchEvent(e1);
        }

        private void OnDestroy()
        {
#if !UNITY_EDITOR
            try
            {
#endif

                EventDispatcher.Instance.RemoveEventListener(ChatVoiceContent.EVENT_TYPE, OnGetVoiceDetails);

#if !UNITY_EDITOR
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
#endif
        }

        public void ResetMessage()
        {
            xPos = 10;
            yPos = -10;
            updateCollider = true;
            ColliderList.Clear();
            ChatNodeList.Clear();
            isFace = false;
            var trans = Content.transform;
            BackGround.height = 45;
            Content.height = 40;
            WidgetList.Clear();
            for (var i = trans.childCount - 1; i >= 0; i--)
            {
                var t = trans.GetChild(i);
                if (t.name.Contains("ChatLableNode")
                    || t.name.Contains("ChatFaceNode")
                    || t.name.Contains("ChatVoiceCell"))
                {
                    t.gameObject.SetActive(false);
                    ComplexObjectPool.Release(t.gameObject);
                    //Destroy(t.gameObject);
                }
            }
        }

        private void SetPositionX(int offset)
        {
            xPos = offset;
        }

        public void SetTime()
        {
            var headX = 0;
            if (isMine == false)
            {
                headX = 3;
            }
            else
            {
                headX = 495;
            }
            if (chatDataMsg.ShowTime == 0)
            {
                TimeWidget.gameObject.SetActive(false);
                HeadIco.transform.localPosition = new Vector3(headX, 0, 0);

                Content.transform.localPosition = new Vector3(45, 0, 0);
            }
            else
            {
                TimeWidget.gameObject.SetActive(true);
                var time = DateTime.FromBinary(chatDataMsg.Times);
                TimeInfo.text = string.Format("{0}-{1}-{2} {3}:{4}", time.Year, time.Month, time.Day, time.Hour, time.Minute);

                HeadIco.transform.localPosition = new Vector3(headX, -20, 0);
                Content.transform.localPosition = new Vector3(45, -20, 0);
                BackGround.height += 20;
            }
        }

        private void Start()
        {
#if !UNITY_EDITOR
            try
            {
#endif

                EventDispatcher.Instance.AddEventListener(ChatVoiceContent.EVENT_TYPE, OnGetVoiceDetails);

#if !UNITY_EDITOR
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
#endif
        }

        private void Update()
        {
#if !UNITY_EDITOR
            try
            {
#endif


#if !UNITY_EDITOR
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
#endif
        }
    }
}
