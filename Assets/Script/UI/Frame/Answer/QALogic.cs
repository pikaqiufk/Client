using System;
#region using

using System.Collections.Generic;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class QALogic : MonoBehaviour
	{
	    public List<UIButton> AnswerBtns;
	    public BindDataRoot Binding;
	    public List<UIEventTrigger> Icon15;
	    public List<UIEventTrigger> Icon8;
	    public List<UIButton> TipsBtns;
	
	    public void QAButtonClick(int index)
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_Answer_AnswerClick(0, index));
	    }
	
	    public void CloseUI()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.AnswerUI));
	    }
	
	    public void IconClick15(int index)
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_Answer_AnswerClick(3, index));
	    }
	
	    public void IconClick8(int index)
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_Answer_AnswerClick(2, index));
	    }
	
	    public void OnFormatTimer(UILabel lable)
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
	
	        var timeSpan = timer.TargetTime - Game.Instance.ServerTime;
	        lable.text = string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        //todo
	        var controllerBase = UIManager.Instance.GetController(UIConfig.AnswerUI);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        Binding.SetBindDataSource(controllerBase.GetDataModel(""));
	
	        for (var i = 0; i < AnswerBtns.Count; i++)
	        {
	            var j = i;
	            var deleget = new EventDelegate(() => { QAButtonClick(j); });
	
	            AnswerBtns[i].onClick.Add(deleget);
	        }
	        for (var i = 0; i < TipsBtns.Count; i++)
	        {
	            var j = i;
	            var deleget = new EventDelegate(() => { TiShiClick(j); });
	
	            TipsBtns[i].onClick.Add(deleget);
	        }
	        for (var i = 0; i < Icon8.Count; i++)
	        {
	            var j = i;
	            var deleget = new EventDelegate(() => { IconClick8(j); });
	
	            Icon8[i].onClick.Add(deleget);
	        }
	        for (var i = 0; i < Icon15.Count; i++)
	        {
	            var j = i;
	            var deleget = new EventDelegate(() => { IconClick15(j); });
	
	            Icon15[i].onClick.Add(deleget);
	        }
	
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void TiShiClick(int index)
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_Answer_AnswerClick(1, index));
	    }
	}
}