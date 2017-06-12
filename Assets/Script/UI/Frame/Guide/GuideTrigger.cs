
#region using

using System;
using System.Collections;
using System.Collections.Generic;
using DataTable;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class GuideTrigger : MonoBehaviour
	{
	    private static bool s_isInited;
	    private static readonly Dictionary<int, List<GuidanceRecord>> s_guidTblDict =
	        new Dictionary<int, List<GuidanceRecord>>();
	    private static readonly List<GuidanceRecord> s_newFunctionList = new List<GuidanceRecord>();
	    private UIConfig curGuidConfig;
	    private int curGuidStep = -1;
	#if UNITY_EDITOR
	    private int missionId = -1;
	#endif
	
	    public List<GuidanceRecord> NewFunction
	    {
	        get { return s_newFunctionList; }
	    }
	
	    private void Awake()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	
	        if (!s_isInited)
	        {
	//初始化表
	            s_guidTblDict.Clear();
	            Table.ForeachGuidance(table =>
	            {
		            if (-1 == table.GuideRule)
		            {
			            return true;
		            }

					if (!string.IsNullOrEmpty(table.Name) || 0!=table.GuideRule)
	                {
	                    s_newFunctionList.Add(table);
	                }
	                else if (-1 != table.UIID)
	                {
	                    List<GuidanceRecord> list = null;
	                    if (!s_guidTblDict.TryGetValue(table.UIID, out list))
	                    {
	                        list = new List<GuidanceRecord>();
	                        s_guidTblDict.Add(table.UIID, list);
	                    }
	                    list.Add(table);
	
	                    return true;
	                }
	                return true;
	            });
	            s_isInited = true;
	        }
	
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    private bool CanTrigger(int id)
	    {
	        if (null == GameLogic.Instance)
	        {
	            return false;
	        }
	
	        if (null == GameLogic.Instance.Scene)
	        {
	            return false;
	        }
	
	        var sceneId = GameLogic.Instance.Scene.SceneTypeId;
	        if (id == 50)
	        {
	            if (sceneId == 100)
	            {
	                return false;
	            }
	        }
	        else if (id == 190)
	        {
	            if (sceneId == 5000)
	            {
	                return false;
	            }
	        }
	
	        return true;
	    }
	
	    public bool CheckCondtion(GuidanceRecord table)
	    {
	        if (table.Level > 0)
	        {
	            if (PlayerDataManager.Instance.GetLevel() < table.Level)
	            {
	                return false;
	            }
	        }
	        if (-1 != table.TaskID)
	        {
	            var data = MissionManager.Instance.GetMissionById(table.TaskID);
	            if (null == data)
	            {
	                return false;
	            }
	            if ((eMissionState) table.State != MissionManager.Instance.GetMissionState(table.TaskID))
	            {
	                return false;
	            }
	        }
	
	        if (table.Career > 0)
	        {
	            if ((table.Career & (1 << PlayerDataManager.Instance.GetRoleId())) <= 0)
	            {
	                return false;
	            }
	        }
	
	        if (-1 != table.FlagPrepose)
	        {
	            if (!PlayerDataManager.Instance.GetFlag(table.FlagPrepose))
	            {
	                return false;
	            }
	        }
	
	        if (-1 != table.FlagPreposeFalse)
	        {
	            if (PlayerDataManager.Instance.GetFlag(table.FlagPreposeFalse))
	            {
	                return false;
	            }
	        }
	
	        return true;
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	
	        EventDispatcher.Instance.RemoveEventListener(UiFrameShowEvent.EVENT_TYPE, OnUIShow);
	        EventDispatcher.Instance.RemoveEventListener(UiFrameCloseEvent.EVENT_TYPE, OnUIClose);
	        EventDispatcher.Instance.RemoveEventListener(UIEvent_WishingOperation.EVENT_TYPE, OnOpenWishPool);
	
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    //当接受任务时
	    public bool OnMissionAccept(int missionId)
	    {
	        var state = MissionManager.Instance.GetMissionState(missionId);
	        return OnMissionChange(missionId, state);
	    }
	
	    //当任务可接受时
	    public bool OnMissionBecomeAcceptable(int missionId)
	    {
	        return OnMissionChange(missionId, eMissionState.Acceptable);
	    }
	
	    //当任务改变时，注意都是瞬间动作
	    public bool OnMissionChange(int missionId, eMissionState state)
	    {
	        if (!GameSetting.Instance.EnableNewFunctionTip)
	        {
	            return false;
	        }

            EventDispatcher.Instance.DispatchEvent(new Event_RefreshFuctionOnState());

            var __list2 = s_newFunctionList;
	        var __listCount2 = __list2.Count;
	        for (var __i2 = 0; __i2 < __listCount2; ++__i2)
	        {
	            var table = __list2[__i2];
	            {
	                //判断任务状态，必须是未完成状态
	                var temp = (eMissionState) table.State;
	                if (missionId != table.TaskID || temp != state)
	                {
	                    continue;
	                }
	
	                //判断等级
	                if (table.Level > 0)
	                {
	                    if (PlayerDataManager.Instance.GetLevel() < table.Level)
	                    {
	                        continue;
	                    }
	                }
	
	                //判断职业
	                if (table.Career > 0)
	                {
	                    if ((table.Career & (1 << PlayerDataManager.Instance.GetRoleId())) <= 0)
	                    {
	                        continue;
	                    }
	                }
	
	                if (ObjManager.Instance.MyPlayer.Dead)
	                {
	                    return false;
	                }
		            

		            if (2 == table.GuideRule)
		            {
			            if (-1 != table.UIID)
			            {
				            GameUtils.GotoUiTab(table.UIID,0);
			            }
		            }
		            else
		            {
						if (-1 == table.Flag)
						{
							return false;
						}
						UIManager.Instance.OpenDefaultFrame();
						EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.MessageBox));

			            //开启该新功能
						EventDispatcher.Instance.DispatchEvent(new UIEvent_PlayMainUIBtnAnimEvent(table.Name, () =>
						{
							//StartCoroutine(StartGuideCoroutine(table.Flag));
							UIManager.Instance.ShowUI(UIConfig.MainUI);
							if (0 != table.Flag)
							{//硬编码，0表示不引导
								GuideManager.Instance.StartGuide(table.Flag);
							}
							//StartGuide(table.Flag);
						}));
		            }
	                
	
	                //屏蔽输入
	                //EventDispatcher.Instance.DispatchEvent(new UI_BlockMainUIInputEvent(2));
	
	                //延迟开启引导
	                //StartCoroutine(StartGuideCoroutine(table.Flag));
	                return true;
	            }
	        }
	        return false;
	    }
	
	    //当任务完成时
	    public bool OnMissionComplete(int missionId)
	    {
	        return OnMissionChange(missionId, eMissionState.Finished);
	    }
	
	    private void OnOpenWishPool(IEvent ievent)
	    {
	        var e = ievent as UIEvent_WishingOperation;
	        if (null == e || 6 != e.Type)
	        {
	            return;
	        }
	        var table = Table.GetGuidance(224);
	        if (CheckCondtion(table))
	        {
	            GuideManager.Instance.StartGuide(table.Flag);
	        }
            EventDispatcher.Instance.DispatchEvent(new Event_RefreshFuctionOnState());
        }
	
	    //监听UI关闭事件，如果引导过程中，ui被其他事件关闭了，引导也要停止
	    private void OnUIClose(IEvent ievent)
	    {
	        if (!GameSetting.Instance.EnableGuide)
	        {
	            return;
	        }
	
	        var e = ievent as UiFrameCloseEvent;
	        if (null == e)
	        {
	            return;
	        }
	
	        var config = e.Config;
	        if (null == config || null == curGuidConfig)
	        {
	            return;
	        }
	
	        if (-1 == curGuidStep)
	        {
	            return;
	        }
	
	        if (config.Equals(curGuidConfig))
	        {
	            if (GuideManager.Instance.IsGuiding())
	            {
	                if (curGuidStep == GuideManager.Instance.GetCurrentGuidingStepId())
	                {
	                    //GuideManager.Instance.StopGuiding();
	                    curGuidStep = -1;
	                }
	            }
	            curGuidConfig = null;
	        }
	    }
	
	    //监听UI打开事件，根据条件触发引导
	    private void OnUIShow(IEvent ievent)
	    {
	        if (!GameSetting.Instance.EnableGuide)
	        {
	            return;
	        }
	
	        var e = ievent as UiFrameShowEvent;
	        if (null == e.Config)
	        {
	            return;
	        }
	
	        if (null == e.Config.UiRecord)
	        {
	            return;
	        }

            EventDispatcher.Instance.DispatchEvent(new Event_RefreshFuctionOnState());
            var id = e.Config.UiRecord.Id; //表里的id
	
	        List<GuidanceRecord> list = null;
	        if (!s_guidTblDict.TryGetValue(id, out list))
	        {
	            return;
	        }
	        {
	            var __list3 = list;
	            var __listCount3 = __list3.Count;
	            for (var __i3 = 0; __i3 < __listCount3; ++__i3)
	            {
	                var table = __list3[__i3];
	                {
	                    if (CheckCondtion(table))
	                    {
	                        curGuidConfig = e.Config;
	                        curGuidStep = table.Flag;
	                        if (CanTrigger(table.Flag))
	                        {
	                            if (224 == table.Id)
	                            {
	//许愿池
	                                return;
	                            }
	                            GuideManager.Instance.StartGuide(table.Flag); //表改了 table.Flag就是开启哪个引导    
	                        }
	                        else
	                        {
	                            Logger.Debug("!CanTrigger ({0})", table.Flag);
	                        }
	                        return;
	                    }
	                }
	            }
	        }
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	
	        EventDispatcher.Instance.AddEventListener(UiFrameShowEvent.EVENT_TYPE, OnUIShow);
	        EventDispatcher.Instance.AddEventListener(UiFrameCloseEvent.EVENT_TYPE, OnUIClose);
	        EventDispatcher.Instance.AddEventListener(UIEvent_WishingOperation.EVENT_TYPE, OnOpenWishPool);
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    private void StartGuide(int guideId)
	    {
	        if (-1 == guideId)
	        {
	            return;
	        }
	        GuideManager.Instance.StartGuide(guideId);
	    }
	
	    private IEnumerator StartGuideCoroutine(int guideId)
	    {
	        using (new BlockingLayerHelper(1))
	        {
	            yield return new WaitForSeconds(1f);
	        }
	        StartGuide(guideId);
	        yield return new WaitForSeconds(0.5f);
	        //开启输入
	        EventDispatcher.Instance.DispatchEvent(new UI_BlockMainUIInputEvent(0));
	    }

		public int Id = 1014;
		[ContextMenu("Trigger Test")]
		public void TriggerNewFunction()
		{
			var tb = Table.GetGuidance(Id);
			if (null == tb || string.IsNullOrEmpty(tb.Name))
			{
				Debug.Log("null == tb || string.IsNullOrEmpty(tb.Name)");
				return;
			}
			EventDispatcher.Instance.DispatchEvent(new UIEvent_PlayMainUIBtnAnimEvent(tb.Name, () =>
			{
				//StartCoroutine(StartGuideCoroutine(tb.Flag));
				GuideManager.Instance.StartGuide(tb.Flag);
				//StartGuide(table.Flag);
			}));
		}
	}
}