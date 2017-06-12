#region using

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ClientDataModel;
using DataContract;
using DataTable;
using EventSystem;
using LZ4s;
using ProtoBuf;
using Shared;
using UnityEngine;

#endregion

public class CharInfoNode
{
    public CharInfoNode(bool checkSensitive, string info, ChatInfoNodeData data = null)
    {
        if (data == null)
        {
            NodeData = new ChatInfoNodeData();
            NodeData.Type = (int) eChatLinkType.Text;
        }
        else
        {
            NodeData = data;
        }
        if (LinkType == eChatLinkType.Text && checkSensitive)
            InfoContent = info.CheckSensitive();
        else
            InfoContent = info;
    }

    public ulong CharacterId { get; set; }
    public string InfoContent { get; private set; }

    public eChatLinkType LinkType
    {
        get { return (eChatLinkType) NodeData.Type; }
    }

    public ChatInfoNodeData NodeData { get; set; }
}

public class ChatMessageLogic : MonoBehaviour
{
    public UIWidget BackGround;
    public UILabel ChannelLabel;
    public float FaceScale = 1.0f;
    public int FontSize;
    public bool IsCenter = false;
    public ListItemLogic ItemLogic;
    public UILabel MaoHao;
    //public float Face
    public int MaxLineWidth = 900;
    private int mChannelId;
    private readonly List<CharInfoNode> mCharInfoNodes = new List<CharInfoNode>();
    private string mCharName;
    private ChatMessageDataModel mChatMessageData;
    private readonly List<GameObject> mColliderObjects = new List<GameObject>();
    private string mColorString;
    private bool mLineFace;
    private readonly int mLineOffset = 2;
    private int mPositionX = 10;
    private int mPositionY;
    private bool mRefreshCollider;
    private string mText;
    private int mTotalLength;
    private ChatVoiceCellLogic mVoiceCell;
    public UILabel NameLabel;
    public UILabel NameLabelBegin;
    public UILabel NameLabelEnd;
    public UILabel VipLabel;
    public UISprite VipSprite;

    public int ChannelId
    {
        get { return mChannelId; }
        set
        {
            ResetMessage();
            mChannelId = value;
            if (mChannelId == -1)
            {
                return;
            }
            SetPositionX((int) (FontSize*0.5f));
            mPositionY = 0;
            BackGround.height = (int) (FontSize + FontSize*0.2f);

            if (mChannelId == 99)
            {
                ChatRecord = Table.GetChatInfo(6);
            }
            else
            {
                ChatRecord = Table.GetChatInfo(mChannelId);
            }

            StrokeColor = GameUtils.GetTableColor(ChatRecord.Stroke);
            if (StrokeColor == MColor.white)
            {
                StrokeEffect = UILabel.Effect.None;
            }
            else
            {
                StrokeEffect = UILabel.Effect.Shadow;
            }

            var strColor = GameUtils.GetTableColorString(ChatRecord.ColorId[0]);
            if (strColor == "FFFFFF")
            {
                mColorString = "";
            }
            else
            {
                mColorString = strColor;
            }


            //positionX = ChannelLabel.width + 3;

            if (mChannelId == 8
                || mChannelId == 9)
            {
                ChannelLabel.gameObject.SetActive(false);
            }
            else
            {
                var str = GameUtils.GetDictionaryText(242010);
                str = string.Format(str, ChatRecord.Desc);
                ChannelLabel.gameObject.SetActive(true);
                ChannelLabel.fontSize = FontSize;
                SetLableStroke(ChannelLabel);
                SetLableColor(ChannelLabel, str);
                SetPositionX(ChannelLabel.width + 3);
            }
        }
    }

    public string CharName
    {
        get { return mCharName; }
        set
        {
            mCharName = value;
            if (ChannelId == (int) eChatChannel.City && ChatMessageData.ChannelName != "")
            {
                if (ChatMessageData.ChannelName != "")
                {
                    mCharName = "[" + ChatMessageData.ChannelName + "]" + value;
                }
            }
            NameLabelEnd.gameObject.SetActive(false);
            NameLabelBegin.gameObject.SetActive(false);
            NameLabel.gameObject.SetActive(false);
            if (ChannelId == 0)
            {
                NameLabel.gameObject.SetActive(false);
                NameLabelEnd.gameObject.SetActive(false);
                NameLabelBegin.gameObject.SetActive(false);
            }
            else if (ChannelId == 99)
            {
                NameLabelBegin.gameObject.SetActive(true);
                //你对
                var str = GameUtils.GetDictionaryText(270060);
                SetLableColor(NameLabelBegin, str);
                NameLabel.text = mCharName;
                //说:
                str = GameUtils.GetDictionaryText(270061);
                SetLableColor(NameLabelEnd, str);
                NameLabelEnd.gameObject.SetActive(true);
                NameLabel.gameObject.SetActive(true);
                mColliderObjects.Add(NameLabel.gameObject);
            }
            else if (ChannelId == 6)
            {
                NameLabelBegin.gameObject.SetActive(false);
                NameLabelEnd.gameObject.SetActive(true);
                NameLabel.text = mCharName;
                //对你说:
                var str = GameUtils.GetDictionaryText(270062);
                SetLableColor(NameLabelEnd, str);
                NameLabel.gameObject.SetActive(true);
                mColliderObjects.Add(NameLabel.gameObject);
            }
            else
            {
                if (!string.IsNullOrEmpty(value))
                {
                    NameLabelEnd.gameObject.SetActive(false);
                    NameLabelBegin.gameObject.SetActive(false);
                    var str = mCharName; //+ ":";
                    SetLableColor(NameLabel, str);
                    mColliderObjects.Add(NameLabel.gameObject);
                    NameLabel.gameObject.SetActive(true);
                }
            }

            if (NameLabelBegin.gameObject.activeSelf)
            {
                NameLabelBegin.fontSize = FontSize;
                SetLableStroke(NameLabelBegin);
                NameLabelBegin.transform.localPosition = new Vector3(mPositionX, 0, 0);
                AddPositionX(NameLabelBegin.width);
            }

            if (NameLabel.gameObject.activeSelf)
            {
                var labelTransform = NameLabel.transform;
                NameLabel.fontSize = FontSize;
                labelTransform.localPosition = new Vector3(mPositionX, labelTransform.localPosition.y, 0);
                //增加：
                if (MaoHao != null)
                {
                    MaoHao.gameObject.transform.localPosition = labelTransform.localPosition +
                                                                new Vector3(NameLabel.width, 0, 0);
                    SetLableStroke(MaoHao);
                    AddPositionX(MaoHao.width);
                    MaoHao.gameObject.SetActive(true);
                }
                SetLableStroke(NameLabel);
                AddPositionX(NameLabel.width);
            }
            else
            {
                if (MaoHao != null)
                {
                    MaoHao.gameObject.SetActive(false);
                }
            }

            if (NameLabelEnd.gameObject.activeSelf)
            {
                NameLabelEnd.fontSize = FontSize;
                SetLableStroke(NameLabelEnd);
                NameLabelEnd.transform.localPosition = new Vector3(mPositionX, 0, 0);
                AddPositionX(NameLabelEnd.width);
            }

            //vip显示
            if (VipLabel != null)
            {
                if (NameLabel.gameObject.activeSelf && ChatMessageData.VipLevel > 0)
                {
                    //MaxLineWidth = 680;
                    VipLabel.text = ChatMessageData.VipLevel.ToString();
                    VipLabel.gameObject.SetActive(true);
                    VipSprite.gameObject.SetActive(true);
                    VipSprite.transform.localPosition = NameLabel.transform.localPosition +
                                                        new Vector3(NameLabel.width, -3, 0);
                    //var moveSize = new Vector3(VipLabel.width, 0, 0) + new Vector3(VipSprite.width, 0, 0) - new Vector3(5,0,0);
                    //NameLabelBegin.transform.localPosition = NameLabelBegin.transform.localPosition + moveSize;
                    //NameLabel.transform.localPosition = NameLabel.transform.localPosition + moveSize;
                    //NameLabelEnd.transform.localPosition = NameLabelEnd.transform.localPosition + moveSize;
                    AddPositionX(VipLabel.width + VipSprite.width - 5);
                    if (MaoHao != null)
                    {
                        MaoHao.gameObject.transform.localPosition = new Vector3(mPositionX - 10, 0, 0);
                    }
                }
                else
                {
                    VipLabel.gameObject.SetActive(false);
                    VipSprite.gameObject.SetActive(false);
                }
            }

            AddPositionX(2);
        }
    }

    public ChatMessageDataModel ChatMessageData
    {
        get { return mChatMessageData; }
        set
        {
            mTotalLength = 0;
            mChatMessageData = value;
            ChannelId = mChatMessageData.Type;
            CharName = mChatMessageData.Name;
            if (mChatMessageData.SoundData != null)
            {
                var data = new ChatInfoNodeData();
                data.Type = (int) eChatLinkType.Voice;
                data.SoundData = mChatMessageData.SoundData;
                var node = new CharInfoNode(false, "", data);
                mCharInfoNodes.Add(node);
            }
            else
            {
                if (!string.IsNullOrEmpty(mChatMessageData.Content))
                {
                    Text = mChatMessageData.Content;
                }
            }
            Display();
        }
    }
   
    public ChatInfoRecord ChatRecord { get; set; }
    public Color StrokeColor { get; set; }
    public UILabel.Effect StrokeEffect { get; set; }

    public string Text
    {
        get { return mText; }
        set
        {
            mText = value;
            Analyse(mText);
        }
    }


    private static byte[] decodeBuffer = new byte[1024 * 32]; 
    public void AddChatNode(string str)
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
            mCharInfoNodes.Add(new CharInfoNode(ShouldCheckSensitiveWord(), str));
            return;
        }
        if (data.Type == (int) eChatLinkType.Dictionary)
        {
            AnalyseDictionaryNode(data);
            return;
        }
        var infoNode = new CharInfoNode(ShouldCheckSensitiveWord(), "", data);
        mCharInfoNodes.Add(infoNode);
    }

    private void AddPositionX(int offset)
    {
        mPositionX += offset;
        if (mPositionX > mTotalLength)
        {
            mTotalLength = mPositionX;
        }
    }

    private void AddVoiceLabel()
    {
        var strContent = ChatMessageData.Content;
        if (string.IsNullOrEmpty(strContent))
        {
            return;
        }
        var height = FontSize;
        height += mLineOffset;
        mPositionY -= height;
        BackGround.height += height;
        CreateTextLabel(strContent);
    }

    private void Analyse(string text)
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
                    mCharInfoNodes.Add(new CharInfoNode(ShouldCheckSensitiveWord(), str));
                }
                str = endString.Substring(startIndex + SpecialCode.ChatBegin.Length,
                    endIndex - startIndex - SpecialCode.ChatEnd.Length);
                AddChatNode(str);
                endString = endString.Substring(endIndex + SpecialCode.ChatEnd.Length);
            }
            else
            {
                mCharInfoNodes.Add(new CharInfoNode(ShouldCheckSensitiveWord(), endString));
                endString = "";
            }
        }
    }

    private bool ShouldCheckSensitiveWord()
    {
        if (ChatMessageData == null)
        {
            return false;
        }
        return ChatMessageData.Type != (int) eChatChannel.System 
            && ChatMessageData.Type != (int) eChatChannel.SystemScroll
            && ChatMessageData.Type != (int)eChatChannel.WishingDraw
            && ChatMessageData.Type != (int)eChatChannel.WishingGroup
            && ChatMessageData.Type != (int)eChatChannel.Scene
            && ChatMessageData.Type != (int)eChatChannel.Help;
    }

    private void AnalyseDictionaryNode(ChatInfoNodeData data)
    {
        var strDic = GameUtils.GetDictionaryText(data.Id);
        var begin = 0;
        var flag = 0;
        while (begin != -1)
        {
            begin = strDic.IndexOf("{", StringComparison.Ordinal);
            var end = strDic.IndexOf("}", StringComparison.Ordinal);

            if (begin != -1 && end != -1)
            {
                var perColor = "";
                if (begin >= 8)
                {
                    var s = strDic.Substring(begin - 8, 8);
                    if (Regex.IsMatch(s, GameUtils.BeginCoclorStrRegex))
                    {
                        perColor = s;
                    }
                }

                var endColor = "";
                if (strDic.Length >= end + 3)
                {
                    var s = strDic.Substring(end + 1, 3);
                    if (s == GameUtils.EndCoclorStrRegex)
                    {
                        end += 3;
                        endColor = s;
                    }
                }


                if (begin != 0)
                {
                    var s = strDic.Substring(0, begin);
                    mCharInfoNodes.Add(new CharInfoNode(ShouldCheckSensitiveWord(), s));
                }

                if (data.StrExData.Count == flag)
                {
                    break;
                }

                var strEx = data.StrExData[flag];
                if (strEx.Length > 0 && strEx[0] == '#')
                {
                    var args = strEx.Split(':');
                    if (args[0] == "#ItemBase.Name")
                    {
                        var itemId = args[1].ToInt();
                        var newData = new ChatInfoNodeData();
                        newData.Type = (int) eChatLinkType.Equip;
                        newData.Id = itemId;
                        newData.ExData.AddRange(data.ExData);
                        mCharInfoNodes.Add(new CharInfoNode(ShouldCheckSensitiveWord(), "", newData));
                    }
                    else if (args[0] == "#Character")
                    {
                        var name = args[1];
                        var newData = new ChatInfoNodeData();
                        newData.Type = (int) eChatLinkType.Character;

                        if (perColor != "")
                        {
                            name = perColor + name;
                        }
                        if (endColor != "")
                        {
                            name = name + endColor;
                        }

                        var node = new CharInfoNode(ShouldCheckSensitiveWord(), name, newData);
                        node.CharacterId = args[2].ToUlong();
                        mCharInfoNodes.Add(node);
                    }
                    else if (args[0] == "#Scene.Name")
                    {
                        var ret = "";
                        var sceneId = args[1].ToInt();
                        var tbScene = Table.GetScene(sceneId);
                        if (tbScene != null)
                        {
                            ret += tbScene.Name;
                        }
                        if (args.Length > 2)
                        {
                            for (var i = 2; i < args.Length; i++)
                            {
                                sceneId = args[i].ToInt();
                                tbScene = Table.GetScene(sceneId);
                                if (tbScene != null)
                                {
                                    ret += "," + tbScene.Name;
                                }
                            }
                        }
                        mCharInfoNodes.Add(new CharInfoNode(ShouldCheckSensitiveWord(), ret));
                    }
                }
                else
                {
                    if (perColor != "")
                    {
                        strEx = perColor + strEx;
                    }
                    if (endColor != "")
                    {
                        strEx = strEx + endColor;
                    }

                    mCharInfoNodes.Add(new CharInfoNode(ShouldCheckSensitiveWord(), strEx));
                }
                flag++;
                strDic = strDic.Substring(end + 1);
            }
        }
        if (!string.IsNullOrEmpty(strDic))
        {
            mCharInfoNodes.Add(new CharInfoNode(ShouldCheckSensitiveWord(), strDic));
        }
    }

    private void ChangeLine()
    {
        var height = 0;
        if (mLineFace)
        {
            mPositionX = (int) (FontSize*FaceScale*0.5f);
            height = (int) (FontSize*FaceScale);
            mLineFace = false;
        }
        else
        {
            mPositionX = (int) (FontSize*0.5f);
            height = FontSize;
        }
        height += mLineOffset;
        mPositionY -= height;
        BackGround.height += height;
    }

    private void CreateCharacterLabel(CharInfoNode node)
    {
        //var objres = ResourceManager.PrepareResourceSync<GameObject>("UI/ChatMain/ChatLableNode.prefab");
        //var obj = Instantiate(objres) as GameObject;
        var obj = ComplexObjectPool.NewObjectSync("UI/ChatMain/ChatLableNode.prefab");
        if (obj == null)
        {
            return;
        }
        var objTransform = obj.transform;
        //objTransform.parent = BackGround.transform;
        objTransform.SetParentEX(BackGround.transform);
        obj.gameObject.collider.enabled = true;
        obj.SetActive(true);
        var chatLogic = obj.GetComponent<ChatLableLogic>();
        chatLogic.InfoNode = node;
        var label = obj.GetComponent<UILabel>();
        var sbstr = "";
        var text = "";
        //SetLableColor(label);
        label.fontSize = FontSize;
        NGUIText.fontSize = label.fontSize;
        NGUIText.finalSize = label.fontSize;
        NGUIText.dynamicFont = label.trueTypeFont;
        NGUIText.regionWidth = MaxLineWidth - mPositionX;
        NGUIText.pixelDensity = 1.0f;
        NGUIText.maxLines = 10000;
        NGUIText.regionHeight = 10000;
        NGUIText.finalLineHeight = label.fontSize;
        NGUIText.fontScale = 1.0f;
        var value = string.Format("[{0}]", node.InfoContent);
        NGUIText.WrapText(value, out sbstr, false, true);
        label.text = value;

        var index = sbstr.IndexOf("\n");
        if (index > -1)
        {
            mPositionX = (int) (FontSize*0.5f);
            var height = mLineOffset;
            if (mLineFace)
            {
                height = (int) (FontSize*FaceScale);
                mLineFace = false;
            }
            else
            {
                height = FontSize;
            }
            mPositionY -= height;
            BackGround.height += height;
        }
        label.gameObject.transform.localPosition = new Vector3(mPositionX, mPositionY, 0);
        objTransform.localScale = Vector3.one;
        AddPositionX(label.width);
    }

    private void CreateFaceLabel(CharInfoNode node)
    {
//         var objres = ResourceManager.PrepareResourceSync<GameObject>("UI/ChatMain/ChatFaceNode.prefab");
// 
//         var obj = Instantiate(objres) as GameObject;
        var obj = ComplexObjectPool.NewObjectSync("UI/ChatMain/ChatFaceNode.prefab");
        if (obj == null)
        {
            return;
        }

        var faceSprite = obj.GetComponent<UISprite>();
        var objTransform = obj.transform;
        //objTransform.parent = BackGround.transform;
        objTransform.SetParentEX(BackGround.transform);
        objTransform.localScale = Vector3.one;
        obj.SetActive(true);
        var faceWidth = (int) (FontSize*FaceScale);
        var faceHeight = (int) (FontSize*FaceScale);
        faceSprite.width = faceWidth;
        faceSprite.height = faceHeight;
        if (MaxLineWidth < mPositionX + faceWidth)
        {
            mPositionX = (int) (FontSize*0.5f);

            if (mLineFace)
            {
                var y = FontSize*FaceScale;
                mPositionY -= (int) y;
                BackGround.height += (int) y;
                mLineFace = false;
            }
            else
            {
                mPositionY -= FontSize;
                BackGround.height += FontSize;
            }
            mPositionY -= mLineOffset;
        }
        var face = node.NodeData.Id;
        var tbFace = Table.GetFace(face);
        if (tbFace == null)
        {
            return;
        }
        mLineFace = true;
        faceSprite.spriteName = tbFace.Name + "_1";
        var ani = obj.GetComponent<UISpriteAnimation>();
        ani.namePrefix = tbFace.Name + "_";
        var faceY = mPositionY;
        if (IsCenter)
        {
            faceY += (int) (FontSize*(FaceScale - 1.0f)/2.0f);
        }
        objTransform.localPosition = new Vector3(mPositionX, faceY, 0);
        AddPositionX(faceWidth);
    }

    private void CreateItemLabel(CharInfoNode node)
    {
        //var objres = ResourceManager.PrepareResourceSync<GameObject>("UI/ChatMain/ChatLableNode.prefab");
        //var obj = Instantiate(objres) as GameObject;
        var obj = ComplexObjectPool.NewObjectSync("UI/ChatMain/ChatLableNode.prefab");
        if (obj == null)
        {
            return;
        }

        var objTransform = obj.transform;
        //objTransform.parent = BackGround.transform;
        objTransform.SetParentEX(BackGround.transform);
        obj.gameObject.collider.enabled = true;
        obj.SetActive(true);
        var label = obj.GetComponent<UILabel>();
        mColliderObjects.Add(obj.gameObject);
        var sbstr = "";
        var text = "";
        label.fontSize = FontSize;

        NGUIText.fontSize = label.fontSize;
        NGUIText.finalSize = label.fontSize;
        NGUIText.dynamicFont = label.trueTypeFont;
        NGUIText.regionWidth = MaxLineWidth - mPositionX;
        NGUIText.pixelDensity = 1.0f;
        NGUIText.maxLines = 10000;
        NGUIText.regionHeight = 10000;
        NGUIText.finalLineHeight = label.fontSize;
        NGUIText.fontScale = 1.0f;
        var chatLogic = obj.GetComponent<ChatLableLogic>();
        chatLogic.InfoNode = node;
        var nodeNodeDataExDataCount0 = node.NodeData.ExData.Count;
        var tbTable = Table.GetItemBase(node.NodeData.Id);
        var value = tbTable.Name;
        var itemClor = GameUtils.GetTableColor(tbTable.Quality);
        //label.color = itemClor;
        NGUIText.WrapText(value, out sbstr, false, true);

        var index = sbstr.IndexOf("\n");

        label.text = "[" + GameUtils.ColorToString(itemClor) + "]" + "[" + value + "]" + "[-]";
        if (index > -1)
        {
            mPositionX = (int) (FontSize*0.5f);

            var height = 0;
            if (mLineFace)
            {
                height = (int) (FontSize*FaceScale);
                mLineFace = false;
            }
            else
            {
                height = FontSize;
            }
            height += mLineOffset;
            mPositionY -= height;
            BackGround.height += height;
        }
        label.gameObject.transform.localPosition = new Vector3(mPositionX, mPositionY, 0);
        objTransform.localScale = Vector3.one;
        AddPositionX(label.width);
    }

    private void CreatePostionLabel(CharInfoNode node)
    {
        //var objres = ResourceManager.PrepareResourceSync<GameObject>("UI/ChatMain/ChatLableNode.prefab");
        //var obj = Instantiate(objres) as GameObject;
        var obj = ComplexObjectPool.NewObjectSync("UI/ChatMain/ChatLableNode.prefab");
        if (obj == null)
        {
            return;
        }
        var objTransform = obj.transform;
        //objTransform.parent = BackGround.transform;
        objTransform.SetParentEX(BackGround.transform);
        obj.gameObject.collider.enabled = true;
        obj.SetActive(true);
        var chatLogic = obj.GetComponent<ChatLableLogic>();
        chatLogic.InfoNode = node;
        var label = obj.GetComponent<UILabel>();
        mColliderObjects.Add(obj.gameObject);
        var sbstr = "";
        var text = "";
        //SetLableColor(label);
        label.fontSize = FontSize;
        NGUIText.fontSize = label.fontSize;
        NGUIText.finalSize = label.fontSize;
        NGUIText.dynamicFont = label.trueTypeFont;
        NGUIText.regionWidth = MaxLineWidth - mPositionX;
        NGUIText.pixelDensity = 1.0f;
        NGUIText.maxLines = 10000;
        NGUIText.regionHeight = 10000;
        NGUIText.finalLineHeight = label.fontSize;
        NGUIText.fontScale = 1.0f;
        var sceneId = node.NodeData.ExData[0];
        var x = node.NodeData.ExData[1];
        var y = node.NodeData.ExData[2];

        var tbScene = Table.GetScene(sceneId);
        var strDic = GameUtils.GetDictionaryText(242011);
        var value = String.Format(strDic, tbScene.Name, x/100, y/100);
        NGUIText.WrapText(value, out sbstr, false, true);
        label.text = value;
        var index = sbstr.IndexOf("\n");
        if (index > -1)
        {
            mPositionX = (int) (FontSize*0.5f);
            var height = mLineOffset;
            if (mLineFace)
            {
                height = (int) (FontSize*FaceScale);
                mLineFace = false;
            }
            else
            {
                height = FontSize;
            }
            mPositionY -= height;
            BackGround.height += height;
        }
        label.gameObject.transform.localPosition = new Vector3(mPositionX, mPositionY, 0);
        objTransform.localScale = Vector3.one;
        AddPositionX(label.width);
    }

    private void CreateTextLabel(string value)
    {
        var obj = ComplexObjectPool.NewObjectSync("UI/ChatMain/ChatLableNode.prefab");
        if (obj == null)
        {
            return;
        }
        var objTransform = obj.transform;
        //objTransform.parent = BackGround.transform;
        objTransform.SetParentEX(BackGround.transform);
        obj.gameObject.collider.enabled = false;
        obj.SetActive(true);
        var label = obj.GetComponent<UILabel>();

        var sbstr = "";
        var text = "";


        label.fontSize = FontSize;
        NGUIText.fontSize = label.fontSize;
        NGUIText.finalSize = label.fontSize;
        NGUIText.dynamicFont = label.trueTypeFont;
        if (mPositionX > MaxLineWidth)
        {
            ChangeLine();
        }
        NGUIText.regionWidth = MaxLineWidth - mPositionX;
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
        SetLableStroke(label);
        SetLableColor(label, text);
        objTransform.localPosition = new Vector3(mPositionX, mPositionY, 0);
        objTransform.localScale = Vector3.one;
        //positionX += label.width;
        AddPositionX(label.width);
        sbstr = sbstr.Remove(0, text.Length);
        if (sbstr.Length > 0)
        {
            ChangeLine();
            sbstr = sbstr.Replace("\n", "");
            CreateTextLabel(sbstr);
        }
    }

    private void CreateVoiceLabel(CharInfoNode node)
    {
        //var objres = ResourceManager.PrepareResourceSync<GameObject>("UI/ChatMain/ChatVoiceCell.prefab");
        //var obj = Instantiate(objres) as GameObject;
        var obj = ComplexObjectPool.NewObjectSync("UI/ChatMain/ChatVoiceCell.prefab");
        if (obj == null)
        {
            return;
        }
        mVoiceCell = obj.GetComponent<ChatVoiceCellLogic>();
        if (mVoiceCell == null)
        {
            return;
        }
        mVoiceCell.SetChatInfo(FontSize, mChatMessageData.Seconds, MaxLineWidth);
        //mVoiceCell.ChatMessageData = mChatMessageData;
        mVoiceCell.Seconds = mChatMessageData.Seconds;
        mVoiceCell.SoundData = mChatMessageData.SoundData;
        var objTransform = obj.transform;
        //objTransform.parent = BackGround.transform;
        objTransform.SetParentEX(BackGround.transform);
        obj.SetActive(true);
        objTransform.localScale = Vector3.one;
        objTransform.localPosition = new Vector3(mPositionX, mPositionY, 0);

        mTotalLength = MaxLineWidth;
        mPositionX = 10;
        AddVoiceLabel();
    }

    public void Display()
    {
        {
            var __list1 = mCharInfoNodes;
            var __listCount1 = __list1.Count;
            for (var __i1 = 0; __i1 < __listCount1; ++__i1)
            {
                var infoNode = __list1[__i1];
                {
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
                        case eChatLinkType.Postion:
                            CreatePostionLabel(infoNode);
                            break;
                        case eChatLinkType.Character:
                            CreateCharacterLabel(infoNode);
                            break;
                        case eChatLinkType.Voice:
                            CreateVoiceLabel(infoNode);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        if (mLineFace)
        {
            mLineFace = false;
            BackGround.height += (int) (FontSize*(FaceScale - 1.0f));
        }
        BackGround.height -= mLineOffset;
    }

    public int GetMaxLength()
    {
        return mTotalLength;
    }

    private void LateUpdate()
    {
#if !UNITY_EDITOR
        try
        {
#endif
        if (mRefreshCollider)
        {
            mRefreshCollider = false;
            {
                var __list2 = mColliderObjects;
                var __listCount2 = __list2.Count;
                for (var __i2 = 0; __i2 < __listCount2; ++__i2)
                {
                    var o = __list2[__i2];
                    {
                        if (o == NameLabel.gameObject)
                        {
                            var c = o.GetComponent<BoxCollider>();
                            NGUITools.UpdateWidgetCollider(c, true, 30, 10, 2, 2);
                        }
                        else
                        {
                            NGUITools.UpdateWidgetCollider(o, true);
                        }
                    }
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

    private void OnChatVoiceContent(IEvent ievent)
    {
        var e = ievent as ChatVoiceContent;
        if (e.SoundData != ChatMessageData.SoundData)
        {
            return;
        }
        AddVoiceLabel();
        var ee = new ChatVoiceContentRefresh(BackGround);
        EventDispatcher.Instance.DispatchEvent(ee);
    }

    public void OnClickNameLable()
    {
        if (ItemLogic == null)
        {
            return;
        }
        var data = mChatMessageData;

        if (data.CharId == 0)
        {
            return;
        }

        if (data.CharId == PlayerDataManager.Instance.GetGuid())
        {
            return;
        }

        var worldPos = UICamera.currentCamera.ScreenToWorldPoint(UICamera.lastTouchPosition);
        var localPos = transform.root.InverseTransformPoint(worldPos);
        localPos.z = 0;
        UIConfig.OperationList.Loction = localPos;

        PlayerDataManager.Instance.ShowCharacterPopMenu(data.CharId, data.Name, 10);
    }

    private void OnDestroy()
    {
#if !UNITY_EDITOR
try
{
#endif

        EventDispatcher.Instance.RemoveEventListener(ChatVoiceContent.EVENT_TYPE, OnChatVoiceContent);


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
        mLineFace = false;
        mRefreshCollider = true;
        mColliderObjects.Clear();
        mCharInfoNodes.Clear();
        var myTransform = transform;
        for (var i = myTransform.childCount - 1; i >= 0; i--)
        {
            var t = myTransform.GetChild(i);
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

    private void SetLableColor(UILabel lable, string content)
    {
        if (string.IsNullOrEmpty(mColorString))
        {
            lable.text = content;
        }
        else
        {
            lable.text = string.Format("[{0}]{1}[-]", mColorString, content);
        }
    }

    private void SetLableStroke(UILabel lable)
    {
        if (StrokeEffect == UILabel.Effect.Outline)
        {
            lable.effectStyle = UILabel.Effect.Outline;
            lable.effectColor = StrokeColor;
        }
        else
        {
            lable.effectStyle = UILabel.Effect.None;
        }
    }

    private void SetPositionX(int offset)
    {
        mPositionX = offset;
    }

    private void Start()
    {
#if !UNITY_EDITOR
        try
        {
#endif
        EventDispatcher.Instance.AddEventListener(ChatVoiceContent.EVENT_TYPE, OnChatVoiceContent);

#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }
}