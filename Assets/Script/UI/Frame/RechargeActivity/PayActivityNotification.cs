#region using

using System.Collections.Generic;
using DataTable;
using Shared;
using UnityEngine;

#endregion

namespace GameUI
{
	public class PayActivityNotification : MonoBehaviour
	{
	    private bool _mNeedSetLine = true;
	    public List<GameObject> _mObj = new List<GameObject>();
	    public List<GameObjectType> _mVarObjList = new List<GameObjectType>();
	    public UIWidget BackGround;
	    public int FontSize = 18;
	    public int MaxLineWidth;
	    private readonly List<NoticeData> mCharInfoNodes = new List<NoticeData>();
	    private readonly int mLineOffset = 6;
	    private readonly int mPicOffset = 4;
	    private int mPositionX = 10;
	    private int mPositionY;
	    private int mTotalLength;
	    public int PicWidth = 25;
	    public int VerticalSpacing = 5;
	
	    public enum eNoticeType
	    {
	        Text = 0,
	        Icon = 1
	    }
	
	    public string Content
	    {
	        set
	        {
	            CleanUp();
	            Analyse(value);
	            Display();
	        }
	    }
	
	    private void AddPositionX(int offset)
	    {
	        mPositionX += offset;
	        if (mPositionX > mTotalLength)
	        {
	            mTotalLength = mPositionX;
	        }
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
	                    mCharInfoNodes.Add(new NoticeData(eNoticeType.Text, -1, str));
	                }
	                str = endString.Substring(startIndex + SpecialCode.ChatBegin.Length,
	                    endIndex - startIndex - SpecialCode.ChatEnd.Length);
	                mCharInfoNodes.Add(new NoticeData(eNoticeType.Icon, str.ToInt(), ""));
	                endString = endString.Substring(endIndex + SpecialCode.ChatEnd.Length);
	            }
	            else
	            {
	                mCharInfoNodes.Add(new NoticeData(eNoticeType.Text, -1, endString));
	                endString = "";
	            }
	        }
	    }
	
	    private void ChangeLine()
	    {
	        if (_mNeedSetLine)
	        {
	            CleanVarObjList();
	        }
	        mPositionX = (int) (FontSize*0.5f);
	        var height = FontSize;
	        height += mLineOffset;
	        mPositionY -= height;
	        _mNeedSetLine = true;
	        _mVarObjList.Clear();
	    }
	
	    public void CleanUp()
	    {
	        mCharInfoNodes.Clear();
	        _mVarObjList.Clear();
	        mPositionX = 10;
	        mPositionY = 0;
	        if (_mObj.Count != 0)
	        {
	            for (var i = _mObj.Count - 1; i >= 0; i--)
	            {
	                ComplexObjectPool.Release(_mObj[i]);
	                _mObj.RemoveAt(i);
	            }
	        }
	    }
	
	    private void CleanVarObjList()
	    {
	        var count = _mVarObjList.Count;
	        for (var i = 0; i < count; i++)
	        {
	            _mVarObjList[i].Obj.transform.localPosition += new Vector3(0, mPicOffset, 0);
	        }
	        mPositionY += mPicOffset;
	    }
	
	    private void CreateFaceLabel(NoticeData node)
	    {
	        var obj = ComplexObjectPool.NewObjectSync("UI/RechargeActivity/RechargeActivityIcon.prefab");
	        if (obj == null)
	        {
	            return;
	        }
	        _mObj.Add(obj);
	        var faceSprite = obj.GetComponent<UISprite>();
	        var objTransform = obj.transform;
	        objTransform.SetParentEX(BackGround.transform);
	        objTransform.localScale = Vector3.one;
	        obj.SetActive(true);
	        var faceWidth = PicWidth; //(int)(FontSize * 2.0f);
	        var faceHeight = PicWidth; // (int)(FontSize * 2.0f);
	        faceSprite.width = faceWidth;
	        faceSprite.height = faceHeight;
	        if (MaxLineWidth < mPositionX + faceWidth)
	        {
	            mPositionX = (int) (FontSize*0.5f);
	            mPositionY -= mLineOffset + FontSize;
	        }
	        var picPos = mPositionY + (PicWidth - FontSize)/2;
	        var iconId = node.IconId;
	        var tbFace = Table.GetIcon(iconId);
	        if (tbFace == null)
	        {
	            return;
	        }
	        GameUtils.SetSpriteIcon(faceSprite, tbFace.Atlas, tbFace.Sprite);
	        _mNeedSetLine = false;
	        objTransform.localPosition = new Vector3(mPositionX, picPos, 0);
	        //  Logger.Info("FaceLabel:{0},{1}", objTransform.localPosition.x, objTransform.localPosition.y);
	        AddPositionX(faceWidth);
	    }
	
	    private void CreateTextLabel(NoticeData noticeNotice)
	    {
	        var value = noticeNotice.Content;
	        var obj = ComplexObjectPool.NewObjectSync("UI/RechargeActivity/RechargeActivityText.prefab");
	        if (obj == null)
	        {
	            return;
	        }
	        _mObj.Add(obj);
	        _mVarObjList.Add(new GameObjectType(obj, noticeNotice.Type));
	        var objTransform = obj.transform;
	        objTransform.SetParentEX(BackGround.transform);
	        obj.SetActive(true);
	        var label = obj.GetComponent<UILabel>();
	
	        var sbstr = "";
	        var text = "";
	
	
	        label.fontSize = FontSize;
	        NGUIText.fontSize = label.fontSize;
	        NGUIText.finalSize = label.fontSize;
	        if (mPositionX > MaxLineWidth)
	        {
	            ChangeLine();
	        }
	        NGUIText.dynamicFont = label.trueTypeFont;
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
	        objTransform.localScale = Vector3.one;
	        objTransform.localPosition = new Vector3(mPositionX, mPositionY, 0);
	        //Logger.Info("TextLabel:{0},{1},{2}", objTransform.localPosition.x, objTransform.localPosition.y,text);
	        label.text = text;
	        //positionX += label.width;
	        AddPositionX(label.width);
	        sbstr = sbstr.Remove(0, text.Length);
	        if (sbstr.Length > 0)
	        {
	            ChangeLine();
	            index = sbstr.IndexOf("\n");
	            sbstr = sbstr.Substring(index + "\n".Length, sbstr.Length - "\n".Length);
	            var notice = new NoticeData(eNoticeType.Text, -1, sbstr);
	            CreateTextLabel(notice);
	        }
	    }
	
	    public void Display()
	    {
	        var __list1 = mCharInfoNodes;
	        var __listCount1 = __list1.Count;
	        for (var __i1 = 0; __i1 < __listCount1; ++__i1)
	        {
	            var infoNode = __list1[__i1];
	            {
	                switch (infoNode.Type)
	                {
	                    case eNoticeType.Text:
	                        CreateTextLabel(infoNode);
	                        break;
	                    case eNoticeType.Icon:
	                        CreateFaceLabel(infoNode);
	                        break;
	                    default:
	                        break;
	                }
	            }
	        }
	    }
	
	    public int GetMaxLength()
	    {
	        return mTotalLength;
	    }
	
	    public class NoticeData
	    {
	        public NoticeData(eNoticeType type, int iconId, string content)
	        {
	            Type = type;
	            IconId = iconId;
	            Content = content;
	        }
	
	        public string Content;
	        public int IconId;
	        public eNoticeType Type;
	    }
	
	    public class GameObjectType
	    {
	        public GameObjectType(GameObject obj, eNoticeType type)
	        {
	            Type = type;
	            Obj = obj;
	        }
	
	        public GameObject Obj;
	        public eNoticeType Type;
	    }
	}
}