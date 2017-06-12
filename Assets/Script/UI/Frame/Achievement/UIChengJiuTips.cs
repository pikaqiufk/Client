using System;
#region using

using ClientDataModel;
using DataTable;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class UIChengJiuTips : MonoBehaviour
	{
	    public BindDataRoot BindRoot;
	    private float time;
	    public float ShowTime = 2.0f;
	
	    public void OnBtnClick()
	    {
	        var controllerBase = UIManager.Instance.GetController(UIConfig.AchievementTip);
	        var dataModel = controllerBase.GetDataModel("") as AchievementTipDataModel;
	
	        var tableAchievement = Table.GetAchievement(dataModel.Id);
	        if (null == tableAchievement)
	        {
	            return;
	        }
	
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.AchievementTip));
	
	        //show ui,到对应的页
	        EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.AchievementFrame));
	        EventDispatcher.Instance.DispatchEvent(new Event_ShowAchievementPage(tableAchievement.Type, 0));
	
	        //现已经改成打开未领取奖励的总是在最上面，下面的不要删，策划有可能会改回去
	
	        /*
			int MyLevel = PlayerDataManager.Instance.GetLevel();
			var achievementMgr = AchievementManager.Instance;
	
			float percent = 0.0f;
	
			int idx = -1;
			int count = 0;
			Table.ForeachAchievement((table) =>
			{
				if (tableAchievement.Type != table.Type)
					return true;
	
	            //没达到可视等级隐藏
				if (table.ViewLevel > 0 && MyLevel < table.ViewLevel)
				{
					if (!AchievementManager.Instance.IsAchievementAccomplished(table.Id))
	                {//并且也没完成
						return true;
					}
				}
	
	            //扩展数据可见性判断
				if (-1 != table.ClientDisplay)
				{
					if (0 == AchievementManager.Instance.FlagData.GetFlag(table.ClientDisplay))
					{
						return true;
					}
				}
	
				if (tableAchievement.Id == table.Id)
				{
					idx = count;
				}
				count++;
	
				return true;
			});
	
			EventDispatcher.Instance.DispatchEvent(new Event_ShowAchievementPage(tableAchievement.Type, idx));
			EventDispatcher.Instance.DispatchEvent(new Event_ScrollAchievement(idx));
			*/
	    }
	
	    public void OnCloseClick()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.AchievementTip));
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        BindRoot.RemoveBinding();
	
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
	        time = ShowTime;
	        var controllerBase = UIManager.Instance.GetController(UIConfig.AchievementTip);
	        BindRoot.SetBindDataSource(controllerBase.GetDataModel(""));
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void Start()
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
	
	    // Update is called once per frame
	    private void Update()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        if (null != LoadingLogic.Instance)
	        {
	            if (null != LoadingLogic.Instance.DestroyObj)
	            {
	                return;
	            }
	        }
	
	        time -= Time.deltaTime;
	        if (time <= 0)
	        {
	            EventDispatcher.Instance.DispatchEvent(new Event_NextAchievementTip());
	            time = ShowTime;
	            //OnCloseClick();
	        }
	
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