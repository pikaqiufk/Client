using System;
#region using

using EventSystem;
using SignalChain;
using UnityEngine;

#endregion

namespace GameUI
{
	public class MainScreenChatFrame : MonoBehaviour, IChainRoot, IChainListener
	{
	    public GameObject ActiveTip;
	    public UIWidget ChatContent;
	    public StackLayout ChatMessages;
	    private Transform chatMessageTrans;
	    private bool isEnable = true;
	    private Transform messageTrans;
	    private int maxLength;
        //once time
	    private float hornTime;
        private float onceTime;
        private bool Playing = false;
	    public GameObject TrumpetBg;
	    public UIPanel TrumpetIPanel;
	    public ChatMessageLogic TrumpetMsg;
	
	    private void Awake()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        messageTrans = TrumpetMsg.transform;
	        chatMessageTrans = ChatMessages.transform;
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void SetHornPostion()
	    {
	        return;
	        if (ActiveTip.activeSelf && TrumpetBg.activeSelf)
	        {
	            ActiveTip.transform.localPosition = new Vector3(0, 70, 0);
	
	            TrumpetBg.transform.localPosition = new Vector3(0, 30, 0);
	        }
	        else if (!ActiveTip.activeSelf && TrumpetBg.activeSelf)
	        {
	            TrumpetBg.transform.localPosition = new Vector3(0, 30, 0);
	        }
	        else if (ActiveTip.activeSelf && !TrumpetBg.activeSelf)
	        {
	            ActiveTip.transform.localPosition = new Vector3(0, 30, 0);
	        }
	    }
	
	    private void LateUpdate()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        if (isEnable)
	        {
	            isEnable = false;
	            ChatMessages.height = 0;
	            ChatMessages.ResetLayout();
	            var pos = chatMessageTrans.localPosition;
	            var h = ChatMessages.height - 80;
	            chatMessageTrans.localPosition = new Vector3(pos.x, h, pos.z);
	
	            SetHornPostion();
	        }
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnClickChatNode()
	    {
	        var e = new Show_UI_Event(UIConfig.ChatMainFrame);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }

        public void OnClickGoToActivity()
        {
            var e = new OnCLickGoToActivityByMainUIEvent();
	        EventDispatcher.Instance.DispatchEvent(e);
        }

	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        EventDispatcher.Instance.RemoveEventListener(ChatMainNewTrumpet.EVENT_TYPE, OnNewHorn);
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void OnNewHorn(IEvent ievent)
	    {
	        var e = ievent as ChatMainNewTrumpet;
	        hornTime = GameUtils.TrumpetDurationTime;
            onceTime = 0;
            TrumpetBg.SetActive(true);
	        messageTrans.localPosition = Vector3.zero;
            Playing = false;

            SetHornPostion();
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        TrumpetBg.SetActive(false);
	        EventDispatcher.Instance.AddEventListener(ChatMainNewTrumpet.EVENT_TYPE, OnNewHorn);
	        maxLength = (int) TrumpetIPanel.baseClipRegion.z;
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
            //初始
            if (hornTime > 0 && !Playing)
            {
                Playing = true;
                TrumpetBg.SetActive(true);
               
            }
            //进行中
            else if (Playing)
            {
                //至少播放一次
                var max = TrumpetMsg.GetMaxLength();
                var loc = messageTrans.localPosition;

                hornTime -= Time.deltaTime;
                onceTime += Time.deltaTime;

                loc.x -= Time.deltaTime * GameUtils.TrumpeMoveSpeedt;
                //单次结束
                if (loc.x < -max)
                {
                    if (hornTime <= 0 || hornTime < onceTime)
                    {
                        Playing = false;
                        hornTime = -1f;
                    }
                    else
                    {
                        //再走一轮
                        loc.x = maxLength;
                        messageTrans.localPosition = loc;
                        onceTime = 0;
                    }

                }
                else
                {
                    //滚动
                    messageTrans.localPosition = loc;
                }
            }
            //结束
            else if (hornTime < 0 && !Playing)
            {
                hornTime = 0;
                TrumpetBg.SetActive(false);
            }

	        //if (hornTime > 0.0f)
	        //{
	        //    hornTime -= Time.deltaTime;
         //       onceTime += Time.deltaTime;
         //       if (hornTime <= 0.0f)
	        //    {
	        //        hornTime = 0.0f;
         //           onceTime = 0;
         //           TrumpetBg.SetActive(false);
	
	        //        SetHornPostion();
	        //    }
	        //    var max = TrumpetMsg.GetMaxLength();
	        // //   if (max > maxLength)
	        //    {
	        //        var loc = messageTrans.localPosition;
	        //        loc.x -= Time.deltaTime*GameUtils.TrumpeMoveSpeedt;
	        //        if (loc.x < -max)
	        //        {
	        //            loc.x = maxLength;
         //               //If the remaining time is less than the time required to run once, give up
         //               if (hornTime < onceTime)
         //               {
         //                  /// if (hornTime <= 0)
         //                   {
         //                       onceTime = 0;
         //                   }
                        
                          
         //                   TrumpetBg.SetActive(false);

         //                   SetHornPostion();
         //                   //return;
         //               }
         //               else
         //               {
         //                   onceTime = 0;
         //               }
	        //        }
	        //        messageTrans.localPosition = loc;
	        //    }
	        //}
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void Listen<T>(T message)
	    {
	        if (message is string && (message as string) == "ActiveChanged")
	        {
	            isEnable = true;
	        }
	    }
	}
}