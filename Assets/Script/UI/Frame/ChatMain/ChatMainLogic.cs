using System;
#region using

using System.Collections.Generic;
using EventSystem;
using UnityEngine;

#endregion

public class ChatMainLogic : MonoBehaviour
{
    public BindDataRoot Binding;
    public List<UIButton> ChannelBtn;
    public GameObject ChannelList;
    public List<UIButton> ChannelSelect;
    //-----------------------------------------------City-------------

    public Transform CityChoose;
    public Transform CityList;
    public GameObject CusomBtn;
    public GameObject CusomList;
    public List<UIButton> CustomSetting;
    public UIInput MainInput;
    private bool mScrollBottom;
    private int mTrumpetCount;
    private string mTrumpetTipStr;
    public GameObject TrumpetFrame;
    public UIInput TrumpetInput;
    public UILabel TrumpetTip;

    public void CheckMainInput()
    {
        var e = new ChatWordCountChage(2);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void CheckTrumpetInput()
    {
        var e = new ChatWordCountChage(1);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void OnBtnChooseChannel()
    {
        var active = !ChannelList.activeSelf;
        ChannelList.SetActive(active);
        OnCloseCustomSetList();
    }

    public void OnBtnClose()
    {
        var e = new Close_UI_Event(UIConfig.ChatMainFrame);
        EventDispatcher.Instance.DispatchEvent(e);

        var e2 = new Close_UI_Event(UIConfig.OperationList);
        EventDispatcher.Instance.DispatchEvent(e2);
    }

    public void OnBtnCustomSetting()
    {
        var active = CusomList.activeSelf;
        active = !active;
        CusomList.SetActive(active);
        OnCloseChannelSetList();
    }

    public void OnBtnMessageFace()
    {
        var arg = new FaceListArguments();
        arg.Type = 2;
        var e = new Show_UI_Event(UIConfig.FaceList, arg);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void OnBtnMessageSend()
    {
        var e = new ChatMainSendBtnClick(1);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void OnBtnSetCustom()
    {
    }

    public void OnBtnTrumpet()
    {
    }

    public void OnBtnTrumpetBack()
    {
        //TrumpetFrame.SetActive(false);
        var e = new ChatTrumpetVisibleChange(false);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void OnBtnTrumpetFace()
    {
        var arg = new FaceListArguments();
        arg.Type = 2;
        var e = new Show_UI_Event(UIConfig.FaceList, arg);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void OnBtnTrumpetSend()
    {
        if (mTrumpetCount > 150)
        {
            //字数太长
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(270059));
            return;
        }
        EventDispatcher.Instance.DispatchEvent(new ChatMainSendBtnClick(2));
        //TrumpetFrame.SetActive(false);
    }

    public void OnBtnTrumpetShow()
    {
        var e = new ChatTrumpetVisibleChange(true);
        EventDispatcher.Instance.DispatchEvent(e);
        //TrumpetFrame.SetActive(true);
    }

    private void OnChatTrumpetWordCountCheck(IEvent ievent)
    {
        var e = ievent as ChatTrumpetWordCountCheck;

        RefreshTrumpetTip(e.Count);
    }

    public void OnClickAddFriend()
    {
        var e = new Show_UI_Event(UIConfig.FriendUI);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void OnClickBagContent()
    {
        var arg = new ChatItemListArguments();
        arg.Type = 1;
        var e = new Show_UI_Event(UIConfig.ChatItemList, arg);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void OnClickChannel(int index)
    {
        OnCloseChannelSetList();
        EventDispatcher.Instance.DispatchEvent(new ChatMainChangeChannel(index));
    }

    public void OnClickChannelSelect(int index)
    {
    }

    public void OnClickCityCancel()
    {
        var e = new ChatMainOperate(5);
        EventDispatcher.Instance.DispatchEvent(e);
        CityChoose.gameObject.SetActive(false);
        CityList.gameObject.SetActive(false);
    }

    public void OnClickCityConfirm()
    {
        var e = new ChatMainOperate(4);
        EventDispatcher.Instance.DispatchEvent(e);

        CityChoose.gameObject.SetActive(false);
        CityList.gameObject.SetActive(false);
    }

    public void OnClickCitySetting()
    {
        var e = new ChatMainOperate(1);
        EventDispatcher.Instance.DispatchEvent(e);
        CityChoose.gameObject.SetActive(true);
        CityList.gameObject.SetActive(false);
    }

    public void OnClickFaceCell(int index)
    {
        if (TrumpetFrame.activeSelf)
        {
            var e = new ChatMainFaceAdd(1, index);
            EventDispatcher.Instance.DispatchEvent(e);
        }
        else
        {
            var e = new ChatMainFaceAdd(0, index);
            EventDispatcher.Instance.DispatchEvent(e);
        }
    }

    public void OnClickFriendContent()
    {
    }

    public void OnClickMainCityChoose()
    {
        var e = new ChatMainOperate(2);
        EventDispatcher.Instance.DispatchEvent(e);

        CityList.gameObject.SetActive(true);
    }

    public void OnClickSubCityChoose()
    {
        var e = new ChatMainOperate(3);
        EventDispatcher.Instance.DispatchEvent(e);

        CityList.gameObject.SetActive(true);
    }

    public void OnClickSubCityClose()
    {
        CityList.gameObject.SetActive(false);
    }

    public void OnCloseChannelSetList()
    {
        if (ChannelList.activeSelf)
        {
            ChannelList.SetActive(false);
        }
    }

    public void OnCloseCustomSetList()
    {
        if (CusomList.activeSelf)
        {
            CusomList.SetActive(false);
        }
    }

    private void OnDestroy()
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

    private void OnDisable()
    {
#if !UNITY_EDITOR
        try
        {
#endif
        ChannelList.SetActive(false);
        Binding.RemoveBinding();
        var e = new ChatTrumpetVisibleChange(false);
        EventDispatcher.Instance.DispatchEvent(e);
        EventDispatcher.Instance.RemoveEventListener(ChatTrumpetWordCountCheck.EVENT_TYPE, OnChatTrumpetWordCountCheck);
#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    private void OnEnable()
    {
#if !UNITY_EDITOR
        try
        {
#endif
        CityChoose.gameObject.SetActive(false);
        CityList.gameObject.SetActive(false);

        var controllerBase = UIManager.Instance.GetController(UIConfig.ChatMainFrame);
        if (controllerBase == null)
        {
            return;
        }
        Binding.SetBindDataSource(controllerBase.GetDataModel(""));
        Binding.SetBindDataSource(UIManager.Instance.GetController(UIConfig.FriendUI).GetDataModel(""));
        EventDispatcher.Instance.AddEventListener(ChatTrumpetWordCountCheck.EVENT_TYPE, OnChatTrumpetWordCountCheck);

        ChannelList.SetActive(false);
#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    public void OnInputForcus()
    {
        var e = new ChatMainOperate(6);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void OnInputLaBaForcus()
    {
        var e = new ChatMainOperate(7);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void OnInputLabaLostForcus()
    {
        if (TrumpetInput.label.text == string.Empty)
        {
            TrumpetInput.label.text = GameUtils.GetDictionaryText(100001058);
        }
    }

    public void OnInputLostForcus()
    {
        if (MainInput.label.text == string.Empty)
        {
            MainInput.label.text = GameUtils.GetDictionaryText(100001058);
        }
    }

    private void RefreshTrumpetTip(int count)
    {
        mTrumpetCount = count;
        count = GameUtils.HornWorldCount - count;
        if (count >= 0)
        {
            //一共可输入{0}个字，还可以输入{1}个字
            mTrumpetTipStr = string.Format(GameUtils.GetDictionaryText(270057), GameUtils.HornWorldCount, count);
            TrumpetTip.text = mTrumpetTipStr;
        }
        else
        {
            //一共可输入{0}个字，已经超过[FF0000]{1}[-]个字
            mTrumpetTipStr = string.Format(GameUtils.GetDictionaryText(270058), GameUtils.HornWorldCount,
                Mathf.Abs(count));
            TrumpetTip.text = mTrumpetTipStr;
        }
    }

    private void Start()
    {
#if !UNITY_EDITOR
        try
        {
#endif
        CheckTrumpetInput();

        var index = 0;
        var channelListtransformchildCount0 = ChannelBtn.Count;
        for (var i = 0; i < channelListtransformchildCount0; i++)
        {
            var o = ChannelBtn[i];
            var j = i + 1;
            var deleget = new EventDelegate(() => { OnClickChannel(j); });
            o.onClick.Add(deleget);
        }
        var c = ChannelSelect.Count;
        for (var i = 0; i < c; i++)
        {
            var btn = ChannelSelect[i];
            if (btn == null)
            {
                continue;
            }
            var j = i;
            btn.onClick.Add(new EventDelegate(() => { OnClickChannelSelect(j); }));
        }
        TrumpetInput.onChange.Add(new EventDelegate(CheckTrumpetInput));

        MainInput.onHide.Add(new EventDelegate(CheckMainInput));
        //MainInput.onChange.Add(new EventDelegate(CheckMainInput));

        mTrumpetTipStr = string.Format(GameUtils.GetDictionaryText(270057), GameUtils.HornWorldCount,
            GameUtils.HornWorldCount);
        TrumpetTip.text = mTrumpetTipStr;
#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }
}