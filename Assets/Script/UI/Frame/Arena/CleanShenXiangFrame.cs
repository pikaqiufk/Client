using System;
#region using

using System.Collections;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class CleanShenXiangFrame : MonoBehaviour
	{
	    public BindDataRoot Binding;
	    public CleanMianUIFrame CleanMask;
	    public TimerLogic CleanPlay;
	    public UISliderNormal CleanSlider;
	    public UILabel CleanSliderTime;
	    public GameObject CleanStart;
	    public TimerLogic CleanWarn;
	    public UIFlowTexture EffecTexture;
	
	    public void OnCloseClean()
	    {
	        if (CleanMask.IsStart && !CleanMask.IsFinish)
	        {
	            var e = new SatueOperateEvent(12);
	            EventDispatcher.Instance.DispatchEvent(e);
	        }
	        var e1 = new Close_UI_Event(UIConfig.CleanDust);
	        EventDispatcher.Instance.DispatchEvent(e1);
	        OnResetClean();
	    }
	
	    public void OnResetClean()
	    {
	        EffecTexture.SetFlow(false);
	        CleanMask.Reset();
	    }
	
	    public void OnClickCleanClose()
	    {
	//设置结束
	        if (CleanMask.IsStart)
	        {
	            CleanPlay.TargetTime = Game.Instance.ServerTime;
	        }
	        else
	        {
	            var e1 = new Close_UI_Event(UIConfig.CleanDust);
	            EventDispatcher.Instance.DispatchEvent(e1);
	        }
	    }
	
	    public void OnClickCleanStart()
	    {
	        CleanWarn.TargetTime = Game.Instance.ServerTime.AddSeconds(3);
	        CleanWarn.gameObject.SetActive(true);
	        CleanStart.SetActive(false);
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (CleanMask != null)
	        {
	            if (CleanMask.OnCleanFinish != null)
	            {
	                CleanMask.OnCleanFinish -= OnCleanFinish;
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
	
	        var controllerBase = UIManager.Instance.GetController(UIConfig.CleanDust);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        Binding.SetBindDataSource(controllerBase.GetDataModel(""));
	
	        OnCleanShow();
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnCleanFinish()
	    {
	        //挑战成功
	        EffecTexture.SetFlow(true);
	
	        var e1 = new ShowUIHintBoard(270004);
	        EventDispatcher.Instance.DispatchEvent(e1);
	        NetManager.Instance.StartCoroutine(ShowCleanFinish());
	    }
	
	    public void OnFormatCleanPlay(UILabel lable)
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
	            var dif = taget - Game.Instance.ServerTime;
	            lable.text = string.Format("{0}", (int) dif.TotalSeconds);
	            CleanSlider.value = (float) dif.TotalSeconds/30.0f;
	        }
	        else
	        {
	            if (!CleanMask.IsFinish)
	            {
	                OnCloseClean();
	            }
	        }
	    }
	
	    public void OnFormatCleanWarn(UILabel lable)
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
	            var secend = (int) (taget - Game.Instance.ServerTime).TotalSeconds + 1;
	            lable.text = string.Format("{0}", secend);
	        }
	        else
	        {
	            lable.gameObject.SetActive(false);
	            CleanMask.Restart();
	            CleanPlay.TargetTime = Game.Instance.ServerTime.AddSeconds(30);
	        }
	    }
	
	    public void OnCleanShow()
	    {
	        OnResetClean();
	        CleanStart.SetActive(true);
	        CleanWarn.gameObject.SetActive(false);
	        CleanSlider.value = 1.0f;
	        CleanSliderTime.text = "30";
	    }
	
	    public IEnumerator ShowCleanFinish()
	    {
	        yield return new WaitForSeconds(1.5f);
	        CleanPlay.TargetTime = Game.Instance.ServerTime;
	        var e = new SatueOperateEvent(1);
	        EventDispatcher.Instance.DispatchEvent(e);
	        OnCloseClean();
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        CleanMask.OnCleanFinish += OnCleanFinish;
	
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