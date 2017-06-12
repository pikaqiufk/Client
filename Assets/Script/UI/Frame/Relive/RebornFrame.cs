using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class RebornFrame : MonoBehaviour
	{
	    public BindDataRoot Binding;
	    public TimerLogic FreeTimer;
	
	    public void OnClickBtnBook()
	    {
	        var e = new Show_UI_Event(UIConfig.HandBook,
	            new HandBookArguments());
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickBtnEquip()
	    {
	        var e = new Show_UI_Event(UIConfig.EquipUI);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickBtnPet()
	    {
	        var e = new Show_UI_Event(UIConfig.ElfUI);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickBtnReliveStone()
	    {
	        var e = new RelieveOperateEvent(0);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickBtnRelveDiamond()
	    {
	        var e = new RelieveOperateEvent(1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickBtnRelveFree()
	    {
	        var e = new RelieveOperateEvent(2);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickBtnSkill()
	    {
	        var e = new Show_UI_Event(UIConfig.SkillFrameUI);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickBtnWing()
	    {
	        if (PlayerDataManager.Instance.GetWingId() == -1)
	        {
	            return;
	        }
	        var e = new Show_UI_Event(UIConfig.WingUI);
	        EventDispatcher.Instance.DispatchEvent(e);
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
	
	        Binding.RemoveBinding();
	
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
	
	        var controllerBase = UIManager.Instance.GetController(UIConfig.ReliveUI);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        Binding.SetBindDataSource(controllerBase.GetDataModel(""));
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnFormatRelive(UILabel lable)
	    {
	        if (lable == null)
	        {
	            return;
	        }
	        var timer = lable.GetComponent<TimerLogic>();
	        if (timer == null)
	        {
	            return;
	        }
	        var taget = timer.TargetTime;
	        if (taget > Game.Instance.ServerTime)
	        {
	            lable.text = string.Format("{0}", (int) (taget - Game.Instance.ServerTime).TotalSeconds);
	        }
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
	}
}